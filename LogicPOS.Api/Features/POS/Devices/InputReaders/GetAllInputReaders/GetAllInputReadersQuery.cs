using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.InputReaders.GetAllInputReaders
{
    public class GetAllInputReadersQuery : IRequest<ErrorOr<IEnumerable<InputReader>>>
    {
    }
}
