using Gtk;
using logicpos.Classes.Enums.Hardware;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Windows
{
    public partial class POSWindow
    {
        private Guid TableId = Guid.Empty;

        private void PosMainWindow_WindowStateEvent(object o, WindowStateEventArgs args)
        {
            if (args.Event.NewWindowState == Gdk.WindowState.Fullscreen)
            {
                MenuFamilies.SelectedFamily = null;
                MenuFamilies.Refresh();
                MenuSubfamilies.Refresh();
                MenuArticles.Refresh();
            }
        }

        private void PosMainWindow_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
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
            BackOfficeWindow.ShowBackOffice(this);
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
            LoginWindow.Instance.LogOutUser(true);
        }

        private void BtnCashDrawer_Clicked(object sender, EventArgs e)
        {
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
            ChangeUserModal dialogChangeUser = new ChangeUserModal(this, DialogFlags.DestroyWithParent);


            int responseChangeUser = dialogChangeUser.Run();
            if (responseChangeUser != (int)ResponseType.Ok)
            {
                dialogChangeUser.Destroy();
                return;
            }

            UserPinModal pinModal = new UserPinModal(dialogChangeUser, DialogFlags.DestroyWithParent, dialogChangeUser.User);
            int responsePinPad = pinModal.Run();
            if (responsePinPad != (int)ResponseType.Ok)
            {
                pinModal.Destroy();
                dialogChangeUser.Destroy();
                return;
            }

            LabelTerminalInfo.Text = $"{TerminalService.Terminal.Designation} : {AuthenticationService.User.Name}";

            //Utils.ShowNotifications(pinModal); -> tchial0

            pinModal.Destroy();
            dialogChangeUser.Destroy();
        }

        private void ButtonFavorites_Clicked(object sender, EventArgs e)
        {
            if (MenuFamilies.SelectedButton != null && !MenuFamilies.SelectedButton.Sensitive) MenuFamilies.SelectedButton.Sensitive = true;
            if (MenuSubfamilies.SelectedButton != null && !MenuSubfamilies.SelectedButton.Sensitive) MenuSubfamilies.SelectedButton.Sensitive = true;
        }

        private void ImageLogo_Clicked(object o, ButtonPressEventArgs args)
        {
            UserPinModal dialogPinPad = new UserPinModal(this,
                                                               DialogFlags.Modal,
                                                               null, //tchial0
                                                               true);
            int responsePinPad = dialogPinPad.Run();
            if (responsePinPad == (int)ResponseType.Ok)
            {
                var resultOpenDoor = LogicPOS.Printing.Utility.PrintingUtils.OpenDoor();
                if (!resultOpenDoor)
                {
                    CustomAlerts.Information(this)
                                .WithSize(new Size(500, 340))
                                .WithTitleResource("global_information")
                                .WithMessage(string.Format(GeneralUtils.GetResourceByName("open_cash_draw_permissions")))
                                .ShowAlert();
                }
                else
                {
                    XPOUtility.Audit("CASHDRAWER_OUT", string.Format(
                         GeneralUtils.GetResourceByName("audit_message_cashdrawer_out"),
                         TerminalService.Terminal.Designation,
                         "Button Open Door"));
                }

            };

            dialogPinPad.Destroy();

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
