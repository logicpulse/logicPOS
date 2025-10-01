using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System.Collections.Generic;
using DocumentDetail = LogicPOS.UI.Components.Documents.CreateDocument.DocumentDetail;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal partial class DocumentPreviewModal : Modal
    {
        Pango.FontDescription TitleFontStyle = Pango.FontDescription.FromString("Bold 11");
        Pango.FontDescription ValueFontStyle = Pango.FontDescription.FromString("11");

        private IconButtonWithText BtnOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        Label labelDesignation = new Label(GeneralUtils.GetResourceByName("global_designation"));
        Label labelBaseValue = new Label(GeneralUtils.GetResourceByName("global_total_tax_base"));
        Label labelValue = new Label(GeneralUtils.GetResourceByName("global_vat_rate"));
        Label labelTaxTotal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totaltax_acronym"));
        Label labelDiscountCustomer = new Label(GeneralUtils.GetResourceByName("global_documentfinance_discount_customer") + " (%)"); /* IN009206 */
        Label labelTotalNet = new Label(GeneralUtils.GetResourceByName("global_totalnet"));
        Label labelTotalGross = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totalgross"));
        Label labelDiscountTotal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_total_discount"));
        Label LabelTaxTotalFinal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totaltax"));
        private Label labelFinalTotal = new Label(GeneralUtils.GetResourceByName("global_documentfinance_totalfinal"));
        private readonly List<DocumentDetail> _items= new List<DocumentDetail>();
        private readonly decimal _customerDiscount;
        ListStore listModel = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
        private Label labelDiscountCustomerValue = new Label();
        private Label labelDiscountTotalValue = new Label();
        private Label labelTotalGrossValue = new Label();
        private Label labelTotalNetValue = new Label();
        private Label LabelTaxTotalFinalValue = new Label();
        private Label labelTotalFinalValue = new Label();

    }
}
