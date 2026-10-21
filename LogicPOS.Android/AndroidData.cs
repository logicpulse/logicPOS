using Android.Content;
using Android.Content.Res;

namespace LogicPOS.Android;

/// <summary>
/// Extracts packaged assets (appsettings + db_seeds) into the app files directory.
/// </summary>
internal static class AndroidData
{
    public static string EnsureDataDirectory(Context context)
    {
        var root = context.FilesDir
            ?? throw new InvalidOperationException("Android FilesDir is unavailable.");
        var rootPath = root.AbsolutePath;
        Directory.CreateDirectory(rootPath);

        CopyAssetIfMissing(context.Assets, "appsettings.json", Path.Combine(rootPath, "appsettings.json"));
        CopyAssetTree(context.Assets, "db_seeds", Path.Combine(rootPath, "db_seeds"));
        return rootPath;
    }

    private static void CopyAssetIfMissing(AssetManager? assets, string assetPath, string destination)
    {
        if (assets is null || File.Exists(destination))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        using var input = assets.Open(assetPath);
        using var output = File.Create(destination);
        input.CopyTo(output);
    }

    private static void CopyAssetTree(AssetManager? assets, string assetDir, string destinationDir)
    {
        if (assets is null)
        {
            return;
        }

        Directory.CreateDirectory(destinationDir);
        var children = assets.List(assetDir) ?? [];
        if (children.Length == 0)
        {
            // Leaf file path (List returns empty for files on some API levels).
            try
            {
                CopyAssetIfMissing(assets, assetDir, destinationDir);
            }
            catch
            {
                // Not a file; ignore.
            }

            return;
        }

        foreach (var child in children)
        {
            if (string.IsNullOrWhiteSpace(child))
            {
                continue;
            }

            var childAsset = $"{assetDir}/{child}";
            var nested = assets.List(childAsset) ?? [];
            if (nested.Length > 0)
            {
                CopyAssetTree(assets, childAsset, Path.Combine(destinationDir, child));
            }
            else
            {
                CopyAssetIfMissing(assets, childAsset, Path.Combine(destinationDir, child));
            }
        }
    }
}
