namespace logicpos.shared.Classes.Finance
{
    public class TaxBagProperties
    {
        //Optional, used to add Designation when needed ex Preview
        private string _designation;
        public string Designation
        {
            get { return _designation; }
            set { _designation = value; }
        }

        private decimal _total;
        public decimal Total
        {
            get { return _total; }
            set { _total = value; }
        }

        private decimal _totalBase;
        public decimal TotalBase
        {
            get { return _totalBase; }
            set { _totalBase = value; }
        }

        public TaxBagProperties(string pDesignation, decimal pTotal, decimal pTotalBase)
        {
            _designation = pDesignation;
            _total = pTotal;
            _totalBase = pTotalBase;
        }
    }
}
