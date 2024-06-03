using System;

namespace LogicPOS.DTOs.Common
{
    public class CurrenyDto
    {
        public Guid Id { get; set; }
        public string Acronym { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}
