using System;
using System.Collections.Generic;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Photo booth system for capturing character milestones and family memories
    /// Builds emotional bonds and creates shareable achievements
    /// </summary>
    public class PhotoBoothSystem : MonoBehaviour
    {
        [Header("Photo Settings")]
        [SerializeField] private int maxPhotosPerUser = 50;
        [SerializeField] private int photoResolutionWidth = 1920;
        [SerializeField] private int photoResolutionHeight = 1080;
        [SerializeField] private bool enablePhotoSharing = true;

        [Header("Camera References")]
        [SerializeField] private Camera photoCamera;
        [SerializeField] private Transform[] photoBackgrounds;

        private Dictionary<string, List<MemoryPhoto>> userPhotos = new Dictionary<string, List<MemoryPhoto>>();

        public static PhotoBoothSystem Instance { get; private set; }

        // Events
        public System.Action<MemoryPhoto> OnPhotoTaken;
        public System.Action<string> OnMilestoneAchieved;

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
        /// Take a photo of current character state
        /// </summary>
        public MemoryPhoto TakePhoto(UserProfile user, string caption = "")
        {
            if (!userPhotos.ContainsKey(user.UserId))
            {
                userPhotos[user.UserId] = new List<MemoryPhoto>();
            }

            var photos = userPhotos[user.UserId];
            if (photos.Count >= maxPhotosPerUser)
            {
                Debug.LogWarning($"Photo limit reached for {user.DisplayName}");
                return null;
            }

            // Capture photo from camera
            RenderTexture renderTexture = new RenderTexture(photoResolutionWidth, photoResolutionHeight, 24);
            RenderTexture currentRT = RenderTexture.active;

            if (photoCamera != null)
            {
                photoCamera.targetTexture = renderTexture;
                photoCamera.Render();
                RenderTexture.active = renderTexture;

                Texture2D photo = new Texture2D(photoResolutionWidth, photoResolutionHeight, TextureFormat.RGB24, false);
                photo.ReadPixels(new Rect(0, 0, photoResolutionWidth, photoResolutionHeight), 0, 0);
                photo.Apply();

                RenderTexture.active = currentRT;
                photoCamera.targetTexture = null;

                var memoryPhoto = new MemoryPhoto
                {
                    PhotoId = Guid.NewGuid().ToString(),
                    UserId = user.UserId,
                    Caption = caption,
                    Timestamp = DateTime.Now,
                    CharacterHappiness = user.CharacterHappiness,
                    HomeworkCompleted = user.HomeworkCompleted,
                    PhotoData = photo.EncodeToPNG()
                };

                photos.Add(memoryPhoto);
                OnPhotoTaken?.Invoke(memoryPhoto);

                CheckMilestones(user, photos.Count);

                Debug.Log($"📸 Photo taken for {user.DisplayName}: {caption}");
                return memoryPhoto;
            }

            return null;
        }

        /// <summary>
        /// Get all photos for a user
        /// </summary>
        public List<MemoryPhoto> GetUserPhotos(string userId)
        {
            if (userPhotos.TryGetValue(userId, out List<MemoryPhoto> photos))
            {
                return new List<MemoryPhoto>(photos); // Return copy
            }
            return new List<MemoryPhoto>();
        }

        /// <summary>
        /// Delete a photo
        /// </summary>
        public bool DeletePhoto(string userId, string photoId)
        {
            if (userPhotos.TryGetValue(userId, out List<MemoryPhoto> photos))
            {
                var photo = photos.Find(p => p.PhotoId == photoId);
                if (photo != null)
                {
                    photos.Remove(photo);
                    Debug.Log($"Photo {photoId} deleted");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Auto-capture milestone moments
        /// </summary>
        public void AutoCaptureMillestone(UserProfile user, MilestoneType milestone)
        {
            string caption = milestone switch
            {
                MilestoneType.FirstHomework => "🎉 First Homework Complete!",
                MilestoneType.TenHomework => "💯 10 Homework Achievements!",
                MilestoneType.HundredCoins => "💰 100 Coins Collected!",
                MilestoneType.MaxHappiness => "😊 Maximum Happiness Reached!",
                MilestoneType.FirstWeek => "📅 One Week Together!",
                MilestoneType.LevelUp => $"⭐ Level {GameUtilities.CalculateLevel(user.ExperiencePoints)} Reached!",
                _ => "📸 Milestone Achieved!"
            };

            TakePhoto(user, caption);
        }

        /// <summary>
        /// Check for photo collection milestones
        /// </summary>
        private void CheckMilestones(UserProfile user, int photoCount)
        {
            string milestone = photoCount switch
            {
                1 => "First Photo",
                10 => "Photo Collector",
                25 => "Memory Keeper",
                50 => "Scrapbook Master",
                _ => null
            };

            if (milestone != null)
            {
                OnMilestoneAchieved?.Invoke(milestone);
                Debug.Log($"🏆 Milestone: {milestone}");
            }
        }

        /// <summary>
        /// Export photo as shareable image (for social media, parent reports)
        /// </summary>
        public byte[] ExportPhoto(string userId, string photoId)
        {
            if (!enablePhotoSharing) return null;

            if (userPhotos.TryGetValue(userId, out List<MemoryPhoto> photos))
            {
                var photo = photos.Find(p => p.PhotoId == photoId);
                if (photo != null)
                {
                    return photo.PhotoData;
                }
            }
            return null;
        }

        /// <summary>
        /// Create digital scrapbook summary
        /// </summary>
        public ScrapbookSummary CreateScrapbook(string userId, DateTime startDate, DateTime endDate)
        {
            if (!userPhotos.TryGetValue(userId, out List<MemoryPhoto> allPhotos))
            {
                return new ScrapbookSummary();
            }

            var filteredPhotos = allPhotos.FindAll(p => p.Timestamp >= startDate && p.Timestamp <= endDate);

            return new ScrapbookSummary
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                TotalPhotos = filteredPhotos.Count,
                Photos = filteredPhotos,
                HighestHappiness = GetHighestHappiness(filteredPhotos),
                HomeworkProgress = GetHomeworkProgress(filteredPhotos)
            };
        }

        private float GetHighestHappiness(List<MemoryPhoto> photos)
        {
            float max = 0f;
            foreach (var photo in photos)
            {
                if (photo.CharacterHappiness > max)
                    max = photo.CharacterHappiness;
            }
            return max;
        }

        private int GetHomeworkProgress(List<MemoryPhoto> photos)
        {
            if (photos.Count == 0) return 0;
            return photos[photos.Count - 1].HomeworkCompleted - photos[0].HomeworkCompleted;
        }
    }

    /// <summary>
    /// Memory photo with metadata
    /// </summary>
    [Serializable]
    public class MemoryPhoto
    {
        public string PhotoId;
        public string UserId;
        public string Caption;
        public DateTime Timestamp;
        public float CharacterHappiness;
        public int HomeworkCompleted;
        public byte[] PhotoData;
    }

    /// <summary>
    /// Digital scrapbook summary for parent reports
    /// </summary>
    [Serializable]
    public class ScrapbookSummary
    {
        public string UserId;
        public DateTime StartDate;
        public DateTime EndDate;
        public int TotalPhotos;
        public List<MemoryPhoto> Photos = new List<MemoryPhoto>();
        public float HighestHappiness;
        public int HomeworkProgress;
    }

    /// <summary>
    /// Auto-capture milestone types
    /// </summary>
    public enum MilestoneType
    {
        FirstHomework,
        TenHomework,
        HundredCoins,
        MaxHappiness,
        FirstWeek,
        LevelUp
    }
}
