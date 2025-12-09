using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Adds professional hover micro-interactions to UI buttons.
    /// Implements modern UX patterns with smooth scale and color transitions.
    /// Optimized for Unity 2022.3 LTS with minimal allocations.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Hover Settings")]
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float hoverDuration = 0.15f;
        [SerializeField] private bool enableColorChange = true;
        [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 1f);
        
        [Header("Animation Curve")]
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        private Button button;
        private Image buttonImage;
        private Vector3 originalScale;
        private Color originalColor;
        private Coroutine hoverCoroutine;
        private bool isHovering = false;
        
        private void Awake()
        {
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            button = GetComponent<Button>();
            buttonImage = GetComponent<Image>();
            originalScale = transform.localScale;
            
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }
        }
        
        /// <summary>
        /// Handles pointer enter event with smooth scale-up animation.
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            // Only animate if button is interactable
            if (button != null && !button.interactable) return;
            
            isHovering = true;
            
            // Stop any existing animation
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
            }
            
            // Start hover animation
            hoverCoroutine = StartCoroutine(AnimateHover(true));
        }
        
        /// <summary>
        /// Handles pointer exit event with smooth scale-down animation.
        /// </summary>
        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
            
            // Stop any existing animation
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
            }
            
            // Start return animation
            hoverCoroutine = StartCoroutine(AnimateHover(false));
        }
        
        /// <summary>
        /// Animates hover state transition with smooth easing.
        /// </summary>
        /// <param name="hovering">True for hover in, false for hover out</param>
        private IEnumerator AnimateHover(bool hovering)
        {
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = hovering ? originalScale * hoverScale : originalScale;
            
            Color startColor = buttonImage != null ? buttonImage.color : Color.white;
            Color targetColor = hovering && enableColorChange ? hoverColor : originalColor;
            
            float elapsed = 0f;
            
            while (elapsed < hoverDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / hoverDuration);
                
                // Apply animation curve for smooth transition
                float curveValue = scaleCurve.Evaluate(t);
                
                // Animate scale
                transform.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
                
                // Animate color if enabled
                if (enableColorChange && buttonImage != null)
                {
                    buttonImage.color = Color.Lerp(startColor, targetColor, curveValue);
                }
                
                yield return null;
            }
            
            // Ensure final state is exact
            transform.localScale = targetScale;
            if (enableColorChange && buttonImage != null)
            {
                buttonImage.color = targetColor;
            }
        }
        
        /// <summary>
        /// Resets the button to its original state.
        /// Call this when button becomes disabled or before destruction.
        /// </summary>
        public void ResetToOriginalState()
        {
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
            }
            
            transform.localScale = originalScale;
            
            if (buttonImage != null)
            {
                buttonImage.color = originalColor;
            }
            
            isHovering = false;
        }
        
        private void OnDisable()
        {
            // Reset state when button is disabled
            ResetToOriginalState();
        }
        
        private void OnDestroy()
        {
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
            }
        }
    }
}
