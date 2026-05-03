import * as signalR from "@microsoft/signalr";

const accessToken = "YOUR_JWT_TOKEN";

export let connection;

export function startConnection() {
  const token = localStorage.getItem("access_token");

  connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7081/chathub", {
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