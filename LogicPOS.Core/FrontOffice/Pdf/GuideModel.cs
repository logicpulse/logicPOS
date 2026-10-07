using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.FrontOffice.Pdf;

public class GuideModel : DocumentModel
{
    public GuideModel(DocumentPdfData data) : base(data)
    {
    }

    private void DesignGuideInfoTable(ColumnDescriptor xcolumn)
    {
        xcolumn.Item().ExtendVertical().AlignBottom().PaddingTop(3).ShowEntire().Table(finalTable =>
        {
            finalTable.ColumnsDefinition(columns => { columns.ConstantColumn(18, Unit.Centimetre); });

            finalTable.Cell().Table(DesignTotalsTable);

            DesignTransportShipmentSection(finalTable);

            finalTable.Cell().Height(10);
            finalTable.Cell().Table(qrTable =>
            {
                qrTable.ColumnsDefinition(col =>
                {
                    col.ConstantColumn(14, Unit.Centimetre);
                    col.ConstantColumn(4, Unit.Centimetre);
                });

                DesignNotesAndQrSection(qrTable);
            });

            if (Data.Country.IsAngola)
            {
                finalTable.Cell().Table(auditTable =>
                {
                    auditTable.ColumnsDefinition(col =>
                    {
                        col.ConstantColumn(9, Unit.Centimetre);
                        col.ConstantColumn(9, Unit.Centimetre);
                    });

                    auditTable.Cell().PaddingVertical(5, Unit.Millimetre).PaddingHorizontal(5, Unit.Millimetre)
                        .PaddingBottom(0).Text("Entregou").Bold();
                    auditTable.Cell().PaddingVertical(5, Unit.Millimetre).PaddingHorizontal(5, Unit.Millimetre)
                        .PaddingBottom(0).Text("Recebeu").Bold();

                    auditTable.Cell().PaddingVertical(2, Unit.Millimetre).PaddingHorizontal(5, Unit.Millimetre)
                        .PaddingRight(2, Unit.Centimetre).LineHorizontal(0.1f, Unit.Millimetre);
                    auditTable.Cell().PaddingVertical(2, Unit.Millimetre).PaddingHorizontal(5, Unit.Millimetre)
                        .PaddingRight(2, Unit.Centimetre).LineHorizontal(0.1f, Unit.Millimetre);
                });
            }

            finalTable.Cell().PaddingTop(3).Text("Este documento não serve de fatura").AlignCenter().FontSize(7);
        });
    }

    protected override void DesignFinalTable(ColumnDescriptor xcolumn)
    {
        DesignGuideInfoTable(xcolumn);
    }

    protected override void DesignNotesAndQrSection(TableDescriptor table)
    {
        GetReferences();
        AddNotesCell(table);
    }

    protected override void AddNotesCell(TableDescriptor table)
    {
        table.Cell().ColumnSpan(2)
            .Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196))
            .Height(1.5f, Unit.Centimetre)
            .Padding(6)
            .Text(txt =>
            {
                
                txt.DefaultTextStyle(ts => ts.FontSize(7));
                txt.Span("Notas\n").Bold();

                if (Data.Document!.RelatedDocuments?.Count() != 0)
                {
                    txt.Span($" | Documento de Origem: ");
                    foreach (var doc in Data.Document.RelatedDocuments!)
                    {
                        txt.Span($"{doc}").Bold();
                    }
                }
            });
    }

    protected override void DesignTotalsTable(TableDescriptor table)
    {
        table.ColumnsDefinition(columns =>
        {
            columns.ConstantColumn(9, Unit.Centimetre);
            columns.ConstantColumn(5.5f, Unit.Centimetre);
            columns.ConstantColumn(3.5f, Unit.Centimetre);
        });

        table.Cell().PaddingTop(-4).PaddingRight(6).Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196))
            .Table(DesignTaxResumesTable);

        table.Cell().PaddingTop(-4).Border(0.5f)
            .BorderColor(Color.FromRGB(192, 194, 196))
            .Table(discountTable =>
            {
                discountTable.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(3, Unit.Centimetre);
                    columns.ConstantColumn(2.5f, Unit.Centimetre);
                });

                discountTable.Cell().PaddingTop(3).Text("Desconto Cliente (%)").AlignRight();
                discountTable.Cell().PaddingTop(3).PaddingRight(6).Text($"{Data.Document!.Discount:N2}")
                    .AlignRight();

                discountTable.Cell().PaddingTop(3).Text("Total de Descontos").AlignRight();
                discountTable.Cell().PaddingTop(3).PaddingRight(6).Text($"{Data.Document.TotalDiscount:N2}")
                    .AlignRight();

                discountTable.Cell().PaddingTop(3).Text("Total líquido").AlignRight();
                discountTable.Cell().PaddingTop(3).PaddingRight(6).Text($"{Data.Document.TotalNet:N2}")
                    .AlignRight();

                discountTable.Cell().PaddingTop(3).Text("Total Imposto").AlignRight();
                discountTable.Cell().PaddingTop(3).PaddingRight(6).Text($"{Data.Document.TotalTax:N2}")
                    .AlignRight();

                if (Data.Country.IsAngola)
                {
                    discountTable.Cell().PaddingTop(3).Text("Retenção na Fonte").AlignRight();
                    discountTable.Cell().PaddingTop(3).PaddingRight(6)
                        .Text($"{Data.Document.WithholdingTaxAmount:N2}").AlignRight();
                }

                discountTable.Cell().PaddingTop(3).Text("Total do Documento").AlignRight().Bold();
                discountTable.Cell().PaddingTop(3).PaddingBottom(12).PaddingRight(6)
                    .Text($"{Data.Document.TotalFinal:N2}")
                    .AlignRight().Bold();
            });

        AddQrCell(table);

        table.Cell().Height(12);
    }
}