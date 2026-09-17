using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.POS
{
    public class PosTicket
    {
        public int Number { get; }
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

        public PosTicket(int number)
        {
            Number = number;
        }

        public decimal TotalFinal => Items.Sum(i => i.TotalFinal);
        public decimal ServicesTotalFinal => Items.Where(i => i.Article.ClassAcronym == "S").Sum(i => i.TotalFinal);
    }
}
