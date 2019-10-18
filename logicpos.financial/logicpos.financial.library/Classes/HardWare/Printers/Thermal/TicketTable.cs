using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal
{
    public class TicketTable : DataTable
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Parameters
        private Session _session;
        private List<TicketColumn> _columnsProperties;
        //Other
        private char _columnDivider = ' ';
        //Public
        private int _tableWidth;
        public int TableWidth
        {
            get { return _tableWidth; }
            set { _tableWidth = value; }
        }
        
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

        public TicketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
        {
            _columnsProperties = pColumnsProperties;
            _tableWidth = pTableWidth;

            //Load Data
            Load(pDataTable);

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

        //Load Data from GetDataTableFromQuery
        private void Load(string pSql)
        {
            //Get SelectedData
            XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(_session, pSql);
            //Add Columns
            string fieldName;
            string fieldType;
            string fieldValue;

            foreach (SelectStatementResultRow column in xPSelectData.Meta)
            {
                fieldName = column.Values[0].ToString();
                fieldType = column.Values[2].ToString();
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
                        fieldValue = string.Empty;
                    }
                    dataRow[xPSelectData.GetFieldIndex(fieldName)] = fieldValue;
                }
                this.Rows.Add(dataRow);
            }
        }

        //Load Data from DataTable
        private void Load(DataTable pDataTable)
        {
            //Add Columns
            string fieldName;
            string fieldType;
            string fieldValue;

            foreach (DataColumn column in pDataTable.Columns)
            {
                fieldName = column.ColumnName;
                fieldType = column.DataType.ToString();
                this.Columns.Add(fieldName, typeof(string));
            }

            //Add Rows
            foreach (DataRow rowData in pDataTable.Rows)
            {
                int i = -1;
                //Init a new DataRow
                Object[] dataRow = new String[pDataTable.Columns.Count];

                foreach (DataColumn column in pDataTable.Columns)
                {
                    i++;
                    fieldName = column.ColumnName;
                    fieldType = column.DataType.ToString();

                    //Check if is Not Null
                    if (rowData[fieldName] != null)
                    {
                        fieldValue = FrameworkUtils.FormatDataTableFieldFromType(rowData[fieldName].ToString(), fieldType);
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }
                    dataRow[i] = fieldValue;
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
                    this.Columns[i].ExtendedProperties.Add("Ticket", new TicketColumn(item.Name, item.Title, item.Width, item.Align, item.DataType, item.Format));
                }

                //Check if final Table Width is bigger than pTableWidth
                if (checkTableWidth > pTableWidth)
                {
                    _log.Error($"ConfigColumns > checkTableWidth: [{checkTableWidth}] > [{pTableWidth}]");
                    throw new Exception($"Error columns to large to fit{Environment.NewLine}checkTableWidth: [{checkTableWidth}] > [{pTableWidth}]");
                }
                else if (dynamicColumn > -1)//0
                {
                    //Calc and Assign Dynamic Width
                    (this.Columns[dynamicColumn].ExtendedProperties["Ticket"] as TicketColumn).Width = pTableWidth - checkTableWidth;
                }
            }
        }

        /// <summary>
        /// Print Table Titles
        /// </summary>
        /// <param name="DataRow"></param>
        /// <returns></returns>
        public string GetLine()
        {
            return GetLine(null);
        }

        public string GetLine(DataRow pDataRow)
        {
            bool debug = false;
            string result = string.Empty;
            string formatedColumn = string.Empty;
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
                    if (debug) _log.Debug(string.Format(
                        "FormatedColumn[{0}]: [{1}], Width: [{2}], Align: [{3}], Format: [{4}]",
                        i, formatedColumn, ticketColumn.Width,
                        Enum.GetName(typeof(TicketColumnsAlign), ticketColumn.Align),
                        (ticketColumn.Format != string.Empty) ? ticketColumn.Format : "NOFORMAT")
                    );
                }
                if (debug) _log.Debug(string.Format("result: [{0}], Chars: [{1}]", result, result.Length));

            }
            catch (Exception ex)
            {
                _log.Debug("string GetLine(DataRow pDataRow) :: " + ex.Message, ex);
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
                _log.Debug("List<string> GetTable() :: " + ex.Message, ex);
                throw ex;
            }

            return result;
        }

        public void Print(ThermalPrinterGeneric pThermalPrinterGeneric)
        {
            Print (pThermalPrinterGeneric, WriteLineTextMode.Normal);
        }

        public void Print(ThermalPrinterGeneric pThermalPrinterGeneric, WriteLineTextMode pTextMode)
        {
            Print(pThermalPrinterGeneric, pTextMode, false, string.Empty);
        }

        public void Print(ThermalPrinterGeneric pThermalPrinterGeneric, bool pIgnoreFirstRow)
        {
            Print(pThermalPrinterGeneric, WriteLineTextMode.Normal, pIgnoreFirstRow, string.Empty);
        }

        public void Print(ThermalPrinterGeneric pThermalPrinterGeneric, string pLineFormat)
        {
            Print(pThermalPrinterGeneric, WriteLineTextMode.Normal, false, pLineFormat);
        }

        public void Print(ThermalPrinterGeneric pThermalPrinterGeneric, WriteLineTextMode pTextMode, bool pIgnoreFirstRow)
        {
            Print(pThermalPrinterGeneric, pTextMode, pIgnoreFirstRow, string.Empty);
        }

        public void Print(ThermalPrinterGeneric pThermalPrinterGeneric, WriteLineTextMode pTextMode, bool pIgnoreFirstRow, string pLineFormat)
        {
            bool debug = false;

            try
            {
                List<string> table = GetTable();
                
                int startRow = (pIgnoreFirstRow || string.IsNullOrEmpty(table[0].Trim())) ? 1 : 0;

                for (int i = startRow; i < table.Count; i++)
                {
                    if (debug) _log.Debug(string.Format("Table Row: [{0}], TextMode: [{1}]", table[i], Enum.GetName(typeof(WriteLineTextMode), pTextMode)));
                    //Apply Format to Row
                    if (pLineFormat != string.Empty) table[i] = string.Format(pLineFormat, table[i]);
                    //if (debug) _log.Debug(String.Format("pLineFormat:[{0}], table:[{1}]: ", pLineFormat, table[i]));
                    //Print Row
                    pThermalPrinterGeneric.WriteLine(table[i], pTextMode);
                }
            }
            catch (Exception ex)
            {
                _log.Debug("void Print(ThermalPrinterGeneric pThermalPrinterGeneric, WriteLineTextMode pTextMode, bool pIgnoreFirstRow, string pLineFormat) :: " + ex.Message, ex);
                throw ex;
            }
        }

        //Debug Output
        public void ShowFieldProperties(string pField)
        {
            TicketColumn ticketColumn;

            foreach (DataRow item in this.Rows)
            {
                ticketColumn = (this.Columns[pField].ExtendedProperties.ContainsKey("Ticket")) ? (TicketColumn)this.Columns[pField].ExtendedProperties["Ticket"] : null;

                _log.Debug(string.Format("[{0}], [{1}], [{2}], [{3}], [{4}], [{5}]",
                    item[pField],
                    this.Columns[pField].ColumnName,
                    this.Columns[pField].DataType,
                    ticketColumn.Title,
                    ticketColumn.Width,
                    ticketColumn.Align
                 ));
            }
        }

        //STATIC : Init DataTable Structure/Columns from List<TicketColumn>
        public static DataTable InitDataTableFromTicketColumns(List<TicketColumn> pTicketColumns)
        {
            DataTable result = new DataTable();

            try
            {
                foreach (var item in pTicketColumns)
                {
                    DataColumn dataColumn = new DataColumn(item.Name, item.DataType);
                    result.Columns.Add(dataColumn);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
    }
}
