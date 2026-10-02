using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.FrontOffice.Pdf;

public class InformativeModel : DocumentModel
{
    public InformativeModel(DocumentPdfData data) : base(data)
    {
    }

    protected override void DesignFinalTable(ColumnDescriptor xcolumn)
    {
        xcolumn.Item().ExtendVertical().AlignBottom().PaddingTop(3).ShowEntire().Table(finalTable =>
        {
            finalTable.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(18, Unit.Centimetre);
            });

            finalTable.Cell().Table(DesignTotalsTable);

            finalTable.Cell().Table(qrTable =>
            {
                qrTable.ColumnsDefinition(col =>
                {
                    col.ConstantColumn(14.5f, Unit.Centimetre);
                    col.ConstantColumn(3.5f, Unit.Centimetre);
                });

                DesignNotesAndQrSection(qrTable);
            });

            finalTable.Cell().PaddingTop(3).Text("Este documento não serve de fatura").AlignCenter().FontSize(7);
        });
    }
}