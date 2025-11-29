# GameConfiguration Setup Guide

## Overview

The SANGSOMminiME project now uses a **ScriptableObject-based configuration system** that allows game designers to adjust game balance and behavior without touching code.

**Date Implemented**: November 29, 2025  
**Status**: ✅ Fully Integrated

---

## Quick Start

### 1. Create a Configuration Asset

1. Open Unity Editor
2. Navigate to your Project window
3. Right-click in `Assets/Resources/` (or any folder)
4. Select **Create → SangsomMiniMe → Game Configuration**
5. Name it `DefaultGameConfig`

### 2. Configure Settings

Select your newly created asset and configure in the Inspector:

#### User Settings
- **Starting Coins**: Default coins for new users (default: 100)
- **Starting Happiness**: Initial character happiness 0-100 (default: 75)
- **Starting Days Active**: Initial day count (default: 1)

#### Eye Customization
- **Min Eye Scale**: Minimum eye size multiplier (default: 0.5)
- **Max Eye Scale**: Maximum eye size multiplier (default: 2.0)

#### Homework Rewards
- **Experience Reward**: XP gained per homework (default: 10)
- **Coin Reward**: Coins earned per homework (default: 5)
- **Happiness Reward**: Happiness boost per homework (default: 5)

#### Happiness System
- **Happy Threshold**: Happiness level for happy particles (default: 70)
- **Sad Threshold**: Happiness level for sadness indicator (default: 30)
- **Dance Happiness Bonus**: Happiness gained from dancing (default: 2)

#### Auto-Save
- **Auto Save Interval**: Seconds between auto-saves (default: 30)
- **Enable Auto Save**: Toggle auto-save on/off

#### Animation & Levels
- **Animation Duration**: Default animation length in seconds (default: 2)
- **Experience Per Level**: XP needed per level (default: 100)

### 3. Assign to Scene

1. Open `MainScene.unity`
2. Select the **GameManager** GameObject
3. In the Inspector, find the **Configuration** section
4. Drag your `DefaultGameConfig` asset to the **Game Config** field
5. Select the **CharacterController** (on your character GameObject)
6. Assign the same config to its **Game Config** field

### 4. Test

Press Play and observe that your configured values are being used!

---

## Advanced Usage

### Creating Difficulty Presets

You can create multiple configurations for different difficulty modes:

**EasyMode.asset**:
- Starting Coins: 500
- Homework Rewards: 20 XP, 10 Coins
- Happy Threshold: 60

**HardMode.asset**:
- Starting Coins: 50
- Homework Rewards: 5 XP, 2 Coins
- Happy Threshold: 80

Then swap configs by assigning different assets to GameManager in different scenes or through code.

### Code Access

Developers can access config values programmatically:

```csharp
// From GameManager
int startingCoins = GameManager.Instance.Config.StartingCoins;

// From CharacterController (if you have a reference)
float happyThreshold = characterController.gameConfig.HappyThreshold;
```

### Fallback Behavior

If no GameConfiguration is assigned, the system automatically falls back to values in `GameConstants.cs`. This ensures the game always works, even without a config asset.

---

## Configuration Properties Reference

| Property | Type | Range | Default | Description |
|----------|------|-------|---------|-------------|
| StartingCoins | int | 0+ | 100 | Initial coins for new users |
| StartingHappiness | float | 0-100 | 75 | Initial character happiness |
| StartingDaysActive | int | 1+ | 1 | Starting day count |
| MinEyeScale | float | 0.1-1.0 | 0.5 | Minimum eye size multiplier |
| MaxEyeScale | float | 1.0-3.0 | 2.0 | Maximum eye size multiplier |
| HomeworkExperienceReward | int | 0+ | 10 | XP per homework |
| HomeworkCoinReward | int | 0+ | 5 | Coins per homework |
| HomeworkHappinessReward | float | 0-100 | 5 | Happiness per homework |
| HappyThreshold | float | 0-100 | 70 | Happiness for happy state |
| SadThreshold | float | 0-100 | 30 | Happiness for sad state |
| DanceHappinessBonus | float | 0+ | 2 | Happiness from dance |
| AutoSaveInterval | float | 5+ | 30 | Seconds between saves |
| EnableAutoSave | bool | - | true | Auto-save toggle |
| AnimationDuration | float | 0+ | 2 | Default animation length |
| ExperiencePerLevel | int | 1+ | 100 | XP needed per level |

---

## Validation

The GameConfiguration includes built-in validation:

- **Min Eye Scale** cannot exceed **Max Eye Scale**
- **Sad Threshold** cannot exceed **Happy Threshold**
- All numerical values are clamped to sensible ranges
- Validation runs automatically in the Unity Editor (OnValidate)

---

## Troubleshooting

**Q: My config changes aren't taking effect**  
A: Make sure you've assigned the config asset to both GameManager AND CharacterController in your scene.

**Q: Can I change config at runtime?**  
A: Yes! You can swap the config asset reference or modify values directly through code. Changes take effect immediately.

**Q: What if I don't assign a config?**  
A: The game will use default values from `GameConstants.cs`. Everything will still work normally.

**Q: Can I use different configs for different scenes?**  
A: Absolutely! Just assign different config assets to the managers in each scene.

---

## Next Steps

- Create your first GameConfiguration asset
- Experiment with different values
- Create difficulty presets (Easy, Normal, Hard)
- Share configs with your team through version control

Happy configuring! 🎮
