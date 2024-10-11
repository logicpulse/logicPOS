using Gtk;

namespace LogicPOS.UI.Components.Accordions
{
    public class AccordionChildButton : Button
    {
        public Widget Page { get; set; }
        public string ExternalApplication { get; set; }

        private void ResizeButton()
        {
            if (GlobalApp.BackOfficeScreenSize.Height <= 800)
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
