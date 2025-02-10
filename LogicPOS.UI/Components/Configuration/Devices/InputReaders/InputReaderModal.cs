
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.InputReaders.AddInputReader;
using LogicPOS.Api.Features.InputReaders.UpdateInputReader;


namespace LogicPOS.UI.Components.Modals
{
    public partial class InputReaderModal : EntityEditionModal<InputReader>
    {
        public InputReaderModal(EntityEditionModalMode modalMode, InputReader inputReader = null) : base(modalMode, inputReader)
        {
        }

        private AddInputReaderCommand CreateAddCommand()
        {
            return new AddInputReaderCommand
            {
                Designation = _txtDesignation.Text,
                ReaderSizes = _txtReaderSizes.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        private UpdateInputReaderCommand CreateUpdateCommand()
        {
            return new UpdateInputReaderCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewDesignation = _txtDesignation.Text,
                NewReaderSizes = _txtReaderSizes.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());
 
        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtReaderSizes.Text = _entity.ReaderSizes;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text= _entity.Notes;
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand());

    }
}