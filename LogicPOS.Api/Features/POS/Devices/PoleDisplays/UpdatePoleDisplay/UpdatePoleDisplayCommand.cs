using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PoleDisplays.UpdatePoleDisplay
{
    public class UpdatePoleDisplayCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewVendorId { get; set; } 
        public string NewProductId { get; set; } 
        public string NewEndPoint { get; set; } 
        public string NewCodeTable { get; set; } 
        public string NewCOMPort { get; set; } 
        public uint NewCharactersPerLine { get; set; }
        public uint NewGoToStandByInSeconds { get; set; }
        public string NewStandByLine1 { get; set; }
        public string NewStandByLine2 { get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
