namespace logicpos.Classes.Logic.Others
{
    public class TableConfig
    {
        public uint Rows { get; set; }

        public uint Columns { get; set; }

        public TableConfig() { }
        public TableConfig(uint pRows, uint pColumns)
        {
            Rows = pRows;
            Columns = pColumns;
        }
    }
}
