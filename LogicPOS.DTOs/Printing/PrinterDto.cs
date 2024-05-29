using System;

namespace LogicPOS.DTOs.Printing
{
    public class PrinterDto
    {
        public Guid Id { get; set; }
        public string Designation { get; set; }
        public string NetworkName { get; set; }
        public string Token { get; set; }
        public bool IsThermal { get; set; }
    }
}
