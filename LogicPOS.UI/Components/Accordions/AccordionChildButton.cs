using Gtk;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;

namespace LogicPOS.UI.Components.Accordions
{
    public class AccordionChildButton : Button
    {
        public Widget Page { get; set; }
        public string ExternalApplication { get; set; }

        private void ResizeButton()
        {
            if (BackOfficeWindow.ScreenSize.Height <= 800)
            {
                HeightRequest = 23;
            }
            else
            {
                HeightRequest = 25;
            }
        }

        public AccordionChildButton(Widget header)
            : base(header)
        {
            ResizeButton();
            ExposeEvent += delegate { SetAlignment(0.0F, 0.5F); };
        }
    }
}
