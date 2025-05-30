using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Holidays.GetAllHolidays
{
    public class GetAllHolidaysQuery : IRequest<ErrorOr<IEnumerable<Holiday>>>
    {

    }
}
