using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Utility methods for common operations across the Sangsom Mini-Me system
    /// </summary>
    public static class GameUtilities
    {
        /// <summary>
        /// Gets the mood state based on happiness value
        /// </summary>
        public static MoodState GetMoodState(float happiness)
        {
            if (happiness >= GameConstants.VeryHappyThreshold)
                return MoodState.VeryHappy;
            if (happiness >= GameConstants.HappyThreshold)
                return MoodState.Happy;
            if (happiness >= GameConstants.NeutralThreshold)
                return MoodState.Neutral;
            if (happiness >= GameConstants.SadThreshold)
                return MoodState.Sad;
            
            return MoodState.VerySad;
        }
        
        /// <summary>
        /// Gets a display string for the mood state
        /// </summary>
        public static string GetMoodDisplayText(MoodState mood)
        {
            return mood switch
            {
                MoodState.VeryHappy => "Very Happy! 😄",
                MoodState.Happy => "Happy 😊",
                MoodState.Neutral => "Okay 😐",
                MoodState.Sad => "Sad 😢",
                MoodState.VerySad => "Very Sad 😭",
                _ => "Unknown"
            };
        }
        
        /// <summary>
        /// Gets a color representing the mood state
        /// </summary>
        public static Color GetMoodColor(MoodState mood)
        {
            return mood switch
            {
                MoodState.VeryHappy => new Color(0.2f, 0.8f, 0.2f), // Bright Green
                MoodState.Happy => new Color(0.5f, 0.8f, 0.3f),     // Light Green
                MoodState.Neutral => new Color(0.8f, 0.8f, 0.2f),   // Yellow
                MoodState.Sad => new Color(0.8f, 0.5f, 0.2f),       // Orange
                MoodState.VerySad => new Color(0.8f, 0.2f, 0.2f),   // Red
                _ => Color.gray
            };
        }
        
        /// <summary>
        /// Calculates level from experience points
        /// </summary>
        public static int CalculateLevel(int experiencePoints)
        {
            return experiencePoints / GameConstants.ExperiencePerLevel + 1;
        }
        
        /// <summary>
        /// Gets experience progress within current level (0-1)
        /// </summary>
        public static float GetLevelProgress(int experiencePoints)
        {
            int expInLevel = experiencePoints % GameConstants.ExperiencePerLevel;
            return expInLevel / (float)GameConstants.ExperiencePerLevel;
        }
        
        /// <summary>
        /// Formats experience display text
        /// </summary>
        public static string FormatExperienceDisplay(int experiencePoints)
        {
            int level = CalculateLevel(experiencePoints);
            int expInLevel = experiencePoints % GameConstants.ExperiencePerLevel;
            return $"Level {level} ({expInLevel}/{GameConstants.ExperiencePerLevel} XP)";
        }
        
        /// <summary>
        /// Formats coin display text
        /// </summary>
        public static string FormatCoinsDisplay(int coins)
        {
            if (coins >= 1000000)
                return $"{coins / 1000000f:F1}M";
            if (coins >= 1000)
                return $"{coins / 1000f:F1}K";
            return coins.ToString();
        }
        
        /// <summary>
        /// Validates username format
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
            
            if (username.Length < 3 || username.Length > 20)
                return false;
            
            // Only allow alphanumeric and underscore
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validates display name format
        /// </summary>
        public static bool IsValidDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                return false;
            
            return displayName.Length >= 1 && displayName.Length <= 50;
        }
        
        /// <summary>
        /// Gets a random animation for character interaction
        /// </summary>
        public static CharacterAnimation GetRandomAnimation()
        {
            // Exclude Idle from random selection
            CharacterAnimation[] animations = 
            {
                CharacterAnimation.Dance,
                CharacterAnimation.Wave,
                CharacterAnimation.Wai,
                CharacterAnimation.Curtsy,
                CharacterAnimation.Bow
            };
            
            return animations[Random.Range(0, animations.Length)];
        }
        
        /// <summary>
        /// Clamps a value between min and max happiness
        /// </summary>
        public static float ClampHappiness(float value)
        {
            return Mathf.Clamp(value, GameConstants.MinHappiness, GameConstants.MaxHappiness);
        }
        
        /// <summary>
        /// Clamps a value between min and max eye scale
        /// </summary>
        public static float ClampEyeScale(float value)
        {
            return Mathf.Clamp(value, GameConstants.MinEyeScale, GameConstants.MaxEyeScale);
        }
        
        /// <summary>
        /// Gets time-based greeting message
        /// </summary>
        public static string GetTimeBasedGreeting()
        {
            int hour = System.DateTime.Now.Hour;
            
            if (hour >= 5 && hour < 12)
                return "Good morning";
            if (hour >= 12 && hour < 17)
                return "Good afternoon";
            if (hour >= 17 && hour < 21)
                return "Good evening";
            
            return "Hello";
        }
        
        /// <summary>
        /// Gets a motivational message based on homework count
        /// </summary>
        public static string GetHomeworkMotivation(int homeworkCompleted)
        {
            if (homeworkCompleted == 0)
                return "Complete your first homework to make your Mini-Me happy!";
            if (homeworkCompleted < 5)
                return "Great start! Keep going!";
            if (homeworkCompleted < 10)
                return "You're doing amazing! Your Mini-Me is so proud!";
            if (homeworkCompleted < 25)
                return "Incredible progress! You're a homework champion!";
            if (homeworkCompleted < 50)
                return "Outstanding! Your dedication is inspiring!";
            if (homeworkCompleted < 100)
                return "You're a superstar student!";
            
            return "You're a true master of learning!";
        }
    }
}
