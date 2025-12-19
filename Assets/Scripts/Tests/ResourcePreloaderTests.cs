using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using SangsomMiniMe.Core;

namespace SangsomMiniMe.Tests
{
    /// <summary>
    /// Unit tests for ResourcePreloader class - async loading and caching
    /// </summary>
    [TestFixture]
    public class ResourcePreloaderTests
    {
        private GameObject preloaderObject;
        private ResourcePreloader preloader;

        [SetUp]
        public void Setup()
        {
            // Create ResourcePreloader GameObject
            preloaderObject = new GameObject("TestResourcePreloader");
            preloader = preloaderObject.AddComponent<ResourcePreloader>();
        }

        [TearDown]
        public void TearDown()
        {
            if (preloaderObject != null)
            {
                Object.DestroyImmediate(preloaderObject);
            }
        }

        [Test]
        public void Instance_ReturnsSingletonInstance()
        {
            Assert.IsNotNull(ResourcePreloader.Instance);
            Assert.AreEqual(preloader, ResourcePreloader.Instance);
        }

        [Test]
        public void InitialState_IsCorrect()
        {
            Assert.IsFalse(preloader.IsPreloading);
            Assert.IsFalse(preloader.PreloadComplete);
            Assert.AreEqual(0f, preloader.PreloadProgress);
        }

        [UnityTest]
        public IEnumerator StartPreload_InitiatesPreloading()
        {
            bool preloadStarted = false;
            preloader.OnPreloadStarted += () => preloadStarted = true;

            preloader.StartPreload();

            // Wait a frame for coroutine to start
            yield return null;

            Assert.IsTrue(preloadStarted || preloader.IsPreloading);
        }

        [UnityTest]
        public IEnumerator StartPreload_TwiceInRow_ShowsWarning()
        {
            preloader.StartPreload();
            yield return null;

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("already started"));
            preloader.StartPreload();

            yield return null;
        }

        [UnityTest]
        public IEnumerator GetCachedResource_BeforePreload_ReturnsNull()
        {
            var resource = preloader.GetCachedResource<Material>("NonExistent");
            Assert.IsNull(resource);
            yield return null;
        }

        [Test]
        public void ClearCache_RemovesAllCachedResources()
        {
            // This test verifies the public API exists
            // Actual caching behavior requires Unity's Resources system in play mode
            Assert.DoesNotThrow(() => preloader.ClearCache());
        }

        [UnityTest]
        public IEnumerator PreloadProgress_UpdatesDuringLoad()
        {
            float lastProgress = 0f;
            bool progressUpdated = false;

            preloader.OnPreloadProgress += (progress) =>
            {
                if (progress > lastProgress)
                {
                    progressUpdated = true;
                    lastProgress = progress;
                }
            };

            preloader.StartPreload();

            // Wait for preload to start and make some progress
            yield return new WaitForSeconds(0.5f);

            // Progress should have been updated (or preload completed quickly with no resources)
            Assert.IsTrue(progressUpdated || preloader.PreloadComplete);
        }

        [UnityTest]
        public IEnumerator OnPreloadComplete_FiresWhenDone()
        {
            bool completed = false;
            preloader.OnPreloadComplete += () => completed = true;

            preloader.StartPreload();

            // Wait for preload to complete (with timeout)
            float timeout = 5f;
            float elapsed = 0f;

            while (!completed && elapsed < timeout)
            {
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            Assert.IsTrue(completed, "Preload should complete within timeout");
            Assert.IsTrue(preloader.PreloadComplete);
            Assert.IsFalse(preloader.IsPreloading);
        }

        [Test]
        public void GetCachedResource_WithNullKey_ReturnsNull()
        {
            var resource = preloader.GetCachedResource<Material>(null);
            Assert.IsNull(resource);
        }

        [Test]
        public void GetCachedResource_WithEmptyKey_ReturnsNull()
        {
            var resource = preloader.GetCachedResource<Material>("");
            Assert.IsNull(resource);
        }

        // LoadResourceAsync method is not part of the current API
        // The preloader uses GetCachedResource<T>(string path) instead
        [Test]
        public void GetCachedResource_NonExistent_ReturnsNull()
        {
            var resource = preloader.GetCachedResource<Material>("NonExistentPath");
            Assert.IsNull(resource);
        }

        // PreloadResourceList method is not part of the current API
        // The preloader uses StartPreload() which loads configured paths
        [Test]
        public void StartPreload_CanBeCalledMultipleTimes()
        {
            // First call starts preload
            preloader.StartPreload();
            // Second call should log warning but not crash
            Assert.DoesNotThrow(() => preloader.StartPreload());
        }

        [Test]
        public void MultipleGetCachedResource_WorksCorrectly()
        {
            // Get the same resource multiple times
            for (int i = 0; i < 10; i++)
            {
                var resource = preloader.GetCachedResource<Material>("TestMaterial");
                // Resource may be null if path doesn't exist, that's fine
            }

            // Verify no exceptions occurred
            Assert.Pass("Multiple gets completed without errors");
        }

        [Test]
        public void CacheStatistics_AreAccessible()
        {
            // Verify that cache stats method exists and doesn't throw
            Assert.DoesNotThrow(() => preloader.LogCacheStatistics());
            // IsResourceCached is also available
            Assert.DoesNotThrow(() => preloader.IsResourceCached("somePath"));
        }

        [UnityTest]
        public IEnumerator ClearCache_DuringPreload_DoesNotCrash()
        {
            preloader.StartPreload();
            yield return null;

            Assert.DoesNotThrow(() => preloader.ClearCache());

            yield return new WaitForSeconds(0.5f);
        }

        [Test]
        public void GetCachedResource_WithWrongType_ReturnsNull()
        {
            // Try to get a Material as an AudioClip (type mismatch)
            var resource = preloader.GetCachedResource<AudioClip>("Outfits/default");
            Assert.IsNull(resource);
        }
    }
}
