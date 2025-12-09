using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace SangsomMiniMe.Shop
{
    /// <summary>
    /// UI controller for the shop interface.
    /// Handles item display, category filtering, purchasing, and visual feedback.
    /// 
    /// Best practices:
    /// - Pooled item slots for performance
    /// - Event-driven updates from ShopManager
    /// - Category tabs with active state
    /// - Rarity color coding
    /// - Responsive to coin changes
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        [Header("Panel References")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Category Tabs")]
        [SerializeField] private Transform categoryTabContainer;
        [SerializeField] private Button[] categoryButtons;
        [SerializeField] private TextMeshProUGUI[] categoryLabels;

        [Header("Item Grid")]
        [SerializeField] private Transform itemGridContainer;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private ScrollRect itemScrollRect;

        [Header("Item Detail Panel")]
        [SerializeField] private GameObject detailPanel;
        [SerializeField] private Image detailIcon;
        [SerializeField] private TextMeshProUGUI detailName;
        [SerializeField] private TextMeshProUGUI detailDescription;
        [SerializeField] private TextMeshProUGUI detailRarity;
        [SerializeField] private TextMeshProUGUI detailPrice;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private TextMeshProUGUI purchaseButtonText;
        [SerializeField] private Button equipButton;
        [SerializeField] private TextMeshProUGUI equipButtonText;

        [Header("Header")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private Button closeButton;

        [Header("Feedback")]
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private float feedbackDuration = 3f;

        [Header("Configuration")]
        [SerializeField] private int itemPoolSize = 30;
        [SerializeField] private Color selectedTabColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color normalTabColor = new Color(0.7f, 0.7f, 0.7f);

        // State
        private ShopCategory currentCategory = ShopCategory.All;
        private ShopItem selectedItem;
        private List<ShopItemSlot> itemSlotPool = new List<ShopItemSlot>();
        private Coroutine feedbackCoroutine;

        private void Awake()
        {
            InitializeUI();
            CreateItemSlotPool();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
            RefreshDisplay();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void InitializeUI()
        {
            // Setup close button
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            // Setup purchase button
            if (purchaseButton != null)
            {
                purchaseButton.onClick.AddListener(HandlePurchaseClicked);
            }

            // Setup equip button
            if (equipButton != null)
            {
                equipButton.onClick.AddListener(HandleEquipClicked);
            }

            // Setup category buttons
            SetupCategoryButtons();

            // Hide detail panel initially
            if (detailPanel != null)
            {
                detailPanel.SetActive(false);
            }
        }

        private void SetupCategoryButtons()
        {
            var categories = new ShopCategory[]
            {
                ShopCategory.All,
                ShopCategory.Outfits,
                ShopCategory.Accessories,
                ShopCategory.Hats,
                ShopCategory.Jewelry,
                ShopCategory.Special
            };

            for (int i = 0; i < categoryButtons.Length && i < categories.Length; i++)
            {
                int index = i; // Capture for closure
                var category = categories[i];

                if (categoryButtons[i] != null)
                {
                    categoryButtons[i].onClick.AddListener(() => SelectCategory(category));
                }

                if (categoryLabels != null && i < categoryLabels.Length && categoryLabels[i] != null)
                {
                    categoryLabels[i].text = GetCategoryDisplayName(category);
                }
            }
        }

        private void CreateItemSlotPool()
        {
            if (itemSlotPrefab == null || itemGridContainer == null)
            {
                Debug.LogWarning("[ShopUI] Item slot prefab or grid container not assigned.");
                return;
            }

            for (int i = 0; i < itemPoolSize; i++)
            {
                var slotObj = Instantiate(itemSlotPrefab, itemGridContainer);
                var slot = slotObj.GetComponent<ShopItemSlot>();

                if (slot == null)
                {
                    slot = slotObj.AddComponent<ShopItemSlot>();
                }

                slot.OnSlotClicked += HandleItemSlotClicked;
                slot.gameObject.SetActive(false);
                itemSlotPool.Add(slot);
            }

            Debug.Log($"[ShopUI] Created {itemPoolSize} item slot pool.");
        }

        private void SubscribeToEvents()
        {
            ShopManager.OnItemPurchased += HandleItemPurchased;
            ShopManager.OnItemUnlocked += HandleItemUnlocked;
            ShopManager.OnPurchaseAttempted += HandlePurchaseAttempted;

            // Subscribe to coin changes
            var user = Core.UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                user.OnCoinsChanged += UpdateCoinsDisplay;
            }
        }

        private void UnsubscribeFromEvents()
        {
            ShopManager.OnItemPurchased -= HandleItemPurchased;
            ShopManager.OnItemUnlocked -= HandleItemUnlocked;
            ShopManager.OnPurchaseAttempted -= HandlePurchaseAttempted;

            var user = Core.UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                user.OnCoinsChanged -= UpdateCoinsDisplay;
            }
        }

        /// <summary>
        /// Shows the shop panel.
        /// </summary>
        public void Show()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
            }

            gameObject.SetActive(true);
            RefreshDisplay();

            // Play open sound
            Core.AudioManager.Instance?.PlayButtonClick();
        }

        /// <summary>
        /// Hides the shop panel.
        /// </summary>
        public void Hide()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }

            selectedItem = null;

            // Play close sound
            Core.AudioManager.Instance?.PlayButtonClick();
        }

        /// <summary>
        /// Selects a category and updates display.
        /// </summary>
        public void SelectCategory(ShopCategory category)
        {
            currentCategory = category;
            UpdateCategoryTabs();
            RefreshItemGrid();
            ClearSelection();

            Core.AudioManager.Instance?.PlayButtonClick();
        }

        /// <summary>
        /// Refreshes all shop displays.
        /// </summary>
        public void RefreshDisplay()
        {
            UpdateCoinsDisplay(Core.UserManager.Instance?.CurrentUser?.Coins ?? 0);
            UpdateCategoryTabs();
            RefreshItemGrid();
            UpdateDetailPanel();
        }

        private void UpdateCoinsDisplay(int coins)
        {
            if (coinsText != null)
            {
                coinsText.text = $"💰 {coins:N0}";
            }
        }

        private void UpdateCategoryTabs()
        {
            var categories = new ShopCategory[]
            {
                ShopCategory.All,
                ShopCategory.Outfits,
                ShopCategory.Accessories,
                ShopCategory.Hats,
                ShopCategory.Jewelry,
                ShopCategory.Special
            };

            for (int i = 0; i < categoryButtons.Length && i < categories.Length; i++)
            {
                bool isSelected = categories[i] == currentCategory;

                if (categoryButtons[i] != null)
                {
                    var colors = categoryButtons[i].colors;
                    colors.normalColor = isSelected ? selectedTabColor : normalTabColor;
                    categoryButtons[i].colors = colors;
                }

                if (categoryLabels != null && i < categoryLabels.Length && categoryLabels[i] != null)
                {
                    categoryLabels[i].fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
                }
            }
        }

        private void RefreshItemGrid()
        {
            // Hide all slots first
            foreach (var slot in itemSlotPool)
            {
                slot.gameObject.SetActive(false);
            }

            var catalog = ShopManager.Instance?.Catalog;
            if (catalog == null)
            {
                Debug.LogWarning("[ShopUI] No catalog available.");
                return;
            }

            var items = catalog.GetItemsByCategory(currentCategory);
            int slotIndex = 0;

            foreach (var item in items)
            {
                if (slotIndex >= itemSlotPool.Count) break;
                if (item == null || !item.IsAvailable) continue;

                var slot = itemSlotPool[slotIndex];
                bool isOwned = ShopManager.Instance.IsItemOwned(item.ItemId);

                slot.Setup(item, isOwned);
                slot.gameObject.SetActive(true);
                slotIndex++;
            }

            // Reset scroll position
            if (itemScrollRect != null)
            {
                itemScrollRect.verticalNormalizedPosition = 1f;
            }
        }

        private void HandleItemSlotClicked(ShopItem item)
        {
            SelectItem(item);
        }

        private void SelectItem(ShopItem item)
        {
            selectedItem = item;
            UpdateDetailPanel();

            Core.AudioManager.Instance?.PlayButtonClick();
        }

        private void ClearSelection()
        {
            selectedItem = null;
            UpdateDetailPanel();
        }

        private void UpdateDetailPanel()
        {
            if (detailPanel == null) return;

            if (selectedItem == null)
            {
                detailPanel.SetActive(false);
                return;
            }

            detailPanel.SetActive(true);

            // Update detail info
            if (detailIcon != null && selectedItem.Icon != null)
            {
                detailIcon.sprite = selectedItem.Icon;
            }

            if (detailName != null)
            {
                detailName.text = $"{selectedItem.CategoryEmoji} {selectedItem.DisplayName}";
            }

            if (detailDescription != null)
            {
                detailDescription.text = selectedItem.Description;
            }

            if (detailRarity != null)
            {
                detailRarity.text = selectedItem.RarityDisplayName;
                detailRarity.color = selectedItem.RarityColor;
            }

            if (detailPrice != null)
            {
                if (selectedItem.UnlockMethod == UnlockMethod.Purchase)
                {
                    detailPrice.text = $"💰 {selectedItem.Price}";
                }
                else
                {
                    detailPrice.text = GetUnlockMethodText(selectedItem);
                }
            }

            // Update buttons
            UpdateActionButtons();
        }

        private void UpdateActionButtons()
        {
            if (selectedItem == null) return;

            bool isOwned = ShopManager.Instance?.IsItemOwned(selectedItem.ItemId) ?? false;
            int userCoins = Core.UserManager.Instance?.CurrentUser?.Coins ?? 0;
            bool canAfford = userCoins >= selectedItem.Price;

            // Purchase button
            if (purchaseButton != null)
            {
                if (isOwned)
                {
                    purchaseButton.gameObject.SetActive(false);
                }
                else if (selectedItem.UnlockMethod == UnlockMethod.Purchase)
                {
                    purchaseButton.gameObject.SetActive(true);
                    purchaseButton.interactable = canAfford;

                    if (purchaseButtonText != null)
                    {
                        purchaseButtonText.text = canAfford ? $"Buy ({selectedItem.Price} 🪙)" : "Not Enough Coins";
                    }
                }
                else
                {
                    purchaseButton.gameObject.SetActive(false);
                }
            }

            // Equip button
            if (equipButton != null)
            {
                equipButton.gameObject.SetActive(isOwned);

                if (equipButtonText != null)
                {
                    var equippedItem = ShopManager.Instance?.GetEquippedItem(selectedItem.Category);
                    bool isEquipped = equippedItem?.ItemId == selectedItem.ItemId;
                    equipButtonText.text = isEquipped ? "✓ Equipped" : "Equip";
                    equipButton.interactable = !isEquipped;
                }
            }
        }

        private void HandlePurchaseClicked()
        {
            if (selectedItem == null) return;

            ShopManager.Instance?.TryPurchase(selectedItem);
        }

        private void HandleEquipClicked()
        {
            if (selectedItem == null) return;

            ShopManager.Instance?.EquipItem(selectedItem);
            UpdateActionButtons();

            ShowFeedback($"Equipped {selectedItem.DisplayName}! ✨", Color.cyan);
        }

        private void HandleItemPurchased(ShopItem item)
        {
            RefreshItemGrid();

            if (item == selectedItem)
            {
                UpdateDetailPanel();
            }
        }

        private void HandleItemUnlocked(ShopItem item)
        {
            RefreshItemGrid();
            ShowFeedback($"🎉 Unlocked: {item.DisplayName}!", item.RarityColor);
        }

        private void HandlePurchaseAttempted(PurchaseResultInfo result)
        {
            if (result.IsSuccess)
            {
                ShowFeedback(result.Message, Color.green);
                Core.AudioManager.Instance?.PlayCoin();
            }
            else
            {
                ShowFeedback(result.Message, Color.yellow);
            }
        }

        private void ShowFeedback(string message, Color color)
        {
            if (feedbackText == null) return;

            if (feedbackCoroutine != null)
            {
                StopCoroutine(feedbackCoroutine);
            }

            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.gameObject.SetActive(true);

            feedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay());
        }

        private System.Collections.IEnumerator HideFeedbackAfterDelay()
        {
            yield return new WaitForSeconds(feedbackDuration);

            if (feedbackText != null)
            {
                feedbackText.gameObject.SetActive(false);
            }
        }

        private string GetCategoryDisplayName(ShopCategory category)
        {
            return category switch
            {
                ShopCategory.All => "🛒 All",
                ShopCategory.Outfits => "👗 Outfits",
                ShopCategory.Accessories => "💍 Accessories",
                ShopCategory.Hats => "🎩 Hats",
                ShopCategory.Jewelry => "💎 Jewelry",
                ShopCategory.Eyes => "👁️ Eyes",
                ShopCategory.Food => "🍎 Food",
                ShopCategory.Special => "⭐ Special",
                _ => category.ToString()
            };
        }

        private string GetUnlockMethodText(ShopItem item)
        {
            return item.UnlockMethod switch
            {
                UnlockMethod.LevelUnlock => $"🎯 Reach Level {item.RequiredLevel}",
                UnlockMethod.HomeworkReward => $"📚 Complete {item.RequiredHomework} homework",
                UnlockMethod.StreakReward => $"🔥 {item.RequiredStreak} day streak",
                UnlockMethod.Achievement => "🏆 Achievement reward",
                UnlockMethod.Default => "✨ Free",
                _ => ""
            };
        }
    }
}
