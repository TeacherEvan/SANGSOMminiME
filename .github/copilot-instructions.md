# Sangsom Mini-Me: Educational Tamagotchi Development Instructions

**ALWAYS FOLLOW THESE INSTRUCTIONS FIRST.** Only fallback to additional search and context gathering if the information here is incomplete or found to be in error.

## Project Overview
Sangsom Mini-Me is an educational 3D tamagotchi-style Blender project where students nurture AI-generated personalized Mini-Me characters through homework completion. This project emphasizes AI-assisted development using Cursor AI with Blender 5.0.0.

## Working Effectively

### Initial Environment Setup
**CRITICAL**: Complete ALL these steps before starting development:

```bash
# Install Blender 5.0.0
# NOTE: Blender installation requires manual download
# Download from: https://www.blender.org/download/
# Install Blender 5.0.0
# Blender path typically: /usr/bin/blender or C:\Program Files\Blender Foundation\Blender 5.0

# Verify Python (should already be available with Blender)
python --version  # Expected: 3.11 or higher

# Verify Git is configured
git --no-pager config --list | grep user
```

### Project Structure Validation
**ALWAYS validate project structure before coding:**
```bash
# Verify Python modules exist
find Assets/Scripts -name "*.py" -exec echo "Found: {}" \;
# Expected output:
# Found: Assets/Scripts/Runtime/__init__.py  
# Found: Assets/Scripts/Editor/__init__.py

# Test Python with project patterns
mkdir -p /tmp/blender-test && cd /tmp/blender-test
cat > test_controller.py << 'EOF'
class TestController:
    def validate_pattern(self):
        print("SangsomMiniMe namespace works!")

if __name__ == "__main__":
    controller = TestController()
    controller.validate_pattern()
EOF

# Run test - NEVER CANCEL: Takes ~1 second, set timeout to 30+ seconds
python test_controller.py
# Expected output: "SangsomMiniMe namespace works!"
```

### Blender Development Workflow
**Blender Editor Operations:**
```bash
# CRITICAL: Blender can be scripted via Python but GUI operations are manual
# All Blender operations can be done through Blender Editor GUI
# Project files are at: /home/runner/work/SANGSOMminiME/SANGSOMminiME

# To work with Blender:
# 1. Open Blender
# 2. Open project files from: /home/runner/work/SANGSOMminiME/SANGSOMminiME
# 3. Open with Blender 5.0.0
```

## Building and Testing

### Python Script Development
**Use Python for rapid script validation:**
```bash
# Always test Python scripts before adding to Blender
cd /tmp && mkdir blender-script-test && cd blender-script-test

# Create test script following Blender patterns:
cat > test_script.py << 'EOF'
class CharacterController:
    def __init__(self):
        self.happiness = 100.0
    
    def complete_homework(self, points: int) -> None:
        self.happiness += points * 10
        print(f"Homework completed! Happiness: {self.happiness}")

if __name__ == "__main__":
    character = CharacterController()
    character.complete_homework(5)
    print("Blender script pattern validated!")
EOF

# Run test - NEVER CANCEL: Takes 1 second
python test_script.py
```

### Git Operations
**ALWAYS use --no-pager flag to avoid timeouts:**
```bash
# Check project status
git --no-pager status

# View recent changes
git --no-pager log --oneline -5

# View current diff
git --no-pager diff

# Stage and commit changes
git add Assets/Scripts/Runtime/your_new_script.py
git commit -m "Add new character controller script"
```

## Validation Requirements

### ALWAYS Validate Changes
**MANDATORY**: After ANY code changes, run these validation steps:

```bash
# 1. Validate Python syntax (NEVER CANCEL - timeout: 30+ seconds)
cd /tmp/validation && mkdir -p test && cd test
# Copy your new script and test with python

# 2. Check Blender project integrity
# Open Blender and verify:
# - No Python errors in Console
# - Scripts load correctly
# - Scripts appear in Text Editor

# 3. Test character workflow if modifying logic
# Create test character, complete homework, verify happiness increase
```

### Manual Testing Scenarios
**CRITICAL**: Execute these scenarios after changes to core systems:

1. **Character Creation Test**:
   - Use sample photos in `Assets/Characters/Leandi/Photos/`
   - Verify character initialization and happiness system
   - Test homework completion mechanics

2. **Cursor AI Integration Test**:
   - Use AI prompts from `.cursor/rules/` directory
   - Generate code following project patterns
   - Validate namespace usage: `SangsomMiniMe` for runtime, `SangsomMiniMe.Editor` for editor

## Timing and Performance Expectations

### Build Times (NEVER CANCEL these operations)
- **Python validation**: 1-2 seconds (timeout: 30+ seconds)
- **Blender script loading**: 5-15 seconds (timeout: 60+ seconds)  
- **Blender project open**: 10-60 seconds first time (timeout: 180+ seconds)
- **Blender render**: 1-30 minutes depending on complexity (timeout: varies)

### Common Issues and Solutions
```bash
# If Blender won't open project:
# - Verify Blender 5.0.0 is installed
# - Check that all files in Assets/ directory are accessible
# - Allow Blender to load assets on first open

# If Python errors:
python --version  # Verify Python 3.11+ is available
# Check namespace usage matches module structure

# If Git operations hang:
# ALWAYS use --no-pager flag for git commands
```

## Key Project Locations

### Essential Directories
```
SANGSOMminiME/
├── Assets/
│   ├── Characters/Leandi/Photos/          # Sample photos for AI model generation
│   ├── Scripts/Runtime/                   # Main Python scripts
│   └── Scripts/Editor/                    # Editor Python scripts
├── .cursor/rules/                         # AI development rules
│   ├── core-rules/                        # Project-wide AI guidelines
│   ├── blender-rules/                     # Blender-specific standards
│   ├── educational-rules/                 # Educational game principles
│   └── character-rules/                   # Character generation rules
├── README.md                              # Comprehensive project documentation
├── History2.md                            # Development session history
└── SangsomMini-Me.mdc                     # Updated project specification
```

### Important Files to Monitor
```bash
ls -la Assets/Scripts/Runtime/ Assets/Scripts/Editor/    # Python scripts
ls -la .cursor/rules/core-rules/          # AI development rules
ls -la Assets/Characters/Leandi/Photos/   # Sample character data
```

## Development Standards

### Code Organization
- **Namespace pattern**: `SangsomMiniMe.{Folder}.{Subfolder}`
- **Runtime scripts**: Use `SangsomMiniMe` namespace
- **Editor scripts**: Use `SangsomMiniMe.Editor` namespace
- **File naming**: Match class names exactly

### Educational Game Principles
- **No stress mechanics**: No timers or failure states
- **Homework integration**: Academic completion drives character wellbeing
- **Authentic nurturing**: Characters develop meaningful student relationships
- **Cultural sensitivity**: Proper Thai gesture implementation (wai, curtsy, bow)

### AI-Assisted Development
**Cursor AI Integration**: Use rules from `.cursor/rules/` for:
- Blender Python code generation following project standards
- Educational game design principles
- Character customization systems
- Multi-user account management

**Example AI Prompts**:
```
"Create a character customization system with scalable eye sizes using Blender Python following SangsomMiniMe namespace conventions"

"Generate homework integration that increases character happiness when academic tasks complete"

"Build animation controller for Thai cultural gestures: wai, curtsy, bow with smooth transitions"
```

## Quick Reference Commands

### Daily Development Workflow
```bash
# 1. Check project status
git --no-pager status

# 2. Test Python scripts before Blender integration
mkdir -p /tmp/test && cd /tmp/test

# 3. Validate Blender project (open Blender)
# Launch Blender → Open project → Wait for loading (60+ seconds)

# 4. Commit validated changes
git add . && git commit -m "Descriptive message"

# 5. Always verify script integrity before finishing
python -m py_compile your_script.py  # For syntax validation
```

### Emergency Debugging
```bash
# If stuck, validate basic environment:
python --version                    # Should show 3.11+
git --no-pager log --oneline -3    # Recent commits
find Assets -name "*.py" -exec echo "Found: {}" \;       # Python scripts exist
ls -la .cursor/rules/*/             # AI rules are present
```

**REMEMBER**: This is an AI-assisted Blender project. Always leverage Cursor AI rules for code generation and follow the educational game design principles for authentic nurturing mechanics without stress elements.