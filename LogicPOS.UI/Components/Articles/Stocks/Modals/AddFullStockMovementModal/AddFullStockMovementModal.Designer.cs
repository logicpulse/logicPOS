using Gtk;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddFullStockMovementModal
    {
        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel)
            };
        }

        protected override Widget CreateBody()
        {
            InitializeComponents();

            var vbox = new VBox(false, 2);
            vbox.PackStart(TxtSupplier.Component, false, false, 0);
            vbox.PackStart(TxtDate.Component, false, false, 0);
            vbox.PackStart(TxtDocumnetNumber.Component, false, false, 0);
            vbox.PackStart(TxtNotes.Component, false, false, 0);
            vbox.PackStart(ArticlesContainer.Component, true, true, 0);
            return vbox;
        }
    }
}
