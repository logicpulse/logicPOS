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
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosPaymentsDialog
    {
        //Commmon Button Event
        void buttonCheck_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }
        void buttonMB_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }
        void buttonCredit_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }
        /* IN009142 */
        void buttonDebitCard_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }
        void buttonVisa_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }
        void buttonCurrentAccount_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }
        void buttonCustomerCard_Clicked(object sender, EventArgs e) { AssignPaymentMethod(sender); }

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
                    GlobalFramework.UsingThermalPrinter = true;
                    //SaveOrUpdateCustomer Before use _selectedCustomer (Can be null)
                    resultObject = Utils.SaveOrUpdateCustomer(
                        this,
                        _selectedCustomer,
                        _entryBoxSelectCustomerName.EntryValidation.Text,
                        _entryBoxCustomerAddress.EntryValidation.Text,
                        _entryBoxCustomerLocality.EntryValidation.Text,
                        _entryBoxCustomerZipCode.EntryValidation.Text,
                        _entryBoxCustomerCity.EntryValidation.Text,
                        null,// Phone : Used only in PosDocumentFinanceDialog
                        null,// Email : Used only in PosDocumentFinanceDialog 
                        _selectedCountry,
                        _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text,
                        _entryBoxSelectCustomerCardNumber.EntryValidation.Text,
                        FrameworkUtils.StringToDecimal(_entryBoxCustomerDiscount.EntryValidation.Text),
                        _entryBoxCustomerNotes.EntryValidation.Text
                     );

                    if (resultObject.GetType() == typeof(erp_customer))
                    {
                        _selectedCustomer = (erp_customer)resultObject;

                        //Prevent Default Customer Entity and Hidden Customer (Only with Name Filled) to Process CC Documents
                        if (
                            _selectedPaymentMethod != null && _selectedPaymentMethod.Token == "CURRENT_ACCOUNT" &&
                            (
                                _selectedCustomer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity ||
                                _entryBoxSelectCustomerName.EntryValidation.Text == string.Empty ||
                                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text == string.Empty
                            )
                        )
                        {
                            Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_cant_create_cc_document_with_default_entity"));
                            //Prevent Parent Dialog Payments from Close
                            this.Run();
                        }
                        //Proceed with normal ProcessFinanceDocument
                        else
                        {
                            //Get Document Type to Emmit, based on Payment Mode
                            _processDocumentType = (_selectedPaymentMethod.Token == "CURRENT_ACCOUNT")
                                ? SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput
                                : SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice;

                            ArticleBag processArticleBag;

                            if (_articleBagPartialPayment == null)
                            {
                                processArticleBag = _articleBagFullPayment;
                            }
                            else
                            {
                                processArticleBag = _articleBagPartialPayment;
                            }

                            //Default UnAssigned Value
                            ResponseType responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice = ResponseType.None;
                            //Get response for user confirmation to emmit Invoice-Payment before Extra Protections, we must have user respose before enter in "Extra Protections" above
                            if (
                                    processArticleBag.TotalFinal > SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal ||
                                    processArticleBag.GetClassTotals("S") > SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices
                                )
                            {
                                responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice = Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceed(_sourceWindow, ShowMessageTouchSimplifiedInvoiceMaxValueExceedMode.PaymentsDialog, processArticleBag.TotalFinal, SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal, processArticleBag.GetClassTotals("S"), SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices);
                                //Override Back processDocumentType if Exceed FS Max Total
                                if (responseTypeOverrideDefaultDocumentTypeSimplifiedInvoice == ResponseType.Yes)
                                {
                                    _processDocumentType = SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment;
                                }
                            }

                            //Start Extra Protections

                            //Protection to prevent Exceed FinanceRuleSimplifiedInvoiceMaxValue
                            if (
                                    (
                                        processArticleBag.TotalFinal > SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotal ||
                                        processArticleBag.GetClassTotals("S") > SettingsApp.FinanceRuleSimplifiedInvoiceMaxTotalServices
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
                                    _processDocumentType == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                                    _processDocumentType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                                    _processDocumentType == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                                ) &&
                                FrameworkUtils.IsInValidFinanceDocumentCustomer(
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
                                Utils.ShowMessageTouchSimplifiedInvoiceMaxValueExceedForFinalConsumer(this, processArticleBag.TotalFinal, SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue);

                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            //Protection to prevent Exceed Customer CardCredit
                            else if (
                                _selectedPaymentMethod.Token == "CUSTOMER_CARD" &&
                                (
                                    processArticleBag.TotalFinal > _selectedCustomer.CardCredit
                                )
                            )
                            {
                                Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_value_exceed_customer_card_credit"), FrameworkUtils.DecimalToStringCurrency(_selectedCustomer.CardCredit), FrameworkUtils.DecimalToStringCurrency(processArticleBag.TotalFinal)));

                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            //Protection to Prevent Recharge Customer Card with Invalid User (User without Card or FinalConsumer...)
                            //Check if Article Bag Full|Partial has Recharge Article and Valid customer Card
                            else if (!FrameworkUtils.IsCustomerCardValidForArticleBag(processArticleBag, _selectedCustomer))
                            {
                                Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_invalid_customer_card_detected"));

                                //Prevent Parent Dialog Payments from Close
                                this.Run();
                            }
                            else
                            {
                                //Prepare ProcessFinanceDocumentParameter : Shared for PartialPayment and FullPayment
                                _processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
                                    _processDocumentType, processArticleBag
                                )
                                {
                                    PaymentMethod = _selectedPaymentMethod.Oid,
                                    PaymentCondition = SettingsApp.XpoOidConfigurationPaymentConditionDefaultInvoicePaymentCondition,
                                    Customer = _selectedCustomer.Oid,
                                    TotalDelivery = _totalDelivery,
                                    TotalChange = _totalChange
                                };

                                //Get Latest DocumentConference Document if Exists, and assign if (REMOVED Total Equality, Request from Carlos)
                                fin_documentfinancemaster conferenceDocument = FrameworkUtils.GetOrderMainLastDocumentConference(false);
                                if (
                                    conferenceDocument != null
                                /*&& (conferenceDocument.TotalFinal.Equals(processArticleBag.TotalFinal) && conferenceDocument.DocumentDetail.Count.Equals(processArticleBag.Count))*/
                                )
                                {
                                    _processFinanceDocumentParameter.DocumentParent = conferenceDocument.Oid;
                                    _processFinanceDocumentParameter.OrderReferences = new List<fin_documentfinancemaster>();
                                    _processFinanceDocumentParameter.OrderReferences.Add(conferenceDocument);
                                }

                                // PreventPersistFinanceDocument : Used in SplitPayments, to get ProcessFinanceDocumentParameter and Details without PreventPersistFinanceDocument
                                if (!_skipPersistFinanceDocument)
                                {
                                    fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(this, _processFinanceDocumentParameter);
                                    //If Errors Occurs, return null Document, Keep Running until user cancel or a Valid Document is Returned
                                    if (resultDocument == null)
                                    {
                                        this.Run();
                                    }
                                    else
                                    {
                                        //Update Display
                                        if (GlobalApp.UsbDisplay != null) GlobalApp.UsbDisplay.ShowPayment(_selectedPaymentMethod.Designation, _totalDelivery, _totalChange);
                                    }
                                }
                            }
                        }
                    }
                    //If error in Save or Update
                    else if (resultObject.GetType() == typeof(ConstraintViolationException))
                    {
                        Exception ex = (Exception)resultObject;
                        ResponseType response = Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_exception_error"), ex.InnerException.Message);
                        //Prevent Parent Dialog Payments from Close
                        this.Run();
                    }
                }
                //ResponseTypeFullPayment
                else if (pResponse == _responseTypeFullPayment)
                {
                    //Update UI
                    UpdateUIWhenAlternateFullToPartialPayment(false);

                    //Clean _articleBagPartialPayment, this Enable Normal mode
                    _articleBagPartialPayment = null;

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

                    //Prevent Parent Dialog Payments from Close 
                    this.Run();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                string errorMessage = string.Empty;

                switch (ex.Message)
                {
                    case "ERROR_MISSING_SERIE":
                        errorMessage = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_creating_financial_document"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_creating_financial_document_missing_series"));
                        break;
                    default:
                        errorMessage = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_error_creating_financial_document"), ex.Message);
                        break;
                }
                Utils.ShowMessageTouch(
                    _sourceWindow,
                    DialogFlags.Modal,
                    new Size(600, 400),
                    MessageType.Error,
                    ButtonsType.Close,
                    resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"),
                    errorMessage
                );

                this.Run();
            }
        }

        void buttonMoney_Clicked(object sender, EventArgs e)
        {
            //Settings
            int decimalRoundTo = SettingsApp.DecimalRoundTo;

            //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
            decimal _totalOrder = (_articleBagPartialPayment == null) ? _articleBagFullPayment.TotalFinal : _articleBagPartialPayment.TotalFinal;

            MoneyPadResult result = PosMoneyPadDialog.RequestDecimalValue(this, _totalOrder);
            if (result.Response == ResponseType.Ok)
            {
                _totalDelivery = result.Value;
                //Round currentOrderMain.GlobalTotalFinal, else we can have values like _totalChange -0.000000000000069
                _totalChange = Math.Round(_totalDelivery, decimalRoundTo) - Math.Round(_totalOrder, decimalRoundTo);
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
            ArticleBag articleBag = (_partialPaymentEnabled) ? _articleBagPartialPayment : _articleBagFullPayment;
            if (articleBag != null)
            {
                //Required to Update articleBag.
                _discountGlobal = (_selectedCustomer != null && _selectedCustomer.Discount > 0) ? _selectedCustomer.Discount : 0;
                articleBag.DiscountGlobal = _discountGlobal;
                articleBag.UpdateTotals();
                //Update UI
                UpdateUIWhenAlternateFullToPartialPayment(_partialPaymentEnabled, false);
                //Require to Update _totalDelivery when Discount is Changed, if no Money
                //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
                if (_selectedPaymentMethod != null && _selectedPaymentMethod.Token != "MONEY")
                {
                    _totalDelivery = (_articleBagPartialPayment == null) ? _articleBagFullPayment.TotalFinal : _articleBagPartialPayment.TotalFinal;
                    if (_labelDeliveryValue.Text != FrameworkUtils.DecimalToStringCurrency(_totalDelivery)) _labelDeliveryValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalDelivery);
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
                    bool isValidFiscalNumber = FiscalNumber.IsValidFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2);
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

        void _entryBoxCountry_EntryValidation_FocusGrabbed(object sender, EventArgs e)
        {
            EntryValidation entryValidation = (EntryValidation)sender;
            //Initialize Country DeafultValue
            cfg_configurationcountry defaultValue = (_selectedCustomer.Country != null)
              ? _selectedCustomer.Country
              : SettingsApp.ConfigurationSystemCountry
            ;

            //CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("OrderMain = '{0}'", currentOrderMain.PersistentOid));
            PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationCountry>
              dialog = new PosSelectRecordDialog<XPCollection, XPGuidObject, TreeViewConfigurationCountry>(
                this.SourceWindow,
                DialogFlags.DestroyWithParent,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_select_country"),
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
                _selectedCountry = (cfg_configurationcountry)FrameworkUtils.GetXPGuidObject(typeof(cfg_configurationcountry), dialog.GenericTreeView.DataSourceRow.Oid);
                entryValidation.Text = _selectedCountry.Designation;
                Validate();
            }
            dialog.Destroy();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void AssignPaymentMethod(object pSender)
        {
            //Disable old selectedPaymentMethodButton, if is Selected
            if (_selectedPaymentMethodButton != null) _selectedPaymentMethodButton.Sensitive = true;

            //Enable Sender
            _selectedPaymentMethodButton = (TouchButtonBase)pSender;
            _selectedPaymentMethod = (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationpaymentmethod), _selectedPaymentMethodButton.CurrentButtonOid);
            //_log.Debug(string.Format("AssignPaymentMethod: ButtonName: [{0}], PaymentMethodToken: [{1}]", _selectedPaymentMethodButton.Name, _selectedPaymentMethod.Token));

            if (_selectedPaymentMethod.Token == "MONEY")
            {
                if (_labelDeliveryValue.Text != FrameworkUtils.DecimalToStringCurrency(_totalDelivery)) _labelDeliveryValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalDelivery);
                if (_labelChangeValue.Text != FrameworkUtils.DecimalToStringCurrency(_totalChange)) _labelChangeValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalChange);
                //Only Disable Money Button if Delivery is Greater than Total
                if (_totalDelivery >= ((_articleBagPartialPayment == null) ? _articleBagFullPayment.TotalFinal : _articleBagPartialPayment.TotalFinal)) _selectedPaymentMethodButton.Sensitive = false;
            }
            else
            {
                if (_labelDeliveryValue.Text != _labelTotalValue.Text) _labelDeliveryValue.Text = _labelTotalValue.Text;
                if (_labelChangeValue.Text != FrameworkUtils.DecimalToStringCurrency(0)) _labelChangeValue.Text = FrameworkUtils.DecimalToStringCurrency(0);

                //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
                _totalDelivery = (_articleBagPartialPayment == null) ? _articleBagFullPayment.TotalFinal : _articleBagPartialPayment.TotalFinal;

                _totalChange = 0.0m;

                if (_selectedPaymentMethod.Token == "CURRENT_ACCOUNT")
                {
                    ////Hide Partial Payment
                    //_buttonPartialPayment.HideAll();
                    ////Update UI
                    //UpdateUIWhenAlternateFullToPartialPayment(false);
                    //Clean _articleBagPartialPayment, this Enable Normal mode
                    //_articleBagPartialPayment = null;
                }
                else
                {
                    //_buttonPartialPayment.ShowAll();
                }
            }

            _selectedPaymentMethodButton.Sensitive = false;

            //Force Required CustomerCardNumber if Payment is CUSTOMER_CARD
            _entryBoxSelectCustomerCardNumber.EntryValidation.Required = (_selectedPaymentMethod.Token == "CUSTOMER_CARD");
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
                            _selectedCustomer != null
                            ||
                            (
                                _entryBoxSelectCustomerName.EntryValidation.Text != string.Empty || _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text != string.Empty
                            )
                        ) &&
                        _selectedPaymentMethod != null &&
                        _totalChange >= 0 &&
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
                if (_totalChange >= 0)
                {
                    _labelChangeValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(Color.White));
                }
                else
                {
                    _labelChangeValue.ModifyFg(StateType.Normal, Utils.ColorToGdkColor(_colorEntryValidationInvalidFont));
                };
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
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
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    // Only Encrypt Encrypted Fields
                    if (pFieldName == nameof(erp_customer.FiscalNumber) || pFieldName == nameof(erp_customer.CardNumber))
                    {
                        pFieldValue = GlobalFramework.PluginSoftwareVendor.Encrypt(pFieldValue);
                    }
                }

                string sql = string.Format("SELECT Oid FROM erp_customer WHERE {0} = '{1}' AND (Hidden IS NULL OR Hidden = 0);", pFieldName, pFieldValue);

                if (pFieldValue != string.Empty)
                {
                    customerGuid = FrameworkUtils.GetGuidFromQuery(sql);
                }

                if (customerGuid != Guid.Empty)
                {
                    _selectedCustomer = (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), customerGuid);
                }
                else
                {
                    _selectedCustomer = null;
                }

                //If Valid Customer, and not Not SimplifiedInvoice, and ! isSingularEntity
                if (
                    _selectedCustomer != null
                )
                {
                    _selectedCountry = _selectedCustomer.Country;
                    _discountGlobal = (_selectedCustomer.Discount > 0) ? _selectedCustomer.Discount : 0;
                    //Update EntryBoxs
                    _entryBoxSelectCustomerName.EntryValidation.Text = (_selectedCustomer != null) ? _selectedCustomer.Name : string.Empty;
                    _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(_discountGlobal);

                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text = (_selectedCustomer.FiscalNumber != null) ? _selectedCustomer.FiscalNumber.ToString() : string.Empty;
                    _entryBoxSelectCustomerCardNumber.EntryValidation.Text = (_selectedCustomer.CardNumber != null) ? _selectedCustomer.CardNumber.ToString() : string.Empty;
                    _entryBoxCustomerAddress.EntryValidation.Text = (_selectedCustomer.Address != null) ? _selectedCustomer.Address.ToString() : string.Empty;
                    _entryBoxCustomerLocality.EntryValidation.Text = (_selectedCustomer.Locality != null) ? _selectedCustomer.Locality.ToString() : string.Empty;
                    _entryBoxCustomerZipCode.EntryValidation.Text = (_selectedCustomer.ZipCode != null) ? _selectedCustomer.ZipCode.ToString() : string.Empty;
                    _entryBoxCustomerCity.EntryValidation.Text = (_selectedCustomer.City != null) ? _selectedCustomer.City.ToString() : string.Empty;
                    _entryBoxSelectCustomerCountry.Value = _selectedCountry;
                    _entryBoxSelectCustomerCountry.EntryValidation.Text = (_selectedCountry != null) ? _selectedCountry.Designation : string.Empty;
                    _entryBoxCustomerNotes.EntryValidation.Text = (_selectedCustomer.Notes != null) ? _selectedCustomer.Notes.ToString() : string.Empty;
                }
                //IN:009275 Use Euro VAT Info 
                else if (Utils.UseVatAutocomplete())
                {
                    string cod_FiscalNumber = string.Format("{0}{1}", cfg_configurationpreferenceparameter.GetCountryCode2, _entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                    var address = EuropeanVatInformation.Get(cod_FiscalNumber).Address.Split('\n');
                    if (address != null)
                    {
                        string zip = address[2].Substring(0, address[2].IndexOf(' '));
                        string city = address[2].Substring(address[2].IndexOf(' ') + 1);
                        _entryBoxCustomerAddress.EntryValidation.Text = address[0];
                        _entryBoxCustomerLocality.EntryValidation.Text = address[1];
                        _entryBoxCustomerZipCode.EntryValidation.Text = zip;
                        _entryBoxCustomerCity.EntryValidation.Text = city;
                        _entryBoxSelectCustomerName.EntryValidation.Text = EuropeanVatInformation.Get(cod_FiscalNumber).Name;
                        _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(_discountGlobal);
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
                    _selectedCustomer = null;
                    _discountGlobal = 0;
                    //Update EntryBoxs
                    _entryBoxSelectCustomerName.EntryValidation.Text = string.Empty;
                    _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(_discountGlobal);
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
                _log.Error(ex.Message, ex);
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
                _selectedCustomer = null;

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
                _log.Error(ex.Message, ex);
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
                decimal totalDocument = (_articleBagPartialPayment == null) ? _articleBagFullPayment.TotalFinal : _articleBagPartialPayment.TotalFinal;
                bool isFinalConsumerEntity = (_selectedCustomer != null && _selectedCustomer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) ? true : false;
                bool isSingularEntity = (isFinalConsumerEntity || FiscalNumber.IsSingularEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2));
                // Encrypt pFieldValue to use in Sql Filter
                string fiscalNumberFilterValue = string.Empty;
                if (GlobalFramework.PluginSoftwareVendor != null)
                {
                    fiscalNumberFilterValue = GlobalFramework.PluginSoftwareVendor.Encrypt(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text);
                }
                //Used To Disable FiscalNumber Edits and to Get Customer
                string sql = string.Format("SELECT Oid FROM erp_customer WHERE FiscalNumber = '{0}' AND (Hidden IS NULL OR Hidden = 0);", fiscalNumberFilterValue);
                Guid customerGuid = FrameworkUtils.GetGuidFromQuery(sql);
                erp_customer customer = (customerGuid != Guid.Empty) ? (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), customerGuid) : null;

                //Required Minimal Fields Edit : SingularEntity
                if (isFinalConsumerEntity)
                {
                    //EntryBox
                    _entryBoxCustomerDiscount.EntryValidation.Sensitive = false;
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
                else if (totalDocument < SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue && isSingularEntity)
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
                _discountGlobal = (customer != null && customer.Discount > 0) ? customer.Discount : 0;
                _entryBoxCustomerDiscount.EntryValidation.Text = FrameworkUtils.DecimalToString(_discountGlobal);

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
                    bool isValidFiscalNumber = FiscalNumber.IsValidFiscalNumber(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2);
                    _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated = isValidFiscalNumber;
                }

                //Shared

                //Require Validate
                Validate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //Almost Equal to DocumentFinanceDialogPage2 Method : Both methods have same Name
        //Update Address And FiscalNumber Require Fields
        private void UpdateCustomerAddressAndFiscalNumberRequireFields()
        {
            bool isFinalConsumerEntity = (_selectedCustomer != null && _selectedCustomer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity) ? true : false;
            bool isSingularEntity = (
                isFinalConsumerEntity ||
                _entryBoxSelectCustomerFiscalNumber.EntryValidation.Validated &&
                FiscalNumber.IsSingularEntity(_entryBoxSelectCustomerFiscalNumber.EntryValidation.Text, _entryBoxSelectCustomerCountry.Value.Code2)
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
            if (_selectedPaymentMethod != null && _selectedPaymentMethod.Token == "MONEY")
            {
                int decimalRoundTo = SettingsApp.DecimalRoundTo;
                //If Has a _articleBagPartialPayment Defined use its Total else use _articleBagFullPayment TotalFinal
                decimal _totalOrder = (_articleBagPartialPayment == null) ? _articleBagFullPayment.TotalFinal : _articleBagPartialPayment.TotalFinal;
                _totalChange = Math.Round(_totalDelivery, decimalRoundTo) - Math.Round(_totalOrder, decimalRoundTo);
                if (_labelChangeValue.Text != FrameworkUtils.DecimalToStringCurrency(_totalChange)) _labelChangeValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalChange);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PartialPayment

        private void PartialPayment()
        {
            //Default ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            buttonOk.Sensitive = false;

            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonOk = new ActionAreaButton(buttonOk, ResponseType.Ok);
            ActionAreaButton actionAreaButtonCancel = new ActionAreaButton(buttonCancel, ResponseType.Cancel);
            actionAreaButtons.Add(actionAreaButtonOk);
            actionAreaButtons.Add(actionAreaButtonCancel);

            //Reset Vars in Next Call
            _totalPartialPaymentItems = 0;

            _dialogPartialPayment =
              new PosSelectRecordDialog<DataTable, DataRow, TreeViewPartialPayment>(
                this,
                DialogFlags.DestroyWithParent,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_partial_payment"),
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
                    actionAreaButtonOk.Button.Sensitive = (_dialogPartialPayment.GenericTreeView.MarkedCheckBoxs > 0) ? true : false;

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
                    _dialogPartialPayment.WindowTitle = (_totalPartialPaymentItems > 0) ? string.Format("{0} : {1}", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_partial_payment"), FrameworkUtils.DecimalToStringCurrency(_totalPartialPaymentItems)) : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_partial_payment");
                }
            };

            //Events
            _dialogPartialPayment.Response += dialogPartialPayment_Response;

            //Call Dialog
            int response = _dialogPartialPayment.Run();
            //Destroy Dialog
            _dialogPartialPayment.Destroy();
        }

        void dialogPartialPayment_Response(object o, ResponseArgs args)
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
                        _articleBagPartialPayment = new ArticleBag();
                        //Fill articleBagPartialPayment with checked items
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));
                        //Process ArticleBag
                        ProcessPartialPayment(_articleBagPartialPayment);
                    }
                }
                //dialog.Run();
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
                Int32 itemIndex = Convert.ToInt32(model.GetValue(iter, columnIndexIndex).ToString());
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));

                string token1 = string.Empty;
                string token2 = string.Empty;

                if (itemChecked)
                {
                    //Always get values from DataTable, this way we have Types :)
                    Guid itemGuid = (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")];
                    //_log.Debug(string.Format("{0}:{1}:{2}", itemIndex, itemChecked, itemGuid));

                    //Prepare articleBag Key and Props
                    articleBagKey = new ArticleBagKey(
                      (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")],
                      (String)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Designation")],
                      (Decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Price")],
                      (Decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Discount")],
                      (Decimal)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Vat")]
                    );
                    articleBagProps = new ArticleBagProperties(
                      (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PlaceOid")],
                      (Guid)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("TableOid")],
                      (PriceType)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("PriceType")],
                      (String)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Code")],
                      (Int16)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Quantity")],
                      (String)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("UnitMeasure")]
                    );
                    //Token Work
                    if (!dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token1")].Equals(System.DBNull.Value))
                        token1 = (String)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token1")];
                    if (!dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token2")].Equals(System.DBNull.Value))
                        token2 = (String)dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Token2")];
                    if (token1 != string.Empty) articleBagProps.Token1 = token1;
                    if (token2 != string.Empty) articleBagProps.Token2 = token2;
                    //Send to Bag
                    _articleBagPartialPayment.Add(articleBagKey, articleBagProps);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return false;
        }

        private void ProcessPartialPayment(ArticleBag pArticleBag)
        {
            bool debug = false;

            //Update UI
            UpdateUIWhenAlternateFullToPartialPayment(true);
            //Debug
            if (debug) pArticleBag.ShowInLog();
        }

        public void UpdateUIWhenAlternateFullToPartialPayment(bool pIsPartialPayment, bool pResetPaymentMethodButton = true)
        {
            //Update private member _partialPaymentEnabled
            _partialPaymentEnabled = pIsPartialPayment;
            // Commented to Prevend Cleaning _totalDelivery and _totalChange
            //Shared: Update Total Delivery and TotalChange 
            if (pResetPaymentMethodButton) 
            {
                _totalDelivery = 0;
                _totalChange = 0;
            }
            //Discount
            _discountGlobal = FrameworkUtils.StringToDecimal(_entryBoxCustomerDiscount.EntryValidation.Text);

            //PartialPayment
            if (_partialPaymentEnabled)
            {
                //Update Discount
                _articleBagPartialPayment.DiscountGlobal = _discountGlobal;
                //Update Totals after Change Discount
                _articleBagPartialPayment.UpdateTotals();
                //Update UI
                _labelTotalValue.Text = FrameworkUtils.DecimalToStringCurrency(_articleBagPartialPayment.TotalFinal);
                _labelDeliveryValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalDelivery);
                _labelChangeValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalChange);
                //Update UI Buttons
                _buttonFullPayment.Sensitive = true;
                _buttonPartialPayment.Sensitive = false;
            }
            //Full Payment
            else
            {
                //Update Discount
                _articleBagFullPayment.DiscountGlobal = _discountGlobal;
                //Update Totals after Change Discount
                _articleBagFullPayment.UpdateTotals();
                //Update UI to Default From OrderMain
                _labelTotalValue.Text = FrameworkUtils.DecimalToStringCurrency(_articleBagFullPayment.TotalFinal);
                _labelDeliveryValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalDelivery);
                _labelChangeValue.Text = FrameworkUtils.DecimalToStringCurrency(_totalChange);
                //Update UI Buttons
                if (_buttonFullPayment != null) _buttonFullPayment.Sensitive = false;
                if (_buttonPartialPayment != null) _buttonPartialPayment.Sensitive = true;
            }

            //Shared: Disable Payment Method if it is Assigned
            if (pResetPaymentMethodButton)
            {
                if (_selectedPaymentMethodButton != null && !_selectedPaymentMethodButton.Sensitive) _selectedPaymentMethodButton.Sensitive = true;
                if (_selectedPaymentMethod != null) _selectedPaymentMethod = null;
            }

            Validate();
        }
    }
}