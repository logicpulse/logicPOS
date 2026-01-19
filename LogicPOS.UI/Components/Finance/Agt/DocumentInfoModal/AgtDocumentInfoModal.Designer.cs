using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public  partial class AgtDocumentInfoModal 
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnCorrectDocument, ResponseType.Accept),
                new ActionAreaButton(BtnClose, ResponseType.Close)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var body = new VBox(false, 2);
            body.PackStart(TextBox.CreateHbox(TxtRequestId, TxtSubmissionDate), false, false, 0);
            body.PackStart(TxtDocumentNumber.Component, false, false, 0);
            body.PackStart(TextBox.CreateHbox(TxtSubmissionErrorCode, TxtValidationStatus), false, false, 0);
            body.PackStart(TxtSubmissionErrorDescription.Component, false, false, 0);
            body.PackStart(TextBox.CreateHbox(TxtHttpStatusCode, TxtValidationResultCode), false, false, 0);
            body.PackStart(TxtRejectedDocumentNumber.Component, false, false, 0);
            body.PackStart(TxtValidationErrors.Component, false, false, 0);

            return body;
        }
    }
}
