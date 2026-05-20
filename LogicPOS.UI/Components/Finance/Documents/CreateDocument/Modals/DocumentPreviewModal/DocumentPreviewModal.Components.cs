using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System.Collections.Generic;
using DocumentDetail = LogicPOS.UI.Components.Documents.CreateDocument.DocumentDetail;
using LogicPOS.Globalization;

namespace LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.CreateDocumentModal.DocumentPreviewModal
{
    internal partial class DocumentPreviewModal : Modal
    {
        private Pango.FontDescription TitleFontStyle = Pango.FontDescription.FromString("Bold 11");
        private Pango.FontDescription ValueFontStyle = Pango.FontDescription.FromString("11");

        private IconButtonWithText BtnNo { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.No);
        private IconButtonWithText BtnYes { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Yes);

        private Label labelDesignation = new Label(LocalizedString.Instance["global_designation"]);
        private Label labelBaseValue = new Label(LocalizedString.Instance["global_total_tax_base"]);
        private Label labelValue = new Label(LocalizedString.Instance["global_vat_rate"]);
        private Label labelTaxTotal = new Label(LocalizedString.Instance["global_documentfinance_totaltax_acronym"]);
        private Label labelDiscountCustomer = new Label(LocalizedString.Instance["global_documentfinance_discount_customer"] + " (%)"); /* IN009206 */
        private Label labelTotalNet = new Label(LocalizedString.Instance["global_totalnet"]);
        private Label labelTotalGross = new Label(LocalizedString.Instance["global_documentfinance_totalgross"]);
        private Label labelDiscountTotal = new Label(LocalizedString.Instance["global_documentfinance_total_discount"]);
        private Label LabelTaxTotalFinal = new Label(LocalizedString.Instance["global_documentfinance_totaltax"]);
        private Label labelFinalTotal = new Label(LocalizedString.Instance["global_documentfinance_totalfinal"]);
        private ListStore listModel = new ListStore(typeof(string), typeof(string), typeof(string), typeof(string));
        private Label labelDiscountCustomerValue = new Label();
        private Label labelDiscountTotalValue = new Label();
        private Label labelTotalGrossValue = new Label();
        private Label labelTotalNetValue = new Label();
        private Label LabelTaxTotalFinalValue = new Label();
        private Label labelTotalFinalValue = new Label();

    }
}
