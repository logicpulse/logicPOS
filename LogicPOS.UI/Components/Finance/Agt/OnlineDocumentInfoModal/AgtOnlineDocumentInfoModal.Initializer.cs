using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public partial class AgtOnlineDocumentInfoModal
    {
        private void Initialize()
        {
            InitializeTxtDocumentType();
            InititalizeTxtDate();
            InitializeTxtDocumentNumber();
            InitializeTxtStatus();
            InitializeTxtCustomerNif();
            InitializeTxtTaxPayable();
            InitializeTxtNetTotal();
            InitializeTxtGrossTotal();
        }

        private void InititalizeTxtDate()
        {
            TxtDate = new TextBox(this,
                                       "Data",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtDate.Entry.Sensitive = false;
        }

        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new TextBox(this,
                                 "Tipo",
                                 isRequired: false,
                                 isValidatable: false,
                                 includeSelectButton: false,
                                 includeKeyBoardButton: false);
            TxtDocumentType.Entry.Sensitive = false;
        }

        private void InitializeTxtGrossTotal()
        {
            TxtGrossTotal = new TextBox(this,
                                       "Total Ilíquido",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);
            TxtGrossTotal.Entry.Sensitive = false;
        }

        private void InitializeTxtNetTotal()
        {
            TxtNetTotal = new TextBox(this,
                                            "Total Líquido",
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: false,
                                            includeKeyBoardButton: false);
            TxtNetTotal.Entry.Sensitive = false;
        }

        private void InitializeTxtTaxPayable()
        {
            TxtTaxPayable = new TextBox(this,
                                      "Total Imposto",
                                      isRequired: false,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: false);

            TxtTaxPayable.Entry.Sensitive = false;
        }

        private void InitializeTxtCustomerNif()
        {
            TxtCustomerNif = new TextBox(this,
                                               "NIF do Cliente",
                                               isRequired: false,
                                               isValidatable: false,
                                               includeSelectButton: false,
                                               includeKeyBoardButton: false);
            TxtCustomerNif.Entry.Sensitive = false;
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

        private void InitializeTxtDocumentNumber()
        {
            TxtDocumentNumber = new TextBox(this,
                                       "Nº Documento",
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtDocumentNumber.Entry.Sensitive = false;
        }
    }
}