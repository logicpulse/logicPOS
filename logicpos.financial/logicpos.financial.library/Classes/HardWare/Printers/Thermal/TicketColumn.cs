using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using System;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal
{
    public class TicketColumn
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private TicketColumnsAlign _align;
        public TicketColumnsAlign Align
        {
            get { return _align; }
            set { _align = value; }
        }
        private Type _dataType;
        public Type DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }
        private string _format;
        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        public TicketColumn(string pName, string pText, int pWidth)
            : this(pName, pText, pWidth, TicketColumnsAlign.Left, typeof(String), "")
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlign pAlign)
            : this(pName, pText, pWidth, pAlign, typeof(String), string.Empty)
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlign pAlign, Type pDateType)
            : this(pName, pText, pWidth, pAlign, typeof(String), "")
        {
        }

        public TicketColumn(string pName, string pText, int pWidth, TicketColumnsAlign pAlign, Type pDateType, string pFormat)
        {
            _name = pName;
            _title = pText;
            _width = pWidth;
            _align = pAlign;
            _dataType = pDateType;
            _format = pFormat;
        }
    }
}
