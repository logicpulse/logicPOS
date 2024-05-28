using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Exceptions;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using logicpos.Extensions;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Finance.Utility;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PaymentDialog
    {
        //Commmon Button Event
        private void buttonCheck_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        private void buttonMB_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        private void buttonCredit_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        /* IN009142 */
        private void buttonDebitCard_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        private void buttonVisa_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        private void buttonCurrentAccount_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        private void buttonCustomerCard_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

        //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
        private decimal newValuePrice = 0.00m;

        protected override void OnResponse(ResponseType pResponse)
        {
            try
            {
                object resultObject;

                //ResponseTypeCancel
                if (pResponse == ResponseType.Cancel || pResponse == ResponseType.DeleteEvent)
                {
                    this.Destroy();
                }
                //ResponseTypeOk
                else if (pResponse == ResponseType.Ok)
                {
                    //TK016249 - Impressoras - Diferenciação entre Tipos
                    PrintingSettings.ThermalPrinter.UsingThermalPrinter = true;
                    //SaveOrUpdateCustomer Before use _selectedCustomer (Can be null)
                    resultObject = logicpos.Utils.SaveOrUpdateCustomer(
                        this,
                        Customer,
                        _entryBoxSelectCustomerName.EntryValidation.Text,
                        _entryBoxCustomerAddress.EntryValidation.Text,
                        _entryBoxCustomerLocality.EntryValidation.Text,
                        _entryBoxCustomerZipCode.EntryValidation.Text,
                        _entryBoxCustomerCity.EntryValidation.Text,
                        null,// Phone : Used only in PosDocumentFinanceDialog
                        null,// Email : Used only in PosDocumentFinanceDialog 
                        Country,
                        _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text,
                        _entryBoxSelectCustomerCardNumber.EntryValidation.Text,
                        LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxCustomerDiscount.EntryValidation.Text),
                        _entryBoxCustomerNotes.EntryValidation.Text
                     );

                    if (resultObject.GetType() == typeof(erp_customer))
                    {
                        Customer = (erp_customer)resultObject;

                        //Prevent Default Customer Entity and Hidden Customer (Only with Name Filled) to Process CC Documents
                        if (
                            PaymentMethod != null && PaymentMethod.Token == "CURRENT_ACCOUNT" &&
                            (
                                Customer.Oid == InvoiceSettings.FinalConsumerId ||
                                _entryBoxSelectCustomerName.EntryValidation.Text == string.Empty ||
                                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty
                            )
                        )
                        {
                            logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_cant_create_cc_document_with_default_entity"));
                            //Prevent Parent Dialog Payments from Close
                            this.Run();
                        }
                        //Proceed with normal ProcessFinanceDocument
                        else
                        {
                            //Get Document Type to Emmit, based on Payment Mode
                            _processDocumentType = (PaymentMethod.Token == "CURRENT_ACCOUNT")
                                ? InvoiceSettings.XpoOidDocumentFinanceTypeInvoice
                                : DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice;

                            ArticleBag processArticleBag;

                            if (ArticleBagPartialPayment == null)
                            {
                                processArticleBag = ArticleBagFullPayment;
                            }
                            else
                            {
                                processArticleBag = ArticleBagPartialPayment;
                            }

                            //Default UnAssigned Value
                            ResponseType responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice = ResponseType.None;
                            //Get response for user confirmation to emmit Invoice-Payment before Extra Protections, we must have user respose before enter in "Extra Protections" above
                            if (
                                    _processDocumentType != InvoiceSettings.XpoOidDocumentFinanceTypeInvoice &&
                                    (processArticleBag.TotalFinal > InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid) ||
                                    processArticleBag.GetClassTotals("S") > InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid))
                                )
                            {
                                responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice = logicpos.Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceed(_sourceWindow, ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.PaymentsDialog, processArticleBag.TotalFinal, InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid), processArticleBag.GetClassTotals("S"), InvoiceSettings.GetSimplifiedInvoiceMaxServices(XPOSettings.ConfigurationSystemCountry.Oid));
                                //Override Back processDocumentType if Exceed FS Max Total
                                if (responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice == ResponseType.Yes)
                                {
                                    _processDocumentType = DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment;
                                }
                            }

                            //Start Extra Protections

                            //Protection to prevent Exceed FinanceRuleSimplifiedInvoiceMaxValue
                            if (
                                    (
                                        processArticleBag.TotalFinal > InvoiceSettings.GetSimplifiedInvoiceMaxItems(XPOSettings.ConfigurationSystemCountry.Oid) ||
                                        processArticleBag.GetClassTotals("S") > InvoiceSettings.GetSimplifiedInvoiceMaxServices(XPOSettings.ConfigurationSystemCountry.Oid)
                                    ) &&
                                    responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice == ResponseType.No
                                )
                            {
                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            //Check if TotalFinal is above 1000 and request fill all customer details
                            else if (
                                (
                                    _processDocumentType == InvoiceSettings.XpoOidDocumentFinanceTypeInvoice ||
                                    _processDocumentType == DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                                    _processDocumentType == DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice
                                ) &&
                                DocumentProcessingUtils.IsInValidFinanceDocumentCustomer(
                                    processArticleBag.TotalFinal,
                                    _entryBoxSelectCustomerName.EntryValidation.Text,
                                    _entryBoxCustomerAddress.EntryValidation.Text,
                                    _entryBoxCustomerZipCode.EntryValidation.Text,
                                    _entryBoxCustomerCity.EntryValidation.Text,
                                    _entryBoxSelectCustomerCountry.EntryValidation.Text,
                                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text
                                )
                            )
                            {
                                logicpos.Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceedForFinalConsumer(this, processArticleBag.TotalFinal, GeneralSettings.GetRequiredCustomerDetailsAboveValue(XPOSettings.ConfigurationSystemCountry.Oid));

                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            //Protection to prevent Exceed Customer CardCredit
                            else if (
                                PaymentMethod.Token == "CUSTOMER_CARD" &&
                                (
                                    processArticleBag.TotalFinal > Customer.CardCredit
                                )
                            )
                            {
                                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_value_exceed_customer_card_credit"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(Customer.CardCredit, XPOSettings.ConfigurationSystemCurrency.Acronym), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(processArticleBag.TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym)));

                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            //Protection to Prevent Recharge Customer Card with Invalid User (User without Card or FinalConsumer...)
                            //Check if Article Bag Full|Partial has Recharge Article and Valid customer Card
                            else if (!DocumentProcessingUtils.IsCustomerCardValidForArticleBag(processArticleBag, Customer))
                            {
                                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_invalid_customer_card_detected"));

                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            else
                            {
                                //Prepare ProcessFinanceDocumentParameter : Shared for PartialPayment and FullPayment
                                ProcessFinanceDocumentParameter = new DocumentProcessingParameters(
                                    _processDocumentType, processArticleBag
                                )
                                {
                                    PaymentMethod = PaymentMethod.Oid,
                                    PaymentCondition = POSSettings.XpoOidConfigurationPaymentConditionDefaultInvoicePaymentCondition,
                                    Customer = Customer.Oid,
                                    TotalDelivery = TotalDelivery,
                                    TotalChange = TotalChange
                                };

                                //Get Latest DocumentConference Document if Exists, and assign if (REMOVED Total Equality, Request from Carlos)
                                fin_documentfinancemaster conferenceDocument = DocumentProcessingUtils.GetOrderMainLastDocumentConference(false);
                                if (
                                    conferenceDocument != null
                                /*&& (conferenceDocument.TotalFinal.Equals(processArticleBag.TotalFinal) && conferenceDocument.DocumentDetail.Count.Equals(processArticleBag.Count))*/
                                )
                                {
                                    ProcessFinanceDocumentParameter.DocumentParent = conferenceDocument.Oid;
                                    ProcessFinanceDocumentParameter.OrderReferences = new List<fin_documentfinancemaster>
                                    {
                                        conferenceDocument
                                    };
                                }

                                // PreventPersistFinanceDocument : Used in SplitPayments, to get ProcessFinanceDocumentParameter and Details without PreventPersistFinanceDocument
                                if (!_skipPersistFinanceDocument)
                                {
                                    fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(this, ProcessFinanceDocumentParameter);
                                    //If Errors Occurs, return null Document, Keep Running until user cancel or a Valid Document is Returned
                                    if (resultDocument == null)
                                    {
                                        this.Run();
                                    }
                                    else
                                    {
                                        //Update Display
                                        if (GlobalApp.UsbDisplay != null) GlobalApp.UsbDisplay.ShowPayment(PaymentMethod.Designation, TotalDelivery, TotalChange);
                                    }
                                }
                            }
                        }
                    }
                    //If error in Save or Update
                    else if (resultObject.GetType() == typeof(ConstraintViolationException))
                    {
                        Exception ex = (Exception)resultObject;
                        ResponseType response = logicpos.Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_exception_error"), ex.InnerException.Message);
                        //Prevent Parent Dialog Payments from Close
                        this.Run();
                    }
                }
                //ResponseTypeFullPayment
                else if (pResponse == _responseTypeFullPayment)
                {
                    //Update UI
                    UpdateUIWhenAlternateFullToPartialPayment(false, false);

                    //Clean _articleBagPartialPayment, this Enable Normal mode
                    ArticleBagPartialPayment = null;

                    //Prevent Parent Dialog Payments from Close 
                    this.Run();
                }
                //ResponseTypePartialPayment
                else if (pResponse == _responseTypePartialPayment)
                {
                    PartialPayment();
                    //Prevent Parent Dialog Payments from Close 
                    this.Run();
                }
                //ResponseTypeClearCustomer
                else if (pResponse == _responseTypeClearCustomer)
                {
                    //Clear Customer Details
                    ClearCustomer();
                    if (PaymentMethod != null) PaymentMethod = null;
                    if (SelectedPaymentMethodButton != null) SelectedPaymentMethodButton.Sensitive = true;

                    //Prevent Parent Dialog Payments from Close 
                    this.Run();
                }
                else if (pResponse == _responseTypeCurrentAccount)
                {
                    _buttonCurrentAccount.Token = "CURRENT_ACCOUNT";
                    _buttonCurrentAccount.CurrentButtonOid = InvoiceSettings.XpoOidConfigurationPaymentMethodCurrentAccount;
                    //Prevent Default Customer Entity and Hidden Customer (Only with Name Filled) to Process CC Documents
                    if (
                        (Customer != null && (
                            Customer.Oid == InvoiceSettings.FinalConsumerId ||
                            _entryBoxSelectCustomerName.EntryValidation.Text == string.Empty ||
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty)
                        )
                    )
                    {
                        logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_cant_create_cc_document_with_default_entity"));
                        //Prevent Parent Dialog Payments from Close
                        this.Run();
                    }
                    else if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty || _entryBoxSelectCustomerName.EntryValidation.Text == string.Empty)
                    {
                        logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_cant_create_cc_document_with_default_entity"));
                        //Prevent Parent Dialog Payments from Close
                        this.Run();
                    }
                    else
                    {
                        _entryBoxSelectCustomerName.EntryValidation.Sensitive = false;
                        _entryBoxCustomerAddress.EntryValidation.Sensitive = false;
                        _entryBoxCustomerZipCode.EntryValidation.Sensitive = false;
                        _entryBoxCustomerCity.EntryValidation.Sensitive = false;
                        _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = false;
                        _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = false;
                        _entryBoxCustomerLocality.EntryValidation.Sensitive = false;
                        buttonCurrentAccount_Clicked((TouchButtonBase)_buttonCurrentAccount, null);
                        //Keep Running
                        this.Run();
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                string errorMessage;
                switch (ex.Message)
                {
                    case "ERROR_MISSING_SERIE":
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document_missing_series"));
                        break;
                    default:
                        errorMessage = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_error_creating_financial_document"), ex.Message);
                        break;
                }
                logicpos.Utils.ShowMessageTouch(
                    _sourceWindow,
                    DialogFlags.Modal,
                    new Size(600, 400),
                    MessageType.Error,
                    ButtonsType.Close,
                    CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_error"),
                    errorMessage
                );

                this.Run();
            }
        }

        private void buttonMoney_Clicked(object sender, EventArgs e)
        {
            //Settings
            int decimalRoundTo = LogicPOS.Settings.CultureSettings.DecimalRoundTo;

            //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
            decimal _totalOrder = (ArticleBagPartialPayment == null) ? ArticleBagFullPayment.TotalFinal : ArticleBagPartialPayment.TotalFinal;

            MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(this, _totalOrder);
            if (result.Response == ResponseType.Ok)
            {
                TotalDelivery = result.Value;
                //Round currentOrderMain.GlobalTotalFinal, else we can have values like _totalChange -0.000000000000069
                TotalChange = Math.Round(TotalDelivery, decimalRoundTo) - Math.Round(_totalOrder, decimalRoundTo);
                AssignPaymentMethod(sender);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //UI Events

        private void _entryBoxSelectCustomerName_Changed(object sender, EventArgs e)
        {
            if (_enableGetCustomerDetails)
            {
                UpdateCustomerAddressAndFiscalNumberRequireFields();
                Validate();
            }
        }

        private void _entryBoxCustomerDiscount_Changed(object sender, EventArgs e)
        {
            //Get Current ArticleBag
            ArticleBag articleBag = (_partialPaymentEnabled) ? ArticleBagPartialPayment : ArticleBagFullPayment;
            if (articleBag != null)
            {
                //Required to Update articleBag.
                DiscountGlobal = (Customer != null && Customer.Discount > 0) ? Customer.Discount : 0;
                articleBag.DiscountGlobal = DiscountGlobal;
                articleBag.UpdateTotals();
                //Update UI
                UpdateUIWhenAlternateFullToPartialPayment(_partialPaymentEnabled, false);
                //Require to Update _totalDelivery when Discount is Changed, if no Money
                //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
                if (PaymentMethod != null && PaymentMethod.Token != "MONEY")
                {
                    TotalDelivery = (ArticleBagPartialPayment == null) ? ArticleBagFullPayment.TotalFinal : ArticleBagPartialPayment.TotalFinal;
                    if (_labelDeliveryValue.Text != LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym)) _labelDeliveryValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym);
                }
                //Update Change Value
                UpdateChangeValue();
                //Required to Change Color Enable/Disable if TotalChange is Negativ
                Validate();
            }
        }

        private void _entryBoxSelectCustomerFiscalNumber_Changed(object sender, EventArgs e)
        {
            if (_enableGetCustomerDetails)
            {
                UpdateCustomerAddressAndFiscalNumberRequireFields();

                if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated)
                {
                    bool isValidFiscalNumber = FiscalNumberUtils.IsValidFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2);
                    //Get Customer from Fiscal Number
                    if (isValidFiscalNumber)
                    {
                        //Replaced without FiscalNumber.ExtractFiscalNumber Method else we can get From FiscanNumber when dont have Country Prefix
                        //GetCustomerDetails("FiscalNumber", FiscalNumber.ExtractFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2));
                        GetCustomerDetails("FiscalNumber", _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    }
                    else
                    {
                        if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty) _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = false;
                    }
                }
                Validate();
            }
        }

        private void _entryBoxCountry_EntryValidation_FocusGrabbed(object sender, EventArgs e)
        {
            ValidatableTextBox entryValidation = (ValidatableTextBox)sender;
            //Initialize Country DeafultValue
            cfg_configurationcountry defaultValue = (Customer.Country != null)
              ? Customer.Country
              : XPOSettings.ConfigurationSystemCountry
            ;

            //CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("OrderMain = '{0}'", currentOrderMain.PersistentOid));
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationCountry>
              dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationCountry>(
                this.SourceWindow,
                DialogFlags.DestroyWithParent,
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_select_country"),
                //TODO:THEME
                GlobalApp.MaxWindowSize,
                defaultValue,
                null, //CriteriaOperator,
                GenericTreeViewMode.Default,
                null  //ActionAreaButtons
              );

            int response = dialog.Run();
            if (response == (int)ResponseType.Ok)
            {
                //Get Object from dialog else Mixing Sessions, Both belong to diferente Sessions
                Country = (cfg_configurationcountry)XPOHelper.GetXPGuidObject(typeof(cfg_configurationcountry), dialog.GenericTreeView.DataSourceRow.Oid);
                entryValidation.Text = Country.Designation;
                Validate();
            }
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void AssignPaymentMethod(object pSender)
        {
            //Disable old selectedPaymentMethodButton, if is Selected
            if (SelectedPaymentMethodButton != null) SelectedPaymentMethodButton.Sensitive = true;

            //Enable Sender
            SelectedPaymentMethodButton = (TouchButtonBase)pSender;
            PaymentMethod = (fin_configurationpaymentmethod)XPOHelper.GetXPGuidObject(typeof(fin_configurationpaymentmethod), SelectedPaymentMethodButton.CurrentButtonOid);
            //_logger.Debug(string.Format("AssignPaymentMethod: ButtonName: [{0}], PaymentMethodToken: [{1}]", _selectedPaymentMethodButton.Name, _selectedPaymentMethod.Token));

            if (PaymentMethod.Token == "MONEY")
            {
                if (_labelDeliveryValue.Text != LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym)) _labelDeliveryValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym);
                if (_labelChangeValue.Text != LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym)) _labelChangeValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym);
                //Only Disable Money Button if Delivery is Greater than Total
                if (TotalDelivery >= ((ArticleBagPartialPayment == null) ? ArticleBagFullPayment.TotalFinal : ArticleBagPartialPayment.TotalFinal)) SelectedPaymentMethodButton.Sensitive = false;
            }
            else
            {
                if (_labelDeliveryValue.Text != _labelTotalValue.Text) _labelDeliveryValue.Text = _labelTotalValue.Text;
                if (_labelChangeValue.Text != LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym)) _labelChangeValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(0, XPOSettings.ConfigurationSystemCurrency.Acronym);

                //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
                TotalDelivery = (ArticleBagPartialPayment == null) ? ArticleBagFullPayment.TotalFinal : ArticleBagPartialPayment.TotalFinal;

                TotalChange = 0.0m;

                if (PaymentMethod.Token == "CURRENT_ACCOUNT")
                {
                    ////Hide Partial Payment
                    //_buttonPartialPayment.HideAll();
                    //////Update UI
                    //UpdateUIWhenAlternateFullToPartialPayment(false, false);
                    ////Clean _articleBagPartialPayment, this Enable Normal mode
                    //_articleBagPartialPayment = null;
                }
                else
                {
                    //_buttonPartialPayment.ShowAll();
                }
            }

            SelectedPaymentMethodButton.Sensitive = false;

            //Force Required CustomerCardNumber if Payment is CUSTOMER_CARD
            _entryBoxSelectCustomerCardNumber.EntryValidation.Required = (PaymentMethod.Token == "CUSTOMER_CARD");
            _entryBoxSelectCustomerCardNumber.EntryValidation.Validate();

            Validate();
        }

        private void Validate()
        {
            try
            {
                //Validate Selected Entities and Change Value
                if (_buttonOk != null)
                    _buttonOk.Sensitive =
                      (
                        (
                            Customer != null
                            ||
                            (
                                _entryBoxSelectCustomerName.EntryValidation.Text != string.Empty || _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty
                            )
                        ) &&
                        PaymentMethod != null &&
                        TotalChange >= 0 &&
                        (
                            _entryBoxSelectCustomerName.EntryValidation.Validated &&
                            _entryBoxCustomerDiscount.EntryValidation.Validated &&
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Validated &&
                            _entryBoxCustomerAddress.EntryValidation.Validated &&
                            _entryBoxCustomerLocality.EntryValidation.Validated &&
                            _entryBoxCustomerZipCode.EntryValidation.Validated &&
                            _entryBoxCustomerCity.EntryValidation.Validated &&
                            _entryBoxSelectCustomerCountry.EntryValidation.Validated &&
                            _entryBoxCustomerNotes.EntryValidation.Validated
                        )
                      );

                //Validate Change Value
                if (TotalChange >= 0)
                {
                    _labelChangeValue.ModifyFg(StateType.Normal, Color.White.ToGdkColor());
                }
                else
                {
                    _labelChangeValue.ModifyFg(StateType.Normal, _colorEntryValidationInvalidFont.ToGdkColor());
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public void GetCustomerDetails(string pFieldName, string pFieldValue)
        {
            try
            {
                //Disable calls to this function when we trigger .Changed events, creating recursive calls to this function
                _enableGetCustomerDetails = false;

                Guid customerGuid = new Guid();

                // Encrypt pFieldValue to use in Sql Filter
                if (LogicPOS.Settings.PluginSettings.HasSoftwareVendorPlugin)
                {
                    // Only Encrypt Encrypted Fields
                    if (pFieldName == nameof(erp_customer.FiscalNumber) || pFieldName == nameof(erp_customer.CardNumber))
                    {
                        pFieldValue = LogicPOS.Settings.PluginSettings.SoftwareVendor.Encrypt(pFieldValue);
                    }
                }

                string sql = string.Format("SELECT Oid FROM erp_customer WHERE {0} = '{1}' AND (Hidden IS NULL OR Hidden = 0);", pFieldName, pFieldValue);

                if (pFieldValue != string.Empty)
                {
                    customerGuid = XPOHelper.GetGuidFromQuery(sql);
                }

                if (customerGuid != Guid.Empty)
                {
                    Customer = (erp_customer)XPOHelper.GetXPGuidObject(typeof(erp_customer), customerGuid);
                }
                else
                {
                    Customer = null;
                }

                //If Valid Customer, and not Not SimplifiedInvoice, and ! isSingularEntity
                if (
                    Customer != null
                )
                {
                    Country = Customer.Country;
                    DiscountGlobal = (Customer.Discount > 0) ? Customer.Discount : 0;
                    //Update EntryBoxs
                    _entryBoxSelectCustomerName.EntryValidation.Text = (Customer != null) ? Customer.Name : string.Empty;
                    _entryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(DiscountGlobal);

                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = (Customer.FiscalNumber != null) ? Customer.FiscalNumber.ToString() : string.Empty;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Text = (Customer.CardNumber != null) ? Customer.CardNumber.ToString() : string.Empty;
                    _entryBoxCustomerAddress.EntryValidation.Text = (Customer.Address != null) ? Customer.Address.ToString() : string.Empty;
                    _entryBoxCustomerLocality.EntryValidation.Text = (Customer.Locality != null) ? Customer.Locality.ToString() : string.Empty;
                    _entryBoxCustomerZipCode.EntryValidation.Text = (Customer.ZipCode != null) ? Customer.ZipCode.ToString() : string.Empty;
                    _entryBoxCustomerCity.EntryValidation.Text = (Customer.City != null) ? Customer.City.ToString() : string.Empty;
                    _entryBoxSelectCustomerCountry.Value = Country;
                    _entryBoxSelectCustomerCountry.EntryValidation.Text = (Country != null) ? Country.Designation : string.Empty;
                    _entryBoxCustomerNotes.EntryValidation.Text = (Customer.Notes != null) ? Customer.Notes.ToString() : string.Empty;
                }
                //IN:009275 Use Euro VAT Info 
                else if (logicpos.Utils.UseVatAutocomplete())
                {
                    string cod_FiscalNumber = string.Format("{0}{1}", cfg_configurationpreferenceparameter.GetCountryCode2, _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    if (EuropeanVatInformation.Get(cod_FiscalNumber) != null)
                    {
                        var address = EuropeanVatInformation.Get(cod_FiscalNumber).Address.Split('\n');
                        string zip = address[2].Substring(0, address[2].IndexOf(' '));
                        string city = address[2].Substring(address[2].IndexOf(' ') + 1);
                        _entryBoxCustomerAddress.EntryValidation.Text = address[0];
                        _entryBoxCustomerLocality.EntryValidation.Text = address[1];
                        _entryBoxCustomerZipCode.EntryValidation.Text = zip;
                        _entryBoxCustomerCity.EntryValidation.Text = city;
                        _entryBoxSelectCustomerName.EntryValidation.Text = EuropeanVatInformation.Get(cod_FiscalNumber).Name;
                        _entryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(DiscountGlobal);
                        if (pFieldName != "CardNumber")
                        {
                            _entryBoxSelectCustomerCardNumber.Value = null;
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                        }
                        _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;
                    }
                    else
                    {
                        _entryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                        _entryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                        _entryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                        _entryBoxCustomerCity.EntryValidation.Text = string.Empty;
                        _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                        _entryBoxCustomerDiscount.EntryValidation.Text = string.Empty;
                        if (pFieldName != "CardNumber")
                        {
                            _entryBoxSelectCustomerCardNumber.Value = null;
                            _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                        }
                        _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;
                    }
                }
                ////IN:009275 ENDS
                //New User
                else
                {
                    Customer = null;
                    DiscountGlobal = 0;
                    //Update EntryBoxs
                    _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(DiscountGlobal);
                    _entryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerCity.EntryValidation.Text = string.Empty;
                    if (pFieldName != "FiscalNumber")
                    {
                        _entryBoxSelectCustomerFiscalNumber.Value = null;
                        _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                    }
                    if (pFieldName != "CardNumber")
                    {
                        _entryBoxSelectCustomerCardNumber.Value = null;
                        _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                    }
                    _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;
                }

                //Require to Update RegEx and Criteria to filter Country Clients Only
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExFiscalNumber;
                _entryBoxCustomerZipCode.EntryValidation.Rule = _entryBoxSelectCustomerCountry.Value.RegExZipCode;

                //Apply Criteria Operators
                ApplyCriteriaToCustomerInputs();
                //Call UpdateCustomerEditMode
                UpdateCustomerEditMode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                //Re Enable GetCustomerDetails
                _enableGetCustomerDetails = true;
            }
        }

        //Clear Customer Details
        public void ClearCustomer()
        {
            ClearCustomer(true);
        }

        public void ClearCustomer(bool pClearCountry)
        {
            try
            {
                //Disable calls to this function when we trigger .Changed events, creating recursive calls to this function
                _enableGetCustomerDetails = false;

                //Clear Reference
                Customer = null;

                //Clear Fields
                _entryBoxSelectCustomerName.Value = null;
                _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                _entryBoxSelectCustomerName.EntryValidation.Validate();

                _entryBoxCustomerAddress.EntryValidation.Text = string.Empty;
                _entryBoxCustomerLocality.EntryValidation.Text = string.Empty;
                _entryBoxCustomerZipCode.EntryValidation.Text = string.Empty;
                _entryBoxCustomerCity.EntryValidation.Text = string.Empty;

                if (pClearCountry)
                {
                    _entryBoxSelectCustomerCountry.Value = _intialValueConfigurationCountry;
                    _entryBoxSelectCustomerCountry.EntryValidation.Text = _intialValueConfigurationCountry.Designation;
                    _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                }

                _entryBoxSelectCustomerFiscalNumber.Value = null;
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = string.Empty;
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();

                _entryBoxSelectCustomerCardNumber.Value = null;
                _entryBoxSelectCustomerCardNumber.EntryValidation.Text = string.Empty;
                _entryBoxSelectCustomerCardNumber.EntryValidation.Validate();

                _entryBoxCustomerNotes.EntryValidation.Text = string.Empty;

                //Call UpdateCustomerEditMode
                UpdateCustomerEditMode();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                //Re Enable GetCustomerDetails
                _enableGetCustomerDetails = true;
            }
        }

        //Almost Equal to DocumentFinanceDialogPage2 Method
        //Method to enable/disable edit components based on customer type and many other combinations
        private void UpdateCustomerEditMode()
        {
            try
            {
                //Init Variables
                decimal totalDocument = (ArticleBagPartialPayment == null) ? ArticleBagFullPayment.TotalFinal : ArticleBagPartialPayment.TotalFinal;
                bool isFinalConsumerEntity = (Customer != null && Customer.Oid == InvoiceSettings.FinalConsumerId);
                bool isSingularEntity = (isFinalConsumerEntity || FiscalNumberUtils.IsSingularEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2));
                // Encrypt pFieldValue to use in Sql Filter
                string fiscalNumberFilterValue = string.Empty;
                if (LogicPOS.Settings.PluginSettings.HasSoftwareVendorPlugin)
                {
                    fiscalNumberFilterValue = LogicPOS.Settings.PluginSettings.SoftwareVendor.Encrypt(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                }
                //Used To Disable FiscalNumber Edits and to Get Customer
                string sql = string.Format("SELECT Oid FROM erp_customer WHERE FiscalNumber = '{0}' AND (Hidden IS NULL OR Hidden = 0);", fiscalNumberFilterValue);
                Guid customerGuid = XPOHelper.GetGuidFromQuery(sql);
                erp_customer customer = (customerGuid != Guid.Empty) ? (erp_customer)XPOHelper.GetXPGuidObject(typeof(erp_customer), customerGuid) : null;

                //Required Minimal Fields Edit : SingularEntity
                if (isFinalConsumerEntity)
                {
                    //EntryBox
                    //Janela Pagamentos - Aplicação de desconto total ao utilizador consumidor-final
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = true;
                    _entryBoxCustomerAddress.EntryValidation.Sensitive = false;
                    _entryBoxCustomerLocality.EntryValidation.Sensitive = false;
                    _entryBoxCustomerZipCode.EntryValidation.Sensitive = false;
                    _entryBoxCustomerCity.EntryValidation.Sensitive = false;
                    _entryBoxCustomerNotes.EntryValidation.Sensitive = false;
                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;
                    //Validation
                    //EntryBox
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                }
                else if (totalDocument < GeneralSettings.GetRequiredCustomerDetailsAboveValue(XPOSettings.ConfigurationSystemCountry.Oid) && isSingularEntity)
                {
                    //Enable edit User details, usefull to edit Name, Address etc
                    bool enableEditCustomerDetails = !isFinalConsumerEntity;

                    //Address
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerAddress.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerAddress.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Locality
                    _entryBoxCustomerLocality.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerLocality.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //ZipCode
                    _entryBoxCustomerZipCode.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerZipCode.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //City
                    _entryBoxCustomerCity.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerCity.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    //Notes
                    _entryBoxCustomerNotes.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxCustomerNotes.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;

                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = enableEditCustomerDetails;
                    //Always Disabled/Only Enabled in New Customer
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = (customer == null || customer != null && customer.FiscalNumber != string.Empty);
                    _entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = (customer == null || customer != null && customer.FiscalNumber != string.Empty);
                    _entryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = enableEditCustomerDetails;
                    _entryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = true;
                    //Validation
                    //EntryBox
                    _entryBoxSelectCustomerName.EntryValidation.Required = false;
                    _entryBoxCustomerAddress.EntryValidation.Required = false;
                    _entryBoxCustomerZipCode.EntryValidation.Required = false;
                    _entryBoxCustomerCity.EntryValidation.Required = false;
                    //EntryBoxSelect
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                }
                else
                {
                    //EntryBox
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = true;
                    _entryBoxCustomerAddress.EntryValidation.Sensitive = true;
                    _entryBoxCustomerLocality.EntryValidation.Sensitive = true;
                    _entryBoxCustomerZipCode.EntryValidation.Sensitive = true;
                    _entryBoxCustomerCity.EntryValidation.Sensitive = true;
                    _entryBoxCustomerNotes.EntryValidation.Sensitive = true;
                    //EntryBoxSelect
                    _entryBoxSelectCustomerName.EntryValidation.Sensitive = (customer == null || !string.IsNullOrEmpty(customer.Name));
                    _entryBoxSelectCustomerName.ButtonKeyBoard.Sensitive = (customer == null || !string.IsNullOrEmpty(customer.Name));
                    _entryBoxSelectCustomerName.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCountry.EntryValidation.Sensitive = true;
                    _entryBoxSelectCustomerCountry.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Sensitive = (customer == null || !string.IsNullOrEmpty(customer.FiscalNumber));
                    _entryBoxSelectCustomerFiscalNumber.ButtonKeyBoard.Sensitive = (customer == null || !string.IsNullOrEmpty(customer.FiscalNumber));
                    _entryBoxSelectCustomerFiscalNumber.ButtonSelectValue.Sensitive = true;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.ButtonKeyBoard.Sensitive = false;
                    _entryBoxSelectCustomerCardNumber.ButtonSelectValue.Sensitive = false;

                    //Validation
                    //EntryBox
                    _entryBoxCustomerAddress.EntryValidation.Required = true;
                    _entryBoxCustomerZipCode.EntryValidation.Required = true;
                    _entryBoxCustomerCity.EntryValidation.Required = true;
                }

                //Always update Discount Global
                DiscountGlobal = (customer != null && customer.Discount > 0) ? customer.Discount : 0;
                _entryBoxCustomerDiscount.EntryValidation.Text = LogicPOS.Utility.DataConversionUtils.DecimalToString(DiscountGlobal);

                //Always Validate All Fields
                //EntryBox
                _entryBoxCustomerDiscount.EntryValidation.Validate();
                _entryBoxCustomerAddress.EntryValidation.Validate();
                _entryBoxCustomerLocality.EntryValidation.Validate();
                _entryBoxCustomerZipCode.EntryValidation.Validate();
                _entryBoxCustomerCity.EntryValidation.Validate();
                //EntryBoxSelect
                _entryBoxSelectCustomerCountry.EntryValidation.Validate(_entryBoxSelectCustomerCountry.Value.Oid.ToString());
                _entryBoxSelectCustomerCardNumber.EntryValidation.Validate();

                //Call Before ReCheck if FiscalNumber is Valid
                UpdateCustomerAddressAndFiscalNumberRequireFields();

                //ReCheck if FiscalNumber is Valid
                if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty && _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated)
                {
                    bool isValidFiscalNumber = FiscalNumberUtils.IsValidFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2);
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = isValidFiscalNumber;
                }

                //Shared

                //Require Validate
                Validate();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //Almost Equal to DocumentFinanceDialogPage2 Method : Both methods have same Name
        //Update Address And FiscalNumber Require Fields
        private void UpdateCustomerAddressAndFiscalNumberRequireFields()
        {
            bool isFinalConsumerEntity = (Customer != null && Customer.Oid == InvoiceSettings.FinalConsumerId);
            bool isSingularEntity = (
                isFinalConsumerEntity ||
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                FiscalNumberUtils.IsSingularEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2)
            );

            //If is a SingularEntity Disable Address, ZipCode and City
            if (isSingularEntity)
            {
                _entryBoxCustomerAddress.EntryValidation.Required = false;
                _entryBoxCustomerZipCode.EntryValidation.Required = false;
                _entryBoxCustomerCity.EntryValidation.Required = false;
                _entryBoxCustomerAddress.EntryValidation.Validate();
                _entryBoxCustomerZipCode.EntryValidation.Validate();
                _entryBoxCustomerCity.EntryValidation.Validate();
            }

            //Always Required NIF or Client Name, or Both if none has Filled or ! isSingularEntity
            if (
                    (
                        _entryBoxSelectCustomerName.EntryValidation.Text == string.Empty &&
                        (
                            _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty
                            || !_entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated
                         )
                    )
                    ||
                    (
                        !isSingularEntity && _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty
                    )
                )
            {
                _entryBoxSelectCustomerName.EntryValidation.Required = true;
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = true;
                _entryBoxSelectCustomerName.EntryValidation.Validate();
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
            }
            else if (_entryBoxSelectCustomerName.EntryValidation.Text == string.Empty)
            {
                _entryBoxSelectCustomerName.EntryValidation.Required = false;
                _entryBoxSelectCustomerName.EntryValidation.Validate();
            }
            else if (_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty)
            {
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Required = false;
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validate();
            }
        }

        private void ApplyCriteriaToCustomerInputs()
        {
            _entryBoxSelectCustomerName.CriteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0) AND (Country = '{0}')", _entryBoxSelectCustomerCountry.Value.Oid));
            _entryBoxSelectCustomerFiscalNumber.CriteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0) AND (Country = '{0}') AND (FiscalNumber IS NOT NULL AND FiscalNumber <> '')", _entryBoxSelectCustomerCountry.Value.Oid));
            _entryBoxSelectCustomerCardNumber.CriteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled IS NULL OR Disabled  <> 1) AND (Hidden IS NULL OR Hidden = 0) AND (Country = '{0}') AND (CardNumber IS NOT NULL AND CardNumber <> '')", _entryBoxSelectCustomerCountry.Value.Oid));
        }

        private void UpdateChangeValue()
        {
            if (PaymentMethod != null && PaymentMethod.Token == "MONEY")
            {
                int decimalRoundTo = LogicPOS.Settings.CultureSettings.DecimalRoundTo;
                //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
                decimal _totalOrder = (ArticleBagPartialPayment == null) ? ArticleBagFullPayment.TotalFinal : ArticleBagPartialPayment.TotalFinal;
                TotalChange = Math.Round(TotalDelivery, decimalRoundTo) - Math.Round(_totalOrder, decimalRoundTo);
                if (_labelChangeValue.Text != LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym)) _labelChangeValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PartialPayment

        private void PartialPayment()
        {
            //Button icon source path
            string buttonIconChangePrice = PathsSettings.ImagesFolderLocation + @"Icons\icon_pos_cashdrawer_out.png";

            //Default ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);

            //Pagamentos parciais -Escolher valor a pagar por artigo[TK: 019295]
            TouchButtonIconWithText touchButtonChangePrice = new TouchButtonIconWithText("touchButtonChangePrice", Color.Transparent, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_price"), _fontBaseDialogActionAreaButton, _colorBaseDialogActionAreaButtonFont, buttonIconChangePrice, _sizeBaseDialogActionAreaButtonIcon, _sizeBaseDialogActionAreaButton.Width, _sizeBaseDialogActionAreaButton.Height);
            buttonOk.Sensitive = false;

            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonOk = new ActionAreaButton(buttonOk, ResponseType.Ok);
            ActionAreaButton actionAreaButtonCancel = new ActionAreaButton(buttonCancel, ResponseType.Cancel);

            //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
            ActionAreaButton actionAreaButtonChangePrice = new ActionAreaButton(touchButtonChangePrice, ResponseType.Apply);
            actionAreaButtons.Add(actionAreaButtonChangePrice);
            actionAreaButtonChangePrice.Button.Sensitive = false;

            actionAreaButtons.Add(actionAreaButtonOk);
            actionAreaButtons.Add(actionAreaButtonCancel);
            //if(_articleBagFullPayment.TotalQuantity <= 1)
            //{


            //Reset Vars in Next Call
            _totalPartialPaymentItems = 0;

            _dialogPartialPayment =
              new PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment>(
                this,
                DialogFlags.DestroyWithParent,
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_partial_payment"),
                //TODO:THEME
                GlobalApp.MaxWindowSize,
                //new Guid(), //use this to test pDefaultValue 
                //new Guid("630ff869-e433-46bb-a53b-563c43535424"), 
                null, //pDefaultValue : Require to Send a DataRow
                GenericTreeViewMode.CheckBox,
                actionAreaButtons
              );

            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            //_dialogPartialPayment.CursorChanged += delegate
            _dialogPartialPayment.CheckBoxToggled += delegate
            {
                //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
                if (_dialogPartialPayment.GenericTreeViewMode == GenericTreeViewMode.Default)
                {
                    //DataTableMode else use XPGuidObject
                    if (_dialogPartialPayment.GenericTreeView.DataSourceRow != null) actionAreaButtonOk.Button.Sensitive = true;
                }
                else if (_dialogPartialPayment.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                {
                    actionAreaButtonOk.Button.Sensitive = (_dialogPartialPayment.GenericTreeView.MarkedCheckBoxs > 0);

                    //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
                    actionAreaButtonChangePrice.Button.Sensitive = (_dialogPartialPayment.GenericTreeView.MarkedCheckBoxs > 0);

                    //Get Indexes
                    int indexColumnCheckBox = _dialogPartialPayment.GenericTreeView.DataSource.Columns.IndexOf("CheckBox");
                    int indexColumnPrice = _dialogPartialPayment.GenericTreeView.DataSource.Columns.IndexOf("Price");
                    int indexColumnDiscount = _dialogPartialPayment.GenericTreeView.DataSource.Columns.IndexOf("Discount");
                    int indexColumnVat = _dialogPartialPayment.GenericTreeView.DataSource.Columns.IndexOf("Vat");
                    int indexColumnPriceFinal = _dialogPartialPayment.GenericTreeView.DataSource.Columns.IndexOf("PriceFinal");

                    //Update Dialog Title with Total Checked Items
                    bool itemChecked = (bool)_dialogPartialPayment.GenericTreeView.DataSourceRow.ItemArray[indexColumnCheckBox];
                    decimal currentRowPrice = (decimal)_dialogPartialPayment.GenericTreeView.DataSourceRow.ItemArray[indexColumnPriceFinal];
                    _totalPartialPaymentItems += (itemChecked) ? currentRowPrice : -currentRowPrice;
                    _dialogPartialPayment.WindowTitle = (_totalPartialPaymentItems > 0) ? string.Format("{0} : {1}", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_partial_payment"), LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_totalPartialPaymentItems, XPOSettings.ConfigurationSystemCurrency.Acronym)) : CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_partial_payment");
                }
            };

            //Events
            _dialogPartialPayment.Response += dialogPartialPayment_Response;

            //Call Dialog
            int response = _dialogPartialPayment.Run();
            //Destroy Dialog
            _dialogPartialPayment.Destroy();
        }

        private void dialogPartialPayment_Response(object o, ResponseArgs args)
        {
            PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment>
              dialog = (PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment>)o;

            //Article article = (Article) dialog.XPGuidObject;

            if (args.ResponseId != ResponseType.Cancel)
            {
                if (args.ResponseId == ResponseType.Ok)
                {
                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        //use dialog.GenericTreeView.DataTableRow.ItemArray
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Init Global ArticleBag
                        ArticleBagPartialPayment = new ArticleBag();
                        //Fill articleBagPartialPayment with checked items
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));
                        //Process ArticleBag
                        ProcessPartialPayment(ArticleBagPartialPayment);
                    }
                }
                //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
                if (args.ResponseId == ResponseType.Apply)
                {
                    //Init Global ArticleBag
                    if (ArticleBagPartialPayment == null)
                    {
                        ArticleBagPartialPayment = new ArticleBag();
                    }

                    dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTaskDivide));

                    if (newValuePrice > 0)
                    {
                        ProcessPartialPayment(ArticleBagPartialPayment, false);
                    }
                    //dialog.Run();
                }
                //[TK:019295] Ends
            }
        }

        private bool TreeModelForEachTask(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexIndex = 0;
            int columnIndexCheckBox = 1;
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;
            DataTable dataTable = _dialogPartialPayment.GenericTreeView.DataSource;

            try
            {
                int itemIndex = Convert.ToInt32(model.GetValue(iter, columnIndexIndex).ToString());
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));

                string token1 = string.Empty;
                string token2 = string.Empty;

                if (itemChecked)
                {
                    //Always get values from DataTable, this way we have Types :)
                    Guid itemGuid = (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")];
                    //_logger.Debug(string.Format("{0}:{1}:{2}", itemIndex, itemChecked, itemGuid));

                    //Prepare articleBag Key and Props
                    articleBagKey = new ArticleBagKey(
                      (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")],
                      (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Designation")],
                      (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Price")],
                      (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Discount")],
                      (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Vat")]
                    );
                    articleBagProps = new ArticleBagProperties(
                      (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PlaceOid")],
                      (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("TableOid")],
                      (PriceType)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceType")],
                      (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Code")],
                      Convert.ToDecimal(dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Quantity")]),
                      (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("UnitMeasure")]
                    );
                    //Token Work
                    if (!dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token1")].Equals(DBNull.Value))
                        token1 = (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token1")];
                    if (!dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token2")].Equals(DBNull.Value))
                        token2 = (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token2")];
                    if (token1 != string.Empty) articleBagProps.Token1 = token1;
                    if (token2 != string.Empty) articleBagProps.Token2 = token2;
                    //Send to Bag
                    ArticleBagPartialPayment.Add(articleBagKey, articleBagProps);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return false;
        }

        //Pagamentos parciais - Escolher valor a pagar por artigo [TK:019295]
        private bool TreeModelForEachTaskDivide(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexIndex = 0;
            int columnIndexCheckBox = 1;
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;
            DataTable dataTable = _dialogPartialPayment.GenericTreeView.DataSource;
            int itemIndex = Convert.ToInt32(model.GetValue(iter, columnIndexIndex).ToString());
            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));

            try
            {
                if (itemChecked)
                {
                    //Get Money pad title based on line selected (Desigantion & Price)
                    decimal priceFinal = decimal.Round((decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceFinal")], 2);
                    string priceFinalText = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(priceFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);

                    string moneyPadTitle = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_moneypad_product_price") + " :: " + (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Designation")] + " :: " +
                        priceFinalText);


                    MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(_sourceWindow, moneyPadTitle, priceFinal, priceFinal);
                    newValuePrice = result.Value;

                    if (result.Response == ResponseType.Ok && newValuePrice > 0)
                    {
                        if (newValuePrice > decimal.Round((decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceFinal")], 2))
                        {
                            logicpos.Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent, MessageType.Warning, ButtonsType.Ok, "Valor Errado", "Valor inserido superior ao total");
                        }
                        else
                        {
                            //dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceFinal")] = decimal.Round(newValuePrice,2);

                            string token1 = string.Empty;
                            string token2 = string.Empty;


                            //Always get values from DataTable, this way we have Types :)
                            Guid itemGuid = (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")];
                            //_logger.Debug(string.Format("{0}:{1}:{2}", itemIndex, itemChecked, itemGuid));

                            //Calculate values with new price
                            decimal vatRate = (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Vat")];
                            decimal PriceNet = (newValuePrice / (vatRate / 100 + 1));
                            decimal totalTax = newValuePrice - PriceNet;
                            decimal oldQuantity = Convert.ToDecimal(dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Quantity")]);
                            decimal quantity = (newValuePrice * oldQuantity) / (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceFinal")];

                            //Prepare articleBag Key and Props
                            articleBagKey = new ArticleBagKey(
                              (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")],
                              (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Designation")],
                              (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Price")],
                              (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Discount")],
                              (decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Vat")]
                            );
                            articleBagProps = new ArticleBagProperties(
                              (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PlaceOid")],
                              (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("TableOid")],
                              (PriceType)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceType")],
                              (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Code")],
                              quantity,
                              (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("UnitMeasure")]
                            );
                            //Token Work
                            if (!dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token1")].Equals(DBNull.Value))
                                token1 = (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token1")];
                            if (!dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token2")].Equals(DBNull.Value))
                                token2 = (string)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token2")];
                            if (token1 != string.Empty) articleBagProps.Token1 = token1;
                            if (token2 != string.Empty) articleBagProps.Token2 = token2;

                            //Send to Bag
                            ArticleBagPartialPayment.Add(articleBagKey, articleBagProps);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return false;
        }

        private void ProcessPartialPayment(ArticleBag pArticleBag)
        {
            ProcessPartialPayment(pArticleBag, false);
        }

        private void ProcessPartialPayment(ArticleBag pArticleBag, bool changePrice)
        {
            bool debug = false;

            //Update UI
            UpdateUIWhenAlternateFullToPartialPayment(true, changePrice);
            //Debug
            if (debug) pArticleBag.ShowInLog();
        }

        public void UpdateUIWhenAlternateFullToPartialPayment(bool pIsPartialPayment, bool changePrice, bool pResetPaymentMethodButton = true)
        {
            //Update private member _partialPaymentEnabled
            _partialPaymentEnabled = pIsPartialPayment;
            // Commented to Prevend Cleaning _totalDelivery and _totalChange
            //Shared: Update Total Delivery and TotalChange 
            if (pResetPaymentMethodButton)
            {
                TotalDelivery = 0;
                TotalChange = 0;
            }
            //Discount
            DiscountGlobal = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_entryBoxCustomerDiscount.EntryValidation.Text);

            //PartialPayment
            if (_partialPaymentEnabled)
            {
                //Update Discount
                ArticleBagPartialPayment.DiscountGlobal = DiscountGlobal;
                //Update Totals after Change Discount
                ArticleBagPartialPayment.UpdateTotals();
                //Update UI
                _labelTotalValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(ArticleBagPartialPayment.TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);
                _labelDeliveryValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym);
                _labelChangeValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym);
                //Update UI Buttons
                _buttonFullPayment.Sensitive = true;
                _buttonPartialPayment.Sensitive = false;
            }
            //Full Payment
            else
            {
                //Update Discount
                ArticleBagFullPayment.DiscountGlobal = DiscountGlobal;
                //Update Totals after Change Discount
                ArticleBagFullPayment.UpdateTotals();
                //Update UI to Default From OrderMain
                _labelTotalValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(ArticleBagFullPayment.TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym);
                _labelDeliveryValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalDelivery, XPOSettings.ConfigurationSystemCurrency.Acronym);
                _labelChangeValue.Text = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(TotalChange, XPOSettings.ConfigurationSystemCurrency.Acronym);
                //Update UI Buttons
                if (_buttonFullPayment != null) _buttonFullPayment.Sensitive = false;
                if (_buttonPartialPayment != null) _buttonPartialPayment.Sensitive = true;
            }

            //Shared: Disable Payment Method if it is Assigned
            if (pResetPaymentMethodButton)
            {
                if (SelectedPaymentMethodButton != null && !SelectedPaymentMethodButton.Sensitive) SelectedPaymentMethodButton.Sensitive = true;
                if (PaymentMethod != null) PaymentMethod = null;
            }

            Validate();
        }
    }
}