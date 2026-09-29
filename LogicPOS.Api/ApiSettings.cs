using Microsoft.Extensions.Configuration;

namespace LogicPOS.Api
{
    public struct ApiSettings
    {
        public string BaseAddress { get; set; }
        public int DefaultPageSize { get; set; }

        public static ApiSettings Default { get; set; }
    }
}
