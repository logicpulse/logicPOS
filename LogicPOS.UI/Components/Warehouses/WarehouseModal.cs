using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.AddWarehouse;
using LogicPOS.Api.Features.Warehouses.Locations.AddWarehouseLocations;
using LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation;
using LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation;
using LogicPOS.Api.Features.Warehouses.UpdateWarehouse;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Warehouses;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WarehouseModal : EntityEditionModal<Warehouse>
    {
        public WarehouseModal(EntityEditionModalMode modalMode, Warehouse entity = null) : base(modalMode, entity)
        {
        }


        protected override void AddEntity()=>ExecuteAddCommand(CreateAddCommand());


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
            _txtNotes.Value.Text = _entity.Notes;

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

            if(_locations.Any(x => x.Location == null))
            {
                AddNewLocations();
            }

            if(_locations.Any(x => x.Location != null && x.Location.Designation != x.TxtLocation.Text))
            {
                UpdateLocations();
            }
        }

        private void UpdateLocations()
        {
            var locationsToUpdate = _locations.Where(x => x.Location != null && x.Location.Designation != x.TxtLocation.Text).ToList();

            foreach (var location in locationsToUpdate)
            {
                UpdateLocation(location);
            }
        }

        private void UpdateLocation(WarehouseLocationField field)
        {
            var command = CreateUpdateLocationCommand(field);
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        private void AddNewLocations()
        {
            var command = CreateAddLocationsCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                HandleApiError(result.FirstError);
                return;
            }
        }

        private UpdateWarehouseLocationCommand CreateUpdateLocationCommand(WarehouseLocationField field)
        {
            return new UpdateWarehouseLocationCommand
            {
                Id = field.Location.Id,
                NewDesignation = field.TxtLocation.Text
            };
        }

        private AddWarehouseLocationsCommand CreateAddLocationsCommand()
        {
            return new AddWarehouseLocationsCommand()
            {
                Id = _entity.Id,
                Locations = _locations.Where(x => x.Location == null).Select(x => x.TxtLocation.Text).ToList()
            };
        }

        private UpdateWarehouseCommand CreateUpdateCommand()
        {
            return new UpdateWarehouseCommand()
            {
                Id = _entity.Id,
                NewCode = _txtCode.Text,
                NewOrder = uint.Parse(_txtOrder.Text),
                NewDesignation = _txtDesignation.Text,
                IsDefault = _checkDefaultWarehouse.Active,
                IsDeleted = _checkDisabled.Active,
                NewNotes = _txtNotes.Value.Text
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
                                          .ShowAlert();

                if (responseType != ResponseType.Yes)
                {
                    return;
                }

                DeleteLocation(warehouseLocation);
            }

            _boxLocations.Remove(field.Component);
            _locations.Remove(field);
            ValidatableFields.Remove(field.TxtLocation);
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
    }
}
