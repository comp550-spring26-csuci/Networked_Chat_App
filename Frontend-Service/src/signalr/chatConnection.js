import * as signalR from "@microsoft/signalr";

const accessToken = "YOUR_JWT_TOKEN";
const SIGNALR_URL = "https://sslk8rt0-7081.usw3.devtunnels.ms/";

let connection;

export function getConnection() {
    return connection;
}

export function startSignalRConnection() {
  const token = localStorage.getItem("access_token");

  connection = new signalR.HubConnectionBuilder()
    .withUrl("https://sslk8rt0-7081.usw3.devtunnels.ms/chathub", {
      accessTokenFactory: () => token
    })
    .withAutomaticReconnect()
    .build();

  connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error:", err));
}

export function joinChatRoom(chatRoomId) {
    return connection.invoke("JoinChatRoom", {
        ChatRoomId: chatRoomId
    });
}

export function leaveChatRoom(chatRoomId) {
    return connection.invoke("LeaveChatRoom", {
        ChatRoomId: chatRoomId
    });
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