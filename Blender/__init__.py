"""
Sangsom Mini-Me - Blender Python Package

This package provides core functionality for the Sangsom Mini-Me
educational tamagotchi project in Blender.

Modules:
    startup_script: Project initialization
    character_controller: Character behavior and animations
    user_manager: User profiles and authentication
    minime_addon: Blender addon with UI panels

Usage:
    # Import all modules
    from Blender import character_controller, user_manager
    
    # Or import specific classes
    from Blender.character_controller import CharacterController
    from Blender.user_manager import UserManager
"""

__version__ = "1.0.0"
__author__ = "Sangsom Mini-Me Development Team"

# Expose main classes at package level
try:
    from .character_controller import CharacterController, CharacterAnimation, MoodState
    from .user_manager import UserManager, UserProfile, get_user_manager
except ImportError:
    # Running outside Blender context
    pass

__all__ = [
    'CharacterController',
    'CharacterAnimation', 
    'MoodState',
    'UserManager',
    'UserProfile',
    'get_user_manager',
]
