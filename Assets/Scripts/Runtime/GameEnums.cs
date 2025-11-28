namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Character animation types supported by the Mini-Me system
    /// </summary>
    public enum CharacterAnimation
    {
        Idle = 0,
        Dance = 1,
        Wave = 2,
        Wai = 3,      // Thai traditional greeting gesture
        Curtsy = 4,
        Bow = 5
    }
    
    /// <summary>
    /// Mood states for character happiness display
    /// </summary>
    public enum MoodState
    {
        VeryHappy,    // 80-100 happiness
        Happy,        // 60-80 happiness
        Neutral,      // 40-60 happiness
        Sad,          // 20-40 happiness
        VerySad       // 0-20 happiness
    }
    
    /// <summary>
    /// Types of rewards that can be earned
    /// </summary>
    public enum RewardType
    {
        Coins,
        Experience,
        Happiness,
        Outfit,
        Accessory
    }
    
    /// <summary>
    /// Categories of customization items
    /// </summary>
    public enum CustomizationCategory
    {
        Outfit,
        Accessory,
        EyeScale,
        RoomTheme
    }
    
    /// <summary>
    /// Types of educational activities
    /// </summary>
    public enum ActivityType
    {
        HomeworkCompletion,
        MiniGameVictory,
        DailyLogin,
        CharacterInteraction,
        CustomizationPurchase
    }
    
    /// <summary>
    /// User account status
    /// </summary>
    public enum AccountStatus
    {
        Active,
        Inactive,
        Suspended,
        Pending
    }
}
