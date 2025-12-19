using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Handles coin collection animations with pooling for performance.
    /// Creates "juice" with flying coins, scale punches, and audio feedback.
    /// Optimized for 60 FPS with coroutine-based tweening.
    /// </summary>
    public class CoinAnimationController : MonoBehaviour
    {
        [Header("Prefab References")]
        [SerializeField] private GameObject coinSpritePrefab;
        [SerializeField] private Transform coinUITarget; // Where coins fly to

        [Header("Animation Settings")]
        [SerializeField] private float flightDuration = 0.6f;
        [SerializeField] private float scaleUpDuration = 0.15f;
        [SerializeField] private float delayBetweenCoins = 0.05f;
        [SerializeField] private AnimationCurve flightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private Vector2 spawnOffsetRange = new Vector2(30f, 50f);

        [Header("Visual Effects")]
        [SerializeField] private float scaleUpMultiplier = 1.3f;
        [SerializeField] private float rotationSpeed = 360f;

        // Singleton instance
        private static CoinAnimationController instance;
        public static CoinAnimationController Instance => instance;

        // Object pool for coin sprites (reusing existing ObjectPool pattern)
        private Core.ObjectPool<RectTransform> coinPool;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeCoinPool();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initializes the coin sprite pool with reasonable capacity.
        /// Pre-warms pool to avoid frame drops during first animation.
        /// </summary>
        private void InitializeCoinPool()
        {
            if (coinSpritePrefab == null)
            {
                Debug.LogWarning("[CoinAnimationController] coinSpritePrefab not assigned. Animations will not work.");
                return;
            }

            var prefabRectTransform = coinSpritePrefab.GetComponent<RectTransform>();
            if (prefabRectTransform == null)
            {
                Debug.LogWarning("[CoinAnimationController] coinSpritePrefab is missing a RectTransform. Animations will not work.");
                return;
            }

            coinPool = new Core.ObjectPool<RectTransform>(
                prefabRectTransform,
                initialCapacity: 10,
                maxCapacity: 30,
                expandable: true,
                poolParent: transform
            );

            // Pre-warm pool to avoid first-frame lag
            for (int i = 0; i < 5; i++)
            {
                var warmup = coinPool.Get();
                coinPool.Return(warmup);
            }
        }

        /// <summary>
        /// Plays coin collection animation with specified amount.
        /// Spawns multiple coins that fly to UI target with staggered timing.
        /// </summary>
        /// <param name="amount">Number of coins to animate (clamped to reasonable max)</param>
        /// <param name="spawnPosition">World position where coins spawn from</param>
        /// <param name="onComplete">Callback when all coins reach target</param>
        public void PlayCoinCollectAnimation(int amount, Vector3 spawnPosition, System.Action onComplete = null)
        {
            if (coinPool == null || coinSpritePrefab == null)
            {
                Debug.LogWarning("[CoinAnimationController] Pool not initialized. Skipping animation.");
                onComplete?.Invoke();
                return;
            }

            if (coinUITarget == null)
            {
                Debug.LogWarning("[CoinAnimationController] coinUITarget not assigned. Skipping animation.");
                onComplete?.Invoke();
                return;
            }

            // Clamp to reasonable max for performance (avoid 100+ coins flying)
            int coinsToAnimate = Mathf.Clamp(amount / 5, 1, 15); // Show 1 coin per 5 currency, max 15

            StartCoroutine(SpawnCoinSequence(coinsToAnimate, spawnPosition, onComplete));
        }

        /// <summary>
        /// Staggered coin spawn coroutine for smooth animation.
        /// </summary>
        private IEnumerator SpawnCoinSequence(int count, Vector3 spawnPosition, System.Action onComplete)
        {
            int coinsReached = 0;

            for (int i = 0; i < count; i++)
            {
                var coin = coinPool.Get();

                // Add random offset for natural spread
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnOffsetRange.x, spawnOffsetRange.x),
                    Random.Range(-spawnOffsetRange.y, spawnOffsetRange.y),
                    0f
                );

                coin.transform.position = spawnPosition + randomOffset;
                coin.transform.localScale = Vector3.zero;

                // Start individual coin flight (fire-and-forget)
                StartCoroutine(AnimateSingleCoin(coin, () =>
                {
                    coinsReached++;
                    if (coinsReached >= count)
                    {
                        // All coins reached target - trigger UI update
                        onComplete?.Invoke();
                        PunchCoinUIScale();
                    }
                }));

                yield return new WaitForSeconds(delayBetweenCoins);
            }
        }

        /// <summary>
        /// Animates single coin from spawn to target with scale-up and flight.
        /// </summary>
        private IEnumerator AnimateSingleCoin(RectTransform coin, System.Action onReachTarget)
        {
            Transform coinTransform = coin.transform;
            Vector3 startPos = coinTransform.position;
            Vector3 targetPos = coinUITarget.position;

            // Phase 1: Scale up from zero
            float elapsed = 0f;
            while (elapsed < scaleUpDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / scaleUpDuration;
                coinTransform.localScale = Vector3.one * Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            coinTransform.localScale = Vector3.one;

            // Phase 2: Fly to target with rotation
            elapsed = 0f;
            while (elapsed < flightDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / flightDuration;

                // Smooth position interpolation with curve
                float curveT = flightCurve.Evaluate(t);
                coinTransform.position = Vector3.Lerp(startPos, targetPos, curveT);

                // Rotate for visual interest
                coinTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

                // Shrink slightly as it approaches target
                coinTransform.localScale = Vector3.one * Mathf.Lerp(1f, 0.3f, curveT);

                yield return null;
            }

            // Phase 3: Cleanup
            coinPool.Return(coin);
            onReachTarget?.Invoke();
        }

        /// <summary>
        /// Adds satisfying "punch" scale animation to coin UI element.
        /// Creates tactile feedback when coins arrive.
        /// </summary>
        private void PunchCoinUIScale()
        {
            if (coinUITarget != null)
            {
                StartCoroutine(PunchScaleCoroutine(coinUITarget));
            }
        }

        /// <summary>
        /// Simple scale punch animation with elastic overshoot.
        /// </summary>
        private IEnumerator PunchScaleCoroutine(Transform target)
        {
            Vector3 originalScale = target.localScale;
            Vector3 punchScale = originalScale * scaleUpMultiplier;

            float duration = 0.2f;
            float elapsed = 0f;

            // Scale up quickly
            while (elapsed < duration / 2)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration / 2);
                target.localScale = Vector3.Lerp(originalScale, punchScale, t);
                yield return null;
            }

            // Scale down with slight overshoot (elastic feel)
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Use elastic easing for bounce-back
                float elasticT = 1f - Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.1f) * 5f * Mathf.PI);
                target.localScale = Vector3.Lerp(punchScale, originalScale, elasticT);
                yield return null;
            }

            target.localScale = originalScale;
        }

        /// <summary>
        /// Plays a simple coin spawn effect at world position (no flight).
        /// Useful for instant feedback on smaller rewards.
        /// </summary>
        public void PlayCoinSpawnEffect(Vector3 worldPosition)
        {
            if (coinPool == null || coinSpritePrefab == null) return;

            var coin = coinPool.Get();
            coin.transform.position = worldPosition;
            StartCoroutine(SpawnEffectCoroutine(coin));
        }

        private IEnumerator SpawnEffectCoroutine(RectTransform coin)
        {
            Transform t = coin.transform;
            Vector3 startPos = t.position;
            t.localScale = Vector3.zero;

            // Quick scale up and fade out
            float duration = 0.4f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                t.localScale = Vector3.one * (1f - progress) * 1.5f;
                t.position = startPos + Vector3.up * (progress * 50f); // Float upward

                // Fade out sprite if it has CanvasGroup or Image
                var canvasGroup = coin.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - progress;
                }

                yield return null;
            }

            coinPool.Return(coin);
        }

        private void OnDestroy()
        {
            // Clean up pool on destroy
            coinPool?.Clear();
            coinPool = null;
        }
    }
}
