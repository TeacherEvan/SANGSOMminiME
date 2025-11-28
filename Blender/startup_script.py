"""
Sangsom Mini-Me - Blender Startup Script

This script initializes the Blender environment for the Sangsom Mini-Me project.
Run this script when opening Blender to set up the project workspace.

Usage:
    1. Open Blender 5.0.0
    2. Go to Scripting workspace
    3. Load and run this script
    OR
    4. Set as startup script in Blender preferences
"""

import bpy
import os
import sys
from pathlib import Path

# Constants
PROJECT_NAME = "Sangsom Mini-Me"
VERSION = "1.0.0"
TARGET_FPS = 60


def get_project_root() -> Path:
    """Get the project root directory."""
    # Try to find project root from blend file location
    if bpy.data.filepath:
        return Path(bpy.data.filepath).parent.parent
    # Fallback to script location
    return Path(__file__).parent.parent


def setup_project_paths():
    """Add project paths to Python path for module imports."""
    project_root = get_project_root()
    
    scripts_path = project_root / "Assets" / "Scripts"
    if scripts_path.exists():
        runtime_path = scripts_path / "Runtime"
        editor_path = scripts_path / "Editor"
        
        for path in [str(runtime_path), str(editor_path)]:
            if path not in sys.path:
                sys.path.insert(0, path)
                print(f"Added to Python path: {path}")


def setup_scene_settings():
    """Configure scene settings for the project."""
    scene = bpy.context.scene
    
    # Set render engine
    scene.render.engine = 'BLENDER_EEVEE_NEXT'
    
    # Frame rate for animations
    scene.render.fps = TARGET_FPS
    
    # Set up render resolution for preview
    scene.render.resolution_x = 1920
    scene.render.resolution_y = 1080
    scene.render.resolution_percentage = 100
    
    # Enable some EEVEE features
    if hasattr(scene.eevee, 'use_bloom'):
        scene.eevee.use_bloom = True
    
    print(f"Scene configured: {TARGET_FPS}fps, EEVEE Next renderer")


def setup_workspace():
    """Set up the Blender workspace for Mini-Me development."""
    # Ensure we have the standard workspaces
    workspaces_needed = ['Layout', 'Modeling', 'Animation', 'Scripting']
    
    for ws_name in workspaces_needed:
        if ws_name not in bpy.data.workspaces:
            print(f"Note: Workspace '{ws_name}' not found - using defaults")
    
    print("Workspace setup complete")


def setup_character_collection():
    """Create collection structure for character organization."""
    collection_structure = {
        'Characters': ['Leandi', 'Templates'],
        'Environment': ['Rooms', 'Props'],
        'UI': ['Screens', 'Elements'],
        'Effects': ['Particles', 'Shaders']
    }
    
    scene_collection = bpy.context.scene.collection
    
    for parent_name, children in collection_structure.items():
        # Create parent collection
        if parent_name not in bpy.data.collections:
            parent_col = bpy.data.collections.new(parent_name)
            scene_collection.children.link(parent_col)
        else:
            parent_col = bpy.data.collections[parent_name]
            if parent_col.name not in [c.name for c in scene_collection.children]:
                scene_collection.children.link(parent_col)
        
        # Create child collections
        for child_name in children:
            full_name = f"{parent_name}_{child_name}"
            if full_name not in bpy.data.collections:
                child_col = bpy.data.collections.new(full_name)
                parent_col.children.link(child_col)
    
    print("Collection hierarchy created")


def setup_world():
    """Set up world lighting for anime-style rendering."""
    world = bpy.context.scene.world
    if world is None:
        world = bpy.data.worlds.new("Mini-Me World")
        bpy.context.scene.world = world
    
    world.use_nodes = True
    nodes = world.node_tree.nodes
    
    # Clear existing nodes
    nodes.clear()
    
    # Create background node
    bg_node = nodes.new('ShaderNodeBackground')
    bg_node.inputs['Color'].default_value = (0.8, 0.85, 0.95, 1.0)  # Soft blue
    bg_node.inputs['Strength'].default_value = 1.0
    bg_node.location = (0, 0)
    
    # Create output node
    output_node = nodes.new('ShaderNodeOutputWorld')
    output_node.location = (300, 0)
    
    # Connect nodes
    world.node_tree.links.new(bg_node.outputs['Background'], output_node.inputs['Surface'])
    
    print("World lighting configured")


def create_camera_setup():
    """Create a camera setup suitable for character viewing."""
    # Check if camera already exists
    camera_name = "MiniMe_Camera"
    if camera_name in bpy.data.objects:
        print(f"Camera '{camera_name}' already exists")
        return
    
    # Create camera
    camera_data = bpy.data.cameras.new(name=camera_name)
    camera_obj = bpy.data.objects.new(camera_name, camera_data)
    
    # Link to scene
    bpy.context.scene.collection.objects.link(camera_obj)
    
    # Position camera
    camera_obj.location = (0, -3, 1.5)
    camera_obj.rotation_euler = (1.3, 0, 0)
    
    # Set as active camera
    bpy.context.scene.camera = camera_obj
    
    print("Camera setup complete")


def create_basic_lighting():
    """Create three-point lighting setup."""
    light_configs = [
        {"name": "Key_Light", "type": "AREA", "location": (2, -2, 3), "energy": 1000, "color": (1, 0.98, 0.95)},
        {"name": "Fill_Light", "type": "AREA", "location": (-2, -2, 2), "energy": 400, "color": (0.9, 0.95, 1)},
        {"name": "Rim_Light", "type": "AREA", "location": (0, 2, 3), "energy": 600, "color": (1, 1, 1)},
    ]
    
    for config in light_configs:
        if config["name"] in bpy.data.objects:
            continue
        
        light_data = bpy.data.lights.new(name=config["name"], type=config["type"])
        light_data.energy = config["energy"]
        light_data.color = config["color"]
        
        light_obj = bpy.data.objects.new(config["name"], light_data)
        bpy.context.scene.collection.objects.link(light_obj)
        light_obj.location = config["location"]
    
    print("Lighting setup complete")


def print_project_info():
    """Print project information to console."""
    print("\n" + "=" * 50)
    print(f"🎮 {PROJECT_NAME} v{VERSION}")
    print("=" * 50)
    print("Educational Tamagotchi Universe - Blender Project")
    print(f"Project Root: {get_project_root()}")
    print(f"Blender Version: {bpy.app.version_string}")
    print("=" * 50)
    print("\nQuick Start:")
    print("  1. Import character photos to Assets/Characters/Leandi/Photos/")
    print("  2. Use AI tools to generate character model")
    print("  3. Set up animations (dance, wave, wai, curtsy, bow)")
    print("  4. Configure customization (eye scaling, outfits)")
    print("\nHotkeys:")
    print("  F12 - Render preview")
    print("  Ctrl+S - Save project")
    print("=" * 50 + "\n")


def main():
    """Main initialization function."""
    print("\n🚀 Initializing Sangsom Mini-Me Blender Project...")
    
    # Setup in order
    setup_project_paths()
    setup_scene_settings()
    setup_workspace()
    setup_character_collection()
    setup_world()
    create_camera_setup()
    create_basic_lighting()
    
    # Print info
    print_project_info()
    
    print("✅ Project initialization complete!")


# Run on script execution
if __name__ == "__main__":
    main()
