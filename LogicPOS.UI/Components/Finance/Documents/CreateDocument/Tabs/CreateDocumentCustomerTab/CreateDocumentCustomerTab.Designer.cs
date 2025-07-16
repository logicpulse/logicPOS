using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Finance.Customers;
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
    public partial class CreateDocumentCustomerTab
    {
        private void Design()
        {
            var verticalLayout = new VBox(false, 2);
            verticalLayout.PackStart(TxtCustomer.Component, false, false, 0);

            verticalLayout.PackStart(TextBox.CreateHbox(TxtFiscalNumber,
                                                            TxtCardNumber,
                                                            TxtDiscount), false, false, 0);

            verticalLayout.PackStart(TextBox.CreateHbox(TxtAddress,
                                                            TxtLocality), false, false, 0);


            verticalLayout.PackStart(TextBox.CreateHbox(TxtZipCode,
                                                            TxtCity,
                                                            TxtCountry), false, false, 0);


            verticalLayout.PackStart(TextBox.CreateHbox(TxtPhone,
                                                            TxtEmail), false, false, 0);



            verticalLayout.PackStart(TxtNotes.Component, false, false, 0);

            PackStart(verticalLayout);
        }
    }
}
