"""
Sangsom Mini-Me - Character Controller for Blender

This module handles character behavior, animations, and customization
within the Blender environment using modern Python 3.11+ patterns.

Features:
    - Type-safe character management with comprehensive type hints
    - Performance-optimized asset caching
    - Event-driven architecture for extensibility
    - Defensive programming with error handling
    - Clear separation of concerns

Usage:
    from character_controller import CharacterController
    
    controller = CharacterController("Leandi")
    controller.play_animation("dance")
    controller.set_eye_scale(1.5)
    
Author: Sangsom Mini-Me Development Team
Version: 2.0.0
"""

import bpy
from typing import Optional, List, Dict, Callable, Union, Any
from enum import Enum
from dataclasses import dataclass
import logging

try:
    from game_configuration import get_game_config, GameConfiguration
except ImportError:
    logging.warning("game_configuration module not found. Using default configuration.")
    GameConfiguration = None
    def get_game_config():
        return None


# Configure logging for better debugging
logging.basicConfig(level=logging.INFO, format='[%(levelname)s] %(message)s')
logger = logging.getLogger(__name__)


class CharacterAnimation(Enum):
    """Character animation types supported by the Mini-Me system."""
    IDLE = "idle"
    DANCE = "dance"
    WAVE = "wave"
    WAI = "wai"          # Thai traditional greeting gesture
    CURTSY = "curtsy"
    BOW = "bow"
    
    @classmethod
    def from_string(cls, name: str) -> Optional['CharacterAnimation']:
        """
        Convert string to CharacterAnimation enum safely.
        
        Args:
            name: Animation name as string
            
        Returns:
            CharacterAnimation enum or None if not found
        """
        try:
            return cls(name.lower())
        except (ValueError, AttributeError):
            logger.warning(f"Unknown animation: {name}")
            return None


class MoodState(Enum):
    """Mood states for character happiness display with modern enum patterns."""
    VERY_HAPPY = "very_happy"    # 80-100 happiness
    HAPPY = "happy"               # 60-80 happiness
    NEUTRAL = "neutral"           # 40-60 happiness
    SAD = "sad"                   # 20-40 happiness
    VERY_SAD = "very_sad"         # 0-20 happiness
    
    def __str__(self) -> str:
        """String representation of mood."""
        return self.name.replace('_', ' ').title()
    
    @property
    def emoji(self) -> str:
        """Get emoji representation of mood."""
        mood_emojis = {
            MoodState.VERY_HAPPY: "😄",
            MoodState.HAPPY: "😊",
            MoodState.NEUTRAL: "😐",
            MoodState.SAD: "😟",
            MoodState.VERY_SAD: "😢"
        }
        return mood_emojis.get(self, "😐")


@dataclass
class AnimationMetadata:
    """Metadata for character animations with dataclass pattern."""
    name: str
    duration: float = 0.0
    happiness_bonus: float = 0.0
    is_looping: bool = False
    requires_idle_return: bool = True


class CharacterController:
    """
    Controls the Mini-Me character behavior and interactions in Blender with production-grade quality.
    
    This class manages:
    - Character animations with state management
    - Customization (eye scale, outfits, accessories) with caching
    - Happiness and mood system with callbacks
    - Event-driven architecture for extensibility
    - Performance optimization through asset caching
    
    Attributes:
        character_name: Name of the character
        armature: Blender armature object for the character
        mesh: Blender mesh object for the character
        game_config: Game configuration for constants and settings
        
    Example:
        >>> controller = CharacterController("Leandi")
        >>> controller.set_eye_scale(1.5)
        >>> controller.play_dance()
        >>> print(f"Current mood: {controller.mood}")
    """
    
    # Class-level constants for configuration
    DEFAULT_EYE_SCALE: float = 1.0
    MIN_EYE_SCALE: float = 0.5
    MAX_EYE_SCALE: float = 2.0
    MIN_HAPPINESS: float = 0.0
    MAX_HAPPINESS: float = 100.0
    DEFAULT_HAPPINESS: float = 75.0
    
    def __init__(self, character_name: str) -> None:
        """
        Initialize the character controller with defensive programming.
        
        Args:
            character_name: Name of the character to control
            
        Raises:
            ValueError: If character_name is empty or invalid
        """
        if not character_name or not isinstance(character_name, str):
            raise ValueError("Character name must be a non-empty string")
        
        self.character_name: str = character_name.strip()
        self.game_config: Optional[Any] = get_game_config()
        
        # State management with type hints
        self._eye_scale: float = self.DEFAULT_EYE_SCALE
        self._outfit: str = "default"
        self._accessory: str = "none"
        self._happiness: float = (
            self.game_config.starting_happiness 
            if self.game_config and hasattr(self.game_config, 'starting_happiness')
            else self.DEFAULT_HAPPINESS
        )
        
        # Blender object references with Optional types
        self.armature: Optional[bpy.types.Object] = None
        self.mesh: Optional[bpy.types.Object] = None
        self._is_animating: bool = False
        
        # Event callbacks with proper type hints
        self._on_animation_started: List[Callable[[str], None]] = []
        self._on_animation_completed: List[Callable[[str], None]] = []
        self._on_happiness_changed: List[Callable[[float], None]] = []
        
        # Performance-optimized asset caching
        self._accessories: Dict[str, bpy.types.Object] = {}
        self._materials: Dict[str, bpy.types.Material] = {}
        self._actions: Dict[str, bpy.types.Action] = {}
        self._bones: Dict[str, bpy.types.PoseBone] = {}
        
        # Initialize character assets
        self._find_character_objects()
        self._scan_and_cache_assets()
        
        logger.info(f"CharacterController initialized for '{character_name}'")
    
    def _find_character_objects(self) -> None:
        """
        Find and cache the character's armature and mesh objects in the scene.
        Uses defensive programming with fallback searches.
        """
        try:
            # Primary search: exact name match
            armature_name = f"{self.character_name}_Armature"
            if armature_name in bpy.data.objects:
                self.armature = bpy.data.objects[armature_name]
                logger.info(f"Found armature: {armature_name}")
            else:
                # Fallback: search by type and partial name match
                for obj in bpy.data.objects:
                    if obj.type == 'ARMATURE' and self.character_name.lower() in obj.name.lower():
                        self.armature = obj
                        logger.info(f"Found armature via fallback: {obj.name}")
                        break
            
            # Find mesh object
            mesh_name = f"{self.character_name}_Mesh"
            if mesh_name in bpy.data.objects:
                self.mesh = bpy.data.objects[mesh_name]
                logger.info(f"Found mesh: {mesh_name}")
            else:
                # Fallback search
                for obj in bpy.data.objects:
                    if obj.type == 'MESH' and self.character_name.lower() in obj.name.lower():
                        self.mesh = obj
                        logger.info(f"Found mesh via fallback: {obj.name}")
                        break
            
            # Validation
            if not self.armature:
                logger.warning(f"No armature found for character '{self.character_name}'")
            if not self.mesh:
                logger.warning(f"No mesh found for character '{self.character_name}'")
                
        except Exception as e:
            logger.error(f"Error finding character objects: {e}")
    
    def _scan_and_cache_assets(self) -> None:
        """
        Scan and cache available assets for performance optimization.
        Implements lazy loading pattern for better memory management.
        """
        try:
            # Cache accessories (lazy loading)
            self._cache_accessories()
            
            # Cache materials (outfits)
            self._cache_materials()
            
            # Cache actions (animations)
            self._cache_actions()
            
            # Cache bones if armature exists
            self._cache_bones()
            
            logger.info(f"Asset cache initialized: {len(self._accessories)} accessories, "
                       f"{len(self._materials)} materials, {len(self._actions)} actions")
                       
        except Exception as e:
            logger.error(f"Error scanning assets: {e}")
    
    def _cache_accessories(self) -> None:
        """Cache accessory objects for quick access."""
        self._accessories.clear()
        for obj in bpy.data.objects:
            if "accessory" in obj.name.lower():
                key = self._extract_asset_key(obj.name, "accessory")
                self._accessories[key] = obj
    
    def _cache_materials(self) -> None:
        """Cache material objects for quick access."""
        self._materials.clear()
        for mat in bpy.data.materials:
            if "outfit" in mat.name.lower():
                key = self._extract_asset_key(mat.name, "outfit")
                self._materials[key] = mat
    
    def _cache_actions(self) -> None:
        """Cache animation actions for quick access."""
        self._actions.clear()
        for action in bpy.data.actions:
            self._actions[action.name] = action
            # Cache by simple name if it follows naming convention
            if self.character_name.lower() in action.name.lower():
                simple_name = action.name.lower().replace(f"{self.character_name.lower()}_", "")
                self._actions[simple_name] = action
    
    def _cache_bones(self) -> None:
        """Cache bone references for quick access."""
        self._bones.clear()
        if self.armature and self.armature.pose:
            for bone in self.armature.pose.bones:
                self._bones[bone.name] = bone
                self._bones[bone.name.lower()] = bone
    
    @staticmethod
    def _extract_asset_key(name: str, prefix: str) -> str:
        """
        Extract asset key from name with prefix.
        
        Args:
            name: Full asset name
            prefix: Prefix to remove (e.g., "accessory", "outfit")
            
        Returns:
            Extracted key in lowercase
        """
        key = name.lower()
        prefix_with_underscore = f"{prefix}_"
        if prefix_with_underscore in key:
            return key.split(prefix_with_underscore, 1)[1]
        return key
    
    # ===== PROPERTIES WITH TYPE SAFETY =====
    
    @property
    def is_animating(self) -> bool:
        """Check if the character is currently animating."""
        return self._is_animating
    
    @property
    def happiness(self) -> float:
        """Get current happiness level (0-100)."""
        return self._happiness
    
    @property
    def mood(self) -> MoodState:
        """
        Get current mood state based on happiness level.
        
        Returns:
            MoodState enum representing current emotional state
        """
        # Use game config if available, otherwise use defaults
        if self.game_config and hasattr(self.game_config, 'happy_threshold'):
            happy_thresh = self.game_config.happy_threshold
            sad_thresh = self.game_config.sad_threshold
        else:
            happy_thresh = 70.0
            sad_thresh = 30.0
        
        if self._happiness >= happy_thresh + 10:
            return MoodState.VERY_HAPPY
        elif self._happiness >= happy_thresh:
            return MoodState.HAPPY
        elif self._happiness >= sad_thresh:
            return MoodState.NEUTRAL
        elif self._happiness >= sad_thresh - 10:
            return MoodState.SAD
        else:
            return MoodState.VERY_SAD
    
    @property
    def eye_scale(self) -> float:
        """Get current eye scale value."""
        return self._eye_scale
    
    @property
    def current_outfit(self) -> str:
        """Get current outfit name."""
        return self._outfit
    
    @property
    def current_accessory(self) -> str:
        """Get current accessory name."""
        return self._accessory
    
    # ===== EVENT MANAGEMENT =====
    
    def add_animation_started_callback(self, callback: Callable[[str], None]) -> None:
        """
        Register a callback for animation start events.
        
        Args:
            callback: Function to call when animation starts
        """
        if callback not in self._on_animation_started:
            self._on_animation_started.append(callback)
    
    def add_animation_completed_callback(self, callback: Callable[[str], None]) -> None:
        """
        Register a callback for animation completion events.
        
        Args:
            callback: Function to call when animation completes
        """
        if callback not in self._on_animation_completed:
            self._on_animation_completed.append(callback)
    
    def add_happiness_changed_callback(self, callback: Callable[[float], None]) -> None:
        """
        Register a callback for happiness change events.
        
        Args:
            callback: Function to call when happiness changes
        """
        if callback not in self._on_happiness_changed:
            self._on_happiness_changed.append(callback)
    
    # ===== ANIMATION CONTROL =====
    
    def play_animation(self, animation: Union[CharacterAnimation, str]) -> bool:
        """
        Play a character animation with comprehensive error handling.
        
        Args:
            animation: The animation to play (CharacterAnimation enum or string name)
            
        Returns:
            True if animation started successfully, False otherwise
            
        Raises:
            No exceptions raised - all errors are logged and return False
        """
        try:
            # Check if already animating
            if self._is_animating:
                logger.warning(f"Animation already in progress for '{self.character_name}'")
                return False
            
            # Validate armature exists
            if not self.armature:
                logger.error(f"No armature found for character '{self.character_name}'")
                return False
            
            # Resolve animation name with type safety
            if isinstance(animation, CharacterAnimation):
                anim_value = animation.value
            elif isinstance(animation, str):
                anim_value = animation.lower()
            else:
                logger.error(f"Invalid animation type: {type(animation)}")
                return False
            
            # Find animation action (cache-first approach)
            action = self._find_animation_action(anim_value)
            if not action:
                logger.warning(f"Animation '{anim_value}' not found for '{self.character_name}'")
                return False
            
            # Apply animation to armature
            if not self._apply_animation_action(action):
                logger.error(f"Failed to apply animation '{anim_value}'")
                return False
            
            # Update state
            self._is_animating = True
            
            # Trigger animation started callbacks
            self._trigger_callbacks(self._on_animation_started, anim_value)
            
            # Apply special effects for certain animations
            self._apply_animation_effects(anim_value)
            
            logger.info(f"Playing animation: {anim_value}")
            return True
            
        except Exception as e:
            logger.error(f"Error playing animation: {e}")
            return False
    
    def _find_animation_action(self, anim_name: str) -> Optional[bpy.types.Action]:
        """
        Find animation action with fallback search.
        
        Args:
            anim_name: Name of the animation
            
        Returns:
            Action object if found, None otherwise
        """
        # Try cache first (performance optimization)
        action_name = f"{self.character_name}_{anim_name}"
        action = self._actions.get(action_name) or self._actions.get(anim_name)
        
        if action:
            return action
        
        # Fallback: direct lookup in Blender data
        action = bpy.data.actions.get(action_name) or bpy.data.actions.get(anim_name)
        
        if action:
            # Update cache for future lookups
            self._actions[anim_name] = action
            
        return action
    
    def _apply_animation_action(self, action: bpy.types.Action) -> bool:
        """
        Apply animation action to armature safely.
        
        Args:
            action: Blender action to apply
            
        Returns:
            True if successful, False otherwise
        """
        try:
            if not self.armature:
                return False
            
            # Ensure animation data exists
            if not self.armature.animation_data:
                self.armature.animation_data_create()
            
            # Apply action
            if self.armature.animation_data:
                self.armature.animation_data.action = action
                return True
            
            return False
            
        except Exception as e:
            logger.error(f"Error applying animation action: {e}")
            return False
    
    def _apply_animation_effects(self, anim_name: str) -> None:
        """
        Apply special effects for certain animations.
        
        Args:
            anim_name: Name of the animation
        """
        # Dance animation increases happiness
        if anim_name == CharacterAnimation.DANCE.value:
            bonus = (
                self.game_config.dance_happiness_bonus 
                if self.game_config and hasattr(self.game_config, 'dance_happiness_bonus')
                else 5.0
            )
            self.increase_happiness(bonus)
    
    def _trigger_callbacks(self, callbacks: List[Callable], *args, **kwargs) -> None:
        """
        Safely trigger callbacks with error handling.
        
        Args:
            callbacks: List of callback functions
            *args: Positional arguments for callbacks
            **kwargs: Keyword arguments for callbacks
        """
        for callback in callbacks:
            try:
                callback(*args, **kwargs)
            except Exception as e:
                logger.error(f"Error in callback {callback.__name__}: {e}")
    
    def stop_animation(self) -> None:
        """
        Stop the current animation and return to idle.
        """
        self._is_animating = False
        logger.info("Animation stopped")
    
    # Convenience methods for specific animations
    
    def play_idle(self) -> bool:
        """Play idle animation."""
        return self.play_animation(CharacterAnimation.IDLE)
    
    def play_dance(self) -> bool:
        """Play dance animation."""
        return self.play_animation(CharacterAnimation.DANCE)
    
    def play_wave(self) -> bool:
        """Play wave animation."""
        return self.play_animation(CharacterAnimation.WAVE)
    
    def play_wai(self) -> bool:
        """Play Thai wai greeting animation."""
        return self.play_animation(CharacterAnimation.WAI)
    
    def play_curtsy(self) -> bool:
        """Play curtsy animation."""
        return self.play_animation(CharacterAnimation.CURTSY)
    
    def play_bow(self) -> bool:
        """Play bow animation."""
        return self.play_animation(CharacterAnimation.BOW)
    
    # ===== CUSTOMIZATION CONTROL =====
    
    def set_eye_scale(self, scale: float) -> bool:
        """
        Set the eye scale for the character with clamping and validation.
        
        Args:
            scale: Eye scale value (will be clamped between min and max)
            
        Returns:
            True if successful, False otherwise
        """
        try:
            # Get config values or use defaults
            if self.game_config:
                min_scale = getattr(self.game_config, 'min_eye_scale', self.MIN_EYE_SCALE)
                max_scale = getattr(self.game_config, 'max_eye_scale', self.MAX_EYE_SCALE)
            else:
                min_scale, max_scale = self.MIN_EYE_SCALE, self.MAX_EYE_SCALE
            
            # Clamp value to valid range
            scale = max(min_scale, min(scale, max_scale))
            self._eye_scale = scale
            
            # Apply to bones if armature exists
            if self.armature and self.armature.pose:
                eye_bone_names = ["eye_L", "eye_R", "Eye.L", "Eye.R", "eye.L", "eye.R"]
                bones_updated = 0
                
                for bone_name in eye_bone_names:
                    # Try cache first (performance optimization)
                    bone = self._bones.get(bone_name) or self._bones.get(bone_name.lower())
                    
                    if bone:
                        bone.scale = (scale, scale, scale)
                        bones_updated += 1
                    elif bone_name in self.armature.pose.bones:
                        # Fallback to direct lookup and update cache
                        pose_bone = self.armature.pose.bones[bone_name]
                        pose_bone.scale = (scale, scale, scale)
                        self._bones[bone_name] = pose_bone
                        self._bones[bone_name.lower()] = pose_bone
                        bones_updated += 1
                
                if bones_updated > 0:
                    logger.info(f"Eye scale set to {scale:.2f} ({bones_updated} bones updated)")
                    return True
                else:
                    logger.warning(f"No eye bones found to apply scale")
                    return False
            else:
                logger.warning("No armature or pose found for eye scaling")
                return False
                
        except Exception as e:
            logger.error(f"Error setting eye scale: {e}")
            return False
    
    def set_outfit(self, outfit_name: str) -> bool:
        """
        Set the character's outfit with error handling.
        
        Args:
            outfit_name: Name of the outfit to apply
            
        Returns:
            True if successful, False otherwise
        """
        try:
            self._outfit = outfit_name.lower().strip()
            
            # Validate mesh exists
            if not self.mesh:
                logger.warning(f"No mesh found for character '{self.character_name}'")
                return False
            
            # Find material (cache-first approach)
            material = self._find_outfit_material(self._outfit)
            
            if not material:
                logger.warning(f"Outfit material '{outfit_name}' not found")
                return False
            
            # Apply material to mesh
            if len(self.mesh.material_slots) > 0:
                self.mesh.material_slots[0].material = material
                logger.info(f"Outfit set to: {outfit_name}")
                return True
            else:
                logger.warning(f"Mesh has no material slots")
                return False
                
        except Exception as e:
            logger.error(f"Error setting outfit: {e}")
            return False
    
    def _find_outfit_material(self, outfit_name: str) -> Optional[bpy.types.Material]:
        """
        Find outfit material with fallback search.
        
        Args:
            outfit_name: Name of the outfit
            
        Returns:
            Material object if found, None otherwise
        """
        # Try cache first
        material = self._materials.get(outfit_name)
        
        if material:
            return material
        
        # Try with outfit_ prefix
        material_name = f"outfit_{outfit_name}"
        material = bpy.data.materials.get(material_name)
        
        if material:
            # Update cache
            self._materials[outfit_name] = material
            
        return material
    
    def set_accessory(self, accessory_name: str) -> bool:
        """
        Set the character's accessory with comprehensive error handling.
        
        Args:
            accessory_name: Name of the accessory ("none" to remove all)
            
        Returns:
            True if successful, False otherwise
        """
        try:
            self._accessory = accessory_name.lower().strip()
            
            # Hide all cached accessories first
            self._hide_all_accessories()
            
            # If "none", we're done
            if self._accessory == "none":
                logger.info("All accessories cleared")
                return True
            
            # Find and show the selected accessory
            accessory_obj = self._find_accessory(self._accessory)
            
            if accessory_obj:
                accessory_obj.hide_viewport = False
                accessory_obj.hide_render = False
                logger.info(f"Accessory set to: {accessory_name}")
                return True
            else:
                logger.warning(f"Accessory '{accessory_name}' not found")
                return False
                
        except Exception as e:
            logger.error(f"Error setting accessory: {e}")
            return False
    
    def _hide_all_accessories(self) -> None:
        """Hide all cached accessory objects."""
        for acc_obj in self._accessories.values():
            try:
                acc_obj.hide_viewport = True
                acc_obj.hide_render = True
            except Exception as e:
                logger.error(f"Error hiding accessory {acc_obj.name}: {e}")
    
    def _find_accessory(self, accessory_name: str) -> Optional[bpy.types.Object]:
        """
        Find accessory object with fallback search.
        
        Args:
            accessory_name: Name of the accessory
            
        Returns:
            Object if found, None otherwise
        """
        # Try cache first
        accessory_obj = self._accessories.get(accessory_name)
        
        if accessory_obj:
            return accessory_obj
        
        # Fallback: scan for accessory by name
        for obj in bpy.data.objects:
            if "accessory" in obj.name.lower() and accessory_name in obj.name.lower():
                # Update cache
                self._accessories[accessory_name] = obj
                logger.info(f"Found accessory via fallback: {obj.name}")
                return obj
        
        return None
    
    # ===== HAPPINESS MANAGEMENT =====
    
    def set_happiness(self, happiness: float) -> None:
        """
        Set the character's happiness level with validation and callbacks.
        
        Args:
            happiness: Happiness value (automatically clamped to 0-100)
        """
        try:
            # Store old value for change detection
            old_happiness = self._happiness
            
            # Clamp to valid range
            self._happiness = max(self.MIN_HAPPINESS, min(happiness, self.MAX_HAPPINESS))
            
            # Trigger callbacks only if value actually changed
            if old_happiness != self._happiness:
                self._trigger_callbacks(self._on_happiness_changed, self._happiness)
                logger.info(f"Happiness changed: {old_happiness:.1f} -> {self._happiness:.1f} ({self.mood})")
                
        except Exception as e:
            logger.error(f"Error setting happiness: {e}")
    
    def increase_happiness(self, amount: float) -> None:
        """
        Increase happiness by the specified amount.
        
        Args:
            amount: Amount to increase (positive value)
        """
        if amount > 0:
            self.set_happiness(self._happiness + amount)
        else:
            logger.warning(f"Increase happiness called with non-positive value: {amount}")
    
    def decrease_happiness(self, amount: float) -> None:
        """
        Decrease happiness by the specified amount.
        
        Args:
            amount: Amount to decrease (positive value)
        """
        if amount > 0:
            self.set_happiness(self._happiness - amount)
        else:
            logger.warning(f"Decrease happiness called with non-positive value: {amount}")
    
    # ===== GAME EVENT HANDLERS =====
    
    def complete_homework(self) -> None:
        """
        Handle homework completion event with celebration.
        Increases happiness and triggers celebratory animation.
        """
        try:
            # Get reward amount from config or use default
            reward = (
                self.game_config.homework_happiness_reward
                if self.game_config and hasattr(self.game_config, 'homework_happiness_reward')
                else 10.0
            )
            
            self.increase_happiness(reward)
            
            # Celebrate if not already animating
            if not self._is_animating:
                self.play_dance()
            
            logger.info(f"Homework completed! Happiness increased by {reward}")
            
        except Exception as e:
            logger.error(f"Error completing homework: {e}")
    
    # ===== STATUS & DEBUGGING =====
    
    def get_status(self) -> Dict[str, Any]:
        """
        Get comprehensive status information about the character.
        
        Returns:
            Dictionary containing all character state information
        """
        return {
            "name": self.character_name,
            "happiness": round(self._happiness, 2),
            "mood": str(self.mood),
            "mood_emoji": self.mood.emoji,
            "eye_scale": round(self._eye_scale, 2),
            "outfit": self._outfit,
            "accessory": self._accessory,
            "is_animating": self._is_animating,
            "has_armature": self.armature is not None,
            "has_mesh": self.mesh is not None,
            "cached_assets": {
                "accessories": len(self._accessories),
                "materials": len(self._materials),
                "actions": len(self._actions),
                "bones": len(self._bones)
            }
        }
    
    def refresh_asset_cache(self) -> None:
        """
        Refresh the asset cache by rescanning the scene.
        Useful when assets are added/removed at runtime.
        """
        logger.info("Refreshing asset cache...")
        self._scan_and_cache_assets()
    
    def __str__(self) -> str:
        """String representation of the controller."""
        return (f"CharacterController(name='{self.character_name}', "
                f"happiness={self.happiness:.1f}, mood={self.mood}, "
                f"animating={self.is_animating})")
    
    def __repr__(self) -> str:
        """Detailed representation for debugging."""
        return (f"CharacterController(character_name='{self.character_name}', "
                f"happiness={self._happiness}, eye_scale={self._eye_scale}, "
                f"outfit='{self._outfit}', accessory='{self._accessory}')")


# ===== CONVENIENCE FUNCTIONS =====

def create_leandi_controller() -> CharacterController:
    """
    Create a character controller for the Leandi test character.
    
    Returns:
        Initialized CharacterController for Leandi
    """
    try:
        controller = CharacterController("Leandi")
        logger.info("Leandi controller created successfully")
        return controller
    except Exception as e:
        logger.error(f"Failed to create Leandi controller: {e}")
        raise


def create_controller_from_selection() -> Optional[CharacterController]:
    """
    Create a character controller from the currently selected armature in Blender.
    
    Returns:
        CharacterController if successful, None otherwise
    """
    try:
        selected = bpy.context.selected_objects
        
        for obj in selected:
            if obj.type == 'ARMATURE':
                # Extract character name from armature
                char_name = obj.name.replace("_Armature", "").replace("_armature", "")
                controller = CharacterController(char_name)
                logger.info(f"Controller created from selection: {char_name}")
                return controller
        
        logger.warning("No armature found in selection")
        return None
        
    except Exception as e:
        logger.error(f"Failed to create controller from selection: {e}")
        return None


# ===== MODULE INITIALIZATION =====

if __name__ == "__main__":
    """Example usage and testing when run directly."""
    print("=" * 50)
    print("Sangsom Mini-Me Character Controller Test")
    print("=" * 50)
    
    try:
        controller = create_leandi_controller()
        print(f"\n{controller}")
        print(f"\nStatus:\n{controller.get_status()}")
        
        # Test animations
        print("\nTesting animations...")
        controller.play_wave()
        
        # Test customization
        print("\nTesting customization...")
        controller.set_eye_scale(1.5)
        
        # Test happiness
        print("\nTesting happiness system...")
        controller.increase_happiness(10)
        
        print(f"\nFinal status:\n{controller.get_status()}")
        
    except Exception as e:
        logger.error(f"Test failed: {e}")
