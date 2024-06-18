namespace LogicPOS.Printing.Documents
{
    public partial class WorkSessionPrinter
    {
        private class DataTableGroupProperties
        {
            public string Title { get; set; }

            public string Sql { get; set; }

            public bool Enabled { get; set; }

            public DataTableGroupProperties(string pTitle, string pSql) : this(pTitle, pSql, true) { }
            public DataTableGroupProperties(string pTitle, string pSql, bool pEnabled)
            {
                Title = pTitle;
                Sql = pSql;
                Enabled = pEnabled;
            }
        }
    }
}
