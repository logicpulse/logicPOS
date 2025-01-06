using LogicPOS.UI.Application;
using System;

namespace LogicPOS.UI.Components.Windows
{
    public partial class LoginWindow : POSBaseWindow
    {
        public LoginWindow(string backgroundImage)
            : base(backgroundImage)
        {
            AddEventHandlers();
            InitializeUI();
        }

        private void AddEventHandlers()
        {
            this.KeyReleaseEvent += Window_KeyReleaseEvent;
            this.Shown += LoginWindow_Shown;
        }

        private dynamic GetTheme()
        {
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "StartupWindow");
            var theme = LogicPOSAppContext.Theme.Theme.Frontoffice.Window.Find(predicate);
            return theme;
        }

        #region Static 
        private static LoginWindow _instance;

        public static LoginWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateLoginWindow();
                }
                return _instance;
            }
        }

        private static LoginWindow CreateLoginWindow()
        {
            var predicate = (Predicate<dynamic>)((x) => x.ID == "StartupWindow");
            var themeWindow = LogicPOSAppContext.Theme.Theme.Frontoffice.Window.Find(predicate);

            string windowImageFileName = string.Format(themeWindow.Globals.ImageFileName,
                                                       LogicPOSAppContext.ScreenSize.Width,
                                                       LogicPOSAppContext.ScreenSize.Height);

            return new LoginWindow(windowImageFileName);
        }
        #endregion
    }
}
