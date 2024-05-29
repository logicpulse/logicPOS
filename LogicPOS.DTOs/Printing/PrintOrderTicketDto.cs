using System.Collections.Generic;

namespace LogicPOS.DTOs.Printing
{
    public struct PrintOrderTicketDto
    {
        public int TicketId { get; set; }
        public string TableDesignation { get; set; }
        public string PlaceDesignation { get; set; }

        public List<PrintOrderDetailDto> OrderDetails { get; set; } 
    }
}
