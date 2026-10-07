using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.ValueObjects;

[ComplexType]
public sealed class ArticlePrice
{
    [Column(TypeName = "decimal(18,6)")]
    public decimal Value { get; set; }
    [Column(TypeName = "decimal(18,6)")]
    public decimal PromotionValue { get; set; }
    public bool UsePromotion { get; set; }

    public static ArticlePrice Default()
    {
        return new ArticlePrice
        {
            Value = 0,
            PromotionValue = 0,
            UsePromotion = false
        };
    }
}
