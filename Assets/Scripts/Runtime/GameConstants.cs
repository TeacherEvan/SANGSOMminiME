namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Game-wide constants for consistent values across the application.
    /// Centralized configuration prevents magic numbers and improves maintainability.
    /// </summary>
    public static class GameConstants
    {
        // User Profile Defaults
        public const int DefaultStartingCoins = 100;
        public const float DefaultStartingHappiness = 75f;
        public const float DefaultEyeScale = 1.0f;
        public const string DefaultOutfit = "default";
        public const string DefaultAccessory = "none";
        
        // Currency and Resource Limits (anti-cheat)
        public const int MaxCoins = 999999;
        public const int MaxExperience = 999999;
        public const int MaxHomeworkCompleted = 10000;
        
        // Eye Scale Limits
        public const float MinEyeScale = 0.5f;
        public const float MaxEyeScale = 2.0f;

        // Happiness Limits
        public const float MinHappiness = 0f;
        public const float MaxHappiness = 100f;

        // Happiness Thresholds for mood states
        public const float VeryHappyThreshold = 80f;
        public const float HappyThreshold = 60f;
        public const float NeutralThreshold = 40f;
        public const float SadThreshold = 20f;

        // Reward Values
        public const int HomeworkExperienceReward = 10;
        public const int HomeworkCoinReward = 5;
        public const float HomeworkHappinessReward = 5f;
        public const float DanceHappinessBonus = 2f;

        // Daily Login Bonus System (positive-only, no penalties)
        public const int DailyLoginBonusCoins = 5;
        public const float DailyLoginHappinessBonus = 3f;
        public const int MaxStreakBonusCoins = 10;

        // Streak Milestone Bonuses (celebration rewards)
        public const int Milestone3DayBonus = 10;
        public const int Milestone7DayBonus = 25;
        public const int Milestone14DayBonus = 50;
        public const int Milestone30DayBonus = 100;

        // Meter Decay System (gentle, with floors - no stress mechanics)
        public const float HappinessDecayPerMinute = 0.5f;   // Very slow decay
        public const float HungerDecayPerMinute = 1.0f;      // Moderate decay
        public const float EnergyDecayPerMinute = 0.75f;     // Slow decay

        // Meter Floors (meters never drop below these - no stress!)
        public const float HappinessFloor = 20f;   // Character stays "okay" at minimum
        public const float HungerFloor = 10f;      // Never starving
        public const float EnergyFloor = 15f;      // Always has some energy

        // Meter Defaults
        public const float DefaultHunger = 75f;
        public const float DefaultEnergy = 100f;

        // Meter Recovery Values
        public const float FeedHungerRecovery = 25f;
        public const float RestEnergyRecovery = 30f;
        public const float PlayHappinessBonus = 10f;

        // Care Action Costs (coins) - affordable to encourage engagement
        public const int FeedCost = 5;
        public const int RestCost = 3;
        public const int PlayCost = 0;  // Play is free!

        // Decay Check Interval (seconds)
        public const float MeterDecayInterval = 60f;  // Check every minute

        // Auto-Save Settings
        public const float DefaultAutoSaveInterval = 30f;
        public const float MinAutoSaveInterval = 10f;
        public const float MaxAutoSaveInterval = 600f;
        
        // Animation Settings
        public const float DefaultAnimationDuration = 2f;
        public const float MinAnimationDuration = 0.5f;
        public const float MaxAnimationDuration = 10f;
        
        // Level System
        public const int ExperiencePerLevel = 100;
        public const int MaxLevel = 999;
        
        // Resource Cache Settings
        public const int DefaultCacheSize = 50;
        public const int MaxCacheSize = 200;
        public const float CacheCleanupInterval = 60f;
        
        // File Paths
        public const string UserProfilesSaveFileName = "userProfiles.json";
        public const string ConfigurationResourcePath = "Configuration/GameConfig";
        
        // Animation Parameter Names
        public static class AnimationParams
        {
            public const string Idle = "Idle";
            public const string Dance = "Dance";
            public const string Wave = "Wave";
            public const string Wai = "Wai";
            public const string Curtsy = "Curtsy";
            public const string Bow = "Bow";
        }

        // UI Layer Names
        public static class UILayers
        {
            public const string LoginPanel = "LoginPanel";
            public const string RegisterPanel = "RegisterPanel";
            public const string GamePanel = "GamePanel";
            public const string CustomizationPanel = "CustomizationPanel";
        }

        // PlayerPrefs Keys
        public static class PlayerPrefsKeys
        {
            public const string LastLoggedInUser = "LastLoggedInUser";
            public const string MusicVolume = "MusicVolume";
            public const string SfxVolume = "SfxVolume";
            public const string DebugModeEnabled = "DebugModeEnabled";
        }
    }
}
