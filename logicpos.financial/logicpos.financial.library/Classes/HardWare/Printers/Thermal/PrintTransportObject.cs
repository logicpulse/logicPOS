using System;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal
{
    //Transport object, to Transport binary Objects to External Applications
    [Serializable]
    public class PrintTransportObject
    {
        private int _copyNames;
        public int CopyNames
        {
            get { return _copyNames; }
            set { _copyNames = value; }
        }

        private DataTable _dataTableLoop;
        public DataTable DataTableLoop
        {
            get { return _dataTableLoop; }
            set { _dataTableLoop = value; }
        }

        private DataTable _dataTableStatic;
        public DataTable DataTableStatic
        {
            get { return _dataTableStatic; }
            set { _dataTableStatic = value; }
        }

        public PrintTransportObject() { }
        public PrintTransportObject(int pCopyNames, DataTable pDataTableLoop, DataTable pDataTableStatic)
        {
            _copyNames = pCopyNames;
            _dataTableLoop = pDataTableLoop;
            _dataTableStatic = pDataTableStatic;
        }
    }
}
