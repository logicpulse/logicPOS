using System;

namespace LogicPOS.Settings
{
    public static class NotificationSettings
    {

        public static int XpoOidSystemNotificationDaysBackWhenFiltering { get; set; } = 5;
        public static Guid XpoOidSystemNotificationTypeNewTerminalRegistered { get; set; } = new Guid("bc1f6a82-fa8e-49c8-981c-46ff21aef8b4");
        public static Guid XpoOidSystemNotificationTypeCurrentAccountDocumentsToInvoice { get; set; } = new Guid("06319d46-e7b5-4cca-8257-55eff4cfe0fa");
        public static Guid XpoOidSystemNotificationTypeConsignationInvoiceDocumentsToInvoice { get; set; } = new Guid("a567578b-53e9-4b5c-848f-183c65194971");
        public static Guid XpoOidSystemNotificationTypeSaftDocumentTypeMovementOfGoods { get; set; } = new Guid("80a03838-0937-4ae3-921f-75a1e358f7bf");
    }
}
