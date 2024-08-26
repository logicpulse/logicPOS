using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Holidays.UpdateHoliday
{
    public class UpdateHolidayCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public string NewDescription {  get; set; }
        public int NewDay {  get; set; }
        public int NewMonth { get; set; }
        public int NewYear {  get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
