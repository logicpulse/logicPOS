using System.Drawing;
using System.IO;

namespace LogicPOS.UI.Settings
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
            if (!File.Exists("appsettings.json"))
            {
                throw new FileNotFoundException("The appsettings.json file is missing.");
            }

            var json = File.ReadAllText("appsettings.json");
            _instance = Newtonsoft.Json.JsonConvert.DeserializeObject<AppSettings>(json, new ColorConverter());
        }

        #region Properties
        public Size AppScreenSize { get; set; }

        public int AppScreen { get; set; }

        public bool AppThemeCalcDynamicSize { get; set; }

        public string AppOperationModeToken { get; set; }
        public string CustomCultureResourceDefinition { get; set; }
        public string CultureFinancialRules { get; set; }

        public bool UseImageOverlay { get; set; }
        public bool AppShowMinimize { get; set; }
        public bool SendDocumentsATinRealTime { get; set; }
        public bool SendDocumentsATinRealTimeWB { get; set; }
        public bool UseVirtualKeyBoard { get; set; }
        public bool UseBaseDialogWindowMask { get; set; }
        public int PosBaseButtonMaxCharsPerLabel { get; set; }
        public string DateTimeFormatStatusBar { get; set; }

        public string PathAssets { get; set; }
        public string PathImages { get; set; }
        public string PathThemes { get; set; }
        public string PathSounds { get; set; }
        public string PathResources { get; set; }
        public string PathReports { get; set; }
        public string PathTickets { get; set; }
        public string PathKeyboards { get; set; }
        public string PathTemp { get; set; }
        public string PathCache { get; set; }
        public string PathDocuments { get; set; }
        public string PathPlugins { get; set; }
        public string PathCertificates { get; set; }

        public decimal DecimalMoneyButtonL1Value { get; set; }
        public decimal DecimalMoneyButtonL2Value { get; set; }
        public decimal DecimalMoneyButtonL3Value { get; set; }
        public decimal DecimalMoneyButtonL4Value { get; set; }
        public decimal DecimalMoneyButtonL5Value { get; set; }
        public decimal DecimalMoneyButtonR1Value { get; set; }
        public decimal DecimalMoneyButtonR2Value { get; set; }
        public decimal DecimalMoneyButtonR3Value { get; set; }
        public decimal DecimalMoneyButtonR4Value { get; set; }
        public decimal DecimalMoneyButtonR5Value { get; set; }

        public string FileImageBackgroundWindowStartup { get; set; }
        public string FileImageBackgroundWindowPos { get; set; }
        public string FileImageBackgroundDialogDefault { get; set; }
        public string FileImageBackgroundDialogTables { get; set; }
        public string FileImageDialogBaseMessageTypeImage { get; set; }
        public string FileImageDialogBaseMessageTypeIcon { get; set; }
        public string FileImageBackOfficeLogo { get; set; }
        public string FontPosBaseButtonSize { get; set; }

        public int FontPosToolbarButton { get; set; }

        public string FontPosStatusBar { get; set; }
        public string FontPosStatusBarSmall { get; set; }
        public string FontStartupWindowVersion { get; set; }
        public string FontBaseDialogButton { get; set; }
        public string FontBaseDialogActionAreaButton { get; set; }
        public string FontKeyboardPadTextEntry { get; set; }
        public string FontKeyboardPadPrimaryKey { get; set; }
        public string FontKeyboardPadSecondaryKey { get; set; }
        public string FontBackOfficeStatusBar { get; set; }
        public string FontNumberPadPinButtonKeysTextAndLabel { get; set; }
        public string FontMoneyPadButtonKeys { get; set; }
        public string FontMoneyPadTextEntry { get; set; }
        public string FontTicketPadPadButtonKeys { get; set; }
        public string FontTicketListColumnTitle { get; set; }
        public string FontTicketListColumn { get; set; }
        public string FontTicketListLabelLabelTotal { get; set; }
        public string FontTicketListLabelTotal { get; set; }
        public string FontPosBackOfficeParent { get; set; }
        public string FontPosBackOfficeChild { get; set; }
        public string FontPosBackOfficeParent_1024 { get; set; }
        public string FontPosBackOfficeChild_1024 { get; set; }
        public string FontTableDialogTableNumber { get; set; }
        public string FontEntryBoxLabel { get; set; }
        public string FontEntryBoxValue { get; set; }
        public string FontGenericTreeViewColumnTitle { get; set; }
        public string FontGenericTreeViewColumn { get; set; }
        public string FontGenericTreeViewSelectRecordColumnTitle { get; set; }

        public int FontGenericTreeViewSelectRecordColumn { get; set; }

        public string FontGenericTreeViewFinanceDocumentArticleColumnTitle { get; set; }
        public string FontGenericTreeViewFinanceDocumentArticleColumn { get; set; }
        public string FontPagePadNavigatorButton { get; set; }
        public string FontSplitPaymentTouchButtonSplitPayment { get; set; }

        public bool RequireToChooseVatExemptionReason { get; set; }

        public int IntStartupWindowObjectsNumberPadPinRight { get; set; }
        public int IntPosMainWindowComponentsMargin { get; set; }
        public int IntPosMainWindowEventBoxStatusBar1And2Height { get; set; }
        public int IntSplitPaymentTouchButtonSplitPaymentHeight { get; set; }

        public Color ColorPosHelperBoxsBackground { get; set; } 
        public Color ColorPosStatusBar1Background { get; set; }
        public Color ColorPosStatusBar2Background { get; set; }
        public Color ColorPosStatusBarFont { get; set; }
        public Color ColorPosStatusBarFontSmall { get; set; }
        public Color ColorTicketPadButtonFont { get; set; }
        public Color ColorFullScreenBackground { get; set; }
        public Color ColorFullScreenUsefullAreaBackground { get; set; }
        public Color ColorBaseDialogTitleBackground { get; set; }
        public Color ColorBaseDialogWindowBackground { get; set; }
        public Color ColorBaseDialogWindowBackgroundBorder { get; set; }
        public Color ColorBaseDialogActionAreaButtonFont { get; set; }
        public Color ColorBaseDialogActionAreaButtonBackground { get; set; }
        public Color ColorBaseDialogEntryBoxBackground { get; set; }
        public Color ColorBaseDialogDefaultButtonFont { get; set; }
        public Color ColorBaseDialogDefaultButtonBackground { get; set; }
        public Color ColorBaseDialogSecondaryButtonBackground { get; set; }
        public Color ColorBaseDialogSecondaryButtonFont { get; set; }
        public Color ColorBaseDialogEmptyButtonBackground { get; set; }
        public Color ColorPosPaymentsDialogTotalPannelBackground { get; set; }
        public Color ColorPosToolbarDefaultButtonFont { get; set; }
        public Color ColorPosTablePadTableTableStatusOpenButtonBackground { get; set; }
        public Color ColorPosTablePadTableTableStatusReservedButtonBackground { get; set; }
        public Color ColorPosTicketListModeTicketBackground { get; set; }
        public Color ColorPosTicketListModeOrderMainBackground { get; set; }
        public Color ColorPosTicketListModeEditBackground { get; set; }
        public Color ColorPosNumberPadLeftButtonBackground { get; set; }
        public Color ColorPosNumberRightButtonBackground { get; set; }
        public Color ColorEntryValidationValidFont { get; set; }
        public Color ColorEntryValidationInvalidFont { get; set; }
        public Color ColorEntryValidationInvalidFontLighter { get; set; }
        public Color ColorEntryValidationValidBackground { get; set; }
        public Color ColorEntryValidationInvalidBackground { get; set; }
        public Color ColorKeyboardPadKeyDefaultFont { get; set; }
        public Color ColorKeyboardPadKeySecondaryFont { get; set; }
        public Color ColorKeyboardPadKeyBackground { get; set; }
        public Color ColorKeyboardPadKeyBackgroundActive { get; set; }
        public Color ColorBackOfficeContentBackground { get; set; }
        public Color ColorBackOfficeAccordionFixBackground { get; set; }
        public Color ColorBackOfficeStatusBarBackground { get; set; }
        public Color ColorBackOfficeStatusBarBottomBackground { get; set; }
        public Color ColorBackOfficeStatusBarFont { get; set; }
        public Color ColorSplitPaymentTouchButtonFilledDataBackground { get; set; }
        public Color ColorPagePadHotButtonBackground { get; set; }

        public Point PositionButtonFavorites { get; set; }
        public Point PositionTablePadFamily { get; set; }
        public Point PositionTablePadSubFamily { get; set; }
        public Point PositionTablePadArticle { get; set; }

        public string TableConfigTablePadFamily { get; set; }
        public string TableConfigTablePadSubFamily { get; set; }
        public string TableConfigTablePadArticle { get; set; }
        public string TableConfigTablePadLoginUser { get; set; }

        public Size SizePosBaseButton { get; set; }
        public Size SizePosToolbarButton { get; set; }
        public Size SizePosTicketPadButton { get; set; }
        public Size SizePosTicketPadButtonDoubleWidth { get; set; }
        public Size SizePosToolbarButtonIcon { get; set; }
        public Size SizePosTicketPadButtonIcon { get; set; }
        public Size SizePosSmallButtonScroller { get; set; }
        public Size SizePosTableButton { get; set; }
        public Size SizePosUserButton { get; set; }
        public Size SizeBaseDialogDefaultButton { get; set; }
        public Size SizeBaseDialogDefaultButtonIcon { get; set; }
        public Size SizeBaseDialogActionAreaButton { get; set; }
        public Size SizeBaseDialogActionAreaButtonIcon { get; set; }
        public Size SizeBaseDialogActionAreaBackOfficeNavigatorButton { get; set; }
        public Size SizeBaseDialogActionAreaBackOfficeNavigatorButtonIcon { get; set; }
        public Size SizeKeyboardPadDefaultKey { get; set; }
        public Size SizePagesPadNavigatorButton { get; set; }
        public Size SizePagesPadNavigatorButtonIcon { get; set; }

        public bool UsePDFviewer { get; set; }
        public bool PrintTicket { get; set; }

        public Size SizeStartupWindowObjectsTablePadUserMarginLeftTop { get; set; }
        public Size SizeStartupWindowObjectsTablePadUserButton { get; set; }
        public Size SizeStartupWindowObjectsTablePadUserTablePadUserButtonPrev { get; set; }
        public Size SizeStartupWindowObjectsNumberPadPin { get; set; }
        public Size SizeStartupWindowObjectsNumberPadPinButton { get; set; }
        public Size SizeStartupWindowObjectsLabelVersion { get; set; }
        public Size SizeStartupWindowObjectsLabelVersionSizeMarginRightBottom { get; set; }

        public bool PosPaymentsDialogUseCurrentAccount { get; set; }
        public string ClientSettingsProviderServiceUri { get; set; }
        public string AppHardwareId { get; set; }
        public string FontPosBackOfficeParentLowRes { get; set; }
        public string FontPosBackOfficeChildLowRes { get; set; }
        #endregion
    }
}

