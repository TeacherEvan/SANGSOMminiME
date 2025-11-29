import json
import os
from dataclasses import dataclass, asdict
from pathlib import Path
from typing import Optional

@dataclass
class GameConfigData:
    """Data structure for game configuration."""
    # System
    target_fps: int = 60

    # User Settings
    starting_coins: int = 100
    starting_happiness: float = 75.0
    starting_days_active: int = 1
    
    # Eye Customization
    min_eye_scale: float = 0.5
    max_eye_scale: float = 2.0
    
    # Homework Rewards
    homework_xp_reward: int = 10
    homework_coin_reward: int = 5
    homework_happiness_reward: float = 5.0
    
    # Happiness System
    happy_threshold: float = 70.0
    sad_threshold: float = 30.0
    dance_happiness_bonus: float = 2.0
    
    # Auto-Save
    auto_save_interval: float = 30.0
    enable_auto_save: bool = True
    
    # Animation & Levels
    animation_duration: float = 2.0
    experience_per_level: int = 100

class GameConfiguration:
    """
    Manages game configuration settings.
    Singleton class to ensure consistent settings across the application.
    """
    _instance = None
    
    @classmethod
    def get_instance(cls):
        if cls._instance is None:
            cls._instance = GameConfiguration()
        return cls._instance
    
    def __init__(self):
        if GameConfiguration._instance is not None:
            raise Exception("This class is a singleton!")
        
        self.data = GameConfigData()
        self.config_path = self._get_config_path()
        self.load_config()
    
    def _get_config_path(self) -> Path:
        """Get the path to the configuration file."""
        # Assuming this file is in Blender/
        root_dir = Path(__file__).parent
        data_dir = root_dir / "Data"
        data_dir.mkdir(exist_ok=True)
        return data_dir / "config.json"
    
    def load_config(self):
        """Load configuration from JSON file."""
        if not self.config_path.exists():
            print(f"Config file not found at {self.config_path}, using defaults.")
            self.save_config() # Save defaults
            return

        try:
            with open(self.config_path, 'r') as f:
                data_dict = json.load(f)
                # Update dataclass with loaded values, ignoring unknown keys
                for key, value in data_dict.items():
                    if hasattr(self.data, key):
                        setattr(self.data, key, value)
            print(f"Configuration loaded from {self.config_path}")
        except Exception as e:
            print(f"Error loading config: {e}. Using defaults.")
    
    def save_config(self):
        """Save current configuration to JSON file."""
        try:
            with open(self.config_path, 'w') as f:
                json.dump(asdict(self.data), f, indent=4)
            print(f"Configuration saved to {self.config_path}")
        except Exception as e:
            print(f"Error saving config: {e}")

    def __getattr__(self, name):
        """Delegate attribute access to the data object."""
        if hasattr(self.data, name):
            return getattr(self.data, name)
        raise AttributeError(f"'{type(self).__name__}' object has no attribute '{name}'")

    def __setattr__(self, name, value):
        """Delegate attribute setting to the data object if it exists there."""
        if name == "data" or name == "config_path":
            super().__setattr__(name, value)
        elif hasattr(self.data, name):
            setattr(self.data, name, value)
        else:
            super().__setattr__(name, value)

# Global accessor
def get_game_config():
    return GameConfiguration.get_instance()
