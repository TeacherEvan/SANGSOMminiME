using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SangsomMiniMe.Shop
{
    /// <summary>
    /// Result of a purchase attempt with detailed information.
    /// </summary>
    public struct PurchaseResultInfo
    {
        public PurchaseResult Result;
        public ShopItem Item;
        public int CoinsSpent;
        public int CoinsRemaining;
        public string Message;

        public bool IsSuccess => Result == PurchaseResult.Success;
    }

    /// <summary>
    /// Central manager for shop operations: purchasing, inventory, and unlock tracking.
    /// Integrates with UserProfile for coin management and persistence.
    /// 
    /// Best practices:
    /// - Event-driven notifications for UI updates
    /// - Separation of data (ShopItem) from logic (ShopManager)
    /// - Defensive validation at purchase time
    /// - User inventory persisted through UserProfile
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [Header("Shop Configuration")]
        [SerializeField] private ShopCatalog catalog;

        [Header("Scene References")]
        [SerializeField] private Character.CharacterController characterController;

        [Header("Audio")]
        [SerializeField] private AudioClip purchaseSound;
        [SerializeField] private AudioClip errorSound;

        // Singleton instance
        private static ShopManager instance;
        public static ShopManager Instance => instance;

        // Events for UI updates
        public static event Action<ShopItem> OnItemPurchased;
        public static event Action<ShopItem> OnItemUnlocked;
        public static event Action<ShopItem> OnItemEquipped;
        public static event Action<PurchaseResultInfo> OnPurchaseAttempted;

        // Currently selected/equipped items per category
        private Dictionary<ShopCategory, string> equippedItems = new Dictionary<ShopCategory, string>();

        /// <summary>
        /// Gets the shop catalog.
        /// </summary>
        public ShopCatalog Catalog => catalog;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeShop();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeShop()
        {
            if (catalog == null)
            {
                Debug.LogWarning("[ShopManager] No catalog assigned. Shop functionality will be limited.");
                return;
            }

            if (characterController == null)
            {
                characterController = FindFirstObjectByType<Character.CharacterController>();
            }

            // Initialize equipped items dictionary
            foreach (ShopCategory category in Enum.GetValues(typeof(ShopCategory)))
            {
                equippedItems[category] = string.Empty;
            }

            Debug.Log($"[ShopManager] Initialized with {catalog.ItemCount} items in catalog.");
        }

        /// <summary>
        /// Attempts to purchase an item for the current user.
        /// </summary>
        public PurchaseResultInfo TryPurchase(string itemId)
        {
            var result = new PurchaseResultInfo { Result = PurchaseResult.ItemNotFound };

            // Validate item exists
            var item = catalog?.GetItem(itemId);
            if (item == null)
            {
                result.Message = "Item not found in catalog.";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            result.Item = item;
            return TryPurchase(item);
        }

        /// <summary>
        /// Attempts to purchase a specific item.
        /// </summary>
        public PurchaseResultInfo TryPurchase(ShopItem item)
        {
            var result = new PurchaseResultInfo
            {
                Item = item,
                Result = PurchaseResult.ItemNotFound
            };

            if (item == null)
            {
                result.Message = "Invalid item.";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // Get current user
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user == null)
            {
                result.Result = PurchaseResult.ItemNotAvailable;
                result.Message = "No user logged in.";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // Check if already owned
            if (IsItemOwned(item.ItemId))
            {
                result.Result = PurchaseResult.AlreadyOwned;
                result.Message = $"You already own {item.DisplayName}!";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // Check availability
            if (!item.IsAvailable)
            {
                result.Result = PurchaseResult.ItemNotAvailable;
                result.Message = $"{item.DisplayName} is not currently available.";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // Check level requirement
            int userLevel = Core.GameUtilities.CalculateLevel(user.ExperiencePoints);
            if (item.RequiredLevel > 0 && userLevel < item.RequiredLevel)
            {
                result.Result = PurchaseResult.LevelRequirementNotMet;
                result.Message = $"Requires Level {item.RequiredLevel}. You are Level {userLevel}.";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // Check if purchasable (not unlock-only)
            if (item.UnlockMethod != UnlockMethod.Purchase && item.UnlockMethod != UnlockMethod.Default)
            {
                result.Result = PurchaseResult.ItemNotAvailable;
                result.Message = $"{item.DisplayName} cannot be purchased. It must be unlocked.";
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // Check funds
            if (user.Coins < item.Price)
            {
                result.Result = PurchaseResult.InsufficientFunds;
                result.CoinsRemaining = user.Coins;
                result.Message = $"Not enough coins! Need {item.Price}, have {user.Coins}.";
                Core.AudioManager.Instance?.PlayButtonClick(); // Soft error sound
                OnPurchaseAttempted?.Invoke(result);
                return result;
            }

            // ===== PURCHASE SUCCESS =====

            // Deduct coins
            user.SpendCoins(item.Price);

            // Add to inventory
            AddItemToInventory(item.ItemId);

            // Mark profile dirty for save
            Core.UserManager.Instance?.MarkDirty();

            // Play purchase sound
            Core.AudioManager.Instance?.PlayCoin();

            // Build success result
            result.Result = PurchaseResult.Success;
            result.CoinsSpent = item.Price;
            result.CoinsRemaining = user.Coins;
            result.Message = $"🎉 Purchased {item.DisplayName}!";

            Debug.Log($"[ShopManager] Purchase successful: {item.DisplayName} for {item.Price} coins.");

            // Fire events
            OnItemPurchased?.Invoke(item);
            OnPurchaseAttempted?.Invoke(result);

            return result;
        }

        /// <summary>
        /// Unlocks an item without payment (for rewards, achievements, etc.).
        /// </summary>
        public bool UnlockItem(string itemId, string reason = "")
        {
            var item = catalog?.GetItem(itemId);
            if (item == null)
            {
                Debug.LogWarning($"[ShopManager] Cannot unlock item: {itemId} not found.");
                return false;
            }

            return UnlockItem(item, reason);
        }

        /// <summary>
        /// Unlocks a specific item without payment.
        /// </summary>
        public bool UnlockItem(ShopItem item, string reason = "")
        {
            if (item == null) return false;

            if (IsItemOwned(item.ItemId))
            {
                Debug.Log($"[ShopManager] Item already owned: {item.DisplayName}");
                return false;
            }

            AddItemToInventory(item.ItemId);
            Core.UserManager.Instance?.MarkDirty();

            Debug.Log($"[ShopManager] Item unlocked: {item.DisplayName} ({reason})");

            OnItemUnlocked?.Invoke(item);
            return true;
        }

        /// <summary>
        /// Checks if the current user owns an item.
        /// </summary>
        public bool IsItemOwned(string itemId)
        {
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user == null) return false;

            return user.OwnedItems.Contains(itemId);
        }

        /// <summary>
        /// Gets all items owned by the current user.
        /// </summary>
        public List<ShopItem> GetOwnedItems()
        {
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user == null || catalog == null) return new List<ShopItem>();

            return user.OwnedItems
                .Select(id => catalog.GetItem(id))
                .Where(item => item != null)
                .ToList();
        }

        /// <summary>
        /// Gets all items the user doesn't own yet.
        /// </summary>
        public List<ShopItem> GetUnownedItems()
        {
            if (catalog == null) return new List<ShopItem>();

            return catalog.GetAvailableItems()
                .Where(item => !IsItemOwned(item.ItemId))
                .ToList();
        }

        /// <summary>
        /// Gets items the user can afford and doesn't own.
        /// </summary>
        public List<ShopItem> GetAffordableUnownedItems()
        {
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user == null) return new List<ShopItem>();

            return GetUnownedItems()
                .Where(item => item.UnlockMethod == UnlockMethod.Purchase && item.Price <= user.Coins)
                .ToList();
        }

        /// <summary>
        /// Equips an owned item.
        /// </summary>
        public bool EquipItem(string itemId)
        {
            var item = catalog?.GetItem(itemId);
            if (item == null)
            {
                Debug.LogWarning($"[ShopManager] Cannot equip: {itemId} not found.");
                return false;
            }

            return EquipItem(item);
        }

        /// <summary>
        /// Equips a specific item.
        /// </summary>
        public bool EquipItem(ShopItem item)
        {
            if (item == null) return false;

            if (!IsItemOwned(item.ItemId))
            {
                Debug.LogWarning($"[ShopManager] Cannot equip {item.DisplayName}: not owned.");
                return false;
            }

            // Store equipped item
            equippedItems[item.Category] = item.ItemId;

            // Apply to character based on category
            ApplyItemToCharacter(item);

            Debug.Log($"[ShopManager] Equipped: {item.DisplayName}");

            OnItemEquipped?.Invoke(item);
            return true;
        }

        /// <summary>
        /// Gets the currently equipped item for a category.
        /// </summary>
        public ShopItem GetEquippedItem(ShopCategory category)
        {
            if (!equippedItems.TryGetValue(category, out string itemId) || string.IsNullOrEmpty(itemId))
                return null;

            return catalog?.GetItem(itemId);
        }

        /// <summary>
        /// Applies item effects to the character.
        /// </summary>
        private void ApplyItemToCharacter(ShopItem item)
        {
            if (characterController == null) return;

            switch (item.Category)
            {
                case ShopCategory.Outfits:
                    if (item.OutfitMaterial != null)
                    {
                        characterController.SetOutfitByName(item.ItemId);
                    }
                    break;

                case ShopCategory.Accessories:
                case ShopCategory.Hats:
                case ShopCategory.Jewelry:
                    characterController.SetAccessoryByName(item.ItemId);
                    break;

                case ShopCategory.Eyes:
                    // Eye items might have special properties
                    // characterController.SetEyeStyle(item.ItemId);
                    break;
            }
        }

        /// <summary>
        /// Adds an item to the user's inventory.
        /// </summary>
        private void AddItemToInventory(string itemId)
        {
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user == null) return;

            if (!user.OwnedItems.Contains(itemId))
            {
                user.OwnedItems.Add(itemId);
            }
        }

        /// <summary>
        /// Grants default items to a new user.
        /// </summary>
        public void GrantDefaultItems()
        {
            if (catalog == null) return;

            foreach (var item in catalog.DefaultItems)
            {
                if (item != null && !IsItemOwned(item.ItemId))
                {
                    AddItemToInventory(item.ItemId);
                }
            }

            Core.UserManager.Instance?.MarkDirty();
            Debug.Log($"[ShopManager] Granted {catalog.DefaultItems.Count} default items.");
        }

        /// <summary>
        /// Checks and unlocks items based on user achievements.
        /// Call this when user stats change (homework, streaks, levels).
        /// </summary>
        public void CheckAchievementUnlocks()
        {
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user == null || catalog == null) return;

            int userLevel = Core.GameUtilities.CalculateLevel(user.ExperiencePoints);

            foreach (var item in catalog.AllItems)
            {
                if (item == null || IsItemOwned(item.ItemId)) continue;

                bool shouldUnlock = false;
                string reason = "";

                switch (item.UnlockMethod)
                {
                    case UnlockMethod.LevelUnlock:
                        if (item.RequiredLevel > 0 && userLevel >= item.RequiredLevel)
                        {
                            shouldUnlock = true;
                            reason = $"Reached Level {item.RequiredLevel}";
                        }
                        break;

                    case UnlockMethod.HomeworkReward:
                        if (item.RequiredHomework > 0 && user.HomeworkCompleted >= item.RequiredHomework)
                        {
                            shouldUnlock = true;
                            reason = $"Completed {item.RequiredHomework} homework assignments";
                        }
                        break;

                    case UnlockMethod.StreakReward:
                        if (item.RequiredStreak > 0 && user.LongestStreak >= item.RequiredStreak)
                        {
                            shouldUnlock = true;
                            reason = $"Achieved {item.RequiredStreak} day streak";
                        }
                        break;
                }

                if (shouldUnlock)
                {
                    UnlockItem(item, reason);
                }
            }
        }
    }
}
