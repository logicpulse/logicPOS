using System;

namespace logicpos.financial.console.App
{
    public class SettingsApp : logicpos.financial.library.App.SettingsApp
    {
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Developer

        #if (DEBUG) 
        public static string DatabaseName = "logicposdb_20160919_scripts";
        public static string AppHardwareId = "92A4-3CA3-0CFD-4FF4-2962-5379";
        #else
        public static string DatabaseName = "logicposdb";
        public static string AppHardwareId = String.Empty;
        #endif

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Application

        public static string AppName = "Framework Console Test Project";

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Formats

        //Dont change This, Required to use . in many things like SAF-T etc
        public static string CultureNumberFormat = "pt-PT";
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Add On Parameters to Console Test Project

        //Printers
        public static Guid XpoOidPrinterExportPDF = new Guid("4be24552-4e08-4916-a964-65fdacb523ac");
        public static Guid XpoOidPrinterThermal = new Guid("0c0dc95c-16e9-4d8a-ba3e-86e8f07e073d");

        //Customer
        public static Guid XpoOidDocumentCustomer = new Guid("765859cc-29c2-4925-be89-0486d03684f2");//Carlos Fernandes : Discount 5%
        //public static Guid XpoOidDocumentCustomer = new Guid("b1045682-7559-49a8-9f3e-3cf9cde80f23");//Null Customer, All Blank Fields

        //Currency
        //public static Guid XpoOidDocumentCurrency = new Guid("28dd2a3a-0083-11e4-96ce-00ff2353398c");//Euro/EUR
        public static Guid XpoOidDocumentCurrency = new Guid("28da9212-3423-11e4-96ce-00ff2353398c");//Kwanza|AOA

        //PaymentConditions : Pronto Pagamento
        public static Guid XpoOidDocumentPaymentCondition = new Guid("4261daa6-c0bd-4ac9-949a-cae0be2dd472");
        
        //PaymentMethod : Cartão de Crédito
        public static Guid XpoOidDocumentPaymentMethod = new Guid("fa7ae225-b0c3-4ebc-a1f2-aa8ffef0c59d");

        //Finance Documents
        //FT FT5TEHDX4012016S001/37 - Fatura - Carlos Fernandes
        //public static Guid XpoPrintFinanceDocument = new Guid("6312a079-8f9e-4373-ad3b-e3f50f64c3fe");
        //FS FS5TEHDX4012016S001/31 - Fatura Simplificada - Consumidor Final
        //public static Guid XpoPrintFinanceDocument = new Guid("3afe1e32-9197-4e14-ae44-e12183f9be28");
        //FS FS5TEHDX4012016S001/36 - Fatura Simplificada - Consumidor Final - Many Taxs
        //public static Guid XpoPrintFinanceDocument = new Guid("8c2e1def-f70d-4cc2-9afa-ee7849ccd55c");
        //FS FS5TEHDX4012016S001/45 - Fatura Simplificada - Carlos Fernandes - Many Taxs - With Many VatExemptionReason - Kwanza
        //public static Guid XpoPrintFinanceDocument = new Guid("de82fbea-9174-4888-9dd1-380781270bab");
        //FS FS5TEHDX4012016S001/38 - Fatura Simplificada - Consumidor Final - TotalDelivery/TotalChange
        //public static Guid XpoPrintFinanceDocument = new Guid("94249673-0333-4a86-84a5-c40ea13ea420");
        //GR GR5TEHDX4012016S001/2 - Guia de Remessa -  Luis Santos 
        //public static Guid XpoPrintFinanceDocument = new Guid("da1ac1c5-d4e0-40b3-bbd9-78623f0891f4");
        //NC NC5TEHDX4012016S001/4 - Nota de Crédito - Mário Fernandes
        //public static Guid XpoPrintFinanceDocument = new Guid("edc87cd4-4b1b-4507-b9a2-8c8c83a62ba4");
        //DC DC5TEHDX4012016S001/66 - Documento de Conferência - Consumidor Final
        public static Guid XpoPrintFinanceDocument = new Guid("1900a299-e662-4d17-ae8f-0cddfc73646c");

        //Payment Documents
        //RC RC5TEHDX4012016S001/3 - Recibo - Eur
        public static Guid XpoPrintFinanceDocumentPayment = new Guid("60fc3747-8859-44a1-a2a4-ee46d7a24f19");
        //RC RC5TEHDX4012016S001/4 - Recibo - Kwanza
        //public static Guid XpoPrintFinanceDocumentPayment = new Guid("200f1f5b-0bf0-455d-acf5-7f289f4531b6");
        
        //OrderMain/Tickets
        public static Guid XpoPrintDocumentOrderTicket = new Guid("e936c649-14a4-4190-8144-d23848f7b504");

        //WorkSessions
        //Day
        //public static Guid XpoPrintWorkSessionPeriod = new Guid("d1d7be17-3a77-4212-9fe3-3f74494f4a8f");
        //Terminal
        //public static Guid XpoPrintWorkSessionPeriod = new Guid("a0e8a485-b386-40a6-bdf9-613cbfe19ff0");

        //NEW SESSION TESTS
        //Day
        //public static Guid XpoPrintWorkSessionPeriod = new Guid("b19b48c5-5394-4048-b219-21927fb37d67");
        //Terminal
        //public static Guid XpoPrintWorkSessionPeriod = new Guid("a7eadcb2-e9a8-43f9-8a7e-78c016b01312");
        
        //New Datbase
        //Day
        //public static Guid XpoPrintWorkSessionPeriod = new Guid("6c187b96-5c6c-4dc5-8fbe-8626906280cc");
        //Terminal
        //public static Guid XpoPrintWorkSessionPeriod = new Guid("ea902c7c-6a6b-49b8-9b83-e479ae913d15");
        public static Guid XpoPrintWorkSessionPeriod = new Guid("3e33065a-66a8-44bc-9df7-507ec06d6b99");
    }
}
