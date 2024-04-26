using Gtk;

namespace logicpos.Classes.Gui.Gtk.Widgets.BackOffice
{
    public class BOWidgetBox : VBox
    {
        public Label LabelComponent { get; set; }
        public Widget WidgetComponent { get; set; }

        public BOWidgetBox(string pLabel, Widget pWidget)
            : this(pLabel, pWidget, false, false)
        { 
        }

        public BOWidgetBox(string pLabel, Widget pWidget, bool pExpand, bool pFill)
            : base(false, 2)//Homogeneous False
        {
            LabelComponent = new Label(pLabel);
            LabelComponent.SetAlignment(0.0F, 0.0F);
            WidgetComponent = pWidget;

            PackStart(LabelComponent, false, false, 0);
            PackStart(pWidget, pExpand, pFill, 0);
        }
    }
}
