# Quick Start Guide

## Prerequisites

- **Unity Hub** with Unity 2022.3.12f1 LTS installed
- **Visual Studio Code** with C# extensions

## Getting Started

1. **Open the project** in Unity Hub.
2. **Wait for import** - Unity will compile scripts and import assets.
3. **Open MainScene** - Navigate to `Assets/Scenes/MainScene.unity`.
4. **Press Play** - Test the game in the Editor.

## Debug Controls (Play Mode)

| Key | Action                  |
| --- | ----------------------- |
| F1  | Create test user        |
| F2  | Add 100 coins           |
| F3  | Complete homework       |
| F4  | Trigger dance animation |

## Running Tests

1. Open **Window > General > Test Runner**.
2. Select **PlayMode** tab.
3. Click **Run All** to execute tests.

## Project Structure

```
Assets/
├── Scenes/          # Unity scenes
├── Scripts/         # C# source code
│   ├── Runtime/     # Game logic
│   ├── Editor/      # Editor tools
│   └── Tests/       # Unit tests
├── Prefabs/         # Reusable game objects
└── Resources/       # Runtime-loadable assets
```

## Next Steps

- Review `README.md` for full documentation
- Check `Docs/SETUP_NOTES.md` for detailed setup
- Run tests to verify everything works
