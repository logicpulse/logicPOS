using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.PreferenceParameters.UpdatePreferenceParameter
{
    public class UpdatePreferenceParameterCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Notes { get; set; }
    }
}
