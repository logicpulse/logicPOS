using LogicPOS.Api.Enums;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Entities
{
    public class ArticleSerialNumber : ApiEntity
    {
        public Article Article { get; set; }
        public Guid ArticleId { get; set; }
        public string SerialNumber { get; set; } 
        public ArticleSerialNumberStatus Status { get; set; }
    }
}
