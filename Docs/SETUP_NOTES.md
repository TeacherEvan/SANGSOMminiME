# Sangsom Mini-Me - Setup Guide

## Quick Start (5 Minutes)

### Prerequisites

- [Unity 2022.3.12f1 LTS](https://unity.com/download) - Primary game engine
- [Blender 5.0.0](https://www.blender.org/download/) or later - Asset creation
- [VSCode](https://code.visualstudio.com/) (recommended for C# and Python development)
- Python 3.11+ (bundled with Blender)

### First-Time Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/yourusername/SANGSOMminiME.git
   cd SANGSOMminiME
   ```

2. **Open Unity Project**
   - Open Unity Hub
   - Click "Add" → Select SANGSOMminiME folder
   - Unity Hub will detect version 2022.3.12f1
   - Click to open the project
   - Navigate to Assets/Scenes/ and open **MainScene.unity**

3. **Setup Blender for Asset Creation**
   - Open Blender 5.0.0
   - Go to `Scripting` workspace (top menu)
   - Click `Open` and select `Blender/startup_script.py`
   - Click `Run Script` (▶ button)

4. **Install Mini-Me Blender Addon** (Optional but Recommended)
   - Go to `Edit > Preferences > Add-ons`
   - Click `Install...`
   - Select `Blender/minime_addon.py`
   - Enable "Sangsom Mini-Me Tools"
   - Save Preferences

5. **Verify Setup**
   - **Unity**: Press Play button in Unity Editor - should see login screen
   - **Blender**: Look for "Mini-Me" tab in the 3D View sidebar (press `N` to toggle)
   - Check that project collections are created (Characters, Environment, etc.)

---

## Project Structure

```
SANGSOMminiME/
├── Assets/                     # Unity project assets
│   ├── 3rdParty/              # External Unity packages
│   ├── Art/                   # Art assets by type
│   │   ├── Animation/         # Animation clips (Unity Animator)
│   │   ├── Audio/            # Sound effects & music
│   │   ├── Materials/        # Unity materials
│   │   ├── Models/           # 3D models (FBX from Blender)
│   │   ├── Textures/         # Texture files
│   │   └── ...
│   ├── Characters/            # Per-character folders
│   │   └── Leandi/           # Test character
│   │       ├── Photos/       # Reference images for Blender
│   │       └── Models/       # Exported FBX files from Blender
│   ├── Data/                 # ScriptableObject data
│   ├── Minime-Universe/      # Educational mini-games
│   ├── Prefabs/              # Unity prefabs
│   ├── Resources/            # Runtime loadable assets
│   ├── Scenes/               # Unity scenes (MainScene.unity)
│   ├── Scripts/              # C# Unity scripts
│   │   ├── Runtime/          # Game logic (C#)
│   │   │   ├── Core/         # GameManager, UserManager
│   │   │   ├── Character/    # CharacterController
│   │   │   ├── UI/           # GameUI, LoginUI
│   │   │   └── Educational/  # EducationalAnalytics
│   │   ├── Editor/           # Unity editor tools
│   │   └── Tests/            # NUnit tests (Unity Test Runner)
│   └── Settings/             # Unity project settings
├── Blender/                   # Blender-specific Python scripts
│   ├── startup_script.py     # Project initialization
│   ├── character_controller.py # Character system
│   ├── user_manager.py       # User profiles (mirrors Unity)
│   ├── export_character.py   # Export to Unity (FBX/GLB)
│   └── minime_addon.py       # Blender addon
├── Docs/                      # Documentation
│   ├── SETUP_NOTES.md        # This file
│   └── EXTENSIONS_AND_TOOLS.md
├── ProjectSettings/           # Unity project configuration
│   └── ProjectVersion.txt    # Unity 2022.3.12f1
├── Packages/                  # Unity Package Manager
├── .vscode/                   # VSCode rules
│   └── rules/                # AI development guidelines
├── README.md                  # Project overview
├── IMPLEMENTATION.md          # Implementation details
├── JOBCARD.md                # Development progress
└── SangsomMini-Me.mdc        # Project specification
```

---

## Development Workflows

### Creating a New Character

1. **Prepare Reference Photos**
   - Place 2-3 photos in `Assets/Characters/[Name]/Photos/`
   - Include: front view, side view, and expression reference

2. **Generate Character Base**

   ```
   Option A: Use Mini-Me addon
   - Open Mini-Me panel (3D View > Sidebar > Mini-Me)
   - Set character name
   - Click "Create Character Template"
   
   Option B: Use external tools
   - Generate base mesh with MB-Lab or similar
   - Import into Blender
   ```

3. **Stylize for Anime Look**
   - Apply toon shader material
   - Adjust proportions (larger eyes, etc.)
   - Add hair and accessories

4. **Rig the Character**
   - Use Rigify or Auto-Rig Pro
   - Ensure all required bones are present
   - Test basic movements

5. **Create Animations**
   - Required: idle, dance, wave, wai, curtsy, bow
   - Use Mini-Me addon to create placeholders
   - Import from Mixamo or create manually

6. **Export to Unity**
   - In Blender, select character and animations
   - Run `export_character.py` or use Mini-Me addon export button
   - Choose FBX format for Unity compatibility
   - Export to `Assets/Characters/[Name]/Models/`

7. **Import into Unity**
   - Unity will auto-import FBX files
   - Configure import settings (rig type: Humanoid)
   - Create Unity Animator Controller
   - Set up animation states and transitions

### Scripting with VSCode

1. **Open Project in VSCode**

   ```bash
   code /path/to/SANGSOMminiME
   ```

2. **AI Rules are Auto-Loaded**
   - Rules in `.vscode/rules/` configure AI behavior
   - Separate rules for Unity C# and Blender Python
   - Follow established SangsomMiniMe namespace conventions

3. **Example Prompts**

   ```
   "Create a Unity C# happiness decay system that reduces character happiness by 1% per hour of inactivity"
   
   "Generate a Blender Python script to batch import all photos from a folder as reference images"
   
   "Write a Unity C# function to save user profile data using JsonUtility"
   
   "Create a Blender Python function to export character with all animations in FBX format for Unity"
   ```

### Testing Your Work

1. **In Unity**
   - Press Play button in Unity Editor to test gameplay
   - Use Unity Test Runner (Window > General > Test Runner)
   - Run NUnit tests in Assets/Scripts/Tests/
   - Use Unity Profiler to check performance (Window > Analysis > Profiler)

2. **In Blender**
   - Use the Mini-Me panel to test animations
   - Check Python console for errors (Window > Toggle System Console)
   - Render preview to verify appearance

2. **Unit Tests** (if applicable)
   - Unity: Use Test Runner (Window > General > Test Runner)
   - Blender Python: `python -m pytest Assets/Scripts/Tests/` (if pytest configured)

---

## Common Tasks

### Import Character Reference Photos

```python
# Run in Blender Scripting workspace
import bpy
import os

photo_dir = "//Assets/Characters/Leandi/Photos/"
abs_path = bpy.path.abspath(photo_dir)

for filename in os.listdir(abs_path):
    if filename.lower().endswith(('.png', '.jpg', '.jpeg')):
        bpy.ops.import_image.to_plane(
            files=[{"name": filename}],
            directory=abs_path
        )
print("Photos imported!")
```

### Export Character for Unity

```python
# In Blender, select character and run:
import bpy
from export_character import export_character_logic

# Export to Unity-compatible FBX
export_character_logic(
    character_name="Leandi",
    output_path="//Assets/Characters/Leandi/Models/",
    format='FBX'
)
```

Then in Unity:
1. Unity will auto-detect the FBX file
2. Select the imported model in Project window
3. In Inspector, set Rig > Animation Type to "Humanoid"
4. Click "Apply"
5. Create Animator Controller (Create > Animator Controller)
6. Drag animations into Animator window

### Create Animation Action

```python
# Create a new animation action
import bpy

action_name = "wave"
action = bpy.data.actions.new(action_name)
action.use_fake_user = True  # Prevent deletion

# Assign to armature
armature = bpy.context.active_object
if armature and armature.type == 'ARMATURE':
    if armature.animation_data is None:
        armature.animation_data_create()
    armature.animation_data.action = action
```

---

## Troubleshooting

### "Module not found" errors

```python
# Add project paths to Python
import sys
sys.path.insert(0, "/path/to/SANGSOMminiME/Blender")
```

### Addon not appearing

- Ensure Blender version is 4.0+
- Check Add-ons preferences for errors
- Try running `minime_addon.py` directly in Scripting workspace

### Performance issues

- Simplify viewport shading
- Lower subdivision levels
- Disable unnecessary overlays

### Export problems

- Apply all transforms (Ctrl+A > All Transforms)
- Check for loose vertices/edges
- Ensure proper UV mapping

---

## Resources

- [Unity Manual (2022.3 LTS)](https://docs.unity3d.com/2022.3/Documentation/Manual/)
- [Unity Scripting Reference](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/)
- [Blender Manual](https://docs.blender.org/manual)
- [Blender Python API](https://docs.blender.org/api)
- [Project README](../README.md)
- [Extension Guide](EXTENSIONS_AND_TOOLS.md)

---

## Getting Help

1. Check existing documentation
2. Search project issues on GitHub
3. Ask Cursor AI for code assistance
4. Create an issue with reproduction steps

---

*Happy developing! 🎮✨*
