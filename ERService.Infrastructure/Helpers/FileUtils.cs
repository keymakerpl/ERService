using System;
using System.IO;

namespace ERService.Infrastructure.Helpers
{
    public static class FileUtils
    {
        public static byte[] GetFileBinary(string fileName)
        {
            byte[] fileBytes;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                fileBytes = new byte[fs.Length];
                fs.Read(fileBytes, 0, Convert.ToInt32(fs.Length));
            }

            return fileBytes;
        }
    }
}
