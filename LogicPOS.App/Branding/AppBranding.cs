using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LogicPOS.Core;
using LogicPOS.Core.Licensing;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.App.Branding;

/// <summary>
/// GTK parity: licence <c>reseller</c> (set by software key) selects branded logos under
/// Assets/Images/Branding/{NT|SW|IM}/. LogicPulse uses the default logicpos logos
/// (logicpos_logo.png / logicpos_logo_simple.png), not the encrypted GTK branding pack.
/// </summary>
public static class AppBranding
{
    private const string DefaultBrandCode = "Logicpulse";
    private const string DefaultLoginUri = "avares://LogicPOS.App/Assets/Images/logicpos_logo.png";
    private const string DefaultSimpleUri = "avares://LogicPOS.App/Assets/Images/logicpos_logo_simple.png";

    public static string Reseller
    {
        get
        {
            var reseller = AppComposition.Services?.GetService<ILicenseModule>()?.Reseller;
            return string.IsNullOrWhiteSpace(reseller) ? "LogicPulse" : reseller.Trim();
        }
    }

    public static string BrandCode => ResolveBrandCode(Reseller);

    public static string FormatPoweredBy(string productVersion)
    {
        var version = string.IsNullOrWhiteSpace(productVersion) ? "1.6.0" : productVersion.Trim().TrimStart('v', 'V');
        var reseller = Reseller;
        if (IsDefaultLogicPulse(reseller))
        {
            return $"Powered by LogicPulse Technologies © Vers. {version}";
        }

        return $"Powered by {reseller} © Vers. {version}";
    }

    public static Bitmap LoadLoginLogo()
        => BrandCode == DefaultBrandCode
            ? LoadAvaresBitmap(DefaultLoginUri, applyBrandingDecoder: false)
            : LoadBrandBitmap("login.png", DefaultLoginUri);

    public static Bitmap LoadSimpleLogo()
        => BrandCode == DefaultBrandCode
            ? LoadAvaresBitmap(DefaultSimpleUri, applyBrandingDecoder: false)
            : LoadBrandBitmap("front.png", DefaultSimpleUri);

    public static string ResolveBrandCode(string? reseller)
    {
        if (string.IsNullOrWhiteSpace(reseller))
        {
            return DefaultBrandCode;
        }

        var name = reseller.Trim();
        if (string.Equals(name, "NewTech", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("NewTech", StringComparison.OrdinalIgnoreCase))
        {
            return "NT";
        }

        if (string.Equals(name, "SwissConsultings GmbH", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("SwissConsulting", StringComparison.OrdinalIgnoreCase))
        {
            return "SW";
        }

        if (name.Contains("Informur", StringComparison.OrdinalIgnoreCase))
        {
            return "IM";
        }

        return DefaultBrandCode;
    }

    private static bool IsDefaultLogicPulse(string reseller)
        => reseller.Contains("LogicPulse", StringComparison.OrdinalIgnoreCase) ||
           reseller.Contains("Logicpulse", StringComparison.OrdinalIgnoreCase);

    private static Bitmap LoadBrandBitmap(string fileName, string fallbackAvares)
    {
        var brandUri = $"avares://LogicPOS.App/Assets/Images/Branding/{BrandCode}/{fileName}";
        try
        {
            return LoadAvaresBitmap(brandUri, applyBrandingDecoder: true);
        }
        catch
        {
            // Fall through to default embedded logos.
        }

        // Optional disk overlay next to the executable (installer / custom drop).
        try
        {
            var diskPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Images", "Branding", BrandCode, fileName);
            if (File.Exists(diskPath))
            {
                return BitmapFromBytes(DecodeBrandingBytes(File.ReadAllBytes(diskPath)));
            }
        }
        catch
        {
            // Ignore and use default.
        }

        return LoadAvaresBitmap(fallbackAvares, applyBrandingDecoder: false);
    }

    private static Bitmap LoadAvaresBitmap(string avaresUri, bool applyBrandingDecoder)
    {
        using var stream = AssetLoader.Open(new Uri(avaresUri));
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        var raw = memory.ToArray();
        if (applyBrandingDecoder == false)
        {
            return BitmapFromBytes(raw);
        }

        return BitmapFromBytes(DecodeBrandingBytes(raw));
    }

    private static Bitmap BitmapFromBytes(byte[] bytes)
        => new(new MemoryStream(bytes));

    /// <summary>
    /// GTK used BrandingItens.Conversor.EncodeObj on encrypted themed PNGs.
    /// Default LogicPulse assets are plain PNG — EncodeObj corrupts them, so only keep the
    /// result when it still looks like an image.
    /// </summary>
    private static byte[] DecodeBrandingBytes(byte[] bytes)
    {
        var encoded = TryBrandingItensEncode(bytes);
        if (encoded is null || encoded.Length == 0)
        {
            return bytes;
        }

        if (LooksLikeImage(encoded))
        {
            return encoded;
        }

        return bytes;
    }

    private static bool LooksLikeImage(byte[] bytes)
    {
        if (bytes.Length < 8)
        {
            return false;
        }

        // PNG
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
        {
            return true;
        }

        // JPEG
        if (bytes[0] == 0xFF && bytes[1] == 0xD8)
        {
            return true;
        }

        // BMP
        if (bytes[0] == 0x42 && bytes[1] == 0x4D)
        {
            return true;
        }

        return false;
    }

    private static byte[]? TryBrandingItensEncode(byte[] bytes)
    {
        try
        {
            var dllPath = Path.Combine(AppContext.BaseDirectory, "BrandingItens.dll");
            if (File.Exists(dllPath) == false)
            {
                return null;
            }

            var assembly = System.Reflection.Assembly.LoadFrom(dllPath);
            var type = assembly.GetType("BrandingItens.Conversor");
            if (type is null)
            {
                return null;
            }

            var ctor = type.GetConstructor([typeof(byte[])]);
            var encode = type.GetMethod("EncodeObj", [typeof(byte[])]);
            if (ctor is null || encode is null)
            {
                return null;
            }

            var conversor = ctor.Invoke([File.ReadAllBytes(dllPath)]);
            return encode.Invoke(conversor, [bytes]) as byte[];
        }
        catch
        {
            return null;
        }
    }
}
