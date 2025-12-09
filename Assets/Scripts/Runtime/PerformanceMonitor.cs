using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Production-grade performance monitoring system for runtime profiling.
    /// Tracks FPS, memory usage, garbage collection, and custom metrics.
    /// Provides real-time performance data for optimization and debugging.
    /// </summary>
    public class PerformanceMonitor : MonoBehaviour
    {
        [Header("Monitoring Configuration")]
        [SerializeField] private bool enableMonitoring = true;
        [SerializeField] private bool showDebugOverlay = false;
        [SerializeField] private float updateInterval = 0.5f;

        [Header("Performance Thresholds")]
        [SerializeField] private int targetFPS = 60;
        [SerializeField] private float lowFPSThreshold = 30f;
        [SerializeField] private long highMemoryThreshold = 512 * 1024 * 1024; // 512 MB

        [Header("Debug UI")]
        [SerializeField] private TMPro.TextMeshProUGUI debugText;
        [SerializeField] private bool autoCreateDebugUI = true;

        // Singleton instance
        private static PerformanceMonitor instance;
        public static PerformanceMonitor Instance => instance;

        // Performance metrics
        private float currentFPS;
        private float averageFPS;
        private float minFPS = float.MaxValue;
        private float maxFPS;
        private int frameCount;
        
        // Memory metrics
        private long totalAllocatedMemory;
        private long totalReservedMemory;
        private long monoUsedSize;
        private long monoHeapSize;
        
        // GC tracking
        private int gcCollectionCount;
        private int lastGCCount;
        
        // Timing
        private float lastUpdateTime;
        private float deltaTime;
        
        // Custom metrics
        private readonly Dictionary<string, float> customMetrics = new Dictionary<string, float>();
        
        // Performance warnings
        private readonly Queue<string> performanceWarnings = new Queue<string>();
        private const int MaxWarnings = 5;

        // Statistics
        private float totalFrameTime;
        private int sampleCount;

        public float CurrentFPS => currentFPS;
        public float AverageFPS => averageFPS;
        public long TotalAllocatedMemory => totalAllocatedMemory;
        public bool IsPerformanceGood => currentFPS >= targetFPS * 0.9f && totalAllocatedMemory < highMemoryThreshold;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeMonitoring();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initializes performance monitoring systems.
        /// </summary>
        private void InitializeMonitoring()
        {
            lastUpdateTime = Time.realtimeSinceStartup;
            lastGCCount = GC.CollectionCount(0);
            
            // Create debug UI if needed
            if (autoCreateDebugUI && debugText == null && showDebugOverlay)
            {
                CreateDebugUI();
            }

            Debug.Log("[PerformanceMonitor] Performance monitoring initialized.");
        }

        private void Update()
        {
            if (!enableMonitoring) return;

            // Update metrics every frame
            UpdateFrameMetrics();

            // Update detailed metrics at intervals
            if (Time.realtimeSinceStartup - lastUpdateTime >= updateInterval)
            {
                UpdateDetailedMetrics();
                lastUpdateTime = Time.realtimeSinceStartup;

                // Check for performance issues
                CheckPerformanceThresholds();

                // Update debug UI
                if (showDebugOverlay)
                {
                    UpdateDebugDisplay();
                }
            }
        }

        /// <summary>
        /// Updates frame-based metrics (FPS tracking).
        /// </summary>
        private void UpdateFrameMetrics()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            currentFPS = 1.0f / deltaTime;

            frameCount++;
            totalFrameTime += Time.unscaledDeltaTime;
            sampleCount++;

            // Track min/max FPS
            if (currentFPS < minFPS) minFPS = currentFPS;
            if (currentFPS > maxFPS) maxFPS = currentFPS;

            // Calculate average FPS
            if (sampleCount > 0)
            {
                averageFPS = sampleCount / totalFrameTime;
            }
        }

        /// <summary>
        /// Updates memory and garbage collection metrics.
        /// More expensive - only run at intervals.
        /// </summary>
        private void UpdateDetailedMetrics()
        {
            // Memory metrics
            totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            totalReservedMemory = Profiler.GetTotalReservedMemoryLong();
            monoUsedSize = Profiler.GetMonoUsedSizeLong();
            monoHeapSize = Profiler.GetMonoHeapSizeLong();

            // GC tracking
            int currentGCCount = GC.CollectionCount(0);
            if (currentGCCount > lastGCCount)
            {
                gcCollectionCount = currentGCCount - lastGCCount;
                lastGCCount = currentGCCount;
                
                if (gcCollectionCount > 0)
                {
                    AddPerformanceWarning($"GC occurred: {gcCollectionCount} collection(s)");
                }
            }
        }

        /// <summary>
        /// Checks if performance metrics exceed configured thresholds.
        /// Logs warnings for performance issues.
        /// </summary>
        private void CheckPerformanceThresholds()
        {
            // Check FPS
            if (currentFPS < lowFPSThreshold)
            {
                AddPerformanceWarning($"Low FPS detected: {currentFPS:F1} (target: {targetFPS})");
            }

            // Check memory
            if (totalAllocatedMemory > highMemoryThreshold)
            {
                AddPerformanceWarning($"High memory usage: {totalAllocatedMemory / (1024 * 1024)}MB");
            }
        }

        /// <summary>
        /// Adds a custom performance metric for tracking.
        /// </summary>
        /// <param name="metricName">Name of the metric</param>
        /// <param name="value">Metric value</param>
        public void RecordCustomMetric(string metricName, float value)
        {
            customMetrics[metricName] = value;
        }

        /// <summary>
        /// Gets a custom metric value.
        /// </summary>
        /// <param name="metricName">Name of the metric</param>
        /// <returns>Metric value or 0 if not found</returns>
        public float GetCustomMetric(string metricName)
        {
            return customMetrics.TryGetValue(metricName, out float value) ? value : 0f;
        }

        /// <summary>
        /// Measures execution time of a code block.
        /// Usage: using (PerformanceMonitor.Instance.MeasureScope("MyOperation")) { ... }
        /// </summary>
        /// <param name="scopeName">Name of the scope to measure</param>
        /// <returns>Disposable scope for automatic timing</returns>
        public IDisposable MeasureScope(string scopeName)
        {
            return new PerformanceScope(scopeName, this);
        }

        /// <summary>
        /// Disposable scope for measuring execution time.
        /// </summary>
        private class PerformanceScope : IDisposable
        {
            private readonly string scopeName;
            private readonly PerformanceMonitor monitor;
            private readonly float startTime;

            public PerformanceScope(string name, PerformanceMonitor mon)
            {
                scopeName = name;
                monitor = mon;
                startTime = Time.realtimeSinceStartup;
            }

            public void Dispose()
            {
                float elapsed = (Time.realtimeSinceStartup - startTime) * 1000f; // Convert to ms
                monitor.RecordCustomMetric($"Scope_{scopeName}_ms", elapsed);
                
                if (elapsed > 16.67f) // More than one frame at 60 FPS
                {
                    Debug.LogWarning($"[PerformanceMonitor] {scopeName} took {elapsed:F2}ms (>16.67ms frame budget)");
                }
            }
        }

        /// <summary>
        /// Adds a performance warning to the warning queue.
        /// </summary>
        private void AddPerformanceWarning(string warning)
        {
            performanceWarnings.Enqueue(warning);
            
            // Keep only recent warnings
            while (performanceWarnings.Count > MaxWarnings)
            {
                performanceWarnings.Dequeue();
            }
        }

        /// <summary>
        /// Gets a formatted performance report.
        /// Useful for debugging and logging.
        /// </summary>
        /// <returns>Multi-line performance report string</returns>
        public string GetPerformanceReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== Performance Report ===");
            sb.AppendLine($"FPS: Current={currentFPS:F1}, Avg={averageFPS:F1}, Min={minFPS:F1}, Max={maxFPS:F1}");
            sb.AppendLine($"Memory: Allocated={FormatBytes(totalAllocatedMemory)}, Reserved={FormatBytes(totalReservedMemory)}");
            sb.AppendLine($"Mono: Used={FormatBytes(monoUsedSize)}, Heap={FormatBytes(monoHeapSize)}");
            sb.AppendLine($"GC Collections: {gcCollectionCount} since last update");
            
            if (customMetrics.Count > 0)
            {
                sb.AppendLine("\nCustom Metrics:");
                foreach (var kvp in customMetrics)
                {
                    sb.AppendLine($"  {kvp.Key}: {kvp.Value:F2}");
                }
            }

            if (performanceWarnings.Count > 0)
            {
                sb.AppendLine("\nRecent Warnings:");
                foreach (var warning in performanceWarnings)
                {
                    sb.AppendLine($"  - {warning}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Resets performance statistics.
        /// </summary>
        public void ResetStatistics()
        {
            minFPS = float.MaxValue;
            maxFPS = 0f;
            totalFrameTime = 0f;
            sampleCount = 0;
            frameCount = 0;
            customMetrics.Clear();
            performanceWarnings.Clear();
            
            Debug.Log("[PerformanceMonitor] Statistics reset.");
        }

        /// <summary>
        /// Updates the debug overlay UI with current performance metrics.
        /// </summary>
        private void UpdateDebugDisplay()
        {
            if (debugText == null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"<b>Performance Monitor</b>");
            sb.AppendLine($"FPS: <color={(currentFPS >= targetFPS ? "green" : "red")}>{currentFPS:F1}</color> (Avg: {averageFPS:F1})");
            sb.AppendLine($"Memory: {FormatBytes(totalAllocatedMemory)}");
            sb.AppendLine($"Mono Heap: {FormatBytes(monoHeapSize)}");
            
            if (customMetrics.Count > 0)
            {
                sb.AppendLine("\n<b>Custom Metrics:</b>");
                foreach (var kvp in customMetrics)
                {
                    sb.AppendLine($"{kvp.Key}: {kvp.Value:F2}");
                }
            }

            debugText.text = sb.ToString();
        }

        /// <summary>
        /// Creates a simple debug UI for performance overlay.
        /// </summary>
        private void CreateDebugUI()
        {
            // TODO: [OPTIMIZATION] Create Canvas and TextMeshPro component programmatically
            Debug.Log("[PerformanceMonitor] Auto-create debug UI not yet implemented. Assign debugText manually.");
        }

        /// <summary>
        /// Formats byte count into human-readable string.
        /// </summary>
        private string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:F2} {sizes[order]}";
        }

        /// <summary>
        /// Toggles the debug overlay visibility.
        /// </summary>
        public void ToggleDebugOverlay()
        {
            showDebugOverlay = !showDebugOverlay;
            
            if (debugText != null)
            {
                debugText.gameObject.SetActive(showDebugOverlay);
            }
        }

        private void OnDestroy()
        {
            customMetrics.Clear();
            performanceWarnings.Clear();
        }

#if UNITY_EDITOR
        [ContextMenu("Print Performance Report")]
        private void PrintPerformanceReport()
        {
            Debug.Log(GetPerformanceReport());
        }

        [ContextMenu("Reset Statistics")]
        private void EditorResetStatistics()
        {
            ResetStatistics();
        }
#endif
    }
}
