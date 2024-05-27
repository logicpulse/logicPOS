using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using System;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal
{
    public class TicketColumn
    {
        public string Name { get; set; }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public int Width { get; set; }
        private TicketColumnsAlign _align;
        public TicketColumnsAlign Align
        {
            get { return _align; }
            set { _align = value; }
        }

        public Type DataType { get; set; }
        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public TicketColumn(string pName, string pText, int pWidth)
            : this(pName, pText, pWidth, TicketColumnsAlign.Left, typeof(string), "")
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlign pAlign)
            : this(pName, pText, pWidth, pAlign, typeof(string), string.Empty)
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlign pAlign, Type pDateType)
            : this(pName, pText, pWidth, pAlign, typeof(string), "")
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlign pAlign, Type pDateType, string pFormat)
        {
            Name = pName;
            _title = pText;
            Width = pWidth;
            _align = pAlign;
            DataType = pDateType;
            _format = pFormat;
        }
    }
}
