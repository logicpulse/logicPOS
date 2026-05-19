using LogicPOS.Api.Features.Holidays.AddHoliday;
using LogicPOS.Api.Features.Holidays.UpdateHoliday;
using Holiday = LogicPOS.Api.Entities.Holiday;

namespace LogicPOS.UI.Components.Modals
{
    public partial class HolidayModal : EntityEditionModal<Holiday>
    {
        public HolidayModal(EntityEditionModalMode modalMode, Holiday entity = null) : base(modalMode, entity)
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
                Order = uint.Parse(_txtOrder.Text),
                Code = _txtCode.Text,
                Designation = _txtDesignation.Text,
                Description = _txtDescription.Text,
                Day = int.Parse(_txtDay.Text),
                Month = int.Parse(_txtMonth.Text),
                Year = int.Parse(_txtYear.Text),
                Fixed = _entity.Fixed,
                Notes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;
        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;


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
    }
}
