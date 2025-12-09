namespace SangsomMiniMe.Shop
{
    /// <summary>
    /// Categories of items available in the shop.
    /// Used for filtering and organizing shop display.
    /// </summary>
    public enum ShopCategory
    {
        All = 0,
        Outfits = 1,
        Accessories = 2,
        Hats = 3,
        Jewelry = 4,
        Eyes = 5,
        Food = 6,
        Special = 7
    }

    /// <summary>
    /// Rarity tiers for shop items.
    /// Affects visual styling and perceived value.
    /// Based on tamagotchi-style cozy game design.
    /// </summary>
    public enum ItemRarity
    {
        /// <summary>60% drop rate - everyday items</summary>
        Common = 0,
        /// <summary>25% drop rate - nice upgrades</summary>
        Uncommon = 1,
        /// <summary>10% drop rate - special items</summary>
        Rare = 2,
        /// <summary>4% drop rate - premium items</summary>
        Epic = 3,
        /// <summary>1% drop rate - ultimate treasures</summary>
        Legendary = 4
    }

    /// <summary>
    /// Purchase result status codes.
    /// </summary>
    public enum PurchaseResult
    {
        Success,
        InsufficientFunds,
        AlreadyOwned,
        ItemNotFound,
        LevelRequirementNotMet,
        ItemNotAvailable
    }

    /// <summary>
    /// How an item can be obtained.
    /// </summary>
    public enum UnlockMethod
    {
        /// <summary>Can be purchased with coins</summary>
        Purchase,
        /// <summary>Unlocked by reaching a level</summary>
        LevelUnlock,
        /// <summary>Unlocked by completing homework</summary>
        HomeworkReward,
        /// <summary>Unlocked by login streaks</summary>
        StreakReward,
        /// <summary>Special event or milestone reward</summary>
        Achievement,
        /// <summary>Starts unlocked for everyone</summary>
        Default
    }
}
