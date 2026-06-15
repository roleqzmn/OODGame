# OODGame

# OODGame

A console-based dungeon crawler written in C# (`.NET 8`) with both local gameplay and a `Server–Client` multiplayer architecture.

## Table of Contents
- [Project Overview](#project-overview)
- [Key Features](#key-features)
- [Requirements](#requirements)
- [Running the Project](#running-the-project)
- [Controls](#controls)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Network Protocol (JSON)](#network-protocol-json)
- [Known Limitations](#known-limitations)

---

## Project Overview
`OODGame` is a course project developed in stages. The game currently supports:
- dungeon generation,
- player movement,
- enemies and combat,
- inventory and equipment,
- synchronized game state for multiple clients.

Multiplayer mode uses an authoritative server model: clients send actions, the server validates them, and broadcasts state updates.

---

## Key Features
- Startup modes:
  - `--server [port]`
  - `--client [ip:port]`
  - interactive fallback: `Start as (S)erver or (C)lient?`
- TCP networking (`TcpListener` / `TcpClient`)
- `System.Text.Json` serialization with DTO contracts
- Queued player actions + thread-safe model updates (`lock` + queue)
- Incremental map redraw on clients (only changed cells)
- Server-synchronized combat with a dedicated combat UI on clients
- Reactive enemy behavior (follow/flee/ignore, sound + player presence)
- Client inventory workflow: preview, pickup, drop, equip

---

## Requirements
- .NET SDK 8.0+
- Windows / Linux / macOS terminal (UTF-8 support recommended)

Check installed .NET version:

```powershell
dotnet --version
```

---

## Running the Project
From the repository root:

```powershell
dotnet build .\OODGame\OODGame.csproj
```

### 1) Start server

```powershell
dotnet run --project .\OODGame\OODGame.csproj -- --server
```

Custom port:

```powershell
dotnet run --project .\OODGame\OODGame.csproj -- --server 6000
```

### 2) Start client

```powershell
dotnet run --project .\OODGame\OODGame.csproj -- --client
```

Custom endpoint:

```powershell
dotnet run --project .\OODGame\OODGame.csproj -- --client 127.0.0.1:6000
```

### 3) Interactive mode (no args)

```powershell
dotnet run --project .\OODGame\OODGame.csproj
```

---

## Controls

### Movement and core actions
- `W/A/S/D` — move
- `E` — pickup (client uses a 2-step preview + confirm flow)
- `Q` — drop item
- `I` — open/close inventory
- `ESC` — close inventory / quit game

### Inventory (client)
- `0..9` — select item index
- `←` / `→` — change selection
- `R` — equip selected item
- `L` / `K` — set preferred equip hand (`left` / `right`)

### Combat (client)
- `↑` / `↓` — select attack style
- `E` — confirm attack
- `R` — leave combat

---

## Architecture

### 1. Startup
- `Startup/StartupOptions.cs` — startup option model
- `Startup/StartupParser.cs` — CLI parser
- `Startup/StartupPrompt.cs` — interactive fallback
- `Startup/Startup.cs` — startup coordinator

### 2. Core gameplay
- `Game.cs` — main game state aggregate
- `Actions/Action.cs` — action mapping and execution
- `Map/*`, `Entities/*`, `Player/*`, `Items/*` — domain model

### 3. Rendering
- `Draw/Draw.cs` — map, UI and panel rendering
- `Draw/ConsoleInteractionView.cs` — local inventory/tile interactions
- `Fight/*` — combat logic and view

### 4. Networking
- `Networking/Protocol/*` — message contracts (DTO, envelope, JSON)
- `Networking/Transport/JsonLineChannel.cs` — JSON-per-line transport
- `Networking/Server/ServerRuntime.cs` — server loop and broadcasting
- `Networking/Client/ClientRuntime.cs` — client input + state rendering

---

## Project Structure

```text
OODGame/
├─ OODGame/                      # Main .NET project
│  ├─ Actions/
│  ├─ Draw/
│  ├─ Dungeon/
│  ├─ Entities/
│  ├─ Events/
│  ├─ Fight/
│  ├─ Input/
│  ├─ Items/
│  ├─ Logger/
│  ├─ Map/
│  ├─ Networking/
│  │  ├─ Client/
│  │  ├─ Protocol/
│  │  ├─ Server/
│  │  └─ Transport/
│  ├─ Player/
│  ├─ Startup/
│  ├─ Game.cs
│  ├─ Program.cs
│  └─ OODGame.csproj
├─ handdown.md
├─ handdown_overview.md
└─ README.md
```

---

## Network Protocol (JSON)
Core message groups:
- `ProtocolMessageType`: `InitialState`, `PlayerAction`, `StateUpdate`, `Error`, `Ack`
- `PlayerActionType`: includes `Move*`, `PickupItem`, `DropItem`, `EquipItem`, `Attack`, `Quit`
- `StateUpdateType`: includes `PlayerMoved`, `Attack`, `SoundEvent`, `PlayerActionRejected`

Messages are exchanged as single-line JSON frames via `JsonLineChannel`.

---

## Known Limitations
- Console-first project: functionality was prioritized over advanced presentation.
- Some mechanics (e.g., later item-slot stages) can still be expanded.
- No full automated test suite yet (current verification focuses on manual E2E testing).

---

## Author
Maciej Majewski
Developed as an Object-Oriented Design coursework project.
