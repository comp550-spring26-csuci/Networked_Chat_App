# 💬 Network App Chat

___


## ⭐️ Members: (please enter your name here)
Ivana Bavin-Gomez-San Basilio

Ian Milin

___


## ⭐️ Overview
This project is dedicated to creating a desktop application that is inspired by the Discord application.

___


## ⭐️ Core Intent
To improve the user expereince.

___


## ⭐️ System Highlights
* **IDE**:Visual Studio

### 🔹 Front End 
* **Language**: Javascript, CSS
* **Frameworks**: 
	* **Electron**: Desktop container, hosts Next.js UI, handles native OS notifications.
	* **Next.js/React**: Web framework that handles the interface rendering and client-side state management.
* **Logging**: 
	* **Windston**:Node.js flexible library that supports multiple output destinations

### 🔹 Back End
* **Language**: 
	* **.NET 10**:Underlying runtime and kanguage C# to power back-end
* **Message Broker**: 
	* **RabbitMQ**: Decouples the API from the database saving process to prevent bottlenecks
* **Frameworks and Libraries**: 
	* **ASP.NET Core**: Core business logic and security processor
	* **SignalR Core**: Manages the live path sending messages to grous in real-time
* **Logging**: 
	* **Seralog**: Diagnostics library for .NET apps with structured logging
* **Server**: 
	* **Kestrel**: Open-source, cross-platform, and high-performance web server designed for ASP.NET Core applications
	
### 🔹 Database Layer: Possible DBs to be developed
* **NoSQLDatabase**: 
	* **.MongoDB**:Stores non-relational data like messages and event history for the "Recovery Path"
* **Relational Database**: 
	* **PostgresSQL**: Manages users, permissions, and the social graph (friendships/ roles)
* **In-memory Database**: 
	* **Redis**: Acts as high-speed cache and a backplane to sync events across back-end instances
* **Frameworks and Libraries**: 
	* **EF Core with Npgsql provider**: Entity mapping for PostgresSQL
	* **MongoDB.Driver**: For MongoDB mapping using the built-in Class Mapping features to handle schemas
	* **Microsoft.Extensions.Caching.StackExchangeRedis**: For Redis standardized IDistributeCache interface.
___

## ⭐️ Folder Structure
```

 Networked_Chat_App/                                      ← Root Folder
├── .vs/                                                  ← System metadata (Automatically created)
├── .gitignore                                            ← Files, folders or formats to ignore from git repo
├── docker-compose.yml                                    ← Docker related info for DBs running on it
├── Backend-Service/                                      ← Backend root folder
│   ├── README_BACK.md  							      ← BackEnd specific README file
│   ├── Backend.API/                                      ← Backend project folder
│   │   ├── Properties/                                   ← 
│   │   │   └── launchSettings.json                       ← Configuration info for app initiation with ports and vars for Swagger
|   |   |   
│   │   ├──scripts/                                       ← Folder with scripts to help set up paths for the db
│   │   │   ├── init-paths.ps1                            ← Script to locate root folder 
│   │   │   └── setup-env.ps1                             ← Script to setup the environment, docker related
|   |   |   
│   │   ├──src/                                           ← Folder with implementation
│   │   │   ├── API/                                      ← Front Door: Controllers, SignalR Hubs
│   │   │   │    ├── Controllers/                         ← REST endpoints for Next.js
│   │   │   │    │    └──TestController.cs                ← Sample reference for testing communication with the database for CRUD
│   │   │   │    └── Hubs/                                ← SignalsR Hubs for the live path
|   |   |   |
│   │   │   ├── Application/                              ← Brain: Services, DTOs, Business Logic
│   │   │   │    ├── DTOs/                                ← Data Transfer Objects for the Frontend
│   │   │   │    │    ├── AuthResult.cs				      ← DTO API's response to auth attemptdelivering access token or error message
│   │   │   │    │    ├── CreateAccountRequest.cs	      ← DTO defines required schema and initial validation rules for registration
│   │   │   │    │    └── LoginRequest.cs			      ← DTO to capture and validate user credentials sent form client during login
|   |   |   |    |
│   │   │   │    ├── Services/                            ← Logic for Login/Signup
│   │   │   │    │    └── AuthService.cs			      ← Core service responsible for business login for authenticaiton and more
|   |   |   |    |
│   │   │   │    └── Validators/                            ← Logic for data integrity, ensuring incoming DTOs meet requirements 
│   │   │   │         ├── CreateAccountRequestValidator.cs	← Implements ruls for new user registration, passowrd complexity, valid email format, etc.
│   │   │   │         └── LoginRequestValidator.cs			← Validates login attempts contain proper formatted credentials 
|   |   |   |
│   │   │   ├── Core/                                     ← Central domain layer with business logic: Entities, Domain Models, Utils
│   │   │   │    ├── Caching/                             ← Houses strategies for temporal data storage to improve performance and reduce redundance db calls
│   │   │   │    ├── Entities/                            ← Domain Models: User,cs, Message,cs , etc.
│   │   │   │    │    └── User.cs				          ← Represents the persistent data structures stored in the database
│   │   │   │    ├── Interfaces/                          ← Defines contracts and decoup[le business log form specific technical implementations
│   │   │   │    │     ├── IAuthService.cs	              ← Defines contract for authentication operations
│   │   │   │    │     ├── IUser.cs						  ← provides blueprint for user related properties and behaviors
│   │   │   │    │     └── IUserRepository.cs		      ← Outlines the data access methods for user persistence
│   │   │   │    └── Logging/                             ← Manages system diagnostics ourput
│   │   │   │          └── AppLogger.cs		              ← Logger class for teh backend, tracking applicaiton events and errors for easier debugging
|   |   |   |
│   │   │   └── Infrastructure/                           ← Hands: External Interactions
│   │   │       ├── Persistence/                          ← PostgresSAL & MongoDB implementations (User repository)
│   │   │       ├── MongoDB/                              ← Chat history implementation here
│   │   │       ├── Messaging/                            ← rabbitMQ Logic: Event Producers/Consumers
│   │   │       ├── Caching/                              ← Redis Logic
│   │   │       └── Logging/                              ← Serilog: classes & Interfaces # TO DO: (will be moved here in the futre)
│   │   │
│   │   ├── tests/                                        ← unit and Integration tests
│   │   ├── packages.lock.json
│   │   └── BackendNetworkChatApp.sln
│
│
├── Frontend-Service/                               ← Frontend root folder
│   │
│   ├──public/                                      ← Static assets: Logos, icons
│   ├──src/                                         ← Folder with implementation
│   │   ├── app/                                    ← Next.js 15 App Router: Pages & Layouts
│   │   ├── components/                             ← Reusable UI: Buttons, Chat Windows
│   │   ├── hooks/                                  ← React hooks: useSignalR, useAuth
│   │   ├── services/                               ← API clients & SignalR Hub connection logic
│   │   ├── electron/                               ← Electron main process & preload scripts
│   │   ├── types/                                  ← TypeScript interfaces for data
│   │   └── utils/                                  ← Helper functions
│   │
│   ├── packages.lock.json                                    
│   └── next.config.js
├── media/                                          ← stores images or other media used for documentation
└── docs/                                           ← general project docuemntation
   └── README.md  								    ← general README file for the entire project

```

---

## ⭐️ License
 
This project is not for commercial exploitation.
Falls under the MIT license.
---

## ⭐️ Development Environment Set Up
Here we will enter information regarding prerequisite and step to set the development environment and testing.

### 🔹 For Fornt end

### 🔹 For Back end

---

## ⭐️ Building the project
Here we will enter information regarding how to build the project for deployment











