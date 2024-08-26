using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using System.Drawing;

namespace LogicPOS.UI.Components.Warehouses
{
    public class WarehouseLocationField
    {
        public WarehouseLocation Location { get; set; }

        public TextBox TxtLocation { get; } =  new TextBox("global_ConfigurationDevice_PlaceTerminal", isRequired: true);

        public event System.Action<WarehouseLocationField, WarehouseLocation> OnRemoveLocation;
        public event System.Action<WarehouseLocationField> OnUpdateLocation;

        private readonly IconButton BtnRemove = new IconButton(new ButtonSettings
        {
            Name = "touchButtonIcon",
            Icon = $"{PathsSettings.ImagesFolderLocation}{@"Icons/Windows/icon_window_delete_record.png"}",
            BackgroundColor = Color.LightGray,
            IconSize = new Size(15, 15),
            ButtonSize = new Size(30, 15)
        });

        private readonly IconButton BtnUpdate = new IconButton(new ButtonSettings
        {
            Name = "touchButtonIcon",
            Icon = PathsSettings.ImagesFolderLocation + @"Icons\Dialogs\icon_pos_dialog_action_ok.png",
            BackgroundColor = AppSettings.Instance.colorBaseDialogActionAreaButtonBackground,
            IconSize = new Size(15, 15),
            ButtonSize = new Size(30, 15),
        });

        public HBox Component { get; }

        public WarehouseLocationField(WarehouseLocation location = null)
        {
            Component = new HBox(false, 5);
            Component.PackStart(TxtLocation.Component);
            Component.PackEnd(BtnRemove, false, false, 1);

            Location = location;

            if (Location != null)
            {
                TxtLocation.Text = Location.Designation;
                Component.PackEnd(BtnUpdate, false, false, 1);
            }

            AddEventHandlers();    
        }

        private void AddEventHandlers()
        {
            BtnRemove.Clicked += (s, e) => OnRemoveLocation?.Invoke(this, Location);
            BtnUpdate.Clicked += (s, e) => OnUpdateLocation?.Invoke(this);
        }

    }
}
