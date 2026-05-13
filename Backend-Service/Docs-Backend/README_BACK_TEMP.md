## Building the Project

if you are setting this up for the first time or after moving files:

```
dotnet restore
```

```
dotnet clean
```

* **Entity Framework Core Tools:**
  ```
  dotnet tool install --global dotnet-ef
  ```

* **Database Migration:**
  
  If no table model currently exists in postgres...

  in...

  `\Backend-Service\Backend.API` :

  ```
  dotnet ef database update
  ```

```
dotnet build
```

### Docker:
1. Navigate to the root folder `Networked_Chat_App/` where ```docker-compose.yml``` is located

- (optional) Remove all tables:
  ```
  docker-compose down -v
  ```

3. Spin up the PostgresSQL container:
	```
	docker-compose up -d --remove-orphans
	```
4. The database will be available at ```localhost:5432``` with 
* **Development Secrets**:the connection string and local credentials are stored in `appsettings.Development.json`
* **Note**: this file is tipically excluded from source control in professional settings to protect credentials

### To start the API:

```
dotnet run
```

## devtunnel (optional - mac)

### Command: devtunnel login
```
docker run --rm -it -v ~/.devtunnels:/DevTunnels ubuntu:latest bash -c "
  apt-get update && apt-get install -y curl ca-certificates libicu-dev && 
  export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 &&
  ARCH=\$(uname -m) && 
  if [ \"\$ARCH\" = \"aarch64\" ] || [ \"\$ARCH\" = \"arm64\" ]; then DL_URL='https://aka.ms/TunnelsCliDownload/linux-arm64'; else DL_URL='https://aka.ms/TunnelsCliDownload/linux-x64'; fi &&
  curl -sL \$DL_URL -o /usr/local/bin/devtunnel && chmod +x /usr/local/bin/devtunnel && 
  devtunnel login
"
```

example output (tail-end):
```
Unable to open a web page using xdg-open, gnome-open, kfmclient or wslview tools. See inner exception for details. Possible causes for this error are: tools are not installed or they cannot open a URL. Make sure you can open a web page by invoking from a terminal: xdg-open https://www.bing.com 
Falling back to device code authentication.
To sign in, use a web browser to open the page https://login.microsoft.com/device and enter the code EVXGFGMUZ to authenticate.</code>
```

The above login command is an effective way to begin accessing devtunnel cli tools using a temporary docker container, without installing the tools on your host mac.

### Command: devtunnel list
```
docker run --rm -v "$HOME/.devtunnels:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 matrix-tunnel-image /usr/local/bin/devtunnel list
```

example output (removed some columns):
```
Tunnel ID                           Labels                    Description              
interesting-chair-zmz93hd.usw3                                                         
kind-lake-z59lj74.usw3                                                                 
fancy-hill-jj1qwr4.usw3                                                                
puzzled-field-h34lflk.usw3          VisualStudioCreatedT...   NetworkedChatApp
```

### Start dev tunnel and server:

Navigate to the root folder `Networked_Chat_App/` where ```start-tunnel.sh``` is located,... 

enter (To create a tunnel that will be deleted from your account when you are finished):

```
./start-tunnel-macos.sh
```

or (select by "name", which is what you get from the saved tunnel's `Description`):

```
./start-tunnel-macos.sh --name=NetworkedChatApp
```

or (create and use a new tunnel that may be accessed later using the above command)

```
./start-tunnel-macos.sh --create=ExampleTunnelName
```

### Command: devtunnel show \<tunnel-id\>

```
docker run --rm -v "$HOME/.devtunnels:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 matrix-tunnel-image /usr/local/bin/devtunnel show <tunnel-id>
```

example output (removed some rows):
```
Tunnel ID             : puzzled-field-h34lflk.usw3
Description           : NetworkedChatApp
Labels                : VisualStudioCreatedTunnel
Access control        : {+Anonymous [connect]}
Ports                 : 1
  7081  https  https://sslk8rt0-7081.usw3.devtunnels.ms/  0 client connections  (Host:unchanged)
Tunnel Expiration     : 30 days
```

### Command: devtunnel delete \<tunnel-id\>

```
docker run --rm -v "$HOME/.devtunnels:/DevTunnels" -e DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1 matrix-tunnel-image /usr/local/bin/devtunnel delete <tunnel-id>
```

## Database Migration
To sync the database schema with the C# models:

1. Apply existing migration:
	```
	dotnet ef database update
	```

2. Create a new migration (if there is a change in the model):
	```
	dotnet ef migrations add <MigrationName>
	```

## Automation & Scripts
This project has a modular PowerShell script folder with files that can be used for setting up 
the development environment.

Location: ```Backend.API/scripts/```

### Script Architecture
* ```init-paths.ps1```: It calculates the needed paths needed by the other scripts
* ```setup-env.ps1```: it imports the paths and spins up the Docker containers and builds the .NET project


### How to Run
To synchronize the environment from the project root (```Networked_Chat_App```)
1. Open PowerShell as Administrator
2. Set the Execution Policy (at least for the first time)
```
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

3. Run the Setup Script
```
.\Backend-Service\backend.API\scripts\setup-env.ps1
```

## To kill a running process
If you have several running processes and you need to kill them enter:
```
taskkill /IM Backend.API.exe /F
```

## Testing the API
To verify that the backend is fucntioning properly and communicating with the Database we can use Postman.
Please follow this steps:

1.  Launch the server
* Make sure that Dockern is open and that your Networked chat container is running
* In Visual Studio select the Backend.API from the startup projects dropdown
* Pressn the **Green Play button**
* Confirm the console window appears and says it is listening to http://localhost:5000

2. Import the Postman Collection
* Open Postman
* Click the Import button in the top left
* Drag and drop the ```MastersNetworkChatApp.postman_collection.json``` that is located under the
```tests\PostmanCollection``` folder

3. Execute the Requests
* POST (Seed User): Run this first to create a new user in the PostgreSQL database. 
You should see a ```200 OK``` response with a success message.
(After the first time you wil need to change the user info in the 
```TestController.cs```)
* GET (All Users): Run this to fetch all the current users in the database.
