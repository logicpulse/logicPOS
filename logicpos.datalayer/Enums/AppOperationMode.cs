using System.Collections.Generic;
using System.Linq;

namespace logicpos.datalayer.Enums
{
    public enum AppOperationMode {
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

    public class CustomAppOperationMode
    { 
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly CustomAppOperationMode  UNDEFINED      = new CustomAppOperationMode(string.Empty, string.Empty, string.Empty, string.Empty);
        public static readonly CustomAppOperationMode  DEFAULT        = new CustomAppOperationMode("Default", "pt", "databasedatademo.sql", "Default");
        public static readonly CustomAppOperationMode  RETAIL         = new CustomAppOperationMode("Retail", "pt", "databasedatademo.sql", "Retail");
        public static readonly CustomAppOperationMode  PARKING        = new CustomAppOperationMode("Parking", "pt", "databasedatademo_parking.sql", "Retail");
        public static readonly CustomAppOperationMode  BAKERY         = new CustomAppOperationMode("Bakery", "pt", "databasedatademo_bakery.sql", "Retail");
        public static readonly CustomAppOperationMode  BUTCHERY       = new CustomAppOperationMode("Butchery", "pt", "databasedatademo_butchery.sql", "Retail");
        public static readonly CustomAppOperationMode  CAFE           = new CustomAppOperationMode("Cafe", "pt", "databasedatademo_cafe.sql", "Default");
        public static readonly CustomAppOperationMode  CLOTHING_STORE = new CustomAppOperationMode("ClothingStore", "pt", "databasedatademo_clothing_store.sql", "Retail");
        public static readonly CustomAppOperationMode  HARDWARE_STORE = new CustomAppOperationMode("HardwareStore", "pt", "databasedatademo_hardware_store.sql", "Retail");
        public static readonly CustomAppOperationMode  RESTAURANT     = new CustomAppOperationMode("Restaurant", "pt", "databasedatademo_restaurant.sql", "Default");
        public static readonly CustomAppOperationMode  SEAFOOD_SHOP   = new CustomAppOperationMode("SeafoodShop", "pt", "databasedatademo_seafood_shop.sql", "Retail");
        public static readonly CustomAppOperationMode  SHOE_STORE     = new CustomAppOperationMode("ShoeStore", "pt", "databasedatademo_shoe_store.sql", "Retail");
        //TK016235 BackOffice - Mode
		public static readonly CustomAppOperationMode  BACKOFFICE     = new CustomAppOperationMode("BackOfficeMode", "pt", "databasedatademo.sql", "Default");

        public string AppOperationModeToken { get; private set; }
        public string CustomCultureDefaultCountryPrefix { get; private set; }
        public string DatabaseDemoFileName { get; private set; }
        public string AppOperationTheme { get; private set; }

        CustomAppOperationMode(string appOperationModeToken, string customCultureDefaultCountryPrefix, string databaseDemoFileName, string appOperationTheme)
        {
            AppOperationModeToken = appOperationModeToken;
            CustomCultureDefaultCountryPrefix = customCultureDefaultCountryPrefix;
            DatabaseDemoFileName = databaseDemoFileName;
            AppOperationTheme = appOperationTheme;
        }

        public static IEnumerable<CustomAppOperationMode> Values
        {
            get
            {
                yield return UNDEFINED;
                yield return DEFAULT;
                yield return RETAIL;
                yield return PARKING;
                yield return BAKERY;
                yield return BUTCHERY;
                yield return CAFE;
                yield return CLOTHING_STORE;
                yield return HARDWARE_STORE;
                yield return RESTAURANT;
                yield return SEAFOOD_SHOP;
                yield return SHOE_STORE;
				//TK016235 BackOffice - Mode
                yield return BACKOFFICE;
            }
        }

        public static CustomAppOperationMode GetAppOperationMode(string appOperationModeToken)
        {
            CustomAppOperationMode result = CustomAppOperationMode.DEFAULT;

            if (!string.IsNullOrEmpty(appOperationModeToken))
            {
                result = GetCustomAppOperationMode(appOperationModeToken);
            }

            return result;
        }

        private static CustomAppOperationMode GetCustomAppOperationMode(string token)
        {
            return Values.First(c => c.AppOperationModeToken.Equals(token));
        }
    }
}
