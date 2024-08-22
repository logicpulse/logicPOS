﻿using LogicPOS.Api.Features.Common;

namespace LogicPOS.Api.Entities
{
    public class Country : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string Code2 { get; set; }
        public string Code3 { get; set; }
        public string Capital { get; set; }
        public string TLD { get; set; }
        public string Currency { get; set; }
        public string CurrencyCode { get; set; }
        public string FiscalNumberRegex { get; set; }
        public string ZipCodeRegex { get; set; }

    }
}
