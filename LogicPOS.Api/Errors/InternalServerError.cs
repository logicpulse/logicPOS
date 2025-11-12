using System;
using System.Collections.Generic;
using System.Text;

namespace LogicPOS.Api.Errors
{
    public struct InternalServerError
    {
        public string Status { get; set; }
        public int Code { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }
}
