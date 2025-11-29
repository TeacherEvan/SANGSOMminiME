using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Main game UI for character interaction and customization
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("User Info")]
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private Slider happinessSlider;
        [SerializeField] private TextMeshProUGUI happinessText;

        [Header("Character Controls")]
        [SerializeField] private Button danceButton;
        [SerializeField] private Button waveButton;
        [SerializeField] private Button waiButton;
        [SerializeField] private Button curtsyButton;
        [SerializeField] private Button bowButton;

        [Header("Customization")]
        [SerializeField] private Slider eyeScaleSlider;
        [SerializeField] private Button prevOutfitButton;
        [SerializeField] private Button nextOutfitButton;
        [SerializeField] private Button prevAccessoryButton;
        [SerializeField] private Button nextAccessoryButton;
        [SerializeField] private TextMeshProUGUI outfitText;
        [SerializeField] private TextMeshProUGUI accessoryText;

        [Header("Homework System")]
        [SerializeField] private Button completeHomeworkButton;
        [SerializeField] private TextMeshProUGUI homeworkCountText;
        [SerializeField] private Button homeworkRewardButton;

        [Header("Account")]
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button saveProgressButton;

        private Character.CharacterController characterController;
        private Core.UserProfile currentUser;
        private int currentOutfitIndex = 0;
        private int currentAccessoryIndex = 0;
        private int maxOutfits = 3; // Will be determined by available materials
        private int maxAccessories = 3; // Will be determined by available accessories

        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
            FindCharacterController();

            // Subscribe to user events
            if (Core.UserManager.Instance != null)
            {
                Core.UserManager.Instance.OnUserLoggedIn += OnUserLoggedIn;
                Core.UserManager.Instance.OnUserLoggedOut += OnUserLoggedOut;

                // Check if user is already logged in
                if (Core.UserManager.Instance.CurrentUser != null)
                {
                    OnUserLoggedIn(Core.UserManager.Instance.CurrentUser);
                }
            }
        }

        private void InitializeUI()
        {
            // Setup happiness slider
            if (happinessSlider != null)
            {
                happinessSlider.minValue = 0f;
                happinessSlider.maxValue = 100f;
                happinessSlider.interactable = false;
            }

            // Setup eye scale slider
            if (eyeScaleSlider != null)
            {
                eyeScaleSlider.minValue = 0.5f;
                eyeScaleSlider.maxValue = 2.0f;
                eyeScaleSlider.value = 1.0f;
            }

            // Initially hide UI if no user is logged in
            if (Core.UserManager.Instance?.CurrentUser == null)
            {
                gameObject.SetActive(false);
            }
        }

        private void SetupEventListeners()
        {
            // Character animation buttons
            if (danceButton != null)
                danceButton.onClick.AddListener(() => characterController?.PlayDance());
            if (waveButton != null)
                waveButton.onClick.AddListener(() => characterController?.PlayWave());
            if (waiButton != null)
                waiButton.onClick.AddListener(() => characterController?.PlayWai());
            if (curtsyButton != null)
                curtsyButton.onClick.AddListener(() => characterController?.PlayCurtsy());
            if (bowButton != null)
                bowButton.onClick.AddListener(() => characterController?.PlayBow());

            // Customization controls
            if (eyeScaleSlider != null)
                eyeScaleSlider.onValueChanged.AddListener(OnEyeScaleChanged);
            if (prevOutfitButton != null)
                prevOutfitButton.onClick.AddListener(OnPrevOutfitClicked);
            if (nextOutfitButton != null)
                nextOutfitButton.onClick.AddListener(OnNextOutfitClicked);
            if (prevAccessoryButton != null)
                prevAccessoryButton.onClick.AddListener(OnPrevAccessoryClicked);
            if (nextAccessoryButton != null)
                nextAccessoryButton.onClick.AddListener(OnNextAccessoryClicked);

            // Homework system
            if (completeHomeworkButton != null)
                completeHomeworkButton.onClick.AddListener(OnCompleteHomeworkClicked);
            if (homeworkRewardButton != null)
                homeworkRewardButton.onClick.AddListener(OnHomeworkRewardClicked);

            // Account buttons
            if (logoutButton != null)
                logoutButton.onClick.AddListener(OnLogoutClicked);
            if (saveProgressButton != null)
                saveProgressButton.onClick.AddListener(OnSaveProgressClicked);
        }

        private void FindCharacterController()
        {
            characterController = FindObjectOfType<Character.CharacterController>();
            if (characterController != null)
            {
                characterController.OnHappinessChanged += OnCharacterHappinessChanged;
            }
        }

        private void OnUserLoggedIn(Core.UserProfile user)
        {
            // Unsubscribe from previous user if any
            if (currentUser != null)
            {
                UnsubscribeFromUserEvents();
            }

            currentUser = user;
            gameObject.SetActive(true);

            SubscribeToUserEvents();
            UpdateUserInfoDisplay();
            UpdateCustomizationFromUser();

            Debug.Log($"Game UI updated for user: {user.DisplayName}");
        }

        private void OnUserLoggedOut()
        {
            UnsubscribeFromUserEvents();
            currentUser = null;
            gameObject.SetActive(false);
        }

        private void SubscribeToUserEvents()
        {
            if (currentUser != null)
            {
                currentUser.OnCoinsChanged += UpdateCoinsDisplay;
                currentUser.OnExperienceChanged += UpdateExperienceDisplay;
                currentUser.OnHappinessChanged += OnCharacterHappinessChanged; // Reusing existing handler logic
            }
        }

        private void UnsubscribeFromUserEvents()
        {
            if (currentUser != null)
            {
                currentUser.OnCoinsChanged -= UpdateCoinsDisplay;
                currentUser.OnExperienceChanged -= UpdateExperienceDisplay;
                currentUser.OnHappinessChanged -= OnCharacterHappinessChanged;
            }
        }

        private void UpdateCoinsDisplay(int coins)
        {
            if (coinsText != null)
                coinsText.text = $"Coins: {coins}";
        }

        private void UpdateExperienceDisplay(int experience)
        {
            if (experienceText != null)
            {
                int level = experience / Core.GameConstants.ExperiencePerLevel + 1;
                int expInLevel = experience % Core.GameConstants.ExperiencePerLevel;
                experienceText.text = $"Level {level} ({expInLevel}/{Core.GameConstants.ExperiencePerLevel} XP)";
            }
        }

        private void UpdateUserInfoDisplay()
        {
            if (currentUser == null) return;

            if (userNameText != null)
                userNameText.text = currentUser.DisplayName;

            UpdateCoinsDisplay(currentUser.Coins);
            UpdateExperienceDisplay(currentUser.ExperiencePoints);

            if (happinessSlider != null)
            {
                happinessSlider.value = currentUser.CharacterHappiness;
            }

            if (happinessText != null)
                happinessText.text = $"Happiness: {currentUser.CharacterHappiness:F0}%";

            if (homeworkCountText != null)
                homeworkCountText.text = $"Homework Completed: {currentUser.HomeworkCompleted}";
        }

        private void UpdateCustomizationFromUser()
        {
            if (currentUser == null) return;

            // Update eye scale slider
            if (eyeScaleSlider != null)
            {
                eyeScaleSlider.value = currentUser.EyeScale;
            }

            // Update outfit display
            UpdateOutfitDisplay();
            UpdateAccessoryDisplay();
        }

        private void OnCharacterHappinessChanged(float happiness)
        {
            if (happinessSlider != null)
                happinessSlider.value = happiness;

            if (happinessText != null)
                happinessText.text = $"Happiness: {happiness:F0}%";
        }

        private void OnEyeScaleChanged(float scale)
        {
            characterController?.SetEyeScale(scale);
        }

        private void OnPrevOutfitClicked()
        {
            currentOutfitIndex = (currentOutfitIndex - 1 + maxOutfits) % maxOutfits;
            characterController?.SetOutfit(currentOutfitIndex);
            UpdateOutfitDisplay();
        }

        private void OnNextOutfitClicked()
        {
            currentOutfitIndex = (currentOutfitIndex + 1) % maxOutfits;
            characterController?.SetOutfit(currentOutfitIndex);
            UpdateOutfitDisplay();
        }

        private void OnPrevAccessoryClicked()
        {
            currentAccessoryIndex = (currentAccessoryIndex - 1 + maxAccessories) % maxAccessories;
            characterController?.SetAccessory(currentAccessoryIndex);
            UpdateAccessoryDisplay();
        }

        private void OnNextAccessoryClicked()
        {
            currentAccessoryIndex = (currentAccessoryIndex + 1) % maxAccessories;
            characterController?.SetAccessory(currentAccessoryIndex);
            UpdateAccessoryDisplay();
        }

        private void UpdateOutfitDisplay()
        {
            if (outfitText != null)
            {
                string outfitName = currentOutfitIndex == 0 ? "Default" : $"Outfit {currentOutfitIndex}";
                outfitText.text = outfitName;
            }
        }

        private void UpdateAccessoryDisplay()
        {
            if (accessoryText != null)
            {
                string accessoryName = currentAccessoryIndex == 0 ? "None" : $"Accessory {currentAccessoryIndex}";
                accessoryText.text = accessoryName;
            }
        }

        private void OnCompleteHomeworkClicked()
        {
            if (currentUser != null && Core.UserManager.Instance != null)
            {
                currentUser.CompleteHomework();
                characterController?.IncreaseHappiness(10f);

                // Show reward animation or effect
                if (characterController != null && !characterController.IsAnimating)
                {
                    characterController.PlayDance();
                }

                UpdateUserInfoDisplay();
                Core.UserManager.Instance.SaveCurrentUser();

                Debug.Log("Homework completed! Character is happy!");
            }
        }

        private void OnHomeworkRewardClicked()
        {
            // Give bonus reward for completing homework
            if (currentUser != null && Core.UserManager.Instance != null)
            {
                currentUser.AddCoins(10);
                currentUser.AddExperience(5);
                characterController?.IncreaseHappiness(5f);

                UpdateUserInfoDisplay();
                Core.UserManager.Instance.SaveCurrentUser();

                Debug.Log("Homework reward claimed!");
            }
        }

        private void OnLogoutClicked()
        {
            if (Core.UserManager.Instance != null)
            {
                Core.UserManager.Instance.LogoutUser();
            }
        }

        private void OnSaveProgressClicked()
        {
            if (Core.UserManager.Instance != null)
            {
                Core.UserManager.Instance.SaveCurrentUser();

                // Show save confirmation (you could add a popup here)
                Debug.Log("Progress saved!");
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (Core.UserManager.Instance != null)
            {
                Core.UserManager.Instance.OnUserLoggedIn -= OnUserLoggedIn;
                Core.UserManager.Instance.OnUserLoggedOut -= OnUserLoggedOut;
            }

            UnsubscribeFromUserEvents();

            if (characterController != null)
            {
                characterController.OnHappinessChanged -= OnCharacterHappinessChanged;
            }
        }
    }
}