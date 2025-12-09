using UnityEngine;
using System.Collections;

namespace SangsomMiniMe.Character
{
    /// <summary>
    /// Controls the Mini-Me character behavior and interactions
    /// </summary>
    public class CharacterController : MonoBehaviour
    {
        [Header("Character Settings")]
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private Transform eyeScale1;
        [SerializeField] private Transform eyeScale2;
        [SerializeField] private Transform accessoryPoint;
        [SerializeField] private SkinnedMeshRenderer characterRenderer;

        [Header("Configuration")]
        [SerializeField] private GameConfiguration gameConfig;

        [Header("Customization")]
        [SerializeField] private Material[] outfitMaterials;
        [SerializeField] private GameObject[] accessories;

        [Header("Animation")]
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip danceClip;
        [SerializeField] private AnimationClip waveClip;
        [SerializeField] private AnimationClip waiClip;
        [SerializeField] private AnimationClip curtsyClip;
        [SerializeField] private AnimationClip bowClip;

        [Header("Happiness Indicators")]
        [SerializeField] private ParticleSystem happinessParticles;
        [SerializeField] private GameObject sadnessIndicator;

        private float currentEyeScale = 1.0f;
        private int currentOutfitIndex = 0;
        private int currentAccessoryIndex = 0;
        private float currentHappiness = 75f;
        private bool isAnimating = false;

        // Animation state hashes for performance
        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int DanceHash = Animator.StringToHash("Dance");
        private static readonly int WaveHash = Animator.StringToHash("Wave");
        private static readonly int WaiHash = Animator.StringToHash("Wai");
        private static readonly int CurtsyHash = Animator.StringToHash("Curtsy");
        private static readonly int BowHash = Animator.StringToHash("Bow");

        public float CurrentHappiness => currentHappiness;
        public bool IsAnimating => isAnimating;

        // Events
        public System.Action<string> OnAnimationStarted;
        public System.Action<string> OnAnimationCompleted;
        public System.Action<float> OnHappinessChanged;

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Set default values
            SetEyeScale(currentEyeScale);
            SetOutfit(currentOutfitIndex);
            SetAccessory(currentAccessoryIndex);
            UpdateHappinessDisplay();

            // Subscribe to user manager events
            if (Core.UserManager.Instance != null)
            {
                Core.UserManager.Instance.OnUserLoggedIn += OnUserLoggedIn;
                Core.UserManager.Instance.OnUserLoggedOut += OnUserLoggedOut;
            }

            Debug.Log("Character Controller initialized");
        }

        private void OnUserLoggedIn(Core.UserProfile user)
        {
            // Apply user's character customization
            SetEyeScale(user.EyeScale);
            SetOutfitByName(user.CurrentOutfit);
            SetAccessoryByName(user.CurrentAccessory);
            SetHappiness(user.CharacterHappiness);

            Debug.Log($"Character customized for user: {user.DisplayName}");
        }

        private void OnUserLoggedOut()
        {
            // Reset to default appearance
            SetEyeScale(1.0f);
            SetOutfit(0);
            SetAccessory(0);
            SetHappiness(50f);
        }

        public void SetEyeScale(float scale)
        {
            float minScale = gameConfig != null ? gameConfig.MinEyeScale : GameConstants.MinEyeScale;
            float maxScale = gameConfig != null ? gameConfig.MaxEyeScale : GameConstants.MaxEyeScale;
            scale = Mathf.Clamp(scale, minScale, maxScale);
            currentEyeScale = scale;

            if (eyeScale1 != null)
                eyeScale1.localScale = Vector3.one * scale;
            if (eyeScale2 != null)
                eyeScale2.localScale = Vector3.one * scale;

            // Save to current user if logged in
            if (Core.UserManager.Instance?.CurrentUser != null)
            {
                Core.UserManager.Instance.CurrentUser.SetEyeScale(scale, gameConfig);
                Core.UserManager.Instance.MarkDirty();
            }
        }

        public void SetOutfit(int outfitIndex)
        {
            if (outfitMaterials != null && outfitIndex >= 0 && outfitIndex < outfitMaterials.Length)
            {
                currentOutfitIndex = outfitIndex;
                if (characterRenderer != null)
                {
                    characterRenderer.material = outfitMaterials[outfitIndex];
                }

                // Update user profile (save will be handled by auto-save)
                if (Core.UserManager.Instance?.CurrentUser != null)
                {
                    string outfitName = outfitIndex == 0 ? "default" : $"outfit_{outfitIndex}";
                    Core.UserManager.Instance.CurrentUser.SetOutfit(outfitName);
                    Core.UserManager.Instance.MarkDirty();
                }
            }
        }

        public void SetOutfitByName(string outfitName)
        {
            if (string.IsNullOrEmpty(outfitName) || outfitName == "default")
            {
                SetOutfit(0);
                return;
            }

            // Optimized parsing to avoid string splitting
            if (outfitName.StartsWith("outfit_"))
            {
                // Extract number part: "outfit_1" -> "1"
                // Using substring is better than Replace for simple prefix removal
                string numberPart = outfitName.Substring(7);
                if (int.TryParse(numberPart, out int index))
                {
                    SetOutfit(index);
                }
            }
        }

        public void SetAccessory(int accessoryIndex)
        {
            // Hide all accessories first
            if (accessories != null)
            {
                for (int i = 0; i < accessories.Length; i++)
                {
                    if (accessories[i] != null)
                        accessories[i].SetActive(false);
                }

                // Show selected accessory
                if (accessoryIndex > 0 && accessoryIndex < accessories.Length && accessories[accessoryIndex] != null)
                {
                    accessories[accessoryIndex].SetActive(true);
                    currentAccessoryIndex = accessoryIndex;
                }
                else
                {
                    currentAccessoryIndex = 0; // No accessory
                }

                // Update user profile (save will be handled by auto-save)
                if (Core.UserManager.Instance?.CurrentUser != null)
                {
                    // Cache the string to avoid allocation if possible, but for now this is acceptable
                    string accessoryName = currentAccessoryIndex == 0 ? "none" : $"accessory_{currentAccessoryIndex}";
                    Core.UserManager.Instance.CurrentUser.SetAccessory(accessoryName);
                    Core.UserManager.Instance.MarkDirty();
                }
            }
        }

        public void SetAccessoryByName(string accessoryName)
        {
            if (string.IsNullOrEmpty(accessoryName) || accessoryName == "none")
            {
                SetAccessory(0);
                return;
            }

            if (accessoryName.StartsWith("accessory_"))
            {
                string numberPart = accessoryName.Substring(10); // "accessory_" length is 10
                if (int.TryParse(numberPart, out int index))
                {
                    SetAccessory(index);
                }
            }
        }

        public void SetHappiness(float happiness)
        {
            happiness = Mathf.Clamp(happiness, 0f, 100f);
            currentHappiness = happiness;
            UpdateHappinessDisplay();
            OnHappinessChanged?.Invoke(currentHappiness);
        }

        private void UpdateHappinessDisplay()
        {
            float happyThreshold = gameConfig != null ? gameConfig.HappyThreshold : GameConstants.HappyThreshold;
            float sadThreshold = gameConfig != null ? gameConfig.SadThreshold : GameConstants.SadThreshold;

            // Update particle effects and indicators based on happiness
            if (happinessParticles != null)
            {
                if (currentHappiness > happyThreshold)
                {
                    happinessParticles.gameObject.SetActive(true);
                    var emission = happinessParticles.emission;
                    emission.rateOverTime = (currentHappiness - happyThreshold) / (100f - happyThreshold) * 10f; // Scale particles with happiness
                }
                else
                {
                    happinessParticles.gameObject.SetActive(false);
                }
            }

            if (sadnessIndicator != null)
            {
                sadnessIndicator.SetActive(currentHappiness < sadThreshold);
            }
        }

        // Animation methods
        public void PlayIdle()
        {
            PlayAnimation("Idle", IdleHash);
        }

        public void PlayDance()
        {
            PlayAnimation("Dance", DanceHash);
            float happinessBonus = gameConfig != null ? gameConfig.DanceHappinessBonus : GameConstants.DanceHappinessBonus;
            IncreaseHappiness(happinessBonus);
        }

        public void PlayWave()
        {
            PlayAnimation("Wave", WaveHash);
        }

        public void PlayWai()
        {
            PlayAnimation("Wai", WaiHash);
        }

        public void PlayCurtsy()
        {
            PlayAnimation("Curtsy", CurtsyHash);
        }

        public void PlayBow()
        {
            PlayAnimation("Bow", BowHash);
        }

        private void PlayAnimation(string animationName, int animationHash)
        {
            if (characterAnimator != null && !isAnimating)
            {
                characterAnimator.SetTrigger(animationHash);
                isAnimating = true;
                OnAnimationStarted?.Invoke(animationName);

                StartCoroutine(WaitForAnimationComplete(animationName));

                Debug.Log($"Playing animation: {animationName}");
            }
        }

        private IEnumerator WaitForAnimationComplete(string animationName)
        {
            // Wait for animation to start
            yield return null;

            float waitTime = gameConfig != null ? gameConfig.AnimationDuration : Core.GameConstants.DefaultAnimationDuration;

            // Try to get actual animation length
            if (characterAnimator != null)
            {
                var clipInfo = characterAnimator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    waitTime = clipInfo[0].clip.length;
                }
            }

            yield return new WaitForSeconds(waitTime);

            isAnimating = false;
            OnAnimationCompleted?.Invoke(animationName);
        }

        public void IncreaseHappiness(float amount)
        {
            SetHappiness(currentHappiness + amount);

            // Update user profile (save will be handled by auto-save)
            if (Core.UserManager.Instance?.CurrentUser != null)
            {
                Core.UserManager.Instance.CurrentUser.IncreaseHappiness(amount);
                Core.UserManager.Instance.MarkDirty();
            }
        }

        public void DecreaseHappiness(float amount)
        {
            SetHappiness(currentHappiness - amount);

            // Update user profile (save will be handled by auto-save)
            if (Core.UserManager.Instance?.CurrentUser != null)
            {
                Core.UserManager.Instance.CurrentUser.DecreaseHappiness(amount);
                Core.UserManager.Instance.MarkDirty();
            }
        }

        // Interaction methods for UI buttons
        public void OnCharacterClicked()
        {
            if (!isAnimating)
            {
                // Cycle through different interactions on click
                int randomAction = Random.Range(0, 4);
                switch (randomAction)
                {
                    case 0: PlayWave(); break;
                    case 1: PlayDance(); break;
                    case 2: PlayWai(); break;
                    case 3: PlayCurtsy(); break;
                }
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
        }
    }
}