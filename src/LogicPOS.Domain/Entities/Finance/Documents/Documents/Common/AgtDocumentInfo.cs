using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Common;

[ComplexType]
public sealed class AgtDocumentInfo
{
    private const string ValidStatus = "V"; 
    private const string ValidResultCode = "0";
    public DateTime? SubmissionDate { get; set; } = DateTime.Now;
    public string? RequestId { get; set; }
    public string? SubmissionErrorCode { get; set; }
    public string? SubmissionErrorDescription { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ValidationResultCode { get; set; }
    public string? ValidationStatus { get; set; }
    public string? ValidationErrors { get; set; }
    public string? SubmissionUuid { get; set; }
    public string? RejectedDocumentNumber { get; set; }

    public bool IsValid() => ValidationStatus == ValidStatus;

    public void MarkAsValid()
    {
        ValidationStatus = ValidStatus;
        ValidationResultCode = ValidResultCode;
    }
}
