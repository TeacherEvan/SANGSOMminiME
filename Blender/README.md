# Blender Scripts for Sangsom Mini-Me

This folder contains Blender Python scripts for the Sangsom Mini-Me educational tamagotchi project.

## Contents

| File | Description |
|------|-------------|
| `startup_script.py` | Project initialization script - run this first |
| `character_controller.py` | Character behavior and animation control |
| `user_manager.py` | User profile and authentication system |
| `minime_addon.py` | Blender addon with UI panels and tools |

## Quick Start

### Option 1: Run Startup Script
1. Open Blender 5.0.0
2. Go to `Scripting` workspace
3. Open `startup_script.py`
4. Click "Run Script" (▶ button)

### Option 2: Install as Addon
1. `Edit > Preferences > Add-ons`
2. Click "Install..."
3. Select `minime_addon.py`
4. Enable "Sangsom Mini-Me Tools"

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

# Create controller for character
controller = CharacterController("Leandi")

# Play animations
controller.play_dance()
controller.play_wave()
controller.play_wai()  # Thai greeting

# Customize
controller.set_eye_scale(1.5)
controller.set_outfit("school_uniform")

# Track happiness
controller.increase_happiness(10)
print(controller.mood)  # MoodState.HAPPY
```

### User Manager

```python
from user_manager import UserManager

# Get singleton instance
manager = UserManager.get_instance()

# Create user
user = manager.create_user("student1", "Happy Student")

# Login
manager.login_user("student1")

# Track progress
user.complete_homework()  # Gives XP, coins, happiness
print(f"Level: {user.get_level()}")
```

### Export Character

```python
# Run from CLI
# blender --background character.blend --python export_character.py

# Or import logic in other scripts
from export_character import export_character_logic
export_character_logic(character_name="Leandi")
```

### Mini-Me Addon
After installation, find "Mini-Me" tab in 3D View sidebar (press N):
- **Setup Project**: Initialize scene and collections
- **Character**: Create templates, adjust customization
- **Animations**: Create and play animations
- **Export**: Export character as GLB (uses shared logic from `export_character.py`)

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

### Debugging
- Open Python console: `Window > Toggle System Console`
- Use print statements for debugging
- Check Blender's Info area for operator errors

## Requirements

- Blender 4.0+ (developed with 5.0.0)
- Python 3.11+ (bundled with Blender)

## License

Educational Use License - See project root LICENSE file.
