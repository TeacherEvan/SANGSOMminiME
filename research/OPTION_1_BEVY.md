# Option 1 · The Data-Driven Architect (Bevy)

- **Practicality Rank:** 2 (Primary recommendation; assumes comfort with Rust)
- **Innovation Signal:** Pure ECS core, deterministic parallelism, rapidly evolving open source culture.

## Snapshot
Bevy replaces the traditional scene graph with a data-first ECS where Components are plain Rust structs and Systems are pure functions. The engine schedules systems automatically and parallelizes them when their data access does not conflict, meaning you get multicore performance without manual threading. There is no legacy editor—everything is code, which keeps the architecture transparent and reviewable.

## Best-Case Uses
- **Simulation-heavy games:** colony/automation sims, systemic sandboxes, AI-rich educational tools.
- **Tooling that needs determinism:** analytics, replay systems, or procedural generation pipelines.
- **Cross-platform utilities:** compile to native desktop, web (Wasm), or even embedded targets.

## Best Practices
1. **Treat systems as pure transforms.** Borrow only what you need; let Bevy's scheduler maximize parallelism.
2. **Use schedule configs:** bucket startup, fixed-update, and real-time systems to keep gameplay deterministic.
3. **Limit heavy lights:** as per Bevy PBR guidance, enable shadow-casting sparingly (1–2 lights) to avoid GPU stalls.
4. **Leverage the asset processor:** convert source assets into runtime-friendly formats automatically instead of loading raw files at play time.

## Pros
- **Architectural clarity:** ECS is enforced rather than optional; no hidden global state.
- **Performance-by-default:** Rust safety plus automatic system parallelization.
- **Documentation:** The "Bevy Book" + example gallery acts as living, best-practice documentation.
- **Ecosystem velocity:** Crates like `bevy_rapier`, `bevy_asset_loader`, `bevy_hanabi` add specialized functionality without bloating the core.

## Cons
- **Tooling DIY:** no official editor yet; authoring workflows must be scripted or built in-house.
- **Rust expertise required:** ownership/borrowing concepts add onboarding friction for non-system programmers.
- **Breaking releases:** the project iterates quickly, so long-term branches must pin versions.

## Implementation Notes
- Prototype gameplay entirely in Rust—avoid hybrid pipelines unless you need legacy assets.
- Invest early in internal CLI/visualization tooling (e.g., `bevy_inspector_egui`) so designers can inspect ECS state without reading logs.
- Build CI around `cargo fmt`, `cargo clippy`, and targeted integration tests to catch API changes when updating Bevy.
