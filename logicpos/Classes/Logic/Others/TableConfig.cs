namespace logicpos.Classes.Logic.Others
{
    public class TableConfig
    {
        uint _rows;
        public uint Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        uint _columns;
        public uint Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public TableConfig() { }
        public TableConfig(uint pRows, uint pColumns)
        {
            _rows = pRows;
            _columns = pColumns;
        }
    }
}
