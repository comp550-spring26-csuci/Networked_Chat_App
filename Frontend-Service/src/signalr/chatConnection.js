import * as signalR from "@microsoft/signalr";

const accessToken = "YOUR_JWT_TOKEN";
export const TUNNEL_URL = "https://sslk8rt0-7081.usw3.devtunnels.ms";

let connection;

export function getConnection() {
    return connection;
}

export function startSignalRConnection() {
  console.log(`TOKEN in startSignalRConnection: ${localStorage.getItem("access_token")}`);

  connection = new signalR.HubConnectionBuilder()
    .withUrl("https://sslk8rt0-7081.usw3.devtunnels.ms/chathub", {
      accessTokenFactory: () => localStorage.getItem("access_token")
    })
    .withAutomaticReconnect()
    .build();

  return connection.start()
    .then(() => console.log("SignalR connected"))
    .catch(err => console.error("SignalR error:", err));
}

export function joinChatRoom(chatRoomId) {
    return connection.invoke(
        "JoinChatRoom", { 
            performChatRoomAction : { 
                chatRoomId: chatRoomId 
            } 
        }
    );
}

export function leaveChatRoom(chatRoomId) {
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