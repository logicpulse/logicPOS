namespace LogicPOS.UI.Application.Enums
{
    public enum AppOperationMode
    {
        Undefined,
        Default,
        Retail,
        Parking,
        Bakery,
        Butchery,
        Cafe,
        ClothingStore,
        HardwareStore,
        Restaurant,
        SeafoodShop,
        BackOfficeMode,
        ShoeStore
    }

    public static class AppOperationModeExtensions
    {
        public static bool IsBackOfficeMode(this AppOperationMode mode)
        {
            return mode == AppOperationMode.BackOfficeMode;
        }

        public static bool IsRetailMode(this AppOperationMode mode)
        {
            return mode == AppOperationMode.Retail ||
                   mode == AppOperationMode.Parking ||
                   mode == AppOperationMode.Bakery ||
                   mode == AppOperationMode.Butchery ||
                   mode == AppOperationMode.ClothingStore ||
                   mode == AppOperationMode.HardwareStore ||
                   mode == AppOperationMode.SeafoodShop ||
                   mode == AppOperationMode.ShoeStore;
        }

        public static bool IsDefaultMode(this AppOperationMode mode) => IsRetailMode(mode) == false;
 
        public static bool IsParkingMode(this AppOperationMode mode)
        {
            return mode == AppOperationMode.Parking;
        }

        public static AppOperationMode FromString(string mode)
        {
            switch (mode.ToLower())
            {
                case "retail":
                    return AppOperationMode.Retail;
                case "parking":
                    return AppOperationMode.Parking;
                case "bakery":
                    return AppOperationMode.Bakery;
                case "butchery":
                    return AppOperationMode.Butchery;
                case "cafe":
                    return AppOperationMode.Cafe;
                case "clothingstore":
                    return AppOperationMode.ClothingStore;
                case "hardwarestore":
                    return AppOperationMode.HardwareStore;
                case "restaurant":
                    return AppOperationMode.Restaurant;
                case "seafoodshop":
                    return AppOperationMode.SeafoodShop;
                case "backofficemode":
                    return AppOperationMode.BackOfficeMode;
                case "shoestore":
                    return AppOperationMode.ShoeStore;
                default:
                    return AppOperationMode.Default;
            }
        }

        public static string GetTheme(this AppOperationMode mode)
        {
            switch (mode)
            {
                case AppOperationMode.Retail:
                    return "Retail";
                case AppOperationMode.Parking:
                    return "Retail";
                case AppOperationMode.Bakery:
                    return "Retail";
                case AppOperationMode.Butchery:
                    return "Retail";
                case AppOperationMode.Cafe:
                    return "Default";
                case AppOperationMode.ClothingStore:
                    return "Retail";
                case AppOperationMode.HardwareStore:
                    return "Retail";
                case AppOperationMode.Restaurant:
                    return "Default";
                case AppOperationMode.SeafoodShop:
                    return "Retail";
                case AppOperationMode.BackOfficeMode:
                    return "Retail";
                case AppOperationMode.ShoeStore:
                    return "Retail";
                case AppOperationMode.Default:
                default:
                    return "Default";
            }
        }
    }
}
