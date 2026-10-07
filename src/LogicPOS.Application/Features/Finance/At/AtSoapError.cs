namespace LogicPOS.Application.Features.Finance.At;

public readonly struct AtSoapError
{
    public AtSoapError(string message) => Message = message;
    public string? FaultCode { get; init; }
    public string? FaultString { get; init; }
    public string? TransactionId { get; init; }
    public DateTime? Timestamp { get; init; }
    public string? Message { get; private init; }
    public override string ToString() => Message ?? $"{FaultCode}: {FaultString}";
}