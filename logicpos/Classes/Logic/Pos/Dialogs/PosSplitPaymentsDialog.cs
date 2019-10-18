using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Logic.Others;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Finance;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    partial class PosSplitPaymentsDialog
    {
        protected override void OnResponse(ResponseType pResponse)
        {
            try
            {
                //ResponseTypeCancel
                if (pResponse == ResponseType.Cancel || pResponse == ResponseType.DeleteEvent)
                {
                    this.Destroy();
                }
                //ResponseTypeOk
                else if (pResponse == ResponseType.Ok)
                {
                    // Stub
                    if (!PersistFinanceDocuments())
                    {
                        //Keep Running
                        this.Run();
                    }
                }
                else if (pResponse == _responseTypeRemoveSplit)
                {
                    RemoveSplitButton();
                    //Keep Running
                    this.Run();
                }
                else if (pResponse == _responseTypeAddSplit)
                {
                    AddSplitButton(true);
                    //Keep Running
                    this.Run();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void UpdateActionButtons()
        {
            _buttonTableRemoveSplit.Sensitive = (_splitPaymentButtons.Count > _intSplitPaymentMinClients);
            _buttonTableAddSplit.Sensitive = (_splitPaymentButtons.Count < _intSplitPaymentMaxClients);

            // Default is Enabled
            bool enableOk = true;

            // Require to have intSplitPaymentMinClients to with ProcessFinanceDocumentParameter filled : NOTE: Now we can Persist Document
            for (int i = 0; i < 1/*_intSplitPaymentMinClients*/; i++)
            {
                // Override default enabled to false
                if (_splitPaymentButtons[i].ProcessFinanceDocumentParameter == null)
                {
                    enableOk = false;
                }
            }

            // Require TotalChange greater than Zero
            for (int i = 0; i < _splitPaymentButtons.Count; i++)
            {
                // Override default enabled to false
                if (_splitPaymentButtons[i].ProcessFinanceDocumentParameter != null && _splitPaymentButtons[i].ProcessFinanceDocumentParameter.TotalChange < 0)
                {
                    enableOk = false;
                }
            }

            // Assign Sensitive
            _buttonOk.Sensitive = (enableOk);
        }

        private void UpdateTouchButtonSplitPaymentLabels(TouchButtonSplitPayment touchButtonSplitPayment)
        {
            bool debug = false;

            try
            {
                string labelPaymentDetails = string.Empty;
                ProcessFinanceDocumentParameter processFinanceDocumentParameter = touchButtonSplitPayment.ProcessFinanceDocumentParameter;

                if (processFinanceDocumentParameter != null)
                {
                    if (debug) _log.Debug(Environment.NewLine);
                    foreach (var item in touchButtonSplitPayment.ArticleBag)
                    {
                        if (debug) _log.Debug(string.Format("\t[{0}],[{1}],[{2}]", item.Key.Designation, item.Value.Quantity, item.Value.TotalFinal));
                    }

                    erp_customer customer = (erp_customer)FrameworkUtils.GetXPGuidObject(typeof(erp_customer), processFinanceDocumentParameter.Customer);
                    fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationpaymentmethod), processFinanceDocumentParameter.PaymentMethod);
                    cfg_configurationcurrency currency = (cfg_configurationcurrency)FrameworkUtils.GetXPGuidObject(typeof(cfg_configurationcurrency), processFinanceDocumentParameter.Currency);
                    // Compose labelPaymentDetails
                    string totalFinal = FrameworkUtils.DecimalToStringCurrency(processFinanceDocumentParameter.ArticleBag.TotalFinal, currency.Acronym);
                    string totalDelivery = FrameworkUtils.DecimalToStringCurrency(processFinanceDocumentParameter.TotalDelivery, currency.Acronym);
                    string totalChange = FrameworkUtils.DecimalToStringCurrency(processFinanceDocumentParameter.TotalChange, currency.Acronym);
                    string moneyExtra = (paymentMethod.Token.Equals("MONEY")) ? $" : ({totalDelivery}/{totalChange})" : string.Empty;
                    // Override default labelPaymentDetails
                    labelPaymentDetails = $"{customer.Name} : {paymentMethod.Designation} : {totalFinal}{moneyExtra}";
                }
                // Assign to button Reference
                touchButtonSplitPayment.LabelPaymentDetails.Text = labelPaymentDetails;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void CalculateSplit()
        {
            CalculateTotalPerSplit(_articleBag, _splitPaymentButtons.Count);
        }

        private void AddSplitButton(bool callUpdateAndCalculateMethods)
        {
            bool debug = false;

            try
            {
                string buttonLabel = $"Cliente #{_splitPaymentButtons.Count + 1}";
                TouchButtonSplitPayment touchButtonSplitPayment = new TouchButtonSplitPayment($"splitPaymentButton{_splitPaymentButtons.Count}", _colorSplitPaymentTouchButtonFilledDataBackground, buttonLabel, _fontSplitPaymentTouchButtonSplitPayment, 0, _intSplitPaymentTouchButtonSplitPaymentHeight);
                _splitPaymentButtons.Add(touchButtonSplitPayment);
                _vbox.PackStart(_splitPaymentButtons[_splitPaymentButtons.Count - 1], false, true, 5);
                // Event
                touchButtonSplitPayment.Clicked += TouchButtonSplitPayment_Clicked;
                // Check Label and Position
                if (debug) _log.Debug(string.Format("AddSplitButton: [{0}]", _splitPaymentButtons[_splitPaymentButtons.Count - 1].LabelText));
                // Call Update And Calculate Methods only on Last of PaymentStartClients
                if (callUpdateAndCalculateMethods)
                {
                    // CalculateSplit;
                    CalculateSplit();
                    // UpdateActionButtons
                    UpdateActionButtons();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void RemoveSplitButton()
        {
            bool debug = false;

            try
            {
                // Store Index Position before Removes
                int lastIndex = _splitPaymentButtons.Count - 1;
                if (debug) _log.Debug(string.Format("RemoveSplitButton: [{0}]", _splitPaymentButtons[lastIndex].LabelText));
                // Store Reference to Delete After remove From List
                TouchButtonSplitPayment touchButtonSplitPayment = _splitPaymentButtons[lastIndex];
                _splitPaymentButtons.Remove(touchButtonSplitPayment);
                touchButtonSplitPayment.Destroy();
                // CalculateSplit;
                CalculateSplit();
                // UpdateActionButtons
                UpdateActionButtons();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private void CalculateTotalPerSplit(ArticleBag articleBag, int numberOfSplits)
        {
            bool debug = false;
            // Calculate final Total Pay per Split
            _totalPerSplit = articleBag.TotalFinal / numberOfSplits;

            try
            {
                // Always Init ArticleBags
                foreach (TouchButtonSplitPayment item in _splitPaymentButtons)
                {
                    item.ArticleBag = new ArticleBag();
                }

                // Init Object to Use priceTax on above Loop
                //Get Place Objects to extract TaxSellType Normal|TakeWay, Place, Tables etc
                OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                pos_configurationplace configurationPlace = (pos_configurationplace)GlobalFramework.SessionXpo.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);

                // Loop articleBag, and Add the quantity for Each Split (Total Article Quantity / numberOfSplits)
                foreach (var article in articleBag)
                {
                    // Default quantity to add to all Splitters, last one gets the extra Remains ex 0,0000000000001
                    decimal articleQuantity = (article.Value.Quantity / numberOfSplits);
                    // Store Remain Quantity
                    decimal articleQuantityRemain = article.Value.Quantity;
                    // Check if Total is equal to Origin
                    decimal articleQuantityCheck = 0.0m;
                    decimal articleQuantityCheckModulo = 0.0m;
                    // Reset t
                    int t = 0;
                    foreach (TouchButtonSplitPayment touchButtonSplitPayment in _splitPaymentButtons)
                    {
                        t++;
                        // Discount articleQuantityRemain
                        articleQuantityRemain = articleQuantityRemain - articleQuantity;
                        if (t.Equals(_splitPaymentButtons.Count))
                        {
                            // Override Default split Quantity, adding extra Remain
                            articleQuantity += articleQuantityRemain;
                        }

                        // Add to articleQuantityCheck
                        articleQuantityCheck += articleQuantity;
                        // Modulo
                        articleQuantityCheckModulo = article.Value.Quantity % articleQuantityCheck;

                        if (debug) _log.Debug(string.Format("#{0} Designation: [{1}], PriceFinal: [{2}], Quantity: [{3}]:[{4}]:[{5}]:[{6}]:[{7}]",
                            t, article.Key.Designation, article.Value.PriceFinal, article.Value.Quantity, articleQuantity, articleQuantityRemain, articleQuantityCheck, articleQuantityCheckModulo)
                            );

                        // ArticleBagKey
                        ArticleBagKey articleBagKey = new ArticleBagKey(
                            article.Key.ArticleOid,
                            article.Key.Designation,
                            article.Key.Price,
                            article.Key.Discount,
                            article.Key.Vat
                        );
                        //Detect and Assign VatExemptionReason to ArticleBak Key
                        if (article.Key.VatExemptionReasonOid != null && article.Key.VatExemptionReasonOid != Guid.Empty)
                        {
                            articleBagKey.VatExemptionReasonOid = article.Key.VatExemptionReasonOid;
                        }
                        // ArticleBagProperties
                        ArticleBagProperties articleBagProps = articleBagProps = new ArticleBagProperties(
                          configurationPlace.Oid,
                          currentOrderMain.Table.Oid,
                          (PriceType)configurationPlace.PriceType.EnumValue,
                          article.Value.Code,
                          articleQuantity,
                          article.Value.UnitMeasure
                        );

                        // Add to ArticleBag
                        touchButtonSplitPayment.ArticleBag.Add(articleBagKey, articleBagProps);
                    }
                }

                // After have all splitPaymentButtons ArticleBags (End of arraySplit.Count Loop)
                foreach (TouchButtonSplitPayment item in _splitPaymentButtons)
                {
                    // Require to Update ProcessFinanceDocumentParameter, like when we Close Payment Window, BEFORE UpdateTouchButtonSplitPaymentLabels
                    // This is to Update UI when we Add/Remove Splits, else Already filled Payments dont Update
                    // Only change ArticleBag
                    if (item.ProcessFinanceDocumentParameter != null)
                    {
                        fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationpaymentmethod), item.ProcessFinanceDocumentParameter.PaymentMethod);
                        decimal totalDelivery = (paymentMethod.Token.Equals("MONEY"))
                            ? item.ProcessFinanceDocumentParameter.TotalDelivery
                            : item.ArticleBag.TotalFinal;

                        item.ProcessFinanceDocumentParameter = new ProcessFinanceDocumentParameter(
                            item.ProcessFinanceDocumentParameter.DocumentType, item.ArticleBag
                        )
                        {
                            PaymentMethod = item.ProcessFinanceDocumentParameter.PaymentMethod,
                            PaymentCondition = item.ProcessFinanceDocumentParameter.PaymentCondition,
                            Customer = item.ProcessFinanceDocumentParameter.Customer,
                            TotalDelivery = totalDelivery,
                            // Require to Recalculate TotalChange
                            TotalChange = totalDelivery - item.ArticleBag.TotalFinal
                        };
                    }

                    // Always Update all Buttons, with and without ProcessFinanceDocumentParameter
                    UpdateTouchButtonSplitPaymentLabels(item);

                    // Update Window Title
                    //if (WindowTitle != null) WindowTitle = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_split_payment, numberOfSplits, FrameworkUtils.DecimalToStringCurrency(totalFinal));
                    if (WindowTitle != null) WindowTitle = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_split_payment"), numberOfSplits, FrameworkUtils.DecimalToStringCurrency(_totalPerSplit));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        private decimal CalculateTotal(ArrayList arrayList, bool showLog = false)
        {
            decimal result = 0;

            foreach (SplitPaymentArticleComparable item in arrayList)
            {
                result += item.priceFinal;
                //if (showLog) _log.Debug(string.Format("\t{0}\t{1}\t{2}", result, item.designation, item.price));
            }

            return result;
        }

        private bool PersistFinanceDocuments()
        {
            bool debug = false;
            int padLeftChars = 10;

            try
            {
                int i = 0;
                foreach (TouchButtonSplitPayment item in _splitPaymentButtons)
                {
                    i++;
                    // If have 
                    if (item.ProcessFinanceDocumentParameter != null)
                    {
                        if (debug) _log.Debug(string.Format("TotalFinal: [#{0}]:[{1}]", i,
                            FrameworkUtils.DecimalToStringCurrency(item.ProcessFinanceDocumentParameter.ArticleBag.TotalFinal).PadLeft(padLeftChars, ' ')
                        ));

                        // PersistFinanceDocument
                        item.DocumentFinanceMaster = FrameworkCalls.PersistFinanceDocument(this, item.ProcessFinanceDocumentParameter);
                        //Update Display
                        if (item.DocumentFinanceMaster != null)
                        {
                            fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)FrameworkUtils.GetXPGuidObject(typeof(fin_configurationpaymentmethod), item.ProcessFinanceDocumentParameter.PaymentMethod);
                            if (GlobalApp.UsbDisplay != null) GlobalApp.UsbDisplay.ShowPayment(paymentMethod.Designation, item.ProcessFinanceDocumentParameter.TotalDelivery, item.ProcessFinanceDocumentParameter.TotalChange);
                        }
                    }
                }

                //If has Working Order
                if (
                    GlobalFramework.SessionApp.OrdersMain != null &&
                    GlobalFramework.SessionApp.CurrentOrderMainOid != null &&
                    GlobalFramework.SessionApp.OrdersMain.ContainsKey(GlobalFramework.SessionApp.CurrentOrderMainOid)
                )
                {
                    // Get Current working orderMain
                    OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                    if (debug) _log.Debug(string.Format("Working on currentOrderMain.PersistentOid: [{0}]", currentOrderMain.PersistentOid));
                    //Get OrderDetail
                    OrderDetail currentOrderDetails = currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId].OrderDetails;

                    // Get configurationPlace to get Tax
                    pos_configurationplace configurationPlace = (pos_configurationplace)GlobalFramework.SessionXpo.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Events

        private void TouchButtonSplitPayment_Clicked(object sender, EventArgs e)
        {
            bool debug = false;
            decimal checkTotal = 0.0m;

            try
            {
                TouchButtonSplitPayment touchButtonSplitPayment = (TouchButtonSplitPayment)sender;
                if (debug) _log.Debug(string.Format("TouchButtonSplitPayment Clicked: [{0}]", touchButtonSplitPayment.LabelText));

                foreach (var item in touchButtonSplitPayment.ArticleBag)
                {
                    checkTotal += item.Value.PriceFinal * item.Value.Quantity;
                }
                if (debug) _log.Debug(string.Format("touchButtonSplitPayment.ArticleBag: Total: [{0}]", checkTotal));

                // Using RequestProcessFinanceDocumentParameter, to prevent Emmit Document On Ok/Close
                PosPaymentsDialog dialog = new PosPaymentsDialog(_sourceWindow, DialogFlags.DestroyWithParent, touchButtonSplitPayment.ArticleBag, false, true, true, touchButtonSplitPayment.ProcessFinanceDocumentParameter, touchButtonSplitPayment.SelectedPaymentMethodButtonName);
                int response = dialog.Run();

                if (response == (int)ResponseType.Ok)
                {
                    // Assign ProcessFinanceDocumentParameter (Other dialog.TotalChange | dialog.TotalDelivery)
                    touchButtonSplitPayment.ProcessFinanceDocumentParameter = dialog.ProcessFinanceDocumentParameter;
                    // Store SelectedPaymentMethodButtonName to init Dialog with corrected Button Toggled
                    touchButtonSplitPayment.SelectedPaymentMethodButtonName = dialog.SelectedPaymentMethodButton.Name;
                    // Call UpdateTouchButtonSplitPaymentLabels
                    UpdateTouchButtonSplitPaymentLabels(touchButtonSplitPayment);
                    // UpdateActionButtons
                    UpdateActionButtons();

                    // Valid Result Destroy Dialog
                    dialog.Destroy();
                };
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }
    }
}