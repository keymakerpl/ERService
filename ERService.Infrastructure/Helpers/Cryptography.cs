using System;
using System.IO;
using System.Security.Cryptography;

namespace ERService.Infrastructure.Helpers
{
    public static class Cryptography
    {
        public static string CalculateMD5(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", String.Empty).ToLowerInvariant();
                }
            }
        }
    }
}
