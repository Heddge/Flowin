# NotesFlow

NotesFlow is a cross-platform note-taking application with offline-first storage and synchronization between devices.

The application combines local SQLite storage with a remote PostgreSQL database and provides synchronization through a REST API.

## Features

* Offline-first architecture
* Local SQLite storage
* Cross-device synchronization
* PostgreSQL server database
* User authentication and isolated user data
* Conflict handling during synchronization
* Windows and Android support
* REST API
* Containerized backend infrastructure
* CI/CD pipeline

## Tech Stack

**Client**

* C#
* .NET MAUI
* Blazor Hybrid
* Entity Framework Core
* SQLite

**Backend**

* ASP.NET Core
* REST API
* Entity Framework Core
* PostgreSQL
* JWT

**Infrastructure**

* Docker
* GitHub Actions

## Architecture

```text
┌──────────────────────────┐
│      MAUI / Blazor       │
│          Client          │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│     Application Layer    │
│      NotesContainer      │
└────────────┬─────────────┘
             │
       ┌─────┴─────┐
       ▼           ▼
┌────────────┐ ┌──────────────────┐
│ SQLite     │ │ Synchronization  │
│            │ │     Manager      │
└────────────┘ └────────┬─────────┘
                        │
                        ▼
                 ┌─────────────┐
                 │  REST API   │
                 └──────┬──────┘
                        │
                        ▼
                 ┌─────────────┐
                 │ PostgreSQL  │
                 └─────────────┘
```

Local data remains available without an internet connection. Changes are synchronized with the server when connectivity is available.

## Synchronization

Synchronization is based on note modification timestamps.

The client keeps a local copy of the data and exchanges changes with the server through the REST API. The synchronization layer is isolated from the local storage implementation.

```text
SQLite
  │
  ▼
SynchronizeManager
  │
  ▼
REST API
  │
  ▼
PostgreSQL
```

## Project Structure

```text
NotesFlow/
├── Components/
├── Managers/
│   ├── NoteDbManager.cs
│   ├── SynchronizeManager.cs
│   └── ...
├── Objects/
│   ├── Note.cs
│   └── ...
├── Data/
│   └── NoteDbContext.cs
├── Pages/
├── Services/
└── MauiProgram.cs
```

## Getting Started

### Requirements

* .NET SDK
* Visual Studio 2022+
* .NET MAUI workload
* Docker Desktop

### Run

```bash
git clone https://github.com/Heddge/NotesFlow.git
cd NotesFlow
```

Start the backend infrastructure:

```bash
docker compose up -d
```

Open the solution in Visual Studio and run the application.

## License

This project is licensed under the MIT License.
