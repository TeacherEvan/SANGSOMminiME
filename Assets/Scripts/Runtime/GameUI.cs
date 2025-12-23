using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Main game UI with enhanced UX, loading states, and smooth transitions.
    /// Implements optimistic updates and visual feedback for better user experience.
    /// </summary>
    public class GameUI : MonoBehaviour
    {
        [Header("User Info Display")]
        [SerializeField] private TextMeshProUGUI userNameText;
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private Slider happinessSlider;
        [SerializeField] private TextMeshProUGUI happinessText;

        [Header("Meter Displays (Hunger & Energy)")]
        [SerializeField] private Slider hungerSlider;
        [SerializeField] private TextMeshProUGUI hungerText;
        [SerializeField] private Slider energySlider;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI moodText;

        [Header("Character Interaction Buttons")]
        [SerializeField] private Button danceButton;
        [SerializeField] private Button waveButton;
        [SerializeField] private Button waiButton;
        [SerializeField] private Button curtsyButton;
        [SerializeField] private Button bowButton;

        [Header("Character Care Actions")]
        [SerializeField] private Button feedButton;
        [SerializeField] private Button restButton;
        [SerializeField] private Button playButton;
        [SerializeField] private TextMeshProUGUI feedCostText;
        [SerializeField] private TextMeshProUGUI restCostText;

        [Header("Customization Controls")]
        [SerializeField] private Slider eyeScaleSlider;
        [SerializeField] private Button prevOutfitButton;
        [SerializeField] private Button nextOutfitButton;
        [SerializeField] private Button prevAccessoryButton;
        [SerializeField] private Button nextAccessoryButton;
        [SerializeField] private TextMeshProUGUI outfitText;
        [SerializeField] private TextMeshProUGUI accessoryText;

        [Header("Homework & Rewards")]
        [SerializeField] private Button completeHomeworkButton;
        [SerializeField] private TextMeshProUGUI homeworkCountText;
        [SerializeField] private Button homeworkRewardButton;
        [SerializeField] private GameObject rewardNotification;

        [Header("Account Management")]
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button saveProgressButton;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private UILoadingState loadingState;
        [SerializeField] private TextMeshProUGUI feedbackText;
        [SerializeField] private float feedbackDisplayDuration = 2f;
        [SerializeField] private CanvasGroup mainCanvasGroup;

        [Header("Daily Login Celebration")]
        [SerializeField] private GameObject celebrationPanel;
        [SerializeField] private TextMeshProUGUI celebrationTitleText;
        [SerializeField] private TextMeshProUGUI celebrationMessageText;
        [SerializeField] private TextMeshProUGUI celebrationCoinsText;
        [SerializeField] private TextMeshProUGUI streakDisplayText;
        [SerializeField] private Button closeCelebrationButton;
        [SerializeField] private float celebrationDisplayDuration = 4f;

        [Header("Animation Settings")]
        [SerializeField] private float uiFadeInDuration = 0.3f;
        [SerializeField] private float buttonPressScale = 0.95f;
        [SerializeField] private float buttonPressDuration = 0.1f;

        // Cached references
        private Character.CharacterController characterController;
        private Core.UserProfile currentUser;

        // State tracking
        private int currentOutfitIndex = 0;
        private int currentAccessoryIndex = 0;
        private int maxOutfits = 3;
        private int maxAccessories = 3;
        private bool isProcessingAction = false;

        // Coroutine tracking
        private Coroutine feedbackCoroutine;
        private Coroutine fadeCoroutine;
        private Coroutine coinCountUpCoroutine;
        private Coroutine expCountUpCoroutine;

        // Animation state tracking for micro-interactions
        private int displayedCoins = 0;
        private int displayedExperience = 0;

        // Meter animation tracking (smooth fill for premium UX)
        private Coroutine happinessAnimCoroutine;
        private Coroutine hungerAnimCoroutine;
        private Coroutine energyAnimCoroutine;
        private float meterAnimationDuration = 0.4f;

        private void Start()
        {
            InitializeUIComponents();
            SetupEventListeners();
            EnsureHoverEffects();
            EnsureRewardEffects();
            FindAndCacheCharacterController();
            SubscribeToUserManagementEvents();
        }

        private void EnsureRewardEffects()
        {
            if (UIRewardEffects.Instance != null) return;

            var go = new GameObject("UIRewardEffects");
            go.AddComponent<UIRewardEffects>();
        }

        private void EnsureHoverEffects()
        {
            // Add hover micro-interactions without requiring manual scene wiring.
            // Keep it scale-only to avoid altering button colors unexpectedly.
            TryAddHoverEffect(danceButton);
            TryAddHoverEffect(waveButton);
            TryAddHoverEffect(waiButton);
            TryAddHoverEffect(curtsyButton);
            TryAddHoverEffect(bowButton);

            TryAddHoverEffect(feedButton);
            TryAddHoverEffect(restButton);
            TryAddHoverEffect(playButton);

            TryAddHoverEffect(prevOutfitButton);
            TryAddHoverEffect(nextOutfitButton);
            TryAddHoverEffect(prevAccessoryButton);
            TryAddHoverEffect(nextAccessoryButton);

            TryAddHoverEffect(completeHomeworkButton);
            TryAddHoverEffect(homeworkRewardButton);
            TryAddHoverEffect(logoutButton);
            TryAddHoverEffect(saveProgressButton);
            TryAddHoverEffect(closeCelebrationButton);
        }

        private void TryAddHoverEffect(Button button)
        {
            if (button == null) return;

            var existing = button.GetComponent<UIButtonHoverEffect>();
            if (existing != null) return;

            var hover = button.gameObject.AddComponent<UIButtonHoverEffect>();
            hover.Configure(
                hoverScale: 1.05f,
                hoverDuration: 0.15f,
                enableColorChange: false,
                hoverColor: Color.white
            );
        }

        /// <summary>
        /// Initializes all UI components with proper default states.
        /// </summary>
        private void InitializeUIComponents()
        {
            try
            {
                // Configure happiness slider (read-only visual indicator)
                if (happinessSlider != null)
                {
                    happinessSlider.minValue = 0f;
                    happinessSlider.maxValue = 100f;
                    happinessSlider.value = 50f;
                    happinessSlider.interactable = false;
                }

                // Configure eye scale slider (interactive customization)
                if (eyeScaleSlider != null)
                {
                    eyeScaleSlider.minValue = Core.GameConstants.MinEyeScale;
                    eyeScaleSlider.maxValue = Core.GameConstants.MaxEyeScale;
                    eyeScaleSlider.value = 1.0f;
                }

                // Hide loading and feedback elements initially
                SetLoadingState(false);
                HideFeedbackText();

                if (rewardNotification != null)
                    rewardNotification.SetActive(false);

                // Initially hide UI if no user is logged in
                if (Core.UserManager.Instance?.CurrentUser == null)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    // Fade in if already active
                    StartFadeIn();
                }

                Debug.Log("[GameUI] UI components initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error initializing UI components: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets up all button click listeners and input events.
        /// </summary>
        private void SetupEventListeners()
        {
            try
            {
                // Character animation buttons with visual feedback
                SetupButtonWithFeedback(danceButton, () => TriggerCharacterAction(
                    () => characterController?.PlayDance(), "Playing dance animation..."));

                SetupButtonWithFeedback(waveButton, () => TriggerCharacterAction(
                    () => characterController?.PlayWave(), "Waving..."));

                SetupButtonWithFeedback(waiButton, () => TriggerCharacterAction(
                    () => characterController?.PlayWai(), "Performing Thai Wai gesture..."));

                SetupButtonWithFeedback(curtsyButton, () => TriggerCharacterAction(
                    () => characterController?.PlayCurtsy(), "Performing curtsy..."));

                SetupButtonWithFeedback(bowButton, () => TriggerCharacterAction(
                    () => characterController?.PlayBow(), "Bowing..."));

                // Customization controls
                if (eyeScaleSlider != null)
                    eyeScaleSlider.onValueChanged.AddListener(HandleEyeScaleChanged);

                SetupButtonWithFeedback(prevOutfitButton, HandlePreviousOutfit);
                SetupButtonWithFeedback(nextOutfitButton, HandleNextOutfit);
                SetupButtonWithFeedback(prevAccessoryButton, HandlePreviousAccessory);
                SetupButtonWithFeedback(nextAccessoryButton, HandleNextAccessory);

                // Homework system with optimistic UI updates
                SetupButtonWithFeedback(completeHomeworkButton, HandleCompleteHomework);
                SetupButtonWithFeedback(homeworkRewardButton, HandleClaimHomeworkReward);

                // Character care actions (Feed, Rest, Play)
                SetupButtonWithFeedback(feedButton, HandleFeedCharacter);
                SetupButtonWithFeedback(restButton, HandleRestCharacter);
                SetupButtonWithFeedback(playButton, HandlePlayWithCharacter);

                // Account buttons
                SetupButtonWithFeedback(logoutButton, HandleLogout);
                SetupButtonWithFeedback(saveProgressButton, HandleManualSave);

                // Update care action cost displays
                UpdateCareActionCosts();

                Debug.Log("[GameUI] Event listeners configured successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error setting up event listeners: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets up a button with visual feedback animation.
        /// </summary>
        private void SetupButtonWithFeedback(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null) return;

            button.onClick.AddListener(() =>
            {
                // Visual press feedback
                StartCoroutine(AnimateButtonPress(button.transform));
                action?.Invoke();
            });
        }

        /// <summary>
        /// Animates a button press with smooth scale animation for tactile feedback.
        /// Implements modern UI microinteraction patterns with overshoot (spring) effect.
        /// </summary>
        private IEnumerator AnimateButtonPress(Transform buttonTransform)
        {
            if (buttonTransform == null) yield break;

            Vector3 originalScale = buttonTransform.localScale;
            Vector3 targetScale = originalScale * buttonPressScale;
            float duration = buttonPressDuration;

            // Press down
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Cubic ease out
                t = 1f - Mathf.Pow(1f - t, 3);
                buttonTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Release with overshoot (spring effect)
            elapsed = 0f;
            float releaseDuration = duration * 1.5f;
            while (elapsed < releaseDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / releaseDuration;

                // Elastic overshoot math
                float s = 1.70158f; // Overshoot amount
                float elasticT = --t * t * ((s + 1) * t + s) + 1;

                buttonTransform.localScale = Vector3.Lerp(targetScale, originalScale, elasticT);
                yield return null;
            }

            // Ensure we return to exact original scale
            buttonTransform.localScale = originalScale;
        }

        /// <summary>
        /// Toggles the loading skeleton/state.
        /// </summary>
        public void SetLoadingState(bool isLoading)
        {
            if (loadingState != null)
            {
                if (isLoading)
                {
                    loadingState.Show(useSkeleton: true);
                }
                else
                {
                    loadingState.Hide();
                }
            }
            else if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(isLoading);
            }

            if (mainCanvasGroup != null)
            {
                mainCanvasGroup.interactable = !isLoading;
                mainCanvasGroup.blocksRaycasts = !isLoading;
                mainCanvasGroup.alpha = isLoading ? 0.7f : 1.0f;
            }
        }

        /// <summary>
        /// Finds and caches the character controller reference.
        /// </summary>
        private void FindAndCacheCharacterController()
        {
            try
            {
                characterController = FindFirstObjectByType<Character.CharacterController>();
                if (characterController != null)
                {
                    characterController.OnHappinessChanged += HandleCharacterHappinessChanged;
                    Debug.Log("[GameUI] Character controller found and subscribed.");
                }
                else
                {
                    Debug.LogWarning("[GameUI] Character controller not found in scene. Character interactions will be limited.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error finding character controller: {ex.Message}");
            }
        }

        /// <summary>
        /// Subscribes to user management events.
        /// </summary>
        private void SubscribeToUserManagementEvents()
        {
            try
            {
                if (Core.UserManager.Instance != null)
                {
                    Core.UserManager.Instance.OnUserLoggedIn += HandleUserLoggedIn;
                    Core.UserManager.Instance.OnUserLoggedOut += HandleUserLoggedOut;

                    // Check if user is already logged in
                    if (Core.UserManager.Instance.CurrentUser != null)
                    {
                        HandleUserLoggedIn(Core.UserManager.Instance.CurrentUser);
                    }

                    Debug.Log("[GameUI] Subscribed to UserManager events.");
                }
                else
                {
                    Debug.LogWarning("[GameUI] UserManager.Instance is null. Cannot subscribe to events.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error subscribing to user events: {ex.Message}");
            }
        }

        // ===== USER EVENT HANDLERS =====

        /// <summary>
        /// Handles user login with smooth UI transition.
        /// </summary>
        private void HandleUserLoggedIn(Core.UserProfile user)
        {
            try
            {
                if (user == null)
                {
                    Debug.LogWarning("[GameUI] HandleUserLoggedIn called with null user.");
                    return;
                }

                // Unsubscribe from previous user if any
                UnsubscribeFromCurrentUserEvents();

                // Set new current user
                currentUser = user;

                // Show UI with fade-in animation
                gameObject.SetActive(true);
                StartFadeIn();

                // Subscribe to new user events
                SubscribeToCurrentUserEvents();

                // Update all UI elements
                RefreshAllUIElements();

                ShowFeedbackText($"Welcome back, {user.DisplayName}!", Color.green);
                Debug.Log($"[GameUI] UI updated for user: {user.DisplayName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error handling user login: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles user logout with cleanup.
        /// </summary>
        private void HandleUserLoggedOut()
        {
            try
            {
                UnsubscribeFromCurrentUserEvents();
                currentUser = null;

                // Fade out and hide UI
                StartCoroutine(FadeOutAndHide());

                Debug.Log("[GameUI] UI hidden after user logout.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error handling user logout: {ex.Message}");
            }
        }

        /// <summary>
        /// Subscribes to current user's events.
        /// </summary>
        private void SubscribeToCurrentUserEvents()
        {
            if (currentUser != null)
            {
                currentUser.OnCoinsChanged += UpdateCoinsDisplay;
                currentUser.OnExperienceChanged += UpdateExperienceDisplay;
                currentUser.OnHappinessChanged += HandleCharacterHappinessChanged;
                currentUser.OnHungerChanged += HandleHungerChanged;
                currentUser.OnEnergyChanged += HandleEnergyChanged;
            }
        }

        /// <summary>
        /// Unsubscribes from current user's events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromCurrentUserEvents()
        {
            if (currentUser != null)
            {
                currentUser.OnCoinsChanged -= UpdateCoinsDisplay;
                currentUser.OnExperienceChanged -= UpdateExperienceDisplay;
                currentUser.OnHappinessChanged -= HandleCharacterHappinessChanged;
                currentUser.OnHungerChanged -= HandleHungerChanged;
                currentUser.OnEnergyChanged -= HandleEnergyChanged;
            }
        }

        // ===== UI DISPLAY UPDATES =====

        /// <summary>
        /// Updates the coins display with smooth count-up animation for better UX.
        /// Implements modern micro-interaction pattern for currency changes.
        /// Supports optional CoinAnimationController for flying coin effects.
        /// </summary>
        private void UpdateCoinsDisplay(int targetCoins)
        {
            if (coinsText == null) return;

            // Stop any existing coin animation
            if (coinCountUpCoroutine != null)
            {
                StopCoroutine(coinCountUpCoroutine);
            }

            // Try to use CoinAnimationController if available (from main branch)
            // Check if type exists using reflection to avoid compilation errors
            var coinAnimType = System.Type.GetType("SangsomMiniMe.UI.CoinAnimationController");
            bool usedCoinAnimController = false;
            if (coinAnimType != null && displayedCoins < targetCoins)
            {
                var instanceProp = coinAnimType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (instanceProp != null)
                {
                    var coinAnimInstance = instanceProp.GetValue(null);
                    if (coinAnimInstance != null && coinsText.transform != null)
                    {
                        // Call PlayCoinCollectAnimation via reflection
                        var playMethod = coinAnimType.GetMethod("PlayCoinCollectAnimation");
                        if (playMethod != null)
                        {
                            Vector3 spawnPos = coinsText.transform.position;
                            playMethod.Invoke(coinAnimInstance, new object[] {
                                targetCoins - displayedCoins,
                                spawnPos,
                                (System.Action)(() => {
                                    // Play sound if AudioManager exists
                                    var audioType = System.Type.GetType("SangsomMiniMe.Core.AudioManager");
                                    if (audioType != null)
                                    {
                                        var audioInstanceProp = audioType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                                        if (audioInstanceProp != null)
                                        {
                                            var audioInstance = audioInstanceProp.GetValue(null);
                                            if (audioInstance != null)
                                            {
                                                var playCoinMethod = audioType.GetMethod("PlayCoin");
                                                playCoinMethod?.Invoke(audioInstance, null);
                                            }
                                        }
                                    }
                                })
                            });

                            usedCoinAnimController = true;
                        }
                    }
                }
            }

            // Fallback: if no coin animation controller exists, use UIRewardEffects if present.
            if (!usedCoinAnimController && displayedCoins < targetCoins)
            {
                var rewardEffects = UIRewardEffects.Instance;
                if (rewardEffects != null)
                {
                    rewardEffects.PlayCoinRewardEffect(coinsText.transform.position, targetCoins - displayedCoins);
                }
            }

            // Start count-up animation
            coinCountUpCoroutine = StartCoroutine(AnimateCountUp(
                displayedCoins,
                targetCoins,
                coinsText,
                "💰 {0:N0}",
                (newValue) => displayedCoins = newValue,
                0.5f // Animation duration
            ));
        }

        /// <summary>
        /// Updates the experience and level display with smooth count-up animation.
        /// </summary>
        private void UpdateExperienceDisplay(int targetExperience)
        {
            if (experienceText == null) return;

            // Play a level-up celebration if this update crosses a level boundary.
            int startLevel = displayedExperience / Core.GameConstants.ExperiencePerLevel;
            int targetLevel = targetExperience / Core.GameConstants.ExperiencePerLevel;
            if (targetLevel > startLevel)
            {
                var rewardEffects = UIRewardEffects.Instance;
                if (rewardEffects != null)
                {
                    rewardEffects.PlayLevelUpEffect(experienceText.transform.position);
                }
            }

            // Stop any existing XP animation
            if (expCountUpCoroutine != null)
            {
                StopCoroutine(expCountUpCoroutine);
            }

            // Start count-up animation
            expCountUpCoroutine = StartCoroutine(AnimateExperienceCountUp(
                displayedExperience,
                targetExperience,
                0.5f // Animation duration
            ));
        }

        /// <summary>
        /// Animates the experience display with level calculation.
        /// </summary>
        private IEnumerator AnimateExperienceCountUp(int startExp, int targetExp, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Apply ease-out curve for smooth deceleration
                t = 1f - (1f - t) * (1f - t);

                int currentExp = Mathf.RoundToInt(Mathf.Lerp(startExp, targetExp, t));
                displayedExperience = currentExp;

                // Calculate level display
                int level = currentExp / Core.GameConstants.ExperiencePerLevel + 1;
                int expInLevel = currentExp % Core.GameConstants.ExperiencePerLevel;
                experienceText.text = $"⭐ Level {level} ({expInLevel}/{Core.GameConstants.ExperiencePerLevel} XP)";

                yield return null;
            }

            // Ensure final value is exact
            displayedExperience = targetExp;
            int finalLevel = targetExp / Core.GameConstants.ExperiencePerLevel + 1;
            int finalExpInLevel = targetExp % Core.GameConstants.ExperiencePerLevel;
            experienceText.text = $"⭐ Level {finalLevel} ({finalExpInLevel}/{Core.GameConstants.ExperiencePerLevel} XP)";
        }

        /// <summary>
        /// Generic count-up animation for numeric text displays.
        /// Implements smooth easing for professional UX micro-interactions.
        /// </summary>
        /// <param name="startValue">Starting value</param>
        /// <param name="endValue">Target value</param>
        /// <param name="textComponent">Text component to update</param>
        /// <param name="format">String format with {0} placeholder</param>
        /// <param name="onValueUpdate">Callback to update tracked value</param>
        /// <param name="duration">Animation duration in seconds</param>
        private IEnumerator AnimateCountUp(int startValue, int endValue, TextMeshProUGUI textComponent,
            string format, Action<int> onValueUpdate, float duration)
        {
            if (textComponent == null) yield break;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Apply ease-out curve for smooth deceleration (feels more natural)
                t = 1f - (1f - t) * (1f - t);

                int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));
                onValueUpdate?.Invoke(currentValue);

                textComponent.text = string.Format(format, currentValue);

                yield return null;
            }

            // Ensure final value is exact
            onValueUpdate?.Invoke(endValue);
            textComponent.text = string.Format(format, endValue);
        }

        /// <summary>
        /// Refreshes all UI elements with current user data.
        /// </summary>
        private void RefreshAllUIElements()
        {
            if (currentUser == null) return;

            try
            {
                // Update user info
                if (userNameText != null)
                    userNameText.text = currentUser.DisplayName;

                UpdateCoinsDisplay(currentUser.Coins);
                UpdateExperienceDisplay(currentUser.ExperiencePoints);

                // Update happiness
                if (happinessSlider != null)
                    happinessSlider.value = currentUser.CharacterHappiness;

                if (happinessText != null)
                    happinessText.text = $"😊 {currentUser.CharacterHappiness:F0}%";

                // Update homework count
                if (homeworkCountText != null)
                    homeworkCountText.text = $"📚 Completed: {currentUser.HomeworkCompleted}";

                // Update customization displays
                UpdateCustomizationFromUser();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error refreshing UI elements: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates customization sliders from user data.
        /// </summary>
        private void UpdateCustomizationFromUser()
        {
            if (currentUser == null) return;

            try
            {
                // Update eye scale slider without triggering change event
                if (eyeScaleSlider != null)
                {
                    eyeScaleSlider.SetValueWithoutNotify(currentUser.EyeScale);
                }

                // Update outfit and accessory displays
                RefreshOutfitDisplay();
                RefreshAccessoryDisplay();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error updating customization: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles character happiness changes with smooth slider update and premium animation.
        /// Uses easing for organic feel and visual polish.
        /// </summary>
        private void HandleCharacterHappinessChanged(float happiness)
        {
            // Stop any existing animation
            if (happinessAnimCoroutine != null)
            {
                StopCoroutine(happinessAnimCoroutine);
            }

            if (happinessSlider != null)
            {
                // Start smooth fill animation with elastic easing
                happinessAnimCoroutine = StartCoroutine(AnimateMeterFill(
                    happinessSlider,
                    happiness,
                    UITransitionManager.EasingMode.EaseOut
                ));
            }

            if (happinessText != null)
            {
                // Add emoji based on happiness level
                string emoji = happiness >= 80f ? "😄" : happiness >= 60f ? "😊" : happiness >= 40f ? "😐" : happiness >= 20f ? "😟" : "😢";
                happinessText.text = $"{emoji} {happiness:F0}%";
            }
        }

        // ===== CUSTOMIZATION HANDLERS =====

        /// <summary>
        /// Handles eye scale changes with visual feedback.
        /// </summary>
        private void HandleEyeScaleChanged(float scale)
        {
            try
            {
                characterController?.SetEyeScale(scale);
                ShowFeedbackText($"Eye size: {scale:F1}x", Color.cyan, 1f);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error changing eye scale: {ex.Message}");
            }
        }

        /// <summary>
        /// Cycles to previous outfit with visual feedback.
        /// </summary>
        private void HandlePreviousOutfit()
        {
            if (isProcessingAction) return;

            try
            {
                currentOutfitIndex = (currentOutfitIndex - 1 + maxOutfits) % maxOutfits;
                characterController?.SetOutfit(currentOutfitIndex);
                RefreshOutfitDisplay();
                ShowFeedbackText("Outfit changed", Color.cyan, 1f);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error changing outfit: {ex.Message}");
            }
        }

        /// <summary>
        /// Cycles to next outfit with visual feedback.
        /// </summary>
        private void HandleNextOutfit()
        {
            if (isProcessingAction) return;

            try
            {
                currentOutfitIndex = (currentOutfitIndex + 1) % maxOutfits;
                characterController?.SetOutfit(currentOutfitIndex);
                RefreshOutfitDisplay();
                ShowFeedbackText("Outfit changed", Color.cyan, 1f);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error changing outfit: {ex.Message}");
            }
        }

        /// <summary>
        /// Cycles to previous accessory.
        /// </summary>
        private void HandlePreviousAccessory()
        {
            if (isProcessingAction) return;

            try
            {
                currentAccessoryIndex = (currentAccessoryIndex - 1 + maxAccessories) % maxAccessories;
                characterController?.SetAccessory(currentAccessoryIndex);
                RefreshAccessoryDisplay();
                ShowFeedbackText("Accessory changed", Color.cyan, 1f);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error changing accessory: {ex.Message}");
            }
        }

        /// <summary>
        /// Cycles to next accessory.
        /// </summary>
        private void HandleNextAccessory()
        {
            if (isProcessingAction) return;

            try
            {
                currentAccessoryIndex = (currentAccessoryIndex + 1) % maxAccessories;
                characterController?.SetAccessory(currentAccessoryIndex);
                RefreshAccessoryDisplay();
                ShowFeedbackText("Accessory changed", Color.cyan, 1f);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error changing accessory: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates outfit display text.
        /// </summary>
        private void RefreshOutfitDisplay()
        {
            if (outfitText != null)
            {
                string outfitName = currentOutfitIndex == 0 ? "Default Outfit" : $"Outfit #{currentOutfitIndex}";
                outfitText.text = outfitName;
            }
        }

        /// <summary>
        /// Updates accessory display text.
        /// </summary>
        private void RefreshAccessoryDisplay()
        {
            if (accessoryText != null)
            {
                string accessoryName = currentAccessoryIndex == 0 ? "No Accessory" : $"Accessory #{currentAccessoryIndex}";
                accessoryText.text = accessoryName;
            }
        }

        // ===== HOMEWORK & REWARDS =====

        /// <summary>
        /// Handles homework completion with optimistic UI updates and celebration.
        /// </summary>
        private void HandleCompleteHomework()
        {
            if (isProcessingAction || currentUser == null) return;

            try
            {
                isProcessingAction = true;

                // Optimistic update - immediate feedback
                currentUser.CompleteHomework();
                characterController?.IncreaseHappiness(10f);

                // Play coin reward sound
                Core.AudioManager.Instance?.PlayCoin();

                // Visual celebration if available
                if (coinsText != null)
                {
                    UIRewardEffects.Instance?.PlayCoinRewardEffect(coinsText.transform.position, 10);
                }

                // Trigger celebration animation
                if (characterController != null && !characterController.IsAnimating)
                {
                    characterController.PlayDance();
                }

                // Update UI immediately
                RefreshAllUIElements();
                Core.UserManager.Instance?.MarkDirty();

                // Show rewarding feedback
                ShowFeedbackText("📚 Great job! Homework completed! +10 Happiness", Color.green);

                // Show reward notification
                StartCoroutine(ShowRewardNotificationCoroutine());

                Debug.Log("[GameUI] Homework completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error completing homework: {ex.Message}");
                ShowFeedbackText("Error completing homework", Color.red);
            }
            finally
            {
                isProcessingAction = false;
            }
        }

        /// <summary>
        /// Handles claiming homework rewards with visual feedback.
        /// </summary>
        private void HandleClaimHomeworkReward()
        {
            if (isProcessingAction || currentUser == null) return;

            try
            {
                isProcessingAction = true;

                // Grant rewards
                currentUser.AddCoins(10);
                currentUser.AddExperience(5);
                characterController?.IncreaseHappiness(5f);

                // Reward feedback if available
                if (coinsText != null)
                {
                    UIRewardEffects.Instance?.PlayCoinRewardEffect(coinsText.transform.position, 10);
                }

                // Update UI
                RefreshAllUIElements();
                Core.UserManager.Instance?.MarkDirty();

                // Show feedback
                ShowFeedbackText("🎁 Reward claimed! +10 coins, +5 XP, +5 happiness", Color.yellow);

                Debug.Log("[GameUI] Homework reward claimed.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error claiming reward: {ex.Message}");
                ShowFeedbackText("Error claiming reward", Color.red);
            }
            finally
            {
                isProcessingAction = false;
            }
        }

        /// <summary>
        /// Shows reward notification with animation.
        /// </summary>
        private IEnumerator ShowRewardNotificationCoroutine()
        {
            if (rewardNotification == null) yield break;

            rewardNotification.SetActive(true);
            yield return new WaitForSeconds(2f);
            rewardNotification.SetActive(false);
        }

        // ===== CHARACTER CARE ACTIONS =====

        /// <summary>
        /// Handles feeding the character to restore hunger.
        /// Costs coins but restores hunger meter.
        /// </summary>
        private void HandleFeedCharacter()
        {
            if (isProcessingAction || currentUser == null) return;

            try
            {
                isProcessingAction = true;

                // Check if user can afford
                if (currentUser.Coins < Core.GameConstants.FeedCost)
                {
                    ShowFeedbackText($"Not enough coins! Need {Core.GameConstants.FeedCost} 🪙", Color.yellow);
                    return;
                }

                // Check if hunger is already full
                if (currentUser.CharacterHunger >= 95f)
                {
                    ShowFeedbackText("Your Mini-Me isn't hungry right now! 😊", Color.cyan);
                    return;
                }

                // Spend coins and feed
                currentUser.SpendCoins(Core.GameConstants.FeedCost);
                currentUser.Feed();

                // Play feed sound
                Core.AudioManager.Instance?.PlayFeed();

                // Update UI
                RefreshAllUIElements();
                UpdateMeterDisplays();
                Core.UserManager.Instance?.MarkDirty();

                ShowFeedbackText($"🍎 Yummy! +{Core.GameConstants.FeedHungerRecovery:F0} Hunger", Color.green);
                Debug.Log("[GameUI] Character fed successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error feeding character: {ex.Message}");
                ShowFeedbackText("Error feeding character", Color.red);
            }
            finally
            {
                isProcessingAction = false;
            }
        }

        /// <summary>
        /// Handles resting the character to restore energy.
        /// Costs coins but restores energy meter.
        /// </summary>
        private void HandleRestCharacter()
        {
            if (isProcessingAction || currentUser == null) return;

            try
            {
                isProcessingAction = true;

                // Check if user can afford
                if (currentUser.Coins < Core.GameConstants.RestCost)
                {
                    ShowFeedbackText($"Not enough coins! Need {Core.GameConstants.RestCost} 🪙", Color.yellow);
                    return;
                }

                // Check if energy is already full
                if (currentUser.CharacterEnergy >= 95f)
                {
                    ShowFeedbackText("Your Mini-Me is full of energy! ⚡", Color.cyan);
                    return;
                }

                // Spend coins and rest
                currentUser.SpendCoins(Core.GameConstants.RestCost);
                currentUser.Rest();

                // Play rest sound
                Core.AudioManager.Instance?.PlayRest();

                // Update UI
                RefreshAllUIElements();
                UpdateMeterDisplays();
                Core.UserManager.Instance?.MarkDirty();

                ShowFeedbackText($"😴 Rested! +{Core.GameConstants.RestEnergyRecovery:F0} Energy", Color.green);
                Debug.Log("[GameUI] Character rested successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error resting character: {ex.Message}");
                ShowFeedbackText("Error resting character", Color.red);
            }
            finally
            {
                isProcessingAction = false;
            }
        }

        /// <summary>
        /// Handles playing with the character to restore happiness.
        /// Free action that triggers dance and increases happiness.
        /// </summary>
        private void HandlePlayWithCharacter()
        {
            if (isProcessingAction || currentUser == null) return;

            try
            {
                isProcessingAction = true;

                // Play is free! Just increase happiness
                currentUser.IncreaseHappiness(Core.GameConstants.PlayHappinessBonus);

                // Play happy sound
                Core.AudioManager.Instance?.PlayPlay();

                // Trigger celebration animation
                if (characterController != null && !characterController.IsAnimating)
                {
                    characterController.PlayDance();
                }

                // Update UI
                RefreshAllUIElements();
                UpdateMeterDisplays();
                Core.UserManager.Instance?.MarkDirty();

                ShowFeedbackText($"🎉 Fun time! +{Core.GameConstants.PlayHappinessBonus:F0} Happiness", Color.magenta);
                Debug.Log("[GameUI] Played with character successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error playing with character: {ex.Message}");
                ShowFeedbackText("Error playing", Color.red);
            }
            finally
            {
                isProcessingAction = false;
            }
        }

        /// <summary>
        /// Updates the care action cost display texts.
        /// </summary>
        private void UpdateCareActionCosts()
        {
            if (feedCostText != null)
                feedCostText.text = $"{Core.GameConstants.FeedCost} 🪙";
            if (restCostText != null)
                restCostText.text = $"{Core.GameConstants.RestCost} 🪙";
        }

        // ===== CHARACTER INTERACTIONS =====

        /// <summary>
        /// Triggers a character action with proper state checking.
        /// </summary>
        private void TriggerCharacterAction(Action action, string feedbackMessage)
        {
            if (characterController == null)
            {
                ShowFeedbackText("Character not available", Color.red, 1.5f);
                return;
            }

            if (characterController.IsAnimating)
            {
                ShowFeedbackText("Character is busy...", Color.yellow, 1f);
                return;
            }

            try
            {
                action?.Invoke();
                ShowFeedbackText(feedbackMessage, Color.cyan, 1.5f);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error triggering character action: {ex.Message}");
                ShowFeedbackText("Action failed", Color.red, 1.5f);
            }
        }

        // ===== ACCOUNT MANAGEMENT =====

        /// <summary>
        /// Handles user logout with confirmation feedback.
        /// </summary>
        private void HandleLogout()
        {
            if (isProcessingAction) return;

            try
            {
                isProcessingAction = true;

                ShowFeedbackText("Logging out...", Color.white, 1f);

                // Small delay for feedback visibility
                StartCoroutine(LogoutWithDelay());
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error during logout: {ex.Message}");
                isProcessingAction = false;
            }
        }

        /// <summary>
        /// Logout with a small delay for better UX.
        /// </summary>
        private IEnumerator LogoutWithDelay()
        {
            yield return new WaitForSeconds(0.5f);

            Core.UserManager.Instance?.LogoutUser();
            isProcessingAction = false;
        }

        /// <summary>
        /// Handles manual save with visual confirmation.
        /// </summary>
        private void HandleManualSave()
        {
            if (isProcessingAction) return;

            try
            {
                isProcessingAction = true;
                SetLoadingState(true);

                Core.UserManager.Instance?.SaveCurrentUser();

                ShowFeedbackText("✓ Progress saved successfully!", Color.green);
                Debug.Log("[GameUI] Manual save completed.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error during manual save: {ex.Message}");
                ShowFeedbackText("Save failed", Color.red);
            }
            finally
            {
                SetLoadingState(false);
                isProcessingAction = false;
            }
        }

        // ===== VISUAL FEEDBACK & ANIMATIONS =====

        /// <summary>
        /// Shows a gentle reminder message for low meters (cozy, not stressful).
        /// </summary>
        /// <param name="message">The friendly reminder message</param>
        public void ShowGentleReminder(string message)
        {
            // Play gentle reminder sound
            Core.AudioManager.Instance?.PlayGentleReminder();

            // Use a soft color for gentle reminders - not red/alarming
            ShowFeedbackText(message, new Color(0.4f, 0.7f, 1f), 3f); // Soft blue
        }

        /// <summary>
        /// Shows temporary feedback text to the user.
        /// </summary>
        private void ShowFeedbackText(string message, Color color, float duration = 0f)
        {
            if (feedbackText == null) return;

            // Stop existing feedback coroutine
            if (feedbackCoroutine != null)
                StopCoroutine(feedbackCoroutine);

            feedbackText.text = message;
            feedbackText.color = color;
            feedbackText.gameObject.SetActive(true);

            // Auto-hide after duration
            float displayTime = duration > 0 ? duration : feedbackDisplayDuration;
            feedbackCoroutine = StartCoroutine(HideFeedbackAfterDelay(displayTime));
        }

        /// <summary>
        /// Hides feedback text after delay.
        /// </summary>
        private IEnumerator HideFeedbackAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideFeedbackText();
        }

        /// <summary>
        /// Hides the feedback text immediately.
        /// </summary>
        private void HideFeedbackText()
        {
            if (feedbackText != null)
                feedbackText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Starts fade-in animation for smooth UI appearance.
        /// </summary>
        private void StartFadeIn()
        {
            if (mainCanvasGroup == null) return;

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeCanvasGroup(mainCanvasGroup, 0f, 1f, uiFadeInDuration));
        }

        /// <summary>
        /// Fades out UI and hides it.
        /// </summary>
        private IEnumerator FadeOutAndHide()
        {
            if (mainCanvasGroup != null)
            {
                yield return FadeCanvasGroup(mainCanvasGroup, 1f, 0f, uiFadeInDuration);
            }
            else
            {
                yield return new WaitForSeconds(0.3f);
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Smoothly fades a canvas group between alpha values.
        /// </summary>
        private IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
        {
            if (group == null) yield break;

            float elapsed = 0f;
            group.alpha = startAlpha;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
                yield return null;
            }

            group.alpha = endAlpha;
        }

        // ===== DAILY LOGIN CELEBRATION =====

        /// <summary>
        /// Shows the daily login bonus celebration panel.
        /// Displays streak info, coins earned, and milestone achievements.
        /// </summary>
        /// <param name="result">The login bonus result containing reward details</param>
        public void ShowLoginBonusCelebration(Core.LoginBonusResult result)
        {
            if (!result.IsFirstLoginToday) return;

            try
            {
                // Play appropriate sound effect
                if (result.HitMilestone)
                {
                    Core.AudioManager.Instance?.PlayMilestone();
                }
                else
                {
                    Core.AudioManager.Instance?.PlayLoginBonus();
                }

                // Update celebration UI text
                if (celebrationTitleText != null)
                {
                    if (result.HitMilestone)
                    {
                        celebrationTitleText.text = $"🎉 {result.MilestoneDay} Day Milestone!";
                    }
                    else if (result.IsNewRecord)
                    {
                        celebrationTitleText.text = "🏆 New Streak Record!";
                    }
                    else if (result.CurrentStreak > 1)
                    {
                        celebrationTitleText.text = $"{Core.DailyLoginSystem.GetStreakEmoji(result.CurrentStreak)} Daily Bonus!";
                    }
                    else
                    {
                        celebrationTitleText.text = "✨ Welcome Back!";
                    }
                }

                if (celebrationMessageText != null)
                {
                    celebrationMessageText.text = Core.DailyLoginSystem.GetCelebrationMessage(result);
                }

                if (celebrationCoinsText != null)
                {
                    celebrationCoinsText.text = $"+{result.CoinsEarned} coins";
                }

                if (streakDisplayText != null)
                {
                    string streakEmoji = Core.DailyLoginSystem.GetStreakEmoji(result.CurrentStreak);
                    int daysToNext = Core.DailyLoginSystem.GetDaysUntilNextMilestone(result.CurrentStreak);
                    int nextMilestone = Core.DailyLoginSystem.GetNextMilestoneDay(result.CurrentStreak);

                    if (nextMilestone > 0)
                    {
                        streakDisplayText.text = $"{streakEmoji} {result.CurrentStreak} day streak\n{daysToNext} days to {nextMilestone} day milestone!";
                    }
                    else
                    {
                        streakDisplayText.text = $"👑 {result.CurrentStreak} day streak\nAll milestones achieved!";
                    }
                }

                // Show celebration panel
                if (celebrationPanel != null)
                {
                    celebrationPanel.SetActive(true);
                    StartCoroutine(AutoHideCelebration());
                }

                // Setup close button
                if (closeCelebrationButton != null)
                {
                    closeCelebrationButton.onClick.RemoveAllListeners();
                    closeCelebrationButton.onClick.AddListener(HideCelebration);
                }

                Debug.Log($"[GameUI] Showing login celebration: +{result.CoinsEarned} coins, {result.CurrentStreak} day streak");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error showing celebration: {ex.Message}");
            }
        }

        /// <summary>
        /// Auto-hides celebration panel after a delay.
        /// </summary>
        private IEnumerator AutoHideCelebration()
        {
            yield return new WaitForSeconds(celebrationDisplayDuration);
            HideCelebration();
        }

        /// <summary>
        /// Hides the celebration panel.
        /// </summary>
        private void HideCelebration()
        {
            if (celebrationPanel != null)
            {
                celebrationPanel.SetActive(false);
            }
        }

        // ===== METER DISPLAY UPDATES =====

        /// <summary>
        /// Updates all meter displays (happiness, hunger, energy, mood).
        /// Called by GameManager when meters decay.
        /// </summary>
        public void UpdateMeterDisplays()
        {
            if (currentUser == null) return;

            try
            {
                // Update happiness
                if (happinessSlider != null)
                    happinessSlider.value = currentUser.CharacterHappiness;
                if (happinessText != null)
                    happinessText.text = $"{currentUser.CharacterHappiness:F0}%";

                // Update hunger
                if (hungerSlider != null)
                    hungerSlider.value = currentUser.CharacterHunger;
                if (hungerText != null)
                    hungerText.text = $"{currentUser.CharacterHunger:F0}%";

                // Update energy
                if (energySlider != null)
                    energySlider.value = currentUser.CharacterEnergy;
                if (energyText != null)
                    energyText.text = $"{currentUser.CharacterEnergy:F0}%";

                // Update mood display
                UpdateMoodDisplay();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error updating meter displays: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates the mood display based on overall meter values.
        /// </summary>
        private void UpdateMoodDisplay()
        {
            if (moodText == null || currentUser == null) return;

            var mood = Core.MeterDecaySystem.GetOverallMood(currentUser);
            string emoji = Core.MeterDecaySystem.GetMoodEmoji(mood);
            string description = Core.MeterDecaySystem.GetMoodDescription(mood);

            moodText.text = $"{emoji} {description}";
        }

        /// <summary>
        /// Animates a slider filling to target value with smooth easing.
        /// Provides premium micro-interaction feel with organic motion.
        /// </summary>
        /// <param name="slider">The UI slider to animate</param>
        /// <param name="targetValue">The target fill value</param>
        /// <param name="easingMode">Easing function to use</param>
        private IEnumerator AnimateMeterFill(Slider slider, float targetValue, UITransitionManager.EasingMode easingMode)
        {
            if (slider == null) yield break;

            float startValue = slider.value;
            float elapsed = 0f;

            // Add subtle color pulse for visual feedback
            Image fillImage = slider.fillRect?.GetComponent<Image>();
            Color originalColor = fillImage != null ? fillImage.color : Color.white;

            while (elapsed < meterAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / meterAnimationDuration);

                // Apply easing for smooth, organic feel
                float easedT = UITransitionManager.GetEasedValue(t, easingMode);

                // Interpolate slider value
                slider.value = Mathf.Lerp(startValue, targetValue, easedT);

                // Optional: Pulse color on increase (positive feedback)
                if (fillImage != null && targetValue > startValue)
                {
                    // Subtle brightness pulse
                    float pulseIntensity = Mathf.Sin(t * Mathf.PI) * 0.2f;
                    fillImage.color = Color.Lerp(originalColor, Color.white, pulseIntensity);
                }

                yield return null;
            }

            // Ensure final value is exact
            slider.value = targetValue;

            // Restore original color
            if (fillImage != null)
            {
                fillImage.color = originalColor;
            }
        }

        /// <summary>
        /// Handles hunger meter changes with smooth animation.
        /// </summary>
        private void HandleHungerChanged(float hunger)
        {
            if (hungerAnimCoroutine != null)
            {
                StopCoroutine(hungerAnimCoroutine);
            }

            if (hungerSlider != null)
            {
                hungerAnimCoroutine = StartCoroutine(AnimateMeterFill(
                    hungerSlider,
                    hunger,
                    UITransitionManager.EasingMode.EaseOut
                ));
            }

            if (hungerText != null)
            {
                string emoji = hunger >= 70f ? "🍽️" : hunger >= 40f ? "🍴" : "🍔";
                hungerText.text = $"{emoji} {hunger:F0}%";
            }
        }

        /// <summary>
        /// Handles energy meter changes with smooth animation.
        /// </summary>
        private void HandleEnergyChanged(float energy)
        {
            if (energyAnimCoroutine != null)
            {
                StopCoroutine(energyAnimCoroutine);
            }

            if (energySlider != null)
            {
                energyAnimCoroutine = StartCoroutine(AnimateMeterFill(
                    energySlider,
                    energy,
                    UITransitionManager.EasingMode.EaseOut
                ));
            }

            if (energyText != null)
            {
                string emoji = energy >= 70f ? "⚡" : energy >= 40f ? "🔋" : "🪫";
                energyText.text = $"{emoji} {energy:F0}%";
            }
        }

        // ===== CLEANUP =====

        /// <summary>
        /// Cleanup when UI is destroyed to prevent memory leaks.
        /// </summary>
        private void OnDestroy()
        {
            try
            {
                // Unsubscribe from UserManager events
                if (Core.UserManager.Instance != null)
                {
                    Core.UserManager.Instance.OnUserLoggedIn -= HandleUserLoggedIn;
                    Core.UserManager.Instance.OnUserLoggedOut -= HandleUserLoggedOut;
                }

                // Unsubscribe from current user events
                UnsubscribeFromCurrentUserEvents();

                // Unsubscribe from character controller
                if (characterController != null)
                {
                    characterController.OnHappinessChanged -= HandleCharacterHappinessChanged;
                }

                Debug.Log("[GameUI] Cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GameUI] Error during cleanup: {ex.Message}");
            }
        }
    }
}