using LogicPOS.Api.Features.DocumentTypes.UpdateDocumentType;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentTypeModal : EntityEditionModal<DocumentType>
    {
        public DocumentTypeModal(EntityEditionModalMode modalMode, DocumentType entity = null) : base(EntityEditionModalMode.View, entity)
        {
        }

        private UpdateDocumentTypeCommand CreateUpdateCommand()
        {
            return new UpdateDocumentTypeCommand
            {
                Id = _entity.Id,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewCode = _txtCode.Text,
                NewPrintCopies = int.Parse(_txtPrintCopies.Text),
                PrintRequestConfirmation = _checkRequestPrintConfirmation.Active,
                PrintOpenDrawer = _checkOpenDrawer.Active,
                NewNotes = _txtNotes.Value.Text
            };
        }


        protected override bool AddEntity() => throw new NotImplementedException();
        
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
        }

    }
}
