using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Terminals.GetAllTerminals
{
    public class GetAllTerminalsQuery : IRequest<ErrorOr<IEnumerable<Terminal>>>
    {
    }
}
