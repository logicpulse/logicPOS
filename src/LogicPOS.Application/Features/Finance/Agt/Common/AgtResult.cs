using System.Net;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public struct AgtResult<TResponse>
{
    public TResponse? Value { get; set; }
    public HttpStatusCode? HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasValue => Value != null;
    public string? Request { get; set; }
    public string? Response { get; set; }

    public bool IsSuccessStatusCode =>
        this.HttpStatusCode.HasValue && ((int)HttpStatusCode >= 200) && ((int)HttpStatusCode <= 299);
}