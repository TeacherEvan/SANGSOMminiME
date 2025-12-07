# Architectural Alternatives Overview (ARCHIVED)

> **Note**: This research was conducted before the project committed to Unity 2022.3 LTS. The project now uses Unity exclusively. This document is retained for historical reference only.

This document captures the standout runtime stacks that were evaluated as potential alternatives to Unity.

| Practicality Rank | Stack Title                        | Core Language         | Philosophy                                    | When It Excels                                                           |
| ----------------- | ---------------------------------- | --------------------- | --------------------------------------------- | ------------------------------------------------------------------------ |
| 0                 | The Pragmatic Educator _(Godot 4)_ | GDScript / C#         | Python-like scripting with a full editor.     | Educational projects, Python-fluent teams, rapid prototyping.            |
| 1                 | The Open .NET Sovereign _(Stride)_ | C# / .NET 8           | Keep the editor, keep C#, lose the black box. | Teams with C# talent that want open-source control and modern rendering. |
| 2                 | The Data-Driven Architect _(Bevy)_ | Rust                  | Pure ECS, code-first, no legacy cruft.        | Simulation-heavy work that benefits from deterministic multithreading.   |
| 3                 | The Minimalist Artisan _(Raylib)_  | C / language bindings | Library over engine. You own the loop.        | Tooling, research prototypes, or handcrafted 2D/3D experiences.          |

## Decision Axes

- **Innovation vs. Practicality:** Godot offers the lowest friction for Python-fluent developers; Stride is fastest for C# veterans; Bevy modernizes the architecture; Raylib maximizes control.
- **Tooling Surface:** Godot and Stride ship with full editors; Bevy expects you to write editors if you need them; Raylib stays minimal by design.
- **Documentation Expectations:** Godot has vast beginner-focused tutorials; Stride mirrors Microsoft's .NET style; Bevy provides the technical "Bevy Book"; Raylib relies on exhaustive code samples.

## Complementary Asset Tools

To push fidelity further, pair any runtime with specialist content pipelines:

- **Houdini** for procedural worlds and systems.
- **Cascadeur** for physics-assisted animation.
- **Material Maker** for node-based PBR textures without Adobe lock-in.

## Context-Specific Recommendation (ARCHIVED)

> **Decision**: The project proceeded with Unity 2022.3 LTS for its mature ecosystem, C# support, and WebGL deployment capabilities.

The following was the original analysis:

- **Best fit at the time:** Godot 4 (Python-like GDScript, educational focus).
- **Alternative for Rust mastery:** Bevy (but requires building UI tooling).
- **Alternative for C# and VR potential:** Stride (larger learning curve than Godot).

**Final Decision:** Unity 2022.3 LTS was chosen for stability, asset store resources, and cross-platform deployment.

Use the option briefs below to dive into best cases, best practices, and a pros/cons snapshot for each path.
