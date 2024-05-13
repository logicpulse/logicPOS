using System;

namespace LogicPOS.Shared.CustomDocument
{
    public class CustomDocumentType
    {

        public string Designation { get; private set; }
        public Guid Id { get; private set; }
        public Domain.Enums.DocumentType DocumentTypeGlobal { get; private set; }
        public string Acronym { get; private set; }
        public bool Payed { get; private set; }
        public bool Credit { get; private set; }
        public int CreditDebit { get; private set; }
        public bool WayBill { get; private set; }
        public bool WsAtDocument { get; private set; }
        public bool SaftAuditFile { get; private set; }
        public int SaftDocumentType { get; private set; }
        public bool StockMode { get; private set; }


        public CustomDocumentType(
            string designation,
            Guid oid,
            Domain.Enums.DocumentType documentTypeGlobal,
            string acronym,
            bool payed,
            bool credit,
            int creditDebit,
            bool wayBill,
            bool wsAtDocument,
            bool saftAuditFile,
            int saftDocumentType,
            bool stockMode)
        {
            Designation = designation;
            Id = oid;
            DocumentTypeGlobal = documentTypeGlobal;
            Acronym = acronym;
            Payed = payed;
            Credit = credit;
            CreditDebit = creditDebit;
            WayBill = wayBill;
            WsAtDocument = wsAtDocument;
            SaftAuditFile = saftAuditFile;
            SaftDocumentType = saftDocumentType;
            StockMode = stockMode;
        }
    }
}