---
title: Use .NET 10 as the project's target framework
date: 2025-12-28
status: accepted
---

Context
-------
The HealthReport project is a new .NET-based solution for importing, storing and reporting health measurements (blood pressure, etc.). We must choose a single target framework for all projects in the solution to ensure consistent language features, library compatibility, build and CI configurations.

Decision
--------
We will target .NET 10 (`net10.0`) for all projects in the solution.

Rationale
---------
- LTS and support: .NET 10 is the supported platform for new development (current LTS/Recommended at the time of decision), providing a clear support window and security updates.
- Language and runtime features: .NET 10 includes the latest C# language features and runtime improvements (performance, startup, AOT improvements where relevant).
- Ecosystem and tooling: SDK and tooling (dotnet CLI, VS/JetBrains Rider, CI runners) are compatible and commonly provide images for .NET 10.
- Long-term maintenance: targeting a current, supported platform reduces technical debt and simplifies dependency updates.

Consequences
------------
- All csproj files should set `<TargetFramework>net10.0</TargetFramework>`.
- CI pipelines and Docker images must use .NET 10 images/runners.
- Contributors must install .NET 10 SDK locally or use the repository's recommended development container.
- If downstream consumers or hosting constraints require older runtimes, we must provide guidance or compatibility shims (not part of the default build).

Alternatives considered
-----------------------
- .NET 8 / .NET 7: older LTS releases with broader immediate compatibility for some hosting environments. Rejected because they lack newer runtime and language benefits and have shorter remaining support windows compared to .NET 10 at decision time.
- Multi-targeting (e.g., net7.0;net10.0): increases complexity for libraries and CI, and is unnecessary for an application-focused solution where a single runtime is preferable.

Notes / Implementation
---------------------
- Update each project file to `net10.0` (example already present in several csproj files).
- Ensure Dockerfiles and CI use `mcr.microsoft.com/dotnet/sdk:10.0` / matching runtime images.
- Document `dotnet --info` requirements in `README.md` and `.github/copilot-instructions.md`.

References
----------
- .NET official release notes and support policy.
