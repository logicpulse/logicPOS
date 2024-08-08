using System;
using System.Drawing;
using System.IO;

namespace LogicPOS.Settings
{
    public partial class AppSettings
    {
        private static AppSettings _instance;

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    Initialize();
                }
                return _instance;
            }
        }

        private static void Initialize()
        {
            var json = File.ReadAllText("appsettings.json");
            _instance = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(json, new ColorConverter());
        }

        #region Properties
        public string appSystemProtection { get; set; }
        public bool appMultiUserEnvironment { get; set; }
        public Size appScreenSize { get; set; }
        public int appScreen { get; set; }
        public bool appThemeCalcDynamicSize { get; set; }
        public string appOperationModeToken { get; set; }
        public string customCultureResourceDefinition { get; set; }
        public string cultureFinancialRules { get; set; }
        public string databaseType { get; set; }
        public string databaseName { get; set; }
        public string xpoConnectionString { get; set; }
        public bool generatePdfDocuments { get; set; }
        public bool useImageOverlay { get; set; }
        public bool appShowMinimize { get; set; }
        public bool sendDocumentsATinRealTime { get; set; }
        public bool sendDocumentsATinRealTimeWB { get; set; }
        public bool appUseParkingTicketModule { get; set; }
        public bool useVirtualKeyBoard { get; set; }
        public bool useBaseDialogWindowMask { get; set; }
        public int posBaseButtonMaxCharsPerLabel { get; set; }
        public bool useDatabaseDataDemo { get; set; }
        public string dateTimeFormatStatusBar { get; set; }
        public string pathAssets { get; set; }
        public string pathImages { get; set; }
        public string pathThemes { get; set; }
        public string pathSounds { get; set; }
        public string pathResources { get; set; }
        public string pathReports { get; set; }
        public string pathTickets { get; set; }
        public string pathKeyboards { get; set; }
        public string pathTemp { get; set; }
        public string pathCache { get; set; }
        public string pathDocuments { get; set; }
        public string pathPlugins { get; set; }
        public string pathCertificates { get; set; }
        public decimal decimalMoneyButtonL1Value { get; set; }
        public decimal decimalMoneyButtonL2Value { get; set; }
        public decimal decimalMoneyButtonL3Value { get; set; }
        public decimal decimalMoneyButtonL4Value { get; set; }
        public decimal decimalMoneyButtonL5Value { get; set; }
        public decimal decimalMoneyButtonR1Value { get; set; }
        public decimal decimalMoneyButtonR2Value { get; set; }
        public decimal decimalMoneyButtonR3Value { get; set; }
        public decimal decimalMoneyButtonR4Value { get; set; }
        public decimal decimalMoneyButtonR5Value { get; set; }
        public string xpoOidConfigurationPaymentConditionDefaultInvoicePaymentCondition { get; set; }
        public string xpoOidConfigurationPaymentMethodDefaultInvoicePaymentMethod { get; set; }
        public string xpoOidConfigurationCountrySystemCountry { get; set; }
        public string xpoOidConfigurationCountrySystemCountryCountryCode2 { get; set; }
        public string xpoOidConfigurationCurrencySystemCurrency { get; set; }
        public string xpoOidConfigurationPlaceTableDefaultOpenTable { get; set; }
        public string fileImageBackgroundWindowStartup { get; set; }
        public string fileImageBackgroundWindowPos { get; set; }
        public string fileImageBackgroundDialogDefault { get; set; }
        public string fileImageBackgroundDialogTables { get; set; }
        public string fileImageDialogBaseMessageTypeImage { get; set; }
        public string fileImageDialogBaseMessageTypeIcon { get; set; }
        public string fileImageBackOfficeLogo { get; set; }
        public string fontPosBaseButtonSize { get; set; }
        public int fontPosToolbarButton { get; set; }
        public string fontPosStatusBar { get; set; }
        public string fontPosStatusBarSmall { get; set; }
        public string fontStartupWindowVersion { get; set; }
        public string fontBaseDialogButton { get; set; }
        public string fontBaseDialogActionAreaButton { get; set; }
        public string fontKeyboardPadTextEntry { get; set; }
        public string fontKeyboardPadPrimaryKey { get; set; }
        public string fontKeyboardPadSecondaryKey { get; set; }
        public string fontBackOfficeStatusBar { get; set; }
        public string fontNumberPadPinButtonKeysTextAndLabel { get; set; }
        public string fontMoneyPadButtonKeys { get; set; }
        public string fontMoneyPadTextEntry { get; set; }
        public string fontTicketPadPadButtonKeys { get; set; }
        public string fontTicketListColumnTitle { get; set; }
        public string fontTicketListColumn { get; set; }
        public string fontTicketListLabelLabelTotal { get; set; }
        public string fontTicketListLabelTotal { get; set; }
        public string fontPosBackOfficeParent { get; set; }
        public string fontPosBackOfficeChild { get; set; }
        public string fontPosBackOfficeParent_1024 { get; set; }
        public string fontPosBackOfficeChild_1024 { get; set; }
        public string fontTableDialogTableNumber { get; set; }
        public string fontEntryBoxLabel { get; set; }
        public string fontEntryBoxValue { get; set; }
        public string fontGenericTreeViewColumnTitle { get; set; }
        public string fontGenericTreeViewColumn { get; set; }
        public string fontGenericTreeViewSelectRecordColumnTitle { get; set; }
        public int fontGenericTreeViewSelectRecordColumn { get; set; }
        public string fontGenericTreeViewFinanceDocumentArticleColumnTitle { get; set; }
        public string fontGenericTreeViewFinanceDocumentArticleColumn { get; set; }
        public string fontPagePadNavigatorButton { get; set; }
        public string fontSplitPaymentTouchButtonSplitPayment { get; set; }
        public bool requireToChooseVatExemptionReason { get; set; }
        public int intStartupWindowObjectsNumberPadPinRight { get; set; }
        public int intPosMainWindowComponentsMargin { get; set; }
        public int intPosMainWindowEventBoxStatusBar1And2Height { get; set; }
        public int intSplitPaymentTouchButtonSplitPaymentHeight { get; set; }
        public Color colorPosHelperBoxsBackground { get; set; } 
        public Color colorPosStatusBar1Background { get; set; }
        public Color colorPosStatusBar2Background { get; set; }
        public Color colorPosStatusBarFont { get; set; }
        public Color colorPosStatusBarFontSmall { get; set; }
        public Color colorTicketPadButtonFont { get; set; }
        public Color colorFullScreenBackground { get; set; }
        public Color colorFullScreenUsefullAreaBackground { get; set; }
        public Color colorBaseDialogTitleBackground { get; set; }
        public Color colorBaseDialogWindowBackground { get; set; }
        public Color colorBaseDialogWindowBackgroundBorder { get; set; }
        public Color colorBaseDialogActionAreaButtonFont { get; set; }
        public Color colorBaseDialogActionAreaButtonBackground { get; set; }
        public Color colorBaseDialogEntryBoxBackground { get; set; }
        public Color colorBaseDialogDefaultButtonFont { get; set; }
        public Color colorBaseDialogDefaultButtonBackground { get; set; }
        public Color colorBaseDialogSecondaryButtonBackground { get; set; }
        public Color colorBaseDialogSecondaryButtonFont { get; set; }
        public Color colorBaseDialogEmptyButtonBackground { get; set; }
        public Color colorPosPaymentsDialogTotalPannelBackground { get; set; }
        public Color colorPosToolbarDefaultButtonFont { get; set; }
        public Color colorPosTablePadTableTableStatusOpenButtonBackground { get; set; }
        public Color colorPosTablePadTableTableStatusReservedButtonBackground { get; set; }
        public Color colorPosTicketListModeTicketBackground { get; set; }
        public Color colorPosTicketListModeOrderMainBackground { get; set; }
        public Color colorPosTicketListModeEditBackground { get; set; }
        public Color colorPosNumberPadLeftButtonBackground { get; set; }
        public Color colorPosNumberRightButtonBackground { get; set; }
        public Color colorEntryValidationValidFont { get; set; }
        public Color colorEntryValidationInvalidFont { get; set; }
        public Color colorEntryValidationInvalidFontLighter { get; set; }
        public Color colorEntryValidationValidBackground { get; set; }
        public Color colorEntryValidationInvalidBackground { get; set; }
        public Color colorKeyboardPadKeyDefaultFont { get; set; }
        public Color colorKeyboardPadKeySecondaryFont { get; set; }
        public Color colorKeyboardPadKeyBackground { get; set; }
        public Color colorKeyboardPadKeyBackgroundActive { get; set; }
        public Color colorBackOfficeContentBackground { get; set; }
        public Color colorBackOfficeAccordionFixBackground { get; set; }
        public Color colorBackOfficeStatusBarBackground { get; set; }
        public Color colorBackOfficeStatusBarBottomBackground { get; set; }
        public Color colorBackOfficeStatusBarFont { get; set; }
        public Color colorSplitPaymentTouchButtonFilledDataBackground { get; set; }
        public Color colorPagePadHotButtonBackground { get; set; }
        public Point positionButtonFavorites { get; set; }
        public Point positionTablePadFamily { get; set; }
        public Point positionTablePadSubFamily { get; set; }
        public Point positionTablePadArticle { get; set; }
        public string tableConfigTablePadFamily { get; set; }
        public string tableConfigTablePadSubFamily { get; set; }
        public string tableConfigTablePadArticle { get; set; }
        public string tableConfigTablePadLoginUser { get; set; }
        public Size sizePosBaseButton { get; set; }
        public Size sizePosToolbarButton { get; set; }
        public Size sizePosTicketPadButton { get; set; }
        public Size sizePosTicketPadButtonDoubleWidth { get; set; }
        public Size sizePosToolbarButtonIcon { get; set; }
        public Size sizePosTicketPadButtonIcon { get; set; }
        public Size sizePosSmallButtonScroller { get; set; }
        public Size sizePosTableButton { get; set; }
        public Size sizePosUserButton { get; set; }
        public Size sizeBaseDialogDefaultButton { get; set; }
        public Size sizeBaseDialogDefaultButtonIcon { get; set; }
        public Size sizeBaseDialogActionAreaButton { get; set; }
        public Size sizeBaseDialogActionAreaButtonIcon { get; set; }
        public Size sizeBaseDialogActionAreaBackOfficeNavigatorButton { get; set; }
        public Size sizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon { get; set; }
        public Size sizeKeyboardPadDefaultKey { get; set; }
        public Size sizePagesPadNavigatorButton { get; set; }
        public Size sizePagesPadNavigatorButtonIcon { get; set; }
        public bool usePDFviewer { get; set; }
        public bool printTicket { get; set; }
        public Size sizeStartupWindowObjectsTablePadUserMarginLeftTop { get; set; }
        public Size sizeStartupWindowObjectsTablePadUserButton { get; set; }
        public Size sizeStartupWindowObjectsTablePadUserTablePadUserButtonPrev { get; set; }
        public Size sizeStartupWindowObjectsNumberPadPin { get; set; }
        public Size sizeStartupWindowObjectsNumberPadPinButton { get; set; }
        public Size sizeStartupWindowObjectsLabelVersion { get; set; }
        public Size sizeStartupWindowObjectsLabelVersionSizeMarginRightBottom { get; set; }
        public bool posPaymentsDialogUseCurrentAccount { get; set; }
        public string ClientSettingsProviderServiceUri { get; set; }
        public string appHardwareId { get; set; }
        public string fontPosBackOfficeParentLowRes { get; set; }
        public string fontPosBackOfficeChildLowRes { get; set; }
        #endregion
    }
}

