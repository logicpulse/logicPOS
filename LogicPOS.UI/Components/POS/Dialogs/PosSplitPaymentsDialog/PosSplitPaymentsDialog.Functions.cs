using Gtk;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Finance.DocumentProcessing;
using LogicPOS.Globalization;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.Orders;
using LogicPOS.UI;
using LogicPOS.UI.Buttons;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosSplitPaymentsDialog
    {
        protected override void OnResponse(ResponseType pResponse)
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

        private void UpdateTouchButtonSplitPaymentLabels(SplitPaymentButton touchButtonSplitPayment)
        {
            string labelPaymentDetails = string.Empty;
            DocumentProcessingParameters processFinanceDocumentParameter = touchButtonSplitPayment.ProcessFinanceDocumentParameter;

            if (processFinanceDocumentParameter != null)
            {

                erp_customer customer = XPOUtility.GetEntityById<erp_customer>(processFinanceDocumentParameter.Customer);
                fin_configurationpaymentmethod paymentMethod = XPOUtility.GetEntityById<fin_configurationpaymentmethod>(processFinanceDocumentParameter.PaymentMethod);
                cfg_configurationcurrency currency = XPOUtility.GetEntityById<cfg_configurationcurrency>(processFinanceDocumentParameter.Currency);
                // Compose labelPaymentDetails
                string totalFinal = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(processFinanceDocumentParameter.ArticleBag.TotalFinal, currency.Acronym);
                string totalDelivery = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(processFinanceDocumentParameter.TotalDelivery, currency.Acronym);
                string totalChange = LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(processFinanceDocumentParameter.TotalChange, currency.Acronym);
                string moneyExtra = (paymentMethod.Token.Equals("MONEY")) ? $" : ({totalDelivery}/{totalChange})" : string.Empty;
                // Override default labelPaymentDetails
                labelPaymentDetails = $"{customer.Name} : {paymentMethod.Designation} : {totalFinal}{moneyExtra}";
            }
            // Assign to button Reference
            touchButtonSplitPayment.LabelPaymentDetails.Text = labelPaymentDetails;
        }

        private void CalculateSplit()
        {
            CalculateTotalPerSplit(_articleBag, _splitPaymentButtons.Count);
        }

        private void AddSplitButton(bool callUpdateAndCalculateMethods)
        {
            var buttonSettings = new ButtonSettings
            {
                Name = $"splitPaymentButton{_splitPaymentButtons.Count}",
                Text = $"Cliente #{_splitPaymentButtons.Count + 1}",
                BackgroundColor = _colorSplitPaymentTouchButtonFilledDataBackground,
                Font = _fontSplitPaymentTouchButtonSplitPayment,
                ButtonSize = new System.Drawing.Size(0, _intSplitPaymentTouchButtonSplitPaymentHeight)
            };

            SplitPaymentButton touchButtonSplitPayment = new SplitPaymentButton(buttonSettings);

            _splitPaymentButtons.Add(touchButtonSplitPayment);
            _vbox.PackStart(_splitPaymentButtons[_splitPaymentButtons.Count - 1], false, true, 5);
            // Event
            touchButtonSplitPayment.Clicked += TouchButtonSplitPayment_Clicked;
            // Check Label and Position
            // Call Update And Calculate Methods only on Last of PaymentStartClients
            if (callUpdateAndCalculateMethods)
            {
                // CalculateSplit;
                CalculateSplit();
                // UpdateActionButtons
                UpdateActionButtons();
            }
        }

        private void RemoveSplitButton()
        {

            // Store Index Position before Removes
            int lastIndex = _splitPaymentButtons.Count - 1;

            // Store Reference to Delete After remove From List
            SplitPaymentButton touchButtonSplitPayment = _splitPaymentButtons[lastIndex];
            _splitPaymentButtons.Remove(touchButtonSplitPayment);
            touchButtonSplitPayment.Destroy();
            // CalculateSplit;
            CalculateSplit();
            // UpdateActionButtons
            UpdateActionButtons();
        }

        private void CalculateTotalPerSplit(ArticleBag articleBag, int numberOfSplits)
        {
            bool debug = false;
            // Calculate final Total Pay per Split
            _totalPerSplit = articleBag.TotalFinal / numberOfSplits;


            // Always Init ArticleBags
            foreach (SplitPaymentButton item in _splitPaymentButtons)
            {
                item.ArticleBag = new ArticleBag();
            }

            // Init Object to Use priceTax on above Loop
            //Get PrinterType Objects to extract TaxSellType Normal|TakeWay, PrinterType, Tables etc
            OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);

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
                foreach (SplitPaymentButton touchButtonSplitPayment in _splitPaymentButtons)
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

                    // ArticleBagKey
                    ArticleBagKey articleBagKey = new ArticleBagKey(
                        article.Key.ArticleId,
                        article.Key.Designation,
                        article.Key.Price,
                        article.Key.Discount,
                        article.Key.Vat
                    );
                    //Detect and Assign VatExemptionReason to ArticleBak Key
                    if (article.Key.VatExemptionReasonId != null && article.Key.VatExemptionReasonId != Guid.Empty)
                    {
                        articleBagKey.VatExemptionReasonId = article.Key.VatExemptionReasonId;
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
            foreach (SplitPaymentButton item in _splitPaymentButtons)
            {
                // Require to Update ProcessFinanceDocumentParameter, like when we Close Payment Window, BEFORE UpdateTouchButtonSplitPaymentLabels
                // This is to Update UI when we Add/Remove Splits, else Already filled Payments dont Update
                // Only change ArticleBag
                if (item.ProcessFinanceDocumentParameter != null)
                {
                    fin_configurationpaymentmethod paymentMethod = XPOUtility.GetEntityById<fin_configurationpaymentmethod>(item.ProcessFinanceDocumentParameter.PaymentMethod);
                    decimal totalDelivery = (paymentMethod.Token.Equals("MONEY"))
                        ? item.ProcessFinanceDocumentParameter.TotalDelivery
                        : item.ArticleBag.TotalFinal;

                    item.ProcessFinanceDocumentParameter = new DocumentProcessingParameters(
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
                //if (WindowTitle != null) WindowTitle = string.Format(CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_split_payment, numberOfSplits, LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(totalFinal));
                if (WindowSettings.WindowTitle != null) WindowSettings.WindowTitle.Text = string.Format(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_split_payment"), numberOfSplits, LogicPOS.Utility.DataConversionUtils.DecimalToStringCurrency(_totalPerSplit, XPOSettings.ConfigurationSystemCurrency.Acronym));
            }

        }

        private bool PersistFinanceDocuments()
        {
            bool debug = false;
            int padLeftChars = 10;

            try
            {
                int i = 0;
                foreach (SplitPaymentButton item in _splitPaymentButtons)
                {
                    i++;
                    // If have 
                    if (item.ProcessFinanceDocumentParameter != null)
                    {

                        // PersistFinanceDocument
                        item.DocumentFinanceMaster = FrameworkCalls.PersistFinanceDocument(this, item.ProcessFinanceDocumentParameter);
                        //Update Display
                        if (item.DocumentFinanceMaster != null)
                        {
                            fin_configurationpaymentmethod paymentMethod = XPOUtility.GetEntityById<fin_configurationpaymentmethod>(item.ProcessFinanceDocumentParameter.PaymentMethod);
                            if (GlobalApp.UsbDisplay != null) GlobalApp.UsbDisplay.ShowPayment(paymentMethod.Designation, item.ProcessFinanceDocumentParameter.TotalDelivery, item.ProcessFinanceDocumentParameter.TotalChange);
                        }
                    }
                }

                //If has Working Order
                if (
                    POSSession.CurrentSession.OrderMains != null &&
                    POSSession.CurrentSession.CurrentOrderMainId != null &&
                    POSSession.CurrentSession.OrderMains.ContainsKey(POSSession.CurrentSession.CurrentOrderMainId)
                )
                {
                    // Get Current working orderMain
                    OrderMain currentOrderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                    //Get OrderDetail
                    OrderDetail currentOrderDetails = currentOrderMain.OrderTickets[currentOrderMain.CurrentTicketId].OrderDetails;

                    // Get configurationPlace to get Tax
                    pos_configurationplace configurationPlace = (pos_configurationplace)XPOSettings.Session.GetObjectByKey(typeof(pos_configurationplace), currentOrderMain.Table.PlaceId);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Events

        private void TouchButtonSplitPayment_Clicked(object sender, EventArgs e)
        {
            decimal checkTotal = 0.0m;

            SplitPaymentButton touchButtonSplitPayment = (SplitPaymentButton)sender;


            foreach (var item in touchButtonSplitPayment.ArticleBag)
            {
                checkTotal += item.Value.PriceFinal * item.Value.Quantity;
            }

            //Arredondamento de valores na divisão de contas gera perdas no valor e quantidade [IN:005944]
            //if (_articleBag.TotalFinal < checkTotal * _splitPaymentButtons.Count && touchButtonSplitPayment == _splitPaymentButtons[_splitPaymentButtons.Count - 1])
            //{
            //    var totalPSplit = Convert.ToDecimal(LogicPOS.Utility.DataConversionUtils.DecimalToString(checkTotal));
            //    var totalPTotal = Convert.ToDecimal(LogicPOS.Utility.DataConversionUtils.DecimalToString(_articleBag.TotalFinal));
            //    var missingRoundPayment = Convert.ToDecimal(LogicPOS.Utility.DataConversionUtils.DecimalToString(totalPTotal - (totalPSplit * _splitPaymentButtons.Count)));

            //    touchButtonSplitPayment.ArticleBag.TotalFinal += missingRoundPayment;
            //    //if(missingRoundPayment > 0)
            //    //{
            //    //    foreach (var item in touchButtonSplitPayment.ArticleBag)
            //    //    {
            //    //        item.Value.PriceFinal += (missingRoundPayment / _splitPaymentButtons.Count);
            //    //    }
            //    //}            
            //}

            // Using RequestProcessFinanceDocumentParameter, to prevent Emmit Document On Ok/Close
            PaymentDialog dialog = new PaymentDialog(WindowSettings.Source, DialogFlags.DestroyWithParent, touchButtonSplitPayment.ArticleBag, false, true, true, touchButtonSplitPayment.ProcessFinanceDocumentParameter, touchButtonSplitPayment.SelectedPaymentMethodButtonName);
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
    }
}