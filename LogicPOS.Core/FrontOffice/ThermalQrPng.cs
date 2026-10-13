using QrCodes;
using QrCodes.Renderers;
using QrCodes.Renderers.Abstractions;
using SkiaSharp;
using ZXing;
using ZXing.QrCode;
using ZXing.SkiaSharp.Rendering;

namespace LogicPOS.Core.FrontOffice;

/// <summary>PNG QR for thermal ticket preview and ESC/POS image mode (QRCODE_METHOD=1).</summary>
public static class ThermalQrPng
{
    public static byte[] Render(string text, int size = 220)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<byte>();
        }

        size = Math.Clamp(size, 96, 400);
        try
        {
            var writer = new BarcodeWriter<SKBitmap>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = size,
                    Height = size,
                    Margin = 1,
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M
                },
                Renderer = new SKBitmapRenderer
                {
                    Background = SKColors.White,
                    Foreground = SKColors.Black
                }
            };
            using var bitmap = writer.Write(text);
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 90);
            return data.ToArray();
        }
        catch
        {
            try
            {
                var qrCode = QrCodeGenerator.Generate(text, ErrorCorrectionLevel.Medium, forceUtf8: true);
                return new SkiaSharpRenderer().RenderToBytes(qrCode, new RendererSettings
                {
                    PixelsPerModule = Math.Max(3, size / 40),
                    DrawQuietZones = true,
                    FileFormat = FileFormat.Png
                });
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }
    }
}
