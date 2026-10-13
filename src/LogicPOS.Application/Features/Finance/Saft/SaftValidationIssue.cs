namespace LogicPOS.Application.Features.Finance.Saft;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
// global:: keeps System.Text.Json from binding to LogicPOS.Application.Features.System.
[global::System.Text.Json.Serialization.JsonConverter(typeof(global::System.Text.Json.Serialization.JsonStringEnumConverter))]
public enum SaftValidationSeverity
{
    Error = 0,
    Warning = 1
}

public sealed record SaftValidationIssue(
    SaftValidationSeverity Severity,
    string Code,
    string Message,
    string? Path = null);
