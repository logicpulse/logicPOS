using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Application;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Settings;

namespace LogicPOS.UI.Components.Modals
{
    public class PartialPaymentModal : Modal
    {
        private IconButtonWithText BtnOk { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnCancel { get; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public PartialPaymentPage Page { get; private set; }

        public PartialPaymentModal(Window parent) : base(parent,
                                                         LocalizedString.Instance["window_title_dialog_partial_payment"],
                                                         AppSettings.MaxWindowSize,
                                                         $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_select_record.png"}")
        {

        }

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
            var page = new PartialPaymentPage(this, SaleContext.CurrentOrder);
            page.SetSizeRequest(WindowSettings.Size.Width - 14, WindowSettings.Size.Height - 124);
            Fixed fixedContent = new Fixed();
            fixedContent.Put(page, 0, 0);
            Page = page;
            return fixedContent;
        }
    }
}
