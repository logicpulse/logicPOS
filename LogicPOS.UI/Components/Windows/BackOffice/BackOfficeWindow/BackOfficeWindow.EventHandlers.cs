using Gtk;
using logicpos;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using LogicPOS.Api.Features.Database;
using LogicPOS.Api.Features.Finance.Saft.GetSaft;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pickers;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
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
            var modal = new DocumentsModal(this, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Default);
            modal.Run();
            modal.Destroy();
        }

        private void BtnReceiptsEmission_Clicked(object sender, System.EventArgs e)
        {
            var modal = new DocumentsModal(this, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.UnpaidInvoices);
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
            StockManagementModal.ShowModal(this);
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
            string defaultSaftFileName = $"saft_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
            string destinationFilePath = FilePicker.GetSaveFilePath(this,
                                                                    "Export Angola SAFT",
                                                                    defaultSaftFileName);

            if (destinationFilePath == null)
            {
                return;
            }

            var getSaft = DependencyInjection.Mediator.Send(new GetSaftQuery(startDate, endDate)).Result;

            if (getSaft.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, getSaft.FirstError);
                return;
            }

            string saftFileDestination = destinationFilePath + ".xml";

            File.Copy(getSaft.Value.Path, saftFileDestination, true);
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
            if (CustomAlerts.ShowQuitConfirmationAlert(this))
            {
                Gtk.Application.Quit();
            }
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
                                    .WithTitle(string.Format(GeneralUtils.GetResourceByName("window_title_dialog_update_POS"), AppSettings.ServerVersion))
                                    .ShowAlert();

                if (responseType == ResponseType.Yes)
                {
                    System.Diagnostics.Process.Start(lPathToUpdater);
                    Gtk.Application.Quit();
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
