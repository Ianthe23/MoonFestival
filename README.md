# Festival Muzica - Client-Server Application

A .NET-based client-server application for managing a music festival, featuring both TCP and SignalR communication protocols.

## 🎯 Project Overview

This application is designed to manage various aspects of a music festival, including:

- Employee management
- Client management
- Show scheduling
- Ticket sales and management

The system uses a multi-layered architecture with both TCP and SignalR communication protocols for real-time updates.

## 🏗️ Architecture

The project is structured into several key components:

- **FestivalMuzica.Client**: The client application that provides the user interface
- **FestivalMuzica.Server**: The server application that handles business logic and data persistence
- **FestivalMuzica.Networking**: Contains networking-related code and protocols
- **FestivalMuzica.Common**: Shared models and interfaces used across the application

## 🚀 Features

- Real-time updates using SignalR
- TCP-based communication for core operations
- Database persistence for all entities
- Multi-user support
- Comprehensive logging system

## 🛠️ Technologies Used

- .NET Core
- SignalR for real-time communication
- TCP/IP for core networking
- Entity Framework Core (based on repository pattern)
- Microsoft.Extensions for dependency injection and configuration
- ASP.NET Core for SignalR hosting

## 📋 Prerequisites

- .NET 6.0 or later
- SQL Server (or compatible database)
- Visual Studio 2022 (recommended) or VS Code

## 🔧 Installation

1. Clone the repository:

```bash
git clone [repository-url]
```

2. Navigate to the project directory:

```bash
cd FestivalMuzica
```

3. Restore NuGet packages:

```bash
dotnet restore
```

4. Configure the database connection string in `FestivalMuzica.Server/appsettings.json`

5. Build the solution:

```bash
dotnet build
```

## 🚀 Running the Application

1. Start the server:

```bash
cd FestivalMuzica.Server
dotnet run
```

2. Start the client:

```bash
cd FestivalMuzica.Client
dotnet run
```

## 📁 Project Structure

```
FestivalMuzica/
├── FestivalMuzica.Client/        # Client application
├── FestivalMuzica.Server/        # Server application
├── FestivalMuzica.Networking/    # Networking components
├── FestivalMuzica.Common/        # Shared models and interfaces
└── FestivalMuzica.sln           # Solution file
```

## 🔐 Configuration

The server requires configuration in `appsettings.json`:

- Database connection string
- Server ports for TCP and SignalR
- Logging configuration

## 📝 License

[Specify your license here]

## 👥 Contributing

[Add contribution guidelines if applicable]

## 📞 Support

[Add support information if applicable]
