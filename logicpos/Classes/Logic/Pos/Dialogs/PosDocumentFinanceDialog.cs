using DevExpress.Xpo.DB.Exceptions;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.shared.Enums;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;
using System.Data;
using logicpos.Classes.Enums.Dialogs;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosDocumentFinanceDialog
    {
        protected override void OnResponse(ResponseType pResponse)
        {
            if (pResponse == ResponseType.Ok)
            {
                //Call Last Validation before send to PersistFinanceDocument/Validation, before Preview
                if (!LastValidation())
                {
                    //Keep Running
                    this.Run();
                }
                else
                {
                    //Procced After Post Validation
                    ResponseType response = ShowPreview(DocumentFinanceDialogPreviewMode.Confirmation);
                
                    if (response == ResponseType.Yes)
                    {
                        //Get Parameters
						//TK016249 - Impressoras - Diferenciação entre Tipos
                        GlobalFramework.UsingThermalPrinter = false;
                        ProcessFinanceDocumentParameter processFinanceDocumentParameter = GetFinanceDocumentParameter();

                        //If error in Save or Update Customer
                        if (processFinanceDocumentParameter.Customer == Guid.Empty)
                        {
                            //Keep Running
                            this.Run();
                        }
                        else
                        {
                            //Proccess Document
                            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(_sourceWindow, processFinanceDocumentParameter);
                            //If Errors Occurs, return null Document, Keep Running until user cancel or a Valid Document is Returned
                            if (resultDocument == null) this.Run();
                        }
                    }
                    else
                    {
                        //Keep Running
                        this.Run();
                    }
                }
            }
            //Prevent Cancel Document
            else if (pResponse == ResponseType.Cancel)
            {
                if (_pagePad3.ArticleBag != null && _pagePad3.ArticleBag.Count > 0)
                {
                    ResponseType response = Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_message_dialog"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_finance_dialog_confirm_cancel"));
                    if (response == ResponseType.No)
                    {
                        //Keep Running
                        this.Run();
                    }
                }                
            }
            else if (pResponse == _responseTypePreview)
            {
                //ShowPreview Dialog
                ShowPreview(DocumentFinanceDialogPreviewMode.Preview);

                //Always Keep Running
                this.Run();
            }
            else if (pResponse == _responseTypeClearCustomer)
            {
                //ClearCustomer
                _pagePad2.ClearCustomerAndWayBill();

                //Always Keep Running
                this.Run();
            }
        }

        public ArticleBag GetArticleBag()
        {
            Decimal customerDiscount = FrameworkUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text);
            ArticleBag articleBag = new ArticleBag(customerDiscount);
            ArticleBagKey articleBagKey;
            ArticleBagProperties articleBagProps;
            fin_article article;
            fin_configurationvatrate configurationVatRate;
            fin_configurationvatexemptionreason configurationVatExemptionReason;

            //DocumentParent/SourceDocument
            fin_documentfinancemaster sourceFinanceMaster = null;
            string referencesReason = string.Empty;
            if (
                _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == SettingsApp.XpoOidDocumentFinanceTypeCreditNote
                && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null
                && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.Oid != new Guid()
            )
            {
                Guid guidDocumentParent = _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.Oid;
                //Get Source Document
                sourceFinanceMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), guidDocumentParent);
                referencesReason = _pagePad1.EntryBoxReason.EntryValidation.Text;
            };

            foreach (DataRow item in _pagePad3.TreeViewArticles.DataSource.Rows)
            {
                article = (item["Article.Code"] as fin_article);
                configurationVatRate = (item["ConfigurationVatRate.Value"] as fin_configurationvatrate);
                configurationVatExemptionReason = (item["VatExemptionReason.Acronym"] as fin_configurationvatexemptionreason);

                //Prepare articleBag Key and Props
                articleBagKey = new ArticleBagKey(
                  new Guid(item["Oid"].ToString()),
                  article.Designation,
                  Convert.ToDecimal(item["Price"]),     //Always use Price in DefaultCurrency
                  Convert.ToDecimal(item["Discount"]),
                  configurationVatRate.Value,
                    //If has a Valid ConfigurationVatExemptionReason use it Else send New Guid
                  (configurationVatExemptionReason != null) ? configurationVatExemptionReason.Oid : new Guid()
                );
                articleBagProps = new ArticleBagProperties(
                  new Guid(),         //pPlaceOid,
                  new Guid(),         //pTableOid,
                  PriceType.Price1,   //pPriceType,
                  article.Code,
                  Convert.ToDecimal(item["Quantity"]),
                  article.UnitMeasure.Acronym
                );

                // Notes
                if (!string.IsNullOrEmpty(item["Notes"].ToString()))
                {
                    articleBagProps.Notes = item["Notes"].ToString();
                }

                //Assign DocumentMaster Reference and Reason to ArticleBag item
                if (sourceFinanceMaster != null)
                {
                    articleBagProps.Reference = sourceFinanceMaster;
                    articleBagProps.Reason = referencesReason;
                }

                articleBag.Add(articleBagKey, articleBagProps);
            }

            return articleBag;
        }

        private ProcessFinanceDocumentParameter GetFinanceDocumentParameter()
        {
            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            _pagePad3.ArticleBag = GetArticleBag();

            erp_customer customer = null;
            bool wayBillMode = _pagePad1.EntryBoxSelectDocumentFinanceType.Value.WayBill;

            object resultObject;

            resultObject = Utils.SaveOrUpdateCustomer(
                this,
                _pagePad2.EntryBoxSelectCustomerName.Value,
                _pagePad2.EntryBoxSelectCustomerName.EntryValidation.Text,
                _pagePad2.EntryBoxCustomerAddress.EntryValidation.Text,
                _pagePad2.EntryBoxCustomerLocality.EntryValidation.Text,
                _pagePad2.EntryBoxCustomerZipCode.EntryValidation.Text,
                _pagePad2.EntryBoxCustomerCity.EntryValidation.Text,
_pagePad2.EntryBoxCustomerPhone.EntryValidation.Text,
_pagePad2.EntryBoxCustomerEmail.EntryValidation.Text,
                _pagePad2.EntryBoxSelectCustomerCountry.Value,
                _pagePad2.EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text,
                _pagePad2.EntryBoxSelectCustomerCardNumber.EntryValidation.Text,
                FrameworkUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text),
                _pagePad2.EntryBoxCustomerNotes.EntryValidation.Text
            );

            if (resultObject.GetType() == typeof(erp_customer))
            {
                customer = (erp_customer)resultObject;
            }
            else
            {
                //If error in Save or Update Customer
                if (resultObject.GetType() == typeof(ConstraintViolationException))
                {
                    Exception ex = (Exception)resultObject;
                    ResponseType response = Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_exception_error"), ex.InnerException.Message);
                }
                customer = null;
            }

            //Construct ProcessFinanceDocumentParameter
            ProcessFinanceDocumentParameter result = new ProcessFinanceDocumentParameter(
              _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid, _pagePad3.ArticleBag
            )
            {
                Customer = (customer != null) ? customer.Oid : Guid.Empty,
            };

            //PaymentConditions
            if (_pagePad1.EntryBoxSelectConfigurationPaymentCondition.Value != null
                && _pagePad1.EntryBoxSelectConfigurationPaymentCondition.Value.Oid != new Guid())
            {
                result.PaymentCondition = _pagePad1.EntryBoxSelectConfigurationPaymentCondition.Value.Oid;
            }

            //PaymentMethod
            if (_pagePad1.EntryBoxSelectConfigurationPaymentMethod.Value != null
                && _pagePad1.EntryBoxSelectConfigurationPaymentMethod.Value.Oid != new Guid())
            {
                result.PaymentMethod = _pagePad1.EntryBoxSelectConfigurationPaymentMethod.Value.Oid;
            }

            //TotalDelivery: If Money force TotalDelivery to be equal to TotalFinal
            if (result.PaymentMethod != null && result.PaymentMethod != new Guid())
            {
                fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_configurationpaymentmethod), result.PaymentMethod);
                if (paymentMethod.Token == "MONEY")
                {
                    result.TotalDelivery = _pagePad3.ArticleBag.TotalFinal;
                }
            }

            //Currency
            if (_pagePad1.EntryBoxSelectConfigurationCurrency.Value.Oid != new Guid())
            {
                result.Currency = _pagePad1.EntryBoxSelectConfigurationCurrency.Value.Oid;
                result.ExchangeRate = (_pagePad1.EntryBoxSelectConfigurationCurrency.Value.ExchangeRate == 0) ? 1 : _pagePad1.EntryBoxSelectConfigurationCurrency.Value.ExchangeRate;
            }

            //DocumentParent
            if (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.Oid != new Guid())
            {
                result.DocumentParent = _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.Oid;
            }

            //SourceMode
            result.SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag;

            //Not Used
            //SourceOrderMain : Dont have a OrderMain Source
            //FinanceDocuments
            //TotalChange

            //OrderReferences
            if (_pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null) 
            {
                List<fin_documentfinancemaster> orderReferences = new List<fin_documentfinancemaster>();
                orderReferences.Add(_pagePad1.EntryBoxSelectSourceDocumentFinance.Value);
                result.OrderReferences = orderReferences;
            }

            //If in WayBillMode Assign ShipTo and ShipFrom
            if (wayBillMode)
            {
                //ShipTo: Not Used : Address, BuildingNumber, StreetName
                result.ShipTo = new MovementOfGoodsProperties();
                if (_pagePad4.EntryBoxShipToDeliveryID.EntryValidation.Text != string.Empty) result.ShipTo.DeliveryID = _pagePad4.EntryBoxShipToDeliveryID.EntryValidation.Text;
                if (_pagePad4.EntryBoxShipToDeliveryDate.EntryValidation.Text != string.Empty) result.ShipTo.DeliveryDate = Convert.ToDateTime(_pagePad4.EntryBoxShipToDeliveryDate.EntryValidation.Text);
                if (_pagePad4.EntryBoxShipToWarehouseID.EntryValidation.Text != string.Empty) result.ShipTo.WarehouseID = _pagePad4.EntryBoxShipToWarehouseID.EntryValidation.Text;
                if (_pagePad4.EntryBoxShipToLocationID.EntryValidation.Text != string.Empty) result.ShipTo.LocationID = _pagePad4.EntryBoxShipToLocationID.EntryValidation.Text;
                if (_pagePad4.EntryBoxShipToAddressDetail.EntryValidation.Text != string.Empty) result.ShipTo.AddressDetail = _pagePad4.EntryBoxShipToAddressDetail.EntryValidation.Text;
                if (_pagePad4.EntryBoxShipToCity.EntryValidation.Text != string.Empty) result.ShipTo.City = _pagePad4.EntryBoxShipToCity.EntryValidation.Text;
                if (_pagePad4.EntryBoxShipToPostalCode.EntryValidation.Text != string.Empty) result.ShipTo.PostalCode = _pagePad4.EntryBoxShipToPostalCode.EntryValidation.Text;
                if (_pagePad4.EntryBoxShipToRegion.EntryValidation.Text != string.Empty) result.ShipTo.Region = _pagePad4.EntryBoxShipToRegion.EntryValidation.Text;
                if (_pagePad4.EntryBoxSelectShipToCountry.Value.Oid != new Guid())
                {
                    result.ShipTo.Country = _pagePad4.EntryBoxSelectShipToCountry.Value.Code2;
                    result.ShipTo.CountryGuid = _pagePad4.EntryBoxSelectShipToCountry.Value.Oid;
                }
                //ShipFrom: Not Used : Address, BuildingNumber, StreetName
                result.ShipFrom = new MovementOfGoodsProperties();
                if (_pagePad5.EntryBoxShipFromDeliveryID.EntryValidation.Text != string.Empty) result.ShipFrom.DeliveryID = _pagePad5.EntryBoxShipFromDeliveryID.EntryValidation.Text;
                if (_pagePad5.EntryBoxShipFromDeliveryDate.EntryValidation.Text != string.Empty) result.ShipFrom.DeliveryDate = Convert.ToDateTime(_pagePad5.EntryBoxShipFromDeliveryDate.EntryValidation.Text);
                if (_pagePad5.EntryBoxShipFromWarehouseID.EntryValidation.Text != string.Empty) result.ShipFrom.WarehouseID = _pagePad5.EntryBoxShipFromWarehouseID.EntryValidation.Text;
                if (_pagePad5.EntryBoxShipFromLocationID.EntryValidation.Text != string.Empty) result.ShipFrom.LocationID = _pagePad5.EntryBoxShipFromLocationID.EntryValidation.Text;
                if (_pagePad5.EntryBoxShipFromAddressDetail.EntryValidation.Text != string.Empty) result.ShipFrom.AddressDetail = _pagePad5.EntryBoxShipFromAddressDetail.EntryValidation.Text;
                if (_pagePad5.EntryBoxShipFromCity.EntryValidation.Text != string.Empty) result.ShipFrom.City = _pagePad5.EntryBoxShipFromCity.EntryValidation.Text;
                if (_pagePad5.EntryBoxShipFromPostalCode.EntryValidation.Text != string.Empty) result.ShipFrom.PostalCode = _pagePad5.EntryBoxShipFromPostalCode.EntryValidation.Text;
                if (_pagePad5.EntryBoxShipFromRegion.EntryValidation.Text != string.Empty) result.ShipFrom.Region = _pagePad5.EntryBoxShipFromRegion.EntryValidation.Text;
                if (_pagePad5.EntryBoxSelectShipFromCountry.Value.Oid != new Guid())
                {
                    result.ShipFrom.Country = _pagePad5.EntryBoxSelectShipFromCountry.Value.Code2;
                    result.ShipFrom.CountryGuid = _pagePad5.EntryBoxSelectShipFromCountry.Value.Oid;
                }
            }

            //Notes
            if (_pagePad1.EntryBoxDocumentMasterNotes.EntryValidation.Text != string.Empty)
            {
                result.Notes = _pagePad1.EntryBoxDocumentMasterNotes.EntryValidation.Text;
            }

            return result;
        }

        private ResponseType ShowPreview(DocumentFinanceDialogPreviewMode pMode)
        {
            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            _pagePad3.ArticleBag = GetArticleBag();            
            
            DocumentFinanceDialogPreview dialog = new DocumentFinanceDialogPreview(this, DialogFlags.DestroyWithParent, pMode, _pagePad3.ArticleBag, _pagePad1.EntryBoxSelectConfigurationCurrency.Value);
            ResponseType response = (ResponseType) dialog.Run();
            dialog.Destroy();
            return response;
        }

        //OnResponse Ok Post Validation, Last Validations before Procced to PersistFinanceDocument
        private bool LastValidation()
        {
            //Defaylt is true
            bool result = true;
            ArticleBag articleBag = GetArticleBag();

            try
            {
                //Protection to prevent Exceed Customer CardCredit
                if (
                    //Can be null if not in a Payable DocumentType
                    _pagePad1.EntryBoxSelectConfigurationPaymentMethod.Value != null 
                    && _pagePad1.EntryBoxSelectConfigurationPaymentMethod.Value.Token == "CUSTOMER_CARD" 
                    && articleBag.TotalFinal > _pagePad2.EntryBoxSelectCustomerName.Value.CardCredit
                )
                {
                    Utils.ShowMessageTouch(
                        this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), 
                        string.Format(
                            resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_value_exceed_customer_card_credit"), 
                            FrameworkUtils.DecimalToStringCurrency(_pagePad2.EntryBoxSelectCustomerName.Value.CardCredit), 
                            FrameworkUtils.DecimalToStringCurrency(articleBag.TotalFinal)
                        )
                    );
                    result = false;
                }

                //Protection to Prevent Recharge Customer Card with Invalid User (User without Card or FinalConsumer...)
                if (result && ! FrameworkUtils.IsCustomerCardValidForArticleBag(articleBag, _pagePad2.EntryBoxSelectCustomerName.Value))
                {
                    Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_invalid_customer_card_detected"));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}