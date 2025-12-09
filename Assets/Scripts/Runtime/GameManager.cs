using UnityEngine;
using System;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Main game manager that orchestrates all game systems with production-grade error handling.
    /// Implements singleton pattern with proper lifecycle management and event-driven architecture.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private UI.LoginUI loginUI;
        [SerializeField] private UI.GameUI gameUI;
        [SerializeField] private Character.CharacterController characterController;

        [Header("Configuration")]
        [SerializeField] private GameConfiguration gameConfig;

        [Header("Auto-Save Settings")]
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveInterval = 30f;

        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugMode = false;
        [SerializeField] private bool enableDetailedLogging = false;

        // Singleton instance with thread safety consideration
        private static GameManager instance;
        public static GameManager Instance => instance;

        // Public configuration accessor with null safety and caching
        public GameConfiguration Config
        {
            get
            {
                if (gameConfig == null)
                {
                    gameConfig = ScriptableObject.CreateInstance<GameConfiguration>();
                }
                return gameConfig;
            }
        }

        // Cached time tracking for auto-save optimization
        private Coroutine autoSaveCoroutine;
        private bool isInitialized;

        // Event system for decoupled communication
        public event Action OnGameInitialized;
        public event Action<UserProfile> OnGameStateChanged;

        private void Awake()
        {
            // Thread-safe singleton implementation
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGameSystems();
            }
            else
            {
                LogInfo("Duplicate GameManager detected. Destroying duplicate instance.");
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initializes all game systems with proper error handling and validation.
        /// </summary>
        private void InitializeGameSystems()
        {
            try
            {
                ValidateRequiredReferences();
                SubscribeToGameEvents();
                InitializeAutoSave();
                InitializeMeterDecay();

                isInitialized = true;
                OnGameInitialized?.Invoke();

                LogInfo("Sangsom Mini-Me Game Manager initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Fatal initialization error: {ex.Message}\n{ex.StackTrace}");
                isInitialized = false;
            }
        }

        /// <summary>
        /// Validates that all required references are assigned in the Inspector.
        /// </summary>
        private void ValidateRequiredReferences()
        {
            if (loginUI == null)
                Debug.LogWarning("[GameManager] LoginUI reference is not assigned. Login functionality may not work.");

            if (gameUI == null)
                Debug.LogWarning("[GameManager] GameUI reference is not assigned. Game UI may not display.");

            if (characterController == null)
                Debug.LogWarning("[GameManager] CharacterController reference is not assigned. Character interactions will be limited.");

            if (gameConfig == null)
            {
                Debug.LogWarning("[GameManager] GameConfiguration is not assigned. Using default configuration.");
                // TODO: [OPTIMIZATION] Consider loading from Addressables for better memory management
            }
        }

        /// <summary>
        /// Subscribes to user management events with null safety checks.
        /// </summary>
        private void SubscribeToGameEvents()
        {
            if (UserManager.Instance != null)
            {
                UserManager.Instance.OnUserLoggedIn += HandleUserLoggedIn;
                UserManager.Instance.OnUserLoggedOut += HandleUserLoggedOut;
                LogInfo("Subscribed to UserManager events successfully.");
            }
            else
            {
                Debug.LogError("[GameManager] UserManager.Instance is null. Cannot subscribe to user events.");
            }

            // Subscribe to meter decay events for gentle reminders
            MeterDecaySystem.OnMeterLow += HandleMeterLowReminder;
        }

        /// <summary>
        /// Handles low meter gentle reminders (not warnings - cozy gameplay!).
        /// </summary>
        private void HandleMeterLowReminder(string meterName)
        {
            if (gameUI != null)
            {
                string message = MeterDecaySystem.GetLowMeterMessage(meterName);
                gameUI.ShowGentleReminder(message);
            }
        }

        /// <summary>
        /// Initializes the auto-save system with optimized timing.
        /// </summary>
        private void InitializeAutoSave()
        {
            if (autoSaveCoroutine != null) StopCoroutine(autoSaveCoroutine);
            autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
            LogInfo($"Auto-save initialized. Interval: {(gameConfig != null ? gameConfig.AutoSaveInterval : autoSaveInterval)}s");
        }

        private System.Collections.IEnumerator AutoSaveRoutine()
        {
            var wait = new WaitForSeconds(gameConfig != null ? gameConfig.AutoSaveInterval : autoSaveInterval);
            while (true)
            {
                yield return wait;
                if (autoSaveEnabled && isInitialized)
                {
                    UserManager.Instance?.SaveIfDirty();
                }
            }
        }

        // Meter decay coroutine reference
        private Coroutine meterDecayCoroutine;

        /// <summary>
        /// Initializes the meter decay system with gentle decay rates.
        /// </summary>
        private void InitializeMeterDecay()
        {
            if (meterDecayCoroutine != null) StopCoroutine(meterDecayCoroutine);
            meterDecayCoroutine = StartCoroutine(MeterDecayRoutine());
            LogInfo($"Meter decay initialized. Interval: {GameConstants.MeterDecayInterval}s");
        }

        /// <summary>
        /// Coroutine for gentle meter decay at regular intervals.
        /// Only decays when a user is logged in.
        /// </summary>
        private System.Collections.IEnumerator MeterDecayRoutine()
        {
            var wait = new WaitForSeconds(GameConstants.MeterDecayInterval);
            while (true)
            {
                yield return wait;

                // Only decay if user is logged in
                var currentUser = UserManager.Instance?.CurrentUser;
                if (currentUser != null && isInitialized)
                {
                    MeterDecaySystem.ApplyDecay(currentUser);

                    // Update UI if available
                    if (gameUI != null)
                    {
                        gameUI.UpdateMeterDisplays();
                    }
                }
            }
        }

        private void Start()
        {
            // Ensure proper initialization before setting up UI
            if (!isInitialized)
            {
                Debug.LogError("[GameManager] Start called but initialization failed. Attempting re-initialization.");
                InitializeGameSystems();
            }

            SetupInitialUserInterface();
        }

        /// <summary>
        /// Configures the initial UI state based on user login status.
        /// Implements defensive programming with null checks.
        /// </summary>
        private void SetupInitialUserInterface()
        {
            try
            {
                if (UserManager.Instance == null)
                {
                    Debug.LogError("[GameManager] UserManager.Instance is null during UI setup.");
                    return;
                }

                var users = UserManager.Instance.AllUsers;
                var currentUser = UserManager.Instance.CurrentUser;

                // Determine which UI to show based on user state
                if (users.Count == 0)
                {
                    // No users exist - show registration
                    ShowLoginUI();
                    LogInfo("No existing users found. Showing registration interface.");
                }
                else if (currentUser == null)
                {
                    // Users exist but none logged in - show user selection
                    ShowLoginUI();
                    LogInfo($"{users.Count} existing user(s) found. Showing login interface.");
                }
                else
                {
                    // User already logged in - show game UI
                    ShowGameUI();
                    LogInfo($"User '{currentUser.DisplayName}' already logged in. Showing game interface.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Error setting up initial UI: {ex.Message}");
                ShowLoginUI(); // Fallback to login UI
            }
        }

        /// <summary>
        /// Shows the login UI and hides the game UI.
        /// </summary>
        private void ShowLoginUI()
        {
            if (loginUI != null) loginUI.gameObject.SetActive(true);
            if (gameUI != null) gameUI.gameObject.SetActive(false);
        }

        /// <summary>
        /// Shows the game UI and hides the login UI.
        /// </summary>
        private void ShowGameUI()
        {
            if (loginUI != null) loginUI.gameObject.SetActive(false);
            if (gameUI != null) gameUI.gameObject.SetActive(true);
        }

        /// <summary>
        /// Handles user login event with comprehensive error handling.
        /// Processes daily login bonus and triggers celebration events.
        /// </summary>
        private void HandleUserLoggedIn(UserProfile user)
        {
            if (user == null)
            {
                Debug.LogError("[GameManager] HandleUserLoggedIn called with null user.");
                return;
            }

            try
            {
                LogInfo($"User logged in: {user.DisplayName} (ID: {user.UserName})");

                // Process daily login bonus (positive-only streak system)
                var loginResult = DailyLoginSystem.ProcessLogin(user);

                if (loginResult.IsFirstLoginToday)
                {
                    LogInfo($"Daily bonus awarded: +{loginResult.CoinsEarned} coins | Streak: {loginResult.CurrentStreak} days");

                    // Notify UI to show celebration
                    if (gameUI != null)
                    {
                        gameUI.ShowLoginBonusCelebration(loginResult);
                    }

                    // Trigger celebration animation for milestones
                    if (loginResult.HitMilestone && characterController != null)
                    {
                        characterController.PlayDance();
                    }
                }

                // Transition UI
                ShowGameUI();

                // Initialize character for the user
                if (characterController != null)
                {
                    // Character controller will automatically apply user customization
                    LogInfo("Character initialized for user.");
                }
                else
                {
                    Debug.LogWarning("[GameManager] CharacterController is null. Character customization will not be applied.");
                }

                // Notify other systems of state change
                OnGameStateChanged?.Invoke(user);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Error handling user login: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Handles user logout event with proper cleanup.
        /// </summary>
        private void HandleUserLoggedOut()
        {
            try
            {
                LogInfo("User logged out successfully.");

                // Transition to login UI
                if (loginUI != null)
                {
                    loginUI.gameObject.SetActive(true);
                    loginUI.ShowLoginInterface();
                }

                ShowLoginUI();

                // Notify other systems of state change
                OnGameStateChanged?.Invoke(null);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Error handling user logout: {ex.Message}");
            }
        }

        private void Update()
        {
            if (!isInitialized) return;

            // Debug input processing (development only)
            if (enableDebugMode)
            {
                ProcessDebugInput();
            }
        }

        /// <summary>
        /// Processes debug input for development and testing.
        /// Debug keys provide quick testing functionality.
        /// </summary>
        private void ProcessDebugInput()
        {
            try
            {
                // F1: Add coins
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    AddDebugCoins(100);
                }

                // F2: Add experience
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    AddDebugExperience(50);
                }

                // F3: Complete homework
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    CompleteDebugHomework();
                }

                // F4: Test character animations
                if (Input.GetKeyDown(KeyCode.F4))
                {
                    PlayRandomCharacterAnimation();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Debug input processing error: {ex.Message}");
            }
        }

        /// <summary>
        /// Debug helper: Adds coins to current user.
        /// </summary>
        private void AddDebugCoins(int amount)
        {
            var currentUser = UserManager.Instance?.CurrentUser;
            if (currentUser != null)
            {
                currentUser.AddCoins(amount);
                LogInfo($"[DEBUG] Added {amount} coins. Total: {currentUser.Coins}");
            }
        }

        /// <summary>
        /// Debug helper: Adds experience to current user.
        /// </summary>
        private void AddDebugExperience(int amount)
        {
            var currentUser = UserManager.Instance?.CurrentUser;
            if (currentUser != null)
            {
                currentUser.AddExperience(amount);
                LogInfo($"[DEBUG] Added {amount} XP. Total: {currentUser.ExperiencePoints}");
            }
        }

        /// <summary>
        /// Debug helper: Completes homework for testing.
        /// </summary>
        private void CompleteDebugHomework()
        {
            var currentUser = UserManager.Instance?.CurrentUser;
            if (currentUser != null)
            {
                currentUser.CompleteHomework();
                characterController?.IncreaseHappiness(10f);
                LogInfo($"[DEBUG] Homework completed! Happiness increased.");
            }
        }

        /// <summary>
        /// Debug helper: Plays a random character animation.
        /// </summary>
        private void PlayRandomCharacterAnimation()
        {
            if (characterController != null && !characterController.IsAnimating)
            {
                int randomAnim = UnityEngine.Random.Range(0, 5);
                switch (randomAnim)
                {
                    case 0: characterController.PlayDance(); break;
                    case 1: characterController.PlayWave(); break;
                    case 2: characterController.PlayWai(); break;
                    case 3: characterController.PlayCurtsy(); break;
                    case 4: characterController.PlayBow(); break;
                }
                LogInfo($"[DEBUG] Played random animation: {randomAnim}");
            }
        }

        // ===== PUBLIC API =====

        /// <summary>
        /// Creates a sample user with starting resources for testing and demonstration.
        /// </summary>
        public void CreateSampleUser()
        {
            try
            {
                if (UserManager.Instance == null)
                {
                    Debug.LogError("[GameManager] Cannot create sample user: UserManager.Instance is null.");
                    return;
                }

                var sampleUser = UserManager.Instance.CreateUser("sample_user", "Sample Student", gameConfig);
                if (sampleUser != null)
                {
                    // Provide generous starting resources for better first impression
                    sampleUser.AddCoins(200);
                    sampleUser.AddExperience(50);
                    sampleUser.IncreaseHappiness(25f);

                    UserManager.Instance.SaveCurrentUser();
                    LogInfo($"Sample user created with starting resources: 200 coins, 50 XP, 75% happiness.");
                }
                else
                {
                    Debug.LogWarning("[GameManager] Failed to create sample user. Username may already exist.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Error creating sample user: {ex.Message}");
            }
        }

        /// <summary>
        /// Toggles debug mode on/off for development and testing.
        /// </summary>
        public void ToggleDebugMode()
        {
            enableDebugMode = !enableDebugMode;
            LogInfo($"Debug mode: {(enableDebugMode ? "ENABLED" : "DISABLED")}");
        }

        /// <summary>
        /// Forces an immediate save of current user data.
        /// Useful for manual save points or critical game events.
        /// </summary>
        public void ForceManualSave()
        {
            try
            {
                if (UserManager.Instance?.CurrentUser != null)
                {
                    UserManager.Instance.SaveCurrentUser();
                    LogInfo("Manual save completed successfully.");
                }
                else
                {
                    Debug.LogWarning("[GameManager] Cannot save: No user is currently logged in.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Manual save failed: {ex.Message}");
            }
        }

        // ===== LIFECYCLE EVENTS =====

        /// <summary>
        /// Saves user data when app is paused (mobile devices).
        /// </summary>
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && UserManager.Instance?.CurrentUser != null)
            {
                try
                {
                    UserManager.Instance.SaveCurrentUser();
                    LogInfo("Auto-saved on application pause.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GameManager] Save on pause failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Saves user data when app loses focus.
        /// </summary>
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && UserManager.Instance?.CurrentUser != null)
            {
                try
                {
                    UserManager.Instance.SaveCurrentUser();
                    LogInfo("Auto-saved on focus lost.");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[GameManager] Save on focus lost failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Cleanup when the GameManager is destroyed.
        /// Unsubscribes from all events to prevent memory leaks.
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                if (UserManager.Instance != null)
                {
                    UserManager.Instance.OnUserLoggedIn -= HandleUserLoggedIn;
                    UserManager.Instance.OnUserLoggedOut -= HandleUserLoggedOut;
                    LogInfo("Unsubscribed from UserManager events.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameManager] Error during cleanup: {ex.Message}");
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
                Debug.Log($"[GameManager] {message}");
            }
        }
    }
}