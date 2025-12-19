using NUnit.Framework;
using System;
using UnityEngine;
using SangsomMiniMe.Core;

namespace SangsomMiniMe.Tests
{
    /// <summary>
    /// Unit tests for UserProfile class
    /// </summary>
    [TestFixture]
    public class UserProfileTests
    {
        private UserProfile testUser;

        [SetUp]
        public void Setup()
        {
            testUser = new UserProfile("test_user", "Test User");
        }

        [Test]
        public void UserProfile_Creation_SetsCorrectDefaults()
        {
            Assert.IsNotNull(testUser.UserId);
            Assert.AreEqual("test_user", testUser.UserName);
            Assert.AreEqual("Test User", testUser.DisplayName);
            Assert.AreEqual(0, testUser.ExperiencePoints);
            Assert.AreEqual(GameConstants.DefaultStartingCoins, testUser.Coins); // Starting coins
            Assert.AreEqual(GameConstants.DefaultStartingHappiness, testUser.CharacterHappiness); // Starting happiness
            Assert.AreEqual(GameConstants.DefaultEyeScale, testUser.EyeScale);
            Assert.AreEqual(GameConstants.DefaultOutfit, testUser.CurrentOutfit);
            Assert.AreEqual(GameConstants.DefaultAccessory, testUser.CurrentAccessory);
            Assert.IsTrue(testUser.IsActive);
        }

        [Test]
        public void AddExperience_IncreasesExperiencePoints()
        {
            testUser.AddExperience(50);
            Assert.AreEqual(50, testUser.ExperiencePoints);

            testUser.AddExperience(25);
            Assert.AreEqual(75, testUser.ExperiencePoints);
        }

        [Test]
        public void AddCoins_IncreasesCoins()
        {
            int initialCoins = testUser.Coins;
            testUser.AddCoins(50);
            Assert.AreEqual(initialCoins + 50, testUser.Coins);
        }

        [Test]
        public void SpendCoins_WithSufficientBalance_ReturnsTrue()
        {
            testUser.AddCoins(100);
            int coinsBeforeSpend = testUser.Coins;

            bool result = testUser.SpendCoins(50);

            Assert.IsTrue(result);
            Assert.AreEqual(coinsBeforeSpend - 50, testUser.Coins);
        }

        [Test]
        public void SpendCoins_WithInsufficientBalance_ReturnsFalse()
        {
            int initialCoins = testUser.Coins;

            bool result = testUser.SpendCoins(initialCoins + 100);

            Assert.IsFalse(result);
            Assert.AreEqual(initialCoins, testUser.Coins); // Coins unchanged
        }

        [Test]
        public void CompleteHomework_UpdatesAllRelevantStats()
        {
            int initialHomework = testUser.HomeworkCompleted;
            int initialExp = testUser.ExperiencePoints;
            int initialCoins = testUser.Coins;
            float initialHappiness = testUser.CharacterHappiness;

            testUser.CompleteHomework();

            Assert.AreEqual(initialHomework + 1, testUser.HomeworkCompleted);
            Assert.AreEqual(initialExp + 10, testUser.ExperiencePoints);
            Assert.AreEqual(initialCoins + 5, testUser.Coins);
            Assert.Greater(testUser.CharacterHappiness, initialHappiness);
        }

        [Test]
        public void IncreaseHappiness_ClampsAtMaximum()
        {
            testUser.IncreaseHappiness(1000f);
            Assert.AreEqual(100f, testUser.CharacterHappiness);
        }

        [Test]
        public void DecreaseHappiness_ClampsAtMinimum()
        {
            testUser.DecreaseHappiness(1000f);
            Assert.AreEqual(0f, testUser.CharacterHappiness);
        }

        [Test]
        public void SetEyeScale_ClampsWithinRange()
        {
            testUser.SetEyeScale(0.1f);
            Assert.AreEqual(0.5f, testUser.EyeScale);

            testUser.SetEyeScale(5.0f);
            Assert.AreEqual(2.0f, testUser.EyeScale);

            testUser.SetEyeScale(1.5f);
            Assert.AreEqual(1.5f, testUser.EyeScale);
        }

        [Test]
        public void SetOutfit_UpdatesCurrentOutfit()
        {
            testUser.SetOutfit("fancy_dress");
            Assert.AreEqual("fancy_dress", testUser.CurrentOutfit);
        }

        [Test]
        public void SetAccessory_UpdatesCurrentAccessory()
        {
            testUser.SetAccessory("hat_1");
            Assert.AreEqual("hat_1", testUser.CurrentAccessory);
        }

        [Test]
        public void UserId_IsUniqueGuid()
        {
            var user1 = new UserProfile("user1", "User 1");
            var user2 = new UserProfile("user2", "User 2");

            Assert.AreNotEqual(user1.UserId, user2.UserId);
            Assert.IsTrue(System.Guid.TryParse(user1.UserId, out _));
        }

        [Test]
        public void OfflineMeterDecayCatchUp_ClampsAndRespectsFloors()
        {
            // Start from known highs
            testUser.IncreaseHappiness(1000f);
            testUser.IncreaseHunger(1000f);
            testUser.IncreaseEnergy(1000f);

            long now = DateTime.UtcNow.Ticks;
            long tenHoursAgo = now - TimeSpan.FromHours(10).Ticks;

            testUser.SetLastMeterDecayUtcTicks(tenHoursAgo);
            testUser.ApplyOfflineMeterDecayCatchUp(now);

            float elapsedMinutes = 10f * 60f;
            float clampedMinutes = Mathf.Min(elapsedMinutes, GameConstants.MaxOfflineMeterDecayMinutes);

            float expectedHappiness = Mathf.Max(100f - GameConstants.HappinessDecayPerMinute * clampedMinutes, GameConstants.HappinessFloor);
            float expectedHunger = Mathf.Max(100f - GameConstants.HungerDecayPerMinute * clampedMinutes, GameConstants.HungerFloor);
            float expectedEnergy = Mathf.Max(100f - GameConstants.EnergyDecayPerMinute * clampedMinutes, GameConstants.EnergyFloor);

            Assert.AreEqual(expectedHappiness, testUser.CharacterHappiness, 0.05f);
            Assert.AreEqual(expectedHunger, testUser.CharacterHunger, 0.05f);
            Assert.AreEqual(expectedEnergy, testUser.CharacterEnergy, 0.05f);
            Assert.AreEqual(now, testUser.LastMeterDecayUtcTicks);

            // Floors should always be respected
            Assert.GreaterOrEqual(testUser.CharacterHappiness, GameConstants.HappinessFloor);
            Assert.GreaterOrEqual(testUser.CharacterHunger, GameConstants.HungerFloor);
            Assert.GreaterOrEqual(testUser.CharacterEnergy, GameConstants.EnergyFloor);
        }
    }
}
