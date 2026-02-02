
using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateStockMovementModal
    {
        public override Size ModalSize => new Size(450, 520);
        public override string ModalTitleResourceName => "global_stock_movement";

        #region Components
        private TextBox TxtSupplier { get; set; }
        private TextBox TxtDate { get; set; }
        private TextBox TxtDocumnetNumber { get; set; }
        private TextBox TxtQuantity { get; set; } = TextBox.Simple("global_quantity", true, true, RegularExpressions.Quantity);
        private TextBox TxtPrice { get; set; } = TextBox.Simple("global_price", true, true, RegularExpressions.NullableMoney);
        #endregion

        protected override void Initialize()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            InitializeTxtSupplier();
            InitializeTxtDate();
            InitializeTxtDocumnetNumber();
        }

        private void InitializeTxtSupplier()
        {
            TxtSupplier = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_supplier"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false,
                                          style: TextBoxStyle.Lite,
                                          includeClearButton: false);

            TxtSupplier.Entry.IsEditable = false;

            TxtSupplier.SelectEntityClicked += BtnSelectSupplier_Clicked;

            ValidatableFields.Add(TxtSupplier);
        }

        private void InitializeTxtDocumnetNumber()
        {
            TxtDocumnetNumber = new TextBox(this,
                                            GeneralUtils.GetResourceByName("global_document_number"),
                                            isRequired: false,
                                            isValidatable: false,
                                            includeSelectButton: true,
                                            includeKeyBoardButton: false,
                                            style: TextBoxStyle.Lite,
                                            includeClearButton: false);

            TxtDocumnetNumber.SelectEntityClicked += BtnSelectDocumentNumber_Clicked;
        }

        private void InitializeTxtDate()
        {
            TxtDate = new TextBox(this,
                                      GeneralUtils.GetResourceByName("global_date"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: true,
                                      includeKeyBoardButton: false,
                                      style: TextBoxStyle.Lite,
                                      includeClearButton: false);

            TxtDate.Entry.IsEditable = false;
            TxtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            TxtDate.SelectEntityClicked += TxtDate_SelectEntityClicked;
        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(TxtSupplier);
            ValidatableFields.Add(TxtDate);
            ValidatableFields.Add(TxtQuantity);
            ValidatableFields.Add(TxtPrice);
        }

        protected override void AddSensitiveFields() { }


        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            var tab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            tab.PackStart(TxtSupplier.Component, false, false, 0);
            tab.PackStart(TxtDate.Component, false, false, 0);
            tab.PackStart(TxtDocumnetNumber.Component, false, false, 0);
            tab.PackStart(TxtQuantity.Component, false, false, 0);
            tab.PackStart(TxtPrice.Component, false, false, 0);

            yield return (tab, GeneralUtils.GetResourceByName("Movimento de Entrada"));
        }
    }
}
