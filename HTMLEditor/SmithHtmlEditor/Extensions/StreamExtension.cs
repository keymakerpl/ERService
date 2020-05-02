using System;
using System.IO;

namespace Smith.WPF.HtmlEditor.Extensions
{
    public static class StreamExtension
    {
        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }
    }
}
