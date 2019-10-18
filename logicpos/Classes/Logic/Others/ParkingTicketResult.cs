using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logicpos.Classes.Logic.Others
{
    public class ParkingTicketResult
    {

        //xs:element name =\"date\" type=\"xs:string\" minOccurs=\"0\" /
        public string Date { get; set; }
        //xs:element name =\"minutes\" type=\"xs:string\" minOccurs=\"0\" /
        public string Minutes { get; set; }
        //xs:element name =\"quantity\" type=\"xs:string\" minOccurs=\"0\" /
        public string Quantity { get; set; }
        //xs:element name =\"price\" type=\"xs:string\" minOccurs=\"0\" /
        public Decimal Price { get; set; }
        //xs:element name =\"description\" type=\"xs:string\" minOccurs=\"0\" /
        public string Description { get; set; }
        
        // Stores the Ean of a parking ticket
        public string Ean { get; set; }

        public bool AlreadyPaid { get; set; }
        
        public bool AlreadyExit { get; set; }

        public string DatePaid { get; set; }

        public string DateTolerance { get; set; }

        public string DateExits { get; set; }

    }
}
