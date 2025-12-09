using System;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Result data from processing a daily login bonus
    /// </summary>
    [Serializable]
    public struct LoginBonusResult
    {
        public bool IsFirstLoginToday;
        public int CoinsEarned;
        public int CurrentStreak;
        public int LongestStreak;
        public bool IsNewRecord;
        public bool HitMilestone;
        public int MilestoneDay;
        public int BaseBonus;
        public int StreakBonus;
        public int MilestoneBonus;
    }

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
        [SerializeField] private float characterHunger = 75f;
        [SerializeField] private float characterEnergy = 100f;

        // Daily login & streak tracking (positive-only, no penalties)
        [SerializeField] private string lastLoginDateString = "";
        [SerializeField] private int currentStreak;
        [SerializeField] private int longestStreak;

        // Shop inventory - items owned by this user
        [SerializeField] private System.Collections.Generic.List<string> ownedItems = new System.Collections.Generic.List<string>();

        // Dirty flag propagation helper to avoid missed saves and unnecessary disk writes
        private void MarkDirty()
        {
            UserManager.Instance?.MarkDirty();
        }

        // Events
        public event System.Action<int> OnCoinsChanged;
        public event System.Action<int> OnExperienceChanged;
        public event System.Action<float> OnHappinessChanged;
        public event System.Action<float> OnHungerChanged;
        public event System.Action<float> OnEnergyChanged;

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
        public float CharacterHunger => characterHunger;
        public float CharacterEnergy => characterEnergy;

        // Streak properties (read-only)
        public string LastLoginDateString => lastLoginDateString;
        public int CurrentStreak => currentStreak;
        public int LongestStreak => longestStreak;

        // Shop inventory (read-write for ShopManager)
        public System.Collections.Generic.List<string> OwnedItems => ownedItems;

        /// <summary>
        /// Processes daily login and returns bonus information.
        /// Implements positive-only streak system with no penalties for missed days.
        /// </summary>
        /// <returns>LoginBonusResult with coins earned and streak info</returns>
        public LoginBonusResult ProcessDailyLogin()
        {
            var today = DateTime.Now.Date;
            var result = new LoginBonusResult();

            // Parse last login date
            DateTime lastLogin = DateTime.MinValue;
            if (!string.IsNullOrEmpty(lastLoginDateString))
            {
                DateTime.TryParse(lastLoginDateString, out lastLogin);
                lastLogin = lastLogin.Date;
            }

            // Check if already logged in today
            if (lastLogin == today)
            {
                result.IsFirstLoginToday = false;
                result.CurrentStreak = currentStreak;
                result.CoinsEarned = 0;
                return result;
            }

            result.IsFirstLoginToday = true;

            // Calculate streak (positive-only: consecutive days extend, gaps reset to 1)
            if (lastLogin == today.AddDays(-1))
            {
                // Consecutive day - extend streak
                currentStreak++;
            }
            else
            {
                // Gap in logins - reset streak (no penalty, just fresh start)
                currentStreak = 1;
            }

            // Update longest streak achievement
            if (currentStreak > longestStreak)
            {
                longestStreak = currentStreak;
                result.IsNewRecord = true;
            }

            // Calculate bonus coins based on streak
            int baseBonus = GameConstants.DailyLoginBonusCoins;
            int streakBonus = CalculateStreakBonus(currentStreak);
            int totalBonus = baseBonus + streakBonus;

            // Check for milestone bonus
            int milestoneBonus = GetMilestoneBonus(currentStreak);
            if (milestoneBonus > 0)
            {
                totalBonus += milestoneBonus;
                result.HitMilestone = true;
                result.MilestoneDay = currentStreak;
            }

            // Award bonuses
            AddCoins(totalBonus);
            IncreaseHappiness(GameConstants.DailyLoginHappinessBonus);
            daysActive++;

            // Update last login date
            lastLoginDateString = today.ToString("yyyy-MM-dd");

            result.CoinsEarned = totalBonus;
            result.CurrentStreak = currentStreak;
            result.LongestStreak = longestStreak;
            result.BaseBonus = baseBonus;
            result.StreakBonus = streakBonus;
            result.MilestoneBonus = milestoneBonus;

            MarkDirty();
            return result;
        }

        private int CalculateStreakBonus(int streak)
        {
            // Progressive bonus: +1 coin per streak day, capped at 10
            return Mathf.Min(streak - 1, GameConstants.MaxStreakBonusCoins);
        }

        private int GetMilestoneBonus(int streak)
        {
            // Milestone bonuses at 3, 7, 14, 30 days
            switch (streak)
            {
                case 3: return GameConstants.Milestone3DayBonus;
                case 7: return GameConstants.Milestone7DayBonus;
                case 14: return GameConstants.Milestone14DayBonus;
                case 30: return GameConstants.Milestone30DayBonus;
                default: return 0;
            }
        }

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
            MarkDirty();
        }

        public void AddCoins(int amount)
        {
            coins += amount;
            OnCoinsChanged?.Invoke(coins);
            MarkDirty();
        }

        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                OnCoinsChanged?.Invoke(coins);
                MarkDirty();
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

        // ===== HUNGER METHODS =====

        public void IncreaseHunger(float amount)
        {
            characterHunger = Mathf.Clamp(characterHunger + amount, 0f, 100f);
            OnHungerChanged?.Invoke(characterHunger);
            MarkDirty();
        }

        public void DecreaseHunger(float amount, float floor = -1f)
        {
            float minHunger = floor >= 0 ? floor : GameConstants.HungerFloor;
            characterHunger = Mathf.Clamp(characterHunger - amount, minHunger, 100f);
            OnHungerChanged?.Invoke(characterHunger);
            MarkDirty();
        }

        /// <summary>
        /// Feed the character to restore hunger.
        /// </summary>
        public void Feed(float amount = -1f)
        {
            float recovery = amount >= 0 ? amount : GameConstants.FeedHungerRecovery;
            IncreaseHunger(recovery);
        }

        // ===== ENERGY METHODS =====

        public void IncreaseEnergy(float amount)
        {
            characterEnergy = Mathf.Clamp(characterEnergy + amount, 0f, 100f);
            OnEnergyChanged?.Invoke(characterEnergy);
            MarkDirty();
        }

        public void DecreaseEnergy(float amount, float floor = -1f)
        {
            float minEnergy = floor >= 0 ? floor : GameConstants.EnergyFloor;
            characterEnergy = Mathf.Clamp(characterEnergy - amount, minEnergy, 100f);
            OnEnergyChanged?.Invoke(characterEnergy);
            MarkDirty();
        }

        /// <summary>
        /// Rest the character to restore energy.
        /// </summary>
        public void Rest(float amount = -1f)
        {
            float recovery = amount >= 0 ? amount : GameConstants.RestEnergyRecovery;
            IncreaseEnergy(recovery);
        }

        /// <summary>
        /// Apply meter decay with floors (meters never go below floor values).
        /// Called by MeterDecaySystem at regular intervals.
        /// </summary>
        public void ApplyMeterDecay(float happinessDecay, float hungerDecay, float energyDecay)
        {
            // Decay happiness to floor
            if (characterHappiness > GameConstants.HappinessFloor)
            {
                characterHappiness = Mathf.Max(characterHappiness - happinessDecay, GameConstants.HappinessFloor);
                OnHappinessChanged?.Invoke(characterHappiness);
            }

            // Decay hunger to floor
            if (characterHunger > GameConstants.HungerFloor)
            {
                characterHunger = Mathf.Max(characterHunger - hungerDecay, GameConstants.HungerFloor);
                OnHungerChanged?.Invoke(characterHunger);
            }

            // Decay energy to floor
            if (characterEnergy > GameConstants.EnergyFloor)
            {
                characterEnergy = Mathf.Max(characterEnergy - energyDecay, GameConstants.EnergyFloor);
                OnEnergyChanged?.Invoke(characterEnergy);
            }

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