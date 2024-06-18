using LogicPOS.Globalization;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Shared.CustomDocument
{
    public static class CustomDocumentTypes
    {
        public static readonly CustomDocumentType TransportDocument = new CustomDocumentType(
            CultureResources.GetResourceByLanguage(
                CultureSettings.CurrentCultureName,
                "global_documentfinance_type_title_gt"),
            CustomDocumentSettings.TransportDocumentTypeId,
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

        public static readonly CustomDocumentType CreditNote = new CustomDocumentType(
            CultureResources.GetResourceByLanguage(
                CultureSettings.CurrentCultureName,
                "global_documentfinance_type_title_nc"),
            CustomDocumentSettings.CreditNoteId,
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
                CultureSettings.CurrentCultureName,
                "global_documentfinance_type_title_gr"),
            CustomDocumentSettings.DeliveryNoteDocumentTypeId,
            Domain.Enums.DocumentType.WayBill,
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
                yield return CreditNote;
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
