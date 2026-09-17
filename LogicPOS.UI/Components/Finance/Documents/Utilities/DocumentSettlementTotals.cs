using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Documents.Utilities
{
    /// <summary>
    /// Net amount to settle (cash on receipt), aligned with API <c>DocumentPaymentValidator</c>.
    /// FT/ND outstanding (<see cref="DocumentViewModel.TotalToPay"/>) minus NC totals (<see cref="DocumentViewModel.TotalFinal"/>).
    /// </summary>
    public static class DocumentSettlementTotals
    {
        public static decimal CalculateNetSettlementAmount(IEnumerable<DocumentViewModel> documents)
        {
            if (documents == null)
            {
                return 0;
            }

            var list = documents as IList<DocumentViewModel> ?? documents.ToList();
            if (list.Count == 0)
            {
                return 0;
            }

            var invoicesDebit = list
                .Where(d => d.Type == "FT" || d.Type == "ND")
                .Sum(d => d.TotalToPay);

            var creditNotesTotal = list
                .Where(d => d.Type == "NC")
                .Sum(d => d.TotalFinal);

            return Round(invoicesDebit - creditNotesTotal);
        }

        private static decimal Round(decimal value) => decimal.Round(value, 2);
    }
}
