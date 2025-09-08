using Gtk;
using LogicPOS.UI.Application;
using LogicPOS.UI.Settings;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage : Box
    {
        private readonly Window _parentWindow;

        public DashBoardPage(Window parentWindow)
        {
            _parentWindow = parentWindow;
            int fontGenericTreeViewColumn = Convert.ToInt16(AppSettings.Instance.FontGenericTreeViewColumn);
            var predicate = (Predicate<dynamic>)((dynamic x) => x.ID == "PosBaseWindow");
            var themeWindow = LogicPOSApp.Theme.Theme.Frontoffice.Window.Find(predicate);

            Initialize();
            AddEventHandlers();
            Design(parentWindow, themeWindow);
            ShowAll();
            UpdatePrivileges();
            Instance = this;
        }

        private void Initialize()
        {
            InitializeButtons();
            InitializeComboBoxSalesYears();
        }

        public static DashBoardPage Instance { get; private set; }
    }
}
