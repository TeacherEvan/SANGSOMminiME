using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Main game manager that coordinates all systems
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private UI.LoginUI loginUI;
        [SerializeField] private UI.GameUI gameUI;
        [SerializeField] private Character.CharacterController characterController;
        
        [Header("Game Settings")]
        [SerializeField] private bool autoSaveEnabled = true;
        [SerializeField] private float autoSaveInterval = 30f; // seconds
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugMode = false;
        
        public static GameManager Instance { get; private set; }
        
        private float lastAutoSaveTime;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeGame()
        {
            Debug.Log("Sangsom Mini-Me Game Manager Initialized");
            
            // Subscribe to user events
            if (UserManager.Instance != null)
            {
                UserManager.Instance.OnUserLoggedIn += OnUserLoggedIn;
                UserManager.Instance.OnUserLoggedOut += OnUserLoggedOut;
            }
            
            // Initialize auto-save
            lastAutoSaveTime = Time.time;
        }
        
        private void Start()
        {
            // Ensure UI is properly set up
            SetupInitialUI();
        }
        
        private void SetupInitialUI()
        {
            // Check if we have any existing users
            if (UserManager.Instance != null)
            {
                var users = UserManager.Instance.AllUsers;
                if (users.Count == 0)
                {
                    // No users exist, show login UI for registration
                    if (loginUI != null) loginUI.gameObject.SetActive(true);
                    if (gameUI != null) gameUI.gameObject.SetActive(false);
                }
                else if (UserManager.Instance.CurrentUser == null)
                {
                    // Users exist but none logged in, show user selection
                    if (loginUI != null) loginUI.gameObject.SetActive(true);
                    if (gameUI != null) gameUI.gameObject.SetActive(false);
                }
                else
                {
                    // User already logged in, show game UI
                    if (loginUI != null) loginUI.gameObject.SetActive(false);
                    if (gameUI != null) gameUI.gameObject.SetActive(true);
                }
            }
        }
        
        private void OnUserLoggedIn(UserProfile user)
        {
            Debug.Log($"Game Manager: User logged in - {user.DisplayName}");
            
            // Hide login UI and show game UI
            if (loginUI != null) loginUI.gameObject.SetActive(false);
            if (gameUI != null) gameUI.gameObject.SetActive(true);
            
            // Initialize character for the user
            if (characterController != null)
            {
                // Character controller will automatically apply user customization
                Debug.Log("Character initialized for user");
            }
        }
        
        private void OnUserLoggedOut()
        {
            Debug.Log("Game Manager: User logged out");
            
            // Show login UI and hide game UI
            if (loginUI != null) 
            {
                loginUI.gameObject.SetActive(true);
                loginUI.ShowLoginInterface();
            }
            if (gameUI != null) gameUI.gameObject.SetActive(false);
        }
        
        private void Update()
        {
            // Handle auto-save
            if (autoSaveEnabled && UserManager.Instance?.CurrentUser != null)
            {
                if (Time.time - lastAutoSaveTime >= autoSaveInterval)
                {
                    UserManager.Instance.SaveCurrentUser();
                    lastAutoSaveTime = Time.time;
                    
                    if (enableDebugMode)
                        Debug.Log("Auto-save completed");
                }
            }
            
            // Handle debug input
            if (enableDebugMode)
            {
                HandleDebugInput();
            }
        }
        
        private void HandleDebugInput()
        {
            // Debug keys for testing
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // Add coins
                if (UserManager.Instance?.CurrentUser != null)
                {
                    UserManager.Instance.CurrentUser.AddCoins(100);
                    Debug.Log("Added 100 coins (Debug)");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                // Add experience
                if (UserManager.Instance?.CurrentUser != null)
                {
                    UserManager.Instance.CurrentUser.AddExperience(50);
                    Debug.Log("Added 50 experience (Debug)");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.F3))
            {
                // Complete homework
                if (UserManager.Instance?.CurrentUser != null)
                {
                    UserManager.Instance.CurrentUser.CompleteHomework();
                    characterController?.IncreaseHappiness(10f);
                    Debug.Log("Completed homework (Debug)");
                }
            }
            
            if (Input.GetKeyDown(KeyCode.F4))
            {
                // Test character animations
                if (characterController != null && !characterController.IsAnimating)
                {
                    int randomAnim = Random.Range(0, 5);
                    switch (randomAnim)
                    {
                        case 0: characterController.PlayDance(); break;
                        case 1: characterController.PlayWave(); break;
                        case 2: characterController.PlayWai(); break;
                        case 3: characterController.PlayCurtsy(); break;
                        case 4: characterController.PlayBow(); break;
                    }
                    Debug.Log("Played random animation (Debug)");
                }
            }
        }
        
        // Public methods for external access
        public void CreateSampleUser()
        {
            if (UserManager.Instance != null)
            {
                var sampleUser = UserManager.Instance.CreateUser("sample_user", "Sample Student");
                if (sampleUser != null)
                {
                    // Give sample user some starting resources
                    sampleUser.AddCoins(200);
                    sampleUser.AddExperience(50);
                    sampleUser.IncreaseHappiness(25f);
                    
                    UserManager.Instance.SaveCurrentUser();
                    Debug.Log("Sample user created with starting resources");
                }
            }
        }
        
        public void ToggleDebugMode()
        {
            enableDebugMode = !enableDebugMode;
            Debug.Log($"Debug mode: {(enableDebugMode ? "Enabled" : "Disabled")}");
        }
        
        public void ForceAutoSave()
        {
            if (UserManager.Instance?.CurrentUser != null)
            {
                UserManager.Instance.SaveCurrentUser();
                Debug.Log("Manual save completed");
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && UserManager.Instance?.CurrentUser != null)
            {
                UserManager.Instance.SaveCurrentUser();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && UserManager.Instance?.CurrentUser != null)
            {
                UserManager.Instance.SaveCurrentUser();
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (UserManager.Instance != null)
            {
                UserManager.Instance.OnUserLoggedIn -= OnUserLoggedIn;
                UserManager.Instance.OnUserLoggedOut -= OnUserLoggedOut;
            }
        }
    }
}