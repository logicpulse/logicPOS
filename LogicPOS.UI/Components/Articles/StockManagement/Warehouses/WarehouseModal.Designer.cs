using System.Collections.Generic;
using System.Drawing;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.Warehouses;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WarehouseModal
    {
        public override Size ModalSize => new Size(500, 450);

        public override string ModalTitleResourceName => "global_warehouse";

        #region Components
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private CheckButton _checkDefaultWarehouse = new CheckButton(GeneralUtils.GetResourceByName("global_default_warehouse"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        private List<WarehouseLocationField> _locations = new List<WarehouseLocationField>();
        private VBox _boxLocations;
        #endregion

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtDesignation.Entry);
            SensitiveFields.Add(_txtNotes.TextView);
            SensitiveFields.Add(_checkDefaultWarehouse);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_boxLocations);

            foreach (var location in _locations)
            {
                SensitiveFields.Add(location.TxtLocation.Entry);
            }
        }

        protected override void AddValidatableFields()
        {

            switch (_modalMode)
            {
                case EntityEditionModalMode.Insert:
                    ValidatableFields.Add(_txtDesignation);
                    break;
                case EntityEditionModalMode.Update:
                    ValidatableFields.Add(_txtDesignation);
                    ValidatableFields.Add(_txtOrder);
                    ValidatableFields.Add(_txtCode);
                    break;
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateLocationsTab(), GeneralUtils.GetResourceByName("global_locations"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private ScrolledWindow CreateScrolledWindow()
        {
            var swindow = new ScrolledWindow();
            swindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            swindow.ModifyBg(StateType.Normal, Color.White.ToGdkColor());
            swindow.ShadowType = ShadowType.None;

            _boxLocations = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            swindow.AddWithViewport(_boxLocations);

            return swindow;
        }

        private VBox CreateDetailsTab()
        {
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }

            detailsTab.PackStart(_txtDesignation.Component, false, false, 0);
            detailsTab.PackStart(_checkDefaultWarehouse, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }

        private VBox CreateLocationsTab()
        {
            VBox locationsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            locationsTab.PackStart(CreateScrolledWindow(), true, true, 0);

            var addLocationButton = CreateAddLocationButton();
            locationsTab.PackEnd(addLocationButton, false, false, 0);

            return locationsTab;
        }

        private IconButton CreateAddLocationButton()
        {
            var button =  new IconButton(
                new ButtonSettings 
                {
                    Name = "touchButtonIcon",
                    Icon = $"{PathsSettings.ImagesFolderLocation}{@"Icons/icon_pos_nav_new.png"}",
                    IconSize = new Size(15, 15),
                    ButtonSize = new Size(20, 15)
                });

            button.Clicked += Button_AddLocation_Clicked;

            return button;
        }

        private void AddLocationField(WarehouseLocation location = null)
        {
            var locationField = new WarehouseLocationField(location);
            locationField.OnRemove += Button_RemoveLocation_Clicked;
            _boxLocations.PackStart(locationField.Component, false, true, 0);
            ValidatableFields.Add(locationField.TxtLocation);
            locationField.Component.ShowAll();
            _locations.Add(locationField);
        }
    }
}
