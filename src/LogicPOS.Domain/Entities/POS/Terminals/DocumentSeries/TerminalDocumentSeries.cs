using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_documentfinanceyearserieterminal")]
public class TerminalDocumentSeries : Entity
{
    public Terminal? Terminal { get; set; }
    public Guid TerminalId { get; set; }

    public DocumentSeries? Series { get; set; }
    public Guid SeriesId { get; set; }
}
