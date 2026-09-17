using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Holidays.DeleteHoliday
{
    public class DeleteHolidayCommand : DeleteCommand
    {
        public DeleteHolidayCommand(Guid id) : base(id)
        {
        }
    }
}
