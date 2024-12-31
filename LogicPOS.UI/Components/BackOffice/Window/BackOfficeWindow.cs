using Gtk;
using logicpos;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using System;


namespace LogicPOS.UI.Components.Windows
{
    public partial class BackOfficeWindow : BackOfficeBaseWindow
    {
        public static BackOfficeWindow Instance { get; set; }

        private readonly string _privilegesBackOfficeMenuOperation = $"{LogicPOSSettings.PrivilegesBackOfficeCRUDOperationPrefix}_{"MENU"}";

        public BackOfficeWindow()
        {
            RegisterPanels();
            AddSections();
            AddEventHandlers();
            ShowStartPage();
            ShowAll();
            ShowPanel(PanelArticles);
            UpdateUI();
        }

        private void UpdateUI()
        {
            var hasFiscalYear = FiscalYearService.HasFiscalYear();
            BtnNewDocument.Button.Sensitive = hasFiscalYear;
        }

        private void BackOfficeMainWindow_Show(object sender, EventArgs e)
        {
            LabelTerminalInfo.Text = $"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}";
        }

        private void ShowStartPage()
        {
            CurrentPage = new DashBoardPage(this);
            PageContainer.PackEnd(CurrentPage);
        }

        private void AddEventHandlers()
        {
            BtnExit.Clicked += BtnExit_Clicked;
            BtnNewVersion.Clicked += BtnNewVesion_Clicked;
            BtnDashboard.Clicked += BtnDashBoard_Clicked;
            BtnPOS.Clicked += BtnPOS_Clicked;
            Shown += BackOfficeMainWindow_Show;
        }

        #region Static
        public static System.Drawing.Size ScreenSize { get; set; } = Utils.GetScreenSize();
        public static void ShowBackOffice(Window windowToHide)
        {
            if (Instance == null)
            {
                Instance = new BackOfficeWindow();
            }
            else
            {
                Instance.Show();
            }

            windowToHide.Hide();
        }
        #endregion
    }
}
