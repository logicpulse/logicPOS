using System;

namespace LogicPOS.Shared.CustomDocument
{
    public static class CustomDocumentSettings
    {
        public static Guid CreditNoteDocumentTypeId { get; set; } = new Guid("fa924162-beed-4f2f-938d-919deafb7d47");
        public static Guid DeliveryNoteDocumentTypeId { get; set; } = new Guid("95f6a472-1b12-43aa-a215-a4b406b18924");
        public static Guid TransportDocumentTypeId { get; set; } = new Guid("96bcf534-0dab-48bb-a69e-166e81ae6f7b");
    }
}
