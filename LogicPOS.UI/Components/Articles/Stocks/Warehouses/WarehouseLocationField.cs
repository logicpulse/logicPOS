using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Settings;
using System.Drawing;

namespace LogicPOS.UI.Components.Warehouses
{
    public class WarehouseLocationField
    {
        public WarehouseLocation Location { get; set; }

        public TextBox TxtLocation { get; } = TextBox.Simple("global_ConfigurationDevice_PlaceTerminal", isRequired: true);

        public event global::System.Action<WarehouseLocationField, WarehouseLocation> BtnRemoveClicked;
        public event global::System.Action<WarehouseLocation> BtnSetDefaultClicked;

        private readonly IconButton BtnRemove = new IconButton(new ButtonSettings
        {
            Name = "touchButton_Red",
            Icon = $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_delete_record.png"}",
            IconSize = new Size(15, 15),
            ButtonSize = new Size(30, 15)
        });

        private readonly IconButton BtnSetDefault = new IconButton(new ButtonSettings
        {
            Name = "touchButtonOk_DialogActionArea",
            Icon = $"{AppSettings.Paths.Images}{@"Icons/icon_pos_toolbar_system.png"}",
            IconSize = new Size(15, 15),
            ButtonSize = new Size(30, 15)
        });

        private readonly CheckButton CheckBtnIsDefault = new CheckButton { Sensitive = false };

        public HBox Component { get; }

        public WarehouseLocationField(WarehouseLocation location = null)
        {
            Component = new HBox(false, 5);

            Component.PackStart(CheckBtnIsDefault, false, false, 1);
            Component.PackStart(TxtLocation.Component);

            if(location != null)
            {
                Component.PackEnd(BtnRemove, false, false, 1);
                Component.PackEnd(BtnSetDefault, false, false, 1);
            }     

            Location = location;
            TxtLocation.Text = location?.Designation;
            AddEventHandlers();

            UpdateUI();
        }

        public void UpdateUI()
        {
            if( Location == null)
            {
                return;
            }

            CheckBtnIsDefault.Active = Location.IsDefault;
            BtnRemove.Sensitive = !Location.IsDefault;
            BtnSetDefault.Sensitive = !Location.IsDefault;
        }

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => BtnRemoveClicked?.Invoke(this, Location);
            BtnSetDefault.Clicked += (s, e) => BtnSetDefaultClicked?.Invoke(Location);
        }

    }
}
