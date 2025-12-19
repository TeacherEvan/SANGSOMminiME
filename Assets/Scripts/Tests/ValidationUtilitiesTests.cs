using NUnit.Framework;
using SangsomMiniMe.Core;

namespace SangsomMiniMe.Tests
{
    /// <summary>
    /// Unit tests for ValidationUtilities class - comprehensive validation and security testing
    /// </summary>
    [TestFixture]
    public class ValidationUtilitiesTests
    {
        #region Username Validation Tests

        [Test]
        public void ValidateUsername_NullOrEmpty_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateUsername(null, out string error));
            Assert.IsNotEmpty(error);

            Assert.IsFalse(ValidationUtilities.ValidateUsername("", out error));
            Assert.IsNotEmpty(error);

            Assert.IsFalse(ValidationUtilities.ValidateUsername("   ", out error));
            Assert.IsNotEmpty(error);
        }

        [Test]
        public void ValidateUsername_TooShort_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateUsername("ab", out string error));
            Assert.That(error, Does.Contain("at least 3 characters"));
        }

        [Test]
        public void ValidateUsername_TooLong_ReturnsFalse()
        {
            string longUsername = new string('a', 21);
            Assert.IsFalse(ValidationUtilities.ValidateUsername(longUsername, out string error));
            Assert.That(error, Does.Contain("maximum 20 characters"));
        }

        [Test]
        public void ValidateUsername_InvalidCharacters_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateUsername("user@name", out string error));
            Assert.IsFalse(ValidationUtilities.ValidateUsername("user name", out _));
            Assert.IsFalse(ValidationUtilities.ValidateUsername("user#name", out _));
            Assert.IsFalse(ValidationUtilities.ValidateUsername("user!name", out _));
        }

        [Test]
        public void ValidateUsername_ValidUsernames_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateUsername("user123", out _));
            Assert.IsTrue(ValidationUtilities.ValidateUsername("test_user", out _));
            Assert.IsTrue(ValidationUtilities.ValidateUsername("Test-User", out _));
            Assert.IsTrue(ValidationUtilities.ValidateUsername("abc", out _));
            Assert.IsTrue(ValidationUtilities.ValidateUsername("a1b2c3d4e5f6g7h8i9j0", out _)); // 20 chars
        }

        #endregion

        #region Display Name Validation Tests

        [Test]
        public void ValidateDisplayName_NullOrEmpty_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateDisplayName(null, out string error));
            Assert.IsNotEmpty(error);

            Assert.IsFalse(ValidationUtilities.ValidateDisplayName("", out error));
            Assert.IsNotEmpty(error);
        }

        [Test]
        public void ValidateDisplayName_TooShort_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateDisplayName("A", out string error));
            Assert.That(error, Does.Contain("at least 2 characters"));
        }

        [Test]
        public void ValidateDisplayName_TooLong_ReturnsFalse()
        {
            string longName = new string('a', 31);
            Assert.IsFalse(ValidationUtilities.ValidateDisplayName(longName, out string error));
            Assert.That(error, Does.Contain("maximum 30 characters"));
        }

        [Test]
        public void ValidateDisplayName_ValidNames_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateDisplayName("John Doe", out _));
            Assert.IsTrue(ValidationUtilities.ValidateDisplayName("สมชาย", out _)); // Thai name
            Assert.IsTrue(ValidationUtilities.ValidateDisplayName("Alice-Marie", out _));
            Assert.IsTrue(ValidationUtilities.ValidateDisplayName("Bob_123", out _));
        }

        #endregion

        #region Email Validation Tests

        [Test]
        public void ValidateEmail_NullOrEmpty_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateEmail(null, out string error));
            Assert.IsFalse(ValidationUtilities.ValidateEmail("", out error));
        }

        [Test]
        public void ValidateEmail_InvalidFormat_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateEmail("notanemail", out _));
            Assert.IsFalse(ValidationUtilities.ValidateEmail("missing@domain", out _));
            Assert.IsFalse(ValidationUtilities.ValidateEmail("@nodomain.com", out _));
            Assert.IsFalse(ValidationUtilities.ValidateEmail("user@.com", out _));
        }

        [Test]
        public void ValidateEmail_ValidEmails_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateEmail("user@example.com", out _));
            Assert.IsTrue(ValidationUtilities.ValidateEmail("test.user@domain.co.uk", out _));
            Assert.IsTrue(ValidationUtilities.ValidateEmail("user+tag@example.org", out _));
        }

        #endregion

        #region Currency/Resource Validation Tests

        [Test]
        public void ValidateResourceSpending_SufficientBalance_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateResourceSpending(100, 50, "Coins", out string error));
            Assert.IsEmpty(error);
        }

        [Test]
        public void ValidateResourceSpending_InsufficientBalance_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateResourceSpending(50, 100, "Coins", out string error));
            Assert.That(error, Does.Contain("Insufficient"));
            Assert.That(error, Does.Contain("50"));
            Assert.That(error, Does.Contain("100"));
        }

        [Test]
        public void ValidateResourceSpending_NegativeCost_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateResourceSpending(100, -10, "Coins", out string error));
            Assert.That(error, Does.Contain("negative"));
        }

        [Test]
        public void ValidateResourceSpending_ZeroCost_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateResourceSpending(100, 0, "Coins", out _));
        }

        [Test]
        public void ValidateCurrencyAmount_ValidAmounts_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateCurrencyAmount(0, 0, 1000, "Coins", out _));
            Assert.IsTrue(ValidationUtilities.ValidateCurrencyAmount(500, 0, 1000, "Coins", out _));
            Assert.IsTrue(ValidationUtilities.ValidateCurrencyAmount(1000, 0, 1000, "Coins", out _));
        }

        [Test]
        public void ValidateCurrencyAmount_BelowMinimum_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateCurrencyAmount(-1, 0, 1000, "Coins", out string error));
            Assert.That(error, Does.Contain("cannot be negative"));
        }

        [Test]
        public void ValidateCurrencyAmount_AboveMaximum_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateCurrencyAmount(1001, 0, 1000, "Coins", out string error));
            Assert.That(error, Does.Contain("exceeds maximum"));
            Assert.That(error, Does.Contain("1000"));
        }

        [Test]
        public void ValidateExperienceAmount_WithinBounds_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateExperienceAmount(0, out _));
            Assert.IsTrue(ValidationUtilities.ValidateExperienceAmount(5000, out _));
            Assert.IsTrue(ValidationUtilities.ValidateExperienceAmount(GameConstants.MaxExperiencePoints, out _));
        }

        [Test]
        public void ValidateExperienceAmount_Negative_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateExperienceAmount(-1, out string error));
            Assert.That(error, Does.Contain("negative"));
        }

        [Test]
        public void ValidateExperienceAmount_ExceedsMaximum_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateExperienceAmount(GameConstants.MaxExperiencePoints + 1, out string error));
            Assert.That(error, Does.Contain("exceeds maximum"));
        }

        #endregion

        #region Numeric Range Validation Tests

        [Test]
        public void ValidateNumericRange_WithinRange_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateNumericRange(5f, 0f, 10f, "Value", out _));
            Assert.IsTrue(ValidationUtilities.ValidateNumericRange(0f, 0f, 10f, "Value", out _));
            Assert.IsTrue(ValidationUtilities.ValidateNumericRange(10f, 0f, 10f, "Value", out _));
        }

        [Test]
        public void ValidateNumericRange_BelowMinimum_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateNumericRange(-1f, 0f, 10f, "Value", out string error));
            Assert.That(error, Does.Contain("below minimum"));
        }

        [Test]
        public void ValidateNumericRange_AboveMaximum_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateNumericRange(11f, 0f, 10f, "Value", out string error));
            Assert.That(error, Does.Contain("above maximum"));
        }

        #endregion

        #region Content Filtering Tests

        [Test]
        public void ContainsProfanity_CleanText_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ContainsProfanity("Hello world"));
            Assert.IsFalse(ValidationUtilities.ContainsProfanity("This is a nice day"));
            Assert.IsFalse(ValidationUtilities.ContainsProfanity("Educational content"));
        }

        [Test]
        public void ContainsProfanity_ProfaneText_ReturnsTrue()
        {
            // Test basic profanity detection (using mild examples)
            Assert.IsTrue(ValidationUtilities.ContainsProfanity("This is damn wrong"));
            Assert.IsTrue(ValidationUtilities.ContainsProfanity("What the hell"));
        }

        [Test]
        public void ContainsProfanity_CaseInsensitive_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ContainsProfanity("DAMN"));
            Assert.IsTrue(ValidationUtilities.ContainsProfanity("DaMn"));
        }

        #endregion

        #region Text Sanitization Tests

        [Test]
        public void SanitizeText_NullOrEmpty_ReturnsEmpty()
        {
            Assert.AreEqual("", ValidationUtilities.SanitizeText(null));
            Assert.AreEqual("", ValidationUtilities.SanitizeText(""));
        }

        [Test]
        public void SanitizeText_RemovesHtmlTags()
        {
            Assert.AreEqual("Hello World", ValidationUtilities.SanitizeText("<script>Hello World</script>"));
            Assert.AreEqual("Test", ValidationUtilities.SanitizeText("<div>Test</div>"));
        }

        [Test]
        public void SanitizeText_TrimsWhitespace()
        {
            Assert.AreEqual("Test", ValidationUtilities.SanitizeText("  Test  "));
            Assert.AreEqual("Hello World", ValidationUtilities.SanitizeText("Hello World\n\n"));
        }

        [Test]
        public void SanitizeText_NormalizesWhitespace()
        {
            Assert.AreEqual("Hello World", ValidationUtilities.SanitizeText("Hello    World"));
            Assert.AreEqual("Test Line", ValidationUtilities.SanitizeText("Test\n\nLine"));
        }

        #endregion

        #region Resource Path Validation Tests

        [Test]
        public void ValidateResourcePath_NullOrEmpty_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath(null, out string error));
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("", out error));
        }

        [Test]
        public void ValidateResourcePath_DirectoryTraversal_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("../secret", out string error));
            Assert.That(error, Does.Contain("traversal"));

            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("folder/../../etc", out error));
        }

        [Test]
        public void ValidateResourcePath_AbsolutePath_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("C:\\Windows\\System32", out string error));
            Assert.That(error, Does.Contain("Absolute paths"));

            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("/etc/passwd", out error));
        }

        [Test]
        public void ValidateResourcePath_InvalidCharacters_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("file<name>", out string error));
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("file|name", out error));
            Assert.IsFalse(ValidationUtilities.ValidateResourcePath("file?name", out error));
        }

        [Test]
        public void ValidateResourcePath_ValidPaths_ReturnsTrue()
        {
            Assert.IsTrue(ValidationUtilities.ValidateResourcePath("Outfits/student_uniform", out _));
            Assert.IsTrue(ValidationUtilities.ValidateResourcePath("Audio/Click", out _));
            Assert.IsTrue(ValidationUtilities.ValidateResourcePath("Textures/UI/button_normal", out _));
        }

        #endregion

        #region GameConfiguration Validation Tests

        [Test]
        public void ValidateGameConfiguration_NullConfig_ReturnsFalse()
        {
            Assert.IsFalse(ValidationUtilities.ValidateGameConfiguration(null, out string error));
            Assert.That(error, Does.Contain("null"));
        }

        [Test]
        public void ValidateGameConfiguration_ValidConfig_ReturnsTrue()
        {
            var config = UnityEngine.ScriptableObject.CreateInstance<GameConfiguration>();

            // Set valid values
            config.DefaultHappiness = 50f;
            config.DefaultHunger = 50f;
            config.DefaultEnergy = 50f;
            config.HappinessFloor = 10f;
            config.HungerFloor = 10f;
            config.EnergyFloor = 10f;
            config.HappinessDecayPerHour = 5f;
            config.HungerDecayPerHour = 5f;
            config.EnergyDecayPerHour = 5f;

            Assert.IsTrue(ValidationUtilities.ValidateGameConfiguration(config, out string error));
            Assert.IsEmpty(error);
        }

        [Test]
        public void ValidateGameConfiguration_InvalidDefaults_ReturnsFalse()
        {
            var config = UnityEngine.ScriptableObject.CreateInstance<GameConfiguration>();
            config.DefaultHappiness = -10f; // Invalid
            config.HappinessFloor = 10f;

            Assert.IsFalse(ValidationUtilities.ValidateGameConfiguration(config, out string error));
            Assert.That(error, Does.Contain("DefaultHappiness"));
        }

        [Test]
        public void ValidateGameConfiguration_FloorAboveDefault_ReturnsFalse()
        {
            var config = UnityEngine.ScriptableObject.CreateInstance<GameConfiguration>();
            config.DefaultHappiness = 50f;
            config.HappinessFloor = 60f; // Floor above default is invalid

            Assert.IsFalse(ValidationUtilities.ValidateGameConfiguration(config, out string error));
            Assert.That(error, Does.Contain("floor"));
        }

        #endregion
    }
}
