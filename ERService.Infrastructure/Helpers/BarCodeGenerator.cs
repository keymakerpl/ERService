using NetBarcode;

namespace ERService.Infrastructure.Helpers
{
    public static class BarcodeGenerator
    {
        public static string GenerateBase64String(string text)
        {
            var barcode = new Barcode(text, true, 164, 85);
            return barcode.GetBase64Image();
        }
    }
}
