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

 Networked_Chat_App/                                ← Root Folder
├── .vs/                                            ← System metadata (Automatically created)
├── Backend-Service/                                ← Backend root folder
│   ├──Backend.API/                                 ← Backend project folder
│   │   ├──Controllers/                             ← 
│   │   │   └── TestController.cs                   ← Sample reference for testing communication with the database for CRUD
│   │   ├──scripts/                                 ← Folder with scripts to help set up paths for the db
│   │   │   ├── init-paths.ps1                      ← Script to locate root folder 
│   │   │   └── setup-env.ps1                       ← Script to setup the environment, docker related
│   │   ├──src/                                     ← Folder with implementation
│   │   │   ├── API/                                ← Front Door: Controllers, SignalR Hubs
│   │   │   │    ├── Controllers/                   ← REST endpoints for Next.js
│   │   │   │    └── Hubs/                          ← SignalsR Hubs for the live path
│   │   │   ├── Application/                        ← Brain: Services, DTOs, Business Logic
│   │   │   │    ├── Services/                      ← Logic for Login/Signup
│   │   │   │    └── DTOs/                          ← Data Transfer Objects for the Frontend
│   │   │   ├── Core/                               ← Soul: Entities, Domain Models, Utils
│   │   │   │    ├── Entities/                      ← Create User,cs, Messaage,cs
│   │   │   │    └── Interfaces/                    ← Create IUSerRespository,cs
│   │   │   └── Infrastructure/                     ← Hands: External Interactions
│   │   │       ├── Persistence/                    ← PostgresSAL & MongoDB implementations (User repository)
│   │   │       ├── MongoDB/                        ← Chat history implementation here
│   │   │       ├── Messaging/                      ← rabbitMQ Logic: Event Producers/Consumers
│   │   │       ├── Caching/                        ← Redis Logic
│   │   │       └── Logging/                        ← Serilog: classes & Interfaces
│   │   │
│   │   ├── tests/                                      ← unit and Integration tests
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











