import unittest
import sys
import os
from pathlib import Path

# Add parent directory to path to import modules
sys.path.append(str(Path(__file__).parent.parent))

try:
    import bpy
    from character_controller import CharacterController, CharacterConfig, MoodState
except ImportError:
    print("Skipping Blender tests (bpy not found)")
    bpy = None

class TestCharacterController(unittest.TestCase):
    
    def setUp(self):
        if not bpy:
            self.skipTest("Blender API not available")
            
        # Clear scene
        bpy.ops.wm.read_factory_settings(use_empty=True)
        
        # Create dummy character objects
        bpy.ops.object.armature_add(enter_editmode=False, location=(0, 0, 0))
        self.armature = bpy.context.active_object
        self.armature.name = "Leandi_Armature"
        
        # Add bones
        bpy.ops.object.mode_set(mode='EDIT')
        for bone_name in ["eye_L", "eye_R"]:
            bone = self.armature.data.edit_bones.new(bone_name)
            bone.head = (0, 0, 0)
            bone.tail = (0, 0, 1)
        bpy.ops.object.mode_set(mode='OBJECT')
        
        # Create dummy mesh
        bpy.ops.mesh.primitive_cube_add()
        self.mesh = bpy.context.active_object
        self.mesh.name = "Leandi_Mesh"
        
        # Initialize controller
        self.controller = CharacterController("Leandi")

    def test_initialization(self):
        self.assertEqual(self.controller.character_name, "Leandi")
        self.assertIsNotNone(self.controller.armature)
        self.assertIsNotNone(self.controller.mesh)
        self.assertEqual(self.controller.happiness, 75.0)

    def test_happiness_clamping(self):
        self.controller.set_happiness(150.0)
        self.assertEqual(self.controller.happiness, 100.0)
        
        self.controller.set_happiness(-50.0)
        self.assertEqual(self.controller.happiness, 0.0)

    def test_mood_states(self):
        self.controller.set_happiness(90)
        self.assertEqual(self.controller.mood, MoodState.VERY_HAPPY)
        
        self.controller.set_happiness(70)
        self.assertEqual(self.controller.mood, MoodState.HAPPY)
        
        self.controller.set_happiness(50)
        self.assertEqual(self.controller.mood, MoodState.NEUTRAL)
        
        self.controller.set_happiness(30)
        self.assertEqual(self.controller.mood, MoodState.SAD)
        
        self.controller.set_happiness(10)
        self.assertEqual(self.controller.mood, MoodState.VERY_SAD)

    def test_eye_scaling(self):
        # Test normal scaling
        self.controller.set_eye_scale(1.5)
        self.assertEqual(self.controller.config.eye_scale, 1.5)
        
        # Verify bone scaling
        bone = self.armature.pose.bones["eye_L"]
        self.assertEqual(bone.scale[0], 1.5)
        
        # Test clamping
        self.controller.set_eye_scale(3.0)
        self.assertEqual(self.controller.config.eye_scale, 2.0)
        
        self.controller.set_eye_scale(0.1)
        self.assertEqual(self.controller.config.eye_scale, 0.5)

    def test_asset_caching(self):
        # Verify bones are cached
        self.assertIn("eye_l", self.controller._bones)
        self.assertIn("eye_r", self.controller._bones)
        
        # Verify cache usage
        self.controller.set_eye_scale(1.2)
        self.assertEqual(self.controller._bones["eye_l"].scale[0], 1.2)

if __name__ == '__main__':
    unittest.main()
