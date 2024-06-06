using LogicPOS.DTOs.Reporting;
using LogicPOS.Reporting.Reports.Documents;

namespace LogicPOS.Reporting.Utility
{
    public static class ReportMapping
    {
        public static FinanceMasterViewReportDto GetFinanceMasterViewReportDto(
            FinanceMasterViewReport report)
        {
            var dto = new FinanceMasterViewReportDto
            {
                Oid = report.Oid,
                DocumentType = report.DocumentType,
                DocumentTypeOrd = report.DocumentTypeOrd,
                DocumentTypeCode = report.DocumentTypeCode,
                DocumentTypeDesignation = report.DocumentTypeDesignation,
                DocumentTypeAcronym = report.DocumentTypeAcronym,
                DocumentTypeResourceString = report.DocumentTypeResourceString,
                DocumentTypeResourceStringReport = report.DocumentTypeResourceStringReport,
                DocumentTypeWayBill = report.DocumentTypeWayBill,
                DocumentNumber = report.DocumentNumber,
                Date = report.Date,
                DocumentDate = report.DocumentDate,
                SystemEntryDate = report.SystemEntryDate,
                DocumentCreatorUser = report.DocumentCreatorUser,
                TotalNet = report.TotalNet,
                TotalGross = report.TotalGross,
                TotalDiscount = report.TotalDiscount,
                TotalTax = report.TotalTax,
                TotalFinal = report.TotalFinal,
                TotalFinalRound = report.TotalFinalRound,
                TotalDelivery = report.TotalDelivery,
                TotalChange = report.TotalChange,
                Discount = report.Discount,
                DiscountFinancial = report.DiscountFinancial,
                ExchangeRate = report.ExchangeRate,
                EntityOid = report.EntityOid,
                EntityCode = report.EntityCode,
                EntityHidden = report.EntityHidden,
                EntityInternalCode = report.EntityInternalCode,
                EntityName = report.EntityName,
                EntityAddress = report.EntityAddress,
                EntityZipCode = report.EntityZipCode,
                EntityCity = report.EntityCity,
                EntityLocality = report.EntityLocality,
                EntityCountryCode2 = report.EntityCountryCode2,
                EntityCountry = report.EntityCountry,
                EntityFiscalNumber = report.EntityFiscalNumber,
                DocumentStatusStatus = report.DocumentStatusStatus,
                TransactionID = report.TransactionID,
                ShipToDeliveryID = report.ShipToDeliveryID,
                ShipToDeliveryDate = report.ShipToDeliveryDate,
                ShipToWarehouseID = report.ShipToWarehouseID,
                ShipToLocationID = report.ShipToLocationID,
                ShipToAddressDetail = report.ShipToAddressDetail,
                ShipToCity = report.ShipToCity,
                ShipToPostalCode = report.ShipToPostalCode,
                ShipToRegion = report.ShipToRegion,
                ShipToCountry = report.ShipToCountry,
                ShipFromDeliveryID = report.ShipFromDeliveryID,
                ShipFromDeliveryDate = report.ShipFromDeliveryDate,
                ShipFromWarehouseID = report.ShipFromWarehouseID,
                ShipFromLocationID = report.ShipFromLocationID,
                ShipFromAddressDetail = report.ShipFromAddressDetail,
                ShipFromCity = report.ShipFromCity,
                ShipFromPostalCode = report.ShipFromPostalCode,
                ShipFromRegion = report.ShipFromRegion,
                ShipFromCountry = report.ShipFromCountry,
                MovementStartTime = report.MovementStartTime,
                MovementEndTime = report.MovementEndTime,
                ATDocCodeID = report.ATDocCodeID,
                Payed = report.Payed,
                PayedDate = report.PayedDate,
                Notes = report.Notes,
                PaymentMethod = report.PaymentMethod,
                PaymentMethodOrd = report.PaymentMethodOrd,
                PaymentMethodCode = report.PaymentMethodCode,
                PaymentMethodDesignation = report.PaymentMethodDesignation,
                PaymentMethodToken = report.PaymentMethodToken,
                PaymentCondition = report.PaymentCondition,
                PaymentConditionOrd = report.PaymentConditionOrd,
                PaymentConditionCode = report.PaymentConditionCode,
                PaymentConditionDesignation = report.PaymentConditionDesignation,
                PaymentConditionAcronym = report.PaymentConditionAcronym,
                Country = report.Country,
                CountryOrd = report.CountryOrd,
                CountryCode = report.CountryCode,
                CountryDesignation = report.CountryDesignation,
                Currency = report.Currency,
                CurrencyOrd = report.CurrencyOrd,
                CurrencyCode = report.CurrencyCode,
                CurrencyDesignation = report.CurrencyDesignation,
                CurrencyAcronym = report.CurrencyAcronym,
                ATDocQRCode = report.ATDocQRCode,
            };

            dto.DocumentFinanceDetail = report.DocumentFinanceDetail.ConvertAll(
                detail => GetFinanceDetailReportDto(detail)
                );
            dto.DocumentFinanceMasterTotal = report.DocumentFinanceMasterTotal.ConvertAll(
                total => GetFinanceMasterTotalViewReportDto(total)
                );

            return dto;
        }

        public static FinanceDetailReportDto GetFinanceDetailReportDto(
            FinanceDetailReport financeDetailReport)
        {
            var FinanceDatailReportDto = new FinanceDetailReportDto
            {
                Code = financeDetailReport.Code,
                Designation = financeDetailReport.Designation,
                Discount = financeDetailReport.Discount,
                Notes = financeDetailReport.Notes,
                Price = financeDetailReport.Price,
                Quantity = financeDetailReport.Quantity,
                TotalFinal = financeDetailReport.TotalFinal,
                TotalGross = financeDetailReport.TotalGross,
                TotalNet = financeDetailReport.TotalNet,
                UnitMeasure = financeDetailReport.UnitMeasure,
                Vat = financeDetailReport.Vat,
                VatExemptionReasonDesignation = financeDetailReport.VatExemptionReasonDesignation,
            };
            return FinanceDatailReportDto;
        }

        public static FinanceMasterTotalViewReportDto GetFinanceMasterTotalViewReportDto(
            FinanceMasterTotalViewReport financeMasterTotalViewReport)
        {
            var TotalViewReportDto = new FinanceMasterTotalViewReportDto
            {
                Oid = financeMasterTotalViewReport.Oid,
                Designation = financeMasterTotalViewReport.Designation,
                DocumentMaster = financeMasterTotalViewReport.DocumentMaster,
                TaxCode = financeMasterTotalViewReport.TaxCode,
                TaxCountryRegion = financeMasterTotalViewReport.TaxCountryRegion,
                Total = financeMasterTotalViewReport.Total,
                TotalBase = financeMasterTotalViewReport.TotalBase,
                TotalType = (int)financeMasterTotalViewReport.TotalType,
                Value = financeMasterTotalViewReport.Value,

            };

            return TotalViewReportDto;
        }

        public static FinancePaymentDocumentViewReportDto GetFinancePaymentDocumentViewReportDto(
            FinancePaymentDocumentViewReport report)
        {
            var dto = new FinancePaymentDocumentViewReportDto
            {
                Oid = report.Oid,
                DocumentTypeDesignation = report.DocumentTypeDesignation,
                DocumentNumber = report.DocumentNumber,
                DocumentDate = report.DocumentDate,
                DocumentTotal = report.DocumentTotal,
                TotalTax = report.TotalTax,
                CreditAmount = report.CreditAmount,
                DebitAmount = report.DebitAmount,
                Payed = report.Payed,
            };

            return dto;
        }

        public static FinancePaymentViewReportDto GetFinancePaymentViewReportDto(
            FinancePaymentViewReport report)
        {
            var dto = new FinancePaymentViewReportDto
            {
                Oid = report.Oid,
                DocumentTypeDesignation = report.DocumentTypeDesignation,
                DocumentTypeResourceString = report.DocumentTypeResourceString,
                DocumentTypeResourceStringReport = report.DocumentTypeResourceStringReport,
                PaymentRefNo = report.PaymentRefNo,
                PaymentStatus = report.PaymentStatus,
                PaymentAmount = report.PaymentAmount,
                TaxPayable = report.TaxPayable,
                PaymentDate = report.PaymentDate,
                DocumentDate = report.DocumentDate,
                ExtendedValue = report.ExtendedValue,
                EntityCode = report.EntityCode,
                EntityName = report.EntityName,
                EntityAddress = report.EntityAddress,
                EntityZipCode = report.EntityZipCode,
                EntityCity = report.EntityCity,
                EntityLocality = report.EntityLocality,
                EntityCountry = report.EntityCountry,
                EntityFiscalNumber = report.EntityFiscalNumber,
                MethodCode = report.MethodCode,
                PaymentMethodDesignation = report.PaymentMethodDesignation,
                CurrencyDesignation = report.CurrencyDesignation,
                CurrencyAcronym = report.CurrencyAcronym,
                CurrencySymbol = report.CurrencySymbol,
                ExchangeRate = report.ExchangeRate,
                Notes = report.Notes,

            };

            dto.DocumentFinancePaymentDocument =
                 report
                    .DocumentFinancePaymentDocument
                    .ConvertAll(
                        detail => GetFinancePaymentDocumentViewReportDto(detail));

            return dto;
        }

    }
}
