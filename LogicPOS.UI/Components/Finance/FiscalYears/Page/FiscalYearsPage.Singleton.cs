using LogicPOS.UI.Components.Windows;

namespace LogicPOS.UI.Components.Pages
{
    public partial class FiscalYearsPage
    {
        private static FiscalYearsPage _instance;

        public static FiscalYearsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FiscalYearsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }

    }
}
