using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Manages user profiles, authentication, and data persistence.
    /// Implements async I/O and O(1) user lookups via Dictionary cache.
    /// </summary>
    public class UserManager : MonoBehaviour
    {
        [Header("Persistence Settings")]
        [SerializeField] private bool enableDataPersistence = true;
        [SerializeField] private string saveFileName = "userProfiles.json";

        [Header("Performance Settings")]
        [SerializeField] private bool enableDetailedLogging = false;

        private List<UserProfile> userProfiles = new List<UserProfile>();
        private Dictionary<string, UserProfile> userProfileLookup = new Dictionary<string, UserProfile>(StringComparer.OrdinalIgnoreCase);
        private UserProfile currentUser;
        private string saveFilePath;
        private bool isDirty = false;
        private bool isSaving = false;

        private static UserManager instance;
        public static UserManager Instance => instance;

        public UserProfile CurrentUser => currentUser;
        public IReadOnlyList<UserProfile> AllUsers => userProfiles;

        public event Action<UserProfile> OnUserLoggedIn;
        public event Action OnUserLoggedOut;
        public event Action<UserProfile> OnUserCreated;
        public event Action<UserProfile> OnUserDeleted;
        public event Action OnDataSaved;
        public event Action OnDataLoaded;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            try
            {
                saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
                LoadUserProfilesFromDisk();
                RebuildUserLookup();
                LogInfo($"UserManager initialized. Found {userProfiles.Count} user(s).");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Init error: {ex.Message}");
                userProfiles = new List<UserProfile>();
                userProfileLookup.Clear();
            }
        }

        private void RebuildUserLookup()
        {
            userProfileLookup.Clear();
            foreach (var user in userProfiles)
            {
                if (!string.IsNullOrEmpty(user.UserName) && !userProfileLookup.ContainsKey(user.UserName))
                {
                    userProfileLookup[user.UserName] = user;
                }
            }
        }

        public UserProfile CreateUser(string userName, string displayName, GameConfiguration config = null)
        {
            // Enhanced validation using ValidationUtilities
            if (!ValidationUtilities.ValidateUsername(userName, out string usernameError))
            {
                ValidationUtilities.LogValidationError("CreateUser", usernameError);
                return null;
            }

            if (!ValidationUtilities.ValidateDisplayName(displayName, out string displayNameError))
            {
                ValidationUtilities.LogValidationError("CreateUser", displayNameError);
                return null;
            }

            // Check for duplicate username using lookup dictionary (O(1) performance)
            if (userProfileLookup.ContainsKey(userName))
            {
                Debug.LogWarning($"[UserManager] Username '{userName}' already exists. Please choose a different username.");
                return null;
            }

            // Validate configuration if provided
            if (config != null && !ValidationUtilities.ValidateGameConfiguration(config, out string[] configErrors))
            {
                Debug.LogWarning($"[UserManager] Invalid configuration provided: {string.Join(", ", configErrors)}");
                // Continue with default configuration
                config = null;
            }

            try
            {
                // Create new user profile
                var newUser = new UserProfile(userName, displayName, config);
                userProfiles.Add(newUser);
                userProfileLookup[userName] = newUser;

                // Persist asynchronously
                SaveAsync();

                // Notify listeners
                OnUserCreated?.Invoke(newUser);

                LogInfo($"Successfully created new user: {displayName} (ID: {userName})");
                return newUser;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Error creating user: {ex.Message}");
                return null;
            }
        }

            var newUser = new UserProfile(userName, displayName, config);
            userProfiles.Add(newUser);
            userProfileLookup[userName] = newUser;

            SaveAsync();
            OnUserCreated?.Invoke(newUser);
            LogInfo($"Created user: {displayName}");
            return newUser;
        }

        public bool LoginUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;

            if (!userProfileLookup.TryGetValue(userName, out var user) || !user.IsActive)
            {
                Debug.LogWarning($"[UserManager] Login failed: User '{userName}' not found.");
                return false;
            }

            currentUser = user;
            OnUserLoggedIn?.Invoke(currentUser);
            LogInfo($"User logged in: {currentUser.DisplayName}");
            return true;
        }

        public void LogoutUser()
        {
            if (currentUser == null) return;

            var name = currentUser.DisplayName;
            if (isDirty) SaveAsync();

            currentUser = null;
            OnUserLoggedOut?.Invoke();
            LogInfo($"User logged out: {name}");
        }

        public UserProfile GetUserByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            userProfileLookup.TryGetValue(userName, out var user);
            return user;
        }

        public bool DeleteUser(string userName)
        {
            var user = GetUserByName(userName);
            if (user == null) return false;

            if (currentUser == user) LogoutUser();

            userProfiles.Remove(user);
            userProfileLookup.Remove(userName);
            SaveAsync();
            OnUserDeleted?.Invoke(user);
            LogInfo($"User deleted: {userName}");
            return true;
        }

        public void MarkDirty() => isDirty = true;

        public void SaveCurrentUser()
        {
            if (currentUser != null)
            {
                SaveAsync();
            }
        }

        public void SaveIfDirty()
        {
            if (isDirty && !isSaving)
            {
                SaveAsync();
            }
        }

        private async void SaveAsync()
        {
            if (isSaving || !enableDataPersistence) return;
            isSaving = true;

            try
            {
                string json = JsonUtility.ToJson(new UserProfileCollection { profiles = userProfiles }, true);
                await Task.Run(() => File.WriteAllText(saveFilePath, json));
                isDirty = false;
                OnDataSaved?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Save error: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        private void LoadUserProfilesFromDisk()
        {
            if (!enableDataPersistence || !File.Exists(saveFilePath))
            {
                userProfiles = new List<UserProfile>();
                return;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                var collection = JsonUtility.FromJson<UserProfileCollection>(json);
                userProfiles = collection?.profiles ?? new List<UserProfile>();
                OnDataLoaded?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Load error: {ex.Message}");
                userProfiles = new List<UserProfile>();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && currentUser != null && isDirty) SaveAsync();
        }

        private void OnApplicationQuit()
        {
            if (currentUser != null && isDirty)
            {
                // Synchronous save on quit to ensure data is written
                try
                {
                    string json = JsonUtility.ToJson(new UserProfileCollection { profiles = userProfiles }, true);
                    File.WriteAllText(saveFilePath, json);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[UserManager] Quit save error: {ex.Message}");
                }
            }
        }

        private void LogInfo(string message)
        {
            if (enableDetailedLogging)
                Debug.Log($"[UserManager] {message}");
        }
    }

    [Serializable]
    public class UserProfileCollection
    {
        public List<UserProfile> profiles;
    }
}
