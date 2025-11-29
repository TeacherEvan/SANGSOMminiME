# Sangsom Mini-Me AI Guide
## Project Snapshot
- Unity 2022.3.12f1 drives the interactive tamagotchi (see ProjectSettings/ProjectVersion.txt); Blender 5.0.0 scripts create characters/assets under Blender/.
- VSCode AI rules live in .vscode/rules/** and SangsomMini-Me.mdc; read them before prompting so generated code respects educational + cultural boundaries.
- Assets/Characters/{Student}/ and Assets/Minime-Universe/** store content referenced by both Unity and Blender pipelines.

## Runtime Architecture (Unity)
- Assets/Scripts/Runtime is split by namespace: SangsomMiniMe.Core (game/state), .Character (avatar control), .UI (TMP/UGUI), .Educational (analytics); each asmdef (Runtime.asmdef, Editor.asmdef, Tests.asmdef) enforces dependencies.
- GameManager.cs orchestrates login/UI flow, subscribes to UserManager events, and runs autosave/debug keypaths; always route new global behavior through it instead of duplicating singletons.
- UserManager.cs serializes UserProfile objects to Application.persistentDataPath/userProfiles.json via JsonUtility; keep new profile fields [Serializable] and update UserProfileCollection to avoid save loss.
- CharacterController.cs mirrors saved customization (eye scale, outfit, accessories) and exposes PlayDance/Wai/Curtsy/Bow helpers; UI and debug hooks must call those public APIs rather than driving Animator directly.
- GameUI.cs is the canonical bridge between TMP controls and gameplay (homework flow, reward buttons, customization sliders); when adding UI, wire through UpdateUserInfoDisplay/UpdateCustomizationFromUser to keep HUD consistent.
- GameUtilities.cs + GameConstants.cs centralize thresholds, formatting, and limits; extend these instead of inlining new numbers so tests remain predictable.

## Tests & Validation
- Assets/Scripts/Tests contains NUnit PlayMode tests (see UserProfileTests.cs, GameUtilitiesTests.cs); run them in Unity Test Runner or via `Unity.exe -runTests -testResults results.xml -projectPath <repo>`.
- When touching persistence or calculators, add coverage inside Tests.asmdef to avoid regressions on serialized data.
- Blender Python validation:
  - Syntax check: `python -m compileall Blender/` (or use VSCode Task "Blender: Validate All Scripts")
  - Unit tests: `blender --background --python Blender/run_tests.py` (or use VSCode Task "Blender: Run Unit Tests")

## Blender Workflow
- Run Blender/startup_script.py on open to register project paths, 60 fps scene settings, camera, lighting, and collection hierarchy; without it imports from Assets/Scripts fail.
- character_controller.py mirrors Unity's CharacterController features (eye scaling, happiness, Thai gestures); keep method names consistent so documentation stays aligned.
- minime_addon.py exposes tooling in Blender's "Mini-Me" panel; extend it instead of scattering ad-hoc operators.
- export_character.py contains shared export logic used by both CLI and Addon; modify `export_character_logic()` to update export settings for all tools.
- **VSCode Integration**: Use `.vscode/tasks.json` for automated exports (`Ctrl+Shift+P` → Run Task → "Blender: Export Leandi Character") or run `npm run blender:watch` for auto-export on .blend file saves.
- **Debugging**: Install `debugpy` in Blender's Python, add `enable_debugging()` to scripts, run in Blender, then `F5` in VSCode → "Blender: Attach Debugger". See `Docs/BLENDER_VSCODE_INTEGRATION.md` for complete setup.

## Developer Workflow
- Install Blender 5.0.0 + Python 3.11+, keep Unity Hub targeting 2022.3 LTS; verify with `python --version` and Unity splash before editing.
- Use `git --no-pager status|diff|log` (avoids pager hangs) before/after work; repository often has partially staged art so isolate changes carefully.
- Preferred iteration loop: 1) update Blender assets, 2) export into Assets/Characters/…, 3) use Unity to hook them into prefabs/scenes (MainScene.unity), 4) run PlayMode tests, 5) document prompts in History2.md if AI assisted.

## Design Principles
- Educational gameplay forbids timers/fail states; reward homework completion with happiness/coins as implemented in UserProfile.CompleteHomework().
- Thai cultural gestures (wai/curtsy/bow) already have animation entry points; reuse them when building new interactions to stay respectful.
- Minime Universe side games feed resources back through UserProfile.AddCoins/AddExperience; never bypass these helpers or coins/XP will desync from analytics.

## Reference Material
- README.md and Docs/SETUP_NOTES.md describe the broader vision; SangsomMini-Me.mdc tracks the current spec; History2.md logs previous AI sessions—consult these before large shifts.
