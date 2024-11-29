using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Customers.HasDocumentsAssociated
{
    public class CustomerHasDocumentsAssociatedQuery : IRequest<ErrorOr<bool>>
    {
        public Guid Id { get; set; }

        public CustomerHasDocumentsAssociatedQuery(Guid id)
        {
            Id = id;
        }
    }
}
