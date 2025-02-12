using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.InputFields
{
    public class UniqueArticleField : IValidatableField
    {
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        
        private TextBox TxtSerialNumber { get; set; } = TextBox.Simple("global_serial_number", true);
        private EntityComboBox<Warehouse> _comboWarehouse { get; set; }
        private EntityComboBox<WarehouseLocation> _comboWarehouseLocation { get; set; }
        public string FieldName => TxtSerialNumber.FieldName;
        public Widget Component { get; private set; }

        public UniqueArticleField()
        {
            InitializeComboboxes();
            Component = CreateComponent();
        }

        public Widget CreateComponent()
        {
            var hbox = new HBox(false, 2);
            hbox.PackStart(TxtSerialNumber.Component, true, true, 0);
            hbox.PackStart(_comboWarehouse.Component, true, true, 0);
            hbox.PackStart(_comboWarehouseLocation.Component, true, true, 0);
            return hbox;
        }

        public bool IsValid()
        {
            return TxtSerialNumber.IsValid() && _comboWarehouse.IsValid() && _comboWarehouseLocation.IsValid();
        }

        private void InitializeComboboxes()
        {
            var warehouses = GetWarehouses();
            var labelText = GeneralUtils.GetResourceByName("global_warehouse");

            _comboWarehouse = new EntityComboBox<Warehouse>(labelText,
                                                            warehouses,
                                                            null,
                                                            true);

            _comboWarehouseLocation = new EntityComboBox<WarehouseLocation>(GeneralUtils.GetResourceByName("global_locations"),
                                                                            Enumerable.Empty<WarehouseLocation>(),
                                                                            null,
                                                                            true);


            _comboWarehouse.ComboBox.Changed += (sender, e) =>
            {
                _comboWarehouseLocation.Entities = _comboWarehouse.SelectedEntity?.Locations;
                _comboWarehouseLocation.ReLoad();
            };
        }

        private IEnumerable<Warehouse> GetWarehouses()
        {
            var result = _mediator.Send(new GetAllWarehousesQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return Enumerable.Empty<Warehouse>();
            }

            return result.Value;
        }

    }
}
