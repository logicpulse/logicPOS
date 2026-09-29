using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.POS.Devices.Printers.PrinterAssociation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;
using LogicPOS.Globalization;


namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleSubfamilyModal
    {
        protected override void Initialize()
        {
            InitializeCommissionGroupsComboBox();
            InitializeFamiliesComboBox();
            InitializeDiscountGroupsComboBox();
            InitializeVatOnTableComboBox();
            InitializeVatDirectSellingComboBox();
            InitializePrintersComboBox();

        }

        private void InitializePrintersComboBox()
        {
            var printers = GetPrinters();
            var labelText = LocalizedString.Instance["global_printers"];
            if (_entity != null)
            {
                var currentPrinter = PrinterAssociationService.GetEntityPrinter(_entity.Id);

                _comboPrinters = new EntityComboBox<Api.Entities.Printer>(labelText,
                                                                         printers,
                                                                         currentPrinter,
                                                                         false);
            }
            else
            {
                _comboPrinters = new EntityComboBox<Api.Entities.Printer>(labelText,
                                                         printers,
                                                         null,
                                                         false);
            }
        }

        private void InitializeCommissionGroupsComboBox()
        {
            var groups = GetCommissionGroups();
            var labelText = LocalizedString.Instance["global_commission_group"];
            var currentCommissionGroup = _entity != null ? _entity.CommissionGroup : null;

            _comboCommissionGroups = new EntityComboBox<CommissionGroup>(labelText,
                                                             groups,
                                                             currentCommissionGroup);
        }

        private void InitializeFamiliesComboBox()
        {
            var families = GetFamilies();
            var labelText = LocalizedString.Instance["global_families"];
            var currentFamily = _entity != null ? _entity.Family : null;

            _comboFamilies = new EntityComboBox<ArticleFamily>(labelText,
                                                             families,
                                                             currentFamily,
                                                             true);
        }

        private void InitializeDiscountGroupsComboBox()
        {
            var groups = GetDiscountGroups();
            var labelText = LocalizedString.Instance["global_discount_group"];
            var currentDiscountGroup = _entity != null ? _entity.DiscountGroup : null;

            _comboDiscountGroups = new EntityComboBox<DiscountGroup>(labelText,
                                                             groups,
                                                             currentDiscountGroup);
        }

        private void InitializeVatOnTableComboBox()
        {
            var vatRates = GetVatRates();
            var labelText = LocalizedString.Instance["global_vat_on_table"];
            var currentVatRate = _entity != null ? _entity.VatOnTable : null;

            _comboVatOnTable = new EntityComboBox<VatRate>(labelText,
                                                             vatRates,
                                                             currentVatRate);
        }

        private void InitializeVatDirectSellingComboBox()
        {
            var vatRates = GetVatRates();
            var labelText = LocalizedString.Instance["global_vat_direct_selling"];
            var currentVatRate = _entity != null ? _entity.VatDirectSelling : null;

            _comboVatDirectSelling = new EntityComboBox<VatRate>(labelText,
                                                             vatRates,
                                                             currentVatRate);
        }

    }
}
