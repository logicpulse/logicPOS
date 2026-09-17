using LogicPOS.UI.Application.Enums;
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

        /// <summary>
        /// Overrides <see cref="AppOperationModeToken"/> from the API module when a known mapping exists.
        /// Leaves the value from appsettings.json when module is missing or unknown.
        /// </summary>
        public void ApplyOperationModeFromApiModule(string apiModule)
        {
            var mappedToken = AppOperationModeExtensions.TryMapApiModuleToToken(apiModule);
            if (mappedToken == null)
            {
                return;
            }

            AppOperationModeToken = mappedToken;
        }

        #region Properties
        public Size AppScreenSize { get; set; }
        public int AppScreen { get; set; }
        public bool AppThemeCalcDynamicSize { get; set; }
        public string AppOperationModeToken { get; set; }
        public string CustomCultureResourceDefinition { get; set; }
        public int? InactivityTimeout { get; set; }
        public bool UseImageOverlay { get; set; }
        public bool AppShowMinimize { get; set; }
        public bool UseBaseDialogWindowMask { get; set; }
        public int PosBaseButtonMaxCharsPerLabel { get; set; }

        public string PathImages { get; set; }
        public string PathThemes { get; set; }
        public string PathKeyboards { get; set; }
        public string PathTemp { get; set; }

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

        public string FileImageDialogBaseMessageTypeImage { get; set; }
        public string FileImageDialogBaseMessageTypeIcon { get; set; }
        public string FileImageBackOfficeLogo { get; set; }
        public string FontPosBaseButtonSize { get; set; }
        public string FontPosStatusBar { get; set; }
        public string FontBaseDialogButton { get; set; }
        public string FontBaseDialogActionAreaButton { get; set; }
        public string FontKeyboardPadTextEntry { get; set; }
        public string FontKeyboardPadPrimaryKey { get; set; }
        public string FontKeyboardPadSecondaryKey { get; set; }
        public string FontNumberPadPinButtonKeysTextAndLabel { get; set; }
        public string FontMoneyPadButtonKeys { get; set; }
        public string FontMoneyPadTextEntry { get; set; }
        public string FontTicketListColumn { get; set; }
        public string FontPosBackOfficeParent { get; set; }
        public string FontPosBackOfficeChild { get; set; }
        public string FontPosBackOfficeParentLowRes { get; set; }
        public string FontPosBackOfficeChildLowRes { get; set; }
        public string FontEntryBoxLabel { get; set; }
        public string FontEntryBoxValue { get; set; }
        public string FontGenericTreeViewColumnTitle { get; set; }
        public string FontGenericTreeViewColumn { get; set; }
        public string FontPagePadNavigatorButton { get; set; }
        public string FontSplitPaymentTouchButtonSplitPayment { get; set; }

        public int IntStartupWindowObjectsNumberPadPinRight { get; set; }
        public int IntPosMainWindowComponentsMargin { get; set; }
        public int IntPosMainWindowEventBoxStatusBar1And2Height { get; set; }
        public int IntSplitPaymentTouchButtonSplitPaymentHeight { get; set; }

        public Color ColorBaseDialogTitleBackground { get; set; }
        public Color ColorBaseDialogWindowBackground { get; set; }
        public Color ColorBaseDialogWindowBackgroundBorder { get; set; }
        public Color ColorBaseDialogActionAreaButtonFont { get; set; }
        public Color ColorBaseDialogActionAreaButtonBackground { get; set; }
        public Color ColorBaseDialogEntryBoxBackground { get; set; }
        public Color ColorBaseDialogDefaultButtonFont { get; set; }
        public Color ColorBaseDialogDefaultButtonBackground { get; set; }
        public Color ColorPosPaymentsDialogTotalPannelBackground { get; set; }
        public Color ColorPosTablePadTableTableStatusOpenButtonBackground { get; set; }
        public Color ColorPosTablePadTableTableStatusReservedButtonBackground { get; set; }
        public Color ColorPosTicketListModeTicketBackground { get; set; }
        public Color ColorPosTicketListModeOrderMainBackground { get; set; }
        public Color ColorEntryValidationValidFont { get; set; }
        public Color ColorEntryValidationInvalidFont { get; set; }
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
        public Color ColorPagePadHotButtonBackground { get; set; }

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

        public Size SizeStartupWindowObjectsTablePadUserMarginLeftTop { get; set; }
        public Size SizeStartupWindowObjectsTablePadUserButton { get; set; }
        public Size SizeStartupWindowObjectsTablePadUserTablePadUserButtonPrev { get; set; }
        public Size SizeStartupWindowObjectsNumberPadPin { get; set; }
        public Size SizeStartupWindowObjectsNumberPadPinButton { get; set; }
        public Size SizeStartupWindowObjectsLabelVersion { get; set; }
        public Size SizeStartupWindowObjectsLabelVersionSizeMarginRightBottom { get; set; }
        #endregion
    }
}
