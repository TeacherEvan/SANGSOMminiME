using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Manages user profiles and authentication for the Sangsom Mini-Me system
    /// </summary>
    public class UserManager : MonoBehaviour
    {
        [Header("User Management Settings")]
        [SerializeField] private bool enableDataPersistence = true;
        [SerializeField] private string saveFileName = "userProfiles.json";
        
        private List<UserProfile> userProfiles = new List<UserProfile>();
        private UserProfile currentUser;
        private string saveFilePath;
        
        public static UserManager Instance { get; private set; }
        
        public UserProfile CurrentUser => currentUser;
        public List<UserProfile> AllUsers => userProfiles.ToList(); // Return copy for safety
        
        // Events
        public System.Action<UserProfile> OnUserLoggedIn;
        public System.Action OnUserLoggedOut;
        public System.Action<UserProfile> OnUserCreated;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeUserManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeUserManager()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            LoadUserProfiles();
            
            Debug.Log($"UserManager initialized. Found {userProfiles.Count} user profiles.");
        }
        
        public UserProfile CreateUser(string userName, string displayName)
        {
            // Check if username already exists
            if (userProfiles.Any(u => u.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase)))
            {
                Debug.LogWarning($"Username '{userName}' already exists!");
                return null;
            }
            
            var newUser = new UserProfile(userName, displayName);
            userProfiles.Add(newUser);
            
            SaveUserProfiles();
            OnUserCreated?.Invoke(newUser);
            
            Debug.Log($"Created new user: {displayName} ({userName})");
            return newUser;
        }
        
        public bool LoginUser(string userName)
        {
            var user = userProfiles.FirstOrDefault(u => 
                u.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase) && u.IsActive);
            
            if (user != null)
            {
                currentUser = user;
                OnUserLoggedIn?.Invoke(currentUser);
                Debug.Log($"User logged in: {currentUser.DisplayName}");
                return true;
            }
            
            Debug.LogWarning($"Login failed for username: {userName}");
            return false;
        }
        
        public void LogoutUser()
        {
            if (currentUser != null)
            {
                Debug.Log($"User logged out: {currentUser.DisplayName}");
                currentUser = null;
                OnUserLoggedOut?.Invoke();
            }
        }
        
        public UserProfile GetUserByName(string userName)
        {
            return userProfiles.FirstOrDefault(u => 
                u.UserName.Equals(userName, System.StringComparison.OrdinalIgnoreCase));
        }
        
        public void DeleteUser(string userName)
        {
            var user = GetUserByName(userName);
            if (user != null)
            {
                userProfiles.Remove(user);
                if (currentUser == user)
                {
                    LogoutUser();
                }
                SaveUserProfiles();
                Debug.Log($"Deleted user: {userName}");
            }
        }
        
        public void SaveCurrentUser()
        {
            if (currentUser != null)
            {
                SaveUserProfiles();
            }
        }
        
        private void SaveUserProfiles()
        {
            if (!enableDataPersistence) return;
            
            try
            {
                var jsonData = JsonUtility.ToJson(new UserProfileCollection { profiles = userProfiles }, true);
                File.WriteAllText(saveFilePath, jsonData);
                Debug.Log($"User profiles saved to: {saveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save user profiles: {e.Message}");
            }
        }
        
        private void LoadUserProfiles()
        {
            if (!enableDataPersistence) return;
            
            try
            {
                if (File.Exists(saveFilePath))
                {
                    var jsonData = File.ReadAllText(saveFilePath);
                    var collection = JsonUtility.FromJson<UserProfileCollection>(jsonData);
                    userProfiles = collection.profiles ?? new List<UserProfile>();
                    Debug.Log($"Loaded {userProfiles.Count} user profiles from: {saveFilePath}");
                }
                else
                {
                    Debug.Log("No saved user profiles found. Starting fresh.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load user profiles: {e.Message}");
                userProfiles = new List<UserProfile>();
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveUserProfiles();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                SaveUserProfiles();
            }
        }
        
        private void OnDestroy()
        {
            SaveUserProfiles();
        }
    }
    
    [System.Serializable]
    public class UserProfileCollection
    {
        public List<UserProfile> profiles;
    }
}