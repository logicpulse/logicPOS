namespace LogicPOS.Core;

public static class MachineIdentity
{
    public const string AppName = "logicpos";

    private static Func<string>? _hardwareId;

    public static void UseHardwareId(Func<string> provider) => _hardwareId = provider;

    public static string HardwareId => _hardwareId?.Invoke() ?? LocalId();

    private static string LocalId()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "hardware.id");
        if (File.Exists(path))
        {
            var stored = File.ReadAllText(path).Trim();
            if (stored.Length > 0)
            {
                return stored;
            }
        }

        var created = Guid.NewGuid().ToString("N");
        File.WriteAllText(path, created);
        return created;
    }
}
