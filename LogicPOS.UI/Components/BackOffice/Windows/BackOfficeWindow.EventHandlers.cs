using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Settings;
using LogicPOS.Utility;
using logicpos;
using System;
using System.IO;
using LogicPOS.UI.Buttons;
using logicpos.Classes.DataLayer;
using logicpos.Classes.Enums;
using LogicPOS.UI.Components.Pickers;
using FastReport;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using LogicPOS.Api.Features.Database.GetBackup;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Modals;
using LogicPOS.Api.Features.Company.GetAngolaSaft;
using DocumentFormat.OpenXml.Wordprocessing;
using LogicPOS.UI.Application;

namespace LogicPOS.UI.Components.BackOffice.Windows
{
    public partial class BackOfficeWindow
    {
        private void BtnExit_Clicked(object sender, EventArgs args)
        {
            LogicPOSApp.Quit(this);
        }

        private void BtnDashBoard_Clicked(object sender, EventArgs args)
        {
            BtnDashboard.Page = new DashBoardPage(this);
            MenuBtn_Clicked(BtnDashboard, null);
        }

        private void BtnNewVesion_Clicked(object sender, EventArgs args)
        {
            DateTime actualDate = DateTime.Now;

            string fileName = "\\LPUpdater\\LPUpdater.exe";
            string lPathToUpdater = string.Format(@"{0}\{1}", Environment.CurrentDirectory, fileName);

            if (File.Exists(lPathToUpdater))
            {
                ResponseType responseType = Utils.ShowMessageBox(this, DialogFlags.Modal, new System.Drawing.Size(600, 400), MessageType.Question, ButtonsType.YesNo, string.Format(GeneralUtils.GetResourceByName("window_title_dialog_update_POS"), GeneralSettings.ServerVersion), GeneralUtils.GetResourceByName("global_pos_update"));

                if (responseType == ResponseType.Yes)
                {
                    System.Diagnostics.Process.Start(lPathToUpdater);
                    LogicPOSApp.QuitWithoutConfirmation();
                }
            }
        }

        public void MenuBtn_Clicked(object sender, EventArgs e)
        {
            IconButtonWithText button = (IconButtonWithText)sender;

            if (button.Page == null)
            {
                return;
            }

            if (CurrentPage != null)
            {
                PanelContent.Remove(CurrentPage);
            }

            CurrentPage = button.Page;

            LabelActivePage.Text = button.Label;

            CurrentPage.Visible = true;

            PanelContent.PackStart(CurrentPage);
        }

        #region System
        private void BtnLogout_Clicked(object sender, EventArgs e)
        {
            Hide();
            LogicPOSAppContext.StartupWindow.LogOutUser(true);
        }

        private void BtnPOS_Clicked(object sender, EventArgs e)
        {
            Hide();
            LogicPOSAppContext.PosMainWindow.ShowAll();
        }

        private void BtnNotificaion_Clicked(object sender, EventArgs e)
        {
            Utils.ShowNotifications(this, true);
        }

        private void BtnChangeLog_Clicked(object sender, EventArgs e)
        {
            Utils.ShowChangeLog(this);
        }

        private void BtnBackupDb_Clicked(object sender, EventArgs e)
        {
            FilePicker picker = new FilePicker(this,
                                               DialogFlags.DestroyWithParent,
                                               FilePicker.GetFileFilterBackups(),
                                               FileChooserAction.Save,
                                               "Backup Database");

            picker.FileChooser.SelectMultiple = false;

            var result = (ResponseType)picker.Run();

            if (result != ResponseType.Ok || Directory.Exists(picker.FileChooser.Filename))
            {
                picker.Destroy();
                return;
            }

            string backupFileDestination = picker.FileChooser.Filename + ".bak";

            var getBackup = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetSqliteBackupQuery()).Result;

            if (getBackup.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, getBackup.FirstError);
                picker.Destroy();
                return;
            }

            File.Copy(getBackup.Value, backupFileDestination, true);
            picker.Destroy();
        }

        private void BtnRestoreDb_Clicked(object sender, EventArgs e)
        {
            DataBaseBackup.Restore(this, DataBaseRestoreFrom.SystemBackup);
        }

        private void BtnRestoreDbFromFile_Clicked(object sender, EventArgs e)
        {
            DataBaseBackup.Restore(this, DataBaseRestoreFrom.ChooseFromFilePickerDialog);
        }
        #endregion

        #region Export
        private void BtnExportCustomSaft_Clicked(object sender, EventArgs e)
        {
            PosDatePickerStartEndDateDialog dateRangeModal = new PosDatePickerStartEndDateDialog(this, DialogFlags.DestroyWithParent);
            ResponseType response = (ResponseType)dateRangeModal.Run();
            if (response != ResponseType.Ok)
            {
                dateRangeModal.Destroy();
                return;
            }

            var startDate = dateRangeModal.DateStart;
            var endDate = dateRangeModal.DateEnd;
            dateRangeModal.Destroy();

            ExportSaftByPeriod(startDate, endDate);
        }

        private void ExportSaftByPeriod(DateTime startDate, DateTime endDate)
        {
            var filePath = FilePicker.GetSaveFilePath(this, "Export Angola SAFT");

            if (filePath == null)
            {
                return;
            }

            var getSaft = DependencyInjection.Services.GetRequiredService<ISender>().Send(new GetAngolaSaftQuery(startDate, endDate)).Result;

            if (getSaft.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(this, getSaft.FirstError);
                return;
            }

            string saftFileDestination = filePath + ".xml";

            File.WriteAllBytes(saftFileDestination, getSaft.Value);
        }

        private void BtnExportYearlySaft_Clicked(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);

            ExportSaftByPeriod(startDate, endDate);
        }

        private void BtnExportLastMonthSaft_Clicked(object sender, EventArgs e)
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            ExportSaftByPeriod(startDate, endDate);
        }

        #endregion

    }
}
