using System;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Represents a user profile in the Sangsom Mini-Me system
    /// </summary>
    [Serializable]
    public class UserProfile
    {
        [SerializeField] private string userId;
        [SerializeField] private string userName;
        [SerializeField] private string displayName;
        [SerializeField] private DateTime createdDate;
        [SerializeField] private int experiencePoints;
        [SerializeField] private int coins;
        [SerializeField] private bool isActive;

        // Character customization preferences
        [SerializeField] private float eyeScale = 1.0f;
        [SerializeField] private string currentOutfit = "default";
        [SerializeField] private string currentAccessory = "none";

        // Progress tracking
        [SerializeField] private int homeworkCompleted;
        [SerializeField] private int daysActive;
        [SerializeField] private float characterHappiness = 50f;

        // Dirty flag propagation helper to avoid missed saves and unnecessary disk writes
        private void MarkDirty()
        {
            UserManager.Instance?.MarkDirty();
        }

        // Events
        public event System.Action<int> OnCoinsChanged;
        public event System.Action<int> OnExperienceChanged;
        public event System.Action<float> OnHappinessChanged;

        public string UserId => userId;
        public string UserName => userName;
        public string DisplayName => displayName;
        public DateTime CreatedDate => createdDate;
        public int ExperiencePoints => experiencePoints;
        public int Coins => coins;
        public bool IsActive => isActive;

        public float EyeScale => eyeScale;
        public string CurrentOutfit => currentOutfit;
        public string CurrentAccessory => currentAccessory;

        public int HomeworkCompleted => homeworkCompleted;
        public int DaysActive => daysActive;
        public float CharacterHappiness => characterHappiness;

        public UserProfile(string userName, string displayName, GameConfiguration config = null)
        {
            this.userId = Guid.NewGuid().ToString();
            this.userName = userName;
            this.displayName = displayName;
            this.createdDate = DateTime.Now;
            this.experiencePoints = 0;

            // Use config if provided, otherwise use defaults
            if (config != null)
            {
                this.coins = config.StartingCoins;
                this.characterHappiness = config.StartingHappiness;
                this.daysActive = config.StartingDaysActive;
            }
            else
            {
                this.coins = GameConstants.DefaultStartingCoins;
                this.characterHappiness = GameConstants.DefaultHappiness;
                this.daysActive = 1;
            }

            this.isActive = true;
            this.homeworkCompleted = 0;
        }

        public void AddExperience(int amount)
        {
            // Validation for anti-cheat and overflow protection
            if (!ValidationUtilities.ValidateCurrencyAmount(amount, "Experience", out string error))
            {
                ValidationUtilities.LogValidationError("AddExperience", error);
                return;
            }

            // Prevent overflow
            if (experiencePoints + amount > GameConstants.MaxExperience)
            {
                experiencePoints = GameConstants.MaxExperience;
            }
            else
            {
                experiencePoints += amount;
            }

            OnExperienceChanged?.Invoke(experiencePoints);
            MarkDirty();
        }

        public void AddCoins(int amount)
        {
            // Validation for anti-cheat and overflow protection
            if (!ValidationUtilities.ValidateCurrencyAmount(amount, "Coins", out string error))
            {
                ValidationUtilities.LogValidationError("AddCoins", error);
                return;
            }

            // Prevent overflow
            if (coins + amount > GameConstants.MaxCoins)
            {
                coins = GameConstants.MaxCoins;
            }
            else
            {
                coins += amount;
            }

            OnCoinsChanged?.Invoke(coins);
            MarkDirty();
        }

        public bool SpendCoins(int amount)
        {
            // Enhanced validation for spending
            if (!ValidationUtilities.ValidateResourceSpending(coins, amount, "Coins", out string error))
            {
                ValidationUtilities.LogValidationError("SpendCoins", error);
                return false;
            }

            coins -= amount;
            OnCoinsChanged?.Invoke(coins);
            MarkDirty();
            return true;
        }

        public void CompleteHomework(GameConfiguration config = null)
        {
            homeworkCompleted++;

            if (config != null)
            {
                AddExperience(config.HomeworkExperienceReward);
                AddCoins(config.HomeworkCoinReward);
                IncreaseHappiness(config.HomeworkHappinessReward);
            }
            else
            {
                AddExperience(GameConstants.HomeworkExperienceReward);
                AddCoins(GameConstants.HomeworkCoinReward);
                IncreaseHappiness(GameConstants.HomeworkHappinessReward);
            }

            MarkDirty();
        }

        public void IncreaseHappiness(float amount)
        {
            characterHappiness = Mathf.Clamp(characterHappiness + amount, 0f, 100f);
            OnHappinessChanged?.Invoke(characterHappiness);
            MarkDirty();
        }

        public void DecreaseHappiness(float amount)
        {
            characterHappiness = Mathf.Clamp(characterHappiness - amount, 0f, 100f);
            OnHappinessChanged?.Invoke(characterHappiness);
            MarkDirty();
        }

        public void SetEyeScale(float scale, GameConfiguration config = null)
        {
            float minScale = config != null ? config.MinEyeScale : GameConstants.MinEyeScale;
            float maxScale = config != null ? config.MaxEyeScale : GameConstants.MaxEyeScale;
            eyeScale = Mathf.Clamp(scale, minScale, maxScale);
            MarkDirty();
        }

        public void SetOutfit(string outfit)
        {
            currentOutfit = outfit;
            MarkDirty();
        }

        public void SetAccessory(string accessory)
        {
            currentAccessory = accessory;
            MarkDirty();
        }
    }
}