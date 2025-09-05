using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Manages the user login and registration interface
    /// </summary>
    public class LoginUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject registerPanel;
        [SerializeField] private GameObject userSelectionPanel;
        
        [Header("Login UI")]
        [SerializeField] private TMP_InputField loginUsernameInput;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button showRegisterButton;
        [SerializeField] private TextMeshProUGUI loginStatusText;
        
        [Header("Register UI")]
        [SerializeField] private TMP_InputField registerUsernameInput;
        [SerializeField] private TMP_InputField registerDisplayNameInput;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button backToLoginButton;
        [SerializeField] private TextMeshProUGUI registerStatusText;
        
        [Header("User Selection UI")]
        [SerializeField] private Transform userListParent;
        [SerializeField] private GameObject userButtonPrefab;
        [SerializeField] private Button createNewUserButton;
        [SerializeField] private Button logoutButton;
        
        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
            CheckExistingUsers();
        }
        
        private void InitializeUI()
        {
            // Hide all panels initially
            if (loginPanel != null) loginPanel.SetActive(false);
            if (registerPanel != null) registerPanel.SetActive(false);
            if (userSelectionPanel != null) userSelectionPanel.SetActive(false);
            
            // Clear status texts
            if (loginStatusText != null) loginStatusText.text = "";
            if (registerStatusText != null) registerStatusText.text = "";
        }
        
        private void SetupEventListeners()
        {
            // Login panel
            if (loginButton != null)
                loginButton.onClick.AddListener(OnLoginButtonClicked);
            if (showRegisterButton != null)
                showRegisterButton.onClick.AddListener(ShowRegisterPanel);
            
            // Register panel
            if (registerButton != null)
                registerButton.onClick.AddListener(OnRegisterButtonClicked);
            if (backToLoginButton != null)
                backToLoginButton.onClick.AddListener(ShowLoginPanel);
            
            // User selection panel
            if (createNewUserButton != null)
                createNewUserButton.onClick.AddListener(ShowRegisterPanel);
            if (logoutButton != null)
                logoutButton.onClick.AddListener(OnLogoutButtonClicked);
            
            // Input field events
            if (loginUsernameInput != null)
                loginUsernameInput.onEndEdit.AddListener(OnLoginUsernameEnterPressed);
            if (registerDisplayNameInput != null)
                registerDisplayNameInput.onEndEdit.AddListener(OnRegisterDisplayNameEnterPressed);
        }
        
        private void CheckExistingUsers()
        {
            if (Core.UserManager.Instance != null)
            {
                var users = Core.UserManager.Instance.AllUsers;
                if (users.Count > 0)
                {
                    ShowUserSelectionPanel();
                }
                else
                {
                    ShowRegisterPanel();
                }
            }
            else
            {
                ShowLoginPanel();
            }
        }
        
        private void ShowLoginPanel()
        {
            if (loginPanel != null) loginPanel.SetActive(true);
            if (registerPanel != null) registerPanel.SetActive(false);
            if (userSelectionPanel != null) userSelectionPanel.SetActive(false);
            
            if (loginStatusText != null) loginStatusText.text = "";
            if (loginUsernameInput != null) 
            {
                loginUsernameInput.text = "";
                loginUsernameInput.Select();
            }
        }
        
        private void ShowRegisterPanel()
        {
            if (loginPanel != null) loginPanel.SetActive(false);
            if (registerPanel != null) registerPanel.SetActive(true);
            if (userSelectionPanel != null) userSelectionPanel.SetActive(false);
            
            if (registerStatusText != null) registerStatusText.text = "";
            if (registerUsernameInput != null) 
            {
                registerUsernameInput.text = "";
                registerUsernameInput.Select();
            }
            if (registerDisplayNameInput != null) registerDisplayNameInput.text = "";
        }
        
        private void ShowUserSelectionPanel()
        {
            if (loginPanel != null) loginPanel.SetActive(false);
            if (registerPanel != null) registerPanel.SetActive(false);
            if (userSelectionPanel != null) userSelectionPanel.SetActive(true);
            
            PopulateUserList();
        }
        
        private void PopulateUserList()
        {
            if (userListParent == null || userButtonPrefab == null || Core.UserManager.Instance == null)
                return;
            
            // Clear existing user buttons
            foreach (Transform child in userListParent)
            {
                if (child.gameObject != createNewUserButton?.gameObject && 
                    child.gameObject != logoutButton?.gameObject)
                {
                    Destroy(child.gameObject);
                }
            }
            
            // Create buttons for each user
            var users = Core.UserManager.Instance.AllUsers;
            foreach (var user in users)
            {
                GameObject userButtonObj = Instantiate(userButtonPrefab, userListParent);
                Button userButton = userButtonObj.GetComponent<Button>();
                TextMeshProUGUI userButtonText = userButtonObj.GetComponentInChildren<TextMeshProUGUI>();
                
                if (userButtonText != null)
                {
                    userButtonText.text = $"{user.DisplayName}\nLevel {user.ExperiencePoints / 100 + 1} • {user.Coins} coins";
                }
                
                if (userButton != null)
                {
                    string userName = user.UserName; // Capture for closure
                    userButton.onClick.AddListener(() => OnUserSelected(userName));
                }
            }
        }
        
        private void OnLoginUsernameEnterPressed(string value)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnLoginButtonClicked();
            }
        }
        
        private void OnRegisterDisplayNameEnterPressed(string value)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnRegisterButtonClicked();
            }
        }
        
        private void OnLoginButtonClicked()
        {
            if (loginUsernameInput == null || Core.UserManager.Instance == null)
                return;
            
            string username = loginUsernameInput.text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                SetLoginStatus("Please enter a username", Color.red);
                return;
            }
            
            bool success = Core.UserManager.Instance.LoginUser(username);
            if (success)
            {
                SetLoginStatus("Login successful!", Color.green);
                gameObject.SetActive(false); // Hide login UI
            }
            else
            {
                SetLoginStatus("User not found. Please check your username.", Color.red);
            }
        }
        
        private void OnRegisterButtonClicked()
        {
            if (registerUsernameInput == null || registerDisplayNameInput == null || Core.UserManager.Instance == null)
                return;
            
            string username = registerUsernameInput.text.Trim();
            string displayName = registerDisplayNameInput.text.Trim();
            
            if (string.IsNullOrEmpty(username))
            {
                SetRegisterStatus("Please enter a username", Color.red);
                return;
            }
            
            if (string.IsNullOrEmpty(displayName))
            {
                SetRegisterStatus("Please enter a display name", Color.red);
                return;
            }
            
            var newUser = Core.UserManager.Instance.CreateUser(username, displayName);
            if (newUser != null)
            {
                SetRegisterStatus("Account created successfully!", Color.green);
                
                // Auto-login the new user
                Core.UserManager.Instance.LoginUser(username);
                gameObject.SetActive(false); // Hide login UI
            }
            else
            {
                SetRegisterStatus("Username already exists. Please choose a different one.", Color.red);
            }
        }
        
        private void OnUserSelected(string userName)
        {
            if (Core.UserManager.Instance != null)
            {
                bool success = Core.UserManager.Instance.LoginUser(userName);
                if (success)
                {
                    gameObject.SetActive(false); // Hide login UI
                }
            }
        }
        
        private void OnLogoutButtonClicked()
        {
            if (Core.UserManager.Instance != null)
            {
                Core.UserManager.Instance.LogoutUser();
                CheckExistingUsers(); // Refresh the UI
            }
        }
        
        private void SetLoginStatus(string message, Color color)
        {
            if (loginStatusText != null)
            {
                loginStatusText.text = message;
                loginStatusText.color = color;
            }
        }
        
        private void SetRegisterStatus(string message, Color color)
        {
            if (registerStatusText != null)
            {
                registerStatusText.text = message;
                registerStatusText.color = color;
            }
        }
        
        // Called when user logs out from elsewhere
        public void ShowLoginInterface()
        {
            gameObject.SetActive(true);
            CheckExistingUsers();
        }
    }
}