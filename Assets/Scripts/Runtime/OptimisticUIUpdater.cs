using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Implements optimistic UI updates for better perceived performance.
    /// Updates UI immediately while operations complete in background.
    /// Provides rollback mechanism if operations fail.
    /// </summary>
    public class OptimisticUIUpdater : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float numberCountDuration = 0.5f;
        [SerializeField] private AnimationCurve countCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Visual Feedback")]
        [SerializeField] private Color successColor = Color.green;
        [SerializeField] private Color errorColor = Color.red;
        [SerializeField] private float flashDuration = 0.3f;

        // Singleton instance
        private static OptimisticUIUpdater instance;
        public static OptimisticUIUpdater Instance => instance;

        // Track pending operations for rollback
        private readonly Dictionary<string, PendingOperation> pendingOperations = new Dictionary<string, PendingOperation>();

        /// <summary>
        /// Represents a pending optimistic update that can be rolled back.
        /// </summary>
        private class PendingOperation
        {
            public string OperationId { get; set; }
            public Action RollbackAction { get; set; }
            public float Timestamp { get; set; }

            public PendingOperation(string id, Action rollback)
            {
                OperationId = id;
                RollbackAction = rollback;
                Timestamp = Time.time;
            }
        }

        private void Awake()
        {
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
        /// Updates a number value with smooth counting animation.
        /// Provides optimistic immediate feedback while actual operation completes.
        /// </summary>
        /// <param name="textComponent">Text component to animate</param>
        /// <param name="currentValue">Current displayed value</param>
        /// <param name="newValue">Target value after operation</param>
        /// <param name="operationId">Unique ID for operation tracking</param>
        /// <param name="formatString">Optional format string (e.g., "N0" for whole numbers)</param>
        /// <param name="onComplete">Callback when animation completes</param>
        public void AnimateNumberChange(TextMeshProUGUI textComponent, int currentValue, int newValue,
            string operationId, string formatString = "N0", Action onComplete = null)
        {
            if (textComponent == null)
            {
                Debug.LogWarning("[OptimisticUIUpdater] Text component is null.");
                onComplete?.Invoke();
                return;
            }

            // Store rollback action
            int originalValue = currentValue;
            Action rollback = () =>
            {
                if (textComponent != null)
                {
                    textComponent.text = originalValue.ToString(formatString);
                }
            };

            RegisterPendingOperation(operationId, rollback);

            // Start counting animation
            StartCoroutine(CountToValue(textComponent, currentValue, newValue, formatString, () =>
            {
                onComplete?.Invoke();
            }));
        }

        /// <summary>
        /// Updates a float number with smooth animation.
        /// </summary>
        public void AnimateFloatChange(TextMeshProUGUI textComponent, float currentValue, float newValue,
            string operationId, string formatString = "F1", Action onComplete = null)
        {
            if (textComponent == null)
            {
                Debug.LogWarning("[OptimisticUIUpdater] Text component is null.");
                onComplete?.Invoke();
                return;
            }

            // Store rollback action
            float originalValue = currentValue;
            Action rollback = () =>
            {
                if (textComponent != null)
                {
                    textComponent.text = originalValue.ToString(formatString);
                }
            };

            RegisterPendingOperation(operationId, rollback);

            // Start counting animation
            StartCoroutine(CountToFloatValue(textComponent, currentValue, newValue, formatString, () =>
            {
                onComplete?.Invoke();
            }));
        }

        /// <summary>
        /// Immediately updates UI text optimistically.
        /// Useful for instant feedback before server confirmation.
        /// </summary>
        /// <param name="textComponent">Text component to update</param>
        /// <param name="newText">New text value</param>
        /// <param name="operationId">Unique operation ID</param>
        public void OptimisticTextUpdate(TextMeshProUGUI textComponent, string newText, string operationId)
        {
            if (textComponent == null)
            {
                Debug.LogWarning("[OptimisticUIUpdater] Text component is null.");
                return;
            }

            // Store original value for rollback
            string originalText = textComponent.text;
            Action rollback = () =>
            {
                if (textComponent != null)
                {
                    textComponent.text = originalText;
                }
            };

            RegisterPendingOperation(operationId, rollback);

            // Immediate update
            textComponent.text = newText;
        }

        /// <summary>
        /// Confirms an optimistic operation succeeded.
        /// Removes rollback capability for this operation.
        /// </summary>
        /// <param name="operationId">Operation ID to confirm</param>
        /// <param name="showSuccessFeedback">Whether to show visual success feedback</param>
        public void ConfirmOperation(string operationId, bool showSuccessFeedback = false)
        {
            if (pendingOperations.ContainsKey(operationId))
            {
                pendingOperations.Remove(operationId);
                
                if (showSuccessFeedback)
                {
                    Debug.Log($"[OptimisticUIUpdater] Operation confirmed: {operationId}");
                }
            }
        }

        /// <summary>
        /// Rolls back an optimistic operation that failed.
        /// Restores UI to pre-operation state.
        /// </summary>
        /// <param name="operationId">Operation ID to rollback</param>
        /// <param name="showErrorFeedback">Whether to show visual error feedback</param>
        public void RollbackOperation(string operationId, bool showErrorFeedback = true)
        {
            if (pendingOperations.TryGetValue(operationId, out var operation))
            {
                // Execute rollback
                operation.RollbackAction?.Invoke();
                pendingOperations.Remove(operationId);

                if (showErrorFeedback)
                {
                    Debug.LogWarning($"[OptimisticUIUpdater] Operation rolled back: {operationId}");
                }
            }
        }

        /// <summary>
        /// Flashes a UI element with a color to indicate success or error.
        /// </summary>
        /// <param name="graphic">UI graphic to flash</param>
        /// <param name="isSuccess">True for success (green), false for error (red)</param>
        public void FlashFeedback(UnityEngine.UI.Graphic graphic, bool isSuccess = true)
        {
            if (graphic == null) return;

            Color flashColor = isSuccess ? successColor : errorColor;
            StartCoroutine(FlashColorCoroutine(graphic, flashColor));
        }

        /// <summary>
        /// Scales a UI element briefly for feedback.
        /// </summary>
        /// <param name="transform">Transform to scale</param>
        public void PopFeedback(Transform transform)
        {
            if (transform == null) return;

            StartCoroutine(PopScaleCoroutine(transform));
        }

        private void RegisterPendingOperation(string operationId, Action rollback)
        {
            // Remove if already exists (new operation replaces old)
            if (pendingOperations.ContainsKey(operationId))
            {
                pendingOperations.Remove(operationId);
            }

            pendingOperations[operationId] = new PendingOperation(operationId, rollback);
        }

        private IEnumerator CountToValue(TextMeshProUGUI textComponent, int startValue, int endValue, 
            string formatString, Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < numberCountDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / numberCountDuration);
                float curveValue = countCurve.Evaluate(t);

                int currentValue = (int)Mathf.Lerp(startValue, endValue, curveValue);
                textComponent.text = currentValue.ToString(formatString);

                yield return null;
            }

            // Ensure final value is exact
            textComponent.text = endValue.ToString(formatString);
            onComplete?.Invoke();
        }

        private IEnumerator CountToFloatValue(TextMeshProUGUI textComponent, float startValue, float endValue,
            string formatString, Action onComplete)
        {
            float elapsed = 0f;

            while (elapsed < numberCountDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / numberCountDuration);
                float curveValue = countCurve.Evaluate(t);

                float currentValue = Mathf.Lerp(startValue, endValue, curveValue);
                textComponent.text = currentValue.ToString(formatString);

                yield return null;
            }

            // Ensure final value is exact
            textComponent.text = endValue.ToString(formatString);
            onComplete?.Invoke();
        }

        private IEnumerator FlashColorCoroutine(UnityEngine.UI.Graphic graphic, Color flashColor)
        {
            Color originalColor = graphic.color;
            float elapsed = 0f;
            float halfDuration = flashDuration / 2f;

            // Flash to color
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                graphic.color = Color.Lerp(originalColor, flashColor, t);
                yield return null;
            }

            // Flash back to original
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / halfDuration;
                graphic.color = Color.Lerp(flashColor, originalColor, t);
                yield return null;
            }

            graphic.color = originalColor;
        }

        private IEnumerator PopScaleCoroutine(Transform transform)
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;
            float duration = 0.3f;
            float elapsed = 0f;

            // Scale up
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2f);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Scale back
            elapsed = 0f;
            while (elapsed < duration / 2f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2f);
                transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// Cleans up old pending operations that never completed.
        /// Call periodically to prevent memory leaks.
        /// </summary>
        public void CleanupStalePendingOperations(float maxAge = 60f)
        {
            var staleOperations = new List<string>();
            float currentTime = Time.time;

            foreach (var kvp in pendingOperations)
            {
                if (currentTime - kvp.Value.Timestamp > maxAge)
                {
                    staleOperations.Add(kvp.Key);
                }
            }

            foreach (var operationId in staleOperations)
            {
                Debug.LogWarning($"[OptimisticUIUpdater] Cleaning up stale operation: {operationId}");
                pendingOperations.Remove(operationId);
            }
        }

        private void Update()
        {
            // Periodic cleanup every 30 seconds
            if (Time.frameCount % (30 * 60) == 0) // Assuming 60 FPS
            {
                CleanupStalePendingOperations();
            }
        }

        private void OnDestroy()
        {
            pendingOperations.Clear();
        }
    }
}
