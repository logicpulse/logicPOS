using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.FrontOffice.Pdf;

public sealed class InvoiceReceiptModel : DocumentModel
{
    public InvoiceReceiptModel(DocumentPdfData data) : base(data)
    {
    }

    protected override void DesignFinalTable(ColumnDescriptor xcolumn)
    {
        xcolumn.Item().ExtendVertical().AlignBottom().PaddingTop(3).ShowEntire().Table(finalTable =>
        {
            finalTable.ColumnsDefinition(columns => { columns.ConstantColumn(18, Unit.Centimetre); });

            finalTable.Cell().Table(DesignTotalsTable);

            if (HasCompleteTransportData())
            {
                DesignTransportShipmentSection(finalTable);
            }

            finalTable.Cell().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(9, Unit.Centimetre);
                    columns.ConstantColumn(5.5f, Unit.Centimetre);
                    columns.ConstantColumn(3.5f, Unit.Centimetre);
                });


                table.Cell().PaddingRight(6).Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                    .Height(3, Unit.Centimetre)
                    .Padding(6).Table(tb =>
                    {
                        tb.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                        });
                        tb.Header(header =>
                        {
                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background((Color.FromRGB(225, 232, 242))).Padding(2).Text("Designação");

                            header.Cell().Border(0.5f).BorderColor(Color.FromRGB(192, 194, 196))
                                .Background((Color.FromRGB(225, 232, 242))).Padding(2).Text("Montante");
                        });
                        for (int x = 0; x < Data.Document!.PaymentMethods!.Count && x < 4; x++)
                        {
                            var payment = Data.Document.PaymentMethods![x];
                            tb.Cell().Padding(1).Text(payment.Designation);
                            tb.Cell().Padding(1).Text($"{payment.Amount:N2}");
                        }
                    });

                DesignNotesAndQrSection(table);
            });
            
            finalTable.Cell().PaddingTop(3).Text(GetInvoiceDeclarationLine()).AlignCenter().FontSize(7);
        });
    }
}