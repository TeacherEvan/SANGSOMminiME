using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Simple test script to verify the user interaction system works
    /// </summary>
    public class SystemTester : MonoBehaviour
    {
        [Header("Test Controls")]
        [SerializeField] private bool autoCreateSampleUser = true;
        [SerializeField] private bool enableTestLogging = true;
        
        private void Start()
        {
            if (enableTestLogging)
            {
                Debug.Log("=== Sangsom Mini-Me System Test Started ===");
                TestSystemInitialization();
            }
            
            if (autoCreateSampleUser)
            {
                Invoke(nameof(CreateTestUser), 1f); // Delay to ensure all systems are ready
            }
        }
        
        private void TestSystemInitialization()
        {
            // Test UserManager
            if (UserManager.Instance != null)
            {
                Debug.Log("✅ UserManager initialized successfully");
                Debug.Log($"Found {UserManager.Instance.AllUsers.Count} existing users");
            }
            else
            {
                Debug.LogError("❌ UserManager not found!");
            }
            
            // Test GameManager
            if (GameManager.Instance != null)
            {
                Debug.Log("✅ GameManager initialized successfully");
            }
            else
            {
                Debug.LogError("❌ GameManager not found!");
            }
            
            // Test CharacterController
            var characterController = FindObjectOfType<Character.CharacterController>();
            if (characterController != null)
            {
                Debug.Log("✅ CharacterController found");
            }
            else
            {
                Debug.Log("⚠️ CharacterController not found (this is expected for initial setup)");
            }
        }
        
        private void CreateTestUser()
        {
            if (UserManager.Instance == null) return;
            
            // Only create if no users exist
            if (UserManager.Instance.AllUsers.Count == 0)
            {
                Debug.Log("Creating sample user for testing...");
                var testUser = UserManager.Instance.CreateUser("test_student", "Test Student");
                
                if (testUser != null)
                {
                    // Give the test user some starting resources
                    testUser.AddCoins(150);
                    testUser.AddExperience(25);
                    testUser.IncreaseHappiness(20f);
                    
                    // Auto-login the test user
                    UserManager.Instance.LoginUser("test_student");
                    
                    Debug.Log("✅ Sample user created and logged in:");
                    Debug.Log($"   Username: {testUser.UserName}");
                    Debug.Log($"   Display Name: {testUser.DisplayName}");
                    Debug.Log($"   Coins: {testUser.Coins}");
                    Debug.Log($"   Experience: {testUser.ExperiencePoints}");
                    Debug.Log($"   Happiness: {testUser.CharacterHappiness:F1}%");
                    
                    UserManager.Instance.SaveCurrentUser();
                }
                else
                {
                    Debug.LogError("❌ Failed to create sample user");
                }
            }
            else
            {
                Debug.Log($"Users already exist ({UserManager.Instance.AllUsers.Count} found). Skipping sample user creation.");
            }
        }
        
        private void Update()
        {
            // Test input for quick testing
            if (enableTestLogging && Input.GetKeyDown(KeyCode.T))
            {
                TestQuickActions();
            }
        }
        
        private void TestQuickActions()
        {
            Debug.Log("=== Quick Test Actions ===");
            
            if (UserManager.Instance?.CurrentUser != null)
            {
                var user = UserManager.Instance.CurrentUser;
                Debug.Log($"Current User: {user.DisplayName}");
                Debug.Log($"Coins: {user.Coins}, XP: {user.ExperiencePoints}, Happiness: {user.CharacterHappiness:F1}%");
                
                // Test character interaction
                var characterController = FindObjectOfType<Character.CharacterController>();
                if (characterController != null && !characterController.IsAnimating)
                {
                    characterController.PlayDance();
                    Debug.Log("Character is dancing!");
                }
            }
            else
            {
                Debug.Log("No user currently logged in");
            }
        }
        
        // Public method for testing from inspector or other scripts
        [ContextMenu("Run System Test")]
        public void RunSystemTest()
        {
            TestSystemInitialization();
        }
        
        [ContextMenu("Create Sample User")]
        public void CreateSampleUserManual()
        {
            CreateTestUser();
        }
        
        [ContextMenu("Test Homework Completion")]
        public void TestHomeworkCompletion()
        {
            if (UserManager.Instance?.CurrentUser != null)
            {
                var user = UserManager.Instance.CurrentUser;
                Debug.Log($"Before homework - Coins: {user.Coins}, XP: {user.ExperiencePoints}, Happiness: {user.CharacterHappiness:F1}%");
                
                user.CompleteHomework();
                UserManager.Instance.SaveCurrentUser();
                
                Debug.Log($"After homework - Coins: {user.Coins}, XP: {user.ExperiencePoints}, Happiness: {user.CharacterHappiness:F1}%");
                Debug.Log("Homework completed! Character should be happier now.");
            }
            else
            {
                Debug.LogWarning("No user logged in to test homework completion");
            }
        }
    }
}