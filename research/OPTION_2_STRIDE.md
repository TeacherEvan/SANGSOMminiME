# Option 2 · The Open .NET Sovereign (Stride)

- **Practicality Rank:** 1 (Most pragmatic shift for C#/Unity veterans)
- **Innovation Signal:** Fully open-source C# stack with a modular renderer and Game Studio editor.

## Snapshot
Stride (formerly Xenko) embraces modern .NET 8, giving you access to the entire NuGet ecosystem while keeping an approachable editor for scene layout, asset import, and shader graph authoring. Because the engine source is MIT-licensed, you can patch or extend anything, from the rendering pipeline (RootRenderFeatures) to the audio stack.

## Best-Case Uses
- **Teams fluent in C#:** reuse domain knowledge without carrying Unity's legacy baggage.
- **Cinematic/VR experiences:** high-end PBR rendering, clustered lighting, and VR templates ship with the engine.
- **Tool+Game hybrids:** editor extensibility plus code-first access makes it simple to build bespoke pipelines.

## Best Practices
1. **Split game and engine assemblies:** mirror Stride's guidance so gameplay logic stays clean while engine forks remain upstreamable.
2. **Leverage Game Studio for content, code for behavior:** use the editor for asset orchestration, but keep gameplay systems in C# partial classes for testability.
3. **Use RootRenderFeatures intentionally:** customize the render graph (outlines, decals, terrain) via the documented extension points rather than hacking shaders in place.
4. **Automate builds with `stride build`:** ensures consistent content compilation and shader generation across contributors.

## Pros
- **Familiar workflow:** resembles Unity's scene/prefab pattern but with modern .NET and source access.
- **Extensible renderer:** documented community modules cover outlines, screen-space decals, atmospherics, etc.
- **Strong docs and samples:** official manual, API reference, and curated community project list.
- **VR-ready:** templates for OpenXR, plus robust input abstraction.

## Cons
- **Smaller ecosystem:** fewer off-the-shelf asset packs and third-party plugins.
- **Editor polish:** Game Studio is capable but less refined than Unity's decades-old UX.
- **Toolchain weight:** shader compilation and asset cooking can be slower on older hardware.

## Implementation Notes
- Fork the engine only when necessary; otherwise depend on upstream to stay current.
- Use CI runners with the Stride build pipeline to catch shader/asset regressions early.
- Pair with Houdini or Material Maker exports using FBX/GLTF to keep the art workflow engine-agnostic.
