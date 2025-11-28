using NUnit.Framework;
using SangsomMiniMe.Core;

namespace SangsomMiniMe.Tests
{
    /// <summary>
    /// Unit tests for GameUtilities class
    /// </summary>
    [TestFixture]
    public class GameUtilitiesTests
    {
        [Test]
        public void GetMoodState_ReturnsVeryHappy_WhenHappinessAboveThreshold()
        {
            Assert.AreEqual(MoodState.VeryHappy, GameUtilities.GetMoodState(80f));
            Assert.AreEqual(MoodState.VeryHappy, GameUtilities.GetMoodState(100f));
        }
        
        [Test]
        public void GetMoodState_ReturnsHappy_WhenHappinessInRange()
        {
            Assert.AreEqual(MoodState.Happy, GameUtilities.GetMoodState(60f));
            Assert.AreEqual(MoodState.Happy, GameUtilities.GetMoodState(79f));
        }
        
        [Test]
        public void GetMoodState_ReturnsNeutral_WhenHappinessInRange()
        {
            Assert.AreEqual(MoodState.Neutral, GameUtilities.GetMoodState(40f));
            Assert.AreEqual(MoodState.Neutral, GameUtilities.GetMoodState(59f));
        }
        
        [Test]
        public void GetMoodState_ReturnsSad_WhenHappinessInRange()
        {
            Assert.AreEqual(MoodState.Sad, GameUtilities.GetMoodState(20f));
            Assert.AreEqual(MoodState.Sad, GameUtilities.GetMoodState(39f));
        }
        
        [Test]
        public void GetMoodState_ReturnsVerySad_WhenHappinessBelowThreshold()
        {
            Assert.AreEqual(MoodState.VerySad, GameUtilities.GetMoodState(0f));
            Assert.AreEqual(MoodState.VerySad, GameUtilities.GetMoodState(19f));
        }
        
        [Test]
        public void CalculateLevel_ReturnsCorrectLevel()
        {
            Assert.AreEqual(1, GameUtilities.CalculateLevel(0));
            Assert.AreEqual(1, GameUtilities.CalculateLevel(99));
            Assert.AreEqual(2, GameUtilities.CalculateLevel(100));
            Assert.AreEqual(2, GameUtilities.CalculateLevel(150));
            Assert.AreEqual(11, GameUtilities.CalculateLevel(1000));
        }
        
        [Test]
        public void GetLevelProgress_ReturnsCorrectProgress()
        {
            Assert.AreEqual(0f, GameUtilities.GetLevelProgress(0));
            Assert.AreEqual(0.5f, GameUtilities.GetLevelProgress(50));
            Assert.AreEqual(0.99f, GameUtilities.GetLevelProgress(99), 0.01f);
            Assert.AreEqual(0f, GameUtilities.GetLevelProgress(100)); // Reset at new level
        }
        
        [Test]
        public void IsValidUsername_ReturnsFalse_ForNullOrEmpty()
        {
            Assert.IsFalse(GameUtilities.IsValidUsername(null));
            Assert.IsFalse(GameUtilities.IsValidUsername(""));
            Assert.IsFalse(GameUtilities.IsValidUsername("   "));
        }
        
        [Test]
        public void IsValidUsername_ReturnsFalse_ForTooShort()
        {
            Assert.IsFalse(GameUtilities.IsValidUsername("ab"));
        }
        
        [Test]
        public void IsValidUsername_ReturnsFalse_ForTooLong()
        {
            Assert.IsFalse(GameUtilities.IsValidUsername("abcdefghijklmnopqrstuvwxyz"));
        }
        
        [Test]
        public void IsValidUsername_ReturnsFalse_ForInvalidCharacters()
        {
            Assert.IsFalse(GameUtilities.IsValidUsername("user@name"));
            Assert.IsFalse(GameUtilities.IsValidUsername("user name"));
            Assert.IsFalse(GameUtilities.IsValidUsername("user-name"));
        }
        
        [Test]
        public void IsValidUsername_ReturnsTrue_ForValidUsernames()
        {
            Assert.IsTrue(GameUtilities.IsValidUsername("user123"));
            Assert.IsTrue(GameUtilities.IsValidUsername("test_user"));
            Assert.IsTrue(GameUtilities.IsValidUsername("UserName"));
            Assert.IsTrue(GameUtilities.IsValidUsername("abc"));
        }
        
        [Test]
        public void IsValidDisplayName_ReturnsFalse_ForNullOrEmpty()
        {
            Assert.IsFalse(GameUtilities.IsValidDisplayName(null));
            Assert.IsFalse(GameUtilities.IsValidDisplayName(""));
            Assert.IsFalse(GameUtilities.IsValidDisplayName("   "));
        }
        
        [Test]
        public void IsValidDisplayName_ReturnsTrue_ForValidNames()
        {
            Assert.IsTrue(GameUtilities.IsValidDisplayName("John"));
            Assert.IsTrue(GameUtilities.IsValidDisplayName("John Doe"));
            Assert.IsTrue(GameUtilities.IsValidDisplayName("สมชาย")); // Thai name
        }
        
        [Test]
        public void FormatCoinsDisplay_FormatsCorrectly()
        {
            Assert.AreEqual("50", GameUtilities.FormatCoinsDisplay(50));
            Assert.AreEqual("999", GameUtilities.FormatCoinsDisplay(999));
            Assert.AreEqual("1.0K", GameUtilities.FormatCoinsDisplay(1000));
            Assert.AreEqual("1.5K", GameUtilities.FormatCoinsDisplay(1500));
            Assert.AreEqual("1.0M", GameUtilities.FormatCoinsDisplay(1000000));
        }
        
        [Test]
        public void ClampHappiness_ClampsCorrectly()
        {
            Assert.AreEqual(0f, GameUtilities.ClampHappiness(-10f));
            Assert.AreEqual(50f, GameUtilities.ClampHappiness(50f));
            Assert.AreEqual(100f, GameUtilities.ClampHappiness(150f));
        }
        
        [Test]
        public void ClampEyeScale_ClampsCorrectly()
        {
            Assert.AreEqual(GameConstants.MinEyeScale, GameUtilities.ClampEyeScale(0f));
            Assert.AreEqual(1f, GameUtilities.ClampEyeScale(1f));
            Assert.AreEqual(GameConstants.MaxEyeScale, GameUtilities.ClampEyeScale(5f));
        }
        
        [Test]
        public void GetRandomAnimation_DoesNotReturnIdle()
        {
            // Run multiple times to ensure Idle is never returned
            for (int i = 0; i < 100; i++)
            {
                var animation = GameUtilities.GetRandomAnimation();
                Assert.AreNotEqual(CharacterAnimation.Idle, animation);
            }
        }
        
        [Test]
        public void GetHomeworkMotivation_ReturnsAppropriateMessage()
        {
            Assert.IsNotNull(GameUtilities.GetHomeworkMotivation(0));
            Assert.IsNotNull(GameUtilities.GetHomeworkMotivation(1));
            Assert.IsNotNull(GameUtilities.GetHomeworkMotivation(50));
            Assert.IsNotNull(GameUtilities.GetHomeworkMotivation(100));
        }
        
        [Test]
        public void GetTimeBasedGreeting_ReturnsNonEmptyString()
        {
            string greeting = GameUtilities.GetTimeBasedGreeting();
            Assert.IsNotNull(greeting);
            Assert.IsNotEmpty(greeting);
        }
    }
}
