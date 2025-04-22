using Gtk;
using logicpos;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Api.Features.Company.GetAngolaSaft;
using LogicPOS.Api.Features.Database;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pickers;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace LogicPOS.UI.Components.Windows
{
    public partial class BackOfficeWindow
    {
        private void Window_Show(object sender, EventArgs e)
        {
            UpdateUI();
        }

        #region Documents
        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            CreateDocumentModal.ShowModal(this);
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(this);
            modal.Run();
            modal.Destroy();
        }

        private void BtnReceipts_Clicked(object sender, EventArgs e)
        {
            var modal = new ReceiptsModal(this);
            modal.Run();
            modal.Destroy();
        }

        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            CustomerCurrentAccountFilterModal.ShowModal(this);
        }

        #endregion

        #region Reports
        private void BtnReports_Clicked(object sender, EventArgs e)
        {
            ReportsModal.ShowModal(this);
        }
        #endregion

        #region Articles 
        private void BtnStock_Clicked(object sender, EventArgs e)
        {
            var messageDialog = new CustomAlert(this)
                                        .WithMessageType(MessageType.Warning)
                                        .WithButtonsType(ButtonsType.OkCancel)
                                        .WithTitleResource("global_warning")
                                        .WithMessageResource("global_warning_acquire_module_stocks")
                                        .ShowAlert();


            //if (messageDialog == ResponseType.Ok)
            //{
            //    Process.Start("https://logic-pos.com/");
            //}


            var modal = new StockManagementModal(this);
            modal.Run();
            modal.Destroy();
        }
        #endregion

        #region System
        private void BtnLogout_Clicked(object sender, EventArgs e)
        {
            Hide();
            LoginWindow.Instance.ShowAll();
        }

        private void BtnPOS_Clicked(object sender, EventArgs e)
        {
            Hide();
            POSWindow.Instance.ShowAll();
        }

        private void BtnChangeLog_Clicked(object sender, EventArgs e)
        {
            Utils.ShowChangeLog(this);
        }

        private void BtnBackupDb_Clicked(object sender, EventArgs e)
        {
            var createBackupResponse = CustomAlerts.Question(this)
                                                   .WithTitle("Backup")
                                                   .WithMessage("Tem a certeza que pretende fazer o backup da base de dados?")
                                                   .ShowAlert();

            if (createBackupResponse != ResponseType.Yes)
            {
                return;
            }

            var backupResult = DependencyInjection.Mediator.Send(new BackupDatabaseCommand()).Result;

            if (backupResult.IsError)
            {
                ErrorHandlingService.HandleApiError(backupResult, source: this);
                return;
            }

            CustomAlerts.Information(this)
                         .WithMessage("Backup criado com sucesso.")
                         .ShowAlert();
        }

        private void BtnRestoreDb_Clicked(object sender, EventArgs e)
        {
            var restoreDatabaseResponse = CustomAlerts.Question(this)
                                                      .WithTitle("Restauro")
                                                      .WithMessage("Tem a certeza que pretende restaurar a base de dados?\n")
                                                      .ShowAlert();

            if (restoreDatabaseResponse != ResponseType.Yes)
            {
                return;
            }

            var restoreResult = DependencyInjection.Mediator.Send(new RestoreDatabaseCommand()).Result;

            if (restoreResult.IsError)
            {
                ErrorHandlingService.HandleApiError(restoreResult, source: this);
                return;
            }

            CustomAlerts.Information(this)
                         .WithMessage("Por favor reinicie completamente o sistema (API, Aplicação)!")
                         .ShowAlert();
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

            var getSaft = DependencyInjection.Mediator.Send(new GetAngolaSaftQuery(startDate, endDate)).Result;

            if (getSaft.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, getSaft.FirstError);
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


        private void BtnExit_Clicked(object sender, EventArgs args)
        {
            LogicPOSAppUtils.Quit(this);
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
                var responseType = new CustomAlert(this)
                                    .WithMessageResource("global_pos_update")
                                    .WithSize(new Size(600, 400))
                                    .WithMessageType(MessageType.Question)
                                    .WithButtonsType(ButtonsType.YesNo)
                                    .WithTitle(string.Format(GeneralUtils.GetResourceByName("window_title_dialog_update_POS"), GeneralSettings.ServerVersion))
                                    .ShowAlert();

                if (responseType == ResponseType.Yes)
                {
                    System.Diagnostics.Process.Start(lPathToUpdater);
                    LogicPOSAppUtils.QuitWithoutConfirmation();
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
                PageContainer.Remove(CurrentPage);
            }

            CurrentPage = button.Page;

            LabelActivePage.Text = button.Label;

            CurrentPage.Visible = true;

            PageContainer.PackStart(CurrentPage);
        }
    }
}
