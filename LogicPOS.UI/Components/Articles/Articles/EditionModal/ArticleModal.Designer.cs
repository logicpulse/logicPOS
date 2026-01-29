using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.Utility;
using Pango;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        public override Size ModalSize => new Size(500, 740);
        public override string ModalTitleResourceName => "window_title_edit_article";

        protected override void Initialize()
        {
            InitializeFields();

            _checkUniqueArticles.Sensitive = false;

            AddEventHandlers();
        }


        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateFinanceDetailsTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab2_label"));
            yield return (CreateOtherDetailsTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab3_label"));
            yield return (CreateCompositionTab(), GeneralUtils.GetResourceByName("dialog_edit_article_tab4_label1"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));

            if (_entity != null)
            {
                yield return (CreateUniqueArticlesTab(), GeneralUtils.GetResourceByName("global_serial_number"));
            }

        }

        private VBox CreateDetailsTab()
        {
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
            }
                detailsTab.PackStart(_txtCode.Component, false, false, 0);

            detailsTab.PackStart(_txtCodeDealer.Component, false, false, 0);
            detailsTab.PackStart(_txtDesignation.Component, false, false, 0);
            detailsTab.PackStart(_txtButtonName.Component, false, false, 0);
            detailsTab.PackStart(_comboFamilies.Component, false, false, 0);
            detailsTab.PackStart(_comboSubfamilies.Component, false, false, 0);
            detailsTab.PackStart(_comboTypes.Component, false, false, 0);
            detailsTab.PackStart(_imagePicker.Component, false, false, 0);
            detailsTab.PackStart(_checkIsComposed, false, false, 0);
            detailsTab.PackStart(_checkUniqueArticles, false, false, 0);
            detailsTab.PackStart(_checkFavorite, false, false, 0);
            detailsTab.PackStart(_checkUseWeighingBalance, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }

        private VBox CreateCompositionTab()
        {
            _compositionTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            _compositionTab.PackStart(_addArticlesBox.Component, true, true, 0);
            return _compositionTab;
        }

        private VBox CreateFinanceDetailsTab()
        {
            var financeDetailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            financeDetailsTab.PackStart(CreatePricesArea(), false, false, 0);
            financeDetailsTab.PackStart(_checkPVPVariable, false, false, 0);
            financeDetailsTab.PackStart(_checkPriceWithVat, false, false, 0);
            financeDetailsTab.PackStart(_txtDiscount.Component, false, false, 0);
            financeDetailsTab.PackStart(_comboClasses.Component, false, false, 0);
            financeDetailsTab.PackStart(_comboVatDirectSelling.Component, false, false, 0);
            financeDetailsTab.PackStart(_comboVatExemptionReasons.Component, false, false, 0);

            return financeDetailsTab;
        }

        private VBox CreateOtherDetailsTab()
        {
            var otherDetailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            otherDetailsTab.PackStart(_txtBarcode.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtMinimumStock.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtTare.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtWeight.Component, false, false, 0);
            otherDetailsTab.PackStart(_txtDefaultQuantity.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboMeasurementUnits.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboSizeUnits.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboCommissionGroups.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboDiscountGroups.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboPrinters.Component, false, false, 0);
            otherDetailsTab.PackStart(_comboPrintModels.Component, false, false, 0);

            return otherDetailsTab;
        }

        private VBox CreatePricesArea()
        {
            int[] columnsWidths = new int[] { 100, 90, 90, 160 };
            var vbox = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            Label labelEmpty = new Label(string.Empty) { WidthRequest = columnsWidths[0] };
            Label labelNormal = new Label(GeneralUtils.GetResourceByName("article_normal_price")) { WidthRequest = columnsWidths[1] };
            Label labelPromotion = new Label(GeneralUtils.GetResourceByName("article_promotion_price")) { WidthRequest = columnsWidths[2] };
            Label labelUsePromotion = new Label(GeneralUtils.GetResourceByName("article_use_promotion_price")) { WidthRequest = columnsWidths[3] };
            labelNormal.SetAlignment(0.0F, 0.5F);
            labelPromotion.SetAlignment(0.0F, 0.5F);
            labelUsePromotion.SetAlignment(0.0F, 0.5F);

            //Header
            HBox header = new HBox(false, _boxSpacing);
            header.PackStart(labelEmpty, true, true, 0);
            header.PackStart(labelNormal, false, false, 0);
            header.PackStart(labelPromotion, false, false, 0);
            header.PackStart(labelUsePromotion, false, false, 0);
            vbox.PackStart(header, false, false, 0);

            //Prices
            foreach (var price in _prices)
            {
                vbox.PackStart(price.Component, false, false, 0);
            }

            return vbox;
        }

        private VBox CreateUniqueArticlesTab()
        {
            var vbox = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };
            vbox.PackStart(new UniqueArticleFieldsContainer(_entity.Id).Component, true, true, 0);
            return vbox;
        }
    }
}
