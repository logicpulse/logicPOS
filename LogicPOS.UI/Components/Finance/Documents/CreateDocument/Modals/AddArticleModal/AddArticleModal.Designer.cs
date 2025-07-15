using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.Documents.CreateDocument.Modals.SearchModelObject;
using LogicPOS.UI.Components.InputFields;


namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel),
                new ActionAreaButton(BtnClear, ResponseType.DeleteEvent)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var vbox = new VBox(false, 2);
            vbox.PackStart(TextBox.CreateHbox(TxtCode, TxtArticle), false, false, 0);
            vbox.PackStart(TextBox.CreateHbox(TxtPrice,
                                                  TxtQuantity,
                                                  TxtDiscount,
                                                  TxtTotal,
                                                  TxtTotalWithTax), false, false, 0);

            vbox.PackStart(TextBox.CreateHbox(TxtTax, TxtVatExemptionReason), false, false, 0);

            vbox.PackStart(TxtNotes.Component, false, false, 0);

            return vbox;
        }
    }
}
