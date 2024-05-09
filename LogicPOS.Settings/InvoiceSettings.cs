using System;

namespace LogicPOS.Settings
{
    public static class InvoiceSettings
    {
        public static Guid XpoOidDocumentFinanceTypeInvoice { get; set; } = new Guid("7af04618-74a6-42a3-aaba-454b7076f5a6");

        public static Guid FinalConsumerId { get; set; } = new Guid("0cf40622-578b-417d-b50f-e945fefb5d68");
        public static Guid XpoOidConfigurationVatRateDutyFree { get; set; } = new Guid("e74faad7-f5c9-4206-a662-f95820014195");
        public static Guid XpoOidConfigurationVatRateNormalPT { get; set; } = new Guid("cee00590-7317-41b8-af46-66560401096b");
        public static Guid XpoOidConfigurationVatRateIntermediatePT { get; set; } = new Guid("f73e3b41-4967-48c6-9f9a-260abf2146e1");
        public static Guid XpoOidConfigurationVatRateReducedPT { get; set; } = new Guid("7e89eaed-ce56-4565-8eec-98f2e8d004a5");
        public static Guid XpoOidConfigurationVatRateNormalPTMA { get; set; } = new Guid("ecd64d02-5249-4303-a35c-c662ffba4844");
        public static Guid XpoOidConfigurationVatRateIntermediatePTMA { get; set; } = new Guid("f0281b91-83d7-482f-bfd8-e52461983136");
        public static Guid XpoOidConfigurationVatRateReducedPTMA { get; set; } = new Guid("b57d85a5-843e-4b84-9660-9124006b9b05");
        public static Guid XpoOidConfigurationVatRateNormalPTAC { get; set; } = new Guid("389661c1-05f6-4830-bc06-176e2fdb3dc2");
        public static Guid XpoOidConfigurationVatRateIntermediatePTAC { get; set; } = new Guid("52c6ce3c-9246-4b8b-a143-b84733a074d4");
        public static Guid XpoOidConfigurationVatRateReducedPTAC { get; set; } = new Guid("e4478dea-9272-4090-a71a-df775b96c4b3");
        public static Guid XpoOidConfigurationPaymentMethodCurrentAccount { get; set; } = new Guid("6db009fd-6729-4353-a4d0-d599c4c19297");
        public static Guid XpoOidConfigurationPaymentMethodInstantPayment { get; set; } = new Guid("4261daa6-c0bd-4ac9-949a-cae0be2dd472");
        public static Guid XpoOidConfigurationVatExemptionReasonM99 { get; set; } = new Guid("f60f97c0-390e-4d76-90d7-204b6ea57949");

        public static int GetSimplifiedInvoiceMaxItems(Guid countryId)
        {
            if (CultureSettings.CountryIdIsPortugal(countryId))
            {
                return PluginSettings.HasPlugin
                    ? PluginSettings.PluginSoftwareVendor.GetSimplifiedInvoiceMaxItems()
                    : 1000;

            }

            return 999999999;
        }

        public static int GetSimplifiedInvoiceMaxServices(Guid countryId)
        {
            if (CultureSettings.CountryIdIsPortugal(countryId))
            {
                return PluginSettings.HasPlugin
                    ? PluginSettings.PluginSoftwareVendor.GetSimplifiedInvoiceMaxServices()
                    : 100;
            }

            return 999999999;
        }

    }
}
