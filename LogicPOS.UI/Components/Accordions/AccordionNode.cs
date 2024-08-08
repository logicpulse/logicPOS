using Gtk;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Accordions
{
    public class AccordionNode
    {
        public string Label { get; set; }
        public Dictionary<string, AccordionNode> Children { get; set; }
        public Widget Button { get; set; }
        public Widget Content { get; set; }
        public Image GroupIcon { get; set; }
        public string ExternalAppFileName { get; set; }
        public bool Sensitive;
        public EventHandler Clicked { get; set; }

        public AccordionNode(string label,
                             bool sensitive = true)
        {
            Label = label;
            Sensitive = sensitive;
        }
    }
}
