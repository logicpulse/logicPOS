using LogicPOS.UI.Printing.Enums;
using System;

namespace LogicPOS.UI.Printing.Tickets
{
    public class TicketColumn
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public int Width { get; set; }
        public TicketColumnsAlignment Align { get; set; }
        public Type DataType { get; set; }
        public string Format { get; set; }
    
        public TicketColumn(
            string name, 
            string text, 
            int width)
            : this(
                  name, 
                  text, 
                  width, 
                  TicketColumnsAlignment.Left, 
                  typeof(string), 
                  "")
        {
        }

        public TicketColumn(
            string name, 
            string text, 
            int width, 
            TicketColumnsAlignment align)
            : this(
                  name, 
                  text, 
                  width, 
                  align, 
                  typeof(string), 
                  string.Empty)
        {
        }


        public TicketColumn(
            string name, 
            string text, 
            int width, 
            TicketColumnsAlignment align, 
            Type dataType, 
            string format)
        {
            Name = name;
            Title = text;
            Width = width;
            Align = align;
            DataType = dataType;
            Format = format;
        }
    }
}
