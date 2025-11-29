import bpy
import os
import sys

def install():
    # Calculate path to minime_addon.py relative to this script
    script_dir = os.path.dirname(os.path.abspath(__file__))
    addon_path = os.path.join(script_dir, "minime_addon.py")
    
    if not os.path.exists(addon_path):
        print(f"Error: Addon file not found at {addon_path}")
        sys.exit(1)

    print(f"Installing addon from: {addon_path}")
    
    try:
        # Install
        bpy.ops.preferences.addon_install(filepath=addon_path)
        
        # Enable
        # The module name is the filename without extension
        module_name = "minime_addon"
        
        # Check if already enabled to avoid error, or just enable it
        if bpy.context.preferences and module_name not in bpy.context.preferences.addons:
            bpy.ops.preferences.addon_enable(module=module_name)
            
        # Save preferences so it persists
        bpy.ops.wm.save_userpref()
        print("✅ Successfully installed and enabled 'Sangsom Mini-Me Tools'")
    except Exception as e:
        print(f"❌ Error installing addon: {e}")
        sys.exit(1)

if __name__ == "__main__":
    install()
