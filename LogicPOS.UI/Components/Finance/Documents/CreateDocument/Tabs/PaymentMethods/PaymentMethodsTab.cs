using Gtk;
using LogicPOS.UI.Components.Documents.CreateDocument.Fields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class PaymentMethodsTab : ModalTab
    {
        public DocumentPaymentMethodsBox PaymentMethodsBox { get; set; }
        public PaymentMethodsTab(Window parent) : base(parent: parent,
                                                                     name: GeneralUtils.GetResourceByName("global_payment_method"),
                                                                     icon: AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_1_new_document.png",
                                                                     false)
        {
            Initialize();
            Design();
        }


        private void Initialize()
        {
            PaymentMethodsBox = new DocumentPaymentMethodsBox(SourceWindow);
        }

        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(PaymentMethodsBox.Component);
            PackStart(verticalLayout);
        }


        public override bool IsValid()
        {
            return PaymentMethodsBox.Fields.All(x => x.IsValid());
        }
    }
}
