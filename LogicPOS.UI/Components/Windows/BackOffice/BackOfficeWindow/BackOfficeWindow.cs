using logicpos;
using LogicPOS.UI.Application.Screen;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;

namespace LogicPOS.UI.Components.Windows
{
    public partial class BackOfficeWindow : BackOfficeBaseWindow
    {
        public BackOfficeWindow()
        {
            RegisterPanels();
            AddSections();
            AddEventHandlers();
            ShowStartPage();
            ShowAll();
            ShowPanel(PanelArticles);
        }

        public void UpdateUI()
        {
            LabelTerminalInfo.Text = $"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}";
            UpdatePrivileges();
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
            Shown += Window_Show;
        }

        #region Static
        private static BackOfficeWindow _instance;
        public static bool HasInstance => _instance != null;
        public static BackOfficeWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BackOfficeWindow();
                }

                return _instance;
            }
        }

        public static global::System.Drawing.Size ScreenSize { get; set; } = ScreenSizeUtil.GetScreenSize();

        public static void ShowBackOffice()
        {
            Instance.UpdateUI();
            Instance.Show();
        }

        #endregion
    }
}
