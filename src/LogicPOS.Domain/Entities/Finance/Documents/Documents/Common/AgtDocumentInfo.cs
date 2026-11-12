using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.Entities.Finance.Documents.Documents.Common;

[ComplexType]
public sealed class AgtDocumentInfo
{
    private const string ValidStatus = "V";
    private const string ValidResultCode = "0";

    public DateTime? SubmissionDate { get; set; }
    public string? RequestId { get; set; }
    public string? SubmissionErrorCode { get; set; }
    public string? SubmissionErrorDescription { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ValidationResultCode { get; set; }
    public string? ValidationStatus { get; set; }
    public string? ValidationErrors { get; set; }
    public string? SubmissionUuid { get; set; }
    public string? RejectedDocumentNumber { get; set; }

    /// <summary>True when the document still needs to be registered with AGT (auto-send or retry).</summary>
    public bool SubmissionPending { get; set; }

    /// <summary>True when the document was submitted and still needs a validation-status sync from AGT.</summary>
    public bool ValidationSyncPending { get; set; }

    /// <summary>Last time the API contacted AGT for this document (submit or status poll).</summary>
    public DateTime? LastSyncAt { get; set; }

    /// <summary>Number of register attempts (successful or not).</summary>
    public int SubmissionAttempts { get; set; }

    public bool IsValid() => ValidationStatus == ValidStatus;

    public bool IsSubmitted() => string.IsNullOrWhiteSpace(RequestId) == false;

    public void MarkSubmissionPending()
    {
        SubmissionPending = true;
    }

    public void MarkSubmissionAttemptFailed(int? httpStatusCode, string? errorCode, string? errorDescription)
    {
        SubmissionAttempts++;
        LastSyncAt = DateTime.Now;
        HttpStatusCode = httpStatusCode;
        SubmissionErrorCode = errorCode;
        SubmissionErrorDescription = errorDescription;
        SubmissionPending = true;
        ValidationSyncPending = false;
    }

    public void MarkSubmitted(
        string requestId,
        string? submissionUuid,
        int? httpStatusCode,
        string? submissionErrorCode,
        string? submissionErrorDescription,
        string? rejectedDocumentNumber)
    {
        SubmissionAttempts++;
        LastSyncAt = DateTime.Now;
        SubmissionDate = DateTime.Now;
        RequestId = requestId;
        SubmissionUuid = submissionUuid;
        HttpStatusCode = httpStatusCode;
        SubmissionErrorCode = submissionErrorCode;
        SubmissionErrorDescription = submissionErrorDescription;
        RejectedDocumentNumber = rejectedDocumentNumber;
        SubmissionPending = false;
        ValidationSyncPending = true;
        ValidationStatus = null;
        ValidationResultCode = null;
        ValidationErrors = null;
    }

    public void MarkValidationSynced()
    {
        LastSyncAt = DateTime.Now;
        if (IsValid())
        {
            ValidationSyncPending = false;
        }
    }

    /// <summary>Updates <see cref="LastSyncAt"/> without clearing pending (throttle / backoff after AGT errors).</summary>
    public void TouchLastSync()
    {
        LastSyncAt = DateTime.Now;
    }

    public void MarkAsValid()
    {
        ValidationStatus = ValidStatus;
        ValidationResultCode = ValidResultCode;
        ValidationErrors = null;
        ValidationSyncPending = false;
        LastSyncAt = DateTime.Now;
    }
}
