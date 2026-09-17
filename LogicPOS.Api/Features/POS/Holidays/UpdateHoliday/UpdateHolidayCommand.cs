using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Holidays.UpdateHoliday
{
    public class UpdateHolidayCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Description {  get; set; }
        public int Day {  get; set; }
        public int Month { get; set; }
        public int Year {  get; set; }
        public string Notes { get; set; }
        public bool Fixed { get; set; }
        public bool IsDeleted { get; set; }
    }
}
