


using System.Text.RegularExpressions;
using static SubscriptionBillingAndNotificationCore.Utilities.CustomExceptions;

namespace SubscriptionBillingAndNotificationCore.Utilities
{
    public static class Helpers
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }


        /// <summary>
        /// Validates a password based on the following criteria:
        /// - At least 8 characters long
        /// - Contains at least one uppercase letter
        /// - Contains at least one lowercase letter
        /// - Contains at least one digit
        /// - Contains at least one special character (@$!%*?&)
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>True if valid, False otherwise</returns>
        public static bool IsValidPasswords(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";

            return Regex.IsMatch(password, pattern);
        }

        /// <summary>
        /// Validates an email address using regex pattern
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns>True if valid, False otherwise</returns>
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, pattern);
        }
    }
}
