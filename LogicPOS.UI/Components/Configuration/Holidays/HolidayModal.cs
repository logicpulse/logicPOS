using DevExpress.Schedule;
using LogicPOS.Api.Features.Holidays.AddHoliday;
using LogicPOS.Api.Features.Holidays.UpdateHoliday;
using Holiday = LogicPOS.Api.Entities.Holiday;

namespace LogicPOS.UI.Components.Modals
{
    public partial class HolidayModal: EntityModal<Holiday>
    {
        public HolidayModal(EntityModalMode modalMode, Holiday entity = null) : base(modalMode, entity)
        {
        }

        private AddHolidayCommand CreateAddCommand()
        {
            return new AddHolidayCommand
            {
                Designation = _txtDesignation.Text,
                Description = _txtDescription.Text,
                Day = int.Parse(_txtDay.Text),
                Month = int.Parse(_txtMonth.Text),
                Year = int.Parse(_txtYear.Text),
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateHolidayCommand CreateUpdateCommand()
        {
            return new UpdateHolidayCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewDescription = _txtDescription.Text,
                NewDay = int.Parse(_txtDay.Text),
                NewMonth = int.Parse(_txtMonth.Text),
                NewYear = int.Parse(_txtYear.Text),
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtDescription.Text = _entity.Description;
            _txtDay.Text = _entity.Day.ToString();
            _txtMonth.Text = _entity.Month.ToString();
            _txtYear.Text = _entity.Year.ToString();
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        protected override void UpdateEntity()
        {
            var command = CreateUpdateCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }
    }
}
