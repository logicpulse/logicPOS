using Gtk;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents.CreateDocument.Fields
{
    public partial class DocumentPaymentMethodField
    {
        private void InitializeButtons()
        {
            string iconClearRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}";

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

            TxtPaymentMethod.Entry.IsEditable = false;

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
