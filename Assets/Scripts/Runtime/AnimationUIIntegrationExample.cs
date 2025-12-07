using UnityEngine;
using SangsomMiniMe.Character;
using SangsomMiniMe.UI;
using System.Collections;

namespace SangsomMiniMe.Examples
{
    /// <summary>
    /// Example demonstrating integration of Unity-native animation pipeline
    /// with UITransitionManager for synchronized UI and character animations.
    /// 
    /// This example shows best practices for:
    /// - Coordinating UI transitions with character animations
    /// - Using animation events for timing
    /// - Handling both parallel and sequential animation+UI patterns
    /// </summary>
    public class AnimationUIIntegrationExample : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CharacterController character;
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject customizationPanel;
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private GameObject homeworkPanel;
        
        [Header("Settings")]
        [SerializeField] private bool useParallelTransitions = true;
        [SerializeField] private float sequentialDelay = 1.5f;
        
        // Example 1: Parallel UI and Animation
        // Character waves while UI panel slides in simultaneously
        public void ShowCustomization()
        {
            if (useParallelTransitions)
            {
                // Play character animation
                character.PlayWave();
                
                // Transition UI panels at the same time
                UITransitionManager.Instance.TransitionPanels(
                    mainPanel,
                    customizationPanel,
                    UITransitionManager.TransitionType.SlideLeft
                );
                
                Debug.Log("Parallel transition: Character waves while UI slides");
            }
            else
            {
                // Sequential version (animation first, then UI)
                StartCoroutine(ShowCustomizationSequential());
            }
        }
        
        private IEnumerator ShowCustomizationSequential()
        {
            // Step 1: Character animation
            character.PlayWave();
            yield return new WaitForSeconds(sequentialDelay);
            
            // Step 2: UI transition
            UITransitionManager.Instance.TransitionPanels(
                mainPanel,
                customizationPanel,
                UITransitionManager.TransitionType.SlideLeft
            );
        }
        
        // Example 2: Animation Triggered by UI Transition Completion
        // UI transitions first, then character celebrates
        public void ShowReward()
        {
            // Transition UI with callback
            UITransitionManager.Instance.TransitionPanels(
                mainPanel,
                rewardPanel,
                UITransitionManager.TransitionType.ScaleUp,
                onComplete: OnRewardPanelShown
            );
        }
        
        private void OnRewardPanelShown()
        {
            // UI is fully visible, now celebrate
            character.PlayDance();
            Debug.Log("Reward panel shown, character is dancing!");
        }
        
        // Example 3: UI Transition Triggered by Animation Event
        // Character performs action, then UI responds
        public void CompleteHomework()
        {
            // Subscribe to animation completion event
            character.OnAnimationCompleted += OnHomeworkAnimationComplete;
            
            // Character performs Thai "Wai" gesture (respectful greeting)
            character.PlayWai();
            
            Debug.Log("Character performing Wai gesture before showing homework...");
        }
        
        private void OnHomeworkAnimationComplete(string animationName)
        {
            if (animationName == "Wai")
            {
                // Unsubscribe to prevent duplicate calls
                character.OnAnimationCompleted -= OnHomeworkAnimationComplete;
                
                // Show homework panel after gesture completes
                UITransitionManager.Instance.ShowPanel(
                    homeworkPanel,
                    UITransitionManager.TransitionType.FadeAndSlide
                );
                
                Debug.Log("Homework panel shown after Wai gesture");
            }
        }
        
        // Example 4: Smooth Cross-Fade Transitions
        // For rapid UI changes with animations
        public void QuickPanelChange(GameObject targetPanel, string animationName)
        {
            // Short fade duration for snappy feel
            UITransitionManager.Instance.TransitionPanels(
                FindActivePanel(),
                targetPanel,
                UITransitionManager.TransitionType.Fade
            );
            
            // Play quick animation (using Animancer for better control if available)
            switch (animationName)
            {
                case "Wave":
                    character.PlayWave();
                    break;
                case "Dance":
                    character.PlayDance();
                    break;
                case "Bow":
                    character.PlayBow();
                    break;
                default:
                    character.PlayIdle();
                    break;
            }
        }
        
        // Example 5: Synchronized Timing (Advanced)
        // UI and animation durations match exactly
        public void SynchronizedTransition()
        {
            StartCoroutine(SynchronizedTransitionCoroutine());
        }
        
        private IEnumerator SynchronizedTransitionCoroutine()
        {
            // Get animation duration dynamically
            float animationDuration = GetAnimationDuration("Dance");
            
            // Start both simultaneously
            character.PlayDance();
            
            // UI transition with matching duration
            float uiDuration = 0.3f; // UITransitionManager default
            float syncDelay = Mathf.Max(0, animationDuration - uiDuration);
            
            yield return new WaitForSeconds(syncDelay);
            
            UITransitionManager.Instance.ShowPanel(
                rewardPanel,
                UITransitionManager.TransitionType.ScaleUp
            );
            
            Debug.Log($"Synchronized: Animation ({animationDuration}s) and UI transition");
        }
        
        // Example 6: Error-Resistant Pattern
        // Handles null references and edge cases gracefully
        public void SafeTransition(GameObject targetPanel, string animationName)
        {
            // Validate references
            if (character == null)
            {
                Debug.LogError("CharacterController not assigned!");
                return;
            }
            
            if (targetPanel == null)
            {
                Debug.LogError("Target panel not assigned!");
                return;
            }
            
            // Check if character is already animating
            if (character.IsAnimating)
            {
                Debug.LogWarning("Character is already animating, queuing transition...");
                StartCoroutine(WaitAndTransition(targetPanel, animationName));
                return;
            }
            
            // Safe execution
            PlayAnimationByName(animationName);
            UITransitionManager.Instance.ShowPanel(targetPanel, UITransitionManager.TransitionType.Fade);
        }
        
        private IEnumerator WaitAndTransition(GameObject targetPanel, string animationName)
        {
            // Wait for current animation to finish
            while (character.IsAnimating)
            {
                yield return null;
            }
            
            // Now safe to proceed
            SafeTransition(targetPanel, animationName);
        }
        
        // Helper Methods
        
        private GameObject FindActivePanel()
        {
            GameObject[] panels = { mainPanel, customizationPanel, rewardPanel, homeworkPanel };
            
            foreach (var panel in panels)
            {
                if (panel != null && panel.activeSelf)
                    return panel;
            }
            
            return mainPanel; // fallback
        }
        
        private void PlayAnimationByName(string animationName)
        {
            switch (animationName.ToLower())
            {
                case "idle":
                    character.PlayIdle();
                    break;
                case "dance":
                    character.PlayDance();
                    break;
                case "wave":
                    character.PlayWave();
                    break;
                case "wai":
                    character.PlayWai();
                    break;
                case "curtsy":
                    character.PlayCurtsy();
                    break;
                case "bow":
                    character.PlayBow();
                    break;
                default:
                    Debug.LogWarning($"Unknown animation: {animationName}");
                    character.PlayIdle();
                    break;
            }
        }
        
        private float GetAnimationDuration(string animationName)
        {
            // This would need access to actual animation clips
            // For now, return estimated durations
            switch (animationName.ToLower())
            {
                case "wave":
                case "wai":
                case "bow":
                case "curtsy":
                    return 1.5f;
                case "dance":
                    return 3.0f;
                case "idle":
                default:
                    return 0.5f;
            }
        }
        
        // Test Methods (call from Inspector buttons or debug console)
        
        [ContextMenu("Test Parallel Transition")]
        public void TestParallel()
        {
            ShowCustomization();
        }
        
        [ContextMenu("Test Sequential Transition")]
        public void TestSequential()
        {
            useParallelTransitions = false;
            ShowCustomization();
            useParallelTransitions = true;
        }
        
        [ContextMenu("Test Reward Animation")]
        public void TestReward()
        {
            ShowReward();
        }
        
        [ContextMenu("Test Homework Flow")]
        public void TestHomework()
        {
            CompleteHomework();
        }
        
        [ContextMenu("Test Synchronized")]
        public void TestSynchronized()
        {
            SynchronizedTransition();
        }
        
        private void OnDestroy()
        {
            // Clean up event subscriptions
            if (character != null)
            {
                character.OnAnimationCompleted -= OnHomeworkAnimationComplete;
            }
        }
    }
}
