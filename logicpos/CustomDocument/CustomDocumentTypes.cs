using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.Settings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.CustomDocument
{
    public static class CustomDocumentTypes
    {
        public static readonly CustomDocumentType TransportDocument = new CustomDocumentType(
            CultureResources.GetResourceByLanguage(
                GeneralSettings.Settings.GetCultureName(),
                "global_documentfinance_type_title_gt"),
            DocumentSettings.TransportDocumentId,
            Domain.Enums.DocumentType.WayBill,
            "GT",
            false,
            true,
            0,
            true,
            true,
            true,
            2,
            false);

        public static readonly CustomDocumentType CreditSlip = new CustomDocumentType(
            CultureResources.GetResourceByLanguage(
                GeneralSettings.Settings.GetCultureName(),
                "global_documentfinance_type_title_nc"),
            DocumentSettings.XpoOidDocumentFinanceTypeCreditNote,
            Domain.Enums.DocumentType.Invoice,
            "NC",
            false,
            false,
            1,
            false,
            true,
            true,
            1,
            false);

        public static readonly CustomDocumentType DeliveryNote = new CustomDocumentType(
            CultureResources.GetResourceByLanguage(
                GeneralSettings.Settings.GetCultureName(),
                "global_documentfinance_type_title_gr"),
            DocumentSettings.XpoOidDocumentFinanceTypeDeliveryNote,
            LogicPOS.Domain.Enums.DocumentType.WayBill,
            "GR",
            false,
            true,
            0,
            true,
            true,
            true,
            2,
            false);

        public static IEnumerable<CustomDocumentType> All
        {
            get
            {
                yield return CreditSlip;
                yield return TransportDocument;
                yield return DeliveryNote;
            }
        }

        public static CustomDocumentType GetDocumentTypeById(Guid oid)
        {
            var documentType = All.FirstOrDefault(documentT => documentT.Id.Equals(oid));
            return documentType;
        }
    }
}
