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

        [Header("Fallback (No Prefabs)")]
        [SerializeField] private bool autoCreateFallbackParticles = true;
        
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

        private const float DefaultUiEffectDepth = 2.0f;
        
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
            if (autoCreateFallbackParticles)
            {
                coinBurstPrefab ??= CreateFallbackBurst("CoinBurst_Fallback", burstCount: 18, startSpeed: 6f, lifetime: 0.8f);
                starBurstPrefab ??= CreateFallbackBurst("StarBurst_Fallback", burstCount: 24, startSpeed: 7f, lifetime: 0.9f);
                confettiPrefab ??= CreateFallbackBurst("Confetti_Fallback", burstCount: 40, startSpeed: 5f, lifetime: 1.2f);
                glowPrefab ??= CreateFallbackBurst("Glow_Fallback", burstCount: 10, startSpeed: 2.5f, lifetime: 0.6f);
            }

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

        private ParticleSystem CreateFallbackBurst(string name, int burstCount, float startSpeed, float lifetime)
        {
            if (effectsParent == null)
            {
                // Should be created in InitializeEffectSystem, but guard anyway.
                var parentObj = new GameObject("RewardEffectsParent");
                effectsParent = parentObj.transform;
                effectsParent.SetParent(transform);
            }

            var go = new GameObject(name);
            go.transform.SetParent(effectsParent, worldPositionStays: false);

            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.loop = false;
            main.duration = 0.1f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime * 0.85f, lifetime * 1.15f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(startSpeed * 0.85f, startSpeed * 1.15f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.16f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = false;
            main.maxParticles = Mathf.Clamp(burstCount * 2, 32, 512);

            var emission = ps.emission;
            emission.enabled = true;
            emission.rateOverTime = 0f;
            emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)Mathf.Clamp(burstCount, 1, 200)));

            var shape = ps.shape;
            shape.enabled = true;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 0.08f;

            // Gentle fade + shrink (keeps it clean even with default white particles).
            var colorOverLifetime = ps.colorOverLifetime;
            colorOverLifetime.enabled = true;
            {
                var gradient = new Gradient();
                gradient.SetKeys(
                    new[]
                    {
                        new GradientColorKey(Color.white, 0f),
                        new GradientColorKey(Color.white, 1f)
                    },
                    new[]
                    {
                        new GradientAlphaKey(1f, 0f),
                        new GradientAlphaKey(0.65f, 0.35f),
                        new GradientAlphaKey(0f, 1f)
                    }
                );
                colorOverLifetime.color = gradient;
            }

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            {
                var curve = new AnimationCurve(
                    new Keyframe(0f, 1f),
                    new Keyframe(0.5f, 0.9f),
                    new Keyframe(1f, 0f)
                );
                sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
            }

            // Add a bit of organic motion; cheap and noticeable.
            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.25f;
            noise.frequency = 0.35f;
            noise.scrollSpeed = 0.5f;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;

            // Per-effect tweaks (still no prefab assets required).
            // Keep this conservative: the goal is "obviously animated" without turning into a physics sim.
            if (name.Contains("Confetti"))
            {
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = 35f;
                shape.radius = 0.05f;

                main.gravityModifier = 1.2f;
                main.startRotation3D = true;
                main.startRotationX = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
                main.startRotationY = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
                main.startRotationZ = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);

                var rotationOverLifetime = ps.rotationOverLifetime;
                rotationOverLifetime.enabled = true;
                rotationOverLifetime.x = new ParticleSystem.MinMaxCurve(-6f, 6f);
                rotationOverLifetime.y = new ParticleSystem.MinMaxCurve(-6f, 6f);
                rotationOverLifetime.z = new ParticleSystem.MinMaxCurve(-6f, 6f);

                noise.strength = 0.35f;
            }
            else if (name.Contains("Glow"))
            {
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 0.02f;

                main.startSize = new ParticleSystem.MinMaxCurve(0.22f, 0.38f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(startSpeed * 0.5f, startSpeed * 0.9f);
                main.gravityModifier = 0f;

                noise.strength = 0.15f;
            }
            else if (name.Contains("Star"))
            {
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 0.06f;
                main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.12f);
                main.gravityModifier = 0.25f;

                noise.strength = 0.3f;
            }
            else
            {
                // Coin-ish burst: a little heavier and snappier.
                shape.shapeType = ParticleSystemShapeType.Sphere;
                shape.radius = 0.07f;
                main.startSize = new ParticleSystem.MinMaxCurve(0.09f, 0.17f);
                main.gravityModifier = 0.8f;
            }

            go.SetActive(false);
            return ps;
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
            int burstCount = Mathf.Clamp(coinAmount * 3, 5, 50);
            var emission = particles.emission;

            // If the prefab has no bursts configured, add one at t=0.
            if (emission.burstCount <= 0)
            {
                emission.SetBurst(0, new ParticleSystem.Burst(0f, (short)burstCount));
            }
            else
            {
                var burst = emission.GetBurst(0);
                burst.count = new ParticleSystem.MinMaxCurve(burstCount);
                emission.SetBurst(0, burst);
            }
            
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
        /// Plays a coin reward effect anchored to a UI element.
        /// Converts the UI element position to a screen point and spawns near the camera.
        /// Works well with Screen Space Overlay and Screen Space Camera canvases.
        /// </summary>
        public void PlayCoinRewardEffect(RectTransform uiElement, int coinAmount = 1, Camera worldCamera = null, float depthFromCamera = DefaultUiEffectDepth)
        {
            if (uiElement == null)
            {
                return;
            }

            var spawnWorldPos = GetWorldPositionForUI(uiElement, worldCamera, depthFromCamera);
            PlayCoinRewardEffect(spawnWorldPos, coinAmount);
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
        /// Plays level up effect anchored to a UI element.
        /// </summary>
        public void PlayLevelUpEffect(RectTransform uiElement, Camera worldCamera = null, float depthFromCamera = DefaultUiEffectDepth)
        {
            if (uiElement == null)
            {
                return;
            }

            var spawnWorldPos = GetWorldPositionForUI(uiElement, worldCamera, depthFromCamera);
            PlayLevelUpEffect(spawnWorldPos);
        }

        private static Vector3 GetWorldPositionForUI(RectTransform uiElement, Camera worldCamera, float depthFromCamera)
        {
            // If there is no camera to convert to world space, fall back to whatever "world" position
            // the UI element currently has.
            var cam = worldCamera != null ? worldCamera : Camera.main;
            if (cam == null)
            {
                return uiElement.position;
            }

            var screenPoint = RectTransformUtility.WorldToScreenPoint(null, uiElement.position);
            float depth = Mathf.Max(cam.nearClipPlane + 0.5f, depthFromCamera);
            return cam.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, depth));
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
            // TODO: [OPTIMIZATION] Implement actual coin sprite flight animation (Estimated: 4-6 hours)
            // Implementation steps:
            // 1. Create/pool coin UI sprites (2D Image prefabs)
            // 2. Calculate bezier curve control points (mid-point offset for arc)
            // 3. Animate sprites along curve using Mathf.Lerp + curve evaluation
            // 4. Stagger coin departures (0.05s delay between each)
            // 5. Return sprites to pool on arrival
            // 6. Add scale/rotation animation during flight for juice
            // Reference: Use AnimationCurve for smooth easing, WorldToScreenPoint for positioning
            
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
            if (instance == this)
            {
                instance = null;
            }
            // Clean up pools
            coinBurstPool?.Clear();
            starBurstPool?.Clear();
            confettiPool?.Clear();
            glowPool?.Clear();
        }
    }
}
