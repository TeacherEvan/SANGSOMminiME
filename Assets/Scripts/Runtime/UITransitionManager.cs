using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Manages smooth UI panel transitions with fade, slide, and scale effects.
    /// Implements modern UX patterns for visually pleasing navigation.
    /// Optimized for Unity 2022.3 LTS with pooling and caching.
    /// </summary>
    public class UITransitionManager : MonoBehaviour
    {
        /// <summary>
        /// Types of transition animations available.
        /// </summary>
        public enum TransitionType
        {
            Fade,
            SlideLeft,
            SlideRight,
            SlideUp,
            SlideDown,
            ScaleUp,
            ScaleDown,
            FadeAndSlide
        }

        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 0.3f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Slide Distance")]
        [SerializeField] private float slideDistance = 100f;

        // Singleton instance
        private static UITransitionManager instance;
        public static UITransitionManager Instance => instance;

        // Track active transitions to prevent conflicts
        private Dictionary<GameObject, Coroutine> activeTransitions = new Dictionary<GameObject, Coroutine>();

        // Cached components for performance
        private Dictionary<GameObject, CanvasGroup> cachedCanvasGroups = new Dictionary<GameObject, CanvasGroup>();

        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Transitions out one panel and transitions in another with smooth animation.
        /// Implements optimistic UI updates for better perceived performance.
        /// </summary>
        /// <param name="outPanel">Panel to hide</param>
        /// <param name="inPanel">Panel to show</param>
        /// <param name="transitionType">Type of transition animation</param>
        /// <param name="onComplete">Optional callback when transition completes</param>
        public void TransitionPanels(GameObject outPanel, GameObject inPanel, 
            TransitionType transitionType = TransitionType.Fade, Action onComplete = null)
        {
            if (outPanel == null && inPanel == null)
            {
                Debug.LogWarning("[UITransitionManager] Both panels are null.");
                onComplete?.Invoke();
                return;
            }

            StartCoroutine(TransitionPanelsCoroutine(outPanel, inPanel, transitionType, onComplete));
        }

        /// <summary>
        /// Shows a panel with transition animation.
        /// </summary>
        public void ShowPanel(GameObject panel, TransitionType transitionType = TransitionType.Fade, Action onComplete = null)
        {
            if (panel == null)
            {
                Debug.LogWarning("[UITransitionManager] Panel is null.");
                onComplete?.Invoke();
                return;
            }

            StopTransition(panel);
            var coroutine = StartCoroutine(ShowPanelCoroutine(panel, transitionType, onComplete));
            activeTransitions[panel] = coroutine;
        }

        /// <summary>
        /// Hides a panel with transition animation.
        /// </summary>
        public void HidePanel(GameObject panel, TransitionType transitionType = TransitionType.Fade, Action onComplete = null)
        {
            if (panel == null)
            {
                Debug.LogWarning("[UITransitionManager] Panel is null.");
                onComplete?.Invoke();
                return;
            }

            StopTransition(panel);
            var coroutine = StartCoroutine(HidePanelCoroutine(panel, transitionType, onComplete));
            activeTransitions[panel] = coroutine;
        }

        private IEnumerator TransitionPanelsCoroutine(GameObject outPanel, GameObject inPanel, 
            TransitionType transitionType, Action onComplete)
        {
            // Hide outgoing panel
            if (outPanel != null)
            {
                yield return StartCoroutine(HidePanelCoroutine(outPanel, transitionType, null));
            }

            // Small delay between transitions for better UX
            yield return new WaitForSeconds(0.05f);

            // Show incoming panel
            if (inPanel != null)
            {
                yield return StartCoroutine(ShowPanelCoroutine(inPanel, transitionType, null));
            }

            onComplete?.Invoke();
        }

        private IEnumerator ShowPanelCoroutine(GameObject panel, TransitionType transitionType, Action onComplete)
        {
            // Ensure panel is active
            panel.SetActive(true);

            // Get or create CanvasGroup for fade effects
            var canvasGroup = GetOrCreateCanvasGroup(panel);
            var rectTransform = panel.GetComponent<RectTransform>();

            // Store original values
            Vector2 originalPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
            Vector3 originalScale = panel.transform.localScale;

            // Set initial state based on transition type
            SetInitialStateForShow(panel, canvasGroup, rectTransform, transitionType, originalPosition, originalScale);

            // Animate to visible state
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionDuration);
                float curveValue = transitionCurve.Evaluate(t);

                AnimateShow(panel, canvasGroup, rectTransform, transitionType, curveValue, originalPosition, originalScale);

                yield return null;
            }

            // Ensure final state
            SetFinalStateForShow(panel, canvasGroup, rectTransform, originalPosition, originalScale);

            // Remove from active transitions
            if (activeTransitions.ContainsKey(panel))
                activeTransitions.Remove(panel);

            onComplete?.Invoke();
        }

        private IEnumerator HidePanelCoroutine(GameObject panel, TransitionType transitionType, Action onComplete)
        {
            var canvasGroup = GetOrCreateCanvasGroup(panel);
            var rectTransform = panel.GetComponent<RectTransform>();

            // Store original values
            Vector2 originalPosition = rectTransform != null ? rectTransform.anchoredPosition : Vector2.zero;
            Vector3 originalScale = panel.transform.localScale;

            // Animate to hidden state
            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionDuration);
                float curveValue = transitionCurve.Evaluate(t);

                AnimateHide(panel, canvasGroup, rectTransform, transitionType, curveValue, originalPosition, originalScale);

                yield return null;
            }

            // Set final hidden state
            panel.SetActive(false);
            SetFinalStateForShow(panel, canvasGroup, rectTransform, originalPosition, originalScale);

            // Remove from active transitions
            if (activeTransitions.ContainsKey(panel))
                activeTransitions.Remove(panel);

            onComplete?.Invoke();
        }

        private void SetInitialStateForShow(GameObject panel, CanvasGroup canvasGroup, RectTransform rectTransform,
            TransitionType transitionType, Vector2 originalPosition, Vector3 originalScale)
        {
            switch (transitionType)
            {
                case TransitionType.Fade:
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.SlideLeft:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = originalPosition + new Vector2(slideDistance, 0f);
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.SlideRight:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = originalPosition + new Vector2(-slideDistance, 0f);
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.SlideUp:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = originalPosition + new Vector2(0f, -slideDistance);
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.SlideDown:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = originalPosition + new Vector2(0f, slideDistance);
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.ScaleUp:
                    panel.transform.localScale = Vector3.zero;
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.ScaleDown:
                    panel.transform.localScale = originalScale * 1.2f;
                    canvasGroup.alpha = 0f;
                    break;

                case TransitionType.FadeAndSlide:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = originalPosition + new Vector2(0f, -slideDistance * 0.5f);
                    canvasGroup.alpha = 0f;
                    break;
            }
        }

        private void AnimateShow(GameObject panel, CanvasGroup canvasGroup, RectTransform rectTransform,
            TransitionType transitionType, float t, Vector2 originalPosition, Vector3 originalScale)
        {
            switch (transitionType)
            {
                case TransitionType.Fade:
                    canvasGroup.alpha = t;
                    break;

                case TransitionType.SlideLeft:
                case TransitionType.SlideRight:
                case TransitionType.SlideUp:
                case TransitionType.SlideDown:
                case TransitionType.FadeAndSlide:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, originalPosition, t);
                    canvasGroup.alpha = t;
                    break;

                case TransitionType.ScaleUp:
                case TransitionType.ScaleDown:
                    panel.transform.localScale = Vector3.Lerp(panel.transform.localScale, originalScale, t);
                    canvasGroup.alpha = t;
                    break;
            }
        }

        private void AnimateHide(GameObject panel, CanvasGroup canvasGroup, RectTransform rectTransform,
            TransitionType transitionType, float t, Vector2 originalPosition, Vector3 originalScale)
        {
            float reverseT = 1f - t;

            switch (transitionType)
            {
                case TransitionType.Fade:
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.SlideLeft:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + new Vector2(-slideDistance, 0f), t);
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.SlideRight:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + new Vector2(slideDistance, 0f), t);
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.SlideUp:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + new Vector2(0f, slideDistance), t);
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.SlideDown:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + new Vector2(0f, -slideDistance), t);
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.ScaleUp:
                    panel.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t);
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.ScaleDown:
                    panel.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                    canvasGroup.alpha = reverseT;
                    break;

                case TransitionType.FadeAndSlide:
                    if (rectTransform != null)
                        rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, originalPosition + new Vector2(0f, slideDistance * 0.5f), t);
                    canvasGroup.alpha = reverseT;
                    break;
            }
        }

        private void SetFinalStateForShow(GameObject panel, CanvasGroup canvasGroup, RectTransform rectTransform,
            Vector2 originalPosition, Vector3 originalScale)
        {
            canvasGroup.alpha = 1f;
            if (rectTransform != null)
                rectTransform.anchoredPosition = originalPosition;
            panel.transform.localScale = originalScale;
        }

        private void StopTransition(GameObject panel)
        {
            if (activeTransitions.TryGetValue(panel, out var coroutine))
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
                activeTransitions.Remove(panel);
            }
        }

        private CanvasGroup GetOrCreateCanvasGroup(GameObject panel)
        {
            // Check cache first for performance
            if (cachedCanvasGroups.TryGetValue(panel, out var cachedGroup) && cachedGroup != null)
                return cachedGroup;

            // Get or create CanvasGroup
            var canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = panel.AddComponent<CanvasGroup>();

            // Cache for future use
            cachedCanvasGroups[panel] = canvasGroup;

            return canvasGroup;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            activeTransitions.Clear();
            cachedCanvasGroups.Clear();
        }
    }
}
