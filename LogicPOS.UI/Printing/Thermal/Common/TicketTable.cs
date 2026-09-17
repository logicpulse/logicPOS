
using ESC_POS_USB_NET.Printer;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Printing.Enums;
using System;
using System.Collections.Generic;
using System.Data;

namespace LogicPOS.UI.Printing.Tickets
{
    public class TicketTable : DataTable
    {
        //Parameters
        private readonly List<TicketColumn> _columnsProperties;
        //Other
        private readonly char _columnDivider = ' ';
        public int TableWidth { get; set; }

        /*public ticketTable(List<TicketColumn> pColumnsProperties, int pTableWidth)
            : this(pColumnsProperties, pTableWidth)
        {
        }*/

        public TicketTable(List<TicketColumn> pColumnsProperties, int pTableWidth=48)
        {
            //Parameters
            _columnsProperties = pColumnsProperties;
            TableWidth = pTableWidth;

            //Load Data
            Load();

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

        /*public ticketTable(DataTable pDataTable, List<TicketColumn> pColumnsProperties, int pTableWidth)
        {
            _columnsProperties = pColumnsProperties;
            TableWidth = pTableWidth;

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
        }*/

        //Load Data from GetDataTableFromQuery
        private void Load()
        {
            //Get SelectedData

            //Add Columns
            string fieldName = string.Empty;

            foreach (TicketColumn column in _columnsProperties)
            {
                fieldName = column.Name;
                this.Columns.Add(fieldName, typeof(string));
            }


        }

        //Load Data from DataTable
       /* private void Load(DataTable pDataTable)
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
                object[] dataRow = new string[pDataTable.Columns.Count];

                foreach (DataColumn column in pDataTable.Columns)
                {
                    i++;
                    fieldName = column.ColumnName;
                    fieldType = column.DataType.ToString();

                    //Check if is Not Null
                    if (rowData[fieldName] != null)
                    {
                        fieldValue = rowData[fieldName].ToString();
                        
                        
                    }
                    else
                    {
                        fieldValue = string.Empty;
                    }
                    dataRow[i] = fieldValue;
                }
                this.Rows.Add(dataRow);
            }
        }*/

        //Config Ticket Columns Properties
        private void ConfigColumns()
        {
            ConfigColumns(_columnsProperties, TableWidth);
        }

        private void ShrinkFixedColumns(int availableForFixed, ref int checkTableWidth)
        {
            if (checkTableWidth <= 0 || availableForFixed <= 0)
            {
                return;
            }

            var scale = (double)availableForFixed / checkTableWidth;
            checkTableWidth = 0;

            for (var i = 0; i < Columns.Count; i++)
            {
                var ticketColumn = Columns[i].ExtendedProperties["Ticket"] as TicketColumn;
                if (ticketColumn == null || ticketColumn.Width <= 0)
                {
                    continue;
                }

                ticketColumn.Width = Math.Max(1, (int)Math.Floor(ticketColumn.Width * scale));
                checkTableWidth += ticketColumn.Width;
            }

            while (checkTableWidth > availableForFixed)
            {
                var shrunk = false;
                for (var i = 0; i < Columns.Count && checkTableWidth > availableForFixed; i++)
                {
                    var ticketColumn = Columns[i].ExtendedProperties["Ticket"] as TicketColumn;
                    if (ticketColumn == null || ticketColumn.Width <= 1)
                    {
                        continue;
                    }

                    ticketColumn.Width--;
                    checkTableWidth--;
                    shrunk = true;
                }

                if (!shrunk)
                {
                    break;
                }
            }
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

                // Shrink fixed columns when printer width is narrower than the template.
                if (checkTableWidth > pTableWidth)
                {
                    var reservedForDynamic = dynamicColumn > -1 ? Math.Min(4, Math.Max(1, pTableWidth / 5)) : 0;
                    var availableForFixed = Math.Max(pTableWidth - reservedForDynamic, pColumnsProperties.Count);
                    ShrinkFixedColumns(availableForFixed, ref checkTableWidth);
                }

                if (dynamicColumn > -1)
                {
                    (this.Columns[dynamicColumn].ExtendedProperties["Ticket"] as TicketColumn).Width =
                        Math.Max(1, pTableWidth - checkTableWidth);
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
            string result = string.Empty;
            TicketColumn ticketColumn;

            for (int i = 0; i < this.Columns.Count; i++)
            {
                ticketColumn = (TicketColumn)this.Columns[i].ExtendedProperties["Ticket"];
                if (ticketColumn.Width < 0) ticketColumn.Width = 20;
                string formatedColumn;
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

                // Build each cell to exact Width: pad content first, then add divider on non-last columns.
                if (i < this.Columns.Count - 1)
                {
                    var contentWidth = Math.Max(1, ticketColumn.Width - 1);
                    if (formatedColumn.Length > contentWidth)
                    {
                        formatedColumn = formatedColumn.Substring(0, contentWidth);
                    }

                    switch (ticketColumn.Align)
                    {
                        case TicketColumnsAlignment.Left:
                            formatedColumn = formatedColumn.PadRight(contentWidth);
                            break;
                        case TicketColumnsAlignment.Right:
                            formatedColumn = formatedColumn.PadLeft(contentWidth);
                            break;
                    }

                    formatedColumn += _columnDivider;
                }
                else
                {
                    if (formatedColumn.Length > ticketColumn.Width)
                    {
                        formatedColumn = formatedColumn.Substring(0, ticketColumn.Width);
                    }

                    switch (ticketColumn.Align)
                    {
                        case TicketColumnsAlignment.Left:
                            formatedColumn = formatedColumn.PadRight(ticketColumn.Width);
                            break;
                        case TicketColumnsAlignment.Right:
                            formatedColumn = formatedColumn.PadLeft(ticketColumn.Width);
                            break;
                    }
                }

                result += formatedColumn;
            }

            // Never exceed table width (avoids wrap of the last character).
            if (TableWidth > 0 && result.Length > TableWidth)
            {
                result = result.Substring(0, TableWidth);
            }

            return result;
        }

        public List<string> GetTable()
        {
            List<string> result = new List<string>();

            //Print Titles
            result.Add(GetLine());

            foreach (DataRow item in this.Rows)
            {
                result.Add(GetLine(item));
            }


            return result;
        }

        public void Print(Printer thermalPrinter)
        {
            Print(thermalPrinter, false, string.Empty);
        }

        public void Print(Printer thermalPrinter, bool ignoreFirstRow, string lineFormat)
        {
            List<string> table = GetTable();

            int startRow = (ignoreFirstRow || string.IsNullOrEmpty(table[0].Trim())) ? 1 : 0;

            for (int i = startRow; i < table.Count; i++)
            {
                var line = table[i];
                if (lineFormat != string.Empty)
                {
                    line = string.Format(lineFormat, line);
                }

                // Header row (titles) in bold.
                if (i == 0)
                {
                    ThermalPrinter.AppendBoldLine(thermalPrinter, line);
                }
                else
                {
                    thermalPrinter.Append(ThermalPrinter.ToThermalText(line));
                }
            }
        }

        //Debug Output
        public void ShowFieldProperties(string pField)
        {
            TicketColumn ticketColumn;

            foreach (DataRow item in this.Rows)
            {
                ticketColumn = (this.Columns[pField].ExtendedProperties.ContainsKey("Ticket")) ? (TicketColumn)this.Columns[pField].ExtendedProperties["Ticket"] : null;
            }
        }

        //STATIC : Init DataTable Structure/Columns from List<TicketColumn>
        public static DataTable InitDataTableFromTicketColumns(List<TicketColumn> ticketColumns)
        {
            if (ticketColumns.Count <= 0) 
            {
                return null;
            }
            DataTable result = new DataTable();

                foreach (var item in ticketColumns)
                {
                    DataColumn dataColumn = new DataColumn(item.Name, item.DataType);
                    result.Columns.Add(dataColumn);
                }

            return result;
        }
    }
}
