# Sangsom Mini-Me - Setup Guide

## Quick Start (5 Minutes)

### Prerequisites

- [Unity Hub](https://unity.com/download) with **Unity 6000.2.15f1**
- [VSCode](https://code.visualstudio.com/) (recommended for C# development)
- Git for version control

### First-Time Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/TeacherEvan/SANGSOMminiME.git
   cd SANGSOMminiME
   ```

2. **Open Unity Project**

   - Open Unity Hub
   - Click "Add" → Select SANGSOMminiME folder
   - Unity Hub will detect version 6000.2.15f1
   - Click to open the project
   - Navigate to Assets/Scenes/ and open **MainScene.unity**

3. **Verify Setup**
   - Press Play button in Unity Editor - should see login screen
   - Check that no console errors appear
   - Verify UI elements are visible and interactive

---

## Project Structure

```
SANGSOMminiME/
├── Assets/                     # Unity project assets
│   ├── 3rdParty/              # External Unity packages
│   ├── Art/                   # Art assets by type
│   │   ├── Animation/         # Animation clips
│   │   ├── Audio/            # Sound effects & music
│   │   ├── Materials/        # Unity materials
│   │   ├── Models/           # 3D models (FBX)
│   │   ├── Textures/         # Texture files
│   │   └── ...
│   ├── Characters/            # Per-character folders
│   │   └── Leandi/           # Test character
│   │       └── Photos/       # Reference images
│   ├── Data/                 # ScriptableObject data
│   ├── Minime-Universe/      # Educational mini-games
│   │   ├── Core-Game/        # Main tamagotchi systems
│   │   └── Side-Games/       # Educational mini-games
│   ├── Prefabs/              # Unity prefabs
│   ├── Resources/            # Runtime loadable assets
│   │   ├── Outfits/          # Purchasable clothing
│   │   └── Accessories/      # Hats, jewelry, items
│   ├── Scenes/               # Unity scenes
│   │   └── MainScene.unity   # Primary game scene
│   ├── Scripts/              # C# Unity scripts
│   │   ├── Runtime/          # Game logic
│   │   │   ├── GameManager.cs
│   │   │   ├── UserManager.cs
│   │   │   ├── CharacterController.cs
│   │   │   ├── GameUI.cs
│   │   │   └── ...
│   │   ├── Editor/           # Unity editor tools
│   │   └── Tests/            # NUnit PlayMode tests
│   └── Settings/             # Unity project settings
├── Docs/                      # Documentation
│   ├── SETUP_NOTES.md        # This file
│   └── EXTENSIONS_AND_TOOLS.md
├── ProjectSettings/           # Unity project configuration
│   └── ProjectVersion.txt    # Unity 6000.2.15f1
├── Packages/                  # Unity Package Manager
├── .vscode/                   # VSCode configuration
├── README.md                  # Project overview
├── IMPLEMENTATION.md          # Implementation details
├── JOBCARD.md                # Development progress
└── SangsomMini-Me.mdc        # Project specification
```

---

## Development Workflows

### Scripting with VSCode

1. **Open Project in VSCode**

   ```bash
   code /path/to/SANGSOMminiME
   ```

2. **Install Recommended Extensions**

   - C# (ms-dotnettools.csharp)
   - Unity Code Snippets
   - EditorConfig for VS Code

3. **AI Rules are Auto-Loaded**

   - Rules in `.vscode/rules/` configure AI behavior
   - Follow established SangsomMiniMe namespace conventions

4. **Example Prompts**

   ```
   "Create a Unity C# happiness decay system that reduces character happiness by 1% per hour of inactivity"

   "Write a Unity C# function to save user profile data using JsonUtility"

   "Create an animation state machine for character gestures"
   ```

### Testing Your Work

1. **PlayMode Testing**

   - Press Play button in Unity Editor to test gameplay
   - Interact with UI elements to verify functionality
   - Check Console window for any errors

2. **Unit Tests**

   - Open Test Runner: Window > General > Test Runner
   - Click "Run All" to execute all NUnit tests
   - Tests are located in Assets/Scripts/Tests/

3. **Performance Testing**
   - Open Profiler: Window > Analysis > Profiler
   - Check for frame rate drops and memory issues
   - Target 60fps on mobile devices

---

## Core Systems Overview

### GameManager.cs

- Singleton pattern for global game state
- Orchestrates login flow and UI transitions
- Manages autosave functionality

### UserManager.cs

- User profile persistence via JsonUtility
- Saves to Application.persistentDataPath
- Handles multiple user accounts

### CharacterController.cs

- Character animations and customization
- Eye scaling and outfit changes
- Animation triggers: PlayDance, Wai, Curtsy, Bow

### GameUI.cs

- TextMeshPro-based UI system
- Homework flow and reward buttons
- Customization sliders

### EducationalAnalytics.cs

- Homework completion tracking
- Achievement and reward system
- Progress analytics

---

## Common Tasks

### Creating a New Character

1. **Prepare Reference Photos**

   - Place 2-3 photos in `Assets/Characters/[Name]/Photos/`
   - Include: front view, side view, and expression reference

2. **Import 3D Model**

   - Import FBX model into `Assets/Characters/[Name]/Models/`
   - Configure import settings: Rig Type = Humanoid
   - Apply import settings

3. **Create Animator Controller**

   - Right-click in Project: Create > Animator Controller
   - Set up animation states for idle, dance, wave, wai, curtsy, bow
   - Configure transitions between states

4. **Create Character Prefab**
   - Drag model into scene
   - Add CharacterController component
   - Configure customization options
   - Save as prefab in Assets/Prefabs/

### Adding New Customization Options

1. **Outfits**

   - Add outfit models to `Assets/Resources/Outfits/`
   - Register in customization system
   - Update purchasable items list

2. **Accessories**
   - Add accessory models to `Assets/Resources/Accessories/`
   - Define attachment points on character rig
   - Update accessory catalog

---

## Troubleshooting

### Unity project won't open

- Ensure Unity 6000.2.15f1 is installed via Unity Hub
- Delete Library folder and reimport project
- Check Unity Hub for any error messages

### Scripts not compiling

- Check Console for compilation errors
- Verify namespace matches file location
- Ensure assembly definitions are properly configured

### UI not displaying correctly

- Check Canvas Scaler settings
- Verify TextMeshPro essentials are imported
- Check RectTransform anchors

### Performance issues

- Use Unity Profiler to identify bottlenecks
- Implement object pooling for frequently instantiated objects
- Optimize texture sizes and mesh complexity

---

## Resources

- [Unity Manual (2022.3 LTS)](https://docs.unity3d.com/2022.3/Documentation/Manual/)
- [Unity Scripting Reference](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/)
- [TextMeshPro Documentation](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html)
- [Project README](../README.md)
- [Extension Guide](EXTENSIONS_AND_TOOLS.md)

---

## Getting Help

1. Check existing documentation
2. Search project issues on GitHub
3. Ask AI assistant for code assistance
4. Create an issue with reproduction steps

---

_Happy developing! 🎮✨_
