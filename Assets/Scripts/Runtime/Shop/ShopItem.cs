using UnityEngine;

namespace SangsomMiniMe.Shop
{
    /// <summary>
    /// ScriptableObject defining a purchasable/unlockable shop item.
    /// Create instances via Assets > Create > Sangsom Mini-Me > Shop Item.
    /// 
    /// Best practices implemented:
    /// - Data-driven design (no hardcoded values)
    /// - Inspector-friendly with headers and tooltips
    /// - Rarity-based pricing suggestions
    /// - Preview support for UI display
    /// </summary>
    [CreateAssetMenu(fileName = "NewShopItem", menuName = "Sangsom Mini-Me/Shop Item", order = 1)]
    public class ShopItem : ScriptableObject
    {
        [Header("Basic Info")]
        [Tooltip("Unique identifier for this item (used for saving/loading)")]
        [SerializeField] private string itemId;

        [Tooltip("Display name shown in shop UI")]
        [SerializeField] private string displayName;

        [Tooltip("Description shown when item is selected")]
        [TextArea(2, 4)]
        [SerializeField] private string description;

        [Header("Categorization")]
        [Tooltip("Shop category for filtering")]
        [SerializeField] private ShopCategory category = ShopCategory.Outfits;

        [Tooltip("Rarity tier affecting visual styling")]
        [SerializeField] private ItemRarity rarity = ItemRarity.Common;

        [Header("Pricing & Unlock")]
        [Tooltip("Cost in coins (0 = free or unlock-only)")]
        [SerializeField] private int price = 50;

        [Tooltip("How this item can be obtained")]
        [SerializeField] private UnlockMethod unlockMethod = UnlockMethod.Purchase;

        [Tooltip("Required level to purchase (0 = no requirement)")]
        [SerializeField] private int requiredLevel = 0;

        [Tooltip("Required homework completions for homework rewards")]
        [SerializeField] private int requiredHomework = 0;

        [Tooltip("Required streak days for streak rewards")]
        [SerializeField] private int requiredStreak = 0;

        [Header("Visuals")]
        [Tooltip("Icon displayed in shop grid")]
        [SerializeField] private Sprite icon;

        [Tooltip("Preview image for detail view")]
        [SerializeField] private Sprite previewImage;

        [Tooltip("Prefab to instantiate when equipped (optional)")]
        [SerializeField] private GameObject itemPrefab;

        [Tooltip("Material to apply when this outfit is equipped")]
        [SerializeField] private Material outfitMaterial;

        [Header("Availability")]
        [Tooltip("Is this item currently available for purchase?")]
        [SerializeField] private bool isAvailable = true;

        [Tooltip("Is this a limited-time item?")]
        [SerializeField] private bool isLimitedTime = false;

        [Tooltip("Sort order within category (lower = first)")]
        [SerializeField] private int sortOrder = 0;

        // ===== PUBLIC ACCESSORS =====

        public string ItemId => string.IsNullOrEmpty(itemId) ? name : itemId;
        public string DisplayName => string.IsNullOrEmpty(displayName) ? name : displayName;
        public string Description => description;
        public ShopCategory Category => category;
        public ItemRarity Rarity => rarity;
        public int Price => price;
        public UnlockMethod UnlockMethod => unlockMethod;
        public int RequiredLevel => requiredLevel;
        public int RequiredHomework => requiredHomework;
        public int RequiredStreak => requiredStreak;
        public Sprite Icon => icon;
        public Sprite PreviewImage => previewImage;
        public GameObject ItemPrefab => itemPrefab;
        public Material OutfitMaterial => outfitMaterial;
        public bool IsAvailable => isAvailable;
        public bool IsLimitedTime => isLimitedTime;
        public int SortOrder => sortOrder;

        /// <summary>
        /// Gets the suggested price based on rarity tier.
        /// Used for editor validation and auto-pricing.
        /// </summary>
        public int SuggestedPrice => rarity switch
        {
            ItemRarity.Common => 25,
            ItemRarity.Uncommon => 75,
            ItemRarity.Rare => 150,
            ItemRarity.Epic => 350,
            ItemRarity.Legendary => 750,
            _ => 50
        };

        /// <summary>
        /// Gets the rarity color for UI display.
        /// </summary>
        public Color RarityColor => rarity switch
        {
            ItemRarity.Common => new Color(0.8f, 0.8f, 0.8f),      // Light gray
            ItemRarity.Uncommon => new Color(0.2f, 0.8f, 0.2f),    // Green
            ItemRarity.Rare => new Color(0.2f, 0.4f, 1f),          // Blue
            ItemRarity.Epic => new Color(0.6f, 0.2f, 0.8f),        // Purple
            ItemRarity.Legendary => new Color(1f, 0.8f, 0.2f),     // Gold
            _ => Color.white
        };

        /// <summary>
        /// Gets the rarity display name with emoji.
        /// </summary>
        public string RarityDisplayName => rarity switch
        {
            ItemRarity.Common => "⚪ Common",
            ItemRarity.Uncommon => "🟢 Uncommon",
            ItemRarity.Rare => "🔵 Rare",
            ItemRarity.Epic => "🟣 Epic",
            ItemRarity.Legendary => "🌟 Legendary",
            _ => "Unknown"
        };

        /// <summary>
        /// Gets the category emoji for display.
        /// </summary>
        public string CategoryEmoji => category switch
        {
            ShopCategory.Outfits => "👗",
            ShopCategory.Accessories => "💍",
            ShopCategory.Hats => "🎩",
            ShopCategory.Jewelry => "💎",
            ShopCategory.Eyes => "👁️",
            ShopCategory.Food => "🍎",
            ShopCategory.Special => "⭐",
            _ => "🛒"
        };

#if UNITY_EDITOR
        /// <summary>
        /// Editor validation to ensure item data is complete.
        /// </summary>
        private void OnValidate()
        {
            // Auto-generate ID if empty
            if (string.IsNullOrEmpty(itemId))
            {
                itemId = name.ToLowerInvariant().Replace(" ", "_");
            }

            // Warn if price doesn't match rarity suggestion
            if (unlockMethod == UnlockMethod.Purchase && price > 0)
            {
                int suggested = SuggestedPrice;
                if (price < suggested * 0.5f || price > suggested * 2f)
                {
                    Debug.LogWarning($"[ShopItem] {displayName}: Price {price} differs significantly from suggested {suggested} for {rarity} rarity.");
                }
            }
        }
#endif
    }
}
