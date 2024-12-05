using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents.Documents.CreateDocument.Fields
{
    public class DocumentPaymentMethodField : IValidatableField
    {
        public IconButton BtnRemove { get; set; }
        public IconButton BtnAdd { get; set; }
        public PageTextBox TxtPaymentMethod { get; set; }
        public PageTextBox TxtAmount { get; set; }
        public Widget Component { get; private set; }
        public string FieldName => TxtPaymentMethod.Label.Text;

        public event Action<DocumentPaymentMethodField> OnRemove;
        public event System.Action OnAdd;

        public DocumentPaymentMethodField(Window sourceWindow)
        {
            InitializeTxtPaymentMethod(sourceWindow);
            InitializeTxtAmount(sourceWindow);
            InitializeButtons();

            Component = CreateComponent();
        }

        private Widget CreateComponent()
        {
            var hbox = new HBox(false, 0);
            hbox.PackStart(TxtPaymentMethod.Component, true, true, 0);
            hbox.PackStart(TxtAmount.Component, true, true, 0);

            var buttonsBox = new HBox(false, 2);
            buttonsBox.PackStart(BtnAdd, false, true, 0);
            buttonsBox.PackStart(BtnRemove, false, true, 0);

            hbox.PackStart(buttonsBox, false, false, 0);
            return hbox;
        }

        private void InitializeButtons()
        {
            string iconClearRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}";
            string iconAddRecord = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}";

            BtnRemove = new IconButton(
                new ButtonSettings { 
                    Name = "buttonUserId", 
                    Icon = iconClearRecord, 
                    IconSize = new Size(20, 20), 
                    ButtonSize = new Size(30, 30) });

            BtnAdd = new IconButton(
                new ButtonSettings { 
                    Name = "buttonUserId", 
                    Icon = iconAddRecord, 
                    IconSize = new Size(20, 20), 
                    ButtonSize = new Size(30, 30) });

            AddButtonsEventHandlers();
        }

        private void InitializeTxtPaymentMethod(Window sourceWindow)
        {
            TxtPaymentMethod = new PageTextBox(sourceWindow,
                                               GeneralUtils.GetResourceByName("global_payment_method"),
                                               isRequired: true,
                                               isValidatable: false,
                                               includeSelectButton: true,
                                               includeKeyBoardButton: false);

            TxtPaymentMethod.Entry.IsEditable = false;

            TxtPaymentMethod.SelectEntityClicked += BtnSelectPaymentMethod_Clicked;
        }

        private void BtnSelectPaymentMethod_Clicked(object sender, EventArgs e)
        {
            var page = new PaymentMethodsPage(null, PageOptions.SelectionPageOptions);
            var selectPaymentMethodModal = new EntitySelectionModal<PaymentMethod>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectPaymentMethodModal.Run();
            selectPaymentMethodModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtPaymentMethod.Text = page.SelectedEntity.Designation;
                TxtPaymentMethod.SelectedEntity = page.SelectedEntity;
            }
        }

        private void InitializeTxtAmount(Window sourceWindow)
        {
            TxtAmount = new PageTextBox(sourceWindow,
                                        GeneralUtils.GetResourceByName("global_total_deliver"),
                                        isRequired: true,
                                        isValidatable: true,
                                        regex: RegularExpressions.Money,
                                        includeSelectButton: false,
                                        includeKeyBoardButton: true);

            TxtAmount.Entry.Changed += TxtAmount_Changed;
        }

        private void TxtAmount_Changed(object sender, EventArgs e)
        {

        }

        private void AddButtonsEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this);
            BtnAdd.Clicked += (s, e) => OnAdd?.Invoke();
        }

        public bool IsValid()
        {
            return TxtPaymentMethod.IsValid() && TxtAmount.IsValid();
        }
    }
}
