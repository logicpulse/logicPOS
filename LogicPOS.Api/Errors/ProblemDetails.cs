using System.Collections.Generic;

namespace LogicPOS.Api.Errors
{
    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string TraceId { get; set; }
        public IEnumerable<ProblemDetailsError> Errors { get; set; }
    }
}
