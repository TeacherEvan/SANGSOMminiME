# Sangsom Mini-Me - Setup Guide

## Quick Start (5 Minutes)

### Prerequisites

- [Blender 5.0.0](https://www.blender.org/download/) or later
- [VSCode](https://code.visualstudio.com/) (recommended for development)
- Python 3.11+ (bundled with Blender)

### First-Time Setup

1. **Clone the Repository**

   ```bash
   git clone https://github.com/yourusername/SANGSOMminiME.git
   cd SANGSOMminiME
   ```

2. **Open Blender and Load Startup Script**
   - Open Blender 5.0.0
   - Go to `Scripting` workspace (top menu)
   - Click `Open` and select `Blender/startup_script.py`
   - Click `Run Script` (▶ button)

3. **Install Mini-Me Addon** (Optional but Recommended)
   - Go to `Edit > Preferences > Add-ons`
   - Click `Install...`
   - Select `Blender/minime_addon.py`
   - Enable "Sangsom Mini-Me Tools"
   - Save Preferences

4. **Verify Setup**
   - Look for "Mini-Me" tab in the 3D View sidebar (press `N` to toggle)
   - Check that project collections are created (Characters, Environment, etc.)

---

## Project Structure

```
SANGSOMminiME/
├── Assets/                     # All project assets
│   ├── 3rdParty/              # External dependencies
│   ├── Art/                   # Art assets by type
│   │   ├── Animation/         # Animation clips
│   │   ├── Audio/            # Sound effects & music
│   │   ├── Materials/        # Material definitions
│   │   ├── Models/           # 3D models
│   │   ├── Textures/         # Texture files
│   │   └── ...
│   ├── Characters/            # Per-character folders
│   │   └── Leandi/           # Test character
│   │       └── Photos/       # Reference images
│   ├── Data/                 # Data configurations
│   ├── Minime-Universe/      # Educational mini-games
│   ├── Prefabs/              # Reusable objects
│   ├── Resources/            # Runtime loadable assets
│   ├── Scenes/               # Blender scenes
│   ├── Scripts/              # C#/Python code
│   │   ├── Editor/           # Editor tools
│   │   ├── Runtime/          # Game logic
│   │   └── Tests/            # Unit tests
│   └── Settings/             # Configuration files
├── Blender/                   # Blender-specific files
│   ├── startup_script.py     # Project initialization
│   ├── character_controller.py # Character system
│   ├── user_manager.py       # User profiles
│   └── minime_addon.py       # Blender addon
├── Docs/                      # Documentation
│   ├── SETUP_NOTES.md        # This file
│   └── EXTENSIONS_AND_TOOLS.md
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

### Scripting with VSCode

1. **Open Project in VSCode**

   ```bash
   code /path/to/SANGSOMminiME
   ```

2. **AI Rules are Auto-Loaded**
   - Rules in `.vscode/rules/` configure AI behavior
   - Follow established patterns and conventions

3. **Example Prompts**

   ```
   "Create a happiness decay system that reduces character happiness by 1% per hour of inactivity"
   
   "Generate a Blender script to batch import all photos from a folder as reference images"
   
   "Write a function to export character with all animations in GLB format"
   ```

### Testing Your Work

1. **In Blender**
   - Use the Mini-Me panel to test animations
   - Check Python console for errors (Window > Toggle System Console)
   - Render preview to verify appearance

2. **Unit Tests** (if applicable)
   - Run tests for core systems
   - Python: `python -m pytest Assets/Scripts/Tests/`

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

### Export Character for Web

```python
# Export selected character as GLB
import bpy

bpy.ops.export_scene.gltf(
    filepath="//exports/character.glb",
    export_format='GLB',
    export_animations=True,
    export_materials='EXPORT',
    use_selection=True
)
```

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
