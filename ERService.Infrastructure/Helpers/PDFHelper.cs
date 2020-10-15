using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace ERService.Infrastructure.Helpers
{
    public static class PDFHelper
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SaveImageToPDF(Stream imageStream, string filePath, string header = null, byte[] logo = null)
        {
            var document = new Document(PageSize.A4);
            _ = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            try
            {
                if (logo != null)
                {
                    var logoImg = Image.GetInstance(logo);
                    logoImg.ScaleToFit(document.PageSize.Width / 2, document.PageSize.Height / 2);
                    logoImg.Alignment = Element.ALIGN_CENTER;
                    document.Add(logoImg);
                }
                imageStream.Seek(0, SeekOrigin.Begin);
                var img = Image.GetInstance(imageStream);
                img.ScaleToFit(document.PageSize.Width, document.PageSize.Height);
                img.Alignment = Element.ALIGN_CENTER;
                img.SpacingBefore = 24f;
                var font = new Font(Font.COURIER, 14f, Font.BOLD);
                var title = new Paragraph(header ?? "", font);
                title.SetAlignment("Center");
                title.SpacingAfter = 24f;
                title.SpacingBefore = 24f;
                document.Add(title);
                document.Add(img);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw;
            }
            finally
            {
                document.Close();
            }
        }
    }
}
