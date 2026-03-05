using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Holidays.AddHoliday
{
    public class AddHolidayCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public string Description {  get; set; }
        public int Day {  get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Notes { get; set; }
    }
}
