using LogicPOS.Licence;

namespace LogicPOS.Core;

public static class MachineIdentity
{
    public const string AppName = "logicpos";

    public static string HardwareId => HardwareIdProvider.GetHardwareId(AppName);
}
