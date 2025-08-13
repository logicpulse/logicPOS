using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Articles.Stocks.Warehouses.Service
{
    public static class WarehousesService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;
        public  static List<Warehouse> GetAllWarehouses()
        {
            var result= _mediator.Send(new GetAllWarehousesQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            var warehouses=result.Value.ToList();
            return warehouses;
        }
    }
}
