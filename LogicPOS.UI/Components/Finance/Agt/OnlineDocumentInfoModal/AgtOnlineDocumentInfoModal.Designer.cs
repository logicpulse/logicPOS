using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public  partial class AgtOnlineDocumentInfoModal 
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnClose, ResponseType.Close)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var body = new VBox(false, 2);
            body.PackStart(TextBox.CreateHbox(TxtDocumentType, TxtDate), false, false, 0);
            body.PackStart(TxtDocumentNumber.Component, false, false, 0);
            body.PackStart(TxtStatus.Component, false, false, 0);
            body.PackStart(TxtCustomerNif.Component, false, false, 0);
            body.PackStart(TxtTaxPayable.Component, false, false, 0);
            body.PackStart(TxtNetTotal.Component, false, false, 0);
            body.PackStart(TxtGrossTotal.Component, false, false, 0);
            return body;
        }
    }
}
