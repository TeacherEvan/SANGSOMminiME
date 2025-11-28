using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// ScriptableObject-based configuration for game settings.
    /// This allows designers to modify game balance without code changes.
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "SangsomMiniMe/Game Configuration", order = 1)]
    public class GameConfiguration : ScriptableObject
    {
        [Header("User Settings")]
        [Tooltip("Starting coins for new users")]
        [SerializeField] private int startingCoins = 100;
        
        [Tooltip("Starting character happiness for new users (0-100)")]
        [Range(0f, 100f)]
        [SerializeField] private float startingHappiness = 75f;
        
        [Tooltip("Starting days active for new users")]
        [SerializeField] private int startingDaysActive = 1;
        
        [Header("Eye Customization")]
        [Tooltip("Minimum eye scale multiplier")]
        [Range(0.1f, 1f)]
        [SerializeField] private float minEyeScale = 0.5f;
        
        [Tooltip("Maximum eye scale multiplier")]
        [Range(1f, 3f)]
        [SerializeField] private float maxEyeScale = 2.0f;
        
        [Header("Homework Rewards")]
        [Tooltip("Experience points awarded for completing homework")]
        [SerializeField] private int homeworkExperienceReward = 10;
        
        [Tooltip("Coins awarded for completing homework")]
        [SerializeField] private int homeworkCoinReward = 5;
        
        [Tooltip("Happiness increase for completing homework")]
        [SerializeField] private float homeworkHappinessReward = 5f;
        
        [Header("Happiness System")]
        [Tooltip("Happiness threshold above which happy particles appear")]
        [Range(0f, 100f)]
        [SerializeField] private float happyThreshold = 70f;
        
        [Tooltip("Happiness threshold below which sadness indicator appears")]
        [Range(0f, 100f)]
        [SerializeField] private float sadThreshold = 30f;
        
        [Tooltip("Happiness increase from dancing")]
        [SerializeField] private float danceHappinessBonus = 2f;
        
        [Header("Auto-Save Settings")]
        [Tooltip("Time in seconds between auto-saves")]
        [SerializeField] private float autoSaveInterval = 30f;
        
        [Tooltip("Enable automatic saving")]
        [SerializeField] private bool enableAutoSave = true;
        
        [Header("Animation Settings")]
        [Tooltip("Duration of character animations in seconds")]
        [SerializeField] private float animationDuration = 2f;
        
        [Header("Level System")]
        [Tooltip("Experience points required per level")]
        [SerializeField] private int experiencePerLevel = 100;
        
        // Public properties for read-only access
        public int StartingCoins => startingCoins;
        public float StartingHappiness => startingHappiness;
        public int StartingDaysActive => startingDaysActive;
        
        public float MinEyeScale => minEyeScale;
        public float MaxEyeScale => maxEyeScale;
        
        public int HomeworkExperienceReward => homeworkExperienceReward;
        public int HomeworkCoinReward => homeworkCoinReward;
        public float HomeworkHappinessReward => homeworkHappinessReward;
        
        public float HappyThreshold => happyThreshold;
        public float SadThreshold => sadThreshold;
        public float DanceHappinessBonus => danceHappinessBonus;
        
        public float AutoSaveInterval => autoSaveInterval;
        public bool EnableAutoSave => enableAutoSave;
        
        public float AnimationDuration => animationDuration;
        public int ExperiencePerLevel => experiencePerLevel;
        
        /// <summary>
        /// Calculates the level from experience points
        /// </summary>
        public int CalculateLevel(int experiencePoints)
        {
            return experiencePoints / experiencePerLevel + 1;
        }
        
        /// <summary>
        /// Gets the experience progress within current level (0-1)
        /// </summary>
        public float GetLevelProgress(int experiencePoints)
        {
            return (experiencePoints % experiencePerLevel) / (float)experiencePerLevel;
        }
        
        /// <summary>
        /// Gets experience needed for next level
        /// </summary>
        public int GetExperienceToNextLevel(int experiencePoints)
        {
            return experiencePerLevel - (experiencePoints % experiencePerLevel);
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure min is less than max for eye scale
            if (minEyeScale > maxEyeScale)
            {
                minEyeScale = maxEyeScale;
            }
            
            // Ensure sad threshold is less than happy threshold
            if (sadThreshold > happyThreshold)
            {
                sadThreshold = happyThreshold;
            }
            
            // Ensure positive values
            startingCoins = Mathf.Max(0, startingCoins);
            homeworkExperienceReward = Mathf.Max(0, homeworkExperienceReward);
            homeworkCoinReward = Mathf.Max(0, homeworkCoinReward);
            autoSaveInterval = Mathf.Max(5f, autoSaveInterval);
            experiencePerLevel = Mathf.Max(1, experiencePerLevel);
        }
        #endif
    }
}
