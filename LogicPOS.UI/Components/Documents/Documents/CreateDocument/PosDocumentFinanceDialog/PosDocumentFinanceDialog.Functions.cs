using DevExpress.Xpo.DB.Exceptions;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.UI;
using LogicPOS.UI.Alerts;
using System;
using System.Collections.Generic;
using System.Data;

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
                        PrintingSettings.ThermalPrinter.UsingThermalPrinter = false;
                        DocumentProcessingParameters processFinanceDocumentParameter = GetFinanceDocumentParameter();

                        //If error in Save or Update Customer
                        if (processFinanceDocumentParameter.Customer == Guid.Empty)
                        {
                            //Keep Running
                            this.Run();
                        }
                        else
                        {
                            //Proccess Document
                            fin_documentfinancemaster resultDocument = FrameworkCalls.PersistFinanceDocument(WindowSettings.Source,
                                                                                                             processFinanceDocumentParameter);

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
                if (Page3.ArticleBag != null && Page3.ArticleBag.Count > 0)
                {
                    var response = new CustomAlert(WindowSettings.Source)
                                            .WithMessageType(MessageType.Question)
                                            .WithButtonsType(ButtonsType.YesNo)
                                            .WithTitleResource("window_title_dialog_message_dialog")
                                            .WithMessageResource("dialog_message_finance_dialog_confirm_cancel")
                                            .ShowAlert();
                    
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
                Page2.ClearCustomerAndWayBill();

                //Always Keep Running
                this.Run();
            }
        }

        public ArticleBag GetArticleBag()
        {
            decimal customerDiscount = LogicPOS.Utility.DataConversionUtils.StringToDecimal(Page2.EntryBoxCustomerDiscount.EntryValidation.Text);
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
                Page1.EntryBoxSelectDocumentFinanceType.Value.Oid == CustomDocumentSettings.CreditNoteId
                && Page1.EntryBoxSelectSourceDocumentFinance.Value != null
                && Page1.EntryBoxSelectSourceDocumentFinance.Value.Oid != new Guid()
            )
            {
                Guid guidDocumentParent = Page1.EntryBoxSelectSourceDocumentFinance.Value.Oid;
                //Get Source Document
                sourceFinanceMaster = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), guidDocumentParent);
                referencesReason = Page1.EntryBoxReason.EntryValidation.Text;
            };

            if (Page1.EntryBoxSelectDocumentFinanceType.Value.Oid == CustomDocumentSettings.CreditNoteId)
            {

                List<DataRow> dataRows = new List<DataRow>();
                foreach (DataRow item in Page3.TreeViewArticles.Entities.Rows)
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
                                DataRow newRow = Page3.TreeViewArticles.Entities.NewRow();
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
                    Page3.TreeViewArticles.Entities.Rows.Clear();
                    foreach (var items in dataRows)
                    {
                        Page3.TreeViewArticles.Entities.Rows.Add(items);
                    }
                }
            }


            foreach (DataRow item in Page3.TreeViewArticles.Entities.Rows)
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
                    articleBagProps.Notes += CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_serial_number") + ": " + item["SerialNumber"].ToString();
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
            Page3.TreeViewArticles.Refresh();
            return articleBag;
        }

        private DocumentProcessingParameters GetFinanceDocumentParameter()
        {
            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            Page3.ArticleBag = GetArticleBag();
            bool wayBillMode = Page1.EntryBoxSelectDocumentFinanceType.Value.WayBill;

            object resultObject;

            resultObject = logicpos.Utils.SaveOrUpdateCustomer(
                this,
                Page2.EntryBoxSelectCustomerName.Value,
                Page2.EntryBoxSelectCustomerName.EntryValidation.Text,
                Page2.EntryBoxCustomerAddress.EntryValidation.Text,
                Page2.EntryBoxCustomerLocality.EntryValidation.Text,
                Page2.EntryBoxCustomerZipCode.EntryValidation.Text,
                Page2.EntryBoxCustomerCity.EntryValidation.Text,
Page2.EntryBoxCustomerPhone.EntryValidation.Text,
Page2.EntryBoxCustomerEmail.EntryValidation.Text,
                Page2.EntryBoxSelectCustomerCountry.Value,
                Page2.EntryBoxSelectCustomerFiscalNumber.EntryValidation.Text,
                Page2.EntryBoxSelectCustomerCardNumber.EntryValidation.Text,
                LogicPOS.Utility.DataConversionUtils.StringToDecimal(Page2.EntryBoxCustomerDiscount.EntryValidation.Text),
                Page2.EntryBoxCustomerNotes.EntryValidation.Text
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

                    var response = new CustomAlert(WindowSettings.Source)
                                            .WithMessageType(MessageType.Warning)
                                            .WithButtonsType(ButtonsType.Close)
                                            .WithTitleResource("window_title_dialog_exception_error")
                                            .WithMessage(ex.InnerException.Message)
                                            .ShowAlert();
                }
                customer = null;
            }

            //Construct ProcessFinanceDocumentParameter
            DocumentProcessingParameters result = new DocumentProcessingParameters(
              Page1.EntryBoxSelectDocumentFinanceType.Value.Oid, Page3.ArticleBag
            )
            {
                Customer = (customer != null) ? customer.Oid : Guid.Empty,
            };

            //PaymentConditions
            if (Page1.EntryBoxSelectConfigurationPaymentCondition.Value != null
                && Page1.EntryBoxSelectConfigurationPaymentCondition.Value.Oid != new Guid())
            {
                result.PaymentCondition = Page1.EntryBoxSelectConfigurationPaymentCondition.Value.Oid;
            }

            //PaymentMethod
            if (Page1.EntryBoxSelectConfigurationPaymentMethod.Value != null
                && Page1.EntryBoxSelectConfigurationPaymentMethod.Value.Oid != new Guid())
            {
                result.PaymentMethod = Page1.EntryBoxSelectConfigurationPaymentMethod.Value.Oid;
            }

            //TotalDelivery: If Money force TotalDelivery to be equal to TotalFinal
            if (result.PaymentMethod != null && result.PaymentMethod != new Guid())
            {
                fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)XPOSettings.Session.GetObjectByKey(typeof(fin_configurationpaymentmethod), result.PaymentMethod);
                if (paymentMethod.Token == "MONEY")
                {
                    result.TotalDelivery = Page3.ArticleBag.TotalFinal;
                }
            }

            //Currency
            if (Page1.EntryBoxSelectConfigurationCurrency.Value.Oid != new Guid())
            {
                result.Currency = Page1.EntryBoxSelectConfigurationCurrency.Value.Oid;
                result.ExchangeRate = (Page1.EntryBoxSelectConfigurationCurrency.Value.ExchangeRate == 0) ? 1 : Page1.EntryBoxSelectConfigurationCurrency.Value.ExchangeRate;
            }

            //DocumentParent
            if (Page1.EntryBoxSelectSourceDocumentFinance.Value != null && Page1.EntryBoxSelectSourceDocumentFinance.Value.Oid != new Guid())
            {
                result.DocumentParent = Page1.EntryBoxSelectSourceDocumentFinance.Value.Oid;
            }

            //SourceMode
            result.SourceMode = PersistFinanceDocumentSourceMode.CustomArticleBag;

            //Not Used
            //SourceOrderMain : Dont have a OrderMain Source
            //FinanceDocuments
            //TotalChange

            //OrderReferences
            if (Page1.EntryBoxSelectSourceDocumentFinance.Value != null)
            {
                List<fin_documentfinancemaster> orderReferences = new List<fin_documentfinancemaster>
                {
                    Page1.EntryBoxSelectSourceDocumentFinance.Value
                };
                result.OrderReferences = orderReferences;
            }

            //If in WayBillMode Assign ShipTo and ShipFrom
            if (wayBillMode)
            {
                //ShipTo: Not Used : Address, BuildingNumber, StreetName
                result.ShipTo = new MovementOfGoodsProperties();
                if (Page4.EntryBoxShipToDeliveryID.EntryValidation.Text != string.Empty) result.ShipTo.DeliveryID = Page4.EntryBoxShipToDeliveryID.EntryValidation.Text;
                if (Page4.EntryBoxShipToDeliveryDate.EntryValidation.Text != string.Empty) result.ShipTo.DeliveryDate = Convert.ToDateTime(Page4.EntryBoxShipToDeliveryDate.EntryValidation.Text);
                if (Page4.EntryBoxShipToWarehouseID.EntryValidation.Text != string.Empty) result.ShipTo.WarehouseID = Page4.EntryBoxShipToWarehouseID.EntryValidation.Text;
                if (Page4.EntryBoxShipToLocationID.EntryValidation.Text != string.Empty) result.ShipTo.LocationID = Page4.EntryBoxShipToLocationID.EntryValidation.Text;
                if (Page4.EntryBoxShipToAddressDetail.EntryValidation.Text != string.Empty) result.ShipTo.AddressDetail = Page4.EntryBoxShipToAddressDetail.EntryValidation.Text;
                if (Page4.EntryBoxShipToCity.EntryValidation.Text != string.Empty) result.ShipTo.City = Page4.EntryBoxShipToCity.EntryValidation.Text;
                if (Page4.EntryBoxShipToPostalCode.EntryValidation.Text != string.Empty) result.ShipTo.PostalCode = Page4.EntryBoxShipToPostalCode.EntryValidation.Text;
                if (Page4.EntryBoxShipToRegion.EntryValidation.Text != string.Empty) result.ShipTo.Region = Page4.EntryBoxShipToRegion.EntryValidation.Text;
                if (Page4.EntryBoxSelectShipToCountry.Value.Oid != new Guid())
                {
                    result.ShipTo.Country = Page4.EntryBoxSelectShipToCountry.Value.Code2;
                    result.ShipTo.CountryGuid = Page4.EntryBoxSelectShipToCountry.Value.Oid;
                }
                //ShipFrom: Not Used : Address, BuildingNumber, StreetName
                result.ShipFrom = new MovementOfGoodsProperties();
                if (Page5.EntryBoxShipFromDeliveryID.EntryValidation.Text != string.Empty) result.ShipFrom.DeliveryID = Page5.EntryBoxShipFromDeliveryID.EntryValidation.Text;
                if (Page5.EntryBoxShipFromDeliveryDate.EntryValidation.Text != string.Empty) result.ShipFrom.DeliveryDate = Convert.ToDateTime(Page5.EntryBoxShipFromDeliveryDate.EntryValidation.Text);
                if (Page5.EntryBoxShipFromWarehouseID.EntryValidation.Text != string.Empty) result.ShipFrom.WarehouseID = Page5.EntryBoxShipFromWarehouseID.EntryValidation.Text;
                if (Page5.EntryBoxShipFromLocationID.EntryValidation.Text != string.Empty) result.ShipFrom.LocationID = Page5.EntryBoxShipFromLocationID.EntryValidation.Text;
                if (Page5.EntryBoxShipFromAddressDetail.EntryValidation.Text != string.Empty) result.ShipFrom.AddressDetail = Page5.EntryBoxShipFromAddressDetail.EntryValidation.Text;
                if (Page5.EntryBoxShipFromCity.EntryValidation.Text != string.Empty) result.ShipFrom.City = Page5.EntryBoxShipFromCity.EntryValidation.Text;
                if (Page5.EntryBoxShipFromPostalCode.EntryValidation.Text != string.Empty) result.ShipFrom.PostalCode = Page5.EntryBoxShipFromPostalCode.EntryValidation.Text;
                if (Page5.EntryBoxShipFromRegion.EntryValidation.Text != string.Empty) result.ShipFrom.Region = Page5.EntryBoxShipFromRegion.EntryValidation.Text;
                if (Page5.EntryBoxSelectShipFromCountry.Value.Oid != new Guid())
                {
                    result.ShipFrom.Country = Page5.EntryBoxSelectShipFromCountry.Value.Code2;
                    result.ShipFrom.CountryGuid = Page5.EntryBoxSelectShipFromCountry.Value.Oid;
                }
            }

            //Notes
            if (Page1.EntryBoxDocumentMasterNotes.EntryValidation.Text != string.Empty)
            {
                result.Notes = Page1.EntryBoxDocumentMasterNotes.EntryValidation.Text;
            }

            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            Page3.ArticleBag = GetArticleBag();

            return result;
        }

        private ResponseType ShowPreview(DocumentFinanceDialogPreviewMode pMode)
        {
            //Always Recreate ArticleBag before contruct ProcessFinanceDocumentParameter
            //_pagePad3.ArticleBag = GetArticleBag();            

            DocumentFinanceDialogPreview dialog = new DocumentFinanceDialogPreview(this, DialogFlags.DestroyWithParent, pMode, Page3.ArticleBag, Page1.EntryBoxSelectConfigurationCurrency.Value);
            ResponseType response = (ResponseType)dialog.Run();
            dialog.Destroy();
            return response;
        }

        //OnResponse Ok Post Validation, Last Validations before Procced to PersistFinanceDocument
        private bool LastValidation()
        {
            //Defaylt is true
            bool result = true;
            ArticleBag articleBag = GetArticleBag();

            //Protection to prevent Exceed Customer CardCredit
            if (
                //Can be null if not in a Payable DocumentType
                Page1.EntryBoxSelectConfigurationPaymentMethod.Value != null
                && Page1.EntryBoxSelectConfigurationPaymentMethod.Value.Token == "CUSTOMER_CARD"
                && articleBag.TotalFinal > Page2.EntryBoxSelectCustomerName.Value.CardCredit
            )
            {
                var message = string.Format(
                        CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "dialog_message_value_exceed_customer_card_credit"),
                        LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(Page2.EntryBoxSelectCustomerName.Value.CardCredit, XPOSettings.ConfigurationSystemCurrency.Acronym),
                        LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(articleBag.TotalFinal, XPOSettings.ConfigurationSystemCurrency.Acronym));

                var messageDialog = new CustomAlert(this)
                        .WithMessageType(MessageType.Error)
                        .WithButtonsType(ButtonsType.Ok)
                        .WithTitleResource("global_error")
                        .WithMessage(message)
                        .ShowAlert();

                result = false;
            }

            //Protection to Prevent Recharge Customer Card with Invalid User (User without Card or FinalConsumer...)
            if (result && !DocumentProcessingUtils.IsCustomerCardValidForArticleBag(articleBag, Page2.EntryBoxSelectCustomerName.Value))
            {
                var messageDialog = new CustomAlert(this)
                                    .WithMessageType(MessageType.Error)
                                    .WithButtonsType(ButtonsType.Ok)
                                    .WithTitleResource("global_error")
                                    .WithMessageResource("dialog_message_invalid_customer_card_detected")
                                    .ShowAlert();
                result = false;
            }


            return result;
        }
    }
}