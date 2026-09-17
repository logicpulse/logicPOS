using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public  partial class SeriesInfoModal : Modal
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
            body.PackStart(TextBox.CreateHbox(TxtYear, TxtDocumentType), false, false, 0);
            body.PackStart(TextBox.CreateHbox(TxtCode, TxtStatus), false, false, 0);
            body.PackStart(TxtCreationDate.Component, false, false, 0);
            body.PackStart(TxtJoiningDate.Component, false, false, 0);
            body.PackStart(TxtJoiningType.Component, false, false, 0);
            body.PackStart(TxtInvoicingMethod.Component, false, false, 0);
            body.PackStart(TxtNif.Component, false, false, 0);
            body.PackStart(TxtName.Component, false, false, 0);
            return body;
        }
    }
}
