using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Settings;
using System.Diagnostics;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class StockManagementModal : Modal
    {
        private static Size _modalSize;
        public StockManagementModal(Window parent, Size modalSize) : base(parent,
                                                          LocalizedString.Instance["global_stock_movements"],
                                                          modalSize,
                                                          windowMode: true)
        {
        }

        public static void RunModal()
        {
            if (AppSettings.Instance.AppScreenSize.Width <= 1024 && AppSettings.Instance.AppScreenSize.Height <= 800)
            {
                _modalSize=new Size(1000, 700);
            }
            else
            {
                _modalSize=new Size(1200, 700);
            }
            if (AppSettings.License.LicenseData.ModuleStocks)
            {
                var parentWindow = BackOfficeWindow.Instance;
                var stockManagementModal = new StockManagementModal(parentWindow,_modalSize);
                stockManagementModal.Run();
                stockManagementModal.Destroy();
            }
            else
            {
                var messageDialog = new CustomAlert(BackOfficeWindow.Instance)
                    .WithMessageType(MessageType.Warning)
                    .WithButtonsType(ButtonsType.OkCancel)
                    .WithTitleResource("global_warning")
                    .WithMessageResource("global_warning_acquire_module_stocks")
                    .ShowAlert();

                if (messageDialog == ResponseType.Ok)
                {
                    Process.Start("https://logic-pos.com/");
                }
            }
        }

    }
}
