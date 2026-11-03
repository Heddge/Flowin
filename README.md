# Flowin

Flowin is a cross-platform note-taking application built with .NET MAUI and Blazor Hybrid. It follows an offline-first approach, allowing users to manage their notes locally and synchronize changes with a remote server.

The project consists of a client application, an ASP.NET Core backend, shared API contracts, and a test project.

## Features

- **Offline-first architecture** — notes remain accessible without an internet connection.
- **Local storage** — SQLite database for persistent local data.
- **Cloud synchronization** — synchronization between the local database and a remote PostgreSQL database through a REST API.
- **Bidirectional synchronization** — changes can be synchronized between client and server.
- **Offline change tracking** — local changes are tracked and synchronized when connectivity is available.
- **User authentication** — user accounts and isolated user data.
- **Markdown editor** — WYSIWYG editing experience with Markdown support.
- **In-app notifications** — centralized notification management with a queue, history, and automatic dismissal.
- **Cross-platform architecture** — designed for Windows and Android.

## Tech Stack

### Client
- C#
- .NET 9
- .NET MAUI
- Blazor Hybrid
- Entity Framework Core
- SQLite
- JavaScript
- ProseMirror

### Backend
- ASP.NET Core
- ASP.NET Core Identity
- REST API
- Entity Framework Core
- PostgreSQL

### Infrastructure & Tools
- Docker
- Docker Compose
- Git
- Visual Studio

## Architecture

Flowin uses a layered architecture that separates the user interface, application logic, local persistence, and remote synchronization.

```text
┌──────────────────────────────┐
│       MAUI / Blazor          │
│         Client UI            │
└──────────────┬───────────────┘
               │
               ▼
┌──────────────────────────────┐
│      Application Layer       │
│      Containers / Managers   │
└──────────────┬───────────────┘
               │
         ┌─────┴──────┐
         ▼            ▼
┌──────────────┐  ┌──────────────────┐
│    SQLite    │  │ Synchronization  │
│ Local Storage│  │     Manager      │
└──────────────┘  └────────┬─────────┘
                           │
                           ▼
                  ┌──────────────────┐
                  │    REST API      │
                  │  ASP.NET Core    │
                  └────────┬─────────┘
                           │
                           ▼
                  ┌──────────────────┐
                  │   PostgreSQL     │
                  └──────────────────┘
```

The client operates independently of the server for local note management. The synchronization layer handles communication with the backend and keeps local and remote data consistent.

## Synchronization

Synchronization is designed around note modification timestamps and a local change-tracking mechanism.

The client maintains a local SQLite database and tracks offline operations, including:

- Note creation
- Note updates
- Note deletion

When connectivity is available, pending changes are sent to the server through the REST API.

The synchronization process also compares local and remote data using modification timestamps to identify changes.

```text
SQLite
   │
   ▼
SynchronizeManager
   │
   ├── Local change tracking
   │
   ▼
REST API
   │
   ▼
PostgreSQL
```

## Project Structure

```text
Flowin/
├── Flowin/
│   ├── Components/
│   ├── Managers/
│   ├── Objects/
│   ├── Data/
│   ├── Pages/
│   ├── Services/
│   └── MauiProgram.cs
│
├── Flowin.server/
│   └── ASP.NET Core backend
│
├── Flowin.contracts/
│   └── Shared API contracts
│
├── Flowin.Tests/
│   └── Automated tests
│
└── docker-compose.yml
```

## Getting Started

### Requirements

- .NET 9 SDK
- Visual Studio 2022 or later
- .NET MAUI workload
- Docker Desktop

### Setup

Clone the repository:

```bash
git clone <repository-url>
```

Start the PostgreSQL database:

```bash
docker compose up -d
```

Open the solution in Visual Studio and configure the database connection string for the backend.

Run the `Flowin.server` project and then launch the `Flowin` client application.

### Platform Support

| Platform | Status |
|---|---|
| Windows | Implemented and tested |
| Android | Implemented and tested |

## License

This project is licensed under the MIT License.
