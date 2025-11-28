namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Game-wide constants for consistent values across the application
    /// </summary>
    public static class GameConstants
    {
        // User Profile Defaults
        public const int DefaultStartingCoins = 100;
        public const float DefaultStartingHappiness = 75f;
        public const float DefaultEyeScale = 1.0f;
        public const string DefaultOutfit = "default";
        public const string DefaultAccessory = "none";
        
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
        public const int HomeworkExpReward = 10;
        public const int HomeworkCoinReward = 5;
        public const float HomeworkHappinessReward = 5f;
        public const float DanceHappinessBonus = 2f;
        
        // Auto-Save Settings
        public const float DefaultAutoSaveInterval = 30f;
        
        // Animation Settings
        public const float DefaultAnimationDuration = 2f;
        
        // Level System
        public const int ExperiencePerLevel = 100;
        
        // File Paths
        public const string UserProfilesSaveFileName = "userProfiles.json";
        
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
