using Gtk;
using logicpos.Classes.Enums.Hardware;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
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
            if (args.Event.NewWindowState == Gdk.WindowState.Fullscreen)
            {
                MenuFamilies.SelectedEntity = null;
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
                MenuFamilies.SelectedButton.Sensitive == false)
            {
                MenuFamilies.SelectedButton.Sensitive = true;
            }

            if (MenuSubfamilies.SelectedButton != null &&
                MenuSubfamilies.SelectedButton.Sensitive == false)
            {
                MenuSubfamilies.SelectedButton.Sensitive = true;
            }

            MenuArticles.PresentFavorites = true;
            MenuArticles.Refresh();
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

        public void UpdatePrivileges()
        {
            BtnBackOffice.Sensitive = AuthenticationService.UserHasPermission("BACKOFFICE_ACCESS");
            BtnSessionOpening.Sensitive = AuthenticationService.UserHasPermission("WORKSESSION_ALL");
            BtnReports.Sensitive = AuthenticationService.UserHasPermission("REPORT_ACCESS");
        }
    }
}
