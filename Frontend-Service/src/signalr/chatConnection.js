import * as signalR from "@microsoft/signalr";

const accessToken = "YOUR_JWT_TOKEN";
export const TUNNEL_URL = "https://sslk8rt0-7081.usw3.devtunnels.ms";

let connection;

export function getConnection() {
    return connection;
}

export async function ensureSignalRConnection() {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        return connection;
    }

    if (connection && connection.state === signalR.HubConnectionState.Connecting) {
        return connection;
    }

    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl(`${TUNNEL_URL}/chathub`, {
                accessTokenFactory: () => localStorage.getItem("access_token")
            })
            .withAutomaticReconnect()
            .build();

        connection.onclose(async () => {
            console.log("SignalR disconnected — retrying...");
            setTimeout(() => ensureSignalRConnection(), 2000);
        });
    }

    try {
        await connection.start();
        console.log("SignalR connected (or reconnected)");
        return connection;
    } catch (err) {
        console.error("SignalR start failed:", err);

        // retry loop
        setTimeout(() => ensureSignalRConnection(), 3000);
    }
}

export function startSignalRConnection() {
  console.log(`TOKEN in startSignalRConnection: ${localStorage.getItem("access_token")}`);

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${TUNNEL_URL}/chathub`, {
      accessTokenFactory: () => localStorage.getItem("access_token")
    })
    .withAutomaticReconnect()
    .build();

  return connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error:", err));
}

export function joinChatRoom(chatRoomId) {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        console.warn("SignalR not connected yet");
        return;
    }

    return connection.invoke(
        "JoinChatRoom", { 
            performChatRoomAction : { 
                chatRoomId: chatRoomId 
            } 
        }
    );
}

export function leaveChatRoom(chatRoomId) {
    if (!connection || connection.state !== signalR.HubConnectionState.Connected) {
        console.warn("SignalR not connected yet");
        return;
    }

    return connection.invoke(
        "LeaveChatRoom", {
            performChatRoomAction : { 
                chatRoomId: chatRoomId 
            } 
        }
    );
}

export function sendMessage(chatRoomId, content, username) {
    return connection.invoke("SendMessageToChatRoom", {
        SendMessageToChatRoom: {
            ChatRoomId: chatRoomId,
            Content: content
        },
        SenderUsername: username
    });
}