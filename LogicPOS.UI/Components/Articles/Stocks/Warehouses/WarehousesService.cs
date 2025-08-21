using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.Api.Features.Warehouses.Locations.UpdateWarehouseLocation;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Stocks.Warehouses.Service
{
    public static class WarehousesService
    {
        public static List<Warehouse> GetAllWarehouses()
        {
            var result = DependencyInjection.Mediator.Send(new GetAllWarehousesQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            var warehouses = result.Value.ToList();
            return warehouses;
        }

        public static bool SetLocationAsDefault(WarehouseLocation location)
        {
            var updateCommand = new UpdateWarehouseLocationCommand
            {
                Id = location.Id,
                Designation = location.Designation,
                IsDefault = true
            };

            var result = DependencyInjection.Mediator.Send(updateCommand).Result;
          
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return true;
        }
    }
}
