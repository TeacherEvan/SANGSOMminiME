# Sangsom Mini-Me AI Developer Guide

## Project Context

- **Engine**: Unity 6000.2.15f1 (Unity 6)
- **Language**: C# (Unity Runtime), Python (Blender Automation)
- **Core Concept**: Educational Tamagotchi where homework completion drives character growth.
- **Key Constraint**: **NO DEATH/FAILURE STATES.** Positive reinforcement only.

## Architecture & Codebase Map

### Namespace vs. Folder Structure

**Important**: Logical namespaces (`SangsomMiniMe.Core`) do not always match physical folder depth.

- `Assets/Scripts/Runtime/` contains core scripts like `GameManager.cs` and `UserManager.cs` (Namespace: `SangsomMiniMe.Core`).
- `Assets/Scripts/Runtime/Character/` contains `CharacterController.cs` (Namespace: `SangsomMiniMe.Character`).
- `Assets/Scripts/Runtime/UI/` contains `GameUI.cs` (Namespace: `SangsomMiniMe.UI`).

### Core Systems

- **GameManager**: Singleton orchestrator. Handles global state, auto-save loops, and debug keys.
- **UserManager**: Handles `UserProfile` persistence via `JsonUtility`. Uses Async I/O for non-blocking saves.
- **CharacterController**: Manages visual state (Animator), customization (eye scale, outfits), and cultural gestures (Wai, Bow).
- **EducationalAnalytics**: Tracks learning progress without collecting PII.

## Development Rules

### 1. Educational Game Design

- **Never implement punishment mechanics** (e.g., pet death, stat regression below floors).
- **Homework is the primary resource generator** (Happiness/Coins).
- **Respect Thai culture**: Use `Wai`, `Curtsy`, `Bow` animations appropriately.

### 2. Coding Standards

- **Serialization**: Use `[SerializeField] private` for Inspector exposure.
- **Persistence**: Use `JsonUtility` (native) over `Newtonsoft` for simple profile data.
- **Async/Await**: Prefer `async void` (fire-and-forget) or `async Task` for I/O operations to prevent frame drops.
- **Event-Driven**: Use C# Events (`Action<T>`) in `UserManager` instead of polling in `Update()`.

### 3. Unity Specifics

- **Object Pooling**: Use `Core.ObjectPool<T>` for high-frequency spawns (coins, particles).
- **UI Animation**: Use `UITransitionManager` for easing (Elastic, Bounce) instead of raw `Mathf.Lerp`.
- **Performance**: Avoid `GetComponent` in `Update`. Cache references in `Awake`.

## Workflows

### Testing

- **Run Tests**: `Unity.exe -runTests -testResults results.xml -projectPath .`
- **Editor**: Use "Test Runner" window.
- **Key Tests**: `UserProfileTests.cs` (Logic), `GameUtilitiesTests.cs` (Math).

### Asset Pipeline

- **Blender**: Python scripts in `Blender/` folder automate character export.
- **Import**: FBX files go to `Assets/Characters/{StudentName}/Models/`.

## Common Tasks

- **Adding a new Meter**: Update `UserProfile.cs`, add UI slider in `GameUI.cs`, register in `GameManager`.
- **New Animation**: Add trigger to `CharacterController`, update Animator Controller, expose public method.

## Reference Material

- `Docs/GAMEPLAY_UX_GUIDE.md`: Detailed design specs and benchmarks.
- `.cursor/rules/`: Detailed agent-specific rules (read `sangsom-minime-project-agent.mdc`).
