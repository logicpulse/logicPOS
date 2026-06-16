using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument;
using LogicPOS.Api.Features.Finance.Documents.Documents.Sdr;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Printing;
using System.Collections.Generic;
using DocumentDetail = LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument.DocumentDetail;
using IssueDocumentPaymentMethod = LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument.DocumentPaymentMethod;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    public static class VoltaRefundReceiptService
    {
        public static bool TryIssue(Window parent, Customer customer, PaymentMethod paymentMethod, decimal quantity)
        {
            if (quantity <= 0 || customer == null || paymentMethod == null)
            {
                return false;
            }

            if (FiscalYearsService.HasActiveFiscalYear() == false)
            {
                FiscalYearsService.ShowOpenFiscalYearAlert(parent);
                return false;
            }

            var depositArticle = TrvDocumentUiRules.ResolveDepositArticle();
            if (depositArticle == null)
            {
                CustomAlerts.Warning(parent)
                    .WithTitle("Artigo SDR em falta")
                    .WithMessage("Crie o artigo de depósito SDRVDEP antes de emitir o Talão Reembolso Volta.")
                    .ShowAlert();
                return false;
            }

            var total = SdrDepositAmountCalculator.CalculateLineTotal(quantity, depositArticle);
            var currency = CurrenciesService.Default;

            var command = new IssueDocumentCommand
            {
                Type = TrvDocumentUiRules.DocumentTypeAcronym,
                CustomerId = customer.Id,
                CurrencyId = currency?.Id,
                PaymentMethods = new List<IssueDocumentPaymentMethod>
                {
                    new IssueDocumentPaymentMethod
                    {
                        PaymentMethodId = paymentMethod.Id,
                        Amount = total
                    }
                },
                Details = new List<DocumentDetail>
                {
                    new DocumentDetail
                    {
                        ArticleId = depositArticle.Id,
                        Quantity = quantity,
                        UnitPrice = depositArticle.Price1,
                        VatRateId = depositArticle.VatRateId,
                        VatExemptionId = depositArticle.VatExemptionReasonId,
                        Discount = 0
                    }
                }
            };

            var printingData = DocumentsService.IssueDocumentForPrinting(command);
            if (printingData == null)
            {
                return false;
            }

            if (ThermalPrintingService.PrintInvoice(printingData.Value))
            {
                DocumentsService.RegisterPrint(printingData.Value.DocumentId, new List<int> { 1 }, false, null, true);
            }

            return true;
        }
    }
}
