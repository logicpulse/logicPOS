using Gtk;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Accordions;
using LogicPOS.UI.Components.Modals.Common;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public class ReportsModal : Modal
    {
        public ReportsModal(Window parent) : base(parent,
                                                  LocalizedString.Instance["global_reports"],
                                                  new Size(500, 509),
                                                  PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_reports.png")
        {
        }

        protected override ActionAreaButtons CreateActionAreaButtons() => null;

        protected override Widget CreateBody()
        {
            Accordion accordion = new Accordion(GetAccordionDefinition(), "{0}");

            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            viewport.Add(accordion);
            viewport.ResizeMode = ResizeMode.Parent;

            var scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWindow.Add(viewport);
            scrolledWindow.ResizeMode = ResizeMode.Parent;
            return scrolledWindow;
        }

        private Dictionary<string, AccordionNode> GetAccordionDefinition()
        {
            var accordionDefinition = new Dictionary<string, AccordionNode>();
            Dictionary<string, AccordionNode> children = new Dictionary<string, AccordionNode>();

            children.Add("ANY", new AccordionNode("Tchialo", true)
            {
                Clicked = delegate
                {
                    CustomAlerts.ShowContactSupportErrorAlert(this);
                }
            });

            accordionDefinition.Add($"Vision",
                      new AccordionNode("HAHAHAH")
                      {
                          Children = children,
                          GroupIcon = new Gtk.Image($"Assets/Images/Icons/Reports/report_auxiliary_tables.png")
                      });
            return accordionDefinition;
        }
    }
}
