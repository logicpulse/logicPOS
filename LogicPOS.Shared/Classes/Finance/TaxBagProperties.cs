namespace logicpos.shared.Classes.Finance
{
    public class TaxBagProperties
    {
        public string Designation { get; set; }

        public decimal Total { get; set; }

        public decimal TotalBase { get; set; }

        public TaxBagProperties(string pDesignation, decimal pTotal, decimal pTotalBase)
        {
            Designation = pDesignation;
            Total = pTotal;
            TotalBase = pTotalBase;
        }
    }
}
