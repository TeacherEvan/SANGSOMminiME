using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Manages loading states with skeleton screens and smooth transitions.
    /// Implements modern UX patterns for perceived performance optimization.
    /// Follows Unity 2022.3 LTS best practices for UI management.
    /// </summary>
    public class UILoadingState : MonoBehaviour
    {
        [Header("Loading Skeleton Components")]
        [SerializeField] private GameObject skeletonContainer;
        [SerializeField] private CanvasGroup skeletonCanvasGroup;
        [SerializeField] private Image[] skeletonShimmers;

        [Header("Loading Spinner")]
        [SerializeField] private GameObject spinnerContainer;
        [SerializeField] private Image spinnerImage;
        [SerializeField] private TextMeshProUGUI loadingText;

        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private float shimmerSpeed = 1.5f;
        [SerializeField] private float spinnerRotationSpeed = 360f;

        [Header("Shimmer Effect Settings")]
        [SerializeField] private Color shimmerColorStart = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        [SerializeField] private Color shimmerColorEnd = new Color(1f, 1f, 1f, 0.6f);

        private Coroutine currentAnimationCoroutine;
        private Coroutine shimmerCoroutine;
        private Coroutine spinnerCoroutine;
        private bool isShowing = false;

        private void Awake()
        {
            ValidateReferences();
            HideImmediate();
        }

        private void ValidateReferences()
        {
            if (skeletonContainer == null)
                Debug.LogWarning("[UILoadingState] Skeleton container not assigned.");

            if (skeletonCanvasGroup == null && skeletonContainer != null)
            {
                skeletonCanvasGroup = skeletonContainer.GetComponent<CanvasGroup>();
                if (skeletonCanvasGroup == null)
                    skeletonCanvasGroup = skeletonContainer.AddComponent<CanvasGroup>();
            }

            if (spinnerImage != null && spinnerContainer == null)
                spinnerContainer = spinnerImage.gameObject;
        }

        /// <summary>
        /// Shows loading state with skeleton screen or spinner.
        /// Implements smooth fade-in animation for better UX.
        /// </summary>
        /// <param name="useSkeleton">Use skeleton screen (true) or spinner (false)</param>
        /// <param name="message">Optional loading message to display</param>
        public void Show(bool useSkeleton = true, string message = "Loading...")
        {
            if (isShowing) return;

            isShowing = true;

            // Stop any existing animations
            StopAllCoroutines();

            // Determine which loading visual to show
            if (useSkeleton && skeletonContainer != null)
            {
                ShowSkeletonScreen(message);
            }
            else if (spinnerContainer != null)
            {
                ShowSpinner(message);
            }
            else
            {
                Debug.LogWarning("[UILoadingState] No loading visuals configured.");
            }
        }

        /// <summary>
        /// Hides loading state with smooth fade-out animation.
        /// </summary>
        /// <param name="delay">Optional delay before hiding (for minimum display time)</param>
        public void Hide(float delay = 0f)
        {
            if (!isShowing) return;

            if (delay > 0f)
            {
                StartCoroutine(HideAfterDelay(delay));
            }
            else
            {
                PerformHide();
            }
        }

        private void ShowSkeletonScreen(string message)
        {
            // Activate skeleton container
            if (skeletonContainer != null)
            {
                skeletonContainer.SetActive(true);
                if (skeletonCanvasGroup != null)
                {
                    currentAnimationCoroutine = StartCoroutine(FadeCanvasGroup(skeletonCanvasGroup, 0f, 1f, fadeInDuration));
                }
            }

            // Deactivate spinner
            if (spinnerContainer != null)
                spinnerContainer.SetActive(false);

            // Update loading text
            UpdateLoadingText(message);

            // Start shimmer animation
            if (skeletonShimmers != null && skeletonShimmers.Length > 0)
            {
                shimmerCoroutine = StartCoroutine(AnimateShimmer());
            }
        }

        private void ShowSpinner(string message)
        {
            // Deactivate skeleton
            if (skeletonContainer != null)
                skeletonContainer.SetActive(false);

            // Activate spinner
            if (spinnerContainer != null)
            {
                spinnerContainer.SetActive(true);
                spinnerCoroutine = StartCoroutine(AnimateSpinner());
            }

            // Update loading text
            UpdateLoadingText(message);
        }

        private void PerformHide()
        {
            isShowing = false;

            // Stop animations
            if (shimmerCoroutine != null)
            {
                StopCoroutine(shimmerCoroutine);
                shimmerCoroutine = null;
            }

            if (spinnerCoroutine != null)
            {
                StopCoroutine(spinnerCoroutine);
                spinnerCoroutine = null;
            }

            // Fade out skeleton
            if (skeletonContainer != null && skeletonCanvasGroup != null)
            {
                currentAnimationCoroutine = StartCoroutine(FadeCanvasGroupAndDeactivate(
                    skeletonCanvasGroup, 1f, 0f, fadeOutDuration, skeletonContainer));
            }

            // Fade out spinner
            if (spinnerContainer != null)
            {
                var spinnerCanvasGroup = spinnerContainer.GetComponent<CanvasGroup>();
                if (spinnerCanvasGroup == null)
                    spinnerCanvasGroup = spinnerContainer.AddComponent<CanvasGroup>();

                StartCoroutine(FadeCanvasGroupAndDeactivate(
                    spinnerCanvasGroup, 1f, 0f, fadeOutDuration, spinnerContainer));
            }
        }

        private void HideImmediate()
        {
            isShowing = false;

            if (skeletonContainer != null)
            {
                skeletonContainer.SetActive(false);
                if (skeletonCanvasGroup != null)
                    skeletonCanvasGroup.alpha = 0f;
            }

            if (spinnerContainer != null)
                spinnerContainer.SetActive(false);
        }

        private void UpdateLoadingText(string message)
        {
            if (loadingText != null && !string.IsNullOrEmpty(message))
            {
                loadingText.text = message;
            }
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            PerformHide();
        }

        /// <summary>
        /// Smoothly fades a CanvasGroup alpha value.
        /// </summary>
        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
        {
            if (canvasGroup == null) yield break;

            canvasGroup.alpha = startAlpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // Use smooth easing curve
                t = t * t * (3f - 2f * t); // Smoothstep
                canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = endAlpha;
        }

        /// <summary>
        /// Fades a CanvasGroup and deactivates the GameObject when done.
        /// </summary>
        private IEnumerator FadeCanvasGroupAndDeactivate(CanvasGroup canvasGroup, float startAlpha, 
            float endAlpha, float duration, GameObject target)
        {
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, startAlpha, endAlpha, duration));
            
            if (target != null)
                target.SetActive(false);
        }

        /// <summary>
        /// Animates skeleton shimmer effect for visual polish.
        /// Creates a subtle "breathing" animation on skeleton elements.
        /// </summary>
        private IEnumerator AnimateShimmer()
        {
            if (skeletonShimmers == null || skeletonShimmers.Length == 0)
                yield break;

            float time = 0f;

            while (isShowing)
            {
                time += Time.deltaTime * shimmerSpeed;
                float t = Mathf.PingPong(time, 1f);
                // Apply smooth easing
                t = t * t * (3f - 2f * t);

                Color shimmerColor = Color.Lerp(shimmerColorStart, shimmerColorEnd, t);

                foreach (var shimmer in skeletonShimmers)
                {
                    if (shimmer != null)
                        shimmer.color = shimmerColor;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Rotates the spinner for loading indication.
        /// </summary>
        private IEnumerator AnimateSpinner()
        {
            if (spinnerImage == null) yield break;

            while (isShowing)
            {
                spinnerImage.transform.Rotate(0f, 0f, -spinnerRotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        /// <summary>
        /// Shows loading with automatic minimum display time.
        /// Prevents flashing for very fast operations.
        /// </summary>
        /// <param name="useSkeleton">Use skeleton screen or spinner</param>
        /// <param name="message">Loading message</param>
        /// <param name="minimumDisplayTime">Minimum time to display (prevents flashing)</param>
        public void ShowWithMinimumTime(bool useSkeleton = true, string message = "Loading...", float minimumDisplayTime = 0.5f)
        {
            Show(useSkeleton, message);
            StartCoroutine(EnforceMinimumDisplayTime(minimumDisplayTime));
        }

        private IEnumerator EnforceMinimumDisplayTime(float minimumTime)
        {
            float startTime = Time.time;
            yield return new WaitUntil(() => !isShowing || (Time.time - startTime) >= minimumTime);
            
            if (isShowing && (Time.time - startTime) >= minimumTime)
            {
                // Already displayed long enough, do nothing
                // The caller will call Hide() when ready
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
