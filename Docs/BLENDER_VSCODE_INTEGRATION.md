# Blender ↔ VSCode Integration Guide

Complete workflow integration for Blender Python development using VSCode for the Sangsom Mini-Me project.

---

## 🎯 **Overview**

This guide establishes a seamless workflow between:
- **VSCode**: Primary code editor with IntelliSense, debugging, version control
- **Blender 5.0.0**: Asset creation, animation, rendering
- **Unity 2022.3.12f1**: Runtime target (see separate Unity docs)

---

## 🔧 **Prerequisites**

### Required Software
- ✅ **Blender 5.0.0** installed
- ✅ **VSCode** configured (migrated from Cursor AI)
- ✅ **Python 3.11+** (matches Blender's Python version)
- ✅ **Git** for version control

### VSCode Extensions (Install These)

```bash
# ESSENTIAL - Blender Integration (launches Blender, runs scripts, debugging)
code --install-extension JacquesLucke.blender-development

# ESSENTIAL - Python Development
code --install-extension ms-python.python
code --install-extension ms-python.vscode-pylance

# Recommended
code --install-extension ms-vscode.powershell
code --install-extension GitHub.copilot
```

> **Important**: The `JacquesLucke.blender-development` extension is the core of this integration. It provides `Blender: Start`, `Blender: Run Script`, and debugging capabilities.

---

## 📁 **Project Structure for Blender Integration**

```
SANGSOMminiME/
├── Blender/                    # ← Blender Python scripts
│   ├── startup_script.py       # Project initialization
│   ├── character_controller.py # Animation/customization
│   ├── user_manager.py         # User profiles
│   ├── minime_addon.py         # Blender addon UI
│   └── game_manager.py         # Game loop
├── Assets/
│   ├── Characters/Leandi/      # ← Export target
│   ├── Art/                    # Textures, materials
│   └── Scripts/Runtime/        # Unity C# (separate pipeline)
├── .vscode/
│   ├── settings.json           # Python paths, Blender integration
│   ├── tasks.json              # ← Automation tasks (we'll create this)
│   └── launch.json             # ← Debugging config (we'll create this)
└── scripts/
    └── blender-automation.js   # ← Node.js automation (we'll create this)
```

---

## ⚙️ **Step 1: Configure Blender to Use VSCode**

### 1.1 Set VSCode as External Script Editor (Optional)

> **Note**: This step is OPTIONAL. The primary workflow uses the **Blender Development extension** (Step 1.3) which launches Blender FROM VSCode. This setting only enables "Edit Externally" from within Blender.

In Blender:
1. `Edit` → `Preferences` → `File Paths`
2. Scroll down to the **Applications** section
3. Under **Text Editor**:
   - **Program**: Set the path to VSCode:
     - **Windows**: `C:\Users\eboth\AppData\Local\Programs\Microsoft VS Code\Code.exe`
     - **macOS**: `/Applications/Visual Studio Code.app/Contents/MacOS/Electron`
     - **Linux**: `/usr/bin/code`
   - **Arguments**: `-g $filepath:$line:$column` (this tells VSCode to open the file at the specific line/column)
4. Click the **☰** menu (hamburger icon) → **Save Preferences**

**Usage**: In Blender's Text Editor, use `Text` → `Edit Externally` (or place cursor and press the shortcut) to open the current script in VSCode.

### 1.2 Install the Blender Development Extension (REQUIRED)

This is the **PRIMARY** integration method. The extension launches Blender from VSCode and provides:
- Script execution with `Blender: Run Script`
- Full debugging with breakpoints
- Addon hot-reloading with `Blender: Reload Addons`
- Addon scaffolding with `Blender: New Addon`

**Install**:

```bash
code --install-extension JacquesLucke.blender-development
```

**First-Time Setup**:
1. Open VSCode in this project folder
2. Press `Ctrl+Shift+P` → type `Blender: Start`
3. Select your Blender executable (e.g., `C:\Program Files\Blender Foundation\Blender 5.0\blender.exe`)
4. Wait for Blender to launch (first run installs Python dependencies automatically)

**Daily Workflow**:
- `Ctrl+Shift+P` → `Blender: Start` - Launch Blender
- `Ctrl+Shift+P` → `Blender: Run Script` - Execute current Python file in Blender
- `Ctrl+Shift+P` → `Blender: Reload Addons` - Hot-reload after code changes

> **Tip**: Add `"blender.executables": [{"path": "C:\\Program Files\\Blender Foundation\\Blender 5.0\\blender.exe", "isDefault": true}]` to `.vscode/settings.json` to skip the executable prompt.

### 1.3 Enable Blender CLI in Terminal (Optional)

Add to your PowerShell profile (opens with `notepad $PROFILE`):

```powershell
# Blender CLI alias
function Invoke-Blender {
    & "C:\Program Files\Blender Foundation\Blender 5.0\blender.exe" $args
}
Set-Alias blender Invoke-Blender
```

**Test**:

```powershell
blender --version  # Should show Blender 5.0.0
```

---

## 🐍 **Step 2: Setup VSCode Python Environment**

### 2.1 Install Blender Python Stubs

These enable IntelliSense for Blender API in VSCode:

```powershell
# Activate project virtual environment
.\.venv\Scripts\Activate.ps1

# Install fake-bpy-module for Blender 5.0
pip install fake-bpy-module-5.0

# Verify installation
pip list | Select-String "fake-bpy"
```

### 2.2 Update VSCode Settings

Merge this into `.vscode/settings.json`:

```jsonc
{
    "python.defaultInterpreterPath": "${workspaceFolder}/.venv/Scripts/python.exe",
    "python.analysis.extraPaths": [
        "./Assets/Scripts/Runtime",
        "./Assets/Scripts/Editor",
        "./Blender"  // ← Added for Blender scripts
    ],
    "python.autoComplete.extraPaths": [
        "./Blender",
        "${env:APPDATA}/../Local/Programs/Python/Python311/Lib/site-packages"
    ],
    // Blender Python linting exceptions
    "python.linting.pylintArgs": [
        "--disable=C0111,C0103,C0301",  // Allow Blender naming conventions
        "--extension-pkg-whitelist=bpy"
    ],
    // File associations
    "files.associations": {
        "*.blend": "binary",
        "*.blend1": "binary"
    },
    "files.watcherExclude": {
        "**/*.blend1": true,
        "**/__pycache__": true
    }
}
```

---

## 🔨 **Step 3: Create VSCode Tasks for Blender Automation**

### 3.1 Create `.vscode/tasks.json`

This file automates Blender operations from VSCode:

```jsonc
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "Blender: Run Startup Script",
            "type": "shell",
            "command": "blender",
            "args": [
                "--background",
                "--python",
                "${workspaceFolder}/Blender/startup_script.py"
            ],
            "problemMatcher": [],
            "presentation": {
                "echo": true,
                "reveal": "always",
                "panel": "new"
            },
            "group": {
                "kind": "build",
                "isDefault": false
            }
        },
        {
            "label": "Blender: Export Leandi Character",
            "type": "shell",
            "command": "blender",
            "args": [
                "--background",
                "${workspaceFolder}/Blender/characters/leandi.blend",
                "--python",
                "${workspaceFolder}/Blender/export_character.py"
            ],
            "problemMatcher": [],
            "group": "build"
        },
        {
            "label": "Blender: Open in GUI",
            "type": "shell",
            "command": "blender",
            "args": [
                "${workspaceFolder}/Blender/characters/leandi.blend"
            ],
            "isBackground": true,
            "problemMatcher": []
        },
        {
            "label": "Blender: Install Addon",
            "type": "shell",
            "command": "blender",
            "args": [
                "--background",
                "--python-expr",
                "import bpy; bpy.ops.preferences.addon_install(filepath='${workspaceFolder}/Blender/minime_addon.py'); bpy.ops.preferences.addon_enable(module='minime_addon'); bpy.ops.wm.save_userpref()"
            ],
            "problemMatcher": []
        },
        {
            "label": "Blender: Validate All Scripts",
            "type": "shell",
            "command": "python",
            "args": [
                "-m",
                "compileall",
                "."
            ],
            "options": {
                "cwd": "${workspaceFolder}/Blender"
            },
            "problemMatcher": {
                "owner": "python",
                "fileLocation": ["relative", "${workspaceFolder}/Blender"],
                "pattern": {
                    "regexp": "^  File \"(.*)\", line (\\d+)",
                    "file": 1,
                    "line": 2
                }
            },
            "group": "test"
        }
    ]
}
```

**Usage**:
- `Ctrl+Shift+P` → `Tasks: Run Task` → Select a task
- Or set keyboard shortcut: `Ctrl+Shift+B` runs default build task

---

## 🐛 **Step 4: Setup Debugging**

Debugging works automatically when you start Blender via the extension. No manual setup required!

### 4.1 Basic Debugging (Recommended)

When Blender is started via `Blender: Start`:
1. Set breakpoints in VSCode by clicking left of line numbers
2. Run your script with `Ctrl+Shift+P` → `Blender: Run Script`
3. Execution pauses at breakpoints automatically
4. Use the debug toolbar to step through code

### 4.2 Advanced: Manual debugpy Setup (Optional)

For debugging scripts run directly in Blender (not via the extension):

**Install `debugpy` in Blender's Python**:

```powershell
# Find Blender's Python executable
blender --background --python-expr "import sys; print(sys.executable)"

# Example output: C:\Program Files\Blender Foundation\Blender 5.0\5.0\python\bin\python.exe

# Install debugpy into Blender's Python
& "C:\Program Files\Blender Foundation\Blender 5.0\5.0\python\bin\python.exe" -m pip install debugpy
```

### 4.2 Create `.vscode/launch.json`

```jsonc
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Blender: Attach Debugger",
            "type": "python",
            "request": "attach",
            "connect": {
                "host": "localhost",
                "port": 5678
            },
            "pathMappings": [
                {
                    "localRoot": "${workspaceFolder}/Blender",
                    "remoteRoot": "${workspaceFolder}/Blender"
                }
            ]
        },
        {
            "name": "Python: Current Blender Script",
            "type": "python",
            "request": "launch",
            "program": "${file}",
            "console": "integratedTerminal",
            "env": {
                "PYTHONPATH": "${workspaceFolder}/Blender"
            }
        }
    ]
}
```

### 4.3 Add Debugging Hook to Blender Scripts

In any Blender script you want to debug (e.g., `character_controller.py`), add:

```python
import bpy

def enable_debugging():
    """Start debugpy server for VSCode attachment."""
    try:
        import debugpy
        if not debugpy.is_client_connected():
            debugpy.listen(("localhost", 5678))
            print("🐛 Debugger listening on localhost:5678")
            print("   Attach VSCode debugger now (F5)")
            debugpy.wait_for_client()  # Optional: pause until attached
            print("✅ VSCode debugger attached!")
    except ImportError:
        print("⚠️ debugpy not installed. Run: blender --background --python-expr 'import pip; pip.main([\"install\", \"debugpy\"])'")

# Call at script start
enable_debugging()

# Your Blender code with breakpoints
class CharacterController:
    def play_dance(self):
        print("Playing dance animation...")  # ← Set breakpoint here in VSCode
        # ... rest of code
```

**Debugging Workflow**:
1. Add `enable_debugging()` call to your Blender script
2. Run script in Blender (either GUI or `blender --python script.py`)
3. Script will pause, waiting for debugger
4. In VSCode: `F5` → Select "Blender: Attach Debugger"
5. Execution continues; breakpoints now work!

---

## 🤖 **Step 5: Automated Export Pipeline**

### 5.1 Create Export Automation Script

Create `Blender/export_character.py` with a modular design:

```python
"""
Automated character export script
Run with: blender --background leandi.blend --python export_character.py
"""

import bpy
import os
from pathlib import Path

def get_project_root() -> Path:
    """Get the project root directory."""
    if bpy.data.filepath:
        return Path(bpy.data.filepath).parent.parent
    # Fallback for unsaved files or other contexts
    return Path(os.getcwd()).parent

def export_character_logic(character_name: str | None = None, target_dir: Path | None = None) -> bool:
    """
    Core export logic reusable by CLI and Addon.
    
    Args:
        character_name: Name of the character (optional, auto-detected if None)
        target_dir: Target directory for export (optional, auto-detected if None)
    """
    project_root = get_project_root()
    
    # Determine character name if not provided
    if not character_name:
        character_name = "Leandi"  # Default
        if bpy.data.filepath:
            blend_name = Path(bpy.data.filepath).stem
            if blend_name and blend_name != "untitled":
                character_name = blend_name.title()
    
    # Setup export directory
    if not target_dir:
        export_dir = project_root / "Assets" / "Characters" / character_name
    else:
        export_dir = target_dir
        
    export_dir.mkdir(parents=True, exist_ok=True)
    
    # ... (Export logic for GLB and FBX) ...
    
    print(f"✅ Export complete for {character_name}!")
    return True

def export_character():
    """Wrapper for backward compatibility."""
    return export_character_logic()

if __name__ == "__main__":
    export_character()
```

### 5.2 Create Watch Script for Auto-Export

Create `scripts/blender-automation.js`:

```javascript
/**
 * Watches Blender files and triggers automatic exports
 * Run with: node scripts/blender-automation.js
 */

const fs = require('fs');
const path = require('path');
const { exec } = require('child_process');

const BLENDER_DIR = path.join(__dirname, '..', 'Blender', 'characters');
const BLENDER_EXE = 'blender'; // Assumes blender in PATH
const EXPORT_SCRIPT = path.join(__dirname, '..', 'Blender', 'export_character.py');

console.log('🔍 Watching Blender files for changes...');
console.log(`   Directory: ${BLENDER_DIR}`);

fs.watch(BLENDER_DIR, { recursive: true }, (eventType, filename) => {
    if (filename && filename.endsWith('.blend')) {
        console.log(`\n📦 Detected change: ${filename}`);
        
        const blendFile = path.join(BLENDER_DIR, filename);
        const cmd = `${BLENDER_EXE} --background "${blendFile}" --python "${EXPORT_SCRIPT}"`;
        
        console.log('⚙️  Running export...');
        exec(cmd, (error, stdout, stderr) => {
            if (error) {
                console.error(`❌ Export failed: ${error.message}`);
                return;
            }
            if (stderr && !stderr.includes('Info:')) {
                console.warn(`⚠️  Warnings: ${stderr}`);
            }
            console.log(stdout);
            console.log('✅ Export complete!');
        });
    }
});

console.log('Press Ctrl+C to stop watching...\n');
```

**Usage**:

```powershell
# In one terminal, start the watcher
node scripts/blender-automation.js

# In Blender, save changes to .blend files
# → Auto-exports to Assets/Characters/Leandi/
```

---

## 📝 **Step 6: Add npm Scripts**

Update `package.json` with Blender commands:

```jsonc
{
  "scripts": {
    "verify": "node scripts/verify.js",
    "lint:markdown": "markdownlint '**/*.md' --ignore node_modules --ignore .venv",
    "lint:python": "python -m py_compile Blender/*.py Assets/Scripts/**/*.py",
    
    // New Blender scripts
    "blender:watch": "node scripts/blender-automation.js",
    "blender:export": "blender --background Blender/characters/leandi.blend --python Blender/export_character.py",
    "blender:validate": "python -m py_compile Blender/*.py",
    "blender:install-addon": "blender --background --python-expr \"import bpy; bpy.ops.preferences.addon_install(filepath='Blender/minime_addon.py'); bpy.ops.preferences.addon_enable(module='minime_addon'); bpy.ops.wm.save_userpref()\"",
    "blender:test": "blender --background --python Blender/startup_script.py"
  }
}
```

**Usage**:

```powershell
npm run blender:validate      # Check syntax
npm run blender:export        # Export character
npm run blender:watch         # Auto-export on save
npm run blender:install-addon # Install Mini-Me addon
```

---

## 🎬 **Complete Workflow Example**

### Daily Development Loop

1. **Start Blender from VSCode** (Recommended)

   ```powershell
   # Press Ctrl+Shift+P in VSCode, then type:
   # Blender: Start
   # This launches Blender with debugging support
   ```

2. **Edit Blender Scripts in VSCode**

   ```powershell
   # Open any Python file
   code Blender/character_controller.py
   ```

3. **Run Scripts in Blender**

   ```powershell
   # With Blender running (started via extension):
   # Press Ctrl+Shift+P → Blender: Run Script
   # The current file executes in Blender immediately
   ```

4. **Debug Issues**
   - Set breakpoints in VSCode (click left of line numbers)
   - Run script via `Blender: Run Script`
   - Execution pauses at breakpoints automatically
   - Step through code with F10 (step over) / F11 (step into)

5. **Export to Unity**

   ```powershell
   # Manual export
   npm run blender:export
   
   # Or start auto-watcher
   npm run blender:watch
   # → Save .blend file
   # → Automatically exports to Assets/Characters/
   ```

6. **Verify Changes**

   ```powershell
   npm run blender:validate  # Syntax check
   npm run verify            # Full project check
   git status                # Review changes
   ```

---

## 🔍 **Troubleshooting**

### "Module 'bpy' not found" in VSCode

**Cause**: VSCode using wrong Python interpreter

**Fix**:
1. `Ctrl+Shift+P` → "Python: Select Interpreter"
2. Choose `.venv/Scripts/python.exe`
3. Install fake-bpy-module: `pip install fake-bpy-module-5.0`

---

### Debugger Won't Attach

**Cause**: `debugpy` not installed in Blender's Python

**Fix**:

```powershell
# Find Blender's Python
blender --background --python-expr "import sys; print(sys.executable)"

# Install debugpy
& "C:\...\blender.exe" -m pip install debugpy
```

---

### Addon Not Loading

**Cause**: Incorrect addon structure or Python version mismatch

**Fix**:
1. Check addon `bl_info`:

   ```python
   bl_info = {
       "name": "Sangsom Mini-Me Tools",
       "blender": (4, 0, 0),  # Minimum version
       "category": "Development",
   }
   ```

2. Install via VSCode task: `Ctrl+Shift+P` → "Tasks: Run Task" → "Blender: Install Addon"
3. Check Blender console: `Window` → `Toggle System Console`

---

### Export Script Fails

**Cause**: File paths or missing objects

**Fix**:

```python
# Add error handling to export_character.py
try:
    export_character()
except Exception as e:
    import traceback
    print(f"❌ Export failed: {e}")
    traceback.print_exc()
```

---

## 📚 **Additional Resources**

### Official Documentation
- [Blender Python API](https://docs.blender.org/api/current/)
- [VSCode Python Debugging](https://code.visualstudio.com/docs/python/debugging)
- [debugpy Documentation](https://github.com/microsoft/debugpy)

### Project-Specific
- [Blender README](../Blender/README.md) - Script usage guide
- [Setup Notes](SETUP_NOTES.md) - General project setup
- [Extensions Guide](EXTENSIONS_AND_TOOLS.md) - Recommended tools

### Learning Resources
- [CGCookie Blender Python](https://cgcookie.com/courses/scripting-for-artists-in-blender)
- [Blender Artists Forum](https://blenderartists.org/c/coding/python-support/)
- [Interplanety Blog](https://b3d.interplanety.org/en/) - Blender Python tutorials

---

## 🎯 **Quick Reference Commands**

```powershell
# === PRIMARY WORKFLOW (Recommended) ===
# In VSCode, press Ctrl+Shift+P then type:
Blender: Start                            # Launch Blender with debug support
Blender: Run Script                       # Execute current Python file
Blender: Reload Addons                    # Hot-reload addon after changes
Blender: New Addon                        # Scaffold a new addon project

# === Blender CLI (Alternative) ===
blender --version                          # Check version
blender --background                       # Headless mode
blender --python script.py                 # Run script
blender file.blend --python script.py     # Run script with file

# === VSCode Tasks (Ctrl+Shift+P → Tasks: Run Task) ===
Blender: Run Startup Script               # Initialize project
Blender: Export Leandi Character          # Export to Unity
Blender: Open in GUI                      # Open Blender
Blender: Validate All Scripts             # Syntax check

# === npm Scripts ===
npm run blender:validate                  # Lint Python
npm run blender:export                    # Export character
npm run blender:watch                     # Auto-export on save
npm run blender:install-addon             # Install addon
```

---

**Document Version**: 1.1  
**Last Updated**: November 29, 2025  
**Maintained By**: Sangsom Mini-Me Development Team
