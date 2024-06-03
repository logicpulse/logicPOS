using DevExpress.Xpo;
using LogicPOS.Domain.Entities;
using System;

namespace LogicPOS.Data.XPO.Settings
{
    public static class XPOSettings
    {
        public static sys_userdetail LoggedUser { get; set; }
        public static Session Session { get; set; }

        public static cfg_configurationcountry ConfigurationSystemCountry { get; set; } = null;
        public static cfg_configurationcurrency ConfigurationSystemCurrency { get; set; } = null;

        public static pos_worksessionperiod WorkSessionPeriodDay { get; set; }
        public static pos_worksessionperiod WorkSessionPeriodTerminal { get; set; }

        public static Guid XpoOidArticleDefaultType { get; set; } = new Guid("edf4841e-e451-4c7b-9bd0-ee02860ba937");
        public static Guid XpoOidArticleDefaultClass { get; set; } = new Guid("6924945d-f99e-476b-9c4d-78fb9e2b30a3");
        public static Guid XpoOidArticleDefaultVatDirectSelling { get; set; } = new Guid("cee00590-7317-41b8-af46-66560401096b");
        public static Guid XpoOidArticleDefaultVatOnTable { get; set; } = new Guid("cee00590-7317-41b8-af46-66560401096b");
        public static Guid XpoOidArticleDefaultUnitMeasure { get; set; } = new Guid("4c81aa20-98ec-4497-b740-165cdb5fa395");
        public static Guid XpoOidArticleDefaultUnitSize { get; set; } = new Guid("18f564aa-7da5-4a1c-9091-8014638b818c");
        public static Guid XpoOidArticleDefaultTemplate { get; set; } = new Guid("a537cad3-ebf4-4df4-bd8d-28009ad226a2");
        public static string DefaultValueUserDetailButtonImage { get; set; } = @"Images\Icons\Users\icon_user_default.png";
        public static string DefaultValueUserDetailAccessPin { get; set; } = "0000";
        public static Guid XpoOidConfigurationPrintersTemplateTicket { get; set; } = new Guid("a537cad3-ebf4-4df4-bd8d-74209ad226a2");
        public static Guid XpoOidConfigurationPrintersTemplateTableConsult { get; set; } = new Guid("f6a25476-40b0-46d7-9104-d5db3f50d7f1");
        public static Guid XpoOidUndefinedRecord { get; set; } = new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid XpoOidUserRecord { get; set; } = new Guid("00000000-0000-0000-0000-000000000002");
        public static string XpoOidHiddenRecordsFilter { get; set; } = "00000000-0000-0000-0000-000000000%";
        public static Guid XpoOidArticleClassCustomerCard { get; set; } = new Guid("49ea35ba-35f3-440f-946e-ab32578ed741");
        public static Guid XpoOidConfigurationPriceTypeDefault { get; set; } = new Guid("cf17a218-b687-4b82-a8f4-0905594ac1f5");
        public static Guid XpoOidArticleParkingTicket { get; set; } = new Guid("f4c6294d-0a57-4f36-951d-87ab2e076ef1");
        public static Guid XpoOidArticleParkingCard { get; set; } = new Guid("32829702-33fa-48d5-917c-4c1db8720777");
    }
}
