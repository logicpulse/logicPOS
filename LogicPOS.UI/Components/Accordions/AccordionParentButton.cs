using Gtk;

namespace LogicPOS.UI.Components.Accordions
{
    public class AccordionParentButton : Button
    {
        public bool Active { get; set; }
        public VBox PanelButtons { get; set; } = new VBox();

        public AccordionParentButton(string label)
            : base(label)
        {
            ResizeButton();

            AddEventHandlers();
        }

        private void ResizeButton()
        {
            if (GlobalApp.BackOfficeScreenSize.Height <= 800)
            {
                HeightRequest = 25;
            }
            else
            {
                HeightRequest = 35;
            }
        }

        private void AddEventHandlers()
        {
            ExposeEvent += delegate { SetAlignment(0.00F, 0.5F); };

            PanelButtons.ExposeEvent += delegate
            {
                if (Active == false)
                {
                    PanelButtons.Visible = false;
                }
            };
        }

        public AccordionParentButton(Widget header)
            : base(header)
        {

            ResizeButton();

            PanelButtons.ExposeEvent += delegate { if (!Active) PanelButtons.Visible = false; };
        }
    }
}
