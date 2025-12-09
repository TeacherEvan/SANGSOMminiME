using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SangsomMiniMe.UI
{
    /// <summary>
    /// Manages reward celebration particles and visual effects.
    /// Implements object pooling for performance and smooth animations for premium feel.
    /// Optimized for Unity 2022.3 LTS with minimal GC allocations.
    /// </summary>
    public class UIRewardEffects : MonoBehaviour
    {
        [Header("Particle Systems")]
        [SerializeField] private ParticleSystem coinBurstPrefab;
        [SerializeField] private ParticleSystem starBurstPrefab;
        [SerializeField] private ParticleSystem confettiPrefab;
        [SerializeField] private ParticleSystem glowPrefab;
        
        [Header("Effect Settings")]
        [SerializeField] private int poolSize = 5;
        [SerializeField] private Transform effectsParent;
        [SerializeField] private bool enableSoundEffects = true;
        [SerializeField] private AudioClip coinSound;
        [SerializeField] private AudioClip levelUpSound;
        [SerializeField] private AudioClip achievementSound;
        
        [Header("Animation Settings")]
        [SerializeField] private float coinAnimationDuration = 1f;
        [SerializeField] private AnimationCurve coinFlightCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        // Object pools for particles
        private Core.ObjectPool<ParticleSystem> coinBurstPool;
        private Core.ObjectPool<ParticleSystem> starBurstPool;
        private Core.ObjectPool<ParticleSystem> confettiPool;
        private Core.ObjectPool<ParticleSystem> glowPool;
        
        // Audio source for sound effects
        private AudioSource audioSource;
        
        // Singleton instance
        private static UIRewardEffects instance;
        public static UIRewardEffects Instance => instance;
        
        private void Awake()
        {
            // Singleton pattern
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeEffectSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initializes particle pools and audio system.
        /// </summary>
        private void InitializeEffectSystem()
        {
            // Set up effects parent if not assigned
            if (effectsParent == null)
            {
                var parentObj = new GameObject("RewardEffectsParent");
                effectsParent = parentObj.transform;
                effectsParent.SetParent(transform);
            }
            
            // Initialize object pools
            InitializeParticlePools();
            
            // Set up audio source
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
            
            Debug.Log("[UIRewardEffects] Reward effects system initialized.");
        }
        
        /// <summary>
        /// Initializes particle system object pools for performance.
        /// </summary>
        private void InitializeParticlePools()
        {
            if (coinBurstPrefab != null)
            {
                coinBurstPool = new Core.ObjectPool<ParticleSystem>(
                    coinBurstPrefab, 
                    poolSize, 
                    maxCapacity: poolSize * 2, 
                    expandable: true, 
                    poolParent: effectsParent
                );
            }
            
            if (starBurstPrefab != null)
            {
                starBurstPool = new Core.ObjectPool<ParticleSystem>(
                    starBurstPrefab, 
                    poolSize, 
                    maxCapacity: poolSize * 2, 
                    expandable: true, 
                    poolParent: effectsParent
                );
            }
            
            if (confettiPrefab != null)
            {
                confettiPool = new Core.ObjectPool<ParticleSystem>(
                    confettiPrefab, 
                    poolSize, 
                    maxCapacity: poolSize * 2, 
                    expandable: true, 
                    poolParent: effectsParent
                );
            }
            
            if (glowPrefab != null)
            {
                glowPool = new Core.ObjectPool<ParticleSystem>(
                    glowPrefab, 
                    poolSize, 
                    maxCapacity: poolSize * 2, 
                    expandable: true, 
                    poolParent: effectsParent
                );
            }
        }
        
        /// <summary>
        /// Plays a coin reward effect with particle burst and optional sound.
        /// Implements micro-interaction pattern for currency increases.
        /// </summary>
        /// <param name="worldPosition">World position to spawn effect</param>
        /// <param name="coinAmount">Amount of coins earned (affects particle count)</param>
        public void PlayCoinRewardEffect(Vector3 worldPosition, int coinAmount = 1)
        {
            if (coinBurstPool == null) return;
            
            // Get particle system from pool
            var particles = coinBurstPool.Get();
            if (particles == null) return;
            
            // Position at world location
            particles.transform.position = worldPosition;
            
            // Scale emission based on coin amount (more coins = more particles)
            var emission = particles.emission;
            var burst = emission.GetBurst(0);
            burst.count = Mathf.Clamp(coinAmount * 3, 5, 50);
            emission.SetBurst(0, burst);
            
            // Play particles
            particles.Play();
            
            // Play sound effect
            if (enableSoundEffects && coinSound != null)
            {
                PlaySound(coinSound);
            }
            
            // Return to pool after duration
            StartCoroutine(ReturnParticleToPoolAfterDuration(particles, coinBurstPool, particles.main.duration + particles.main.startLifetime.constantMax));
        }
        
        /// <summary>
        /// Plays level up effect with celebration particles.
        /// </summary>
        /// <param name="worldPosition">World position to spawn effect</param>
        public void PlayLevelUpEffect(Vector3 worldPosition)
        {
            // Star burst
            if (starBurstPool != null)
            {
                var stars = starBurstPool.Get();
                if (stars != null)
                {
                    stars.transform.position = worldPosition;
                    stars.Play();
                    StartCoroutine(ReturnParticleToPoolAfterDuration(stars, starBurstPool, stars.main.duration + stars.main.startLifetime.constantMax));
                }
            }
            
            // Confetti
            if (confettiPool != null)
            {
                var confetti = confettiPool.Get();
                if (confetti != null)
                {
                    confetti.transform.position = worldPosition;
                    confetti.Play();
                    StartCoroutine(ReturnParticleToPoolAfterDuration(confetti, confettiPool, confetti.main.duration + confetti.main.startLifetime.constantMax));
                }
            }
            
            // Glow effect
            if (glowPool != null)
            {
                var glow = glowPool.Get();
                if (glow != null)
                {
                    glow.transform.position = worldPosition;
                    glow.Play();
                    StartCoroutine(ReturnParticleToPoolAfterDuration(glow, glowPool, glow.main.duration + glow.main.startLifetime.constantMax));
                }
            }
            
            // Play sound
            if (enableSoundEffects && levelUpSound != null)
            {
                PlaySound(levelUpSound);
            }
        }
        
        /// <summary>
        /// Plays achievement unlocked effect.
        /// </summary>
        /// <param name="worldPosition">World position to spawn effect</param>
        public void PlayAchievementEffect(Vector3 worldPosition)
        {
            // Star burst
            if (starBurstPool != null)
            {
                var stars = starBurstPool.Get();
                if (stars != null)
                {
                    stars.transform.position = worldPosition;
                    stars.Play();
                    StartCoroutine(ReturnParticleToPoolAfterDuration(stars, starBurstPool, stars.main.duration + stars.main.startLifetime.constantMax));
                }
            }
            
            // Play sound
            if (enableSoundEffects && achievementSound != null)
            {
                PlaySound(achievementSound);
            }
        }
        
        /// <summary>
        /// Animates coins flying from source to destination with smooth bezier curve.
        /// Creates a satisfying visual feedback for currency transfers.
        /// </summary>
        /// <param name="startPosition">Starting world position</param>
        /// <param name="endPosition">Ending world position (usually currency UI)</param>
        /// <param name="count">Number of coin sprites to animate</param>
        /// <param name="onComplete">Callback when animation completes</param>
        public void AnimateCoinFlight(Vector3 startPosition, Vector3 endPosition, int count = 5, System.Action onComplete = null)
        {
            StartCoroutine(AnimateCoinFlightCoroutine(startPosition, endPosition, count, onComplete));
        }
        
        /// <summary>
        /// Coroutine for animating coin flight with bezier curve.
        /// </summary>
        private IEnumerator AnimateCoinFlightCoroutine(Vector3 start, Vector3 end, int count, System.Action onComplete)
        {
            // TODO: [OPTIMIZATION] Implement actual coin sprite flight animation
            // For now, just wait and call completion
            // In production, would create temporary coin sprites, animate along bezier curve, pool them
            
            yield return new WaitForSeconds(coinAnimationDuration);
            
            // Play arrival effect
            PlayCoinRewardEffect(end, count);
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// Returns a particle system to its pool after its duration completes.
        /// </summary>
        private IEnumerator ReturnParticleToPoolAfterDuration(ParticleSystem particles, Core.ObjectPool<ParticleSystem> pool, float duration)
        {
            yield return new WaitForSeconds(duration);
            
            // Stop and return to pool
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            pool.Return(particles);
        }
        
        /// <summary>
        /// Plays a sound effect if audio is enabled.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        /// <summary>
        /// Enables or disables sound effects at runtime.
        /// </summary>
        public void SetSoundEffectsEnabled(bool enabled)
        {
            enableSoundEffects = enabled;
        }
        
        private void OnDestroy()
        {
            // Clean up pools
            coinBurstPool?.Clear();
            starBurstPool?.Clear();
            confettiPool?.Clear();
            glowPool?.Clear();
        }
    }
}
