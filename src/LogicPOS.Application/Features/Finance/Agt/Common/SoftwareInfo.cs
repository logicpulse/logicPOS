

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record SoftwareInfo
{
    [JsonProperty("softwareInfoDetail")]
    public SoftwareInfoDetail SoftwareInfoDetail { get; set; } = null!;

    [JsonProperty("jwsSoftwareSignature")]
    public string JwsSoftwareSignature { get; set; } = null!;
}