# Client-side SignalR Method Overview:

These are just the client methods that the server's hub class will try to invoke on one or more client connections.
I'm showing the file paths and class definitions of the argument that the server passes, and then what the json
structure looks like on the client side when the client handles the associated method call.

client methods to implement:
- ReceiveMessage
- AcknowledgeDirectMessage
- ReceiveEvent
- ReceiveError

## ReceiveMessage()

Notes:
- This is for messages that were sent by a client to be stored in the server's `Messages` collection and then received by other clients

#### Server side:

```
Backend-Service/Backend.API/src/Core/Entities

class Message // Stored In Messages collection
{
    private string id;
    Guid senderId;
    Guid chatRoomId;
    string content;
    DateTime timestamp;
}

Backend-Service/Backend.API/src/Application/DTOs/TestDTOs

class TestMessage // Basically just a verbose envelope for the Message object
{
    Message message;
    string? chatRoomName;
    string? username;
}
```

#### Client side:

```
connection.on(
  "ReceiveMessage", 
  { 
    "message":{
      "id":"69e1fc82222ae9b51336d7d3",
      "senderId":"491b2bca-bb7e-4881-8380-8f7361ce6ed2",
      "chatRoomId":"10cf2044-199a-40a9-b91e-df0cc4f1acfa",
      "content":"I didn't think of anything to say.",
      "timestamp":"2026-04-17T09:25:22.0643911Z"
    },
    "chatRoomName":"HubClient_01_HubClient_02",
    "username":"HubClient_01"
  }
)
```

## AcknowledgeDirectMessage()

Notes:
- After a client invokes `StartDirectMessage()` on the server, the server will join that client to a chatroom.
- The following user and chatroom information will then be sent the the user who the client intended to have a direct messaging area with which to message them.
- The client should handle this method by joining the chat room.

#### Server side:

```
Backend-Service/Backend.API/src/Application/DTOs

class AcknowledgeDirectMessage
{
    Guid senderId;
    Guid chatRoomId;
}

Backend-Service/Backend.API/src/Application/DTOs/TestDTOs

class TestAcknowledgeDirectMessage // Verbose envelope for the AcknowledgeDirectMessage object
{
    AcknowledgeDirectMessage acknowledgeDirectMessage;
    string username;
    string chatRoomName;
}
```

#### Client side:

```
connection.on(
  "AcknowledgeDirectMessage",
  {
    "acknowledgeDirectMessage":{
      "senderId":"491b2bca-bb7e-4881-8380-8f7361ce6ed2",
      "chatRoomId":"10cf2044-199a-40a9-b91e-df0cc4f1acfa"
    },
    "username":"HubClient_01",
    "chatRoomName":"HubClient_01_HubClient_02"
  }
)
```

## ReceiveEvent()

Notes:
- This is for chat events recognizing server actions such as users connecting or chat rooms being created
- The event is also stored in the `Events` collection

#### Server side:

```
Backend-Service/Backend.API/src/Core/Entities

public class ChatEvent
{
    string? id;
    Guid chatRoomId;
    string eventType;
    string details;
    DateTime timestamp;
}

Backend-Service/Backend.API/src/Application/DTOs/TestDTOs

public class TestChatEvent
{
    ChatEvent chatEvent;
    string? chatRoomName;
}
```

#### Client side:

```
connection.on(
  "ReceiveEvent",
  {
    "chatEvent":{
      "id":"69e1fbd1222ae9b51336d7d1",
      "chatRoomId":"10cf2044-199a-40a9-b91e-df0cc4f1acfa",
      "eventType":"UserJoinedChatRoom",
      "details":"ChatRoom: HubClient_01_HubClient_02",
      "timestamp":"2026-04-17T09:22:25.5610586Z"
    },
    "chatRoomName":null
  }
)
```

## ReceiveError()

Notes:
- This is for errors that are expected to be sent to the caller after they have invoked a server method that resulted in a server-side error.

#### Server side:

```
// No objects for this one, just a string
string errorMessage;
```

#### Client side:

```
connection.on(
  "ReceiveError",
  "This is an unrealistic error message because I have not taken the time to demonstrate any errors to present"
)
```
