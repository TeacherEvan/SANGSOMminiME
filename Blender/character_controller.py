"""
Sangsom Mini-Me - Character Controller for Blender

This module handles character behavior, animations, and customization
within the Blender environment.

Usage:
    from character_controller import CharacterController
    
    controller = CharacterController("Leandi")
    controller.play_animation("dance")
    controller.set_eye_scale(1.5)
"""

import bpy
from typing import Optional, List, Dict, Callable
from enum import Enum
from dataclasses import dataclass


class CharacterAnimation(Enum):
    """Character animation types supported by the Mini-Me system."""
    IDLE = "idle"
    DANCE = "dance"
    WAVE = "wave"
    WAI = "wai"          # Thai traditional greeting gesture
    CURTSY = "curtsy"
    BOW = "bow"


class MoodState(Enum):
    """Mood states for character happiness display."""
    VERY_HAPPY = "very_happy"    # 80-100 happiness
    HAPPY = "happy"              # 60-80 happiness
    NEUTRAL = "neutral"          # 40-60 happiness
    SAD = "sad"                  # 20-40 happiness
    VERY_SAD = "very_sad"        # 0-20 happiness


@dataclass
class CharacterConfig:
    """Configuration for character customization."""
    eye_scale: float = 1.0
    outfit: str = "default"
    accessory: str = "none"
    happiness: float = 75.0
    
    # Limits
    MIN_EYE_SCALE: float = 0.5
    MAX_EYE_SCALE: float = 2.0
    MIN_HAPPINESS: float = 0.0
    MAX_HAPPINESS: float = 100.0


class CharacterController:
    """
    Controls the Mini-Me character behavior and interactions in Blender.
    
    This class manages:
    - Character animations
    - Customization (eye scale, outfits, accessories)
    - Happiness and mood system
    - Event callbacks
    
    Attributes:
        character_name: Name of the character
        armature: Blender armature object for the character
        config: Character configuration settings
    """
    
    def __init__(self, character_name: str):
        """
        Initialize the character controller.
        
        Args:
            character_name: Name of the character to control
        """
        self.character_name = character_name
        self.config = CharacterConfig()
        self.armature: Optional[bpy.types.Object] = None
        self.mesh: Optional[bpy.types.Object] = None
        self._is_animating = False
        
        # Event callbacks
        self._on_animation_started: List[Callable] = []
        self._on_animation_completed: List[Callable] = []
        self._on_happiness_changed: List[Callable] = []
        
        self._find_character_objects()
    
    def _find_character_objects(self):
        """Find the character's armature and mesh objects in the scene."""
        # Look for armature
        armature_name = f"{self.character_name}_Armature"
        if armature_name in bpy.data.objects:
            self.armature = bpy.data.objects[armature_name]
        else:
            # Try to find any armature with the character name
            for obj in bpy.data.objects:
                if obj.type == 'ARMATURE' and self.character_name.lower() in obj.name.lower():
                    self.armature = obj
                    break
        
        # Look for mesh
        mesh_name = f"{self.character_name}_Mesh"
        if mesh_name in bpy.data.objects:
            self.mesh = bpy.data.objects[mesh_name]
        else:
            # Try to find any mesh with the character name
            for obj in bpy.data.objects:
                if obj.type == 'MESH' and self.character_name.lower() in obj.name.lower():
                    self.mesh = obj
                    break
    
    @property
    def is_animating(self) -> bool:
        """Check if the character is currently animating."""
        return self._is_animating
    
    @property
    def happiness(self) -> float:
        """Get current happiness level."""
        return self.config.happiness
    
    @property
    def mood(self) -> MoodState:
        """Get current mood state based on happiness."""
        if self.config.happiness >= 80:
            return MoodState.VERY_HAPPY
        elif self.config.happiness >= 60:
            return MoodState.HAPPY
        elif self.config.happiness >= 40:
            return MoodState.NEUTRAL
        elif self.config.happiness >= 20:
            return MoodState.SAD
        else:
            return MoodState.VERY_SAD
    
    def play_animation(self, animation: CharacterAnimation) -> bool:
        """
        Play a character animation.
        
        Args:
            animation: The animation to play
            
        Returns:
            True if animation started successfully, False otherwise
        """
        if self._is_animating:
            return False
        
        if self.armature is None:
            print(f"Warning: No armature found for character '{self.character_name}'")
            return False
        
        # Look for the animation action
        action_name = f"{self.character_name}_{animation.value}"
        action = bpy.data.actions.get(action_name)
        
        if action is None:
            # Try without character name prefix
            action = bpy.data.actions.get(animation.value)
        
        if action is None:
            print(f"Warning: Animation '{animation.value}' not found")
            return False
        
        # Set the animation
        if self.armature.animation_data is None:
            self.armature.animation_data_create()
        
        self.armature.animation_data.action = action
        self._is_animating = True
        
        # Trigger callbacks
        for callback in self._on_animation_started:
            callback(animation.value)
        
        # Special effects for certain animations
        if animation == CharacterAnimation.DANCE:
            self.increase_happiness(2.0)
        
        print(f"Playing animation: {animation.value}")
        return True
    
    def play_idle(self):
        """Play idle animation."""
        self.play_animation(CharacterAnimation.IDLE)
    
    def play_dance(self):
        """Play dance animation."""
        self.play_animation(CharacterAnimation.DANCE)
    
    def play_wave(self):
        """Play wave animation."""
        self.play_animation(CharacterAnimation.WAVE)
    
    def play_wai(self):
        """Play Thai wai greeting animation."""
        self.play_animation(CharacterAnimation.WAI)
    
    def play_curtsy(self):
        """Play curtsy animation."""
        self.play_animation(CharacterAnimation.CURTSY)
    
    def play_bow(self):
        """Play bow animation."""
        self.play_animation(CharacterAnimation.BOW)
    
    def set_eye_scale(self, scale: float):
        """
        Set the eye scale for the character.
        
        Args:
            scale: Eye scale value (clamped between 0.5 and 2.0)
        """
        # Clamp value
        scale = max(self.config.MIN_EYE_SCALE, min(scale, self.config.MAX_EYE_SCALE))
        self.config.eye_scale = scale
        
        # Apply to bones if armature exists
        if self.armature:
            for bone_name in ["eye_L", "eye_R", "Eye.L", "Eye.R"]:
                if bone_name in self.armature.pose.bones:
                    bone = self.armature.pose.bones[bone_name]
                    bone.scale = (scale, scale, scale)
        
        print(f"Eye scale set to: {scale}")
    
    def set_outfit(self, outfit_name: str):
        """
        Set the character's outfit.
        
        Args:
            outfit_name: Name of the outfit to apply
        """
        self.config.outfit = outfit_name
        
        # Apply material if mesh exists
        if self.mesh:
            material_name = f"outfit_{outfit_name}"
            material = bpy.data.materials.get(material_name)
            
            if material and len(self.mesh.material_slots) > 0:
                self.mesh.material_slots[0].material = material
        
        print(f"Outfit set to: {outfit_name}")
    
    def set_accessory(self, accessory_name: str):
        """
        Set the character's accessory.
        
        Args:
            accessory_name: Name of the accessory ("none" to remove)
        """
        self.config.accessory = accessory_name
        
        # Look for accessory objects
        for obj in bpy.data.objects:
            if "accessory" in obj.name.lower():
                # Hide all accessories first
                obj.hide_viewport = True
                obj.hide_render = True
                
                # Show the selected accessory
                if accessory_name != "none" and accessory_name.lower() in obj.name.lower():
                    obj.hide_viewport = False
                    obj.hide_render = False
        
        print(f"Accessory set to: {accessory_name}")
    
    def set_happiness(self, happiness: float):
        """
        Set the character's happiness level.
        
        Args:
            happiness: Happiness value (clamped between 0 and 100)
        """
        # Clamp value
        old_happiness = self.config.happiness
        self.config.happiness = max(
            self.config.MIN_HAPPINESS, 
            min(happiness, self.config.MAX_HAPPINESS)
        )
        
        # Trigger callbacks if changed
        if old_happiness != self.config.happiness:
            for callback in self._on_happiness_changed:
                callback(self.config.happiness)
    
    def increase_happiness(self, amount: float):
        """Increase happiness by the specified amount."""
        self.set_happiness(self.config.happiness + amount)
    
    def decrease_happiness(self, amount: float):
        """Decrease happiness by the specified amount."""
        self.set_happiness(self.config.happiness - amount)
    
    def on_animation_started(self, callback: Callable):
        """Register a callback for animation start events."""
        self._on_animation_started.append(callback)
    
    def on_animation_completed(self, callback: Callable):
        """Register a callback for animation completion events."""
        self._on_animation_completed.append(callback)
    
    def on_happiness_changed(self, callback: Callable):
        """Register a callback for happiness change events."""
        self._on_happiness_changed.append(callback)
    
    def complete_homework(self):
        """
        Handle homework completion event.
        Increases happiness and triggers celebration.
        """
        self.increase_happiness(10.0)
        if not self._is_animating:
            self.play_dance()
        print("Homework completed! Character is happy!")
    
    def get_status(self) -> Dict:
        """
        Get the current status of the character.
        
        Returns:
            Dictionary with character status information
        """
        return {
            "name": self.character_name,
            "happiness": self.config.happiness,
            "mood": self.mood.value,
            "eye_scale": self.config.eye_scale,
            "outfit": self.config.outfit,
            "accessory": self.config.accessory,
            "is_animating": self._is_animating,
            "has_armature": self.armature is not None,
            "has_mesh": self.mesh is not None
        }
    
    def __str__(self) -> str:
        return f"CharacterController({self.character_name}, happiness={self.happiness}, mood={self.mood.value})"


# Convenience function to create a controller for the test character
def create_leandi_controller() -> CharacterController:
    """Create a character controller for the Leandi test character."""
    return CharacterController("Leandi")


# Example usage when run directly
if __name__ == "__main__":
    print("Creating character controller...")
    controller = create_leandi_controller()
    print(controller.get_status())
