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
        
        public UserProfile(string userName, string displayName)
        {
            this.userId = Guid.NewGuid().ToString();
            this.userName = userName;
            this.displayName = displayName;
            this.createdDate = DateTime.Now;
            this.experiencePoints = 0;
            this.coins = 100; // Starting coins
            this.isActive = true;
            this.homeworkCompleted = 0;
            this.daysActive = 1;
            this.characterHappiness = 75f; // Start with happy character
        }
        
        public void AddExperience(int amount)
        {
            experiencePoints += amount;
        }
        
        public void AddCoins(int amount)
        {
            coins += amount;
        }
        
        public bool SpendCoins(int amount)
        {
            if (coins >= amount)
            {
                coins -= amount;
                return true;
            }
            return false;
        }
        
        public void CompleteHomework()
        {
            homeworkCompleted++;
            AddExperience(10);
            AddCoins(5);
            IncreaseHappiness(5f);
        }
        
        public void IncreaseHappiness(float amount)
        {
            characterHappiness = Mathf.Clamp(characterHappiness + amount, 0f, 100f);
        }
        
        public void DecreaseHappiness(float amount)
        {
            characterHappiness = Mathf.Clamp(characterHappiness - amount, 0f, 100f);
        }
        
        public void SetEyeScale(float scale)
        {
            eyeScale = Mathf.Clamp(scale, 0.5f, 2.0f);
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