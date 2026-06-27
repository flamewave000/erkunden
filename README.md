# Erkunden

Erkunden is an experimental 3D space project created to learn and explore OpenTK and desktop OpenGL development in .NET.

Purpose
-------

This repository is primarily a learning and experimentation playground. The goal was to prototype a small space game framework while getting hands-on experience with OpenTK (windowing, OpenGL bindings, input, and basic rendering), game loops, and simple ECS-style organization.

What you'll find here
---------------------

- `Erkunden.Client` — the OpenTK-based client and rendering sandbox. This is where most graphics, input, and demo game code lives.
- `Erkunden.Core` — shared game primitives, physics helpers, and basic game object interfaces.
- `Erkunden.ECS` — small entity/component system utilities used by the project.

Quick start
-----------

Prerequisites

- Install the .NET SDK (6.0, 7.0, 8.0 or later).
- The client uses OpenTK via NuGet; restoring packages will install it.

Build and run (from repository root)

```bash
dotnet restore
dotnet build

# Run the client (OpenTK window)
dotnet run --project Erkunden.Client/Erkunden.Client.csproj

# Optionally run the server
dotnet run --project Erkunden.Server/Erkunden.Server.csproj
```

Notes
-----

- This is a learning project: expect rough edges, TODOs, and experimental code.
- The focus was on exploring OpenTK features (context creation, GL bindings, frame timing, input handling), not on production-ready architecture.
- If something doesn't build for you, check your .NET SDK version and restore NuGet packages.

Project layout (high level)
---------------------------

- [Erkunden.Client](Erkunden.Client): OpenTK rendering, game loop, sample scenes, and assets.
- [Erkunden.Core](Erkunden.Core): core types shared across projects (game objects, components, physics helpers).
- [Erkunden.ECS](Erkunden.ECS): small ECS helpers used by the client and experiments.
- [Erkunden.Server](Erkunden.Server): simple headless server for testing game logic separate from rendering.
-- Notes and design writeups are included inline across the repository where relevant.

Contributing & experimentation
------------------------------

This repository isn't structured as a library for reuse — it's a personal sandbox. Contributions are welcome if you keep them small and focused (bug fixes, documentation, small examples). If you plan to extend the client or add features, open an issue first to discuss the approach.

Acknowledgements
----------------

Built as a learning exercise with OpenTK and .NET.

License
-------

This project is released under the license in the repository: [LICENSE](LICENSE).

Enjoy exploring OpenTK and 3D graphics!
