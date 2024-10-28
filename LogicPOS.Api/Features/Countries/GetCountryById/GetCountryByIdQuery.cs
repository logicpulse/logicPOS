using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Features.Countries.GetCountryById
{
    public class GetCountryByIdQuery : IRequest<ErrorOr<Country>>
    {
        public Guid Id { get; set; }

        public GetCountryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
