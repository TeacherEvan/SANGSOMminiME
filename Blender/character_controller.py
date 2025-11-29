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
from game_configuration import get_game_config


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
    """
    
    def __init__(self, character_name: str):
        """
        Initialize the character controller.
        
        Args:
            character_name: Name of the character to control
        """
        self.character_name = character_name
        self.game_config = get_game_config()
        
        # State
        self._eye_scale: float = 1.0
        self._outfit: str = "default"
        self._accessory: str = "none"
        self._happiness: float = self.game_config.starting_happiness
        
        self.armature: Optional[bpy.types.Object] = None
        self.mesh: Optional[bpy.types.Object] = None
        self._is_animating = False
        
        # Event callbacks
        self._on_animation_started: List[Callable] = []
        self._on_animation_completed: List[Callable] = []
        self._on_happiness_changed: List[Callable] = []
        
        # Cache for assets
        self._accessories: Dict[str, bpy.types.Object] = {}
        self._materials: Dict[str, bpy.types.Material] = {}
        self._actions: Dict[str, bpy.types.Action] = {}
        self._bones: Dict[str, bpy.types.PoseBone] = {}
        
        self._find_character_objects()
        self._scan_assets()
    
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
    
    def _scan_assets(self):
        """Scan and cache available assets (accessories, materials, actions)."""
        # Cache accessories
        self._accessories.clear()
        for obj in bpy.data.objects:
            if "accessory" in obj.name.lower():
                # Use the part of the name after "accessory_" or just the name
                key = obj.name.lower()
                if "accessory_" in key:
                    key = key.split("accessory_")[1]
                self._accessories[key] = obj
        
        # Cache materials (outfits)
        self._materials.clear()
        for mat in bpy.data.materials:
            if "outfit" in mat.name.lower():
                key = mat.name.lower()
                if "outfit_" in key:
                    key = key.split("outfit_")[1]
                self._materials[key] = mat
                
        # Cache actions
        self._actions.clear()
        for action in bpy.data.actions:
            self._actions[action.name] = action
            # Also cache by simple name if it follows convention
            if self.character_name.lower() in action.name.lower():
                simple_name = action.name.lower().replace(f"{self.character_name.lower()}_", "")
                self._actions[simple_name] = action
        
        # Cache bones if armature exists
        self._bones.clear()
        if self.armature and self.armature.pose:
            for bone in self.armature.pose.bones:
                self._bones[bone.name] = bone
                # Also cache by simple name (e.g. "eye_L" -> "eye_l")
                self._bones[bone.name.lower()] = bone
    
    @property
    def is_animating(self) -> bool:
        """Check if the character is currently animating."""
        return self._is_animating
    
    @property
    def happiness(self) -> float:
        """Get current happiness level."""
        return self._happiness
    
    @property
    def mood(self) -> MoodState:
        """Get current mood state based on happiness."""
        if self._happiness >= self.game_config.happy_threshold + 10: # Very Happy
            return MoodState.VERY_HAPPY
        elif self._happiness >= self.game_config.happy_threshold:
            return MoodState.HAPPY
        elif self._happiness >= self.game_config.sad_threshold:
            return MoodState.NEUTRAL
        elif self._happiness >= self.game_config.sad_threshold - 10:
            return MoodState.SAD
        else:
            return MoodState.VERY_SAD
    
    def play_animation(self, animation: CharacterAnimation | str) -> bool:
        """
        Play a character animation.
        
        Args:
            animation: The animation to play (Enum or string name)
            
        Returns:
            True if animation started successfully, False otherwise
        """
        if self._is_animating:
            return False
        
        if self.armature is None:
            print(f"Warning: No armature found for character '{self.character_name}'")
            return False
        
        # Resolve animation name
        anim_value = animation.value if isinstance(animation, CharacterAnimation) else animation
        
        # Look for the animation action
        action_name = f"{self.character_name}_{anim_value}"
        
        # Try cache first
        action = self._actions.get(action_name) or self._actions.get(anim_value)
        
        if action is None:
            # Fallback to direct lookup
            action = bpy.data.actions.get(action_name) or bpy.data.actions.get(anim_value)
        
        if action is None:
            print(f"Warning: Animation '{anim_value}' not found")
            return False
        
        # Set the animation (armature check already done above)
        if self.armature:
            if self.armature.animation_data is None:
                self.armature.animation_data_create()
            
            if self.armature.animation_data:
                self.armature.animation_data.action = action
        self._is_animating = True
        
        # Trigger callbacks
        for callback in self._on_animation_started:
            callback(anim_value)
        
        # Special effects for certain animations
        if anim_value == CharacterAnimation.DANCE.value:
            self.increase_happiness(self.game_config.dance_happiness_bonus)
        
        print(f"Playing animation: {anim_value}")
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
        scale = max(self.game_config.min_eye_scale, min(scale, self.game_config.max_eye_scale))
        self._eye_scale = scale
        
        # Apply to bones if armature exists
        if self.armature and self.armature.pose:
            # Try cache first
            for bone_name in ["eye_L", "eye_R", "Eye.L", "Eye.R"]:
                # Try exact match or lowercase match from cache
                if bone := (self._bones.get(bone_name) or self._bones.get(bone_name.lower())):
                    bone.scale = (scale, scale, scale)
                elif bone_name in self.armature.pose.bones:
                    # Fallback to direct lookup
                    pose_bone = self.armature.pose.bones[bone_name]
                    pose_bone.scale = (scale, scale, scale)
                    # Update cache
                    self._bones[bone_name] = pose_bone
                    self._bones[bone_name.lower()] = pose_bone
        
        print(f"Eye scale set to: {scale}")
    
    def set_outfit(self, outfit_name: str):
        """
        Set the character's outfit.
        
        Args:
            outfit_name: Name of the outfit to apply
        """
        self._outfit = outfit_name
        
        # Apply material if mesh exists
        if self.mesh:
            # Try cache first
            material = self._materials.get(outfit_name.lower())
            
            if not material:
                material_name = f"outfit_{outfit_name}"
                material = bpy.data.materials.get(material_name)
            
            if material and len(self.mesh.material_slots) > 0:
                self.mesh.material_slots[0].material = material
                print(f"Outfit set to: {outfit_name}")
            else:
                print(f"Warning: Outfit material '{outfit_name}' not found")
    
    def set_accessory(self, accessory_name: str):
        """
        Set the character's accessory.
        
        Args:
            accessory_name: Name of the accessory ("none" to remove)
        """
        self._accessory = accessory_name
        
        # Hide all known accessories
        for acc_obj in self._accessories.values():
            acc_obj.hide_viewport = True
            acc_obj.hide_render = True
            
        # Show the selected accessory
        if accessory_name == "none":
            print("Accessories cleared")
            return
            
        if acc_obj := self._accessories.get(accessory_name.lower()):
            acc_obj.hide_viewport = False
            acc_obj.hide_render = False
            print(f"Accessory set to: {accessory_name}")
        elif found_obj := next(
            (obj for obj in bpy.data.objects 
             if "accessory" in obj.name.lower() and accessory_name.lower() in obj.name.lower()),
            None
        ):
            # Fallback scan if not in cache (maybe added recently)
            found_obj.hide_viewport = False
            found_obj.hide_render = False
            # Update cache
            self._accessories[accessory_name.lower()] = found_obj
            print(f"Accessory set to: {accessory_name} (found via fallback)")
        else:
            print(f"Warning: Accessory '{accessory_name}' not found")
    
    def set_happiness(self, happiness: float):
        """
        Set the character's happiness level.
        
        Args:
            happiness: Happiness value (clamped between 0 and 100)
        """
        # Clamp value
        old_happiness = self._happiness
        self._happiness = max(0.0, min(happiness, 100.0))
        
        # Trigger callbacks if changed
        if old_happiness != self._happiness:
            for callback in self._on_happiness_changed:
                callback(self._happiness)
    
    def increase_happiness(self, amount: float):
        """Increase happiness by the specified amount."""
        self.set_happiness(self._happiness + amount)
    
    def decrease_happiness(self, amount: float):
        """Decrease happiness by the specified amount."""
        self.set_happiness(self._happiness - amount)
    
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
        self.increase_happiness(self.game_config.homework_happiness_reward)
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
            "happiness": self._happiness,
            "mood": self.mood.value,
            "eye_scale": self._eye_scale,
            "outfit": self._outfit,
            "accessory": self._accessory,
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
