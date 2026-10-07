using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Application.Features.Finance.At;

public record AtSoapResult
{
    private AtSoapError? Error { get; set; }
    public bool IsSuccess { get; private set; }
    public string? Response { get; private init; }
    public string? ReturnCode { get; set; } 
    public string? ReturnMessage { get; set; }
    public string? DocumentNumber { get; set; }
    public SeriesInfo Series { get; } = new();
    public string? DocumentCodeId { get; set; }
    public string? PostData { get; set; }

    public ATAudit GetSeriesAtAudit(string seriesAcronym)
    {
        int.TryParse(ReturnCode, out var returnCode);
        return new ATAudit
        {
            Date = DateTime.Now,
            Type = AtAuditType.DocumentSeries,
            PostData = PostData,
            ReturnCode = returnCode,
            ReturnMessage = ReturnMessage,
            ReturnRaw = Response,
            ATDocCodeID = Series?.ValidationCode,
            DocumentNumber = seriesAcronym
        };
    }
    
    public ATAudit GetTransportDocumentAtAudit(Guid documentId, string documentNumber)
    {
        int.TryParse(ReturnCode, out var returnCode);
        return new ATAudit
        {
            Date = DateTime.Now,
            Type = AtAuditType.DocumentWayBill,
            PostData = PostData,
            ReturnCode = returnCode,
            ReturnMessage = ReturnMessage ?? ToString(),
            ReturnRaw = Response,
            ATDocCodeID = DocumentCodeId,
            DocumentNumber = documentNumber,
            DocumentId = documentId
        };
    }

    public void MakeFailure()
    {
        IsSuccess = false;
        Error = new AtSoapError($"{ReturnCode}:{ReturnMessage}");
    }

    public void RecoverDuplicateTransportDocument(string documentCodeId)
    {
        DocumentCodeId = documentCodeId;
        IsSuccess = true;
        Error = null;
    }
    
    public override string ToString() => IsSuccess ? $"{ReturnCode}:{ReturnMessage}" : Error.ToString()!;
    public static AtSoapResult Failure(AtSoapError error, string response) => new() { Error = error, IsSuccess = false, Response = response};
    public static AtSoapResult Success(string response) => new() { Response = response, IsSuccess = true };
    
}