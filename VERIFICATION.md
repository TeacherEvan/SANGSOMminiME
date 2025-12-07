# Sangsom Mini-Me Verification System

## Quick Start

```bash
# Install dependencies
npm install

# Run full verification
npm run verify
```

## What Gets Verified

### ✅ Critical Checks (Must Pass)

- **Project Structure**: Required directories exist (Assets/Scripts/Runtime, Editor, Tests)
- **Unity Files**: Core C# scripts are present and in correct locations
- **Markdown Files**: Critical formatting rules pass

### ⚠️ Non-Critical Checks (Logged to JSON)

- **Markdown Formatting**: Style issues (code blocks, headings, lists)
- **Summary Statistics**: Tracked for monitoring

## Output

### Terminal

Shows real-time verification status with color-coded results:

- 🟢 Green: Passed
- 🔴 Red: Critical error
- 🟡 Yellow: Non-critical warning

### non-critical-errors.json

Stores all non-critical issues for tracking:

- Markdown linting warnings
- Summary statistics
- Last verification timestamp

## Available Scripts

```bash
# Full verification suite
npm run verify
```

## Unity Testing

For C# unit tests, use Unity's Test Runner:

1. Open Unity Editor
2. Window > General > Test Runner
3. Select PlayMode tab
4. Click Run All

## Verified Files

The verification script checks for the existence of these core files:

### Runtime Scripts

- `Assets/Scripts/Runtime/GameManager.cs`
- `Assets/Scripts/Runtime/UserManager.cs`
- `Assets/Scripts/Runtime/CharacterController.cs`
- `Assets/Scripts/Runtime/GameUI.cs`
- `Assets/Scripts/Runtime/UserProfile.cs`
- `Assets/Scripts/Runtime/GameConstants.cs`
- `Assets/Scripts/Runtime/GameEnums.cs`
- `Assets/Scripts/Runtime/GameUtilities.cs`
- `Assets/Scripts/Runtime/GameConfiguration.cs`
- `Assets/Scripts/Runtime/EducationalAnalytics.cs`

### Test Scripts

- `Assets/Scripts/Tests/UserProfileTests.cs`
- `Assets/Scripts/Tests/GameUtilitiesTests.cs`

### Editor Scripts

- `Assets/Scripts/Editor/SangsomMiniMeEditorTools.cs`

### Project Files

- `ProjectSettings/ProjectVersion.txt`

## Required Directories

- `Assets/Scripts/Runtime`
- `Assets/Scripts/Tests`
- `Assets/Scripts/Editor`
- `Assets/Scenes`
- `Assets/Prefabs`
- `Assets/Resources`
- `Docs`
- `.vscode`
