using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Entities.Articles.Articles.Common;
using LogicPOS.Domain.Entities.Common;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogicPOS.Persistence.Entities.Articles.Articles;

public static class SdrDepositArticleSeeder
{
    public static void EnsureConfigured(LogicPOSDbContext database, ILogger logger)
    {
        if (database.Articles.Any(a => a.Code == SdrConstants.SdrArticleCode))
        {
            logger.LogDebug("SDR deposit article {Code} already exists", SdrConstants.SdrArticleCode);
            return;
        }

        var references = ResolveReferences(database, logger);
        if (references is null)
        {
            return;
        }

        var family = EnsureFamily(database, logger);
        var subfamily = EnsureSubfamily(database, family.Id, references.VatRateId, logger);

        var article = CreateDepositArticle(subfamily.Id, references);
        database.Articles.Add(article);
        database.SaveChanges();

        logger.LogInformation(
            "Created SDR deposit article {Code} under family {FamilyCode}/{SubfamilyCode}",
            SdrConstants.SdrArticleCode,
            SdrConstants.SdrFamilyCode,
            SdrConstants.SdrSubfamilyCode);
    }

    private static SdrDepositReferences? ResolveReferences(LogicPOSDbContext database, ILogger logger)
    {
        var classId = database.ArticleClasses.AsNoTracking()
            .Where(x => x.Acronym == "I")
            .Select(x => x.Id)
            .FirstOrDefault();

        if (classId == Guid.Empty)
        {
            logger.LogWarning(
                "Skipping SDR deposit article setup: article class with acronym 'I' was not found");
            return null;
        }

        var vatRateId = database.VatRates.AsNoTracking()
            .Where(x => x.Value == 0)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (vatRateId == Guid.Empty)
        {
            logger.LogWarning(
                "Skipping SDR deposit article setup: VAT rate with value 0 was not found");
            return null;
        }

        var vatExemptionReasonId = database.VatExemptionReasons.AsNoTracking()
            .Where(x => x.Acronym == "M99")
            .Select(x => x.Id)
            .FirstOrDefault();

        if (vatExemptionReasonId == Guid.Empty)
        {
            logger.LogWarning(
                "Skipping SDR deposit article setup: VAT exemption reason with acronym 'M99' was not found");
            return null;
        }

        var articleTypeId = database.ArticleTypes.AsNoTracking()
            .OrderBy(x => x.Order)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (articleTypeId == Guid.Empty)
        {
            logger.LogWarning(
                "Skipping SDR deposit article setup: no article types are available");
            return null;
        }

        var measurementUnitId = database.MeasurementUnits.AsNoTracking()
            .OrderBy(x => x.Order)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (measurementUnitId == Guid.Empty)
        {
            logger.LogWarning(
                "Skipping SDR deposit article setup: no measurement units are available");
            return null;
        }

        var sizeUnitId = database.SizeUnits.AsNoTracking()
            .OrderBy(x => x.Order)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (sizeUnitId == Guid.Empty)
        {
            logger.LogWarning(
                "Skipping SDR deposit article setup: no size units are available");
            return null;
        }

        return new SdrDepositReferences(
            classId,
            vatRateId,
            vatExemptionReasonId,
            articleTypeId,
            measurementUnitId,
            sizeUnitId);
    }

    private static ArticleFamily EnsureFamily(LogicPOSDbContext database, ILogger logger)
    {
        var existing = database.ArticleFamilies
            .FirstOrDefault(f => f.Code == SdrConstants.SdrFamilyCode);

        if (existing is not null)
        {
            return existing;
        }

        var family = new ArticleFamily
        {
            Code = SdrConstants.SdrFamilyCode,
            Designation = SdrConstants.SdrFamilyDesignation,
            Order = 99990,
            Button = new Button(),
            IsDeleted = true,
            DeletedAt = DateTime.Now
        };

        database.ArticleFamilies.Add(family);
        database.SaveChanges();

        logger.LogInformation(
            "Created SDR family {Code} ({Designation}) with IsDeleted=true",
            SdrConstants.SdrFamilyCode,
            SdrConstants.SdrFamilyDesignation);

        return family;
    }

    private static ArticleSubfamily EnsureSubfamily(
        LogicPOSDbContext database,
        Guid familyId,
        Guid vatRateId,
        ILogger logger)
    {
        var existing = database.ArticleSubfamilies
            .FirstOrDefault(s => s.Code == SdrConstants.SdrSubfamilyCode && s.FamilyId == familyId);

        if (existing is not null)
        {
            return existing;
        }

        var subfamily = new ArticleSubfamily
        {
            FamilyId = familyId,
            Code = SdrConstants.SdrSubfamilyCode,
            Designation = SdrConstants.SdrSubfamilyDesignation,
            Order = 99990,
            Button = new Button(),
            VatDirectSellingId = vatRateId,
            IsDeleted = true,
            DeletedAt = DateTime.Now
        };

        database.ArticleSubfamilies.Add(subfamily);
        database.SaveChanges();

        logger.LogInformation(
            "Created SDR subfamily {Code} ({Designation}) with IsDeleted=true",
            SdrConstants.SdrSubfamilyCode,
            SdrConstants.SdrSubfamilyDesignation);

        return subfamily;
    }

    private static Article CreateDepositArticle(Guid subfamilyId, SdrDepositReferences references)
    {
        var zeroPrice = ArticlePrice.Default();

        return new Article
        {
            Code = SdrConstants.SdrArticleCode,
            Designation = SdrConstants.SdrArticleDesignation,
            Order = 99990,
            ClassId = references.ClassId,
            SubfamilyId = subfamilyId,
            TypeId = references.ArticleTypeId,
            MeasurementUnitId = references.MeasurementUnitId,
            SizeUnitId = references.SizeUnitId,
            VatDirectSellingId = references.VatRateId,
            VatExemptionReasonId = references.VatExemptionReasonId,
            Button = new Button(),
            Price1 = new ArticlePrice
            {
                Value = SdrConstants.SdrArticlePrice,
                PromotionValue = 0,
                UsePromotion = false
            },
            Price2 = zeroPrice,
            Price3 = zeroPrice,
            Price4 = zeroPrice,
            Price5 = zeroPrice,
            PriceWithVat = false,
            Discount = 0,
            DefaultQuantity = 1,
            IsComposed = false,
            UniqueArticles = false,
            IsSdrPackaging = false,
            IsDeleted = false
        };
    }

    private sealed record SdrDepositReferences(
        Guid ClassId,
        Guid VatRateId,
        Guid VatExemptionReasonId,
        Guid ArticleTypeId,
        Guid MeasurementUnitId,
        Guid SizeUnitId);
}
