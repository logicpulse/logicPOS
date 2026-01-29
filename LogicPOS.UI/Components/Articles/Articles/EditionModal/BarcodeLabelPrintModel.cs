using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Articles.Articles.EditionModal
{
    public class BarcodeLabelPrintModel : ApiEntity, IWithDesignation
    {
        public string Designation { get; set; }
        public string Model { get; set; }

        public readonly static List<BarcodeLabelPrintModel> DefaultModels = new List<BarcodeLabelPrintModel>
        {
            new BarcodeLabelPrintModel { Designation = "Modelo para impressão de Código de barras 100*50", Model = "100x50" },
            new BarcodeLabelPrintModel { Designation = "Modelo para impressão de Código de barras 105*53", Model = "105x53" },
            new BarcodeLabelPrintModel { Designation = "Modelo para impressão de Código de barras 40*30", Model = "40x30" },
            new BarcodeLabelPrintModel { Designation = "Modelo para impressão de Código de barras 45*33", Model = "45x33" }
        };

    }
}
