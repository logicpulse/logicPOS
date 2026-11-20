using LogicPOS.Application.Features.Finance.Documents.PdfGeneration;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace LogicPOS.Core.FrontOffice.Pdf;

public class CreditNoteModel : DocumentModel
{
    public CreditNoteModel(DocumentPdfData data) : base(data)
    {
    }

    protected override void DesignFinalTable(ColumnDescriptor xcolumn)
    {
        xcolumn.Item().ExtendVertical().AlignBottom().PaddingTop(3).ShowEntire().Table(finalTable =>
        {
            finalTable.ColumnsDefinition(columns => { columns.ConstantColumn(18, Unit.Centimetre); });

            finalTable.Cell().Table(DesignTotalsTable);
            finalTable.Cell().Table(qrTable =>
            {
                qrTable.ColumnsDefinition(col =>
                {
                    col.ConstantColumn(14.5f, Unit.Centimetre);
                    col.ConstantColumn(3.5f, Unit.Centimetre);
                });
                DesignNotesAndQrSection(qrTable);
                if (IsSecondCopy)
                {
                    finalTable.Cell().PaddingTop(16).Text("Assinatura do cliente").FontSize(8);
                    finalTable.Cell().PaddingTop(18).Text("________________________________").FontSize(8);
                }
            });
        });
    }
}