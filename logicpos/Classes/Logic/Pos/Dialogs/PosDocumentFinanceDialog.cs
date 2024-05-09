using DevExpress.Xpo.DB.Exceptions;
using Gtk;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.shared.Enums;
using logicpos.datalayer.Enums;
using logicpos.shared.Classes.Finance;
using System;
using System.Collections.Generic;
using System.Data;
using logicpos.Classes.Enums.Dialogs;
using logicpos.shared.App;
using logicpos.financial.library.App;
using logicpos.datalayer.Xpo;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosDocumentFinanceDialog
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
                        PrintingSettings.UsingThermalPrinter = false;
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
                    ResponseType response = logicpos.Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_message_dialog"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_finance_dialog_confirm_cancel"));
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
            decimal customerDiscount = LogicPOS.Utility.DataConversionUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text);
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
                _pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == DocumentSettings.XpoOidDocumentFinanceTypeCreditNote
                && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value != null
                && _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.Oid != new Guid()
            )
            {
                Guid guidDocumentParent = _pagePad1.EntryBoxSelectSourceDocumentFinance.Value.Oid;
                //Get Source Document
                sourceFinanceMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), guidDocumentParent);
                referencesReason = _pagePad1.EntryBoxReason.EntryValidation.Text;
            };

            if(_pagePad1.EntryBoxSelectDocumentFinanceType.Value.Oid == DocumentSettings.XpoOidDocumentFinanceTypeCreditNote)
            {

                List<DataRow> dataRows = new List<DataRow>();
                foreach (DataRow item in _pagePad3.TreeViewArticles.DataSource.Rows)
                {
                    article = (item["Article.Code"] as fin_article);
                    if (article != null && article.UniqueArticles)
                    {
                        item["Notes"] = string.Empty;
                        if (item["Warehouse"].ToString().Contains(";"))
                        {
                            var splitArticleWareHouse = item["Warehouse"].ToString().Split(';');
                            var splitArticleSerialNumber = item["SerialNumber"].ToString().Split(';');
                            for (int i = 0; i < splitArticleWareHouse.Length; i++)
                            {
                                DataRow newRow = _pagePad3.TreeViewArticles.DataSource.NewRow();
                                for (int j = 0; j < item.ItemArray.Length; j++)
                                {
                                    newRow[j] = item[j];
                                }
                                newRow[10] = item[5];
                                newRow[11] = item[12];
                                newRow["Warehouse"] = splitArticleWareHouse[i];
                                newRow["SerialNumber"] = splitArticleSerialNumber[i];
                                newRow["Quantity"] = 1;
                                dataRows.Add(newRow);                                
                            }
                            item["Warehouse"] = splitArticleWareHouse[0];
                            item["SerialNumber"] = splitArticleSerialNumber[0];
                        }
                    }              
                }
                if (dataRows.Count > 0)
                {
                    _pagePad3.TreeViewArticles.DataSource.Rows.Clear();
                    foreach (var items in dataRows)
                    {
                        _pagePad3.TreeViewArticles.DataSource.Rows.Add(items);
                    }
                }
            }


            foreach (DataRow item in _pagePad3.TreeViewArticles.DataSource.Rows)
            {
                article = (item["Article.Code"] as fin_article);
                configurationVatRate = (item["ConfigurationVatRate.Value"] as fin_configurationvatrate);
                configurationVatExemptionReason = (item["VatExemptionReason.Acronym"] as fin_configurationvatexemptionreason);
               

                PriceProperties priceProperties = PriceProperties.GetPriceProperties(
                PricePropertiesSourceMode.FromPriceUser,
                false, //PriceWithVat : Always use PricesWithoutVat in Invoices
                Convert.ToDecimal(item["PriceFinal"]),
                LogicPOS.Utility.DataConversionUtils.StringToDecimal(item["Quantity"].ToString()),
                LogicPOS.Utility.DataConversionUtils.StringToDecimal(item["Discount"].ToString()),
                LogicPOS.Utility.DataConversionUtils.StringToDecimal(customerDiscount.ToString()),
                LogicPOS.Utility.DataConversionUtils.StringToDecimal(configurationVatRate.Value.ToString())
            );

                //Documentos - Arredondamentos de preço [IN:016536]
                //PriceProperties calcPriceProps = PriceProperties.GetPriceProperties(
                //  PricePropertiesSourceMode.FromPriceUser,
                //  true,
                //  Convert.ToDecimal(item["Price"]),
                //  Convert.ToDecimal(item["Quantity"]),
                //  Convert.ToDecimal(item["Discount"]),
                //  Convert.ToDecimal(customerDiscount),
                //  Convert.ToDecimal(configurationVatRate.Value)
                //);

                //Prepare articleBag Key and Props
                articleBagKey = new ArticleBagKey(
                  new Guid(item["Oid"].ToString()),
                  article.Designation,
                  Convert.ToDecimal(item["Price"]),     //Always use Price in DefaultCurrency
                  LogicPOS.Utility.DataConversionUtils.StringToDecimal(item["Discount"].ToString()),
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

                //SerialNumber
                if (!string.IsNullOrEmpty(item["SerialNumber"].ToString()))
                {
                    articleBagProps.SerialNumber = item["SerialNumber"].ToString();
                    articleBagProps.Notes += CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_serial_number") + ": " + item["SerialNumber"].ToString();
                }

                // Notes
                if (!string.IsNullOrEmpty(item["Notes"].ToString()))
                {
                    articleBagProps.Notes += item["Notes"].ToString();
                }

                // Warehouse
                if (!string.IsNullOrEmpty(item["Warehouse"].ToString()))
                {
                    articleBagProps.Warehouse += item["Warehouse"].ToString();
                }

                //Assign DocumentMaster Reference and Reason to ArticleBag item
                if (sourceFinanceMaster != null)
                {
                    articleBagProps.Reference = sourceFinanceMaster;
                    articleBagProps.Reason = referencesReason;
                }

                articleBag.Add(articleBagKey, articleBagProps);
            }
            _pagePad3.TreeViewArticles.Refresh();
            return articleBag;
        }

        private ProcessFinanceDocumentParameter GetFinanceDocumentParameter()
        {
            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            _pagePad3.ArticleBag = GetArticleBag();
            bool wayBillMode = _pagePad1.EntryBoxSelectDocumentFinanceType.Value.WayBill;

            object resultObject;

            resultObject = logicpos.Utils.SaveOrUpdateCustomer(
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
                LogicPOS.Utility.DataConversionUtils.StringToDecimal(_pagePad2.EntryBoxCustomerDiscount.EntryValidation.Text),
                _pagePad2.EntryBoxCustomerNotes.EntryValidation.Text
            );


            erp_customer customer;
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
                    ResponseType response = logicpos.Utils.ShowMessageTouch(_sourceWindow, DialogFlags.DestroyWithParent | DialogFlags.Modal, MessageType.Warning, ButtonsType.Close, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_exception_error"), ex.InnerException.Message);
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
                fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationpaymentmethod), result.PaymentMethod);
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
                List<fin_documentfinancemaster> orderReferences = new List<fin_documentfinancemaster>
                {
                    _pagePad1.EntryBoxSelectSourceDocumentFinance.Value
                };
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

            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            _pagePad3.ArticleBag = GetArticleBag();

            return result;
        }

        private ResponseType ShowPreview(DocumentFinanceDialogPreviewMode pMode)
        {
            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            //_pagePad3.ArticleBag = GetArticleBag();            
            
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
                    logicpos.Utils.ShowMessageTouch(
                        this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"), 
                        string.Format(
                            CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_value_exceed_customer_card_credit"), 
                            LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_pagePad2.EntryBoxSelectCustomerName.Value.CardCredit, SharedSettings.ConfigurationSystemCurrency.Acronym), 
                            LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(articleBag.TotalFinal, SharedSettings.ConfigurationSystemCurrency.Acronym)
                        )
                    );
                    result = false;
                }

                //Protection to Prevent Recharge Customer Card with Invalid User (User without Card or FinalConsumer...)
                if (result && ! FinancialLibraryUtils.IsCustomerCardValidForArticleBag(articleBag, _pagePad2.EntryBoxSelectCustomerName.Value))
                {
                    logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_error"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "dialog_message_invalid_customer_card_detected"));
                    result = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }
    }
}