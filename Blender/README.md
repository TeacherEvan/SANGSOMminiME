# Blender Scripts for Sangsom Mini-Me

This folder contains Blender Python scripts for the Sangsom Mini-Me educational tamagotchi project. These scripts create 3D assets that are exported to Unity 2022.3.12f1 for the game runtime.

## Contents

| File | Description |
|------|-------------|
| `startup_script.py` | Project initialization script - run this first |
| `character_controller.py` | Character behavior and animation control (mirrors Unity C# CharacterController) |
| `user_manager.py` | User profile and authentication system (mirrors Unity C# UserManager) |
| `minime_addon.py` | Blender addon with UI panels and tools |
| `export_character.py` | Export characters to Unity-compatible FBX format |

## Quick Start

### Option 1: Run Startup Script
1. Open Blender 5.0.0
2. Go to `Scripting` workspace
3. Open `startup_script.py`
4. Click "Run Script" (▶ button)
5. This sets up the project for Unity export

### Option 2: Install as Addon
1. `Edit > Preferences > Add-ons`
2. Click "Install..."
3. Select `minime_addon.py`
4. Enable "Sangsom Mini-Me Tools"
5. Use addon to create characters and export to Unity

## Usage

### Startup Script

```python
# Sets up:
# - Project paths in Python
# - Scene settings (60fps, EEVEE)
# - Collection hierarchy
# - Basic lighting and camera
# - World background
```

### Character Controller

```python
from character_controller import CharacterController

# Create controller for character (mirrors Unity C# CharacterController)
controller = CharacterController("Leandi")

# Play animations (these will be exported to Unity)
controller.play_dance()
controller.play_wave()
controller.play_wai()  # Thai greeting

# Customize (export settings to Unity)
controller.set_eye_scale(1.5)
controller.set_outfit("school_uniform")

# Track happiness (synced with Unity data)
controller.increase_happiness(10)
print(controller.mood)  # MoodState.HAPPY
```

### User Manager

```python
from user_manager import UserManager

# Get singleton instance (mirrors Unity C# UserManager)
manager = UserManager.get_instance()

# Create user (data structure matches Unity JsonUtility format)
user = manager.create_user("student1", "Happy Student")

# Login
manager.login_user("student1")

# Track progress (synced with Unity save data)
user.complete_homework()  # Gives XP, coins, happiness
print(f"Level: {user.get_level()}")
```

### Export Character to Unity

```python
# Run from Blender CLI or script
# blender --background character.blend --python export_character.py

# Or import logic in other scripts
from export_character import export_character_logic

# Export to Unity-compatible FBX format
export_character_logic(
    character_name="Leandi",
    output_path="../Assets/Characters/Leandi/Models/",
    format='FBX'  # Unity standard format
)
```

**Unity Import Workflow:**
1. Blender exports FBX to `Assets/Characters/Leandi/Models/`
2. Unity auto-detects and imports FBX
3. In Unity Inspector: Set Rig > Animation Type to "Humanoid"
4. Create Unity Animator Controller
5. Add animations to Animator states

### Mini-Me Addon
After installation, find "Mini-Me" tab in 3D View sidebar (press N):
- **Setup Project**: Initialize scene and collections
- **Character**: Create templates, adjust customization
- **Animations**: Create and play animations
- **Export to Unity**: Export character as FBX with proper Unity settings (uses shared logic from `export_character.py`)

## File Naming Conventions

- Scripts: `snake_case.py`
- Classes: `PascalCase`
- Functions: `snake_case`
- Constants: `UPPER_SNAKE_CASE`

## Development Notes

### Adding New Animations
1. Create action in Blender
2. Name it: `character_name_animation_name` (e.g., `leandi_dance`)
3. Set "Fake User" to prevent deletion

### Extending Character Controller
1. Add new animation type to `CharacterAnimation` enum
2. Create convenience method (e.g., `play_new_animation`)
3. Update addon UI if needed
4. Export to Unity and add to Animator Controller

### Debugging
- Open Python console: `Window > Toggle System Console`
- Use print statements for debugging
- Check Blender's Info area for operator errors

## Requirements

- Blender 4.0+ (developed with 5.0.0)
- Python 3.11+ (bundled with Blender)
- Unity 2022.3.12f1 (for importing exported assets)

## Workflow

1. **Create in Blender**: Model and animate characters using these scripts
2. **Export to Unity**: Use `export_character.py` or addon to export FBX
3. **Import in Unity**: Unity auto-imports FBX files
4. **Configure in Unity**: Set up Animator Controller and test in PlayMode

## License

Educational Use License - See project root LICENSE file.
