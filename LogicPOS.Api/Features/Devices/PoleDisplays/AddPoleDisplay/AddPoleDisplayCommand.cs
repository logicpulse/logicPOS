using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PoleDisplays.AddPoleDisplay
{
    public class AddPoleDisplayCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string VendorId { get; set; } 
        public string ProductId { get; set; } 
        public string EndPoint { get; set; }
        public string CodeTable { get; set; } 
        public string COMPort { get; set; } 
        public uint CharactersPerLine { get; set; }
        public uint GoToStandByInSeconds { get; set; }
        public string StandByLine1 { get; set; }
        public string StandByLine2 { get; set; }
        public string Notes { get; set; }
    }
}
