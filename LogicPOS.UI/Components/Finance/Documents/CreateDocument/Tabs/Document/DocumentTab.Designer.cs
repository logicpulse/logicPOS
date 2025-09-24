using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentTab
    {
        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtDocumentType.Component, false, false, 0);
            verticalLayout.PackStart(TxtPaymentCondition.Component, false, false, 0);
            if (SinglePaymentMethod)
            {
                verticalLayout.PackStart(TxtPaymentMethod.Component, false, false, 0);
            }
            verticalLayout.PackStart(TextBox.CreateHbox(TxtCurrency, TxtExchangeRate), false, false, 0);
            verticalLayout.PackStart(TextBox.CreateHbox(TxtOriginDocument, TxtCopyDocument), false, false, 0);
            verticalLayout.PackStart(TxtNotes.Component, false, false, 0);

            PackStart(verticalLayout);
        }
    }
}
