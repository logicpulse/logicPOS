using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Modals.Common;
using System;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtDocumentInfoModal : Modal
    {
        private void Initialize()
        {
            InitializeTxtRequestId();
            InititalizeTxtSubmissionDate();
            InitializeTxtDocumentNumber();
            InitializeTxtSubmissionErrorCode();
            InitializeTxtSubmissionErrorDescription();
            InitializeTxtHttpStatusCode();
            InitializeTxtValidationResultCode();
            InitializeTxtValidationStatus();
            InitializeTxtValidationErrors();
        }

        private void InititalizeTxtSubmissionDate()
        {
            TxtSubmissionDate = new TextBox(this,
                                       "Data de Submissão",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtSubmissionDate.Entry.Sensitive = false;
        }

        private void InitializeTxtRequestId()
        {
            TxtRequestId = new TextBox(this,
                                 "ID da Submissão",
                                 isRequired: false,
                                 isValidatable: false,
                                 includeSelectButton: false,
                                 includeKeyBoardButton: false);
            TxtRequestId.Entry.Sensitive = false;
        }

        private void InitializeTxtValidationErrors()
        {
            TxtValidationErrors = new TextBox(this,
                                       "Erros de Validação",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);
            TxtValidationErrors.Entry.Sensitive = false;
        }

        private void InitializeTxtValidationStatus()
        {
            TxtValidationStatus = new TextBox(this,
                                       "Estado da Validação",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);
            TxtValidationStatus.Entry.Sensitive = false;
        }

        private void InitializeTxtValidationResultCode()
        {
            TxtValidationResultCode = new TextBox(this,
                                            "Código do Resultado da Validação",
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: false);
            TxtValidationResultCode.Entry.Sensitive = false;
        }

        private void InitializeTxtHttpStatusCode()
        {
            TxtHttpStatusCode = new TextBox(this,
                                      "Código HTTP",
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: false);

            TxtHttpStatusCode.Entry.Sensitive = false;
        }

        private void InitializeTxtSubmissionErrorDescription()
        {
            TxtSubmissionErrorDescription = new TextBox(this,
                                               "Descrição do Erro de Submissão",
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: false,
                                               includeKeyBoardButton: false);
            TxtSubmissionErrorDescription.Entry.Sensitive = false;
        }

        private void InitializeTxtSubmissionErrorCode()
        {
            TxtSubmissionErrorCode = new TextBox(this,
                                         "Código de Erro de Submissão",
                                         isRequired: false,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: false);

            TxtSubmissionErrorCode.Entry.Sensitive = false;
        }

        private void InitializeTxtDocumentNumber()
        {
            TxtDocumentNumber = new TextBox(this,
                                       "Documento",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtDocumentNumber.Entry.Sensitive = false;
        }
    }
}