# Sangsom Mini-Me: Educational Tamagotchi Development Instructions

**ALWAYS FOLLOW THESE INSTRUCTIONS FIRST.** Only fallback to additional search and context gathering if the information here is incomplete or found to be in error.

## Project Overview
Sangsom Mini-Me is an educational 3D tamagotchi-style Unity game where students nurture AI-generated personalized Mini-Me characters through homework completion. This project emphasizes AI-assisted development using Cursor AI with Unity 2022.3 LTS.

## Working Effectively

### Initial Environment Setup
**CRITICAL**: Complete ALL these steps before starting development:

```bash
# Install Unity 2022.3 LTS (Linux/Ubuntu)
# NOTE: Unity installation requires manual download due to licensing
# Download Unity Hub from: https://unity3d.com/get-unity/download
# Install Unity 2022.3 LTS through Unity Hub
# Unity Editor path typically: ~/.local/share/UnityHub/Editor/2022.3.XX/Editor/Unity

# Verify .NET SDK (should already be available)
dotnet --version  # Expected: 8.0.119 or higher

# Verify Git is configured
git --no-pager config --list | grep user
```

### Project Structure Validation
**ALWAYS validate project structure before coding:**
```bash
# Verify assembly definitions exist
find Assets/Scripts -name "*.asmdef" -exec echo "Found: {}" \;
# Expected output:
# Found: Assets/Scripts/Runtime/Runtime.asmdef  
# Found: Assets/Scripts/Editor/Editor.asmdef

# Test C# compilation with project patterns
mkdir -p /tmp/unity-test && cd /tmp/unity-test
cat > test.csproj << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RootNamespace>SangsomMiniMe</RootNamespace>
  </PropertyGroup>
</Project>
EOF

cat > TestController.cs << 'EOF'
using System;
namespace SangsomMiniMe {
    public class TestController {
        public void ValidatePattern() => Console.WriteLine("SangsomMiniMe namespace works!");
    }
    class Program { static void Main() => new TestController().ValidatePattern(); }
}
EOF

# Build test - NEVER CANCEL: Takes ~4 seconds, set timeout to 30+ seconds
dotnet build && dotnet run
# Expected output: "SangsomMiniMe namespace works!"
```

### Unity Development Workflow
**Unity Editor Operations:**
```bash
# CRITICAL: Unity cannot be automated from command line in this environment
# All Unity operations must be done through Unity Editor GUI
# Unity project files are at: /home/runner/work/SANGSOMminiME/SANGSOMminiME

# To work with Unity:
# 1. Open Unity Hub
# 2. Add project: /home/runner/work/SANGSOMminiME/SANGSOMminiME
# 3. Open with Unity 2022.3 LTS
# 4. Unity will create missing ProjectSettings/, Packages/, UserSettings/ on first open
```

## Building and Testing

### C# Script Development
**Use .NET for rapid script validation:**
```bash
# Always test C# scripts before adding to Unity
cd /tmp && mkdir unity-script-test && cd unity-script-test
dotnet new console --force

# Create test script following Unity patterns:
cat > Program.cs << 'EOF'
using System;
namespace SangsomMiniMe {
    public class CharacterController {
        private float happiness = 100f;
        public void CompleteHomework(int points) {
            happiness += points * 10;
            Console.WriteLine($"Homework completed! Happiness: {happiness}");
        }
    }
    class Program {
        static void Main() {
            var character = new CharacterController();
            character.CompleteHomework(5);
            Console.WriteLine("Unity script pattern validated!");
        }
    }
}
EOF

# Build and test - NEVER CANCEL: Takes 3-5 seconds
dotnet build && dotnet run
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
git add Assets/Scripts/Runtime/YourNewScript.cs
git commit -m "Add new character controller script"
```

## Validation Requirements

### ALWAYS Validate Changes
**MANDATORY**: After ANY code changes, run these validation steps:

```bash
# 1. Validate C# compilation (NEVER CANCEL - timeout: 30+ seconds)
cd /tmp/validation && mkdir -p test && cd test
# Copy your new script and test compilation with dotnet build

# 2. Check Unity project integrity
# Open Unity Editor and verify:
# - No compilation errors in Console
# - Assembly definitions resolve correctly
# - Scripts appear in Project window

# 3. Test character workflow if modifying game logic
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
- **.NET compilation**: 3-5 seconds (timeout: 30+ seconds)
- **Unity script compilation**: 10-30 seconds (timeout: 60+ seconds)  
- **Unity project open**: 30-120 seconds first time (timeout: 180+ seconds)
- **Unity build (WebGL)**: 5-15 minutes (timeout: 30+ minutes)

### Common Issues and Solutions
```bash
# If Unity won't open project:
# - Verify Unity 2022.3 LTS is installed
# - Check that all files in Assets/ directory are accessible
# - Allow Unity to create ProjectSettings/ on first open

# If C# compilation fails:
dotnet --version  # Verify .NET 8.0+ is available
# Check namespace usage matches assembly definitions

# If Git operations hang:
# ALWAYS use --no-pager flag for git commands
```

## Key Project Locations

### Essential Directories
```
SANGSOMminiME/
├── Assets/
│   ├── Characters/Leandi/Photos/          # Sample photos for AI model generation
│   ├── Scripts/Runtime/Runtime.asmdef     # Main assembly definition
│   └── Scripts/Editor/Editor.asmdef       # Editor assembly definition
├── .cursor/rules/                         # AI development rules
│   ├── core-rules/                        # Project-wide AI guidelines
│   ├── unity-rules/                       # Unity-specific standards
│   ├── educational-rules/                 # Educational game principles
│   └── character-rules/                   # Character generation rules
├── README.md                              # Comprehensive project documentation
├── History2.md                            # Development session history
└── SangsomMini-Me.mdc                     # Updated project specification
```

### Important Files to Monitor
```bash
ls -la Assets/Scripts/Runtime/Runtime.asmdef Assets/Scripts/Editor/Editor.asmdef    # Assembly definitions
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
- Unity C# code generation following project standards
- Educational game design principles
- Character customization systems
- Multi-user account management

**Example AI Prompts**:
```
"Create a character customization system with scalable eye sizes using Unity sliders following SangsomMiniMe namespace conventions"

"Generate homework integration that increases character happiness when academic tasks complete"

"Build animation controller for Thai cultural gestures: wai, curtsy, bow with smooth transitions"
```

## Quick Reference Commands

### Daily Development Workflow
```bash
# 1. Check project status
git --no-pager status

# 2. Test C# scripts before Unity integration
mkdir -p /tmp/test && cd /tmp/test && dotnet new console --force

# 3. Validate Unity project (open Unity Editor)
# Launch Unity Hub → Open project → Wait for compilation (60+ seconds)

# 4. Commit validated changes
git add . && git commit -m "Descriptive message"

# 5. Always verify build integrity before finishing
dotnet build  # For any .NET test projects
```

### Emergency Debugging
```bash
# If stuck, validate basic environment:
dotnet --version                    # Should show 8.0.119+
git --no-pager log --oneline -3    # Recent commits
find Assets -name "*.asmdef" -exec echo "Found: {}" \;       # Assembly definitions exist
ls -la .cursor/rules/*/             # AI rules are present
```

**REMEMBER**: This is an AI-assisted Unity project. Always leverage Cursor AI rules for code generation and follow the educational game design principles for authentic nurturing mechanics without stress elements.