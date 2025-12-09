using UnityEngine;
using System;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Manages the daily login bonus system with positive-only streak mechanics.
    /// No penalties for missing days - only rewards for consecutive logins.
    /// </summary>
    public static class DailyLoginSystem
    {
        // Events for UI celebration triggers
        public static event Action<LoginBonusResult> OnLoginBonusAwarded;
        public static event Action<int> OnMilestoneReached;
        public static event Action<int> OnNewStreakRecord;

        /// <summary>
        /// Processes login for a user and triggers appropriate events.
        /// Call this when a user logs in.
        /// </summary>
        /// <param name="user">The user profile to process</param>
        /// <returns>Login bonus result with all reward details</returns>
        public static LoginBonusResult ProcessLogin(UserProfile user)
        {
            if (user == null)
            {
                Debug.LogWarning("[DailyLoginSystem] Cannot process login for null user.");
                return default;
            }

            var result = user.ProcessDailyLogin();

            if (result.IsFirstLoginToday)
            {
                // Trigger celebration events
                OnLoginBonusAwarded?.Invoke(result);

                if (result.HitMilestone)
                {
                    OnMilestoneReached?.Invoke(result.MilestoneDay);
                    Debug.Log($"[DailyLoginSystem] 🎉 Milestone reached: {result.MilestoneDay} day streak! Bonus: +{result.MilestoneBonus} coins");
                }

                if (result.IsNewRecord)
                {
                    OnNewStreakRecord?.Invoke(result.LongestStreak);
                    Debug.Log($"[DailyLoginSystem] 🏆 New streak record: {result.LongestStreak} days!");
                }

                Debug.Log($"[DailyLoginSystem] Daily login bonus: +{result.CoinsEarned} coins (Base: {result.BaseBonus}, Streak: {result.StreakBonus}, Milestone: {result.MilestoneBonus}) | Streak: {result.CurrentStreak} days");
            }
            else
            {
                Debug.Log($"[DailyLoginSystem] Welcome back! Already received today's bonus. Current streak: {result.CurrentStreak} days");
            }

            return result;
        }

        /// <summary>
        /// Gets a celebration message for the login bonus.
        /// </summary>
        public static string GetCelebrationMessage(LoginBonusResult result)
        {
            if (!result.IsFirstLoginToday)
            {
                return $"Welcome back! Streak: {result.CurrentStreak} days 🔥";
            }

            if (result.HitMilestone)
            {
                return GetMilestoneMessage(result.MilestoneDay, result.CoinsEarned);
            }

            if (result.IsNewRecord)
            {
                return $"🏆 New Record! {result.LongestStreak} day streak!\n+{result.CoinsEarned} coins";
            }

            if (result.CurrentStreak > 1)
            {
                return $"🔥 {result.CurrentStreak} day streak!\n+{result.CoinsEarned} coins";
            }

            return $"Welcome! +{result.CoinsEarned} coins";
        }

        /// <summary>
        /// Gets a special message for milestone achievements.
        /// </summary>
        private static string GetMilestoneMessage(int milestoneDay, int totalCoins)
        {
            switch (milestoneDay)
            {
                case 3:
                    return $"🌟 3 Day Streak!\nYou're building great habits!\n+{totalCoins} coins";
                case 7:
                    return $"⭐ 1 Week Streak!\nAmazing dedication!\n+{totalCoins} coins";
                case 14:
                    return $"🌟⭐ 2 Week Streak!\nYou're a superstar!\n+{totalCoins} coins";
                case 30:
                    return $"👑 1 Month Streak!\nIncredible achievement!\n+{totalCoins} coins";
                default:
                    return $"🎉 {milestoneDay} Day Milestone!\n+{totalCoins} coins";
            }
        }

        /// <summary>
        /// Gets the emoji for streak display based on streak length.
        /// </summary>
        public static string GetStreakEmoji(int streak)
        {
            if (streak >= 30) return "👑";
            if (streak >= 14) return "⭐";
            if (streak >= 7) return "🌟";
            if (streak >= 3) return "🔥";
            return "✨";
        }

        /// <summary>
        /// Gets days until next milestone.
        /// </summary>
        public static int GetDaysUntilNextMilestone(int currentStreak)
        {
            if (currentStreak < 3) return 3 - currentStreak;
            if (currentStreak < 7) return 7 - currentStreak;
            if (currentStreak < 14) return 14 - currentStreak;
            if (currentStreak < 30) return 30 - currentStreak;
            return 0; // All milestones achieved
        }

        /// <summary>
        /// Gets the next milestone day number.
        /// </summary>
        public static int GetNextMilestoneDay(int currentStreak)
        {
            if (currentStreak < 3) return 3;
            if (currentStreak < 7) return 7;
            if (currentStreak < 14) return 14;
            if (currentStreak < 30) return 30;
            return 0; // All milestones achieved
        }
    }
}
