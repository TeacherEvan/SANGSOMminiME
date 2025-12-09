using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace SangsomMiniMe.Shop
{
    /// <summary>
    /// Individual item slot in the shop grid.
    /// Displays item icon, price, rarity indicator, and owned status.
    /// </summary>
    public class ShopItemSlot : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image rarityBorder;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private GameObject ownedIndicator;
        [SerializeField] private GameObject lockedIndicator;
        [SerializeField] private Button slotButton;

        [Header("Visual Settings")]
        [SerializeField] private Color ownedBackgroundColor = new Color(0.3f, 0.8f, 0.3f, 0.3f);
        [SerializeField] private Color normalBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private Color lockedBackgroundColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);

        // Events
        public event Action<ShopItem> OnSlotClicked;

        // State
        private ShopItem currentItem;
        private bool isOwned;

        private void Awake()
        {
            if (slotButton == null)
            {
                slotButton = GetComponent<Button>();
            }

            if (slotButton != null)
            {
                slotButton.onClick.AddListener(HandleClick);
            }
        }

        /// <summary>
        /// Sets up the slot to display a shop item.
        /// </summary>
        public void Setup(ShopItem item, bool owned)
        {
            currentItem = item;
            isOwned = owned;

            if (item == null)
            {
                gameObject.SetActive(false);
                return;
            }

            // Set icon
            if (iconImage != null)
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = item.Icon != null;

                // Gray out if locked (not purchasable and not owned)
                bool isLocked = !owned && item.UnlockMethod != UnlockMethod.Purchase;
                iconImage.color = isLocked ? new Color(0.5f, 0.5f, 0.5f, 0.7f) : Color.white;
            }

            // Set name
            if (nameText != null)
            {
                nameText.text = item.DisplayName;
            }

            // Set price
            if (priceText != null)
            {
                if (owned)
                {
                    priceText.text = "✓ Owned";
                    priceText.color = Color.green;
                }
                else if (item.UnlockMethod == UnlockMethod.Purchase)
                {
                    priceText.text = $"💰 {item.Price}";

                    // Color based on affordability
                    int userCoins = Core.UserManager.Instance?.CurrentUser?.Coins ?? 0;
                    priceText.color = userCoins >= item.Price ? Color.white : Color.red;
                }
                else
                {
                    priceText.text = GetUnlockHint(item);
                    priceText.color = Color.yellow;
                }
            }

            // Set rarity border
            if (rarityBorder != null)
            {
                rarityBorder.color = item.RarityColor;
            }

            // Set background
            if (backgroundImage != null)
            {
                if (owned)
                {
                    backgroundImage.color = ownedBackgroundColor;
                }
                else if (item.UnlockMethod != UnlockMethod.Purchase)
                {
                    backgroundImage.color = lockedBackgroundColor;
                }
                else
                {
                    backgroundImage.color = normalBackgroundColor;
                }
            }

            // Set indicators
            if (ownedIndicator != null)
            {
                ownedIndicator.SetActive(owned);
            }

            if (lockedIndicator != null)
            {
                bool isLocked = !owned && item.UnlockMethod != UnlockMethod.Purchase && item.UnlockMethod != UnlockMethod.Default;
                lockedIndicator.SetActive(isLocked);
            }

            gameObject.SetActive(true);
        }

        private void HandleClick()
        {
            if (currentItem != null)
            {
                OnSlotClicked?.Invoke(currentItem);
            }
        }

        private string GetUnlockHint(ShopItem item)
        {
            return item.UnlockMethod switch
            {
                UnlockMethod.LevelUnlock => $"🎯 Lv{item.RequiredLevel}",
                UnlockMethod.HomeworkReward => $"📚 {item.RequiredHomework}",
                UnlockMethod.StreakReward => $"🔥 {item.RequiredStreak}d",
                UnlockMethod.Achievement => "🏆",
                UnlockMethod.Default => "✨ Free",
                _ => "🔒"
            };
        }

        /// <summary>
        /// Clears the slot display.
        /// </summary>
        public void Clear()
        {
            currentItem = null;
            isOwned = false;
            gameObject.SetActive(false);
        }
    }
}
