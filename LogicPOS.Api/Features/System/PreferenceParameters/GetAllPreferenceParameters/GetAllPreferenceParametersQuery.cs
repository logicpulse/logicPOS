using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters
{
    public class GetAllPreferenceParametersQuery : IRequest<ErrorOr<IEnumerable<PreferenceParameter>>>
    {

    }
}
