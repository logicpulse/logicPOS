

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record SoftwareInfoDetail
{
    [JsonProperty("productId")] public string ProductId { get; set; } = null!;

    [JsonProperty("productVersion")] public string ProductVersion { get; set; } = null!;

    [JsonProperty("softwareValidationNumber")]
    public string SoftwareValidationNumber { get; set; } = null!;
}