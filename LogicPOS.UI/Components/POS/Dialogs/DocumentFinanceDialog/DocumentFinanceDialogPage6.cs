using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPage6 : PagePadPage
    {
        //Constructor
        public DocumentFinanceDialogPage6(Window parentWindow, string pPageName) : this(parentWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage6(Window parentWindow, string pPageName, Widget pWidget) : this(parentWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage6(Window parentWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(parentWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
        }

        //Override Base Validate
        public override void Validate()
        {
            //_logger.Debug(string.Format("Validate: {0}", this.Name));
        }
    }
}
