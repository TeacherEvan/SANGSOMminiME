using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Enhanced button component with micro-interactions for premium UX.
    /// Implements hover states, press feedback, and haptic responses.
    /// Follows modern UI/UX best practices for interactive elements.
    /// 
    /// PERFORMANCE OPTIMIZATION: Uses event-driven coroutine-based animations instead of
    /// per-frame Update() polling. Animations only run when button state changes (hover/press),
    /// significantly reducing CPU overhead for buttons not being interacted with.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class InteractiveButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [Header("Visual Feedback")]
        [SerializeField] private bool enableScaleAnimation = true;
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float pressedScale = 0.95f;
        [SerializeField] private float scaleAnimationSpeed = 10f;

        [Header("Color Feedback")]
        [SerializeField] private bool enableColorAnimation = true;
        [SerializeField] private Image targetGraphic;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 1f, 1f);
        [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.9f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        [SerializeField] private float colorTransitionSpeed = 8f;

        [Header("Sound Effects")]
        [SerializeField] private bool enableSoundEffects = true;
        [SerializeField] private AudioClip hoverSound;
        [SerializeField] private AudioClip clickSound;
        [SerializeField] private float soundVolume = 0.5f;

        [Header("Particle Effects")]
        [SerializeField] private bool enableParticleEffects = false;
        [SerializeField] private ParticleSystem clickParticles;

        [Header("Haptic Feedback")]
        [SerializeField] private bool enableHapticFeedback = true;

        // Component references
        private Button button;
        private RectTransform rectTransform;
        private AudioSource audioSource;

        // Animation state
        private Vector3 originalScale;
        private Vector3 targetScale;
        private Color targetColor;
        private bool isHovering;
        private bool isInitialized;

        // Coroutine tracking for animations
        private Coroutine scaleAnimationCoroutine;
        private Coroutine colorAnimationCoroutine;
        private Coroutine pulseAnimationCoroutine;

        private void Awake()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            if (isInitialized) return;

            // Get required components
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();

            // Cache original scale
            originalScale = transform.localScale;
            targetScale = originalScale;

            // Get or set target graphic
            if (targetGraphic == null)
            {
                targetGraphic = GetComponent<Image>();
                if (targetGraphic == null)
                {
                    targetGraphic = GetComponentInChildren<Image>();
                }
            }

            // Set initial color
            if (targetGraphic != null)
            {
                normalColor = targetGraphic.color;
                targetColor = normalColor;
            }

            // Setup audio source for sound effects
            if (enableSoundEffects && (hoverSound != null || clickSound != null))
            {
                audioSource = gameObject.GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
                audioSource.playOnAwake = false;
                audioSource.volume = soundVolume;
            }

            // Subscribe to button events
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClicked);
            }

            isInitialized = true;
        }

        /// <summary>
        /// Smoothly animates the scale to the target value.
        /// Coroutine-based approach for better performance than Update() polling.
        /// </summary>
        private IEnumerator AnimateScale(Vector3 targetScale)
        {
            const float threshold = 0.001f;
            
            while (Vector3.Distance(transform.localScale, targetScale) > threshold)
            {
                transform.localScale = Vector3.Lerp(
                    transform.localScale,
                    targetScale,
                    Time.deltaTime * scaleAnimationSpeed
                );
                yield return null;
            }
            
            transform.localScale = targetScale;
            scaleAnimationCoroutine = null;
        }

        /// <summary>
        /// Smoothly animates the color to the target value.
        /// Coroutine-based approach for better performance than Update() polling.
        /// </summary>
        private IEnumerator AnimateColor(Color targetColor)
        {
            if (targetGraphic == null) yield break;
            
            const float threshold = 0.01f;
            
            while (Mathf.Abs((targetGraphic.color - targetColor).sqrMagnitude) > threshold)
            {
                targetGraphic.color = Color.Lerp(
                    targetGraphic.color,
                    targetColor,
                    Time.deltaTime * colorTransitionSpeed
                );
                yield return null;
            }
            
            targetGraphic.color = targetColor;
            colorAnimationCoroutine = null;
        }

        /// <summary>
        /// Starts scale animation coroutine, stopping any previous animation.
        /// </summary>
        private void StartScaleAnimation(Vector3 target)
        {
            if (!enableScaleAnimation) return;
            
            if (scaleAnimationCoroutine != null)
            {
                StopCoroutine(scaleAnimationCoroutine);
            }
            
            targetScale = target;
            scaleAnimationCoroutine = StartCoroutine(AnimateScale(target));
        }

        /// <summary>
        /// Starts color animation coroutine, stopping any previous animation.
        /// </summary>
        private void StartColorAnimation(Color target)
        {
            if (!enableColorAnimation || targetGraphic == null) return;
            
            if (colorAnimationCoroutine != null)
            {
                StopCoroutine(colorAnimationCoroutine);
            }
            
            targetColor = target;
            colorAnimationCoroutine = StartCoroutine(AnimateColor(target));
        }

        /// <summary>
        /// Called when pointer enters the button area.
        /// Implements hover state with visual and audio feedback.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button == null || !button.interactable) return;

            isHovering = true;

            // Scale up on hover (coroutine-based)
            StartScaleAnimation(originalScale * hoverScale);

            // Change color on hover (coroutine-based)
            StartColorAnimation(hoverColor);

            // Play hover sound
            if (enableSoundEffects && hoverSound != null)
            {
                PlaySound(hoverSound);
            }
        }

        /// <summary>
        /// Called when pointer exits the button area.
        /// Resets to normal state.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;

            // Return to normal scale (coroutine-based)
            StartScaleAnimation(originalScale);

            // Return to normal color (coroutine-based)
            StartColorAnimation(normalColor);
        }

        /// <summary>
        /// Called when pointer is pressed down on button.
        /// Implements press feedback animation.
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (button == null || !button.interactable) return;

            // Scale down when pressed (coroutine-based)
            StartScaleAnimation(originalScale * pressedScale);

            // Darken color when pressed (coroutine-based)
            StartColorAnimation(pressedColor);

            // Trigger haptic feedback on mobile
            if (enableHapticFeedback)
            {
                TriggerHapticFeedback();
            }
        }

        /// <summary>
        /// Called when pointer is released from button.
        /// Returns to hover state if still hovering.
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            // Return to hover state if still hovering, else normal
            if (isHovering)
            {
                StartScaleAnimation(originalScale * hoverScale);
                StartColorAnimation(hoverColor);
            }
            else
            {
                StartScaleAnimation(originalScale);
                StartColorAnimation(normalColor);
            }
        }

        /// <summary>
        /// Called when button is clicked.
        /// Triggers additional feedback effects.
        /// </summary>
        private void OnButtonClicked()
        {
            // Play click sound
            if (enableSoundEffects && clickSound != null)
            {
                PlaySound(clickSound);
            }

            // Spawn click particles using object pooling to reduce GC pressure
            // TODO: [OPTIMIZATION] Integrate with ObjectPoolManager for particle effects
            // Current approach: Simple instantiate with timed destruction
            // Recommended: Use ObjectPool<ParticleSystem> for frequently clicked buttons
            if (enableParticleEffects && clickParticles != null)
            {
                var particles = Instantiate(clickParticles, transform.position, Quaternion.identity);
                Destroy(particles.gameObject, particles.main.duration + particles.main.startLifetime.constantMax);
            }

            // Pulse animation
            if (pulseAnimationCoroutine != null)
            {
                StopCoroutine(pulseAnimationCoroutine);
            }
            pulseAnimationCoroutine = StartCoroutine(PulseAnimation());
        }

        /// <summary>
        /// Quick pulse animation for satisfying click feedback.
        /// </summary>
        private IEnumerator PulseAnimation()
        {
            float duration = 0.2f;
            float elapsed = 0f;
            Vector3 pulseScale = originalScale * 1.1f;

            // Pulse out
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2f);
                transform.localScale = Vector3.Lerp(originalScale, pulseScale, t);
                yield return null;
            }

            // Pulse in
            elapsed = 0f;
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2f);
                transform.localScale = Vector3.Lerp(pulseScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// Plays a sound effect with the configured audio source.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip, soundVolume);
            }
        }

        /// <summary>
        /// Triggers haptic feedback on supported platforms (mobile).
        /// </summary>
        private void TriggerHapticFeedback()
        {
#if UNITY_ANDROID || UNITY_IOS
            // Light haptic feedback for button press
            Handheld.Vibrate();
#endif
        }

        /// <summary>
        /// Programmatically enables or disables the button.
        /// Updates visual state accordingly.
        /// </summary>
        /// <param name="interactable">Whether button should be interactable</param>
        public void SetInteractable(bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }

            if (!interactable)
            {
                StartColorAnimation(disabledColor);
                StartScaleAnimation(originalScale);
                isHovering = false;
            }
            else
            {
                StartColorAnimation(normalColor);
            }
        }

        /// <summary>
        /// Resets button to its original state.
        /// </summary>
        public void ResetState()
        {
            isHovering = false;
            targetScale = originalScale;
            targetColor = normalColor;
            transform.localScale = originalScale;

            if (targetGraphic != null)
            {
                targetGraphic.color = normalColor;
            }
        }

        private void OnDisable()
        {
            ResetState();
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClicked);
            }

            // Clean up all coroutines to prevent leaks
            if (scaleAnimationCoroutine != null)
            {
                StopCoroutine(scaleAnimationCoroutine);
            }
            if (colorAnimationCoroutine != null)
            {
                StopCoroutine(colorAnimationCoroutine);
            }
            if (pulseAnimationCoroutine != null)
            {
                StopCoroutine(pulseAnimationCoroutine);
            }
        }

#if UNITY_EDITOR
        // Editor helper to set colors from current graphic
        [ContextMenu("Set Colors From Current Graphic")]
        private void SetColorsFromGraphic()
        {
            if (targetGraphic != null)
            {
                normalColor = targetGraphic.color;
                hoverColor = normalColor * 1.1f;
                pressedColor = normalColor * 0.9f;
                disabledColor = new Color(normalColor.r, normalColor.g, normalColor.b, 0.5f);
                Debug.Log("Colors set from current graphic");
            }
        }
#endif
    }
}
