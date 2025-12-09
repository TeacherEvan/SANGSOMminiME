using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SangsomMiniMe.Core
{
    /// <summary>
    /// Comprehensive input validation utilities for enhanced type safety.
    /// Provides robust validation patterns for user inputs and data integrity.
    /// </summary>
    public static class ValidationUtilities
    {
        // Validation regex patterns
        private static readonly Regex UsernamePattern = new Regex(@"^[a-zA-Z0-9_-]{3,20}$", RegexOptions.Compiled);
        private static readonly Regex EmailPattern = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
        private static readonly Regex DisplayNamePattern = new Regex(@"^[\w\s'-]{2,30}$", RegexOptions.Compiled);
        
        // Profanity filter (basic educational-safe list)
        private static readonly string[] ProfanityList = {
            "badword1", "badword2", "inappropriate" // Placeholder - expand as needed
        };

        #region Username Validation

        /// <summary>
        /// Validates username format for educational platform.
        /// Requirements: 3-20 characters, alphanumeric, underscore, or hyphen.
        /// </summary>
        /// <param name="username">Username to validate</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateUsername(string username, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Username cannot be empty.";
                return false;
            }

            if (username.Length < 3)
            {
                errorMessage = "Username must be at least 3 characters long.";
                return false;
            }

            if (username.Length > 20)
            {
                errorMessage = "Username cannot exceed 20 characters.";
                return false;
            }

            if (!UsernamePattern.IsMatch(username))
            {
                errorMessage = "Username can only contain letters, numbers, underscores, and hyphens.";
                return false;
            }

            if (ContainsProfanity(username))
            {
                errorMessage = "Username contains inappropriate content.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Quick validation without detailed error message.
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            return ValidateUsername(username, out _);
        }

        #endregion

        #region Display Name Validation

        /// <summary>
        /// Validates display name for student profiles.
        /// Requirements: 2-30 characters, letters, numbers, spaces, apostrophes, hyphens.
        /// </summary>
        /// <param name="displayName">Display name to validate</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateDisplayName(string displayName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                errorMessage = "Display name cannot be empty.";
                return false;
            }

            string trimmed = displayName.Trim();

            if (trimmed.Length < 2)
            {
                errorMessage = "Display name must be at least 2 characters long.";
                return false;
            }

            if (trimmed.Length > 30)
            {
                errorMessage = "Display name cannot exceed 30 characters.";
                return false;
            }

            if (!DisplayNamePattern.IsMatch(trimmed))
            {
                errorMessage = "Display name contains invalid characters.";
                return false;
            }

            if (ContainsProfanity(trimmed))
            {
                errorMessage = "Display name contains inappropriate content.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Quick validation without detailed error message.
        /// </summary>
        public static bool IsValidDisplayName(string displayName)
        {
            return ValidateDisplayName(displayName, out _);
        }

        #endregion

        #region Email Validation

        /// <summary>
        /// Validates email format for parent/teacher accounts.
        /// Uses standard RFC 5322 simplified pattern.
        /// </summary>
        /// <param name="email">Email address to validate</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateEmail(string email, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Email address cannot be empty.";
                return false;
            }

            if (!EmailPattern.IsMatch(email))
            {
                errorMessage = "Invalid email format. Please use format: example@domain.com";
                return false;
            }

            if (email.Length > 100)
            {
                errorMessage = "Email address is too long (max 100 characters).";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Quick validation without detailed error message.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            return ValidateEmail(email, out _);
        }

        #endregion

        #region Numeric Validation

        /// <summary>
        /// Validates numeric range for game parameters.
        /// Ensures values are within acceptable bounds.
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="min">Minimum allowed value</param>
        /// <param name="max">Maximum allowed value</param>
        /// <param name="parameterName">Name of parameter for error message</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateRange(float value, float min, float max, string parameterName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (float.IsNaN(value))
            {
                errorMessage = $"{parameterName} is not a valid number.";
                return false;
            }

            if (float.IsInfinity(value))
            {
                errorMessage = $"{parameterName} cannot be infinity.";
                return false;
            }

            if (value < min || value > max)
            {
                errorMessage = $"{parameterName} must be between {min} and {max}. Got: {value}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates integer range.
        /// </summary>
        public static bool ValidateRange(int value, int min, int max, string parameterName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (value < min || value > max)
            {
                errorMessage = $"{parameterName} must be between {min} and {max}. Got: {value}";
                return false;
            }

            return true;
        }

        #endregion

        #region Content Filtering

        /// <summary>
        /// Checks text for inappropriate content.
        /// Educational platform requires strict content filtering.
        /// </summary>
        /// <param name="text">Text to check</param>
        /// <returns>True if profanity detected, false otherwise</returns>
        public static bool ContainsProfanity(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string lowerText = text.ToLowerInvariant();

            foreach (var word in ProfanityList)
            {
                if (lowerText.Contains(word.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sanitizes text by removing special characters while preserving letters, numbers, and basic punctuation.
        /// Useful for preventing injection attacks and ensuring data integrity.
        /// </summary>
        /// <param name="text">Text to sanitize</param>
        /// <returns>Sanitized text</returns>
        public static string SanitizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Allow letters, numbers, spaces, and basic punctuation
            var sanitized = Regex.Replace(text, @"[^\w\s.,'!?-]", "");
            return sanitized.Trim();
        }

        #endregion

        #region Currency and Resources

        /// <summary>
        /// Validates currency/resource amounts for anti-cheat protection.
        /// Prevents overflow and negative values.
        /// </summary>
        /// <param name="amount">Amount to validate</param>
        /// <param name="resourceName">Name of resource for error message</param>
        /// <param name="maxAllowed">Maximum allowed value for this resource</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateCurrencyAmount(int amount, string resourceName, out string errorMessage, int maxAllowed = -1)
        {
            errorMessage = string.Empty;

            if (amount < 0)
            {
                errorMessage = $"{resourceName} amount cannot be negative. Got: {amount}";
                return false;
            }

            // Use provided max or default to MaxCoins
            int maxLimit = maxAllowed > 0 ? maxAllowed : GameConstants.MaxCoins;

            if (amount > maxLimit)
            {
                errorMessage = $"{resourceName} amount exceeds maximum allowed ({maxLimit}). Got: {amount}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates resource spending transaction.
        /// Ensures user has sufficient resources.
        /// </summary>
        /// <param name="currentAmount">Current resource amount</param>
        /// <param name="spendAmount">Amount to spend</param>
        /// <param name="resourceName">Name of resource</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid transaction, false otherwise</returns>
        public static bool ValidateResourceSpending(int currentAmount, int spendAmount, string resourceName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (spendAmount < 0)
            {
                errorMessage = $"Cannot spend negative {resourceName}.";
                return false;
            }

            if (spendAmount > currentAmount)
            {
                errorMessage = $"Insufficient {resourceName}. Have: {currentAmount}, Need: {spendAmount}";
                return false;
            }

            return true;
        }

        #endregion

        #region File Path Validation

        /// <summary>
        /// Validates resource path for security.
        /// Prevents directory traversal attacks.
        /// </summary>
        /// <param name="path">Resource path to validate</param>
        /// <param name="errorMessage">Detailed error message if invalid</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateResourcePath(string path, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                errorMessage = "Resource path cannot be empty.";
                return false;
            }

            // Prevent directory traversal
            if (path.Contains("..") || path.Contains("//") || path.Contains("\\\\"))
            {
                errorMessage = "Invalid resource path: Contains unsafe characters.";
                return false;
            }

            // Check for absolute paths (Resources should be relative)
            if (path.StartsWith("/") || path.Contains(":"))
            {
                errorMessage = "Resource path must be relative, not absolute.";
                return false;
            }

            return true;
        }

        #endregion

        #region Configuration Validation

        /// <summary>
        /// Validates GameConfiguration for internal consistency.
        /// Ensures all values are within acceptable ranges.
        /// </summary>
        /// <param name="config">Configuration to validate</param>
        /// <param name="errors">List of validation errors</param>
        /// <returns>True if configuration is valid, false otherwise</returns>
        public static bool ValidateGameConfiguration(GameConfiguration config, out string[] errors)
        {
            var errorList = new System.Collections.Generic.List<string>();

            if (config == null)
            {
                errors = new[] { "Configuration is null." };
                return false;
            }

            // Validate starting values
            if (config.StartingCoins < 0 || config.StartingCoins > GameConstants.MaxCoins)
                errorList.Add($"Invalid StartingCoins: {config.StartingCoins}");

            if (config.StartingHappiness < 0f || config.StartingHappiness > 100f)
                errorList.Add($"Invalid StartingHappiness: {config.StartingHappiness}");

            // Validate rewards
            if (config.HomeworkCoinReward < 0 || config.HomeworkCoinReward > 1000)
                errorList.Add($"Invalid HomeworkCoinReward: {config.HomeworkCoinReward}");

            if (config.HomeworkExperienceReward < 0 || config.HomeworkExperienceReward > 1000)
                errorList.Add($"Invalid HomeworkExperienceReward: {config.HomeworkExperienceReward}");

            // Validate eye scale range
            if (config.MinEyeScale >= config.MaxEyeScale)
                errorList.Add($"Invalid eye scale range: Min ({config.MinEyeScale}) >= Max ({config.MaxEyeScale})");

            // Validate auto-save interval
            if (config.AutoSaveInterval < 10f || config.AutoSaveInterval > 600f)
                errorList.Add($"Invalid AutoSaveInterval: {config.AutoSaveInterval} (should be 10-600 seconds)");

            errors = errorList.ToArray();
            return errorList.Count == 0;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Logs validation error in a consistent format.
        /// </summary>
        /// <param name="context">Context where validation failed</param>
        /// <param name="errorMessage">Error message</param>
        public static void LogValidationError(string context, string errorMessage)
        {
            Debug.LogWarning($"[Validation] {context}: {errorMessage}");
        }

        /// <summary>
        /// Throws exception for critical validation failures.
        /// Use for internal consistency checks.
        /// </summary>
        /// <param name="condition">Condition that must be true</param>
        /// <param name="message">Error message if condition is false</param>
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException($"[Validation Assert Failed] {message}");
            }
        }

        #endregion
    }
}
