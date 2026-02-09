using Gtk;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Licensing;


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

            var boxArticle = new HBox(false, 2);
            if (LicensingService.Data.StocksModule)
            {
                boxArticle.PackStart(TxtSerialNumber.Component, false, true, 0);
            }
            boxArticle.PackStart(TxtCode.Component, false, true, 0);
            boxArticle.PackStart(TxtArticle.Component, false, true, 0);
            boxArticle.PackStart(TextBox.CreateHbox(TxtFamily, TxtSubFamily), true, true, 0);

            vbox.PackStart(boxArticle, false, false, 0);
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
