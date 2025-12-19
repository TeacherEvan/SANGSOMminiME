using System;
using System.Collections.Generic;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Family engagement system supporting multi-user families and collaborative features
    /// Research-backed: Family involvement increases academic achievement by 30%+
    /// </summary>
    public class FamilySystem : MonoBehaviour
    {
        [Header("Family Settings")]
        [SerializeField] private int maxFamilyMembers = 6;
        [SerializeField] private bool enableFamilyGifts = true;
        [SerializeField] private bool enableFamilyLeaderboard = true;
#pragma warning disable CS0414 // Field assigned but never used - configured via Inspector for future privacy features
        [SerializeField] private bool privacyMode = true; // Hide other families' data
#pragma warning restore CS0414

        private Dictionary<string, FamilyGroup> families = new Dictionary<string, FamilyGroup>();
        public static FamilySystem Instance { get; private set; }

        // Events
        public System.Action<FamilyGroup> OnFamilyCreated;
        public System.Action<string, UserProfile> OnMemberJoined;
        public System.Action<FamilyGift> OnGiftSent;
        public System.Action<FamilyChallenge> OnChallengeCompleted;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Create a new family group
        /// </summary>
        public FamilyGroup CreateFamily(string familyName, UserProfile creator)
        {
            if (families.ContainsKey(familyName))
            {
                Debug.LogWarning($"Family '{familyName}' already exists");
                return null;
            }

            var family = new FamilyGroup(familyName, creator);
            families[familyName] = family;
            OnFamilyCreated?.Invoke(family);

            Debug.Log($"Family '{familyName}' created by {creator.DisplayName}");
            return family;
        }

        /// <summary>
        /// Add family member
        /// </summary>
        public bool JoinFamily(string familyName, UserProfile user)
        {
            if (!families.TryGetValue(familyName, out FamilyGroup family))
            {
                Debug.LogWarning($"Family '{familyName}' not found");
                return false;
            }

            if (family.Members.Count >= maxFamilyMembers)
            {
                Debug.LogWarning($"Family '{familyName}' is full");
                return false;
            }

            family.AddMember(user);
            OnMemberJoined?.Invoke(familyName, user);

            Debug.Log($"{user.DisplayName} joined family '{familyName}'");
            return true;
        }

        /// <summary>
        /// Send gift between family members (coins, items, encouragement)
        /// </summary>
        public bool SendGift(UserProfile sender, UserProfile recipient, FamilyGiftType type, int amount, string message = "")
        {
            if (!enableFamilyGifts) return false;

            var gift = new FamilyGift
            {
                SenderId = sender.UserId,
                SenderName = sender.DisplayName,
                RecipientId = recipient.UserId,
                RecipientName = recipient.DisplayName,
                GiftType = type,
                Amount = amount,
                Message = message,
                Timestamp = DateTime.Now
            };

            // Process gift
            switch (type)
            {
                case FamilyGiftType.Coins:
                    if (sender.SpendCoins(amount))
                    {
                        recipient.AddCoins(amount);
                        OnGiftSent?.Invoke(gift);
                        return true;
                    }
                    break;

                case FamilyGiftType.Encouragement:
                    recipient.IncreaseHappiness(10f);
                    OnGiftSent?.Invoke(gift);
                    return true;

                case FamilyGiftType.HelperBonus:
                    recipient.AddExperience(amount);
                    OnGiftSent?.Invoke(gift);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Get family leaderboard for motivation (privacy-respecting)
        /// </summary>
        public List<FamilyLeaderboardEntry> GetFamilyLeaderboard(string familyName)
        {
            if (!enableFamilyLeaderboard || !families.TryGetValue(familyName, out FamilyGroup family))
            {
                return new List<FamilyLeaderboardEntry>();
            }

            var entries = new List<FamilyLeaderboardEntry>();
            foreach (var member in family.Members)
            {
                entries.Add(new FamilyLeaderboardEntry
                {
                    DisplayName = member.DisplayName,
                    HomeworkCompleted = member.HomeworkCompleted,
                    ExperiencePoints = member.ExperiencePoints,
                    CharacterHappiness = member.CharacterHappiness
                });
            }

            // Sort by homework completed (primary motivator)
            entries.Sort((a, b) => b.HomeworkCompleted.CompareTo(a.HomeworkCompleted));
            return entries;
        }

        /// <summary>
        /// Award parent engagement badge
        /// </summary>
        public void AwardParentBadge(UserProfile parent, ParentBadgeType badgeType)
        {
            Debug.Log($"🏆 {parent.DisplayName} earned badge: {badgeType}");
            // Could trigger UI celebration animation
        }
    }

    /// <summary>
    /// Represents a family group
    /// </summary>
    [Serializable]
    public class FamilyGroup
    {
        public string FamilyName;
        public string CreatorId;
        public DateTime CreatedDate;
        public List<UserProfile> Members = new List<UserProfile>();
        public int TotalHomeworkCompleted;
        public int FamilyChallengesCompleted;

        public FamilyGroup(string name, UserProfile creator)
        {
            FamilyName = name;
            CreatorId = creator.UserId;
            CreatedDate = DateTime.Now;
            Members.Add(creator);
        }

        public void AddMember(UserProfile member)
        {
            if (!Members.Contains(member))
            {
                Members.Add(member);
            }
        }
    }

    /// <summary>
    /// Gift system for family encouragement
    /// </summary>
    [Serializable]
    public class FamilyGift
    {
        public string SenderId;
        public string SenderName;
        public string RecipientId;
        public string RecipientName;
        public FamilyGiftType GiftType;
        public int Amount;
        public string Message;
        public DateTime Timestamp;
    }

    public enum FamilyGiftType
    {
        Coins,
        Encouragement,
        HelperBonus
    }

    /// <summary>
    /// Family challenge for collaborative homework
    /// </summary>
    [Serializable]
    public class FamilyChallenge
    {
        public string ChallengeId;
        public string Title;
        public string Description;
        public int RequiredHomework;
        public int CurrentProgress;
        public int RewardCoins;
        public bool IsCompleted;
    }

    /// <summary>
    /// Leaderboard entry (privacy-respecting)
    /// </summary>
    [Serializable]
    public class FamilyLeaderboardEntry
    {
        public string DisplayName;
        public int HomeworkCompleted;
        public int ExperiencePoints;
        public float CharacterHappiness;
    }

    /// <summary>
    /// Parent engagement badges
    /// </summary>
    public enum ParentBadgeType
    {
        FirstHomeworkHelp,
        WeeklyChampion,
        MonthlyMentor,
        ReadingBuddy,
        MotivationalMaster,
        CollaborationKing
    }
}
