using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Holidays.UpdateHoliday;
using LogicPOS.Api.Features.Warehouses.AddWarehouse;
using LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation;
using LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation;
using LogicPOS.Api.Features.Warehouses.UpdateWarehouse;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Warehouses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WarehouseModal : EntityModal<Warehouse>
    {
        public WarehouseModal(EntityModalMode modalMode, Warehouse entity = null) : base(modalMode, entity)
        {
        }


        protected override void AddEntity()
        {
            var command = CreateAddCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        private AddWarehouseCommand CreateAddCommand()
        {
            var command = new AddWarehouseCommand
            {
                Designation = _txtDesignation.Text,
                IsDefault = _checkDefaultWarehouse.Active,
                Locations = _locations.Select(x => x.TxtLocation.Text)
            };

            return command;
        }

        protected override void ShowEntityData()
        {
            _txtCode.Text = _entity.Code;
            _txtDesignation.Text = _entity.Designation;
            _txtOrder.Text = _entity.Order.ToString();
            _checkDefaultWarehouse.Active = _entity.IsDefault;
            _checkDisabled.Active = _entity.IsDeleted;

            foreach (var location in _entity.Locations)
            {
                AddLocationField(location);
            }
        }

        protected override void UpdateEntity()
        {
            var result = _mediator.Send(CreateUpdateCommand()).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }


        private UpdateWarehouseCommand CreateUpdateCommand()
        {
            return new UpdateWarehouseCommand()
            {
                Id = _entity.Id,
                NewDesignation = _txtDesignation.Text,
                IsDefault = _checkDefaultWarehouse.Active,
                Locations = _locations.Select(x=>x.TxtLocation.Text).ToList()
            };
            
        }



        private void Button_AddLocation_Clicked(object sender, System.EventArgs e)
        {
            AddLocationField();
        }

        private void Button_RemoveLocation_Clicked(WarehouseLocationField field, WarehouseLocation warehouseLocation)
        {
            if (warehouseLocation != null)
            {

                ResponseType responseType = SimpleAlerts.Question()
                                          .WithParent(this)
                                          .WithTitleResource("global_warning")
                                          .WithMessageResource("dialog_message_delete_record")
                                          .Show();

                if (responseType != ResponseType.Yes)
                {
                    return;
                }

                DeleteLocation(warehouseLocation);
            }

            _boxLocations.Remove(field.Component);
            _locations.Remove(field);
        }

        private void DeleteLocation(WarehouseLocation warehouseLocation)
        {
            var command = new DeleteWarehouseLocationCommand(warehouseLocation.Id);
  
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }

            _entity.Locations.Remove(warehouseLocation);
        }


        private void Button_UpdateLocation_Clicked(WarehouseLocationField field)
        {
            var id = field.Location.Id;
            var newDesignation = field.TxtLocation.Text;

            foreach (var location in _entity.Locations)
            {
                if (location.Id == id)
                {
                    location.Designation = newDesignation;
                    UpdateLocationCommand(location);
                }
            }
        }

        private void UpdateLocationCommand(WarehouseLocation location)
        {
            var command = new UpdateWarehouseLocationCommand() { 
                Id = location.Id, 
                NewDesignation = location.Designation
            };


            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }
    }
}
