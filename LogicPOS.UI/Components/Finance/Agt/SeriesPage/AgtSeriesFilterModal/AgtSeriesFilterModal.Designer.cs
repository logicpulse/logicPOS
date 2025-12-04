using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.Customers.Customers.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.PaymentConditions;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtSeriesFilterModal
    {
        private void InitializeTextBoxes()
        {
            InitializeTxtDocumentType();
            InitializeTxtEstablishmentNumber();
            InitializeTxtYear();
            InitializeTxtCode();
            InitializeTxtStatus();
        }
        private void InitializeTxtDocumentType()
        {
            TxtDocumentType = new TextBox(this,
                                           GeneralUtils.GetResourceByName("global_documentfinanceseries_documenttype"),
                                           isRequired: false,
                                           isValidatable: true,
                                           regex:RegularExpressions.Acronym2Chars,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

            TxtDocumentType.Entry.Changed += TxtDocumentType_Entry_Changed;
            TxtDocumentType.SelectEntityClicked += BtnSelectDocumentType_Clicked;
        }
       
        private void InitializeTxtEstablishmentNumber()
        {
            TxtEstablishmentNumber = new TextBox(this,
                                           "Nº de Estabelecimento",
                                           isRequired: false,
                                           isValidatable: true,
                                           regex: RegularExpressions.Integer,
                                           includeSelectButton: true,
                                           includeKeyBoardButton: true);

        }

        private void InitializeTxtYear()
        {
            TxtYear= new TextBox(this,
                                 GeneralUtils.GetResourceByName("global_year"),
                                 isRequired: false,
                                 isValidatable: true,
                                 regex: RegularExpressions.Integer,
                                 includeSelectButton: true,
                                 includeKeyBoardButton: true);

        }

        private void InitializeTxtCode()
        {
            TxtCode = new TextBox(this,
                                 GeneralUtils.GetResourceByName("global_article_code"),
                                 isRequired: false,
                                 isValidatable: true,
                                 regex: RegularExpressions.AlfaNumeric,
                                 includeSelectButton: true,
                                 includeKeyBoardButton: true);

        }

        private void InitializeTxtStatus()
        {
            TxtStatus= new TextBox(this,
                                 "Estado",
                                 isRequired: false,
                                 isValidatable: true,
                                 regex: RegularExpressions.AlfaAcronym1Char,
                                 includeSelectButton: true,
                                 includeKeyBoardButton: true);

        }
       


    }
}
