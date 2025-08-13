using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodField
    {
        private List<PaymentMethod> _paymentMethods;
        private List<PaymentMethod> PaymentMethodForCompletion => _paymentMethods ?? InitializePaymenthodsForCompletion();

        private List<PaymentMethod> InitializePaymenthodsForCompletion()
        {
            _paymentMethods=PaymentMethodsService.GetAllPaymentMethods();
            return _paymentMethods;
        }

        private void SelectPaymentMethod(PaymentMethod paymentMethod)
        {
            TxtPaymentMethod.SelectedEntity = paymentMethod;
        }
        private void InitializeButtons()
        {
            string iconClearRecord = $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{AppSettings.Paths.Images}{@"Icons/icon_pos_nav_new.png"}";

            BtnRemove = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Icon = iconClearRecord,
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(30, 30)
                });

            BtnAdd = new IconButton(
                new ButtonSettings
                {
                    Name = "buttonUserId",
                    Icon = iconAddRecord,
                    IconSize = new Size(20, 20),
                    ButtonSize = new Size(30, 30)
                });

            AddButtonsEventHandlers();
        }

        private void InitializeTxtPaymentMethod(Window sourceWindow)
        {
            TxtPaymentMethod = new TextBox(sourceWindow,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = true;
            var paymentMethods = PaymentMethodForCompletion.Select(p => (p as object, p.Designation)).ToList();
            TxtPaymentMethod.WithAutoCompletion(paymentMethods);
            TxtPaymentMethod.OnCompletionSelected += p => SelectPaymentMethod( p as PaymentMethod);
            TxtPaymentMethod.Entry.Changed += TxtPaymentMethod_Changed;
            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void InitializeTxtAmount(Window sourceWindow)
        {
            TxtAmount = new TextBox(sourceWindow,
                                        GeneralUtils.GetResourceByName("global_total_deliver"),
                                        isRequired: true,
                                        isValidatable: true,
                                        regex: RegularExpressions.Money,
                                        includeSelectButton: false,
                                        includeKeyBoardButton: true);

            TxtAmount.Entry.Changed += TxtAmount_Changed;
        }

    }
}
