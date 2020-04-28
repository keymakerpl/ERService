using System;

namespace ERService.OrderModule.OrderNumeration
{
    public static class OrderNumberGenerator
    {
        public static string GetNumberFromPattern(string pattern, string userInitials = null)
        {            
            return GenerateNumber(pattern, userInitials);
        }

        private static string GenerateNumber(string pattern, string userInitials = null)
        {
            string result = pattern
                                    .Replace("[DD]", DateTime.Now.ToString("dd"))
                                    .Replace("[MM]", DateTime.Now.ToString("MM"))
                                    .Replace("[RRRR]", DateTime.Now.ToString("yyyy"))
                                    .Replace("[RR]", DateTime.Now.ToString("yy"));

            if (pattern.Contains("[USER]") && !String.IsNullOrWhiteSpace(userInitials))
                result = result
                                    .Replace("[USER]", userInitials);

            return result;
        }
    }
}