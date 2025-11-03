using System;

namespace LogicPOS.Api.Features.Common.Responses
{
    public struct AutoCompleteLine
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
