using System.Collections.Generic;

namespace LogicPOS.Api.Features.Orders.CreateOrder
{
    public class CreateTicketDto
    {
        public IEnumerable<CreateOrderDetailDto> Details { get; set; } 
    }
}
