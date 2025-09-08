using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using Medsphere.Widgets;
using System.Drawing;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {
        private VBox VBox1 { get; } = new VBox(false, 2);
        private Fixed FixedContainer { get; } = new Fixed();
        private HBox FrameContainer { get; } = new HBox();
        private Frame Frame { get; } = new Frame();
        private Label LabelTotals { get; } = new Label();
        private IconButtonWithText BtnTerminals { get; set; }
        private IconButtonWithText BtnPreferenceParameters { get; set; }
        private IconButtonWithText BtnFiscalYears { get; set; }
        private IconButtonWithText BtnPrinters { get; set; }
        private IconButtonWithText BtnArticles { get; set; }
        private IconButtonWithText BtnCustomers { get; set; }
        private IconButtonWithText BtnUsers { get; set; }
        private IconButtonWithText BtnTables { get; set; }
        private IconButtonWithText BtnDocuments { get; set; }
        private IconButtonWithText BtnNewDocument { get; set; }
        private IconButtonWithText BtnPayments { get; set; }
        private IconButtonWithText BtnArticleStock { get; set; }
        private IconButtonWithText BtnReportsMenu { get; set; }
        private IconButtonWithText BtnPrintReportRouter { get; set; }
        private IconButtonWithText BtnCustomerBalanceDetails { get; set; }
        private IconButtonWithText BtnSalesPerDate { get; set; }
        private ComboBox ComboSalesYear { get; set; } 
        private  Graph Graph { get; } = new Graph2D();
        private HBox GraphComponent { get; set; } = new HBox(false, 0);

        #region Colors
        //ScreenArea
        protected EventBox EventBox { get; set; }
        protected Color _colorBaseDialogDefaultButtonFont = ("76, 72, 70").StringToColor();
        protected Color _colorBaseDialogDefaultButtonBackground = ("156, 191, 42").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonFont = ("0, 0, 0").StringToColor();
        protected Color _colorBaseDialogActionAreaButtonBackground = AppSettings.Instance.ColorBaseDialogActionAreaButtonBackground;
        //protected String _fontBaseDialogButton = SharedUtils.OSSlash(LogicPOS.Settings.AppSettings.Instance.FontBaseDialogButton"]);
        protected string _fontBaseDialogActionAreaButton = AppSettings.Instance.FontBaseDialogActionAreaButton;
        protected string _fileActionDefault = AppSettings.Paths.Images + @"Icons\icon_pos_default.png";
        protected string _fileActionOK = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_ok.png";
        protected string _fileActionCancel = AppSettings.Paths.Images + @"Icons\Dialogs\icon_pos_dialog_action_cancel.png";

        //Colors
        private readonly Color colorBackOfficeContentBackground = AppSettings.Instance.ColorBackOfficeContentBackground;
        private readonly Color colorBackOfficeStatusBarBackground = AppSettings.Instance.ColorBackOfficeStatusBarBackground;
        private readonly Color colorBackOfficeAccordionFixBackground = AppSettings.Instance.ColorBackOfficeAccordionFixBackground;
        private readonly Color colorBackOfficeStatusBarFont = AppSettings.Instance.ColorBackOfficeStatusBarFont;
        private readonly Color colorBackOfficeStatusBarBottomBackground = AppSettings.Instance.ColorBackOfficeStatusBarBottomBackground;
        public Color slateBlue = Color.FromName("White");
        //private Frame frame;
        #endregion

    }
}
