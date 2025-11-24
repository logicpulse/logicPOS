using System.Collections.Generic;

namespace LogicPOS.Api.Errors
{
    public struct InternalServerError
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string Instance { get; set; }
        public string TraceId { get; set; }
        public string Detail { get; set; }
        public List<Error> Errors { get; set; }

        public struct Error
        {
            public string Name { get; set; }
            public string Reason { get; set;  }
        }
    }


}
