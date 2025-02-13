using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components
{
    public class WarehouseSelectionField : IValidatableField
    {
        public EntityComboBox<Warehouse> WarehouseField { get; private set; }
        public EntityComboBox<WarehouseLocation> LocationField { get; private set; }
        private WarehouseArticle _warehouseArticle;
        public string FieldName => WarehouseField.Label.Text;

        public WarehouseSelectionField(WarehouseArticle warehouseArticle = null)
        {
            _warehouseArticle = warehouseArticle;
            InitializeComboboxes();
        }

        private void InitializeComboboxes()
        {
            var warehouses = GetWarehouses();
            var labelText = GeneralUtils.GetResourceByName("global_warehouse");

            WarehouseField = new EntityComboBox<Warehouse>(labelText,
                                                            warehouses,
                                                            _warehouseArticle?.WarehouseLocation.Warehouse,
                                                            true);

            Warehouse currentWarehouse = null;

            if (_warehouseArticle != null)
            {
                currentWarehouse = warehouses.Where(x => x.Id == _warehouseArticle.WarehouseLocation.Warehouse.Id).First();
            }

            LocationField = new EntityComboBox<WarehouseLocation>(GeneralUtils.GetResourceByName("global_locations"),
                                                                             currentWarehouse?.Locations ?? Enumerable.Empty<WarehouseLocation>(),
                                                                             _warehouseArticle.WarehouseLocation,
                                                                            true);


            WarehouseField.ComboBox.Changed += (sender, e) =>
            {
                LocationField.Entities = WarehouseField.SelectedEntity?.Locations;
                LocationField.ReLoad();
            };
        }

        private IEnumerable<Warehouse> GetWarehouses()
        {
            var result = DependencyInjection.Mediator.Send(new GetAllWarehousesQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return Enumerable.Empty<Warehouse>();
            }

            return result.Value;
        }

        public bool IsValid()
        {
            return WarehouseField.IsValid() && LocationField.IsValid();
        }

    }
}
