#!/bin/bash

# Configuration
PORT=7081
CONTAINER_NAME="chat-tunnel"
IMAGE_NAME="chat-tunnel-image"
TOKEN_DIR="$HOME/.devtunnels"
REACT_ENV="./Frontend-Service/.env"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "Error: Docker is not running. Please start Docker Desktop."
    exit 1
fi

# Build the local tunnel image if it doesn't exist
if [[ "$(docker images -q $IMAGE_NAME 2> /dev/null)" == "" ]]; then
    echo "Building local tunnel image (one-time setup)..."
    ARCH=$(uname -m)
    if [ "$ARCH" = "aarch64" ] || [ "$ARCH" = "arm64" ]; then 
        DL_URL='https://aka.ms/TunnelsCliDownload/linux-arm64'
    else 
        DL_URL='https://aka.ms/TunnelsCliDownload/linux-x64'
    fi

    cat <<EOF > Tunnel.Dockerfile
FROM ubuntu:latest
RUN apt-get update && apt-get install -y curl ca-certificates libicu-dev socat
RUN export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
RUN curl -sL '$DL_URL' -o /usr/local/bin/devtunnel && chmod +x /usr/local/bin/devtunnel
EOF
    docker build -t $IMAGE_NAME -f Tunnel.Dockerfile .
    rm Tunnel.Dockerfile
fi

# Parse arguments
CREATE_NAME=""
TUNNEL_NAME=""
for arg in "$@"; do
    case $arg in
        --create=*)
            CREATE_NAME="${arg#*=}"
            ;;
        --name=*)
            TUNNEL_NAME="${arg#*=}"
            ;;
    esac
done

# Determine tunnel ID to use
TUNNEL_ID=""
DELETE_ON_EXIT=false

if [ -n "$TUNNEL_NAME" ]; then
    echo "Looking up tunnel with description '$TUNNEL_NAME'..."
    LIST_OUTPUT=$(docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel list)
    TUNNEL_ID=$(echo "$LIST_OUTPUT" | tail -n +2 | grep -i "$TUNNEL_NAME" | awk '{print $1}' | head -n 1)
    if [ -z "$TUNNEL_ID" ]; then
        echo "Error: No tunnel found with description '$TUNNEL_NAME'. Exiting."
        exit 1
    fi
    echo "Found tunnel (ID: $TUNNEL_ID) matching description '$TUNNEL_NAME'"
elif [ -n "$CREATE_NAME" ]; then
    echo "Checking if tunnel with description '$CREATE_NAME' already exists..."
    LIST_OUTPUT=$(docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel list)
    EXISTING_ID=$(echo "$LIST_OUTPUT" | tail -n +2 | grep -i "$CREATE_NAME" | awk '{print $1}' | head -n 1)
    if [ -n "$EXISTING_ID" ]; then
        echo "Error: Tunnel with description '$CREATE_NAME' already exists (ID: $EXISTING_ID). Exiting."
        exit 1
    fi
    echo "Creating a new persistent tunnel with description '$CREATE_NAME'..."
    TUNNEL_ID=$(docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel create -d "$CREATE_NAME" --allow-anonymous | awk '/^Tunnel ID/ {print $4}')
    if [ -n "$TUNNEL_ID" ]; then
        echo "Created new tunnel (ID: $TUNNEL_ID)"
        docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel port create -p $PORT --protocol https $TUNNEL_ID > /dev/null
    else
        echo "Error: Failed to create tunnel."
        exit 1
    fi
else
    echo "Creating a new temporary tunnel..."
    DELETE_ON_EXIT=true
    TUNNEL_ID=$(docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel create --allow-anonymous | awk '/^Tunnel ID/ {print $4}')
    if [ -n "$TUNNEL_ID" ]; then
        echo "Created temporary tunnel (ID: $TUNNEL_ID)"
        docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel port create -p $PORT --protocol https $TUNNEL_ID > /dev/null
    else
        echo "Error: Failed to create temporary tunnel."
        exit 1
    fi
fi

# Cleanup old containers
docker rm -f $CONTAINER_NAME > /dev/null 2>&1

echo "Starting Dev Tunnel via local Docker image..."

# Start the tunnel
docker run -d --name $CONTAINER_NAME \
  -v "$TOKEN_DIR:/DevTunnels" \
  -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 \
  $IMAGE_NAME bash -c "socat TCP-LISTEN:$PORT,fork TCP:host.docker.internal:$PORT & /usr/local/bin/devtunnel host $TUNNEL_ID --allow-anonymous" > /dev/null

echo "Waiting for tunnel to initialize..."

# Poll logs for the URL
MAX_RETRIES=30
RETRY_COUNT=0
TUNNEL_URL=""

while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
    TUNNEL_URL=$(docker logs $CONTAINER_NAME 2>&1 | grep -oE "https://[a-zA-Z0-9.-]+\.devtunnels\.ms" | head -n 1)
    if [ -n "$TUNNEL_URL" ]; then
        break
    fi
    
    if [ "$(docker inspect -f '{{.State.Running}}' $CONTAINER_NAME 2>/dev/null)" = "false" ]; then
        echo ""
        echo "Error: Tunnel container stopped unexpectedly."
        docker logs $CONTAINER_NAME
        exit 1
    fi
    
    sleep 1
    RETRY_COUNT=$((RETRY_COUNT + 1))
    printf "."
done

echo ""

if [ -z "$TUNNEL_URL" ]; then
    echo "Error: Could not detect Tunnel URL. Check logs with: docker logs $CONTAINER_NAME"
    docker stop $CONTAINER_NAME > /dev/null
    exit 1
fi

echo "---------------------------------------------------"
echo "TUNNEL ACTIVE: $TUNNEL_URL"
echo "Client should use this URL."
echo "---------------------------------------------------"

## Update React .env file
#echo "Updating React configuration..."
## Check if key exists; if so, update it. If not, append it.
#if grep -q "VITE_TUNNEL_URL=" "$REACT_ENV"; then
#    sed -i "" "s|^VITE_TUNNEL_URL=.*|VITE_TUNNEL_URL=$TUNNEL_URL|" "$REACT_ENV"
#else
#    echo "VITE_TUNNEL_URL=$TUNNEL_URL" >> "$REACT_ENV"
#fi

# Export the URL for the backend server
export VS_TUNNEL_URL=$TUNNEL_URL

cleanup() {
    echo ""
    echo "Stopping tunnel and server..."
    docker stop $CONTAINER_NAME > /dev/null
    if [ "$DELETE_ON_EXIT" = true ] && [ -n "$TUNNEL_ID" ]; then
        echo "Deleting temporary tunnel (ID: $TUNNEL_ID)..."
        docker run --rm -v "$TOKEN_DIR:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 $IMAGE_NAME /usr/local/bin/devtunnel delete -f "$TUNNEL_ID" > /dev/null
    fi
    exit
}
trap cleanup SIGINT SIGTERM

echo "Starting Backend Server..."
cd Backend-Service/Backend.API
dotnet run --launch-profile https

wait
