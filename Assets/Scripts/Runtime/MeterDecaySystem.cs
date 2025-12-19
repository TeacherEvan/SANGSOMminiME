using UnityEngine;
using System;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Manages gentle meter decay for the tamagotchi care system.
    /// Implements "cozy" decay mechanics with floors - meters never drop to zero.
    /// No stress mechanics: decay is slow and stops at comfortable floor values.
    /// </summary>
    public static class MeterDecaySystem
    {
        // Events for UI updates
        public static event Action<float, float, float> OnMetersDecayed; // happiness, hunger, energy
        public static event Action<string> OnMeterLow; // meter name when below threshold

        // Low meter thresholds for gentle reminders (not warnings!)
        private const float LowMeterThreshold = 35f;

        /// <summary>
        /// Applies decay to all meters based on elapsed time.
        /// Called from GameManager's coroutine at regular intervals.
        /// </summary>
        /// <param name="user">The user profile to decay</param>
        /// <param name="elapsedMinutes">Time since last decay check</param>
        public static void ApplyDecay(UserProfile user, float elapsedMinutes = 1f)
        {
            if (user == null) return;

            float happinessDecay = GameConstants.HappinessDecayPerMinute * elapsedMinutes;
            float hungerDecay = GameConstants.HungerDecayPerMinute * elapsedMinutes;
            float energyDecay = GameConstants.EnergyDecayPerMinute * elapsedMinutes;

            // Store previous values for comparison
            float prevHappiness = user.CharacterHappiness;
            float prevHunger = user.CharacterHunger;
            float prevEnergy = user.CharacterEnergy;

            // Apply decay (respects floors internally)
            user.ApplyMeterDecay(happinessDecay, hungerDecay, energyDecay);

            // Keep offline catch-up timestamp in sync (ApplyMeterDecay already marks dirty)
            user.SetLastMeterDecayUtcTicks(DateTime.UtcNow.Ticks, markDirty: false);

            // Fire event for UI updates
            OnMetersDecayed?.Invoke(user.CharacterHappiness, user.CharacterHunger, user.CharacterEnergy);

            // Check for low meter gentle reminders (only when crossing threshold)
            CheckLowMeterReminder("Happiness", prevHappiness, user.CharacterHappiness);
            CheckLowMeterReminder("Hunger", prevHunger, user.CharacterHunger);
            CheckLowMeterReminder("Energy", prevEnergy, user.CharacterEnergy);
        }

        /// <summary>
        /// Checks if meter crossed below threshold and fires gentle reminder.
        /// </summary>
        private static void CheckLowMeterReminder(string meterName, float previousValue, float currentValue)
        {
            // Only remind when crossing the threshold downward
            if (previousValue >= LowMeterThreshold && currentValue < LowMeterThreshold)
            {
                OnMeterLow?.Invoke(meterName);
                Debug.Log($"[MeterDecay] Gentle reminder: {meterName} is getting low ({currentValue:F0}%)");
            }
        }

        /// <summary>
        /// Gets a friendly message for low meter reminders.
        /// </summary>
        public static string GetLowMeterMessage(string meterName)
        {
            switch (meterName.ToLower())
            {
                case "happiness":
                    return "Your Mini-Me could use some attention! 💙";
                case "hunger":
                    return "Your Mini-Me is getting hungry! 🍎";
                case "energy":
                    return "Your Mini-Me is getting sleepy! 😴";
                default:
                    return "Your Mini-Me needs some care!";
            }
        }

        /// <summary>
        /// Gets the mood state based on current meter values.
        /// </summary>
        public static MoodState GetOverallMood(UserProfile user)
        {
            if (user == null) return MoodState.Neutral;

            float average = (user.CharacterHappiness + user.CharacterHunger + user.CharacterEnergy) / 3f;

            if (average >= GameConstants.VeryHappyThreshold) return MoodState.VeryHappy;
            if (average >= GameConstants.HappyThreshold) return MoodState.Happy;
            if (average >= GameConstants.NeutralThreshold) return MoodState.Neutral;
            if (average >= GameConstants.SadThreshold) return MoodState.Sad;
            return MoodState.VerySad;
        }

        /// <summary>
        /// Gets a mood emoji for display.
        /// </summary>
        public static string GetMoodEmoji(MoodState mood)
        {
            switch (mood)
            {
                case MoodState.VeryHappy: return "😄";
                case MoodState.Happy: return "🙂";
                case MoodState.Neutral: return "😐";
                case MoodState.Sad: return "😢";
                case MoodState.VerySad: return "😔";
                default: return "🙂";
            }
        }

        /// <summary>
        /// Gets a mood description for UI.
        /// </summary>
        public static string GetMoodDescription(MoodState mood)
        {
            switch (mood)
            {
                case MoodState.VeryHappy: return "Feeling wonderful!";
                case MoodState.Happy: return "Feeling good!";
                case MoodState.Neutral: return "Doing okay";
                case MoodState.Sad: return "Could use some attention";
                case MoodState.VerySad: return "Needs some love";
                default: return "Doing okay";
            }
        }

        /// <summary>
        /// Calculates time until a meter reaches its floor (for UI display).
        /// </summary>
        public static float GetMinutesUntilFloor(float currentValue, float floor, float decayPerMinute)
        {
            if (currentValue <= floor || decayPerMinute <= 0) return float.MaxValue;
            return (currentValue - floor) / decayPerMinute;
        }
    }

    /// <summary>
    /// Mood states for the character based on average meter values.
    /// </summary>
    public enum MoodState
    {
        VeryHappy,
        Happy,
        Neutral,
        Sad,
        VerySad
    }
}
