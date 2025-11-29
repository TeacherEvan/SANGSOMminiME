# Blender Configuration System Guide

This guide explains how to use and modify the configuration system for the Sangsom Mini-Me Blender environment.

## Overview

The configuration system allows you to tweak game balance, rewards, and system settings without modifying the Python code. Settings are stored in a JSON file and loaded when the Blender scripts start.

## Configuration File

The configuration file is located at:
`Blender/Data/config.json`

If this file does not exist, it will be automatically created with default values when you run the game/scripts.

### Example Configuration

```json
{
    "target_fps": 60,
    "starting_coins": 100,
    "starting_happiness": 75.0,
    "starting_days_active": 1,
    "min_eye_scale": 0.5,
    "max_eye_scale": 2.0,
    "homework_xp_reward": 10,
    "homework_coin_reward": 5,
    "homework_happiness_reward": 5.0,
    "happy_threshold": 70.0,
    "sad_threshold": 30.0,
    "dance_happiness_bonus": 2.0,
    "auto_save_interval": 30.0,
    "enable_auto_save": true,
    "animation_duration": 2.0,
    "experience_per_level": 100
}
```

## Modifying Settings

1.  Open `Blender/Data/config.json` in VS Code or any text editor.
2.  Change the values as desired.
3.  Save the file.
4.  Restart the Blender script or reload the scripts in Blender to apply changes.

## Using Configuration in Code

To access configuration values in your Python scripts:

1.  Import the `get_game_config` function.
2.  Get the instance.
3.  Access properties directly.

```python
from game_configuration import get_game_config

def my_function():
    config = get_game_config()
    
    # Access settings
    print(f"Target FPS: {config.target_fps}")
    
    # Use in logic
    if current_happiness > config.happy_threshold:
        play_happy_animation()
```

## Configuration Reference

| Key | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `target_fps` | int | 60 | Target frames per second for the game loop. |
| `starting_coins` | int | 100 | Coins given to a new user. |
| `starting_happiness` | float | 75.0 | Initial happiness level (0-100). |
| `min_eye_scale` | float | 0.5 | Minimum allowed scale for eyes. |
| `max_eye_scale` | float | 2.0 | Maximum allowed scale for eyes. |
| `homework_xp_reward` | int | 10 | XP earned for completing homework. |
| `homework_coin_reward` | int | 5 | Coins earned for completing homework. |
| `homework_happiness_reward` | float | 5.0 | Happiness gained from homework. |
| `happy_threshold` | float | 70.0 | Happiness level required to be "Happy". |
| `sad_threshold` | float | 30.0 | Happiness level below which is "Sad". |
| `dance_happiness_bonus` | float | 2.0 | Happiness gained from dancing. |
| `auto_save_interval` | float | 30.0 | Time in seconds between auto-saves. |
| `enable_auto_save` | bool | true | Whether auto-save is enabled. |
| `animation_duration` | float | 2.0 | Default duration for animations. |
| `experience_per_level` | int | 100 | XP required to level up. |
