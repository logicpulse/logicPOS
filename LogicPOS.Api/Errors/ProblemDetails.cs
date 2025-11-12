using System.Collections.Generic;

namespace LogicPOS.Api.Errors
{
    public struct ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public string Detail { get; set; }
        public string Instance {  get; set; }
        public List<ProblemDetailsError> Errors { get; set; }
    }
}
