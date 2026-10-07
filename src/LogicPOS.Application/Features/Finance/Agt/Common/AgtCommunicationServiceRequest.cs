

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public abstract record AgtCommunicationServiceRequest
{
    [JsonProperty("schemaVersion")] public string SchemaVersion { get; set; } = "2.0";

    [JsonProperty("submissionUUID")]
    public string SubmissionUuid { get; set; } = Guid.NewGuid().ToString().ToLower();

    [JsonProperty("taxRegistrationNumber")]
    public string TaxRegistrationNumber { get; set; } = null!;

    [JsonProperty("submissionTimeStamp")]
    public string SubmissionTimeStamp { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

    [JsonProperty("softwareInfo")] public SoftwareInfo SoftwareInfo { get; set; } = null!;
}