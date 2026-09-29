namespace LogicPOS.Api.Features.Finance.At.RegisterSeries
{
    public struct AtSeriesInfo
    {
        public string ValidationCode { get; set; }
        public string Type { get; set; }
        public string DocumentClass { get; set; }
        public string RangeStart { get; set; }
        public string RegisterDate { get; set; }
        public string Status { get; set; }
        public string StatusDate { get; set; }
        public string Issuer { get; set; }
        public string GetNotes()
        {
            if (string.IsNullOrWhiteSpace(ValidationCode))
            {
                return null;
            }

            string notes = $"ATDocCodeValidacaoSerie: {ValidationCode}\n";
            notes += $"tipoSerie: {Type}\n";
            notes += $"classeDoc: {DocumentClass}\n";
            notes += $"numInicialSeq: {RangeStart}\n";
            notes += $"dataRegisto: {RegisterDate}\n";
            notes += $"estado: {Status}\n";
            notes += $"dataEstado: {StatusDate}\n";
            notes += $"nifComunicou: {Issuer}";

            return notes;
        }
    }
}
