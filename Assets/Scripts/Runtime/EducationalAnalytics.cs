using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Analytics event data structure
    /// </summary>
    [Serializable]
    public class AnalyticsEvent
    {
        public string eventName;
        public string userId;
        public string timestamp; // Serialized as string for JSON compatibility
        public List<AnalyticsParameter> parameters; // List for JsonUtility serialization

        public AnalyticsEvent(string eventName, string userId)
        {
            this.eventName = eventName;
            this.userId = userId;
            this.timestamp = DateTime.UtcNow.ToString("o");
            this.parameters = new List<AnalyticsParameter>();
        }

        public AnalyticsEvent AddParameter(string key, object value)
        {
            parameters.Add(new AnalyticsParameter { key = key, value = value.ToString() });
            return this;
        }
    }

    [Serializable]
    public struct AnalyticsParameter
    {
        public string key;
        public string value;
    }

    /// <summary>
    /// Tracks educational metrics and user engagement for the Mini-Me system.
    /// Provides insights for teachers and administrators.
    /// Implements async persistence to prevent data loss.
    /// </summary>
    public class EducationalAnalytics : MonoBehaviour
    {
        [Header("Analytics Settings")]
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private bool logEventsToConsole = true;
        [SerializeField] private int maxEventsInMemory = 1000;
        [SerializeField] private string analyticsFileName = "analytics_log.json";

        private List<AnalyticsEvent> eventBuffer = new List<AnalyticsEvent>();
        private string saveFilePath;
        private bool isSaving = false;

        public static EducationalAnalytics Instance { get; private set; }

        // Events for external listeners
        public event Action<AnalyticsEvent> OnEventTracked;
        public event Action<string, int> OnHomeworkMilestone;
        public event Action<string, int> OnLevelUp;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAnalytics();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAnalytics()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, analyticsFileName);
            LoadAnalytics();
        }

        private void Start()
        {
            // Subscribe to relevant events
            if (UserManager.Instance != null)
            {
                UserManager.Instance.OnUserLoggedIn += OnUserLoggedIn;
                UserManager.Instance.OnUserLoggedOut += OnUserLoggedOut;
                UserManager.Instance.OnUserCreated += OnUserCreated;
            }
        }

        /// <summary>
        /// Track a custom analytics event
        /// </summary>
        public void TrackEvent(AnalyticsEvent analyticsEvent)
        {
            if (!enableAnalytics) return;

            eventBuffer.Add(analyticsEvent);

            // Trim buffer if too large (keep latest)
            if (eventBuffer.Count > maxEventsInMemory)
            {
                eventBuffer.RemoveAt(0);
            }

            if (logEventsToConsole)
            {
                Debug.Log($"[Analytics] {analyticsEvent.eventName} - User: {analyticsEvent.userId}");
            }

            OnEventTracked?.Invoke(analyticsEvent);

            // Auto-save on critical events or periodically could be added here
            // For now, we save on session end/backgrounding
        }

        private async void SaveAnalytics()
        {
            if (isSaving || eventBuffer.Count == 0) return;
            isSaving = true;

            try
            {
                string json = JsonUtility.ToJson(new AnalyticsWrapper { events = eventBuffer }, true);
                await Task.Run(() => File.WriteAllText(saveFilePath, json));
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Analytics] Failed to save analytics: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void LoadAnalytics()
        {
            if (!File.Exists(saveFilePath)) return;

            try
            {
                string json = File.ReadAllText(saveFilePath);
                var wrapper = JsonUtility.FromJson<AnalyticsWrapper>(json);
                if (wrapper != null && wrapper.events != null)
                {
                    eventBuffer = wrapper.events;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Analytics] Failed to load analytics: {ex.Message}");
            }
        }

        private void OnApplicationQuit()
        {
            SaveAnalytics();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveAnalytics();
        }

        [Serializable]
        private class AnalyticsWrapper
        {
            public List<AnalyticsEvent> events;
        }

        /// <summary>
        /// Track homework completion event
        /// </summary>
        public void TrackHomeworkCompleted(UserProfile user)
        {
            if (user == null) return;

            var analyticsEvent = new AnalyticsEvent("homework_completed", user.UserId)
                .AddParameter("homework_count", user.HomeworkCompleted)
                .AddParameter("total_experience", user.ExperiencePoints)
                .AddParameter("happiness_level", user.CharacterHappiness);

            TrackEvent(analyticsEvent);

            // Check for milestones
            CheckHomeworkMilestones(user);
        }

        /// <summary>
        /// Track character interaction event
        /// </summary>
        public void TrackCharacterInteraction(string userId, CharacterAnimation animation)
        {
            var analyticsEvent = new AnalyticsEvent("character_interaction", userId)
                .AddParameter("animation", animation.ToString());

            TrackEvent(analyticsEvent);
        }

        /// <summary>
        /// Track customization change
        /// </summary>
        public void TrackCustomizationChange(UserProfile user, CustomizationCategory category, string newValue)
        {
            if (user == null) return;

            var analyticsEvent = new AnalyticsEvent("customization_changed", user.UserId)
                .AddParameter("category", category.ToString())
                .AddParameter("new_value", newValue);

            TrackEvent(analyticsEvent);
        }

        /// <summary>
        /// Track session start
        /// </summary>
        public void TrackSessionStart(UserProfile user)
        {
            if (user == null) return;

            var analyticsEvent = new AnalyticsEvent("session_start", user.UserId)
                .AddParameter("days_active", user.DaysActive)
                .AddParameter("current_level", GameUtilities.CalculateLevel(user.ExperiencePoints));

            TrackEvent(analyticsEvent);
        }

        /// <summary>
        /// Track session end
        /// </summary>
        public void TrackSessionEnd(UserProfile user, float sessionDurationMinutes)
        {
            if (user == null) return;

            var analyticsEvent = new AnalyticsEvent("session_end", user.UserId)
                .AddParameter("duration_minutes", sessionDurationMinutes)
                .AddParameter("final_happiness", user.CharacterHappiness);

            TrackEvent(analyticsEvent);
        }

        /// <summary>
        /// Track level up event
        /// </summary>
        public void TrackLevelUp(UserProfile user, int newLevel)
        {
            if (user == null) return;

            var analyticsEvent = new AnalyticsEvent("level_up", user.UserId)
                .AddParameter("new_level", newLevel)
                .AddParameter("total_experience", user.ExperiencePoints)
                .AddParameter("homework_completed", user.HomeworkCompleted);

            TrackEvent(analyticsEvent);
            OnLevelUp?.Invoke(user.UserId, newLevel);
        }

        /// <summary>
        /// Get summary statistics for a user
        /// </summary>
        public UserAnalyticsSummary GetUserSummary(UserProfile user)
        {
            if (user == null) return null;

            int eventsCount = 0;
            int homeworkEvents = 0;
            int interactionEvents = 0;

            foreach (var evt in eventBuffer)
            {
                if (evt.userId == user.UserId)
                {
                    eventsCount++;
                    if (evt.eventName == "homework_completed") homeworkEvents++;
                    if (evt.eventName == "character_interaction") interactionEvents++;
                }
            }

            return new UserAnalyticsSummary
            {
                UserId = user.UserId,
                TotalEvents = eventsCount,
                HomeworkCompletions = homeworkEvents,
                CharacterInteractions = interactionEvents,
                CurrentLevel = GameUtilities.CalculateLevel(user.ExperiencePoints),
                TotalHomeworkCompleted = user.HomeworkCompleted,
                AverageHappiness = user.CharacterHappiness
            };
        }

        /// <summary>
        /// Get all tracked events for export/analysis
        /// </summary>
        public List<AnalyticsEvent> GetAllEvents()
        {
            return new List<AnalyticsEvent>(eventBuffer);
        }

        /// <summary>
        /// Clear the event buffer
        /// </summary>
        public void ClearEventBuffer()
        {
            eventBuffer.Clear();
        }

        private void CheckHomeworkMilestones(UserProfile user)
        {
            int[] milestones = { 1, 5, 10, 25, 50, 100, 250, 500, 1000 };

            foreach (int milestone in milestones)
            {
                if (user.HomeworkCompleted == milestone)
                {
                    var milestoneEvent = new AnalyticsEvent("homework_milestone", user.UserId)
                        .AddParameter("milestone", milestone);

                    TrackEvent(milestoneEvent);
                    OnHomeworkMilestone?.Invoke(user.UserId, milestone);

                    if (logEventsToConsole)
                    {
                        Debug.Log($"[Analytics] Homework Milestone reached: {milestone} for user {user.DisplayName}");
                    }
                    break;
                }
            }
        }

        private void OnUserLoggedIn(UserProfile user)
        {
            TrackSessionStart(user);
        }

        private void OnUserLoggedOut()
        {
            // Session tracking could be enhanced with actual duration calculation
        }

        private void OnUserCreated(UserProfile user)
        {
            var analyticsEvent = new AnalyticsEvent("user_created", user.UserId)
                .AddParameter("display_name", user.DisplayName);

            TrackEvent(analyticsEvent);
        }

        private void OnDestroy()
        {
            if (UserManager.Instance != null)
            {
                UserManager.Instance.OnUserLoggedIn -= OnUserLoggedIn;
                UserManager.Instance.OnUserLoggedOut -= OnUserLoggedOut;
                UserManager.Instance.OnUserCreated -= OnUserCreated;
            }
        }
    }

    /// <summary>
    /// Summary of analytics data for a specific user
    /// </summary>
    [Serializable]
    public class UserAnalyticsSummary
    {
        public string UserId;
        public int TotalEvents;
        public int HomeworkCompletions;
        public int CharacterInteractions;
        public int CurrentLevel;
        public int TotalHomeworkCompleted;
        public float AverageHappiness;
    }
}
