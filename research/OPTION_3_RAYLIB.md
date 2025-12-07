# Option 3 · The Minimalist Artisan (Raylib)

- **Practicality Rank:** 3 (Maximum control, minimal scaffolding)
- **Innovation Signal:** "No engine" ethos—just a tiny C library with bindings for 60+ languages.

## Snapshot
Raylib is closer to SDL than to Unity. You call `InitWindow`, `BeginDrawing`, and handle everything else yourself. That simplicity is intentional: it keeps the mental model transparent and empowers you to craft custom editors, domain-specific runtimes, or research prototypes without engine constraints.

## Best-Case Uses
- **Custom toolchains:** visualizers, educational sandboxes, or diagnostic overlays.
- **Retro/2D/low-poly games:** especially when you want bespoke rendering quirks.
- **Embedded / constrained hardware:** Raspberry Pi, WebAssembly, micro consoles.

## Best Practices
1. **Design your architecture up front:** decide how you manage scenes, assets, and events; Raylib will not do it for you.
2. **Embrace single-responsibility structs:** treat each system (input, audio, physics) as its own module to stay sane.
3. **Use the example suite as documentation:** the official repo's 120+ samples demonstrate idiomatic patterns better than prose.
4. **Add your own hot-reload/dev UI:** instrumentation is on you—build debug overlays early to avoid printf-driven pain.

## Pros
- **Tiny dependency footprint:** compile in seconds, deploy anywhere.
- **Language freedom:** bindings exist for Python, Rust, Zig, Odin, Go, and beyond.
- **Educational clarity:** perfect for onboarding engineers who need to learn rendering basics without abstraction layers.

## Cons
- **You build the engine:** no editor, no asset importer, no prefab system.
- **Scaling cost:** large projects require disciplined architecture and tooling.
- **Limited 3D feature set:** possible, but lacks advanced pipelines (deferred rendering, clustered lighting) unless you implement them.

## Implementation Notes
- Pair with automation scripts (Python, Node, Rust) to handle asset conversion since Raylib lacks a pipeline.
- Consider layering ECS libraries (e.g., `entt`, `flecs`) if the project grows beyond simple loops.
- For UI, integrate immediate-mode libs (Dear ImGui bindings) to avoid writing widgets from scratch.
