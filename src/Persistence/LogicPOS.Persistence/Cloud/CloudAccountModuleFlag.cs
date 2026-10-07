namespace LogicPOS.Persistence.Cloud;

/// <summary>
/// Account module columns are nullable. Only an explicit true enables the module.
/// </summary>
public static class CloudAccountModuleFlag
{
    public static bool FromDatabase(object? value)
    {
        if (value is null || value is DBNull)
            return false;

        if (value is bool enabled)
            return enabled;

        if (value is string text)
            return bool.TryParse(text, out var parsed) && parsed;

        try
        {
            return Convert.ToBoolean(value);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }
}
