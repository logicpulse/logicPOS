using Gtk;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal partial class DocumentPreviewModal : Modal
    {
        private readonly GetDocumentPreviewDataQuery _query;
        public DocumentPreviewModal(Window parent, GetDocumentPreviewDataQuery query) : base(parent: parent,
                                                 title: GeneralUtils.GetResourceByName("window_title_dialog_documentfinance_preview_totals_mode_confirm"),
                                                 size: new Size(700, 360),
                                                 icon: AppSettings.Paths.Images + @"Icons\Windows\icon_window_preview.png")
        {
            _query = query;
            LoadData();
        }

        private void LoadData()
        {
            var data = DocumentsService.GetPreviewData(_query);

            if (data == null || data.Details.Count == 0)
            {
                return;
            }

            var taxResumes = data.GetTaxResumes();
            foreach (var taxResume in taxResumes)
            {
                listModel.AppendValues(taxResume.Designation, taxResume.Rate.ToString("F2"), taxResume.Base.ToString("F2"), taxResume.Total.ToString("F2"));
            }

            labelTotalGrossValue.Text = data.TotalFinal.ToString("F2");
            labelDiscountCustomerValue.Text = data.Discount.ToString("F2");
            labelDiscountTotalValue.Text = data.TotalDiscount.ToString("F2");
            labelTotalNetValue.Text = data.TotalNet.ToString("F2");
            LabelTaxTotalFinalValue.Text = data.TotalTax.ToString("F2");
            labelTotalFinalValue.Text = data.TotalFinal.ToString("F2");
        }

    }
}
