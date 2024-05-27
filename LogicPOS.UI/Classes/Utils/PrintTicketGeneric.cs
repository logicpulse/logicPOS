using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.financial;
using logicpos.financial.DataLayer.Xpo;
using logicpos.finantial.DataLayer.Xpo;
using System;
using System.Collections.Generic;
using System.Data;
using ThermalDotNet;

namespace logicpos
{
    public enum TicketColumnsAlign
    {
        Left, Right
    }

    public class TicketColumn
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
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

        public TicketColumn(string pText, int pWidth)
            : this(pText, pWidth, TicketColumnsAlign.Left, typeof(String), "")
        {
        }

        public TicketColumn(string pText, int pWidth, TicketColumnsAlign pAlign)
            : this(pText, pWidth, pAlign, typeof(String), String.Empty)
        {
        }

        public TicketColumn(string pText, int pWidth, TicketColumnsAlign pAlign, Type pDateType, string pFormat)
        {
            _Title = pText;
            _width = pWidth;
            _align = pAlign;
            _dataType = pDateType;
            _format = pFormat;
        }
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class TicketTable : DataTable
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Parameters
        private Session _session;
        private List<TicketColumn> _columnsProperties;
        private int _tableWidth;
        //Other
        private char _columnDivider = ' ';

        public TicketTable(string pSql, List<TicketColumn> pColumnsProperties, int pTableWidth)
            : this(GlobalFramework.SessionXpo, pSql, pColumnsProperties, pTableWidth)
        {
        }

        public TicketTable(Session pSession, string pSql, List<TicketColumn> pColumnsProperties, int pTableWidth)
        {
            //Parameters
            _session = pSession;
            _columnsProperties = pColumnsProperties;
            _tableWidth = pTableWidth;

            //Load Data
            Load(pSql);

            //Check Columns after Load Data
            if (!this.Columns.Count.Equals(pColumnsProperties.Count))
            {
                throw new Exception(string.Format("Error data columns [{0}] and column properties [{1}] are not equal!", this.Columns.Count, pColumnsProperties.Count));
            }
            else
            {
                //Prepare Ticket Columns
                ConfigColumns();
            }
        }

        //Reused code from GetDataTableFromQuery
        private void Load(string pSql)
        {
            //Get SelectedData
            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(_session, pSql);
            //Add Columns
            String fieldName;
            String fieldType;
            String fieldValue;

            foreach (SelectStatementResultRow row in xPSelectData.Meta)
            {
                fieldName = row.Values[0].ToString();
                fieldType = row.Values[2].ToString();
                this.Columns.Add(fieldName, typeof(string));
            }

            //Add Rows
            foreach (SelectStatementResultRow rowData in xPSelectData.Data)
            {
                //Init a new DataRow
                Object[] dataRow = new String[xPSelectData.Meta.Length];

                foreach (SelectStatementResultRow rowMeta in xPSelectData.Meta)
                {
                    fieldName = rowMeta.Values[0].ToString();
                    fieldType = rowMeta.Values[2].ToString();
                    //Check if is Not Null
                    if (rowData.Values[xPSelectData.GetFieldIndex(fieldName)] != null)
                    {
                        fieldValue = FrameworkUtils.FormatDataTableFieldFromType(rowData.Values[xPSelectData.GetFieldIndex(fieldName)].ToString(), fieldType);
                    }
                    else
                    {
                        fieldValue = String.Empty;
                    }
                    dataRow[xPSelectData.GetFieldIndex(fieldName)] = fieldValue;
                }
                this.Rows.Add(dataRow);
            }
        }

        //Config Ticket Columns Properties
        private void ConfigColumns()
        {
            ConfigColumns(_columnsProperties, _tableWidth);
        }

        //Can be used Outside
        private void ConfigColumns(List<TicketColumn> pColumnsProperties, int pTableWidth)
        {
            //Add Ticket Column Properties to ExtendedProperties
            if (pColumnsProperties != null)
            {
                int i = -1;
                int checkTableWidth = 0;
                int dynamicColumn = -1;

                foreach (TicketColumn item in pColumnsProperties)
                {
                    i++;

                    //Sum final TableWidth
                    checkTableWidth += item.Width;
                    //First Detected
                    if (item.Width == 0 && dynamicColumn == -1)
                    {
                        dynamicColumn = i;
                    }

                    //Add Config Columns to ExtendedProperties
                    this.Columns[i].ExtendedProperties.Add("Ticket", new TicketColumn(item.Title, item.Width, item.Align, item.DataType, item.Format));
                }

                //Check if final Table Width is bigger than pTableWidth
                if (checkTableWidth > pTableWidth)
                {
                    throw new Exception("Error columns to large to fit");
                }
                else if (dynamicColumn > 0)
                {
                    //Calc and Assign Dynamic Width
                    (this.Columns[dynamicColumn].ExtendedProperties["Ticket"] as TicketColumn).Width = pTableWidth - checkTableWidth;
                }
            }
        }

        /// <summary>
        /// Print table titles
        /// </summary>
        /// <param name="pDataRow"></param>
        /// <returns></returns>
        public string GetLine()
        {
            return GetLine(null);
        }

        public string GetLine(DataRow pDataRow)
        {
            bool debug = true;
            string result = String.Empty;
            string formatedColumn = String.Empty;
            TicketColumn ticketColumn;

            try
            {
                for (int i = 0; i < this.Columns.Count; i++)
                {
                    ticketColumn = (TicketColumn)this.Columns[i].ExtendedProperties["Ticket"];
                    //Title Mode
                    if (pDataRow == null)
                    {
                        formatedColumn = ticketColumn.Title;
                    }
                    //Data Mode : Use Format
                    else
                    {
                        formatedColumn = pDataRow.ItemArray[i].ToString();
                        //Require to Convert to Type, else INVALID Formats
                        if (ticketColumn.Format != string.Empty)
                        {
                            if (ticketColumn.DataType == typeof(int))
                            {
                                formatedColumn = string.Format(ticketColumn.Format, Convert.ToInt32(formatedColumn));
                            }
                            else if (ticketColumn.DataType == typeof(decimal))
                            {
                                formatedColumn = string.Format(ticketColumn.Format, Convert.ToDecimal(formatedColumn));
                            }
                            else
                            {
                                formatedColumn = string.Format(ticketColumn.Format, formatedColumn);
                            }                            
                        }
                    }

                    //Add Divider and Trim Column if not last Column
                    if (i < this.Columns.Count - 1)
                    {
                        if (formatedColumn.Length > ticketColumn.Width - 1) formatedColumn = formatedColumn.Substring(0, ticketColumn.Width - 1);
                        formatedColumn += _columnDivider;
                    }
                    else
                    {
                        if (formatedColumn.Length > ticketColumn.Width) formatedColumn = formatedColumn.Substring(0, ticketColumn.Width);
                    }

                    //Padding after Substr
                    switch (ticketColumn.Align)
                    {
                        case TicketColumnsAlign.Left:
                            formatedColumn = formatedColumn.PadRight(ticketColumn.Width);
                            break;
                        case TicketColumnsAlign.Right:
                            formatedColumn = formatedColumn.PadLeft(ticketColumn.Width);
                            break;
                    }

                    result += formatedColumn;
                    if (debug) _log.Debug(String.Format(
                        "FormatedColumn[{0}]: [{1}], Width: [{2}], Align: [{3}], Format: [{4}]", 
                        i, formatedColumn, ticketColumn.Width, 
                        Enum.GetName(typeof(TicketColumnsAlign), ticketColumn.Align), 
                        (ticketColumn.Format != string.Empty) ? ticketColumn.Format : "NOFORMAT")
                    );
                }
                _log.Debug(String.Format("result: [{0}], Chars: [{1}]", result, result.Length));

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<string> GetTable()
        {
            List<string> result = new List<string>();
            try
            {
                //Print Titles
                result.Add(GetLine());

                foreach (DataRow item in this.Rows)
                {
                    result.Add(GetLine(item));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        /*
                public bool PrintTable(ThermalPrinter pThermalPrinter)
                {
                    bool result = false;
                    List<string> table = GetTable();

                    try
                    {
                        for (int i = 0; i < table.Count; i++)
                        {
                    
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message);
                    }

                    return result;
                }
        */

        //Debug
        public void ShowFieldProperties(string pField)
        {
            TicketColumn ticketColumn;

            foreach (DataRow item in this.Rows)
            {
                ticketColumn = (this.Columns[pField].ExtendedProperties.ContainsKey("Ticket")) ? (TicketColumn)this.Columns[pField].ExtendedProperties["Ticket"] : null;

                _log.Debug(String.Format("Message: [{0}], [{1}], [{2}], [{3}], [{4}], [{5}]",
                    item[pField],
                    this.Columns[pField].ColumnName,
                    this.Columns[pField].DataType,
                    ticketColumn.Title,
                    ticketColumn.Width,
                    ticketColumn.Align
                 ));
            }
        }

        //public string GetRow(int pTableWidth)
        //{
        //    string result = string.Empty;
        //}
    }

    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    /*
        public class TicketTable : List<TicketColumn>
        {
            //Log4Net
            private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            private char _columnDivider = '|';
            private List<TicketColumn> _columns;

            public TicketTable(List<TicketColumn> pColumns, int pTableWidth)
            {
                _columns = pColumns;

                int checkTableWidth = 0;
                int dynamicColumn = -1;

                for (int i = 0; i < _columns.Count; i++)
                {
                    checkTableWidth += _columns[i].Width;
                    //First Detected
                    if (_columns[i].Width == 0 && dynamicColumn == -1)
                    {
                        dynamicColumn = i;
                    }
                }

                if (checkTableWidth > pTableWidth)
                {
                    throw new Exception("Error columns to large to fit");
                }
                else
                {
                    //Calc Dynamic Width
                    _columns[dynamicColumn].Width = pTableWidth - checkTableWidth;
                }
            }

            public string GenTableLine()
            {
                bool debug = true;
                string result = String.Empty;
                string formatedColumn = String.Empty;

                try
                {
                    for (int i = 0; i < _columns.Count; i++)
                    {
                        formatedColumn = _columns[i].Title;//.Substring(0, _columns[i].Width - 1);

                        switch (_columns[i].Align)
                        {
                            case TicketColumnsAlign.Left:
                                formatedColumn = formatedColumn.PadRight(_columns[i].Width);
                                break;
                            case TicketColumnsAlign.Right:
                                formatedColumn = formatedColumn.PadLeft(_columns[i].Width);
                                break;
                        }
                        //Add Divider and Trim Column if not last Column
                        if (i < _columns.Count - 1)
                        {
                            if (formatedColumn.Length > _columns[i].Width - 1) formatedColumn = formatedColumn.Substring(0, _columns[i].Width - 1);
                            formatedColumn += _columnDivider;
                        }
                        else
                        {
                            if (formatedColumn.Length > _columns[i].Width) formatedColumn = formatedColumn.Substring(0, _columns[i].Width);
                        }

                        result += formatedColumn;
                        if (debug) _log.Debug(String.Format("FormatedColumn[i].Width[{0}]: [{1}], Width: [{2}], Align: [{3}]", i, formatedColumn, _columns[i].Width, Enum.GetName(typeof(TicketColumnsAlign), _columns[i].Align)));
                    }
                    //result.Add(joinedColumns);
                    //joinedColumns = String.Empty;

                    _log.Debug(String.Format("result: [{0}], Chars: [{1}]", result, result.Length));

                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                }

                return result;
            }

            public void PrintTable(ThermalPrinter pThermalPrinter)
            {

            }
        }
    */
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class PrintTicketGeneric : ThermalPrinter
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Move to Database Printer
        private int _charsPerLine = 64;
        private int _charsPerLineDouble;
        private string _encoding = String.Empty;
        //Object Fields        
        //ThermalPrinter _thermalPrinter;
        ConfigurationPrinters _printer;
        private char _lineChar = '-';
        private string _line = String.Empty;

        public PrintTicketGeneric(ConfigurationPrinters pPrinter, string pEncoding = "PC860")
            : base(pEncoding)
        {
            //Parameters
            _printer = pPrinter;
            _encoding = pEncoding;
            //Other
            _charsPerLineDouble = _charsPerLine / 2;
            _line = new String(_lineChar, _charsPerLine);
        }

        public void PrintBuffer()
        {
            switch (_printer.PrinterType.Token)
            {
                case "SINOCAN_GENERIC_WINDOWS":
                    SinocanGenericWindows.Print.WindowsPrint(_printer.NetworkName, getByteArray());
                    break;
                case "SINOCAN_GENERIC_LINUX":
                    SinocanGenericLinux.Print.LinuxPrint(_printer.NetworkName, getByteArray());
                    break;
                case "SINOCAN_GENERIC_SOCKET":
                    //SinocanGenericSocket.Print.SocketPrint.SocketPrint(pPrinter.NetworkName, _thermalPrinter.getByteArray());
                    break;
            }
        }

        public void Test()
        {
            WriteLine("LINE");
            WriteLine(_line);

            WriteToBuffer("X1");
            WriteToBuffer("X2");
            WriteToBuffer("X3");
            LineFeed();

            WriteLine(_line);
            WriteLine("123456789-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _charsPerLine));
            WriteLine("123456789-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _charsPerLine), (byte)ThermalPrinter.PrintingStyle.Bold);
            WriteLine("123456789-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _charsPerLineDouble), (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
            WriteLine("LINE");
            LineFeed();
            Cut(true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Writers
        /*
                public void Write(byte[] pOutput)
                {
                    try
                    {
                        //Only Write if _usbWriter is Enabled else ignore all calls, this way we can call Write in Pos, and if Device is Missing it skips all writes
                        if (_usbWriter != null)
                        {
                            // write data, read data
                            int bytesWritten;
                            _usbErrorCode = _usbWriter.Write(pOutput, 2000, out bytesWritten);

                            //ErrorCode Enumeration
                            //http://libusbdotnet.sourceforge.net/V2/html/c3eab258-a324-25c8-68ac-06ecf6e0fe7f.htm
                            if (_usbErrorCode != ErrorCode.None)
                            {
                                Close();
                                // Write that output to the console.
                                _log.Error(UsbDevice.LastErrorString);
                                //throw new Exception(UsbDevice.LastErrorString);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }
                }
        */
        //public void Write(string pOutput)
        //{
        //    if (pOutput.Length > _charactersPerLine * 2)
        //    {
        //        pOutput = pOutput.Substring(0, Convert.ToInt16(_charactersPerLine * 2));
        //    }

        //    byte[] output = UsbDisplayDevice.GetBytes(pOutput);
        //    Write(output);
        //}

        public static string TextJustified(string pLeft, string pRight, int pMaxPerLine)
        {
            return TextJustified(pLeft, pRight, pMaxPerLine, "{0,-10}{1,10}");
        }

        public static string TextJustified(string pLeft, string pRight, int pMaxPerLine, string pFormat)
        {
            return String.Format(pFormat, pLeft, pRight);
        }

        public static string TextCentered(string stringToCenter, int pCharactersPerLine)
        {
            return stringToCenter.PadLeft(((pCharactersPerLine - stringToCenter.Length) / 2) + stringToCenter.Length).PadRight(pCharactersPerLine);
        }

        public static string TextCentered(string stringToCenter, int totalLength, char paddingCharacter)
        {
            return stringToCenter.PadLeft(((totalLength - stringToCenter.Length) / 2) + stringToCenter.Length, paddingCharacter).PadRight(totalLength, paddingCharacter);
        }

    }
}
