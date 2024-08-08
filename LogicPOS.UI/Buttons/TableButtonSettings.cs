using LogicPOS.Domain.Enums;
using System;

namespace LogicPOS.UI.Buttons
{
    public class TableButtonSettings : ButtonSettings
    {
        public TableStatus TableStatus { get; set; }
        public decimal Total { get; set; }
        public DateTime OpenedAt { get; set; }
        public DateTime ClosedAt { get; set; }
    }
}
