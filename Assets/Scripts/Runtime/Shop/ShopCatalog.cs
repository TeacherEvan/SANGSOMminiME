using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SangsomMiniMe.Shop
{
    /// <summary>
    /// ScriptableObject containing the complete shop catalog.
    /// Acts as a database of all available shop items.
    /// Create via Assets > Create > Sangsom Mini-Me > Shop Catalog.
    /// </summary>
    [CreateAssetMenu(fileName = "ShopCatalog", menuName = "Sangsom Mini-Me/Shop Catalog", order = 2)]
    public class ShopCatalog : ScriptableObject
    {
        [Header("Catalog Items")]
        [Tooltip("All items available in the shop")]
        [SerializeField] private List<ShopItem> items = new List<ShopItem>();

        [Header("Default Items")]
        [Tooltip("Items that are unlocked by default for new users")]
        [SerializeField] private List<ShopItem> defaultUnlockedItems = new List<ShopItem>();

        // Cached lookup dictionaries for O(1) access
        private Dictionary<string, ShopItem> itemLookup;
        private Dictionary<ShopCategory, List<ShopItem>> categoryLookup;
        private bool isCacheValid = false;

        /// <summary>
        /// Gets all items in the catalog.
        /// </summary>
        public IReadOnlyList<ShopItem> AllItems => items;

        /// <summary>
        /// Gets items that are unlocked by default.
        /// </summary>
        public IReadOnlyList<ShopItem> DefaultItems => defaultUnlockedItems;

        /// <summary>
        /// Gets the total count of items in catalog.
        /// </summary>
        public int ItemCount => items.Count;

        /// <summary>
        /// Ensures lookup caches are built.
        /// </summary>
        private void EnsureCacheValid()
        {
            if (isCacheValid && itemLookup != null && categoryLookup != null)
                return;

            BuildCache();
        }

        /// <summary>
        /// Builds the lookup caches for fast item retrieval.
        /// </summary>
        private void BuildCache()
        {
            itemLookup = new Dictionary<string, ShopItem>();
            categoryLookup = new Dictionary<ShopCategory, List<ShopItem>>();

            // Initialize all category lists
            foreach (ShopCategory category in System.Enum.GetValues(typeof(ShopCategory)))
            {
                categoryLookup[category] = new List<ShopItem>();
            }

            foreach (var item in items)
            {
                if (item == null) continue;

                // Add to ID lookup
                if (!itemLookup.ContainsKey(item.ItemId))
                {
                    itemLookup[item.ItemId] = item;
                }
                else
                {
                    Debug.LogWarning($"[ShopCatalog] Duplicate item ID: {item.ItemId}");
                }

                // Add to category lookup
                categoryLookup[item.Category].Add(item);

                // Also add to "All" category
                if (item.Category != ShopCategory.All)
                {
                    categoryLookup[ShopCategory.All].Add(item);
                }
            }

            // Sort each category by sort order, then by name
            foreach (var category in categoryLookup.Keys.ToList())
            {
                categoryLookup[category] = categoryLookup[category]
                    .OrderBy(i => i.SortOrder)
                    .ThenBy(i => i.DisplayName)
                    .ToList();
            }

            isCacheValid = true;
        }

        /// <summary>
        /// Gets an item by its unique ID.
        /// </summary>
        public ShopItem GetItem(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;

            EnsureCacheValid();
            return itemLookup.TryGetValue(itemId, out var item) ? item : null;
        }

        /// <summary>
        /// Gets all items in a specific category.
        /// </summary>
        public IReadOnlyList<ShopItem> GetItemsByCategory(ShopCategory category)
        {
            EnsureCacheValid();
            return categoryLookup.TryGetValue(category, out var items) ? items : new List<ShopItem>();
        }

        /// <summary>
        /// Gets all items of a specific rarity.
        /// </summary>
        public IReadOnlyList<ShopItem> GetItemsByRarity(ItemRarity rarity)
        {
            EnsureCacheValid();
            return items.Where(i => i != null && i.Rarity == rarity).ToList();
        }

        /// <summary>
        /// Gets all available items (not disabled).
        /// </summary>
        public IReadOnlyList<ShopItem> GetAvailableItems()
        {
            EnsureCacheValid();
            return items.Where(i => i != null && i.IsAvailable).ToList();
        }

        /// <summary>
        /// Gets items that can be purchased (available and purchasable).
        /// </summary>
        public IReadOnlyList<ShopItem> GetPurchasableItems()
        {
            EnsureCacheValid();
            return items.Where(i => i != null && i.IsAvailable && i.UnlockMethod == UnlockMethod.Purchase).ToList();
        }

        /// <summary>
        /// Gets items affordable by a user with specified coins.
        /// </summary>
        public IReadOnlyList<ShopItem> GetAffordableItems(int userCoins)
        {
            return GetPurchasableItems().Where(i => i.Price <= userCoins).ToList();
        }

        /// <summary>
        /// Checks if an item exists in the catalog.
        /// </summary>
        public bool HasItem(string itemId)
        {
            EnsureCacheValid();
            return itemLookup.ContainsKey(itemId);
        }

        /// <summary>
        /// Gets category item counts for UI display.
        /// </summary>
        public Dictionary<ShopCategory, int> GetCategoryCounts()
        {
            EnsureCacheValid();
            return categoryLookup.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count);
        }

        /// <summary>
        /// Invalidates cache (call after catalog changes in editor).
        /// </summary>
        public void InvalidateCache()
        {
            isCacheValid = false;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            InvalidateCache();
        }
#endif
    }
}
