using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.ValueObjects
{
    public class Button 
    {
        public string ButtonLabel { get; set; }
        public bool? ButtonLabelHide { get; set; }
        public string ButtonImage { get; set; }
        public string ButtonIcon { get; set; }
    }
}
