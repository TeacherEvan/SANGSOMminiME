using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Manages user profiles, authentication, and data persistence with production-grade error handling.
    /// Implements optimized save/load with dirty flag tracking and async I/O operations.
    /// </summary>
    public class UserManager : MonoBehaviour
    {
        [Header("Persistence Settings")]
        [SerializeField] private bool enableDataPersistence = true;
        [SerializeField] private string saveFileName = "userProfiles.json";
        [SerializeField] private bool createBackups = true;
        [SerializeField] private int maxBackupCount = 3;

        [Header("Performance Settings")]
        [SerializeField] private bool enableDetailedLogging = false;

        // User data storage
        private List<UserProfile> userProfiles = new List<UserProfile>();
        private Dictionary<string, UserProfile> userProfileLookup = new Dictionary<string, UserProfile>(StringComparer.OrdinalIgnoreCase);
        private UserProfile currentUser;
        private string saveFilePath;
        private bool isDirty = false;
        private bool isLoading = false;
        private bool isSaving = false;

        // Singleton with thread safety
        private static UserManager instance;
        public static UserManager Instance => instance;

        // Public accessors with defensive copies
        public UserProfile CurrentUser => currentUser;
        public IReadOnlyList<UserProfile> AllUsers => userProfiles;

        // Event system for decoupled communication
        public event Action<UserProfile> OnUserLoggedIn;
        public event Action OnUserLoggedOut;
        public event Action<UserProfile> OnUserCreated;
        public event Action<UserProfile> OnUserDeleted;
        public event Action OnDataSaved;
        public event Action OnDataLoaded;

        private void Awake()
        {
            // Thread-safe singleton implementation
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeUserManagerSystem();
            }
            else
            {
                LogInfo("Duplicate UserManager detected. Destroying duplicate instance.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initializes the user management system with proper error handling.
        /// </summary>
        private void InitializeUserManagerSystem()
        {
            try
            {
                // Set up file paths
                saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
                LogInfo($"Save file path: {saveFilePath}");

                // Load existing user profiles
                LoadUserProfilesFromDisk();

                // Rebuild lookup cache
                RebuildUserLookup();

                LogInfo($"UserManager initialized successfully. Found {userProfiles.Count} user profile(s).");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Fatal initialization error: {ex.Message}\n{ex.StackTrace}");
                userProfiles = new List<UserProfile>(); // Ensure we have a valid list
                userProfileLookup.Clear();
            }
        }

        private void RebuildUserLookup()
        {
            userProfileLookup.Clear();
            foreach (var user in userProfiles)
            {
                if (!userProfileLookup.ContainsKey(user.UserName))
                {
                    userProfileLookup.Add(user.UserName, user);
                }
            }
        }

        // ===== USER CREATION & AUTHENTICATION =====

        /// <summary>
        /// Creates a new user with validation and duplicate checking.
        /// </summary>
        /// <param name="userName">Unique username for the user</param>
        /// <param name="displayName">Display name shown in UI</param>
        /// <param name="config">Optional game configuration</param>
        /// <returns>The created UserProfile or null if creation failed</returns>
        public UserProfile CreateUser(string userName, string displayName, GameConfiguration config = null)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(userName))
                {
                    Debug.LogWarning("[UserManager] Create user failed: Username cannot be empty.");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(displayName))
                {
                    Debug.LogWarning("[UserManager] Create user failed: Display name cannot be empty.");
                    return null;
                }

                // Check for duplicate username
                if (userProfileLookup.ContainsKey(userName))
                {
                    Debug.LogWarning($"[UserManager] Username '{userName}' already exists. Please choose a different username.");
                    return null;
                }

                // Create new user profile
                var newUser = new UserProfile(userName, displayName, config);
                userProfiles.Add(newUser);
                userProfileLookup[userName] = newUser;

                // Persist immediately
                SaveUserProfilesToDiskAsync().WrapErrors();

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

        /// <summary>
        /// Logs in a user by username with comprehensive validation.
        /// </summary>
        /// <param name="userName">Username to log in</param>
        /// <returns>True if login successful, false otherwise</returns>
        public bool LoginUser(string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    Debug.LogWarning("[UserManager] Login failed: Empty username provided.");
                    return false;
                }

                // Find user with case-insensitive matching (O(1) lookup)
                if (!userProfileLookup.TryGetValue(userName, out var user) || !user.IsActive)
                {
                    Debug.LogWarning($"[UserManager] Login failed: User '{userName}' not found or inactive.");
                    return false;
                }

                // Set current user and notify
                currentUser = user;
                OnUserLoggedIn?.Invoke(currentUser);

                LogInfo($"User logged in successfully: {currentUser.DisplayName} (ID: {currentUser.UserName})");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Login error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Logs out the current user with proper cleanup.
        /// </summary>
        public void LogoutUser()
        {
            try
            {
                if (currentUser == null)
                {
                    Debug.LogWarning("[UserManager] Logout called but no user is logged in.");
                    return;
                }

                var userName = currentUser.DisplayName;

                // Save any pending changes before logout
                if (isDirty)
                {
                    SaveUserProfilesToDiskAsync().WrapErrors();
                }

                // Clear current user and notify
                currentUser = null;
                OnUserLoggedOut?.Invoke();

                LogInfo($"User logged out: {userName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Logout error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a user profile by username.
        /// </summary>
        /// <param name="userName">Username to search for</param>
        /// <returns>UserProfile if found, null otherwise</returns>
        public UserProfile GetUserByName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return null;

            userProfileLookup.TryGetValue(userName, out var user);
            return user;
        }

        /// <summary>
        /// Deletes a user with validation and cleanup.
        /// </summary>
        /// <param name="userName">Username of the user to delete</param>
        /// <returns>True if deletion successful, false otherwise</returns>
        public bool DeleteUser(string userName)
        {
            try
            {
                var user = GetUserByName(userName);
                if (user == null)
                {
                    Debug.LogWarning($"[UserManager] Cannot delete: User '{userName}' not found.");
                    return false;
                }

                // Logout if deleting current user
                if (currentUser == user)
                {
                    LogoutUser();
                }

                // Remove and persist
                userProfiles.Remove(user);
                userProfileLookup.Remove(userName);
                SaveUserProfilesToDiskAsync().WrapErrors();

                // Notify listeners
                OnUserDeleted?.Invoke(user);

                LogInfo($"User deleted: {userName}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Error deleting user: {ex.Message}");
                return false;
            }
        }

        // ===== PERSISTENCE & SAVE SYSTEM =====

        /// <summary>
        /// Marks data as dirty for optimized batch saving.
        /// Call this when user data changes to enable auto-save.
        /// </summary>
        public void MarkDirty()
        {
            isDirty = true;
        }

        /// <summary>
        /// Saves the current user's data immediately.
        /// </summary>
        public void SaveCurrentUser()
        {
            if (currentUser != null)
            {
                SaveUserProfilesToDiskAsync().WrapErrors();
                isDirty = false;
            }
            else
            {
                Debug.LogWarning("[UserManager] Cannot save: No user is currently logged in.");
            }
        }

        /// <summary>
        /// Saves only if data has changed (optimized for auto-save).
        /// Prevents unnecessary I/O operations.
        /// </summary>
        public void SaveIfDirty()
        {
            if (isDirty && !isSaving)
            {
                SaveUserProfilesToDiskAsync().WrapErrors();
            }
        }

        private async Task SaveUserProfilesToDiskAsync()
        {
            if (isSaving) return;
            isSaving = true;

            try
            {
                // Serialize on main thread (JsonUtility requirement)
                string json = JsonUtility.ToJson(new UserProfileCollection { profiles = userProfiles }, true);

                // Write to disk on background thread
                await Task.Run(() =>
                {
                    File.WriteAllText(saveFilePath, json);
                });

                isDirty = false;
                OnDataSaved?.Invoke();
                if (enableDetailedLogging) LogInfo("User profiles saved successfully (Async).");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Error saving user profiles: {ex.Message}");
            }
            finally
            {
                isSaving = false;
            }
        }

        // Legacy synchronous save for initialization/shutdown if needed
        private void SaveUserProfilesToDisk()
        {
            try
            {
                string json = JsonUtility.ToJson(new UserProfileCollection { profiles = userProfiles }, true);
                File.WriteAllText(saveFilePath, json);
                isDirty = false;
                OnDataSaved?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Error saving user profiles: {ex.Message}");
            }
        }

        private void LoadUserProfilesFromDisk()
        {
            if (!File.Exists(saveFilePath))
            {
                userProfiles = new List<UserProfile>();
                return;
            }

            try
            {
                string json = File.ReadAllText(saveFilePath);
                var collection = JsonUtility.FromJson<UserProfileCollection>(json);
                userProfiles = collection != null ? collection.profiles : new List<UserProfile>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Error loading user profiles: {ex.Message}");
                userProfiles = new List<UserProfile>();
            }
        }

        [Serializable]
        private class UserProfileCollection
        {
            public List<UserProfile> profiles;
        }

        private void LogInfo(string message)
        {
            if (enableDetailedLogging)
                Debug.Log($"[UserManager] {message}");
        }
    }

    // Extension method to fire-and-forget tasks safely
    public static class TaskExtensions
    {
        public static async void WrapErrors(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[UserManager] Async task error: {ex.Message}");
            }
        }
    }
}
if (isDirty && currentUser != null)
{
    SaveUserProfilesToDisk();
    if (!isDirty || currentUser == null)
        return;

    SaveUserProfilesToDisk();
    LogInfo("Dirty data saved successfully.", true);
        /// Saves all user profiles to disk with error handling and optional backup.
        /// </summary>
        private void SaveUserProfilesToDisk()
{
    if (!enableDataPersistence)
        public void SaveCurrentUser(bool force = false)
                LogInfo("Data persistence is disabled. Skipping save.", true);
    if (currentUser == null)
    {
        Debug.LogWarning("[UserManager] Cannot save: No user is currently logged in.");
        return;
    }

    if (!force && !isDirty)
    {
        LogInfo("Save skipped: No pending changes detected.", true);
        return;
    }

    SaveUserProfilesToDisk();
    isDirty = false;
    try
    {
        isSaving = true;

        // Create backup if enabled
        if (createBackups && File.Exists(saveFilePath))
        {
            CreateBackupFile();
        }

        // Serialize and save
        var collection = new UserProfileCollection { profiles = userProfiles };
        string jsonData = JsonUtility.ToJson(collection, true);

        // Write to disk
        File.WriteAllText(saveFilePath, jsonData);

        // Notify listeners
        OnDataSaved?.Invoke();

        LogInfo($"User profiles saved successfully. Total profiles: {userProfiles.Count}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Failed to save user profiles: {ex.Message}\n{ex.StackTrace}");
        // TODO: [OPTIMIZATION] Implement retry logic with exponential backoff for network storage
    }
    finally
    {
        isSaving = false;
    }
}

/// <summary>
/// Creates a backup of the current save file.

// Clear dirty flag after successful write
isDirty = false;
/// Maintains a rolling backup system with configurable max count.
/// </summary>
private void CreateBackupFile()
{
    try
    {
        string backupPath = saveFilePath + $".backup_{DateTime.Now:yyyyMMdd_HHmmss}";
        File.Copy(saveFilePath, backupPath, true);

        // Clean up old backups
        CleanupOldBackups();

        LogInfo($"Backup created: {Path.GetFileName(backupPath)}", true);
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"[UserManager] Failed to create backup: {ex.Message}");
    }
}

/// <summary>
/// Removes old backup files keeping only the most recent ones.
/// </summary>
private void CleanupOldBackups()
{
    try
    {
        var directory = Path.GetDirectoryName(saveFilePath);
        var backupFiles = Directory.GetFiles(directory, "*.backup_*")
            .OrderByDescending(f => File.GetCreationTime(f))
            .Skip(maxBackupCount)
            .ToList();

        foreach (var oldBackup in backupFiles)
        {
            File.Delete(oldBackup);
            LogInfo($"Deleted old backup: {Path.GetFileName(oldBackup)}", true);
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"[UserManager] Failed to cleanup old backups: {ex.Message}");
    }
}

/// <summary>
/// Loads user profiles from disk with error handling and recovery.
/// </summary>
private void LoadUserProfilesFromDisk()
{
    if (!enableDataPersistence)
    {
        LogInfo("Data persistence is disabled. Skipping load.", true);
        userProfiles = new List<UserProfile>();
        return;
    }

    if (isLoading)
    {
        Debug.LogWarning("[UserManager] Load already in progress.");
        return;
    }

    try
    {
        isLoading = true;

        if (!File.Exists(saveFilePath))
        {
            LogInfo("No saved user profiles found. Starting with empty user list.");
            userProfiles = new List<UserProfile>();
            return;
        }

        // Read and deserialize
        string jsonData = File.ReadAllText(saveFilePath);
        var collection = JsonUtility.FromJson<UserProfileCollection>(jsonData);

        userProfiles = collection?.profiles ?? new List<UserProfile>();

        // Notify listeners
        OnDataLoaded?.Invoke();

        LogInfo($"Successfully loaded {userProfiles.Count} user profile(s) from disk.");
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Failed to load user profiles: {ex.Message}");
        userProfiles = new List<UserProfile>();

        // TODO: [OPTIMIZATION] Attempt to load from backup if main save is corrupted
        TryLoadFromBackup();
    }
    finally
    {
        isLoading = false;
    }
}

/// <summary>
/// Attempts to recover data from the most recent backup file.
/// </summary>
private void TryLoadFromBackup()
{
    try
    {
        var directory = Path.GetDirectoryName(saveFilePath);
        var backupFiles = Directory.GetFiles(directory, "*.backup_*")
            .OrderByDescending(f => File.GetCreationTime(f))
            .ToList();

        if (backupFiles.Count == 0)
        {
            Debug.LogWarning("[UserManager] No backup files found for recovery.");
            return;
        }

        // Try most recent backup
        string backupFile = backupFiles[0];
        string jsonData = File.ReadAllText(backupFile);
        var collection = JsonUtility.FromJson<UserProfileCollection>(jsonData);

        userProfiles = collection?.profiles ?? new List<UserProfile>();

        Debug.LogWarning($"[UserManager] Recovered {userProfiles.Count} profile(s) from backup: {Path.GetFileName(backupFile)}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Backup recovery failed: {ex.Message}");
        userProfiles = new List<UserProfile>();
    }
}

// ===== LIFECYCLE EVENTS =====

/// <summary>
/// Saves data when application is paused (important for mobile).
/// </summary>
private void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus && currentUser != null)
    {
        try
        {
            SaveUserProfilesToDisk();
            LogInfo("Auto-saved on application pause.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UserManager] Save on pause failed: {ex.Message}");
        }
    }
}

/// <summary>
/// Saves data when application loses focus.
/// </summary>
private void OnApplicationFocus(bool hasFocus)
{
    if (!hasFocus && currentUser != null)
    {
        try
        {
            SaveUserProfilesToDisk();
            LogInfo("Auto-saved on focus lost.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[UserManager] Save on focus lost failed: {ex.Message}");
        }
    }
}

/// <summary>
/// Final cleanup and save before destruction.
/// </summary>
private void OnDestroy()
{
    try
    {
        if (currentUser != null)
        {
            SaveUserProfilesToDisk();
            LogInfo("Final save completed on destroy.");
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Save on destroy failed: {ex.Message}");
    }
}

// ===== ASYNC SAVE/LOAD FOR PERFORMANCE =====

/// <summary>
/// Asynchronously saves user profiles to disk using modern async/await patterns.
/// Prevents blocking the main thread during I/O operations.
/// Uses CancellationToken for proper async operation cancellation.
/// </summary>
/// <param name="cancellationToken">Token to cancel the async operation</param>
/// <returns>Task representing the async operation</returns>
public async Task SaveUserProfilesAsync(CancellationToken cancellationToken = default)
{
    if (!enableDataPersistence)
    {
        LogInfo("Data persistence is disabled. Skipping async save.", true);
        return;
    }

    if (isSaving)
    {
        Debug.LogWarning("[UserManager] Save already in progress. Skipping duplicate async save request.");
        return;
    }

    try
    {
        isSaving = true;

        // Create backup if enabled
        if (createBackups && File.Exists(saveFilePath))
        {
            await Task.Run(() => CreateBackupFile(), cancellationToken);
        }

        // Serialize on background thread
        var collection = new UserProfileCollection { profiles = userProfiles };
        string jsonData = await Task.Run(() => JsonUtility.ToJson(collection, true), cancellationToken);

        // Write to disk asynchronously
        await File.WriteAllTextAsync(saveFilePath, jsonData, cancellationToken);

        // Notify listeners on main thread
        OnDataSaved?.Invoke();

        LogInfo($"User profiles saved asynchronously. Total profiles: {userProfiles.Count}");
    }
    catch (OperationCanceledException)
    {
        Debug.LogWarning("[UserManager] Async save operation was cancelled.");
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Async save failed: {ex.Message}\n{ex.StackTrace}");
        // TODO: [OPTIMIZATION] Implement exponential backoff retry (max 3 retries, 1s/2s/4s intervals) for transient I/O failures
    }
    finally
    {
        isSaving = false;
    }
}

/// <summary>
/// Asynchronously loads user profiles from disk without blocking main thread.
/// Uses modern async/await patterns for better performance.
/// </summary>
/// <param name="cancellationToken">Token to cancel the async operation</param>
/// <returns>Task representing the async operation</returns>
public async Task LoadUserProfilesAsync(CancellationToken cancellationToken = default)
{
    if (!enableDataPersistence)
    {
        LogInfo("Data persistence is disabled. Skipping async load.", true);
        userProfiles = new List<UserProfile>();
        return;
    }

    if (isLoading)
    {
        Debug.LogWarning("[UserManager] Load already in progress.");
        return;
    }

    try
    {
        isLoading = true;

        if (!File.Exists(saveFilePath))
        {
            LogInfo("No saved user profiles found. Starting with empty user list.");
            userProfiles = new List<UserProfile>();
            return;
        }

        // Read from disk asynchronously
        string jsonData = await File.ReadAllTextAsync(saveFilePath, cancellationToken);

        // Deserialize on background thread
        var collection = await Task.Run(() => JsonUtility.FromJson<UserProfileCollection>(jsonData), cancellationToken);

        userProfiles = collection?.profiles ?? new List<UserProfile>();

        // Notify listeners on main thread
        OnDataLoaded?.Invoke();

        LogInfo($"Asynchronously loaded {userProfiles.Count} user profile(s) from disk.");
    }
    catch (OperationCanceledException)
    {
        Debug.LogWarning("[UserManager] Async load operation was cancelled.");
        userProfiles = new List<UserProfile>();
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Async load failed: {ex.Message}");
        userProfiles = new List<UserProfile>();

        // TODO: [OPTIMIZATION] Attempt async backup recovery
        await TryLoadFromBackupAsync(cancellationToken);
    }
    finally
    {
        isLoading = false;
    }
}

/// <summary>
/// Asynchronously attempts to recover data from backup.
/// </summary>
private async Task TryLoadFromBackupAsync(CancellationToken cancellationToken = default)
{
    try
    {
        var directory = Path.GetDirectoryName(saveFilePath);
        var backupFiles = await Task.Run(() =>
            Directory.GetFiles(directory, "*.backup_*")
                .OrderByDescending(f => File.GetCreationTime(f))
                .ToList(), cancellationToken);

        if (backupFiles.Count == 0)
        {
            Debug.LogWarning("[UserManager] No backup files found for async recovery.");
            return;
        }

        // Try most recent backup
        string backupFile = backupFiles[0];
        string jsonData = await File.ReadAllTextAsync(backupFile, cancellationToken);
        var collection = await Task.Run(() => JsonUtility.FromJson<UserProfileCollection>(jsonData), cancellationToken);

        userProfiles = collection?.profiles ?? new List<UserProfile>();

        Debug.LogWarning($"[UserManager] Async recovered {userProfiles.Count} profile(s) from backup: {Path.GetFileName(backupFile)}");
    }
    catch (Exception ex)
    {
        Debug.LogError($"[UserManager] Async backup recovery failed: {ex.Message}");
        userProfiles = new List<UserProfile>();
    }
}
        }

        // ===== UTILITY METHODS =====

        /// <summary>
        /// Centralized logging method with optional verbose mode.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="verboseOnly">If true, only logs when detailed logging is enabled</param>
        private void LogInfo(string message, bool verboseOnly = false)
{
    if (!verboseOnly || enableDetailedLogging)
    {
        Debug.Log($"[UserManager] {message}");
    }
}
    }

    /// <summary>
    /// Serializable container for user profile collection.
    /// Required for JsonUtility serialization.
    /// </summary>
    [Serializable]
public class UserProfileCollection
{
    public List<UserProfile> profiles;
}
}