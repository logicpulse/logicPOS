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

        public event System.Action<WarehouseLocationField, WarehouseLocation> OnRemove;

        private readonly IconButton BtnRemove = new IconButton(new ButtonSettings
        {
            Name = "touchButtonIcon",
            Icon = $"{AppSettings.Paths.Images}{@"Icons/Windows/icon_window_delete_record.png"}",
            BackgroundColor = Color.LightGray,
            IconSize = new Size(15, 15),
            ButtonSize = new Size(30, 15)
        });

        private readonly IconButton BtnSetDefault = new IconButton(new ButtonSettings
        {
            Name = "touchButtonIcon",
            Icon = $"{AppSettings.Paths.Images}{@"Icons/icon_pos_configuration.png"}",
            BackgroundColor = Color.LightGray,
            IconSize = new Size(15, 15),
            ButtonSize = new Size(30, 15)
        });

        public HBox Component { get; }

        public WarehouseLocationField(WarehouseLocation location = null)
        {
            Component = new HBox(false, 5);
            var radioButton = new CheckButton
            {
                Sensitive = false,
                Active = location?.IsDefault ?? false,
            };

            Component.PackStart(radioButton, false, false, 1);
            Component.PackStart(TxtLocation.Component);

            if(location == null || location.IsDefault == false)
            {
                Component.PackEnd(BtnRemove, false, false, 1);
            }     

            if (location != null && location.IsDefault == false)
            {
                Component.PackEnd(BtnSetDefault, false, false, 1);
            }

            Location = location;
            TxtLocation.Text = location?.Designation;
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemove?.Invoke(this, Location);
        }

    }
}
