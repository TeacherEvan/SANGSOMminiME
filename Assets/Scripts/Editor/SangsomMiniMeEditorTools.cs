using UnityEngine;
using UnityEditor;
using SangsomMiniMe.Core;
using System.IO;

namespace SangsomMiniMe.Editor
{
    /// <summary>
    /// Editor tools for Sangsom Mini-Me development
    /// </summary>
    public class SangsomMiniMeEditorTools : EditorWindow
    {
        private string newUsername = "";
        private string newDisplayName = "";
        private int coinsToAdd = 100;
        private int expToAdd = 50;
        
        [MenuItem("Sangsom Mini-Me/Developer Tools")]
        public static void ShowWindow()
        {
            GetWindow<SangsomMiniMeEditorTools>("Mini-Me Dev Tools");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("Sangsom Mini-Me Developer Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to use most features.", MessageType.Info);
            }
            
            DrawUserManagementSection();
            EditorGUILayout.Space();
            DrawResourcesSection();
            EditorGUILayout.Space();
            DrawDataManagementSection();
            EditorGUILayout.Space();
            DrawAssetCreationSection();
        }
        
        private void DrawUserManagementSection()
        {
            GUILayout.Label("User Management", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            newUsername = EditorGUILayout.TextField("Username", newUsername);
            newDisplayName = EditorGUILayout.TextField("Display Name", newDisplayName);
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            if (GUILayout.Button("Create Test User"))
            {
                CreateTestUser();
            }
            
            if (GUILayout.Button("List All Users"))
            {
                ListAllUsers();
            }
            
            if (GUILayout.Button("Delete All Users (Caution!)"))
            {
                if (EditorUtility.DisplayDialog("Confirm Delete", 
                    "Are you sure you want to delete all user data?", "Yes", "No"))
                {
                    DeleteAllUsers();
                }
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawResourcesSection()
        {
            GUILayout.Label("Resource Management", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            coinsToAdd = EditorGUILayout.IntField("Coins to Add", coinsToAdd);
            expToAdd = EditorGUILayout.IntField("Experience to Add", expToAdd);
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying || UserManager.Instance?.CurrentUser == null);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Coins"))
            {
                AddCoinsToCurrentUser(coinsToAdd);
            }
            if (GUILayout.Button("Add Experience"))
            {
                AddExperienceToCurrentUser(expToAdd);
            }
            EditorGUILayout.EndHorizontal();
            
            if (GUILayout.Button("Complete Homework"))
            {
                CompleteHomework();
            }
            
            if (GUILayout.Button("Max Happiness"))
            {
                SetMaxHappiness();
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawDataManagementSection()
        {
            GUILayout.Label("Data Management", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (GUILayout.Button("Open Save Folder"))
            {
                OpenSaveFolder();
            }
            
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            
            if (GUILayout.Button("Force Save"))
            {
                ForceSave();
            }
            
            if (GUILayout.Button("Show Current User Stats"))
            {
                ShowCurrentUserStats();
            }
            
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
        }
        
        private void DrawAssetCreationSection()
        {
            GUILayout.Label("Asset Creation", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            if (GUILayout.Button("Create Game Configuration Asset"))
            {
                CreateGameConfigurationAsset();
            }
            
            EditorGUILayout.EndVertical();
        }
        
        private void CreateTestUser()
        {
            if (UserManager.Instance == null)
            {
                Debug.LogError("UserManager not found. Make sure you're in Play Mode.");
                return;
            }
            
            string username = string.IsNullOrEmpty(newUsername) ? "test_user_" + Random.Range(1000, 9999) : newUsername;
            string displayName = string.IsNullOrEmpty(newDisplayName) ? "Test User" : newDisplayName;
            
            var user = UserManager.Instance.CreateUser(username, displayName);
            if (user != null)
            {
                Debug.Log($"Created test user: {displayName} ({username})");
                UserManager.Instance.LoginUser(username);
            }
        }
        
        private void ListAllUsers()
        {
            if (UserManager.Instance == null)
            {
                Debug.LogError("UserManager not found. Make sure you're in Play Mode.");
                return;
            }
            
            var users = UserManager.Instance.AllUsers;
            Debug.Log($"=== Total Users: {users.Count} ===");
            
            foreach (var user in users)
            {
                Debug.Log($"User: {user.DisplayName} ({user.UserName}) - Level {GameUtilities.CalculateLevel(user.ExperiencePoints)}, {user.Coins} coins");
            }
        }
        
        private void DeleteAllUsers()
        {
            string savePath = Path.Combine(Application.persistentDataPath, "userProfiles.json");
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Deleted all user data. Restart Play Mode to see changes.");
            }
        }
        
        private void AddCoinsToCurrentUser(int amount)
        {
            var user = UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                user.AddCoins(amount);
                UserManager.Instance.SaveCurrentUser();
                Debug.Log($"Added {amount} coins to {user.DisplayName}. Total: {user.Coins}");
            }
        }
        
        private void AddExperienceToCurrentUser(int amount)
        {
            var user = UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                int oldLevel = GameUtilities.CalculateLevel(user.ExperiencePoints);
                user.AddExperience(amount);
                int newLevel = GameUtilities.CalculateLevel(user.ExperiencePoints);
                UserManager.Instance.SaveCurrentUser();
                
                Debug.Log($"Added {amount} XP to {user.DisplayName}. Total: {user.ExperiencePoints}");
                if (newLevel > oldLevel)
                {
                    Debug.Log($"Level Up! Now Level {newLevel}");
                }
            }
        }
        
        private void CompleteHomework()
        {
            var user = UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                user.CompleteHomework();
                UserManager.Instance.SaveCurrentUser();
                Debug.Log($"Homework completed for {user.DisplayName}. Total: {user.HomeworkCompleted}");
            }
        }
        
        private void SetMaxHappiness()
        {
            var user = UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                user.IncreaseHappiness(100f);
                UserManager.Instance.SaveCurrentUser();
                Debug.Log($"Set max happiness for {user.DisplayName}");
            }
        }
        
        private void OpenSaveFolder()
        {
            string path = Application.persistentDataPath;
            EditorUtility.RevealInFinder(path);
        }
        
        private void ForceSave()
        {
            if (UserManager.Instance != null)
            {
                UserManager.Instance.SaveCurrentUser();
                Debug.Log("Force save completed.");
            }
        }
        
        private void ShowCurrentUserStats()
        {
            var user = UserManager.Instance?.CurrentUser;
            if (user != null)
            {
                Debug.Log("=== Current User Stats ===");
                Debug.Log($"Name: {user.DisplayName} ({user.UserName})");
                Debug.Log($"Level: {GameUtilities.CalculateLevel(user.ExperiencePoints)}");
                Debug.Log($"Experience: {user.ExperiencePoints}");
                Debug.Log($"Coins: {user.Coins}");
                Debug.Log($"Happiness: {user.CharacterHappiness:F1}%");
                Debug.Log($"Homework Completed: {user.HomeworkCompleted}");
                Debug.Log($"Days Active: {user.DaysActive}");
                Debug.Log($"Eye Scale: {user.EyeScale}");
                Debug.Log($"Current Outfit: {user.CurrentOutfit}");
                Debug.Log($"Current Accessory: {user.CurrentAccessory}");
            }
            else
            {
                Debug.Log("No user currently logged in.");
            }
        }
        
        private void CreateGameConfigurationAsset()
        {
            GameConfiguration asset = ScriptableObject.CreateInstance<GameConfiguration>();
            
            string path = "Assets/ScriptableObjects";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            }
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath(path + "/GameConfig.asset");
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            
            Debug.Log($"Created GameConfiguration asset at: {assetPath}");
        }
    }
}
