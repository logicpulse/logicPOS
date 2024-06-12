using System;

namespace LogicPOS.Settings
{
    public static class DocumentSettings
    {
        public static bool DocumentFinanceSeriesGenerationFactoryUseRandomAcronymPrefix { get; set; }
        public static string DocumentFinanceSeriesGenerationFactoryAcronymLastSerieFormat { get; set; }
        public static int HashControl { get; set; }
        public static Guid SimplifiedInvoiceId { get; set; } = new Guid("2c69b109-318a-4375-a573-28e5984b6503");
        public static Guid InvoiceAndPaymentId { get; set; } = new Guid("09b6aa6e-dc0e-41fd-8dbe-8678a3d11cbc");
        public static Guid DebitNoteId { get; set; } = new Guid("3942d940-ed13-4a62-a352-97f1ce006d8a");
      
        public static Guid XpoOidDocumentFinanceTypeOwnAssetsDriveGuide { get; set; } = new Guid("f8e96786-fed8-4143-be9e-b03c3a984a2c");
        public static Guid XpoOidDocumentFinanceTypeConsignmentGuide { get; set; } = new Guid("63d8eb04-983c-4524-96de-979a240b362c");
        public static Guid XpoOidDocumentFinanceTypeReturnGuide { get; set; } = new Guid("f03d2788-bed6-41ab-8d44-100039103e83");

        public static Guid XpoOidDocumentFinanceTypeConferenceDocument { get; set; } = new Guid("afed98d3-eae7-43a7-a7be-515753594c8f");
        public static Guid ConsignationInvoiceId { get; set; } = new Guid("b8554d36-642a-4083-b608-8f1da35f0fec");
        public static Guid XpoOidDocumentFinanceTypeBudget { get; set; } = new Guid("005ac531-31a1-44bb-9346-058f9c9ad01a");
        public static Guid XpoOidDocumentFinanceTypeProformaInvoice { get; set; } = new Guid("6f4249d0-4aaf-4711-814f-7f9533a1ef7f");
        public static Guid PaymentDocumentTypeId { get; set; } = new Guid("a009168d-fed1-4f52-b9e3-77e280b18ff5");
        public static Guid XpoOidDocumentFinanceTypeInvoiceWayBill { get; set; } = new Guid("f8878cf5-0f88-4270-8a55-1fc2488d81a2");
        public static Guid XpoOidDocumentFinanceTypeCurrentAccountInput { get; set; } = new Guid("235f06f3-5ec3-4e13-977b-325614b07e35");
    }
}
