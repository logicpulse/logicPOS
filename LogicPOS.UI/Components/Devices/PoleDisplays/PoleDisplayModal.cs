using LogicPOS.Api.Features.PoleDisplays;
using LogicPOS.Api.Features.PoleDisplays.AddPoleDisplay;
using LogicPOS.Api.Features.PoleDisplays.UpdatePoleDisplay;
using LogicPOS.UI.Components.Modals;

namespace LogicPOS.UI.Components.PoleDisplays
{
    public partial class PoleDisplayModal : EntityEditionModal<PoleDisplay>
    {
        public PoleDisplayModal(EntityEditionModalMode modalMode, PoleDisplay entity = null) : base(modalMode, entity)
        {
        }

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtVendorId.Text = _entity.VendorId;
            _txtProductId.Text = _entity.ProductId;
            _txtEndpoint.Text = _entity.EndPoint;
            _txtCOMPort.Text = _entity.COMPort;
            _txtCodeTable.Text = _entity.CodeTable;
            _txtCharsPerLine.Text = _entity.CharactersPerLine.ToString();
            _txtStandByInSeconds.Text = _entity.GoToStandByInSeconds.ToString();
            _txtStandByLine1.Text = _entity.StandByLine1;
            _txtStandByLine2.Text = _entity.StandByLine2;
            _checkDisabled.Active = _entity.IsDeleted;
            _txtNotes.Value.Text = _entity.Notes;
        }

        private UpdatePoleDisplayCommand UpdatePoleDisplayCommand()
        {
            return new UpdatePoleDisplayCommand
            {
                Id = _entity.Id,
                NewDesignation = _txtDesignation.Text,
                NewProductId = _txtProductId.Text,
                NewVendorId = _txtVendorId.Text,
                NewEndPoint = _txtEndpoint.Text,
                NewCodeTable = _txtCodeTable.Text,
                NewCOMPort = _txtCOMPort.Text,
                NewCharactersPerLine = uint.Parse(_txtCharsPerLine.Text),
                NewGoToStandByInSeconds = uint.Parse(_txtStandByInSeconds.Text),
                NewStandByLine1 = _txtStandByLine1.Text,
                NewStandByLine2 = _txtStandByLine2.Text,
                NewNotes = _txtNotes.Value.Text,
                IsDeleted = _checkDisabled.Active
            };
        }

        protected override void UpdateEntity() => ExecuteUpdateCommand(UpdatePoleDisplayCommand());

        private AddPoleDisplayCommand CreateAddCommand()
        {
            return new AddPoleDisplayCommand
            {
                Designation = _txtDesignation.Text,
                ProductId = _txtProductId.Text,
                VendorId = _txtVendorId.Text,
                EndPoint = _txtEndpoint.Text,
                CodeTable = _txtCodeTable.Text,
                COMPort = _txtCOMPort.Text,
                CharactersPerLine = uint.Parse(_txtCharsPerLine.Text),
                GoToStandByInSeconds = uint.Parse(_txtStandByInSeconds.Text),
                StandByLine1 = _txtStandByLine1.Text,
                StandByLine2 = _txtStandByLine2.Text,
                Notes = _txtNotes.Value.Text
            };
        }

        protected override void AddEntity() => ExecuteAddCommand(CreateAddCommand());

    }
}
