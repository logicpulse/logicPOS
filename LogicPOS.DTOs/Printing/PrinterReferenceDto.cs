using System;

namespace LogicPOS.DTOs.Printing
{
    public class PrinterReferenceDto
    {
        public Guid Id { get; set; }
        public string Designation { get; set; }
        public string NetworkName { get; set; }
        public string Token { get; set; }

    }
}
