//TK016268 Angola - Certificação 

namespace LogicPOS.Finance.Saft
{
    public partial class SaftAo
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //<SourceDocuments><SalesInvoices><Invoice><Line>
        public class TotalLinesResult
        {
            public decimal TaxPayable;
            public decimal NetTotal;
            public decimal GrossTotal;//TotalFinal

            public TotalLinesResult()
            {
                TaxPayable = 0.0m;
                NetTotal = 0.0m;
                GrossTotal = 0.0m;
            }
        }
    }
}