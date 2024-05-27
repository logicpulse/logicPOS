using System;

namespace LogicPOS.Printing.Tickets
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
        private TicketColumnsAlignment _align;
        public TicketColumnsAlignment Align
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
            : this(pName, pText, pWidth, TicketColumnsAlignment.Left, typeof(string), "")
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlignment pAlign)
            : this(pName, pText, pWidth, pAlign, typeof(string), string.Empty)
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlignment pAlign, Type pDateType)
            : this(pName, pText, pWidth, pAlign, typeof(string), "")
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlignment pAlign, Type pDateType, string pFormat)
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
