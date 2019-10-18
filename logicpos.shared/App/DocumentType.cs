using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using logicpos.resources.Resources.Localization;

namespace logicpos.shared.App
{
    /// <summary>
    /// Enum for DocumentTypes and their details:
    /// TRANSPORT_DOCUMENT
    /// CREDIT_SLIP       
    /// DELIVERY_NOTE     
    /// </summary>
    public class DocumentType
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly DocumentType TRANSPORT_DOCUMENT  = new DocumentType(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type_title_gt"), SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide, logicpos.shared.Enums.DocumentType.WayBill, "GT", false, true,  0, true,    true, true, 2, false);
        public static readonly DocumentType CREDIT_SLIP         = new DocumentType(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type_title_nc"), SettingsApp.XpoOidDocumentFinanceTypeCreditNote,          logicpos.shared.Enums.DocumentType.Invoice, "NC", false, false, 1, false,   true, true, 1, false);
        public static readonly DocumentType DELIVERY_NOTE       = new DocumentType(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_documentfinance_type_title_gr"), SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote,        logicpos.shared.Enums.DocumentType.WayBill, "GR", false, true,  0, true,    true, true, 2, false);

        public static IEnumerable<DocumentType> Values
        {
            get
            {
                yield return CREDIT_SLIP;
                yield return TRANSPORT_DOCUMENT;
                yield return DELIVERY_NOTE;
            }
        }

        public string Designation { get; private set; }
        public Guid Oid { get; private set; }
        public logicpos.shared.Enums.DocumentType DocumentTypeGlobal { get; private set; }
        public string Acronym { get; private set; }
        public bool Payed { get; private set; }
        public bool Credit { get; private set; }
        public int CreditDebit { get; private set; }
        public bool WayBill { get; private set; }
        public bool WsAtDocument { get; private set; }
        public bool SaftAuditFile { get; private set; }
        public int SaftDocumentType { get; private set; }
        public bool StockMode { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="designation"></param>
        /// <param name="oid"></param>
        /// <param name="documentTypeGlobal"></param>
        DocumentType(string designation, Guid oid, logicpos.shared.Enums.DocumentType documentTypeGlobal, string acronym, bool payed, bool credit, int creditDebit, bool wayBill, bool wsAtDocument, bool saftAuditFile, int saftDocumentType, bool stockMode)
        {
            Designation         = designation;
            Oid                 = oid;
            DocumentTypeGlobal  = documentTypeGlobal;
            Acronym             = acronym;
            Payed               = payed;
            Credit              = credit;
            CreditDebit         = creditDebit;
            WayBill             = wayBill;
            WsAtDocument        = wsAtDocument;
            SaftAuditFile       = saftAuditFile;
            SaftDocumentType    = saftDocumentType;
            StockMode           = stockMode;
        }

        /// <summary>
        /// Returns the DocumentType based on oid passed as parameter.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public static DocumentType GetDocumentTypeByOid(Guid oid)
        {
            var documentType = Values.FirstOrDefault(documentT => documentT.Oid.Equals(oid));
            return documentType;
        }
    }
}