using LogicPOS.Api.Features.DocumentTypes.UpdateDocumentType;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentTypeModal : EntityEditionModal<DocumentType>
    {
        public DocumentTypeModal(EntityEditionModalMode modalMode, DocumentType entity = null) : base(modalMode, entity)
        {
        }

        private UpdateDocumentTypeCommand CreateUpdateCommand()
        {
            return new UpdateDocumentTypeCommand
            {
                Id = _entity.Id,
                PrintCopies = int.Parse(_txtPrintCopies.Text),
                PrintRequestConfirmation = _checkRequestPrintConfirmation.Active,
                PrintOpenDrawer = _checkOpenDrawer.Active
            };
        }

        protected override bool AddEntity() => throw new global::System.NotImplementedException();

        protected override bool UpdateEntity() => ExecuteUpdateCommand(CreateUpdateCommand()).IsError == false;

        protected override void ShowEntityData()
        {
            _txtOrder.Text = _entity.Order.ToString();
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtAcronym.Text = _entity.Acronym;
            _txtPrintCopies.Text = _entity.PrintCopies.ToString();
            _checkOpenDrawer.Active = _entity.PrintOpenDrawer;
            _checkRequestPrintConfirmation.Active = _entity.PrintRequestConfirmation;
            _txtNotes.Value.Text = _entity.Notes;

            _txtOrder.Entry.Sensitive = false;
            _txtCode.Entry.Sensitive = false;
            _txtDesignation.Entry.Sensitive = false;
            _txtAcronym.Entry.Sensitive = false;
            _txtNotes.TextView.Sensitive = false;
        }
    }
}
