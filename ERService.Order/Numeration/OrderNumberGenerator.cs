using System;

namespace ERService.OrderModule.OrderNumeration
{
    //TODO: Refactor make interface
    public static class OrderNumberGenerator
    {
        public static string GetNumberFromPattern(string pattern, string userInitials = null)
        {            
            return GenerateNumber(pattern, userInitials);
        }

        private static string GenerateNumber(string pattern, string userInitials = null)
        {
            string result = pattern
                                    .Replace("[MM]", DateTime.Now.ToString("MM"))
                                    .Replace("[RRRR]", DateTime.Now.Year.ToString());

            if (!String.IsNullOrWhiteSpace(userInitials))
                result
                                    .Replace("[USER]", userInitials);

            return result;
        }
    }
}