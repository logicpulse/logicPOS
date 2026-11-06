namespace LogicPOS.Core;

/// <summary>LogicPOS 2.0 product edition chosen at first run (or inferred on upgrade).</summary>
public enum ProductEdition
{
    Community,
    Business,
    Cloud,
}

/// <summary>How the chosen edition is hosted on this machine.</summary>
public enum ProductHosting
{
    /// <summary>Local database on this PC (Community, or Business waiting for licence).</summary>
    Local,

    /// <summary>This PC hosts the local API + portal (Business server).</summary>
    LocalServer,

    /// <summary>This PC is a terminal against an existing API.</summary>
    RemoteApi,

    /// <summary>LogicPulse production cloud API.</summary>
    Cloud,
}

public static class ProductEditionNames
{
    public const string Community = "Community";
    public const string Business = "Business";
    public const string Cloud = "Cloud";

    public static bool TryParse(string? value, out ProductEdition edition)
    {
        if (string.Equals(value, Community, StringComparison.OrdinalIgnoreCase))
        {
            edition = ProductEdition.Community;
            return true;
        }

        if (string.Equals(value, Business, StringComparison.OrdinalIgnoreCase))
        {
            edition = ProductEdition.Business;
            return true;
        }

        if (string.Equals(value, Cloud, StringComparison.OrdinalIgnoreCase))
        {
            edition = ProductEdition.Cloud;
            return true;
        }

        edition = ProductEdition.Community;
        return false;
    }

    public static string ToConfig(ProductEdition edition) => edition switch
    {
        ProductEdition.Business => Business,
        ProductEdition.Cloud => Cloud,
        _ => Community,
    };
}

public static class ProductHostingNames
{
    public const string Local = "Local";
    public const string LocalServer = "LocalServer";
    public const string RemoteApi = "RemoteApi";
    public const string Cloud = "Cloud";

    public static bool TryParse(string? value, out ProductHosting hosting)
    {
        if (string.Equals(value, LocalServer, StringComparison.OrdinalIgnoreCase))
        {
            hosting = ProductHosting.LocalServer;
            return true;
        }

        if (string.Equals(value, RemoteApi, StringComparison.OrdinalIgnoreCase))
        {
            hosting = ProductHosting.RemoteApi;
            return true;
        }

        if (string.Equals(value, Cloud, StringComparison.OrdinalIgnoreCase))
        {
            hosting = ProductHosting.Cloud;
            return true;
        }

        if (string.Equals(value, Local, StringComparison.OrdinalIgnoreCase))
        {
            hosting = ProductHosting.Local;
            return true;
        }

        hosting = ProductHosting.Local;
        return false;
    }

    public static string ToConfig(ProductHosting hosting) => hosting switch
    {
        ProductHosting.LocalServer => LocalServer,
        ProductHosting.RemoteApi => RemoteApi,
        ProductHosting.Cloud => Cloud,
        _ => Local,
    };
}