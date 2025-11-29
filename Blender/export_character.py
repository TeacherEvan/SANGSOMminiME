"""
Automated Character Export Script for Sangsom Mini-Me

This script exports characters from Blender to the Unity Assets folder
in formats compatible with Unity 2022.3.12f1 (GLB and FBX).

Usage:
    blender --background character.blend --python export_character.py
    OR
    npm run blender:export
"""

import bpy
import os
from pathlib import Path


def get_project_root() -> Path:
    """Get the project root directory."""
    if bpy.data.filepath:
        return Path(bpy.data.filepath).parent.parent
    return Path(__file__).parent.parent


def write_export_metadata(metadata_path: Path, character_name: str, selected_objects: list) -> None:
    """Write metadata file for exported character."""
    with open(metadata_path, 'w', encoding='utf-8') as f:
        f.write(f"Character: {character_name}\n")
        f.write(f"Blender Version: {bpy.app.version_string}\n")
        f.write(f"Exported Objects: {len(selected_objects)}\n")
        f.write(f"Object Names:\n")
        for obj in selected_objects:
            f.write(f"  - {obj.name} ({obj.type})\n")
        
        # Animation info
        if bpy.data.actions:
            f.write(f"\nAnimations ({len(bpy.data.actions)}):\n")
            for action in bpy.data.actions:
                f.write(f"  - {action.name} ({action.frame_range[0]:.0f}-{action.frame_range[1]:.0f})\n")


def export_character():
    """Export selected character to Unity Assets folder."""
    project_root = get_project_root()
    
    # Determine character name from blend file or selection
    character_name = "Leandi"  # Default
    if bpy.data.filepath:
        blend_name = Path(bpy.data.filepath).stem
        if blend_name and blend_name != "untitled":
            character_name = blend_name.title()
    
    # Setup export directory
    export_dir = project_root / "Assets" / "Characters" / character_name
    export_dir.mkdir(parents=True, exist_ok=True)
    
    # Check if we have selection
    selected_objects = list(bpy.context.selected_objects)
    if not selected_objects:
        print("⚠️  No objects selected. Selecting all mesh objects...")
        bpy.ops.object.select_all(action='DESELECT')
        for obj in bpy.data.objects:
            if obj.type == 'MESH':
                obj.select_set(True)
        selected_objects = list(bpy.context.selected_objects)
    
    if not selected_objects:
        print("❌ No mesh objects found to export!")
        return False
    
    print(f"\n📦 Exporting {len(selected_objects)} object(s): {', '.join([obj.name for obj in selected_objects])}")
    
    # ===========================
    # Export as GLB (recommended for Unity)
    # ===========================
    glb_path = export_dir / f"{character_name.lower()}_character.glb"
    try:
        bpy.ops.export_scene.gltf(
            filepath=str(glb_path),
            export_format='GLB',
            use_selection=True,
            export_animations=True,
            export_materials='EXPORT',
            export_lights=False,
            export_cameras=False,
            export_apply=True,  # Apply modifiers
            export_yup=True,    # Unity uses Y-up
        )
        print(f"✅ GLB exported to: {glb_path}")
        glb_success = True
    except Exception as e:
        print(f"❌ GLB export failed: {e}")
        glb_success = False
    
    # ===========================
    # Export as FBX (compatibility)
    # ===========================
    fbx_path = export_dir / f"{character_name.lower()}_character.fbx"
    try:
        bpy.ops.export_scene.fbx(
            filepath=str(fbx_path),
            use_selection=True,
            global_scale=1.0,
            apply_scale_options='FBX_SCALE_ALL',
            axis_forward='-Z',
            axis_up='Y',
            bake_anim=True,
            bake_anim_use_all_actions=False,
            bake_anim_use_nla_strips=False,
            add_leaf_bones=False,
            mesh_smooth_type='FACE',
        )
        print(f"✅ FBX exported to: {fbx_path}")
        fbx_success = True
    except Exception as e:
        print(f"❌ FBX export failed: {e}")
        fbx_success = False
    
    # ===========================
    # Export Metadata
    # ===========================
    metadata_path = export_dir / f"{character_name.lower()}_metadata.txt"
    try:
        write_export_metadata(metadata_path, character_name, selected_objects)
        print(f"✅ Metadata saved to: {metadata_path}")
    except Exception as e:
        print(f"⚠️  Metadata export failed: {e}")
    
    print(f"\n{'='*60}")
    if glb_success or fbx_success:
        print(f"✅ Export complete for {character_name}!")
        print(f"   Location: {export_dir}")
        return True
    else:
        print("❌ Export failed!")
        return False


def export_all_characters():
    """Export all character collections separately."""
    project_root = get_project_root()
    
    # Find character collections
    character_collections = [col for col in bpy.data.collections if col.name.startswith("Characters_")]
    
    if not character_collections:
        print("No character collections found. Running single export...")
        return export_character()
    
    print(f"Found {len(character_collections)} character collection(s)")
    
    success_count = 0
    for col in character_collections:
        print(f"\n{'='*60}")
        print(f"Processing: {col.name}")
        print(f"{'='*60}")
        
        # Select all objects in this collection
        bpy.ops.object.select_all(action='DESELECT')
        for obj in col.all_objects:
            if obj.type == 'MESH':
                obj.select_set(True)
        
        if export_character():
            success_count += 1
    
    print(f"\n{'='*60}")
    print(f"Batch export complete: {success_count}/{len(character_collections)} succeeded")
    return success_count == len(character_collections)


if __name__ == "__main__":
    print("\n" + "="*60)
    print("🎮 Sangsom Mini-Me Character Export")
    print("="*60)
    
    # Run export
    try:
        export_character()
    except Exception as e:
        import traceback
        print(f"\n❌ Fatal error during export:")
        print(traceback.format_exc())
        exit(1)
