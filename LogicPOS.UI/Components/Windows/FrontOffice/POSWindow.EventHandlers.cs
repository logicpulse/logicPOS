using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.POS.Devices.Hardware;
using LogicPOS.UI.Components.Users;
using System;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSWindow
    {
        private void AddEventHandlers()
        {
            WindowStateEvent += Window_StateEvent;
            this.KeyReleaseEvent += Window_KeyReleaseEvent;
            this.Shown += POSWindow_Shown;

            BtnQuit.Clicked += BtnQuit_Clicked;
            BtnBackOffice.Clicked += BtnBackOffice_Clicked;
            BtnReports.Clicked += BtnReports_Clicked;
            BtnShowSystemDialog.Clicked += delegate { throw new NotImplementedException(); };
            BtnLogOut.Clicked += BtnLogOut_Clicked;
            BtnChangeUser.Clicked += BtnChangeUser_Clicked;
            BtnSessionOpening.Clicked += BtnCashDrawer_Clicked;
            BtnNewDocument.Clicked += BtnNewDocument_Clicked;
            BtnDocuments.Clicked += BtnDocuments_Clicked;
        }

        private void POSWindow_Shown(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void Window_StateEvent(object o, WindowStateEventArgs args)
        {

        }

        private void ImageLogo_Clicked(object o, ButtonPressEventArgs args)
        {
            if (args.Event.Type == Gdk.EventType.TwoButtonPress)
            {
                UserPinModal pinModal = new UserPinModal(this, AuthenticationService.User);
                var pinModalResponse = (ResponseType)pinModal.Run();

                if (pinModalResponse == ResponseType.Ok)
                {
                    AuthenticationService.HardwareOpenDrawer();
                }

                pinModal.Destroy();
            }
        }

        private void Window_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (LogicPOSApp.BarCodeReader != null)
            {
                LogicPOSApp.BarCodeReader.KeyReleaseEvent(this, o, args);
            }
        }

        private void BtnQuit_Clicked(object sender, EventArgs e)
        {
            if (CustomAlerts.ShowQuitConfirmationAlert(this))
            {
                Gtk.Application.Quit();
            }
        }

        private void BtnBackOffice_Clicked(object sender, EventArgs e)
        {
            Hide();
            BackOfficeWindow.ShowBackOffice();
        }

        private void BtnReports_Clicked(object sender, EventArgs e)
        {
            ReportsModal.ShowModal(this);
        }

        private void BtnLogOut_Clicked(object sender, EventArgs e)
        {
            Hide();
            LoginWindow.Instance.ShowAll();
        }

        private void BtnCashDrawer_Clicked(object sender, EventArgs e)
        {
            if (FiscalYearService.HasFiscalYear() == false)
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
            changeUserModal.Run();
            changeUserModal.Destroy();
        }

        private void BtnFavorites_Clicked(object sender, EventArgs e)
        {
            if (MenuFamilies.SelectedButton != null &&
                MenuFamilies.SelectedButton.Sensitive == false && !MenuArticles.PresentFavorites)
            {
                MenuSubfamilies.SelectedButton.Sensitive = true;
                MenuSubfamilies.SelectedButton = null;
            }

            MenuArticles.PresentFavorites = true;
           
            MenuArticles.Refresh();
        }

        private void HWBarCodeReader_Captured(object sender, EventArgs e)
        {
            switch (LogicPOSApp.BarCodeReader.Device)
            {
                case InputReaderType.None:
                    break;
                case InputReaderType.BarCodeReader:
                case InputReaderType.CardReader:
                    throw new NotImplementedException();
                default:
                    break;
            }
        }

        private void ScrollTextViewLog(object o, SizeAllocatedArgs args)
        {
            TextViewLog.ScrollToIter(TextViewLog.Buffer.EndIter, 0, false, 0, 0);
        }

        public void UpdatePrivileges()
        {
            BtnBackOffice.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_ACCESS");
            BtnSessionOpening.Sensitive = AuthenticationService.UserHasPermission("WORKSESSION_ALL");
            BtnReports.Sensitive = AuthenticationService.UserHasPermission("REPORT_ACCESS");
        }
    }
}
