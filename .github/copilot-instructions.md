# Sangsom Mini-Me AI Developer Guide

## Project Context

- **Engine**: Unity 6000.2.15f1 (Unity 6)
- **Language**: C# (Unity Runtime), Python (Blender Automation)
- **Core Concept**: Educational Tamagotchi where homework completion drives character growth.
- **Key Constraint**: **NO DEATH/FAILURE STATES.** Positive reinforcement only.

## Golden Rules (Non-Negotiable)

1. **No death / failure / punishment mechanics.**
   - Meters must never decay below floors.
   - Missed logins reset streak to 1 (no penalties beyond that).
2. **Prefer event-driven + scheduled work over per-frame polling.**
   - Avoid doing gameplay logic in `Update()` unless strictly necessary.
3. **Persist via the existing `UserManager` JSON file flow.**
   - Do not introduce new save systems (e.g., `PlayerPrefs`) for profile state.
4. **Keep edits surgical and consistent with existing patterns.**
   - Reuse existing managers/utilities; don’t invent parallel systems.

## Architecture & Codebase Map

### Namespace vs. Folder Structure

**Important**: Logical namespaces (`SangsomMiniMe.Core`) do not always match physical folder depth.

### Fast File Index (Start Here)

If you are a future agent, start by opening these files first (they are the canonical “source of truth”):

- **Orchestration**: `Assets/Scripts/Runtime/GameManager.cs` (`SangsomMiniMe.Core`)
- **Persistence + user cache**: `Assets/Scripts/Runtime/UserManager.cs` (`SangsomMiniMe.Core`)
- **Profile model (meters, rewards, streak)**: `Assets/Scripts/Runtime/UserProfile.cs` (`SangsomMiniMe.Core`)
- **Meter decay rules**: `Assets/Scripts/Runtime/MeterDecaySystem.cs` (`SangsomMiniMe.Core`)
- **Balance constants (floors, decay rates)**: `Assets/Scripts/Runtime/GameConstants.cs` (`SangsomMiniMe.Core`)
- **Tunable config asset**: `Assets/Scripts/Runtime/GameConfiguration.cs` (`SangsomMiniMe.Core`)
- **Main in-game UI**: `Assets/Scripts/Runtime/GameUI.cs` (`SangsomMiniMe.UI`)
- **Character animation controller**: `Assets/Scripts/Runtime/CharacterController.cs` (`SangsomMiniMe.Character`)

Support systems you will frequently touch:

- **UI easing**: `Assets/Scripts/Runtime/UITransitionManager.cs`
- **Pooling**: `Assets/Scripts/Runtime/ObjectPool.cs`
- **Validation helpers**: `Assets/Scripts/Runtime/ValidationUtilities.cs`
- **Analytics (no PII)**: `Assets/Scripts/Runtime/EducationalAnalytics.cs`

Tests:

- `Assets/Scripts/Tests/UserProfileTests.cs`
- `Assets/Scripts/Tests/GameUtilitiesTests.cs`

### Core Systems

- **GameManager**: Singleton orchestrator. Handles global state, auto-save loops, and debug keys.
- **UserManager**: Handles `UserProfile` persistence via `JsonUtility`. Uses Async I/O for non-blocking saves.
- **CharacterController**: Manages visual state (Animator), customization (eye scale, outfits), and cultural gestures (Wai, Bow).
- **EducationalAnalytics**: Tracks learning progress without collecting PII.

### Data Flow (Where State Lives)

- **Profile state lives in `UserProfile`** (coins/XP, meters, streaks, owned items).
- **Disk persistence is owned by `UserManager`**.
  - Save file path is `Application.persistentDataPath/<saveFileName>`.
  - Default filename is `userProfiles.json`.
- **Game loop writes to profile → marks dirty → UserManager auto-saves**.
  - `UserProfile` calls `UserManager.Instance?.MarkDirty()` internally.

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

### 2a. Serialization Constraints (Unity `JsonUtility`)

- **Assume `JsonUtility` will NOT reliably serialize `DateTime`.**
  - If you need to persist a timestamp, store it as a string (e.g., ISO-8601) or a long (ticks/binary) and parse it.
  - Prefer the existing `lastLoginDateString` pattern in `UserProfile`.
- Keep save data **simple**: primitives, strings, lists, and `[Serializable]` structs/classes.

### 3. Unity Specifics

- **Object Pooling**: Use `Core.ObjectPool<T>` for high-frequency spawns (coins, particles).
- **UI Animation**: Use `UITransitionManager` for easing (Elastic, Bounce) instead of raw `Mathf.Lerp`.
- **Performance**: Avoid `GetComponent` in `Update`. Cache references in `Awake`.

## Workflows

### Testing

- **Run Tests**: `Unity.exe -runTests -testResults results.xml -projectPath .`
- **Editor**: Use "Test Runner" window.
- **Key Tests**: `UserProfileTests.cs` (Logic), `GameUtilitiesTests.cs` (Math).

### Persistence (How Saving Works)

- `UserManager` owns saving/loading:
  - Loads once at startup (`Awake` → `Initialize` → `LoadUserProfilesFromDisk`).
  - Saves asynchronously on changes (`SaveAsync`) and synchronously on quit.
- If you change `UserProfile` fields that should persist:
  1.  Ensure they are `[SerializeField]` and serializable by Unity.
  2.  Ensure every mutation path calls `MarkDirty()` (directly or via existing methods).
  3.  Add/adjust tests in `Assets/Scripts/Tests/UserProfileTests.cs` when practical.

### Asset Pipeline

- **Blender**: Python scripts in `Blender/` folder automate character export.
- **Import**: FBX files go to `Assets/Characters/{StudentName}/Models/`.

## Common Tasks

### Add a New Meter (Exact Checklist)

If you add a new persistent meter, update all of these places (do not skip steps):

1. **Model**: Add a `[SerializeField] private float` field + public getter in `Assets/Scripts/Runtime/UserProfile.cs`.
2. **Mutation APIs**: Add `IncreaseX`, `DecreaseX`, and update `ApplyMeterDecay(...)` to include it.
   - Enforce a **floor** (cozy design) in `ApplyMeterDecay`.
3. **Balance**: Add decay rate + floor + defaults in `Assets/Scripts/Runtime/GameConstants.cs`.
4. **Decay loop**: Update `Assets/Scripts/Runtime/MeterDecaySystem.cs` to decay it and raise UI events.
5. **UI**: Add a Slider/Text in `Assets/Scripts/Runtime/GameUI.cs` and update `UpdateMeterDisplays()`.
6. **Mood/feedback (optional)**: If it affects mood, adjust `GetOverallMood(...)` and any UI wording.
7. **Tests**: Update or add tests in `Assets/Scripts/Tests/UserProfileTests.cs`.

### Add a New Character Action (Exact Checklist)

1. Add a public method on `Assets/Scripts/Runtime/CharacterController.cs`.
2. Add/confirm Animator parameters (keep names centralized in `GameConstants.AnimationParams` if applicable).
3. Wire UI buttons in `Assets/Scripts/Runtime/GameUI.cs` using the existing `SetupButtonWithFeedback(...)` pattern.

### Offline Progress (Implemented: Gentle Meter Catch-Up)

Offline meter decay catch-up is implemented and intentionally “cozy”:

- Timestamp is stored as `long` UTC ticks on `UserProfile` (`lastMeterDecayUtcTicks`) to stay `JsonUtility`-safe.
- On login, `GameManager` calls `UserProfile.ApplyOfflineMeterDecayCatchUp(DateTime.UtcNow.Ticks)`.
- Catch-up decay is clamped via `GameConstants.MaxOfflineMeterDecayMinutes` and still respects meter floors via `ApplyMeterDecay(...)`.
- Runtime decay updates the same timestamp in `MeterDecaySystem.ApplyDecay(...)` to keep offline catch-up accurate.

## Reference Material

- `Docs/GAMEPLAY_UX_GUIDE.md`: Detailed design specs and benchmarks.
- `.cursor/rules/`: Detailed agent-specific rules (read `sangsom-minime-project-agent.mdc`).

## Known Pitfalls (For Future Agents)

- Avoid relying on `DateTime` fields for persisted gameplay logic; prefer string/long timestamps (`lastLoginDateString`, `lastMeterDecayUtcTicks`).
- Be cautious adding new third-party systems (Behavior Trees, DOTween, ML-Agents): only introduce them when there is a clear requirement and integration plan.
