import * as signalR from "@microsoft/signalr";

const accessToken = "YOUR_JWT_TOKEN";
// Synchronized with the correct active tunnel URL (changed to Ivana's)
export const TUNNEL_URL = "https://vg3jzw0g-7081.usw3.devtunnels.ms";

let connection;

export function getConnection() {
    return connection;
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
    if (!connection) return Promise.resolve();
    return connection.invoke("JoinChatRoom", {
        ChatRoomId: chatRoomId
    });
}

export function leaveChatRoom(chatRoomId) {
    if (!connection) return Promise.resolve();
    return connection.invoke("LeaveChatRoom", {
        ChatRoomId: chatRoomId
    });
}

export function sendMessage(chatRoomId, content, username) {
    if (!connection) return Promise.resolve();
    return connection.invoke("SendMessageToChatRoom", {
        SendMessageToChatRoom: {
            ChatRoomId: chatRoomId,
            Content: content
        },
        SenderUsername: username
    });
}