using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.AddWarehouse;
using LogicPOS.Api.Features.Warehouses.Locations.AddWarehouseLocations;
using LogicPOS.Api.Features.Warehouses.Locations.DeleteWarehouseLocation;
using LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation;
using LogicPOS.Api.Features.Warehouses.UpdateWarehouse;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Articles.Stocks.Warehouses.Service;
using LogicPOS.UI.Components.Warehouses;
using LogicPOS.UI.Errors;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class WarehouseModal : EntityEditionModal<Warehouse>
    {
        public WarehouseModal(EntityEditionModalMode modalMode, Warehouse entity = null) : base(modalMode, entity)
        {
        }


        protected override bool AddEntity() => ExecuteAddCommand(CreateAddCommand()).IsError == false;


        private AddWarehouseCommand CreateAddCommand()
        {
            var command = new AddWarehouseCommand
            {
                Designation = _txtDesignation.Text,
                IsDefault = _checkDefaultWarehouse.Active,
                Locations = _locationFields.Select(x => x.TxtLocation.Text)
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

        protected override bool UpdateEntity()
        {
            var result = _mediator.Send(CreateUpdateCommand()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            if (_locationFields.Any(x => x.Location == null))
            {
                AddNewLocations();
            }

            if (_locationFields.Any(x => x.Location != null && x.Location.Designation != x.TxtLocation.Text))
            {
                UpdateLocations();
            }

            return true;
        }

        private void UpdateLocations()
        {
            var locationsToUpdate = _locationFields.Where(x => x.Location != null && x.Location.Designation != x.TxtLocation.Text).ToList();

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
                ErrorHandlingService.HandleApiError(result);
                return;
            }
        }

        private void AddNewLocations()
        {
            var command = CreateAddLocationsCommand();
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }
        }

        private UpdateWarehouseLocationCommand CreateUpdateLocationCommand(WarehouseLocationField field)
        {
            return new UpdateWarehouseLocationCommand
            {
                Id = field.Location.Id,
                Designation = field.TxtLocation.Text
            };
        }

        private AddWarehouseLocationsCommand CreateAddLocationsCommand()
        {
            return new AddWarehouseLocationsCommand()
            {
                Id = _entity.Id,
                Locations = _locationFields.Where(x => x.Location == null).Select(x => x.TxtLocation.Text).ToList()
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

    }
}
