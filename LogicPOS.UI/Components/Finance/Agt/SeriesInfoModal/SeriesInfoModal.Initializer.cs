using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using System;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class SeriesInfoModal : Modal
    {
        private void Initialize()
        {
            InitializeTxtYear();
            InititalizeTxtCode();
            InitializeTxtDocumentType();
            InitializeTxtStatus();
            InitializeTxtCreationDate();
            InitializeTxtFirstDocument();
            InitializeTxtLastDocument();
            InitializeTxtInvoicingMethod();
            InitializeTxtNif();
            InitializeTxtName();
            InitializeTxtJoinigDate();
            InitializeTxtJoiningType();
        }

        private void InitializeTxtJoiningType()
        {
            TxtJoiningType = new TextBox(this,
                                      "Tipo de Adesão",
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: false);

            TxtJoiningType.Entry.Sensitive = false;
        }

        private void InitializeTxtJoinigDate()
        {
            TxtJoiningDate = new TextBox(this,
                                      "Data de Adesão",
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: false);
            TxtJoiningDate.Entry.Sensitive = false;
        }

        private void InitializeTxtName()
        {
            TxtName = new TextBox(this,
                                      "Contribuinte",
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: false);
            TxtName.Entry.Sensitive = false;
        }

        private void InititalizeTxtCode()
        {
            TxtCode = new TextBox(this,
                                       "Código",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtCode.Entry.Sensitive = false;
        }

        private void InitializeTxtYear()
        {
            TxtYear = new TextBox(this,
                                 "Ano Fiscal",
                                 isRequired: false,
                                 isValidatable: false,
                                 includeSelectButton: false,
                                 includeKeyBoardButton: false);
            TxtYear.Entry.Sensitive = false;
        }

        private void InitializeTxtNif()
        {
            TxtNif = new TextBox(this,
                                       "NIF",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtNif.Entry.Sensitive = false;
        }

        private void InitializeTxtInvoicingMethod()
        {
            TxtInvoicingMethod = new TextBox(this,
                                       "Método",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);
            TxtInvoicingMethod.Entry.Sensitive = false;
        }

        private void InitializeTxtLastDocument()
        {
            TxtLastDocument = new TextBox(this,
                                            "Último Documento",
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: false);
            TxtLastDocument.Entry.Sensitive = false;
        }

        private void InitializeTxtFirstDocument()
        {
            TxtFirstDocument = new TextBox(this,
                                      "Primeiro documento",
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: false);

            TxtFirstDocument.Entry.Sensitive = false;
        }

        private void InitializeTxtCreationDate()
        {
            TxtCreationDate = new TextBox(this,
                                               "Data de Criação",
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: false,
                                               includeKeyBoardButton: false);
            TxtCreationDate.Entry.Sensitive = false;
        }

        private void InitializeTxtStatus()
        {
            TxtStatus = new TextBox(this,
                                         "Estado",
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: false);

            TxtStatus.Entry.Sensitive = false;
        }

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new TextBox(this,
                                       "Tipo de Documento",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtDocumentType.Entry.Sensitive = false;
        }
    }
}