# Backend.API Requirements: Netowrk Chat App

## Getting Started
1. Ensure you have **.NET 19 SDK** installed
2. Run `dotnet restore` to installn all requirements
3. Run `dotnet run` to start the API
4. Install Docker Desktop for windows AMD
1. To download the PostgresSQL go to the folder where the docker-compose.yml file is and run

```
docker compose up -d
```

## Requirements & Stack 
* **Framework:** .NET 10.0
* **Real-time:** SignalR Hubs
* **Database:** PostgreSQL (Core Data) & MongoDB (Chat History)
* **Messaging:** RabbitMQ (Event Bus)
* **Docker Desktop:** WSL 2 needs to be enabled for Windows users
* **Entity Framework Core Tools:**
	```
	  dotnet tool install --global dotnet-ef
	```

## Architecture
* **API:** Controllers and Hubs
* **Application:** Business logic and Services
* **Core:** Domain Entities and Interfaces

## Database Set Up
* **Docker:**
1.  Navigate to the root folder where ```docker-compose.yml``` is located
2. Spin up the PostgresSQL container:
	```
	docker-compose up -d
	```
3. The database will be available at ```localhost:5432``` with 
* **Development Secrets**:the connection string and local credentials are stored in `appsettings.Development.json`
* **Note**: this file is tipically excluded from source control in professional settings to protect credentials

## Building the Project
if you are setting this up for the first time or after moving files:

1. Clean and Restore:
	```
	dotnet clean
	```

2. Build:
	```
	dotnet build
	```


## Database Migration
To sync the database schema with the C# models:

1. Apply existing migratione:
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

## Database management & Visualization
To be able to see the PostgresSQL database visually inside Visual Studio

1. Intall the following Extension:
* Open **Extensions** > **Manage Extensions**
* Search for **"Npgsql PostgresSQL Integration"**
* Download and run the **VSIX Installer** (click "Modify" and close Visual Studio during the installation)

2. Connecting to the Database
Once the extension is installed, follow this steos in Visual Studio:
* Open **View*8 > **Server Explorer**
* Right-click **Data Connections*8 > **Add Connection**
* Change the data Source to **PostgreSQL**
* Enter the credentials maching ```appsettings.Development.json```
	*	**Host**: ```localhost```
	*	**Port**" ```5432```
	*	**Database**: ```ChatAppDb```
	*   **Username**: ```admin```
	*	**Password**: ```[the password]```
* Click **Test Conneciton**. 
* If this is successful then you can browse going to **Schemas > public > Tables** to see the tables.

3. Connection String
Ensure your ```appsettings.Development.json``` align with the Docker identity.


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
* GET (All Users): Run this to fetch all teh current users in the database.





# WebSocket Method Signatures Requiring Client-side Definitions:

These are just the client methods that the server's hub class will try to invoke on one or more client connections.
I'm linking to the class definitions of the arguments that the server passes to these methods, and then what the json
structure looks like on the client side when the client handles the associated method call.

client methods to implement:
- ReceiveMessagePreview
- ReceiveMessage
- ReceiveEvent
- ReceiveError

## ReceiveMessagePreview()

Notes:
- This is for notifications and previews, invoked regardless of whether the client is actively viewing the given chat room.

### Server side:

<code>class [MessagePreview](/Backend-Service/Backend.API/src/Application/DTOs/MessagePreview.cs)</code>
- Preview will fit no more than 50 characters

<code>class [TestMessagePreview](/Backend-Service/Backend.API/src/Application/DTOs/TestDTOs/TestMessagePreview.cs)</code>
- Verbose envelope for `MessagePreview`

### Client side:

```
connection.on(
    "ReceiveMessagePreview",
    {
        "messagePreview": {
            "chatRoomId": "d5789a29-0000-0000-0000-000000000000",
            "senderUsername": "ian"
            "content": "Hi My name is Ian how are you. What is your name? ...",
            "timestamp": "2026-05-03T06:02:55.7608972Z"
        },
        "chatRoomName": "ian_kenneth",
        "senderId": "f280b38b-cda0-4465-80c0-2f4db187fe1d"
    }
)
```

## ReceiveMessage()

Notes:
- This is for messages that were sent by a client to be persisted in the server's `Messages` collection and then received by other clients

### Server side:

<code>class [Message](/Backend-Service/Backend.API/src/Core/Entities/Message.cs)</code>
- Persisted in `Messages` collection

<code>class [TestMessage](/Backend-Service/Backend.API/src/Application/DTOs/TestDTOs/TestMessage.cs)</code>
- Verbose envelope for `Message`

```
connection.on(
    "ReceiveMessage",
    {
        "message": {
            "id": "69f6e50f9168a53903471c6f",
            "senderId": "f280b38b-cda0-4465-80c0-2f4db187fe1d",
            "senderUsername": "ian",
            "chatRoomId": "d5789a29-0000-0000-0000-000000000000",
            "content": "Hi My name is Ian how are you. What is your name? What is your favorite color?",
            "timestamp": "2026-05-03T06:02:55.7602882Z"
        },
        "chatRoomName": "ian_kenneth",
        "senderUsername": "ian"
    }
)
```

## ReceiveEvent()

Notes:
- This is for chat events recognizing server actions such as users connecting or chat rooms being created
- The event is also stored in the `Events` collection

### Server side:

<code>class [ChatEvent](/Backend-Service/Backend.API/src/Core/Entities/ChatEvent.cs)</code>

<code>class [TestChatEvent](/Backend-Service/Backend.API/src/Application/DTOs/TestDTOs/TestChatEvent.cs)</code>

### Client side:

```
connection.on(
    "ReceiveEvent",
    {
        "chatEvent": {
            "id": "69f6e5179168a53903471c70",
            "chatRoomId": "d5789a29-0000-0000-0000-000000000000",
            "eventType": "UserLeft",
            "details": "ChatRoom: ian_kenneth, Username: ian",
            "timestamp": "2026-05-03T06:03:03.8497462Z"
        },
        "chatRoomName": "ian_kenneth"
    }
)
```

## ReceiveError()

Notes:
- This is for errors that are expected to be sent to the caller after they have invoked a server method that resulted in a server-side error.

### Server side:

```
string errorMessage;
```

### Client side:

```
connection.on(
  "ReceiveError",
  "This is an unrealistic error message because I have not taken the time to demonstrate any errors to present"
)
```
