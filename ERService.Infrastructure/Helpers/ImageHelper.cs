using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ERService.Infrastructure.Helpers
{
    public static class ImageHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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

        /// <summary>
        /// Save visual WPF element to image file
        /// </summary>
        /// <param name="frameworkElement">Visual element</param>
        /// <param name="bitmapEncoder">Encoder to be used</param>
        /// <param name="stream">Stream to save result</param>        
        public static void SaveVisualToStream(FrameworkElement frameworkElement, BitmapEncoder bitmapEncoder, Stream stream, Action<FrameworkElement> reDrawCallback = null)
        {
            CreateViewBox(frameworkElement, reDrawCallback);
            var bitmap = new RenderTargetBitmap((int)frameworkElement.ActualWidth, (int)frameworkElement.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(frameworkElement);
            var frame = BitmapFrame.Create(bitmap);
            bitmapEncoder.Frames.Add(frame);
            
            bitmapEncoder.Save(stream);
        }

        /// <summary>
        /// Create container for Framework element
        /// </summary>
        /// <param name="frameworkElement"></param>
        /// <param name="redrawCallback">Action called after element added to container</param>
        /// <returns></returns>
        private static Viewbox CreateViewBox(FrameworkElement frameworkElement, Action<FrameworkElement> redrawCallback = null)
        {
            var viewBox = new Viewbox();
            viewBox.Child = frameworkElement;
            viewBox.Measure(frameworkElement.RenderSize);
            viewBox.Arrange(new Rect(new Point(0, 0), frameworkElement.RenderSize));
            redrawCallback?.Invoke(frameworkElement);
            viewBox.UpdateLayout();

            return viewBox; 
        }        
    }
}
