"""
Sangsom Mini-Me - Blender Addon for Mini-Me Development

This addon provides tools and panels for developing the Mini-Me
educational tamagotchi system within Blender.

Installation:
    1. Open Blender > Edit > Preferences > Add-ons
    2. Click "Install..." and select this file
    3. Enable "Sangsom Mini-Me Tools" addon
    
OR run from Scripting workspace to register temporarily.
"""

bl_info = {
    "name": "Sangsom Mini-Me Tools",
    "author": "Sangsom Mini-Me Development Team",
    "version": (1, 0, 0),
    "blender": (4, 0, 0),
    "location": "View3D > Sidebar > Mini-Me",
    "description": "Development tools for Sangsom Mini-Me educational tamagotchi",
    "category": "Development",
}

import bpy
from bpy.types import Panel, Operator, PropertyGroup
from bpy.props import StringProperty, FloatProperty, IntProperty, BoolProperty


# ============================================================================
# Property Groups
# ============================================================================

class MiniMeProperties(PropertyGroup):
    """Properties for Mini-Me panel."""
    
    character_name: StringProperty(
        name="Character Name",
        description="Name of the character",
        default="Leandi"
    )
    
    eye_scale: FloatProperty(
        name="Eye Scale",
        description="Scale of character eyes",
        default=1.0,
        min=0.5,
        max=2.0,
        step=10
    )
    
    happiness: FloatProperty(
        name="Happiness",
        description="Character happiness level",
        default=75.0,
        min=0.0,
        max=100.0,
        subtype='PERCENTAGE'
    )
    
    username: StringProperty(
        name="Username",
        description="Username for new user",
        default=""
    )
    
    display_name: StringProperty(
        name="Display Name",
        description="Display name for new user",
        default=""
    )


# ============================================================================
# Operators
# ============================================================================

class MINIME_OT_setup_project(Operator):
    """Set up the project structure and initial scene."""
    bl_idname = "minime.setup_project"
    bl_label = "Setup Project"
    bl_description = "Initialize the Mini-Me project structure and scene"
    
    def execute(self, context):
        try:
            # Import and run startup script
            import sys
            import os
            
            # Add Blender folder to path
            addon_dir = os.path.dirname(os.path.realpath(__file__))
            if addon_dir not in sys.path:
                sys.path.insert(0, addon_dir)
            
            # Try to import startup module
            try:
                from startup_script import main as setup_main
                setup_main()
                self.report({'INFO'}, "Project setup complete!")
            except ImportError:
                # Run basic setup inline
                self._basic_setup()
                self.report({'INFO'}, "Basic project setup complete!")
            
            return {'FINISHED'}
        except Exception as e:
            self.report({'ERROR'}, f"Setup failed: {str(e)}")
            return {'CANCELLED'}
    
    def _basic_setup(self):
        """Basic setup when startup script not available."""
        # Create collections
        scene_col = bpy.context.scene.collection
        
        for name in ['Characters', 'Environment', 'UI', 'Effects']:
            if name not in bpy.data.collections:
                col = bpy.data.collections.new(name)
                scene_col.children.link(col)
        
        # Set EEVEE renderer
        bpy.context.scene.render.engine = 'BLENDER_EEVEE_NEXT'
        bpy.context.scene.render.fps = 60


class MINIME_OT_create_character_template(Operator):
    """Create a basic character template."""
    bl_idname = "minime.create_character_template"
    bl_label = "Create Character Template"
    bl_description = "Create a placeholder character for development"
    
    def execute(self, context):
        props = context.scene.minime_props
        name = props.character_name
        
        # Create a simple placeholder mesh
        bpy.ops.mesh.primitive_uv_sphere_add(
            radius=0.5,
            location=(0, 0, 1.5)
        )
        head = bpy.context.active_object
        head.name = f"{name}_Head"
        
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.3,
            depth=1.0,
            location=(0, 0, 0.5)
        )
        body = bpy.context.active_object
        body.name = f"{name}_Body"
        
        # Create collection for character
        char_col_name = f"Characters_{name}"
        if char_col_name not in bpy.data.collections:
            char_col = bpy.data.collections.new(char_col_name)
            
            # Link to Characters collection if it exists
            if 'Characters' in bpy.data.collections:
                bpy.data.collections['Characters'].children.link(char_col)
            else:
                bpy.context.scene.collection.children.link(char_col)
        else:
            char_col = bpy.data.collections[char_col_name]
        
        # Move objects to character collection
        for obj in [head, body]:
            for col in obj.users_collection:
                col.objects.unlink(obj)
            char_col.objects.link(obj)
        
        self.report({'INFO'}, f"Created character template: {name}")
        return {'FINISHED'}


class MINIME_OT_apply_eye_scale(Operator):
    """Apply eye scale to character."""
    bl_idname = "minime.apply_eye_scale"
    bl_label = "Apply Eye Scale"
    bl_description = "Apply the eye scale setting to the character"
    
    def execute(self, context):
        props = context.scene.minime_props
        scale = props.eye_scale
        
        # Find eye bones or objects
        found = False
        for obj in bpy.context.selected_objects:
            if obj.type == 'ARMATURE':
                for bone_name in ['eye_L', 'eye_R', 'Eye.L', 'Eye.R']:
                    if bone_name in obj.pose.bones:
                        bone = obj.pose.bones[bone_name]
                        bone.scale = (scale, scale, scale)
                        found = True
        
        if found:
            self.report({'INFO'}, f"Eye scale set to {scale}")
        else:
            self.report({'WARNING'}, "No eye bones found in selected armature")
        
        return {'FINISHED'}


class MINIME_OT_play_animation(Operator):
    """Play a character animation."""
    bl_idname = "minime.play_animation"
    bl_label = "Play Animation"
    bl_description = "Play the specified animation"
    
    animation: StringProperty(default="idle")
    
    def execute(self, context):
        anim_name = self.animation
        
        # Find active armature
        obj = bpy.context.active_object
        if obj and obj.type == 'ARMATURE':
            action = bpy.data.actions.get(anim_name)
            if action:
                if obj.animation_data is None:
                    obj.animation_data_create()
                obj.animation_data.action = action
                
                # Start playback
                bpy.ops.screen.animation_play()
                
                self.report({'INFO'}, f"Playing animation: {anim_name}")
            else:
                self.report({'WARNING'}, f"Animation '{anim_name}' not found")
        else:
            self.report({'WARNING'}, "No armature selected")
        
        return {'FINISHED'}


class MINIME_OT_create_animation_actions(Operator):
    """Create placeholder animation actions."""
    bl_idname = "minime.create_animation_actions"
    bl_label = "Create Animation Placeholders"
    bl_description = "Create placeholder actions for required animations"
    
    def execute(self, context):
        animations = ['idle', 'dance', 'wave', 'wai', 'curtsy', 'bow']
        
        for anim in animations:
            if anim not in bpy.data.actions:
                action = bpy.data.actions.new(anim)
                action.use_fake_user = True
                self.report({'INFO'}, f"Created action: {anim}")
        
        self.report({'INFO'}, f"Created {len(animations)} animation placeholders")
        return {'FINISHED'}


class MINIME_OT_export_character(Operator):
    """Export character for web/mobile deployment."""
    bl_idname = "minime.export_character"
    bl_label = "Export Character"
    bl_description = "Export the character in a format suitable for deployment"
    
    def execute(self, context):
        # Get export path
        export_path = bpy.path.abspath("//exports/")
        
        # Create exports directory if needed
        import os
        os.makedirs(export_path, exist_ok=True)
        
        props = context.scene.minime_props
        filename = f"{props.character_name}.glb"
        filepath = os.path.join(export_path, filename)
        
        # Export as GLB (best for web)
        bpy.ops.export_scene.gltf(
            filepath=filepath,
            export_format='GLB',
            export_animations=True,
            export_materials='EXPORT'
        )
        
        self.report({'INFO'}, f"Exported to: {filepath}")
        return {'FINISHED'}


# ============================================================================
# Panels
# ============================================================================

class MINIME_PT_main_panel(Panel):
    """Main Mini-Me panel in the 3D View sidebar."""
    bl_label = "Sangsom Mini-Me"
    bl_idname = "MINIME_PT_main_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "Mini-Me"
    
    def draw(self, context):
        layout = self.layout
        props = context.scene.minime_props
        
        # Project Setup
        box = layout.box()
        box.label(text="Project Setup", icon='SETTINGS')
        box.operator("minime.setup_project", icon='PLAY')


class MINIME_PT_character_panel(Panel):
    """Character management panel."""
    bl_label = "Character"
    bl_idname = "MINIME_PT_character_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "Mini-Me"
    bl_parent_id = "MINIME_PT_main_panel"
    
    def draw(self, context):
        layout = self.layout
        props = context.scene.minime_props
        
        layout.prop(props, "character_name")
        layout.operator("minime.create_character_template", icon='OUTLINER_OB_ARMATURE')
        
        # Customization
        layout.separator()
        layout.label(text="Customization:")
        
        row = layout.row(align=True)
        row.prop(props, "eye_scale")
        row.operator("minime.apply_eye_scale", text="", icon='CHECKMARK')
        
        layout.prop(props, "happiness")


class MINIME_PT_animation_panel(Panel):
    """Animation management panel."""
    bl_label = "Animations"
    bl_idname = "MINIME_PT_animation_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "Mini-Me"
    bl_parent_id = "MINIME_PT_main_panel"
    
    def draw(self, context):
        layout = self.layout
        
        layout.operator("minime.create_animation_actions", icon='ACTION')
        
        layout.separator()
        layout.label(text="Play Animation:")
        
        # Animation buttons
        animations = [
            ("idle", "Idle", 'ARMATURE_DATA'),
            ("dance", "Dance", 'MOD_WAVE'),
            ("wave", "Wave", 'HAND'),
            ("wai", "Wai (Thai)", 'CON_TRANSLIKE'),
            ("curtsy", "Curtsy", 'MOD_CLOTH'),
            ("bow", "Bow", 'PIVOT_CURSOR'),
        ]
        
        col = layout.column(align=True)
        for anim_id, anim_name, icon in animations:
            op = col.operator("minime.play_animation", text=anim_name, icon=icon)
            op.animation = anim_id


class MINIME_PT_export_panel(Panel):
    """Export panel."""
    bl_label = "Export"
    bl_idname = "MINIME_PT_export_panel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = "Mini-Me"
    bl_parent_id = "MINIME_PT_main_panel"
    
    def draw(self, context):
        layout = self.layout
        
        layout.operator("minime.export_character", icon='EXPORT')


# ============================================================================
# Registration
# ============================================================================

classes = (
    MiniMeProperties,
    MINIME_OT_setup_project,
    MINIME_OT_create_character_template,
    MINIME_OT_apply_eye_scale,
    MINIME_OT_play_animation,
    MINIME_OT_create_animation_actions,
    MINIME_OT_export_character,
    MINIME_PT_main_panel,
    MINIME_PT_character_panel,
    MINIME_PT_animation_panel,
    MINIME_PT_export_panel,
)


def register():
    """Register the addon."""
    for cls in classes:
        bpy.utils.register_class(cls)
    
    bpy.types.Scene.minime_props = bpy.props.PointerProperty(type=MiniMeProperties)
    
    print("Sangsom Mini-Me Tools addon registered")


def unregister():
    """Unregister the addon."""
    for cls in reversed(classes):
        bpy.utils.unregister_class(cls)
    
    del bpy.types.Scene.minime_props
    
    print("Sangsom Mini-Me Tools addon unregistered")


if __name__ == "__main__":
    register()
