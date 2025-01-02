using Gtk;
using logicpos.Classes.Enums.Hardware;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using System;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSWindow
    {
        private Guid TableId = Guid.Empty;

        private void Window_StateEvent(object o, WindowStateEventArgs args)
        {
            if (args.Event.NewWindowState == Gdk.WindowState.Fullscreen)
            {
                MenuFamilies.SelectedFamily = null;
                MenuFamilies.Refresh();
                MenuSubfamilies.Refresh();
                MenuArticles.Refresh();
            }
        }

        private void Window_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (LogicPOSAppContext.BarCodeReader != null)
            {
                LogicPOSAppContext.BarCodeReader.KeyReleaseEvent(this, o, args);
            }
        }

        private void BtnQuit_Clicked(object sender, EventArgs e)
        {
            LogicPOSAppUtils.Quit(this);
        }

        private void BtnBackOffice_Clicked(object sender, EventArgs e)
        {
            Hide();
            BackOfficeWindow.ShowBackOffice();
        }

        private void BtnReports_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsModal(this);
            modal.Run();
            modal.Destroy();
        }

        private void BtnLogOut_Clicked(object sender, EventArgs e)
        {
            Hide();
            LoginWindow.Instance.ShowAll();
        }

        private void BtnCashDrawer_Clicked(object sender, EventArgs e)
        {
            if(FiscalYearService.HasFiscalYear() == false)
            {
                FiscalYearService.ShowOpenFiscalYearAlert();
                return;
            }

            SessionOpeningModal modal = new SessionOpeningModal(this);
            modal.Run();
            modal.Destroy();
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            CreateDocumentModal.ShowModal(this);
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var documentsMenu = new DocumentsMenuModal(this);
            documentsMenu.Run();
            documentsMenu.Destroy();
        }

        private void BtnChangeUser_Clicked(object sender, EventArgs e)
        {
            ChangeUserModal changeUserModal = new ChangeUserModal(this);

            var changeUserModalResponse = (ResponseType)changeUserModal.Run();

            if (changeUserModalResponse != ResponseType.Ok)
            {
                changeUserModal.Destroy();
                return;
            }

            UserPinModal pinModal = new UserPinModal(changeUserModal, changeUserModal.User);
            var pinModalResponse = (ResponseType)pinModal.Run();

            if (pinModalResponse != ResponseType.Ok)
            {
                pinModal.Destroy();
                changeUserModal.Destroy();
                return;
            }

            UpdateUI();
            BackOfficeWindow.Instance?.UpdateUI();

            pinModal.Destroy();
            changeUserModal.Destroy();
        }

        private void ButtonFavorites_Clicked(object sender, EventArgs e)
        {
            if (MenuFamilies.SelectedButton != null && !MenuFamilies.SelectedButton.Sensitive) MenuFamilies.SelectedButton.Sensitive = true;
            if (MenuSubfamilies.SelectedButton != null && !MenuSubfamilies.SelectedButton.Sensitive) MenuSubfamilies.SelectedButton.Sensitive = true;
        }

        private void HWBarCodeReader_Captured(object sender, EventArgs e)
        {
            switch (LogicPOSAppContext.BarCodeReader.Device)
            {
                case InputReaderDevice.None:
                    break;
                case InputReaderDevice.BarCodeReader:
                case InputReaderDevice.CardReader:
                    if (GeneralSettings.AppUseParkingTicketModule)
                    {
                        LogicPOSAppContext.ParkingTicket.GetTicketDetailFromWS(LogicPOSAppContext.BarCodeReader.Buffer);
                    }
                    else
                    {
                        //tchial0 -> TicketList.InsertOrUpdate(LogicPOSAppContext.BarCodeReader.Buffer);
                    }
                    break;

                default:
                    break;
            }
        }

        private void ScrollTextViewLog(object o, SizeAllocatedArgs args)
        {
            TextViewLog.ScrollToIter(TextViewLog.Buffer.EndIter, 0, false, 0, 0);
        }
    }
}
