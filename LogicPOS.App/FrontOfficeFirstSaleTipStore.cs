namespace LogicPOS.App;

/// <summary>
/// One-time "first sale" coach mark. Stored beside the app (same pattern as ui-culture.txt).
/// </summary>
public static class FrontOfficeFirstSaleTipStore
{
    private static string Path => System.IO.Path.Combine(AppContext.BaseDirectory, "fo-first-sale-tip.seen");

    public static bool IsSeen()
    {
        try
        {
            return File.Exists(Path);
        }
        catch
        {
            return false;
        }
    }

    public static void MarkSeen()
    {
        try
        {
            File.WriteAllText(Path, "1");
        }
        catch
        {
            // Tip may reappear if the install folder is not writable.
        }
    }
}
