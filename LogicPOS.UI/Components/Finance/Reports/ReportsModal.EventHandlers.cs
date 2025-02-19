using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Services;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal
    {
        private void BtnCompanyBillingReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                ReportsService.ShowCompanyBillingReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnCustomerBalanceSummaryReport_Clicked(object sender, EventArgs e)
        {
            CustomerCurrentAccountFilterModal.ShowModal(this);
        }

        private void BtnSalesByDocumentReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDocumentTypeReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByDateReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDateReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByUserReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByUserReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByTerminalReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByTerminalReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByCustomerReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByCustomerReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByPaymentMethodReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByPaymentMethodReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByPaymentConditionReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByPaymentConditionReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByCurrencyReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByCurrencyReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByCountryReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByCountryReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByVatAndArticleTypeReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtVatRate.SelectedEntity != null)
                {
                    ReportsService.ShowSalesByVatAndArticleTypeReport(modal.StartDate, modal.EndDate, (modal.TxtVatRate.SelectedEntity as VatRate).Id);
                }
                else
                {
                    ReportsService.ShowSalesByVatAndArticleTypeReport(modal.StartDate, modal.EndDate);
                }

            }
            modal.Destroy();
        }

        private void BtnSalesByVatAndArticleClassReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtVatRate.SelectedEntity != null)
                {
                    ReportsService.ShowSalesByVatAndArticleClassReport(modal.StartDate, modal.EndDate, (modal.TxtVatRate.SelectedEntity as VatRate).Id);
                }
                else
                {
                    ReportsService.ShowSalesByVatAndArticleClassReport(modal.StartDate, modal.EndDate);
                }

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByCustomerReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowDetailedSalesByCustomerReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnCustomerBalanceDetailsReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtCustomer.SelectedEntity != null)
                {
                    ReportsService.ShowCustomerBalanceDetailsReport(modal.StartDate, modal.EndDate, (modal.TxtCustomer.SelectedEntity as Customer).Id);
                }
            }
            modal.Destroy();
        }


        private void BtnDetailedSalesByDocumentReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDocumentDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByDateReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDateDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByUserReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByUserDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByTerminalReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByTerminalDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByPaymentConditionReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByPaymentConditionDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByPaymentMethodReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByPaymentMethodDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByCurrencyReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByCurrencyDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByCountryReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByCountryDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByFamilyReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByFamilyDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesBySubfamilyReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesBySubfamilyDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByPlaceReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByPlaceDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByTableReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByTableDetailsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnDetailedSalesByVatGroupReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtVatRate.SelectedEntity != null)
                {
                    ReportsService.ShowSalesByVatGroupDetailsReport(modal.StartDate, modal.EndDate, (modal.TxtVatRate.SelectedEntity as VatRate).Id);
                }
                else
                {
                    ReportsService.ShowSalesByVatGroupDetailsReport(modal.StartDate, modal.EndDate);
                }
            }
            modal.Destroy();
        }

        private void BtnArticlesReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {

                ReportsService.ShowArticlesReport();

            }
            modal.Destroy();
        }

        private void BtnCustomersReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {

                ReportsService.ShowCustomersReport();

            }
            modal.Destroy();
        }

        private void BtnCommissionsReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {

                ReportsService.ShowCommissionsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnStockMovementsReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {

                ReportsService.ShowStockMovementsReport(modal.StartDate, modal.EndDate);

            }
            modal.Destroy();
        }

        private void BtnStockByWarehouseReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtArticle.SelectedEntity == null && modal.TxtWarehouse.SelectedEntity != null)
                {
                    ReportsService.ShowStockByWarehouseReport(modal.StartDate, modal.EndDate,
                                                    Guid.Empty,
                                                    (modal.TxtWarehouse.SelectedEntity as Warehouse).Id,
                                                    modal.TxtSerialNumber.Text);
                }
                else
                if (modal.TxtArticle.SelectedEntity != null && modal.TxtWarehouse.SelectedEntity == null)
                {
                    ReportsService.ShowStockByWarehouseReport(modal.StartDate, modal.EndDate,
                                                   (modal.TxtArticle.SelectedEntity as Article).Id,
                                                    Guid.Empty,
                                                    modal.TxtSerialNumber.Text);
                }
                else
                if (modal.TxtArticle.SelectedEntity == null && modal.TxtWarehouse.SelectedEntity == null)
                {
                    ReportsService.ShowStockByWarehouseReport(modal.StartDate, 
                                                    modal.EndDate,
                                                    Guid.Empty,
                                                    Guid.Empty,
                                                    modal.TxtSerialNumber.Text);
                }
                else
                {
                    
                        ReportsService.ShowStockByWarehouseReport(modal.StartDate, modal.EndDate,
                                                        (modal.TxtArticle?.SelectedEntity as Article).Id,
                                                        (modal.TxtWarehouse.SelectedEntity as Warehouse).Id,
                                                        modal.TxtSerialNumber.Text);
                }


            }
            modal.Destroy();
        }

        private void BtnStockByArticleReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtArticle.SelectedEntity==null)
                {
                    ReportsService.ShowStockByArticleReport(modal.StartDate, modal.EndDate);
                }
                else
                {
                    ReportsService.ShowStockByArticleReport(modal.StartDate, modal.EndDate, (modal.TxtArticle.SelectedEntity as Article).Id);
                }
            }
            modal.Destroy();
        }

        private void BtnStockBySupplierReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtCustomer.SelectedEntity == null)
                {
                    ReportsService.ShowStockBySupplierReport(modal.StartDate, modal.EndDate, Guid.Empty, modal.TxtDocumentNumber.Text);
                }
                else
                {
                    ReportsService.ShowStockBySupplierReport(modal.StartDate, modal.EndDate, (modal.TxtCustomer.SelectedEntity as Customer).Id, modal.TxtDocumentNumber.Text);
                }
            }
            modal.Destroy();
        }

        private void BtnStockByArticleGainReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtArticle.SelectedEntity == null && modal.TxtCustomer.SelectedEntity != null)
                {
                    ReportsService.ShowStockByArticleGainReport(modal.StartDate, modal.EndDate,
                                                              Guid.Empty,
                                                              (modal.TxtCustomer.SelectedEntity as Customer).Id);
                }
                else
               if (modal.TxtArticle.SelectedEntity != null && modal.TxtCustomer.SelectedEntity == null)
                {
                    ReportsService.ShowStockByArticleGainReport(modal.StartDate, modal.EndDate,
                                                                       (modal.TxtArticle.SelectedEntity as Article).Id,
                                                                        Guid.Empty);
                }
                else
               if (modal.TxtArticle.SelectedEntity == null && modal.TxtCustomer.SelectedEntity == null)
                {
                    ReportsService.ShowStockByArticleGainReport(modal.StartDate,
                                                                        modal.EndDate,
                                                                        Guid.Empty,
                                                                        Guid.Empty);
                }
                else
                {

                    ReportsService.ShowStockByArticleGainReport(modal.StartDate, modal.EndDate,
                                                                        (modal.TxtArticle.SelectedEntity as Article).Id,
                                                                        (modal.TxtCustomer.SelectedEntity as Customer).Id);
                }
            }
            modal.Destroy();
        }
    }
}
