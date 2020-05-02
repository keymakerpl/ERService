using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ERService.Infrastructure.Helpers
{
    public static class ImageHelper
    {
        public static Task<BitmapImage> GenerateBitmap(string file, int scale)
        {
            return Task.Run(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(file);
                image.DecodePixelWidth = scale;
                image.EndInit();
                image.Freeze();

                return image;
            });
        }

        public static Task<BitmapImage> GenerateBitmap(Stream stream, int scale)
        {
            return Task.Run(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.DecodePixelWidth = scale;
                image.EndInit();
                image.Freeze();

                return image;
            });
        }
    }
}
