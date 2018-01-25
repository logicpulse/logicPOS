using System;
using DevExpress.Xpo;
using logicpos.datalayer;
using logicpos.datalayer.Enums;
using logicpos.datalayer.App;

namespace logicpos.datalayer.DataLayer.Xpo
{
    public class InitFixtures
    {
        private static Session _session;

        /// <summary>
        /// Initialize Database Fixtures
        /// </summary>
        /// <param name="pSession"></param>
        public static void InitUserAndTerminal(Session pSession)
        {
            _session = pSession;

            //User
            XPCollection xpcUser = new XPCollection(_session, typeof(SYS_UserDetail));
            if (xpcUser.Count == 0)
            {
                var user1 = new SYS_UserDetail(_session) { Ord = 10, Code = 10, Name = "Administrador", AccessPin = "0000" }; user1.UpdatedBy = user1; user1.Save();
                var user2 = new SYS_UserDetail(_session) { Ord = 20, Code = 20, Name = "Barman", AccessPin = "1111" }; user2.UpdatedBy = user1; user2.Save();

                //ConfigurationPlaceTerminal
                XPCollection _xpcConfigurationPlaceTerminal = new XPCollection(_session, typeof(POS_ConfigurationPlaceTerminal));
                if (xpcUser.Count == 0)
                {
                    var configurationPlaceTerminal1 = new POS_ConfigurationPlaceTerminal(_session) { Ord = 10, Code = 10, Designation = "Terminal #1", HardwareId = "####-####-####-####-####-###1" }; configurationPlaceTerminal1.UpdatedBy = user1; configurationPlaceTerminal1.Save();
                    var configurationPlaceTerminal2 = new POS_ConfigurationPlaceTerminal(_session) { Ord = 20, Code = 20, Designation = "Terminal #2", HardwareId = "####-####-####-####-####-###2", UpdatedBy = user1 }; configurationPlaceTerminal2.Save();
                    var configurationPlaceTerminal3 = new POS_ConfigurationPlaceTerminal(_session) { Ord = 30, Code = 30, Designation = "Terminal #3", HardwareId = "####-####-####-####-####-###3", UpdatedBy = user1 }; configurationPlaceTerminal3.Save();
                };
            }

            //UserPermissionGroup
            XPCollection xpcUserProfile = new XPCollection(_session, typeof(SYS_UserProfile));
            SYS_UserProfile userProfile1 = null;
            SYS_UserProfile userProfile2 = null;
            SYS_UserProfile userProfile3 = null;

            //UserProfile
            if (xpcUserProfile.Count == 0)
            {
                userProfile1 = new SYS_UserProfile(_session) { Code = 10, Designation = "Administração" }; userProfile1.Save();
                userProfile2 = new SYS_UserProfile(_session) { Code = 20, Designation = "Empregado" }; userProfile2.Save();
                userProfile3 = new SYS_UserProfile(_session) { Code = 30, Designation = "Supervisor" }; userProfile3.Save();
            };

            //UserPermissionGroup
            XPCollection xpcUserPermissionGroup = new XPCollection(_session, typeof(SYS_UserPermissionGroup));
            SYS_UserPermissionGroup userPermissionGroup1 = null;
            SYS_UserPermissionGroup userPermissionGroup2 = null;
            SYS_UserPermissionGroup userPermissionGroup3 = null;

            if (xpcUserPermissionGroup.Count == 0)
            {
                userPermissionGroup1 = new SYS_UserPermissionGroup(_session) { Code = 10, Designation = "Administração" }; userPermissionGroup1.Save();
                userPermissionGroup2 = new SYS_UserPermissionGroup(_session) { Code = 20, Designation = "Mesas" }; userPermissionGroup2.Save();
                userPermissionGroup3 = new SYS_UserPermissionGroup(_session) { Code = 30, Designation = "Relatórios" }; userPermissionGroup3.Save();
            };

            //UserPermissionItem
            SYS_UserPermissionItem userPermissionItem1 = null;
            SYS_UserPermissionItem userPermissionItem2 = null;
            SYS_UserPermissionItem userPermissionItem3 = null;
            XPCollection xpcUserPermissionItem = new XPCollection(_session, typeof(SYS_UserPermissionItem));
            if (xpcUserPermissionItem.Count == 0)
            {
                userPermissionItem1 = new SYS_UserPermissionItem(_session) { Token = "TABLE_ALL", Designation = "Mesas – Todas as operações #1" }; userPermissionItem1.Save();
                userPermissionItem2 = new SYS_UserPermissionItem(_session) { Token = "TOTALS_ALL", Designation = "Totais – Todas as operações" }; userPermissionItem2.Save();
                userPermissionItem3 = new SYS_UserPermissionItem(_session) { Token = "ORDER_ALL", Designation = "Pedidos – Todas as operações" }; userPermissionItem3.Save();
            }

            //UserPermissionProfile
            XPCollection xpcUserPermissionProfile = new XPCollection(_session, typeof(SYS_UserPermissionProfile));
            if (xpcUserPermissionProfile.Count == 0)
            {
                var userPermissionProfile1 = new SYS_UserPermissionProfile(_session) { PermissionItem = userPermissionItem1, UserProfile = userProfile1 };
                userPermissionProfile1.Save();
            }

            //SystemAuditType|SystemAudit
            XPCollection xpcSystemAuditType = new XPCollection(_session, typeof(SYS_SystemAuditType));
            if (xpcSystemAuditType.Count == 0)
            {
                var systemAuditType1 = new SYS_SystemAuditType(_session) { Ord = 10, Code = 10, Token = "APP_START", Designation = "Aplicação Iniciada", ResourceString = "systemaudittype_app_start" }; systemAuditType1.Save();
                var systemAuditType2 = new SYS_SystemAuditType(_session) { Ord = 20, Code = 20, Token = "APP_CLOSE", Designation = "Aplicação Encerrada", ResourceString = "systemaudittype_app_close" }; systemAuditType2.Save();

                XPCollection xpcSystemAudit = new XPCollection(_session, typeof(SYS_SystemAudit));
                if (xpcSystemAudit.Count == 0)
                {
                    var systemAudit1 = new SYS_SystemAudit(_session) { Date = DateTime.Now, AuditType = systemAuditType1 }; systemAudit1.Save();
                    var systemAudit2 = new SYS_SystemAudit(_session) { Date = DateTime.Now, AuditType = systemAuditType2 }; systemAudit2.Save();
                };
            };

            //SYS_SystemAuditATWS
            XPCollection xpcSystemAuditATWS = new XPCollection(_session, typeof(SYS_SystemAuditAT));
            if (xpcSystemAuditATWS.Count == 0)
            {
                var systemAuditATWS1 = new SYS_SystemAuditAT(_session) { Date = DateTime.Now, Type = SystemAuditATWSType.Document }; systemAuditATWS1.Save();
            }
        }

        public static void InitOther(Session pSession)
        {
            //Init Parameters
            _session = pSession;

            //ArticleType
            XPCollection xpcArticleType = new XPCollection(_session, typeof(FIN_ArticleType));
            if (xpcArticleType.Count == 0)
            {
                var articleType1 = new FIN_ArticleType(_session) { Ord = 10, Code = 10, Designation = "Normal" }; articleType1.Save();
                var articleType2 = new FIN_ArticleType(_session) { Ord = 20, Code = 20, Designation = "Complemento" }; articleType2.Save();
                var articleType3 = new FIN_ArticleType(_session) { Ord = 30, Code = 30, Designation = "Consumo" }; articleType3.Save();
                var articleType4 = new FIN_ArticleType(_session) { Ord = 40, Code = 40, Designation = "Gorjeta" }; articleType4.Save();
                var articleType5 = new FIN_ArticleType(_session) { Ord = 50, Code = 50, Designation = "Informativo" }; articleType5.Save();
            };

            //ArticleClass
            XPCollection xpcArticleClass = new XPCollection(_session, typeof(FIN_ArticleClass));
            if (xpcArticleClass.Count == 0)
            {
                var articleClass1 = new FIN_ArticleClass(_session) { Ord = 10, Code = 10, Designation = "Produtos", Acronym = "P" }; articleClass1.Save();
                var articleClass2 = new FIN_ArticleClass(_session) { Ord = 20, Code = 20, Designation = "Serviços", Acronym = "S" }; articleClass2.Save();
                var articleClass3 = new FIN_ArticleClass(_session) { Ord = 30, Code = 30, Designation = "Outros", Acronym = "O" }; articleClass3.Save();
            };

            //ArticleStock
            XPCollection xpcArticleStock = new XPCollection(_session, typeof(FIN_ArticleStock));
            if (xpcArticleStock.Count == 0)
            {
                var articleStock1 = new FIN_ArticleStock(_session) { Date = DateTime.Now, DocumentNumber = "Doc1" }; articleStock1.Save();
                var articleStock2 = new FIN_ArticleStock(_session) { Date = DateTime.Now, DocumentNumber = "Doc2" }; articleStock2.Save();
                var articleStock3 = new FIN_ArticleStock(_session) { Date = DateTime.Now, DocumentNumber = "Doc3" }; articleStock3.Save();
            };

            //ConfigurationCashRegister
            XPCollection _xpcConfigurationCashRegister = new XPCollection(_session, typeof(POS_ConfigurationCashRegister));
            if (xpcArticleType.Count == 0)
            {
                var configurationCashRegister1 = new POS_ConfigurationCashRegister(_session) { Ord = 10, Code = 10, Designation = "CashRegister #1" }; configurationCashRegister1.Save();
                var configurationCashRegister2 = new POS_ConfigurationCashRegister(_session) { Ord = 20, Code = 20, Designation = "CashRegister #2" }; configurationCashRegister2.Save();
                var configurationCashRegister3 = new POS_ConfigurationCashRegister(_session) { Ord = 30, Code = 30, Designation = "CashRegister #3" }; configurationCashRegister3.Save();
                var configurationCashRegister4 = new POS_ConfigurationCashRegister(_session) { Ord = 40, Code = 40, Designation = "CashRegister #4" }; configurationCashRegister4.Save();
                var configurationCashRegister5 = new POS_ConfigurationCashRegister(_session) { Ord = 50, Code = 50, Designation = "CashRegister #5" }; configurationCashRegister5.Save();
            };

            //ConfigurationPlaceMovementType
            XPCollection xpcConfigurationPlaceMovementType = new XPCollection(_session, typeof(POS_ConfigurationPlaceMovementType));
            if (xpcConfigurationPlaceMovementType.Count == 0)
            {
                var configurationPlaceMovementType1 = new POS_ConfigurationPlaceMovementType(_session) { Ord = 10, Code = 10, Designation = "Normal" }; configurationPlaceMovementType1.Save();
                var configurationPlaceMovementType2 = new POS_ConfigurationPlaceMovementType(_session) { Ord = 20, Code = 20, Designation = "Consumo Próprio" }; configurationPlaceMovementType2.Save();
                var configurationPlaceMovementType3 = new POS_ConfigurationPlaceMovementType(_session) { Ord = 30, Code = 30, Designation = "Take-Away" }; configurationPlaceMovementType3.Save();
                var configurationPlaceMovementType4 = new POS_ConfigurationPlaceMovementType(_session) { Ord = 40, Code = 40, Designation = "Entrega ao Domicilio" }; configurationPlaceMovementType4.Save();
            };

            //ConfigurationUnitMeasure
            XPCollection xpcConfigurationUnitMeasure = new XPCollection(_session, typeof(CFG_ConfigurationUnitMeasure));
            if (xpcConfigurationUnitMeasure.Count == 0)
            {
                var configurationUnitMeasure1 = new CFG_ConfigurationUnitMeasure(_session) { Ord = 10, Code = 10, Designation = "Unidade", Acronym = "Un" }; configurationUnitMeasure1.Save();
                var configurationUnitMeasure2 = new CFG_ConfigurationUnitMeasure(_session) { Ord = 20, Code = 20, Designation = "Quilograma", Acronym = "Kg" }; configurationUnitMeasure2.Save();
                var configurationUnitMeasure3 = new CFG_ConfigurationUnitMeasure(_session) { Ord = 30, Code = 30, Designation = "Litro", Acronym = "L" }; configurationUnitMeasure3.Save();
                var configurationUnitMeasure4 = new CFG_ConfigurationUnitMeasure(_session) { Ord = 40, Code = 40, Designation = "Metro", Acronym = "m" }; configurationUnitMeasure4.Save();
            };

            //UserCommissionGroup
            XPCollection xpcUserCommissionGroup = new XPCollection(_session, typeof(POS_UserCommissionGroup));
            if (xpcUserCommissionGroup.Count == 0)
            {
                var userCommissionGroup1 = new POS_UserCommissionGroup(_session) { Ord = 10, Code = 10, Designation = "Não Tem" }; userCommissionGroup1.Save();
                var userCommissionGroup2 = new POS_UserCommissionGroup(_session) { Ord = 20, Code = 20, Designation = "Balcão" }; userCommissionGroup2.Save();
                var userCommissionGroup3 = new POS_UserCommissionGroup(_session) { Ord = 30, Code = 30, Designation = "Emp.Mesa" }; userCommissionGroup3.Save();
                var userCommissionGroup4 = new POS_UserCommissionGroup(_session) { Ord = 40, Code = 40, Designation = "Chefe de Mesa" }; userCommissionGroup4.Save();
                var userCommissionGroup5 = new POS_UserCommissionGroup(_session) { Ord = 50, Code = 50, Designation = "Gerente" }; userCommissionGroup5.Save();
                var userCommissionGroup6 = new POS_UserCommissionGroup(_session) { Ord = 60, Code = 60, Designation = "Cozinheiro" }; userCommissionGroup6.Save();
                var userCommissionGroup7 = new POS_UserCommissionGroup(_session) { Ord = 70, Code = 70, Designation = "Ajudante de Cozinha" }; userCommissionGroup7.Save();
            };

            //CustomerDiscountGroup
            XPCollection xpcCustomerDiscountGroup = new XPCollection(_session, typeof(ERP_CustomerDiscountGroup));
            if (xpcCustomerDiscountGroup.Count == 0)
            {
                var customerDiscountGroup1 = new ERP_CustomerDiscountGroup(_session) { Ord = 10, Code = 10, Designation = "Não Tem" }; customerDiscountGroup1.Save();
                var customerDiscountGroup2 = new ERP_CustomerDiscountGroup(_session) { Ord = 20, Code = 20, Designation = "Mínimo" }; customerDiscountGroup2.Save();
                var customerDiscountGroup3 = new ERP_CustomerDiscountGroup(_session) { Ord = 30, Code = 30, Designation = "Normal" }; customerDiscountGroup3.Save();
                var customerDiscountGroup4 = new ERP_CustomerDiscountGroup(_session) { Ord = 40, Code = 40, Designation = "Especial" }; customerDiscountGroup4.Save();
                var customerDiscountGroup5 = new ERP_CustomerDiscountGroup(_session) { Ord = 50, Code = 50, Designation = "VIP" }; customerDiscountGroup5.Save();
            };

            //ConfigurationVatRate
            XPCollection xpcConfigurationVatRate = new XPCollection(_session, typeof(FIN_ConfigurationVatRate));
            if (xpcConfigurationVatRate.Count == 0)
            {
                var configurationVatRate1 = new FIN_ConfigurationVatRate(_session) { Ord = 10, Code = 10, Designation = "Normal", Value = 23.00m }; configurationVatRate1.Save();
                var configurationVatRate2 = new FIN_ConfigurationVatRate(_session) { Ord = 20, Code = 20, Designation = "Intermédia", Value = 13.00m }; configurationVatRate2.Save();
                var configurationVatRate3 = new FIN_ConfigurationVatRate(_session) { Ord = 30, Code = 30, Designation = "Reduzido", Value = 6.00m }; configurationVatRate3.Save();
                var configurationVatRate4 = new FIN_ConfigurationVatRate(_session) { Ord = 40, Code = 40, Designation = "Isento", Value = 0.00m }; configurationVatRate4.Save();
            };

            //ConfigurationVatExemptionReason
            XPCollection xpcConfigurationVatExemptionReason = new XPCollection(_session, typeof(FIN_ConfigurationVatExemptionReason));
            if (xpcConfigurationVatExemptionReason.Count == 0)
            {
                var configurationVatExemptionReason1 = new FIN_ConfigurationVatExemptionReason(_session) { Ord = 10, Code = 10, Designation = "Artigo 16.º n.º 6 alínea c) do CIVA", StandardApplicable = "Artigo 16.º n.º 6 alínea c) do CIVA" }; configurationVatExemptionReason1.Save();
            };

            //ConfigurationUnitSize
            XPCollection xpcConfigurationUnitSize = new XPCollection(_session, typeof(CFG_ConfigurationUnitSize));
            if (xpcConfigurationUnitSize.Count == 0)
            {
                var unitSize1 = new CFG_ConfigurationUnitSize(_session) { Ord = 10, Code = 10, Designation = "Pequeno" }; unitSize1.Save();
                var unitSize2 = new CFG_ConfigurationUnitSize(_session) { Ord = 20, Code = 20, Designation = "Normal" }; unitSize2.Save();
                var unitSize3 = new CFG_ConfigurationUnitSize(_session) { Ord = 30, Code = 30, Designation = "Grande" }; unitSize3.Save();
            };

            //ConfigurationPaymentCondition
            XPCollection xpcConfigurationPaymentCondition = new XPCollection(_session, typeof(FIN_ConfigurationPaymentCondition));
            if (xpcConfigurationPaymentCondition.Count == 0)
            {
                var ConfigurationPaymentCondition1 = new FIN_ConfigurationPaymentCondition(_session) { Ord = 10, Code = 10, Acronym = "PP", Designation = "Pronto Pagamento" }; ConfigurationPaymentCondition1.Save();
                var ConfigurationPaymentCondition2 = new FIN_ConfigurationPaymentCondition(_session) { Ord = 20, Code = 20, Acronym = "30", Designation = "30 Dias" }; ConfigurationPaymentCondition2.Save();
                var ConfigurationPaymentCondition3 = new FIN_ConfigurationPaymentCondition(_session) { Ord = 30, Code = 30, Acronym = "60", Designation = "60 Dias" }; ConfigurationPaymentCondition3.Save();
            };

            //ConfigurationPaymentMethod
            XPCollection xpcConfigurationPaymentMethod = new XPCollection(_session, typeof(FIN_ConfigurationPaymentMethod));
            if (xpcConfigurationPaymentMethod.Count == 0)
            {
                var configurationPaymentMethod1 = new FIN_ConfigurationPaymentMethod(_session) { Ord = 10, Code = 10, Token = "MONEY", Designation = "Dinheiro", Acronym = "BN", ResourceString = "pos_button_label_payment_type_money", ButtonIcon = @"Icons\icon_pos_payment_type_money.png" }; configurationPaymentMethod1.Save();
                var configurationPaymentMethod2 = new FIN_ConfigurationPaymentMethod(_session) { Ord = 20, Code = 20, Token = "BANK_CHECK", Designation = "Cheques", Acronym = "BC", ResourceString = "pos_button_label_payment_type_bank_check", ButtonIcon = @"Icons\icon_pos_payment_type_bank_check.png" }; configurationPaymentMethod2.Save();
                var configurationPaymentMethod3 = new FIN_ConfigurationPaymentMethod(_session) { Ord = 30, Code = 30, Token = "CASH_MACHINE", Designation = "ATM", Acronym = "MB", ResourceString = "pos_button_label_payment_type_cash_machine", ButtonIcon = @"Icons\icon_pos_payment_type_cash_machine.png" }; configurationPaymentMethod3.Save();
                var configurationPaymentMethod4 = new FIN_ConfigurationPaymentMethod(_session) { Ord = 40, Code = 40, Token = "CREDIT_CARD", Designation = "Crédito", Acronym = "CR", ResourceString = "pos_button_label_payment_type_bank_credit_card", ButtonIcon = @"Icons\icon_pos_payment_type_credit_card.png" }; configurationPaymentMethod4.Save();
                var configurationPaymentMethod5 = new FIN_ConfigurationPaymentMethod(_session) { Ord = 50, Code = 50, Token = "VISA", Designation = "Pagamento via VISA", Acronym = "VS", ResourceString = "pos_button_label_payment_type_visa", ButtonIcon = @"Icons\icon_pos_payment_type_visa.png" }; configurationPaymentMethod5.Save();
                var configurationPaymentMethod6 = new FIN_ConfigurationPaymentMethod(_session) { Ord = 60, Code = 60, Token = "CURRENT_ACCOUNT", Designation = "Conta Corrent", Acronym = "CC", ResourceString = "pos_button_label_payment_type_current_account", ButtonIcon = @"Icons\icon_pos_payment_type_current_account.png" }; configurationPaymentMethod6.Save();
            };

            //ConfigurationPaymentMethod
            XPCollection xpcConfigurationKeyboard = new XPCollection(_session, typeof(POS_ConfigurationKeyboard));
            if (xpcConfigurationKeyboard.Count == 0)
            {
                var configurationKeyboard1 = new POS_ConfigurationKeyboard(_session) { Ord = 10, Code = 10, Designation = "Keyboard #1" }; configurationKeyboard1.Save();
                var configurationKeyboard2 = new POS_ConfigurationKeyboard(_session) { Ord = 20, Code = 20, Designation = "Keyboard #2" }; configurationKeyboard2.Save();
                var configurationKeyboard3 = new POS_ConfigurationKeyboard(_session) { Ord = 30, Code = 30, Designation = "Keyboard #3" }; configurationKeyboard3.Save();
                var configurationKeyboard4 = new POS_ConfigurationKeyboard(_session) { Ord = 40, Code = 40, Designation = "Keyboard #4" }; configurationKeyboard4.Save();
                var configurationKeyboard5 = new POS_ConfigurationKeyboard(_session) { Ord = 50, Code = 50, Designation = "Keyboard #5" }; configurationKeyboard5.Save();
            };

            //ConfigurationMaintenance
            XPCollection xpcConfigurationMaintenance = new XPCollection(_session, typeof(POS_ConfigurationMaintenance));
            if (xpcConfigurationMaintenance.Count == 0)
            {
                var configurationMaintenance1 = new POS_ConfigurationMaintenance(_session) { Ord = 10, Code = 10, Designation = "Keyboard #1" }; configurationMaintenance1.Save();
                var configurationMaintenance2 = new POS_ConfigurationMaintenance(_session) { Ord = 20, Code = 20, Designation = "Keyboard #2" }; configurationMaintenance2.Save();
                var configurationMaintenance3 = new POS_ConfigurationMaintenance(_session) { Ord = 30, Code = 30, Designation = "Keyboard #3" }; configurationMaintenance3.Save();
                var configurationMaintenance4 = new POS_ConfigurationMaintenance(_session) { Ord = 40, Code = 40, Designation = "Keyboard #4" }; configurationMaintenance4.Save();
                var configurationMaintenance5 = new POS_ConfigurationMaintenance(_session) { Ord = 50, Code = 50, Designation = "Keyboard #5" }; configurationMaintenance5.Save();
            };

            //ConfigurationCurrency
            XPCollection xpcConfigurationCurrency = new XPCollection(_session, typeof(CFG_ConfigurationCurrency));
            if (xpcConfigurationCurrency.Count == 0)
            {
                var configurationCurrency1 = new CFG_ConfigurationCurrency(_session) { Ord = 10, Code = 10, Acronym = "EUR", Designation = "Euro" }; configurationCurrency1.Save();
                var configurationCurrency2 = new CFG_ConfigurationCurrency(_session) { Ord = 20, Code = 20, Acronym = "KWZ", Designation = "Kwanza" }; configurationCurrency2.Save();
                var configurationCurrency3 = new CFG_ConfigurationCurrency(_session) { Ord = 30, Code = 30, Acronym = "USD", Designation = "US Dollar" }; configurationCurrency3.Save();
            };

            //ConfigurationCurrency
            XPCollection xpcConfigurationCountry = new XPCollection(_session, typeof(CFG_ConfigurationCountry));
            if (xpcConfigurationCountry.Count == 0)
            {
                var configurationCountry1 = new CFG_ConfigurationCountry(_session) { Ord = 10, Code = 10, Code2 = "AD", Code3 = "AND", Designation = "Andorra" }; configurationCountry1.Save();
                var configurationCountry2 = new CFG_ConfigurationCountry(_session) { Ord = 20, Code = 20, Code2 = "AE", Code3 = "ARE", Designation = "Emiratos Árabes Unidos" }; configurationCountry2.Save();
                var configurationCountry3 = new CFG_ConfigurationCountry(_session) { Ord = 30, Code = 30, Code2 = "AF", Code3 = "AFG", Designation = "Afeganistão" }; configurationCountry3.Save();
            };

            //CustomerType
            XPCollection xpcCustomerType = new XPCollection(_session, typeof(ERP_CustomerType));
            if (xpcCustomerType.Count == 0)
            {
                var customerType1 = new ERP_CustomerType(_session) { Ord = 10, Code = 10, Designation = "Normal" }; customerType1.Save();
                var customerType2 = new ERP_CustomerType(_session) { Ord = 20, Code = 20, Designation = "Fraco" }; customerType2.Save();
                var customerType3 = new ERP_CustomerType(_session) { Ord = 30, Code = 30, Designation = "Bom" }; customerType3.Save();
                var customerType4 = new ERP_CustomerType(_session) { Ord = 40, Code = 40, Designation = "Muito Bom" }; customerType4.Save();
                var customerType5 = new ERP_CustomerType(_session) { Ord = 50, Code = 50, Designation = "Empresa" }; customerType5.Save();
                var customerType6 = new ERP_CustomerType(_session) { Ord = 60, Code = 60, Designation = "Fim-de-Semana" }; customerType6.Save();
            };

            //ConfigurationPriceType
            XPCollection xpcConfigurationPriceType = new XPCollection(_session, typeof(FIN_ConfigurationPriceType));
            if (xpcConfigurationPriceType.Count == 0)
            {
                var configurationPriceType1 = new FIN_ConfigurationPriceType(_session) { Ord = 10, Code = 10, Designation = "Normal", EnumValue = 0 }; configurationPriceType1.Save();
                var configurationPriceType2 = new FIN_ConfigurationPriceType(_session) { Ord = 20, Code = 20, Designation = "Balcão", EnumValue = 1 }; configurationPriceType2.Save();
                var configurationPriceType3 = new FIN_ConfigurationPriceType(_session) { Ord = 30, Code = 30, Designation = "Esplanada", EnumValue = 2 }; configurationPriceType3.Save();
                var configurationPriceType4 = new FIN_ConfigurationPriceType(_session) { Ord = 40, Code = 40, Designation = "Especial", EnumValue = 3 }; configurationPriceType4.Save();
                var configurationPriceType5 = new FIN_ConfigurationPriceType(_session) { Ord = 50, Code = 50, Designation = "Consumo", EnumValue = 4 }; configurationPriceType5.Save();

                //ConfigurationPlace
                XPCollection xpcConfigurationPlace = new XPCollection(_session, typeof(POS_ConfigurationPlace));
                if (xpcConfigurationPlace.Count == 0)
                {
                    var configurationPlace1 = new POS_ConfigurationPlace(_session) { Ord = 10, Code = 10, Designation = "Bar", PriceType = configurationPriceType1 }; configurationPlace1.Save();
                    var configurationPlace2 = new POS_ConfigurationPlace(_session) { Ord = 20, Code = 20, Designation = "Balcão", PriceType = configurationPriceType2 }; configurationPlace2.Save();
                    var configurationPlace3 = new POS_ConfigurationPlace(_session) { Ord = 30, Code = 30, Designation = "Sala", PriceType = configurationPriceType3 }; configurationPlace3.Save();
                    var configurationPlace4 = new POS_ConfigurationPlace(_session) { Ord = 40, Code = 40, Designation = "Esplanada", PriceType = configurationPriceType4 }; configurationPlace4.Save();
                    var configurationPlace5 = new POS_ConfigurationPlace(_session) { Ord = 50, Code = 50, Designation = "Take-Away", PriceType = configurationPriceType5 }; configurationPlace5.Save();
                    var configurationPlace6 = new POS_ConfigurationPlace(_session) { Ord = 60, Code = 60, Designation = "Piscina", PriceType = configurationPriceType5 }; configurationPlace6.Save();

                    //ConfigurationPlaceTable
                    XPCollection xpcConfigurationPlaceTable = new XPCollection(_session, typeof(POS_ConfigurationPlaceTable));
                    if (xpcConfigurationPlaceTable.Count == 0)
                    {
                        //Zona1
                        var configurationPlaceTable1 = new POS_ConfigurationPlaceTable(_session) { Ord = 10, Code = 10, Designation = "Table 1.1", Place = configurationPlace1 }; configurationPlaceTable1.Save();
                        var configurationPlaceTable2 = new POS_ConfigurationPlaceTable(_session) { Ord = 20, Code = 20, Designation = "Table 1.2", Place = configurationPlace1, TableStatus = TableStatus.Open }; configurationPlaceTable2.Save();
                        var configurationPlaceTable3 = new POS_ConfigurationPlaceTable(_session) { Ord = 30, Code = 30, Designation = "Table 1.3", Place = configurationPlace1, TableStatus = TableStatus.Reserved }; configurationPlaceTable3.Save();
                        var configurationPlaceTable4 = new POS_ConfigurationPlaceTable(_session) { Ord = 40, Code = 40, Designation = "Table 1.4", Place = configurationPlace1 }; configurationPlaceTable4.Save();
                        var configurationPlaceTable5 = new POS_ConfigurationPlaceTable(_session) { Ord = 50, Code = 50, Designation = "Table 1.5", Place = configurationPlace1 }; configurationPlaceTable5.Save();
                        //Zona2
                        var configurationPlaceTable6 = new POS_ConfigurationPlaceTable(_session) { Ord = 60, Code = 60, Designation = "Table 2.1", Place = configurationPlace2, TableStatus = TableStatus.Open }; configurationPlaceTable6.Save();
                        var configurationPlaceTable7 = new POS_ConfigurationPlaceTable(_session) { Ord = 70, Code = 70, Designation = "Table 2.2", Place = configurationPlace2, TableStatus = TableStatus.Reserved }; configurationPlaceTable7.Save();
                        var configurationPlaceTable8 = new POS_ConfigurationPlaceTable(_session) { Ord = 80, Code = 80, Designation = "Table 2.3", Place = configurationPlace2, TableStatus = TableStatus.Open }; configurationPlaceTable8.Save();
                        var configurationPlaceTable9 = new POS_ConfigurationPlaceTable(_session) { Ord = 90, Code = 90, Designation = "Table 2.4", Place = configurationPlace2, TableStatus = TableStatus.Free }; configurationPlaceTable9.Save();
                        var configurationPlaceTable10 = new POS_ConfigurationPlaceTable(_session) { Ord = 100, Code = 100, Designation = "Table 2.5", Place = configurationPlace2 }; configurationPlaceTable10.Save();
                        //Zona3
                        var configurationPlaceTable11 = new POS_ConfigurationPlaceTable(_session) { Ord = 110, Code = 110, Designation = "Table 3.1", Place = configurationPlace3, TableStatus = TableStatus.Open }; configurationPlaceTable11.Save();
                        var configurationPlaceTable12 = new POS_ConfigurationPlaceTable(_session) { Ord = 120, Code = 120, Designation = "Table 3.2", Place = configurationPlace3, TableStatus = TableStatus.Reserved }; configurationPlaceTable12.Save();
                        var configurationPlaceTable13 = new POS_ConfigurationPlaceTable(_session) { Ord = 130, Code = 130, Designation = "Table 3.3", Place = configurationPlace3, TableStatus = TableStatus.Free }; configurationPlaceTable13.Save();
                        var configurationPlaceTable14 = new POS_ConfigurationPlaceTable(_session) { Ord = 140, Code = 140, Designation = "Table 3.4", Place = configurationPlace3, TableStatus = TableStatus.Reserved }; configurationPlaceTable14.Save();
                        var configurationPlaceTable15 = new POS_ConfigurationPlaceTable(_session) { Ord = 150, Code = 150, Designation = "Table 3.5", Place = configurationPlace3, TableStatus = TableStatus.Free }; configurationPlaceTable15.Save();
                        //Zona4
                        var configurationPlaceTable16 = new POS_ConfigurationPlaceTable(_session) { Ord = 160, Code = 160, Designation = "Table 4.1", Place = configurationPlace4, TableStatus = TableStatus.Reserved }; configurationPlaceTable16.Save();
                        var configurationPlaceTable17 = new POS_ConfigurationPlaceTable(_session) { Ord = 170, Code = 170, Designation = "Table 4.2", Place = configurationPlace4, TableStatus = TableStatus.Open }; configurationPlaceTable17.Save();
                        var configurationPlaceTable18 = new POS_ConfigurationPlaceTable(_session) { Ord = 180, Code = 180, Designation = "Table 4.3", Place = configurationPlace4, TableStatus = TableStatus.Reserved }; configurationPlaceTable18.Save();
                        var configurationPlaceTable19 = new POS_ConfigurationPlaceTable(_session) { Ord = 190, Code = 190, Designation = "Table 4.4", Place = configurationPlace4, TableStatus = TableStatus.Open }; configurationPlaceTable19.Save();
                        var configurationPlaceTable20 = new POS_ConfigurationPlaceTable(_session) { Ord = 200, Code = 200, Designation = "Table 4.5", Place = configurationPlace4, TableStatus = TableStatus.Reserved }; configurationPlaceTable20.Save();
                        //Zona5
                        var configurationPlaceTable21 = new POS_ConfigurationPlaceTable(_session) { Ord = 210, Code = 210, Designation = "Table 5.1", Place = configurationPlace5, TableStatus = TableStatus.Reserved }; configurationPlaceTable21.Save();
                        var configurationPlaceTable22 = new POS_ConfigurationPlaceTable(_session) { Ord = 220, Code = 220, Designation = "Table 5.2", Place = configurationPlace5, TableStatus = TableStatus.Reserved }; configurationPlaceTable22.Save();
                        var configurationPlaceTable23 = new POS_ConfigurationPlaceTable(_session) { Ord = 230, Code = 230, Designation = "Table 5.3", Place = configurationPlace5, TableStatus = TableStatus.Free }; configurationPlaceTable23.Save();
                        var configurationPlaceTable24 = new POS_ConfigurationPlaceTable(_session) { Ord = 240, Code = 240, Designation = "Table 5.4", Place = configurationPlace5, TableStatus = TableStatus.Free }; configurationPlaceTable24.Save();
                        var configurationPlaceTable25 = new POS_ConfigurationPlaceTable(_session) { Ord = 250, Code = 250, Designation = "Table 5.5", Place = configurationPlace5, TableStatus = TableStatus.Free }; configurationPlaceTable25.Save();
                        //Zona6
                        var configurationPlaceTable26 = new POS_ConfigurationPlaceTable(_session) { Ord = 260, Code = 260, Designation = "Table 6.1", Place = configurationPlace6, TableStatus = TableStatus.Open }; configurationPlaceTable26.Save();
                        var configurationPlaceTable27 = new POS_ConfigurationPlaceTable(_session) { Ord = 270, Code = 270, Designation = "Table 6.2", Place = configurationPlace6, TableStatus = TableStatus.Free }; configurationPlaceTable27.Save();
                        var configurationPlaceTable28 = new POS_ConfigurationPlaceTable(_session) { Ord = 280, Code = 280, Designation = "Table 6.3", Place = configurationPlace6, TableStatus = TableStatus.Free }; configurationPlaceTable28.Save();
                        var configurationPlaceTable29 = new POS_ConfigurationPlaceTable(_session) { Ord = 290, Code = 290, Designation = "Table 6.4", Place = configurationPlace6, TableStatus = TableStatus.Open }; configurationPlaceTable29.Save();
                        var configurationPlaceTable30 = new POS_ConfigurationPlaceTable(_session) { Ord = 300, Code = 300, Designation = "Table 6.5", Place = configurationPlace6, TableStatus = TableStatus.Free }; configurationPlaceTable30.Save();

                        //OrderMain
                        XPCollection xpcDocumentOrderMain = new XPCollection(_session, typeof(FIN_DocumentOrderMain));
                        if (xpcDocumentOrderMain.Count == 0)
                        {
                            //Orders
                            //var documentOrderMain1 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable1 }; documentOrderMain1.Save();
                            //var documentOrderMain2 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable2 }; documentOrderMain2.Save();
                            //var documentOrderMain3 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable3 }; documentOrderMain3.Save();

                            //var documentOrderMain4 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable4 }; documentOrderMain4.Save();
                            //var documentOrderMain5 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable5 }; documentOrderMain5.Save();
                            //var documentOrderMain6 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable6 }; documentOrderMain6.Save();

                            //var documentOrderMain7 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable11 }; documentOrderMain7.Save();
                            //var documentOrderMain8 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable12 }; documentOrderMain8.Save();
                            //var documentOrderMain9 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable13 }; documentOrderMain9.Save();

                            //var documentOrderMain10 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable16 }; documentOrderMain10.Save();
                            //var documentOrderMain11 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable17 }; documentOrderMain11.Save();
                            //var documentOrderMain12 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable18 }; documentOrderMain12.Save();

                            //var documentOrderMain13 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable21 }; documentOrderMain13.Save();
                            //var documentOrderMain14 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable22 }; documentOrderMain14.Save();
                            //var documentOrderMain15 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable23 }; documentOrderMain15.Save();

                            //var documentOrderMain16 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable26 }; documentOrderMain16.Save();
                            //var documentOrderMain17 = new DocumentOrderMain(_session) { Status = OrderStatus.Open, Table = configurationPlaceTable27 }; documentOrderMain17.Save();
                            //var documentOrderMain18 = new DocumentOrderMain(_session) { Status = OrderStatus.Close, Table = configurationPlaceTable28 }; documentOrderMain18.Save();
                        }
                    }
                }
            }

            //ReportType
            XPCollection xpcReportType = new XPCollection(_session, typeof(RPT_ReportType));
            if (xpcReportType.Count == 0)
            {
                var reportType1 = new RPT_ReportType(_session) { Ord = 10, Code = 10, Designation = "ReportType #1", ResourceString = "reporttype_label_type1" }; reportType1.Save();
                var reportType2 = new RPT_ReportType(_session) { Ord = 20, Code = 20, Designation = "ReportType #2", ResourceString = "reporttype_label_type2" }; reportType2.Save();
                var reportType3 = new RPT_ReportType(_session) { Ord = 30, Code = 30, Designation = "ReportType #3", ResourceString = "reporttype_label_type3" }; reportType3.Save();
                var reportType4 = new RPT_ReportType(_session) { Ord = 40, Code = 30, Designation = "ReportType #4", ResourceString = "reporttype_label_type4" }; reportType4.Save();
                var reportType5 = new RPT_ReportType(_session) { Ord = 50, Code = 30, Designation = "ReportType #5", ResourceString = "reporttype_label_type5" }; reportType5.Save();

                //Report
                XPCollection xpcReport = new XPCollection(_session, typeof(RPT_Report));
                if (xpcReport.Count == 0)
                {
                    var report1 = new RPT_Report(_session) { Ord = 10, Code = 10, Designation = "Relatório de Vendas por Familia", ReportType = reportType1, ResourceString = "report_label_sales_per_family" }; report1.Save();
                    var report2 = new RPT_Report(_session) { Ord = 20, Code = 20, Designation = "Relatório de Total por Terminal", ReportType = reportType1, ResourceString = "report_label_total_per_terminal" }; report2.Save();
                    var report3 = new RPT_Report(_session) { Ord = 30, Code = 30, Designation = "Relatório de Vendas por Dia", ReportType = reportType1, ResourceString = "report_label_sales_per_date" }; report3.Save();
                    var report4 = new RPT_Report(_session) { Ord = 40, Code = 40, Designation = "Relatório de Vendas por Funcionário", ReportType = reportType1, ResourceString = "report_label_sales_per_employee" }; report4.Save();
                    var report5 = new RPT_Report(_session) { Ord = 50, Code = 50, Designation = "Relatório de Vendas por Tipo de Documento Fiscal", ReportType = reportType1, ResourceString = "report_label_sales_per_finance_document" }; report5.Save();

                    var report6 = new RPT_Report(_session) { Ord = 60, Code = 60, Designation = "Relatório de Vendas por Tipos de Pagamento", ReportType = reportType2, ResourceString = "report_label_sales_per_payment_method" }; report6.Save();
                    var report7 = new RPT_Report(_session) { Ord = 70, Code = 70, Designation = "Relatório de Vendas por Zona/Mesa", ReportType = reportType2, ResourceString = "report_label_sales_per_zone_table" }; report7.Save();
                    var report8 = new RPT_Report(_session) { Ord = 80, Code = 80, Designation = "Relatório de Top Fecho Empregados", ReportType = reportType2, ResourceString = "report_label_top_close_employees" }; report8.Save();
                    var report9 = new RPT_Report(_session) { Ord = 90, Code = 90, Designation = "Relatório de Média de Ocupação", ReportType = reportType2, ResourceString = "report_label_occupation_average" }; report9.Save();
                    var report10 = new RPT_Report(_session) { Ord = 100, Code = 100, Designation = "Relatório de Total por Zona", ReportType = reportType2, ResourceString = "report_label_zone_total" }; report10.Save();

                    var report11 = new RPT_Report(_session) { Ord = 110, Code = 110, Designation = "Relatório de Top Horários Pico de Registo", ReportType = reportType3, ResourceString = "report_label_record_peak_hour" }; report11.Save();
                    var report12 = new RPT_Report(_session) { Ord = 120, Code = 120, Designation = "Relatório de Top Horários Pico de Fecho", ReportType = reportType3, ResourceString = "report_label_close_peak_hour" }; report12.Save();
                    var report13 = new RPT_Report(_session) { Ord = 130, Code = 130, Designation = "Relatório de Top Registos Empregados", ReportType = reportType3, ResourceString = "report_label_top_employee_records" }; report13.Save();
                    var report14 = new RPT_Report(_session) { Ord = 140, Code = 140, Designation = "Relatório de Total de Ofertas", ReportType = reportType3, ResourceString = "report_label_top_offers" }; report14.Save();
                    var report15 = new RPT_Report(_session) { Ord = 150, Code = 150, Designation = "Relatório de Movimentos por Empregado", ReportType = reportType3, ResourceString = "report_label_employee_movements" }; report15.Save();

                    var report16 = new RPT_Report(_session) { Ord = 160, Code = 160, Designation = "Relatório de Saldo de Cliente", ReportType = reportType4, ResourceString = "report_label_account_balance" }; report16.Save();
                    var report17 = new RPT_Report(_session) { Ord = 170, Code = 170, Designation = "Relatório de Horas em Serviço", ReportType = reportType4, ResourceString = "report_label_service_hours" }; report17.Save();
                    var report18 = new RPT_Report(_session) { Ord = 180, Code = 180, Designation = "Relatório de Retenção na Fonte", ReportType = reportType4, ResourceString = "report_label_withholding_tax" }; report18.Save();
                    var report19 = new RPT_Report(_session) { Ord = 190, Code = 190, Designation = "Relatório de Balancete", ReportType = reportType4, ResourceString = "report_label_balance_sheet" }; report19.Save();
                    var report20 = new RPT_Report(_session) { Ord = 200, Code = 200, Designation = "Relatório de Entregas por Estafeta", ReportType = reportType4, ResourceString = "report_label_courier_deliver" }; report20.Save();

                    var report21 = new RPT_Report(_session) { Ord = 210, Code = 210, Designation = "Relatório de Artigos Cancelados por Empregado", ReportType = reportType5, ResourceString = "report_label_canceled_articles_per_employee" }; report21.Save();
                    var report22 = new RPT_Report(_session) { Ord = 220, Code = 220, Designation = "Relatório de Descontos por Utilizador", ReportType = reportType5, ResourceString = "report_label_discounts_per_user" }; report22.Save();
                    var report23 = new RPT_Report(_session) { Ord = 230, Code = 230, Designation = "Relatório de Total de Caixa", ReportType = reportType5, ResourceString = "report_label_cash_total" }; report23.Save();
                    var report24 = new RPT_Report(_session) { Ord = 240, Code = 240, Designation = "Relatório de Inventário", ReportType = reportType5, ResourceString = "report_label_inventory" }; report24.Save();
                    var report25 = new RPT_Report(_session) { Ord = 250, Code = 250, Designation = "Relatório de Consumos por Empregado", ReportType = reportType5, ResourceString = "report_label_consumption_per_user" }; report25.Save();
                };
            };

            //ConfigurationPreferenceParameter
            XPCollection xpcConfigurationPreferenceParameter = new XPCollection(_session, typeof(CFG_ConfigurationPreferenceParameter));
            if (xpcConfigurationPreferenceParameter.Count == 0)
            {
                var configurationPreferenceParameter1 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 10, Code = 10, Token = "COMPANY_NAME", ResourceString = "prefparam_company_name", Value = "LogicPulse" }; configurationPreferenceParameter1.Save();
                var configurationPreferenceParameter2 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 20, Code = 20, Token = "COMPANY_ADDRESS", ResourceString = "prefparam_company_address", Value = "Rua da República, nº39 1º Esq" }; configurationPreferenceParameter2.Save();
                var configurationPreferenceParameter4 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 30, Code = 30, Token = "COMPANY_ZIPCODE", ResourceString = "prefparam_company_zipcode", Value = "3080-035" }; configurationPreferenceParameter4.Save();
                var configurationPreferenceParameter5 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 40, Code = 40, Token = "COMPANY_CITY", ResourceString = "prefparam_company_city", Value = "Figueira da Foz" }; configurationPreferenceParameter5.Save();
                var configurationPreferenceParameter6 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 50, Code = 50, Token = "COMPANY_TELEPHONE", ResourceString = "prefparam_company_telephone", Value = "+351 233 042 347" }; configurationPreferenceParameter6.Save();
                var configurationPreferenceParameter7 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 60, Code = 60, Token = "COMPANY_FAX", ResourceString = "prefparam_company_fax", Value = "" }; configurationPreferenceParameter7.Save();
                var configurationPreferenceParameter8 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 70, Code = 70, Token = "COMPANY_EMAIL", ResourceString = "prefparam_company_email", Value = "mail@logicpulse.pt" }; configurationPreferenceParameter8.Save();
                var configurationPreferenceParameter9 = new CFG_ConfigurationPreferenceParameter(_session) { Ord = 80, Code = 80, Token = "COMPANY_FISCALNUMBER", ResourceString = "prefparam_company_fiscalnumber", Value = "508 278 155" }; configurationPreferenceParameter9.Save();
            }

            //ConfigurationPreferenceParameter
            XPCollection xpcConfigurationPrintersTemplates = new XPCollection(_session, typeof(SYS_ConfigurationPrintersTemplates));
            if (xpcConfigurationPrintersTemplates.Count == 0)
            {
                var configurationPrintersTemplates1 = new SYS_ConfigurationPrintersTemplates(_session) { Ord = 10, Code = 1, Designation = "Modelo A4" }; configurationPrintersTemplates1.Save();
            }

            //PrinterType
            XPCollection xpcConfigurationPrintersType = new XPCollection(_session, typeof(SYS_ConfigurationPrintersType));
            if (xpcConfigurationPrintersType.Count == 0)
            {
                var configurationPrintersType1 = new SYS_ConfigurationPrintersType(_session) { Ord = 10, Code = 10, Designation = "Impressora SINOCAN em ambiente Windows", Token = "THERMAL_PRINTER_WINDOWS" }; configurationPrintersType1.Save();
                var configurationPrintersType2 = new SYS_ConfigurationPrintersType(_session) { Ord = 20, Code = 20, Designation = "Impressora SINOCAN em ambiente Linux", Token = "THERMAL_PRINTER_LINUX" }; configurationPrintersType2.Save();

                var configurationPrinters1 = new SYS_ConfigurationPrinters(_session) { Ord = 10, Code = 10, Designation = "Impressora teste Windows", PrinterType = configurationPrintersType1 }; configurationPrinters1.Save();
                var configurationPrinters2 = new SYS_ConfigurationPrinters(_session) { Ord = 20, Code = 20, Designation = "Impressora teste Linux", PrinterType = configurationPrintersType2 }; configurationPrinters2.Save();
            }

            //ConfigurationDevice
            POS_ConfigurationDevice configurationDevice1 = new POS_ConfigurationDevice(_session) { Ord = 10, Code = 10, Designation = "Table 3.1" }; configurationDevice1.Save();

            //ConfigurationHolidays
            CFG_ConfigurationHolidays configurationHolidays1 = new CFG_ConfigurationHolidays(_session) { Ord = 10, Code = 10, Designation = "Feriado", Day = 1, Month = 1, Year = 1, Fixed = true }; configurationHolidays1.Save();

            //SystemNotification      
            SYS_SystemNotification systemNotification1 = new SYS_SystemNotification(GlobalFramework.SessionXpo) { Message = "Welcome Message" }; systemNotification1.Save();

            //SystemBackup
            XPCollection xpcSystemBackup = new XPCollection(_session, typeof(SYS_SystemBackup));
            if (xpcSystemBackup.Count == 0)
            {
                var systemBackup1 = new SYS_SystemBackup(_session) { FileName = "filename001.sql", FileNamePacked = "filename001.bak" }; systemBackup1.Save();
                var systemBackup2 = new SYS_SystemBackup(_session) { FileName = "filename002.bak", FileNamePacked = "filename002.bak" }; systemBackup2.Save();
            }
        }

        public static void InitDocumentFinance(Session pSession)
        {
            //Init Parameters
            _session = pSession;

            var documentFinanceYear1 = new FIN_DocumentFinanceYears(_session) { Designation = "2013", FiscalYear = 2013 }; documentFinanceYear1.Save();

            //DocumentFinanceType
            XPCollection xpcDocumentFinanceType = new XPCollection(_session, typeof(FIN_DocumentFinanceType));
            if (xpcDocumentFinanceType.Count == 0)
            {
                var documentFinanceType1 = new FIN_DocumentFinanceType(_session) { Ord = 1, Designation = "Fatura", Acronym = "FT", ResourceString = "global_documentfinance_type_title_ft" }; documentFinanceType1.Save();

                XPCollection xpcDocumentFinanceDetail = new XPCollection(_session, typeof(FIN_DocumentFinanceDetail));
                if (xpcDocumentFinanceDetail.Count == 0)
                {
                    var documentFinanceDetail1 = new FIN_DocumentFinanceDetail(_session) { }; documentFinanceDetail1.Save();
                }

                //DocumentFinanceDetailOrderReference
                var documentFinanceDetailOrderReference1 = new FIN_DocumentFinanceDetailOrderReference(_session) { OriginatingON = "ORDER#001", OrderDate = System.DateTime.Now }; documentFinanceDetailOrderReference1.Save();

                //DocumentFinanceDetailReference
                var documentFinanceDetailReference1 = new FIN_DocumentFinanceDetailReference(_session) { Reference = "ORDER#001", Reason = "ORDER#001" }; documentFinanceDetailReference1.Save();

                //DocumentFinanceSeries
                var documentFinanceSeries1 = new FIN_DocumentFinanceSeries(_session) { Designation = "Fatura 2013", FiscalYear = documentFinanceYear1, DocumentType = documentFinanceType1, NextDocumentNumber = 1 }; documentFinanceType1.Save();

                //DocumentFinanceYearSerieTerminal
                var documentFinanceYearSerieTerminal1 = new FIN_DocumentFinanceYearSerieTerminal(_session) { Designation = "Fatura 2013" }; documentFinanceType1.Save();
            }

            XPCollection xpcDocumentFinanceCommission = new XPCollection(_session, typeof(FIN_DocumentFinanceCommission));
            if (xpcDocumentFinanceCommission.Count == 0)
            {
                var documentFinanceCommission1 = new FIN_DocumentFinanceCommission(_session) { Ord = 1, Commission = 10, Total = 10 }; documentFinanceCommission1.Save();
            }
        }

        public static void InitWorkSession(Session pSession)
        {
            //Init Parameters
            _session = pSession;

            //WorkSession
            XPCollection xpcWorkSessionMovementType = new XPCollection(_session, typeof(POS_WorkSessionMovementType));
            if (xpcWorkSessionMovementType.Count == 0)
            {
                var workSessionMovementType1 = new POS_WorkSessionMovementType(_session) { Ord = 1, Code = 10, Token = "FINANCE_DOCUMENT", Designation = "Documento Fiscal" }; workSessionMovementType1.Save();
                var workSessionMovementType2 = new POS_WorkSessionMovementType(_session) { Ord = 2, Code = 20, Token = "CASHDRAWER_OPEN", Designation = "Abertura de Caixa", ResourceString = "pos_button_label_cashdrawer_open", ButtonIcon = @"Icons\icon_pos_cashdrawer_open.png", CashDrawerMovement = true }; workSessionMovementType2.Save();
                var workSessionMovementType6 = new POS_WorkSessionMovementType(_session) { Ord = 3, Code = 30, Token = "CASHDRAWER_CLOSE", Designation = "Fecho de Caixa", ResourceString = "pos_button_label_cashdrawer_close", ButtonIcon = @"Icons\icon_pos_cashdrawer_close.png", CashDrawerMovement = true }; workSessionMovementType6.Save();
                var workSessionMovementType3 = new POS_WorkSessionMovementType(_session) { Ord = 4, Code = 40, Token = "CASHDRAWER_IN", Designation = "Entrada de Caixa", ResourceString = "pos_button_label_cashdrawer_in", ButtonIcon = @"Icons\icon_pos_cashdrawer_in.png", CashDrawerMovement = true }; workSessionMovementType3.Save();
                var workSessionMovementType4 = new POS_WorkSessionMovementType(_session) { Ord = 5, Code = 50, Token = "CASHDRAWER_OUT", Designation = "Saida de Caixa", ResourceString = "pos_button_label_cashdrawer_out", ButtonIcon = @"Icons\icon_pos_cashdrawer_out.png", CashDrawerMovement = true }; workSessionMovementType4.Save();
                var workSessionMovementType5 = new POS_WorkSessionMovementType(_session) { Ord = 6, Code = 60, Token = "CASHDRAWER_MONEY_OUT", Designation = "Sangria", ResourceString = "pos_button_label_cashdrawer_money_out", ButtonIcon = @"Icons\icon_pos_cashdrawer_money_out.png", CashDrawerMovement = true, Disabled = true }; workSessionMovementType5.Save();

                var workSessionPeriod1 = new POS_WorkSessionPeriod(_session) { Designation = "WorkSessionPeriod No.1", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod1.Save();
                var workSessionPeriod2 = new POS_WorkSessionPeriod(_session) { Designation = "WorkSessionPeriod No.2", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod2.Save();
                var workSessionPeriod3 = new POS_WorkSessionPeriod(_session) { Designation = "WorkSessionPeriod No.3", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod3.Save();
                var workSessionPeriod4 = new POS_WorkSessionPeriod(_session) { Designation = "WorkSessionPeriod No.4", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod4.Save();
                var workSessionPeriod5 = new POS_WorkSessionPeriod(_session) { Designation = "WorkSessionPeriod No.5", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod5.Save();

                var workSessionMovement1 = new POS_WorkSessionMovement(_session) { Date = DateTime.Now, MovementAmount = 100.10m, WorkSessionPeriod = workSessionPeriod1, WorkSessionMovementType = workSessionMovementType1 }; workSessionMovement1.Save();
                var workSessionMovement2 = new POS_WorkSessionMovement(_session) { Date = DateTime.Now, MovementAmount = 200.20m, WorkSessionPeriod = workSessionPeriod2, WorkSessionMovementType = workSessionMovementType2 }; workSessionMovement2.Save();
                var workSessionMovement3 = new POS_WorkSessionMovement(_session) { Date = DateTime.Now, MovementAmount = 300.30m, WorkSessionPeriod = workSessionPeriod3, WorkSessionMovementType = workSessionMovementType3 }; workSessionMovement3.Save();
                var workSessionMovement4 = new POS_WorkSessionMovement(_session) { Date = DateTime.Now, MovementAmount = 400.40m, WorkSessionPeriod = workSessionPeriod4, WorkSessionMovementType = workSessionMovementType4 }; workSessionMovement4.Save();

                XPCollection xpcConfigurationPaymentMethod = new XPCollection(_session, typeof(FIN_ConfigurationPaymentMethod));
                decimal amount = 10.10m;
                foreach (FIN_ConfigurationPaymentMethod paymentMethod in xpcConfigurationPaymentMethod)
                {
                    var workSessionPeriodTotal = new POS_WorkSessionPeriodTotal(_session) { PaymentMethod = paymentMethod, Total = amount, Period = workSessionPeriod1 }; workSessionPeriodTotal.Save();
                    amount = amount * 2;
                }
            }
        }
    }
}
