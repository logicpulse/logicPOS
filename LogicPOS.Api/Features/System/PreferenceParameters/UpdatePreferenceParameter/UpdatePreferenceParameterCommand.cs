using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter
{
    public class UpdatePreferenceParameterCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewValue { get; set; }
        public string NewNotes { get; set; }
    }
}
