using System;

namespace ERService.OrderModule.OrderNumeration
{
    //TODO: Refactor make interface
    internal class OrderNumberGenerator
    {
        internal static string GetNumberFromPattern(string pattern)
        {            
            return GenerateNumber(pattern);
        }

        private static string GenerateNumber(string pattern)
        {
            string result = pattern.Replace("[MM]", DateTime.Now.ToString("MM"))
                            .Replace("[RRRR]", DateTime.Now.Year.ToString());

            return result;
        }
    }
}