using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.GetContributor
{
    public record GetContributorResponse
    {
        [JsonProperty("ObterContribuinte")]
        public ObterContribuinteWrapper ObterContribuinte { get; init; } = new();
    }

    public record ObterContribuinteWrapper
    {
        [JsonProperty("mensagem")]
        public string Mensagem { get; init; } = string.Empty;

        [JsonProperty("contribuinte")]
        public Contributor Contributor { get; init; } = new();
    }

    public record Contributor
    {
        [JsonProperty("tipoContribuinte")] public string TipoContribuinte { get; init; } = string.Empty;
        [JsonProperty("numeroNIF")] public string NumeroNif { get; init; } = string.Empty;
        [JsonProperty("nome")] public string? Nome { get; init; }
        [JsonProperty("estadoActividade")] public string? EstadoActividade { get; init; }
        [JsonProperty("estadoContribuinte")] public string? EstadoContribuinte { get; init; }
        [JsonProperty("regimeIva")] public string? RegimeIva { get; init; }
        [JsonProperty("indicadorNaoResidente")] public string? IndicadorNaoResidente { get; init; }
    }
}
