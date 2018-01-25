using Gtk;
using System;
using logicpos.Classes.Gui.Gtk.Widgets;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPage6 : PagePadPage
    {
        //Constructor
        public DocumentFinanceDialogPage6(Window pSourceWindow, String pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage6(Window pSourceWindow, String pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage6(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
        }

        //Override Base Validate
        public override void Validate()
        {
            //_log.Debug(string.Format("Validate: {0}", this.Name));
        }
    }
}
