using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.Entities.Dtos;
using LogicPOS.Domain.Entities.Utilities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.Results;
using LogicPOS.Domain.ValueObjects;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LogicPOS.Domain.Entities.Common.Entity;

namespace LogicPOS.Domain.Entities;

//[Table("fin_article")]
public class Article : Entity.WithCode.AndOrder.AndDesignation
{
    #region  Dependencies
    public ArticleClass? Class { get; set; }
    public Guid ClassId { get; set; }

    public ArticleSubfamily? Subfamily { get; set; }
    public Guid SubfamilyId { get; set; }

    public ArticleType? Type { get; set; }
    public Guid TypeId { get; set; }

    public MeasurementUnit? MeasurementUnit { get; set; }
    public Guid MeasurementUnitId { get; set; }

    public SizeUnit? SizeUnit { get; set; }
    public Guid SizeUnitId { get; set; }

    public CommissionGroup? CommissionGroup { get; set; }
    public Guid? CommissionGroupId { get; set; }

    public DiscountGroup? DiscountGroup { get; set; }
    public Guid? DiscountGroupId { get; set; }

    public Guid? VatOnTableId { get; set; }
    public VatRate? VatOnTable { get; set; }

    public VatRate? VatDirectSelling { get; set; }
    public Guid VatDirectSellingId { get; set; }

    public VatExemptionReason? VatExemptionReason { get; set; }
    public Guid? VatExemptionReasonId { get; set; }

    #endregion

    #region Properties
    public string? CodeDealer { get; set; }

    [Required]
    public Button Button { get; set; } = null!;

    [Required]
    public ArticlePrice Price1 { get; set; } = null!;

    [Required]
    public ArticlePrice Price2 { get; set; } = null!;

    [Required]
    public ArticlePrice Price3 { get; set; } = null!;

    [Required]
    public ArticlePrice Price4 { get; set; } = null!;

    [Required]
    public ArticlePrice Price5 { get; set; } = null!;

    public bool PriceWithVat { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal Discount { get; set; }
       
    [Column(TypeName = "decimal(18,6)")]
    public decimal DefaultQuantity { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal MinimumStock { get; set; }

    public float Tare { get; set; }

    public float Weight { get; set; }
    public string? Barcode { get; set; }
    public bool PVPVariable { get; set; }
    public bool Favorite { get; set; }
    public bool UseWeighingBalance { get; set; }
    public bool IsComposed { get; set; }
    public bool UniqueArticles { get; set; }
    public string? BarcodeLabelPrintModel { get; set; }
    public bool IsSdrPackaging { get; set; }
    public bool IsTicketing { get; set; }
    #endregion

  

    public async Task<Result> UpdateAsync(UpdateArticleDto dto,
        IArticleRepository repository,
        ArticleReferences dependencies,
        CancellationToken cancellationToken = default)
    {
        var updater = new ArticleUpdater(this, dto, repository, dependencies);
        return await updater.UpdateAsync(cancellationToken);
    }


    public static async Task<Result<Article>> CreateAsync(CreateArticleDto dto,
        IArticleRepository repository,
        ArticleReferences dependencies,
        CancellationToken cancellationToken = default)
    {
        var creator = new ArticleCreator(dto, repository, dependencies);
        return await creator.CreateAsync(cancellationToken);
    }
}