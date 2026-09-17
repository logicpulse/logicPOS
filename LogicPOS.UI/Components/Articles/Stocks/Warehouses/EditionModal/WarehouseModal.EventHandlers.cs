using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Articles.Stocks.Warehouses.Service;
using LogicPOS.UI.Components.Warehouses;
using LogicPOS.UI.Errors;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WarehouseModal
    {
        private void Button_AddLocation_Clicked(object sender, EventArgs e)
        {
            AddLocationField();
        }

        private void On_BtnSetDefaultClicked(WarehouseLocation location)
        {
            if (location == null)
            {
                return;
            }

            var result = WarehousesService.SetLocationAsDefault(location);

            if(result == false)
            {
                return;
            }

            foreach (var field in _locationFields.Where(l => l.Location != null))
            {
                field.Location.IsDefault = field.Location.Id == location.Id;
                field.UpdateUI();
            }
        }

        private void On_BtnRemoveLocation_Clicked(WarehouseLocationField field, WarehouseLocation warehouseLocation)
        {
            if (warehouseLocation != null)
            {

                ResponseType responseType = CustomAlerts.Question(this)
                                          .WithTitleResource("global_warning")
                                          .WithMessageResource("dialog_message_delete_record")
                                          .ShowAlert();

                if (responseType != ResponseType.Yes)
                {
                    return;
                }

                DeleteLocation(warehouseLocation);
            }

            _boxLocations.Remove(field.Component);
            _locationFields.Remove(field);
            ValidatableFields.Remove(field.TxtLocation);
        }

        private void DeleteLocation(WarehouseLocation warehouseLocation)
        {
            var command = new DeleteWarehouseLocationCommand(warehouseLocation.Id);

            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }

            _entity.Locations.Remove(warehouseLocation);
        }
    }
}
