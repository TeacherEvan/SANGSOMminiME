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
            experiencePoints += amount;
            OnExperienceChanged?.Invoke(experiencePoints);
        }

        public void AddCoins(int amount)
        {
            coins += amount;
            OnCoinsChanged?.Invoke(coins);
        }

        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                OnCoinsChanged?.Invoke(coins);
                return true;
            }
            return false;
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
        }

        public void IncreaseHappiness(float amount)
        {
            characterHappiness = Mathf.Clamp(characterHappiness + amount, 0f, 100f);
            OnHappinessChanged?.Invoke(characterHappiness);
        }

        public void DecreaseHappiness(float amount)
        {
            characterHappiness = Mathf.Clamp(characterHappiness - amount, 0f, 100f);
            OnHappinessChanged?.Invoke(characterHappiness);
        }

        public void SetEyeScale(float scale, GameConfiguration config = null)
        {
            float minScale = config != null ? config.MinEyeScale : GameConstants.MinEyeScale;
            float maxScale = config != null ? config.MaxEyeScale : GameConstants.MaxEyeScale;
            eyeScale = Mathf.Clamp(scale, minScale, maxScale);
        }

        public void SetOutfit(string outfit)
        {
            currentOutfit = outfit;
        }

        public void SetAccessory(string accessory)
        {
            currentAccessory = accessory;
        }
    }
}