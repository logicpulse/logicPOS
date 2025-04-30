using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System;

namespace LogicPOS.Api.Features.Articles.Common
{
    public class ArticleViewModel : ApiEntity, IWithCode, IWithDesignation
    {
        public string Code { get; set; } 
        public decimal TotalStock { get; set; }
        public bool IsComposed { get; set; }
        public string Family { get; set; } 
        public string Subfamily { get; set; } 
        public string Designation { get; set; } 
        public string Type { get; set; } 
        public string ButtonLabel { get; set; }
        public decimal DefaultQuantity { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal Price { get; set; }
        public decimal? VatDirectSelling { get; set; }
        public decimal Discount { get; set; }
        public string Unit { get; set; }
        public Guid SubfamilyId { get; set; }
        public Guid FamilyId { get; set; }

        public static ArticleViewModel FromEntity(Article article)
        {
            return new ArticleViewModel
            {
                Id = article.Id,
                Code = article.Code,
                Designation = article.Designation,
                Family = article.Subfamily?.Family?.Designation,
                Subfamily = article.Subfamily?.Designation,
                Type = article.Type.Designation,
                ButtonLabel = article.Button.Label,
                DefaultQuantity = article.DefaultQuantity,
                MinimumStock = article.MinimumStock,
                Price = article.Price1.Value,
                VatDirectSelling = article.VatDirectSelling?.Value,
                Discount = article.Discount,
                IsComposed = article.IsComposed,
                Unit = article.MeasurementUnit?.Acronym,
                SubfamilyId = article.SubfamilyId,
                FamilyId = article.Subfamily?.FamilyId ?? Guid.Empty,
            };
        }
    }
}
