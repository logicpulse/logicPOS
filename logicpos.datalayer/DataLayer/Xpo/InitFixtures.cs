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
            XPCollection xpcUser = new XPCollection(_session, typeof(sys_userdetail));
            if (xpcUser.Count == 0)
            {
                var user1 = new sys_userdetail(_session) { Ord = 10, Code = 10, Name = "Administrador", AccessPin = "0000" }; user1.UpdatedBy = user1; user1.Save();
                var user2 = new sys_userdetail(_session) { Ord = 20, Code = 20, Name = "Barman", AccessPin = "1111" }; user2.UpdatedBy = user1; user2.Save();

                //ConfigurationPlaceTerminal
                XPCollection _xpcConfigurationPlaceTerminal = new XPCollection(_session, typeof(pos_configurationplaceterminal));
                if (xpcUser.Count == 0)
                {
                    var configurationPlaceTerminal1 = new pos_configurationplaceterminal(_session) { Ord = 10, Code = 10, Designation = "Terminal #1", HardwareId = "####-####-####-####-####-###1" }; configurationPlaceTerminal1.UpdatedBy = user1; configurationPlaceTerminal1.Save();
                    var configurationPlaceTerminal2 = new pos_configurationplaceterminal(_session) { Ord = 20, Code = 20, Designation = "Terminal #2", HardwareId = "####-####-####-####-####-###2", UpdatedBy = user1 }; configurationPlaceTerminal2.Save();
                    var configurationPlaceTerminal3 = new pos_configurationplaceterminal(_session) { Ord = 30, Code = 30, Designation = "Terminal #3", HardwareId = "####-####-####-####-####-###3", UpdatedBy = user1 }; configurationPlaceTerminal3.Save();
                };
            }

            //UserPermissionGroup
            XPCollection xpcUserProfile = new XPCollection(_session, typeof(sys_userprofile));
            sys_userprofile userProfile1 = null;
            sys_userprofile userProfile2 = null;
            sys_userprofile userProfile3 = null;

            //UserProfile
            if (xpcUserProfile.Count == 0)
            {
                userProfile1 = new sys_userprofile(_session) { Code = 10, Designation = "Administração" }; userProfile1.Save();
                userProfile2 = new sys_userprofile(_session) { Code = 20, Designation = "Empregado" }; userProfile2.Save();
                userProfile3 = new sys_userprofile(_session) { Code = 30, Designation = "Supervisor" }; userProfile3.Save();
            };

            //UserPermissionGroup
            XPCollection xpcUserPermissionGroup = new XPCollection(_session, typeof(sys_userpermissiongroup));
            sys_userpermissiongroup userPermissionGroup1 = null;
            sys_userpermissiongroup userPermissionGroup2 = null;
            sys_userpermissiongroup userPermissionGroup3 = null;

            if (xpcUserPermissionGroup.Count == 0)
            {
                userPermissionGroup1 = new sys_userpermissiongroup(_session) { Code = 10, Designation = "Administração" }; userPermissionGroup1.Save();
                userPermissionGroup2 = new sys_userpermissiongroup(_session) { Code = 20, Designation = "Mesas" }; userPermissionGroup2.Save();
                userPermissionGroup3 = new sys_userpermissiongroup(_session) { Code = 30, Designation = "Relatórios" }; userPermissionGroup3.Save();
            };

            //UserPermissionItem
            sys_userpermissionitem userPermissionItem1 = null;
            sys_userpermissionitem userPermissionItem2 = null;
            sys_userpermissionitem userPermissionItem3 = null;
            XPCollection xpcUserPermissionItem = new XPCollection(_session, typeof(sys_userpermissionitem));
            if (xpcUserPermissionItem.Count == 0)
            {
                userPermissionItem1 = new sys_userpermissionitem(_session) { Token = "TABLE_ALL", Designation = "Mesas – Todas as operações #1" }; userPermissionItem1.Save();
                userPermissionItem2 = new sys_userpermissionitem(_session) { Token = "TOTALS_ALL", Designation = "Totais – Todas as operações" }; userPermissionItem2.Save();
                userPermissionItem3 = new sys_userpermissionitem(_session) { Token = "ORDER_ALL", Designation = "Pedidos – Todas as operações" }; userPermissionItem3.Save();
            }

            //UserPermissionProfile
            XPCollection xpcUserPermissionProfile = new XPCollection(_session, typeof(sys_userpermissionprofile));
            if (xpcUserPermissionProfile.Count == 0)
            {
                var userPermissionProfile1 = new sys_userpermissionprofile(_session) { PermissionItem = userPermissionItem1, UserProfile = userProfile1 };
                userPermissionProfile1.Save();
            }

            //SystemAuditType|SystemAudit
            XPCollection xpcSystemAuditType = new XPCollection(_session, typeof(sys_systemaudittype));
            if (xpcSystemAuditType.Count == 0)
            {
                var systemAuditType1 = new sys_systemaudittype(_session) { Ord = 10, Code = 10, Token = "APP_START", Designation = "Aplicação Iniciada", ResourceString = "systemaudittype_app_start" }; systemAuditType1.Save();
                var systemAuditType2 = new sys_systemaudittype(_session) { Ord = 20, Code = 20, Token = "APP_CLOSE", Designation = "Aplicação Encerrada", ResourceString = "systemaudittype_app_close" }; systemAuditType2.Save();

                XPCollection xpcSystemAudit = new XPCollection(_session, typeof(sys_systemaudit));
                if (xpcSystemAudit.Count == 0)
                {
                    var systemAudit1 = new sys_systemaudit(_session) { Date = DateTime.Now, AuditType = systemAuditType1 }; systemAudit1.Save();
                    var systemAudit2 = new sys_systemaudit(_session) { Date = DateTime.Now, AuditType = systemAuditType2 }; systemAudit2.Save();
                };
            };

            //sys_systemauditatWS
            XPCollection xpcSystemAuditATWS = new XPCollection(_session, typeof(sys_systemauditat));
            if (xpcSystemAuditATWS.Count == 0)
            {
                var systemAuditATWS1 = new sys_systemauditat(_session) { Date = DateTime.Now, Type = SystemAuditATWSType.Document }; systemAuditATWS1.Save();
            }
        }

        public static void InitOther(Session pSession)
        {
            //Init Parameters
            _session = pSession;

            //ArticleType
            XPCollection xpcArticleType = new XPCollection(_session, typeof(fin_articletype));
            if (xpcArticleType.Count == 0)
            {
                var articleType1 = new fin_articletype(_session) { Ord = 10, Code = 10, Designation = "Normal" }; articleType1.Save();
                var articleType2 = new fin_articletype(_session) { Ord = 20, Code = 20, Designation = "Complemento" }; articleType2.Save();
                var articleType3 = new fin_articletype(_session) { Ord = 30, Code = 30, Designation = "Consumo" }; articleType3.Save();
                var articleType4 = new fin_articletype(_session) { Ord = 40, Code = 40, Designation = "Gorjeta" }; articleType4.Save();
                var articleType5 = new fin_articletype(_session) { Ord = 50, Code = 50, Designation = "Informativo" }; articleType5.Save();
            };

            //ArticleClass
            XPCollection xpcArticleClass = new XPCollection(_session, typeof(fin_articleclass));
            if (xpcArticleClass.Count == 0)
            {
                var articleClass1 = new fin_articleclass(_session) { Ord = 10, Code = 10, Designation = "Produtos", Acronym = "P" }; articleClass1.Save();
                var articleClass2 = new fin_articleclass(_session) { Ord = 20, Code = 20, Designation = "Serviços", Acronym = "S" }; articleClass2.Save();
                var articleClass3 = new fin_articleclass(_session) { Ord = 30, Code = 30, Designation = "Outros", Acronym = "O" }; articleClass3.Save();
            };

            //ArticleStock
            XPCollection xpcArticleStock = new XPCollection(_session, typeof(fin_articlestock));
            if (xpcArticleStock.Count == 0)
            {
                var articleStock1 = new fin_articlestock(_session) { Date = DateTime.Now, DocumentNumber = "Doc1" }; articleStock1.Save();
                var articleStock2 = new fin_articlestock(_session) { Date = DateTime.Now, DocumentNumber = "Doc2" }; articleStock2.Save();
                var articleStock3 = new fin_articlestock(_session) { Date = DateTime.Now, DocumentNumber = "Doc3" }; articleStock3.Save();
            };

            //ConfigurationCashRegister
            XPCollection _xpcConfigurationCashRegister = new XPCollection(_session, typeof(pos_configurationcashregister));
            if (xpcArticleType.Count == 0)
            {
                var configurationCashRegister1 = new pos_configurationcashregister(_session) { Ord = 10, Code = 10, Designation = "CashRegister #1" }; configurationCashRegister1.Save();
                var configurationCashRegister2 = new pos_configurationcashregister(_session) { Ord = 20, Code = 20, Designation = "CashRegister #2" }; configurationCashRegister2.Save();
                var configurationCashRegister3 = new pos_configurationcashregister(_session) { Ord = 30, Code = 30, Designation = "CashRegister #3" }; configurationCashRegister3.Save();
                var configurationCashRegister4 = new pos_configurationcashregister(_session) { Ord = 40, Code = 40, Designation = "CashRegister #4" }; configurationCashRegister4.Save();
                var configurationCashRegister5 = new pos_configurationcashregister(_session) { Ord = 50, Code = 50, Designation = "CashRegister #5" }; configurationCashRegister5.Save();
            };

            //ConfigurationPlaceMovementType
            XPCollection xpcConfigurationPlaceMovementType = new XPCollection(_session, typeof(pos_configurationplacemovementtype));
            if (xpcConfigurationPlaceMovementType.Count == 0)
            {
                var configurationPlaceMovementType1 = new pos_configurationplacemovementtype(_session) { Ord = 10, Code = 10, Designation = "Normal" }; configurationPlaceMovementType1.Save();
                var configurationPlaceMovementType2 = new pos_configurationplacemovementtype(_session) { Ord = 20, Code = 20, Designation = "Consumo Próprio" }; configurationPlaceMovementType2.Save();
                var configurationPlaceMovementType3 = new pos_configurationplacemovementtype(_session) { Ord = 30, Code = 30, Designation = "Take-Away" }; configurationPlaceMovementType3.Save();
                var configurationPlaceMovementType4 = new pos_configurationplacemovementtype(_session) { Ord = 40, Code = 40, Designation = "Entrega ao Domicilio" }; configurationPlaceMovementType4.Save();
            };

            //ConfigurationUnitMeasure
            XPCollection xpcConfigurationUnitMeasure = new XPCollection(_session, typeof(cfg_configurationunitmeasure));
            if (xpcConfigurationUnitMeasure.Count == 0)
            {
                var configurationUnitMeasure1 = new cfg_configurationunitmeasure(_session) { Ord = 10, Code = 10, Designation = "Unidade", Acronym = "Un" }; configurationUnitMeasure1.Save();
                var configurationUnitMeasure2 = new cfg_configurationunitmeasure(_session) { Ord = 20, Code = 20, Designation = "Quilograma", Acronym = "Kg" }; configurationUnitMeasure2.Save();
                var configurationUnitMeasure3 = new cfg_configurationunitmeasure(_session) { Ord = 30, Code = 30, Designation = "Litro", Acronym = "L" }; configurationUnitMeasure3.Save();
                var configurationUnitMeasure4 = new cfg_configurationunitmeasure(_session) { Ord = 40, Code = 40, Designation = "Metro", Acronym = "m" }; configurationUnitMeasure4.Save();
            };

            //UserCommissionGroup
            XPCollection xpcUserCommissionGroup = new XPCollection(_session, typeof(pos_usercommissiongroup));
            if (xpcUserCommissionGroup.Count == 0)
            {
                var userCommissionGroup1 = new pos_usercommissiongroup(_session) { Ord = 10, Code = 10, Designation = "Não Tem" }; userCommissionGroup1.Save();
                var userCommissionGroup2 = new pos_usercommissiongroup(_session) { Ord = 20, Code = 20, Designation = "Balcão" }; userCommissionGroup2.Save();
                var userCommissionGroup3 = new pos_usercommissiongroup(_session) { Ord = 30, Code = 30, Designation = "Emp.Mesa" }; userCommissionGroup3.Save();
                var userCommissionGroup4 = new pos_usercommissiongroup(_session) { Ord = 40, Code = 40, Designation = "Chefe de Mesa" }; userCommissionGroup4.Save();
                var userCommissionGroup5 = new pos_usercommissiongroup(_session) { Ord = 50, Code = 50, Designation = "Gerente" }; userCommissionGroup5.Save();
                var userCommissionGroup6 = new pos_usercommissiongroup(_session) { Ord = 60, Code = 60, Designation = "Cozinheiro" }; userCommissionGroup6.Save();
                var userCommissionGroup7 = new pos_usercommissiongroup(_session) { Ord = 70, Code = 70, Designation = "Ajudante de Cozinha" }; userCommissionGroup7.Save();
            };

            //CustomerDiscountGroup
            XPCollection xpcCustomerDiscountGroup = new XPCollection(_session, typeof(erp_customerdiscountgroup));
            if (xpcCustomerDiscountGroup.Count == 0)
            {
                var customerDiscountGroup1 = new erp_customerdiscountgroup(_session) { Ord = 10, Code = 10, Designation = "Não Tem" }; customerDiscountGroup1.Save();
                var customerDiscountGroup2 = new erp_customerdiscountgroup(_session) { Ord = 20, Code = 20, Designation = "Mínimo" }; customerDiscountGroup2.Save();
                var customerDiscountGroup3 = new erp_customerdiscountgroup(_session) { Ord = 30, Code = 30, Designation = "Normal" }; customerDiscountGroup3.Save();
                var customerDiscountGroup4 = new erp_customerdiscountgroup(_session) { Ord = 40, Code = 40, Designation = "Especial" }; customerDiscountGroup4.Save();
                var customerDiscountGroup5 = new erp_customerdiscountgroup(_session) { Ord = 50, Code = 50, Designation = "VIP" }; customerDiscountGroup5.Save();
            };

            //ConfigurationVatRate
            XPCollection xpcConfigurationVatRate = new XPCollection(_session, typeof(fin_configurationvatrate));
            if (xpcConfigurationVatRate.Count == 0)
            {
                var configurationVatRate1 = new fin_configurationvatrate(_session) { Ord = 10, Code = 10, Designation = "Normal", Value = 23.00m }; configurationVatRate1.Save();
                var configurationVatRate2 = new fin_configurationvatrate(_session) { Ord = 20, Code = 20, Designation = "Intermédia", Value = 13.00m }; configurationVatRate2.Save();
                var configurationVatRate3 = new fin_configurationvatrate(_session) { Ord = 30, Code = 30, Designation = "Reduzido", Value = 6.00m }; configurationVatRate3.Save();
                var configurationVatRate4 = new fin_configurationvatrate(_session) { Ord = 40, Code = 40, Designation = "Isento", Value = 0.00m }; configurationVatRate4.Save();
            };

            //ConfigurationVatExemptionReason
            XPCollection xpcConfigurationVatExemptionReason = new XPCollection(_session, typeof(fin_configurationvatexemptionreason));
            if (xpcConfigurationVatExemptionReason.Count == 0)
            {
                var configurationVatExemptionReason1 = new fin_configurationvatexemptionreason(_session) { Ord = 10, Code = 10, Designation = "Artigo 16.º n.º 6 alínea c) do CIVA", StandardApplicable = "Artigo 16.º n.º 6 alínea c) do CIVA" }; configurationVatExemptionReason1.Save();
            };

            //ConfigurationUnitSize
            XPCollection xpcConfigurationUnitSize = new XPCollection(_session, typeof(cfg_configurationunitsize));
            if (xpcConfigurationUnitSize.Count == 0)
            {
                var unitSize1 = new cfg_configurationunitsize(_session) { Ord = 10, Code = 10, Designation = "Pequeno" }; unitSize1.Save();
                var unitSize2 = new cfg_configurationunitsize(_session) { Ord = 20, Code = 20, Designation = "Normal" }; unitSize2.Save();
                var unitSize3 = new cfg_configurationunitsize(_session) { Ord = 30, Code = 30, Designation = "Grande" }; unitSize3.Save();
            };

            //ConfigurationPaymentCondition
            XPCollection xpcConfigurationPaymentCondition = new XPCollection(_session, typeof(fin_configurationpaymentcondition));
            if (xpcConfigurationPaymentCondition.Count == 0)
            {
                var ConfigurationPaymentCondition1 = new fin_configurationpaymentcondition(_session) { Ord = 10, Code = 10, Acronym = "PP", Designation = "Pronto Pagamento" }; ConfigurationPaymentCondition1.Save();
                var ConfigurationPaymentCondition2 = new fin_configurationpaymentcondition(_session) { Ord = 20, Code = 20, Acronym = "30", Designation = "30 Dias" }; ConfigurationPaymentCondition2.Save();
                var ConfigurationPaymentCondition3 = new fin_configurationpaymentcondition(_session) { Ord = 30, Code = 30, Acronym = "60", Designation = "60 Dias" }; ConfigurationPaymentCondition3.Save();
            };

            //ConfigurationPaymentMethod
            XPCollection xpcConfigurationPaymentMethod = new XPCollection(_session, typeof(fin_configurationpaymentmethod));
            if (xpcConfigurationPaymentMethod.Count == 0)
            {
                var configurationPaymentMethod1 = new fin_configurationpaymentmethod(_session) { Ord = 10, Code = 10, Token = "MONEY", Designation = "Dinheiro", Acronym = "BN", ResourceString = "pos_button_label_payment_type_money", ButtonIcon = @"Icons\icon_pos_payment_type_money.png" }; configurationPaymentMethod1.Save();
                var configurationPaymentMethod2 = new fin_configurationpaymentmethod(_session) { Ord = 20, Code = 20, Token = "BANK_CHECK", Designation = "Cheques", Acronym = "BC", ResourceString = "pos_button_label_payment_type_bank_check", ButtonIcon = @"Icons\icon_pos_payment_type_bank_check.png" }; configurationPaymentMethod2.Save();
                var configurationPaymentMethod3 = new fin_configurationpaymentmethod(_session) { Ord = 30, Code = 30, Token = "CASH_MACHINE", Designation = "ATM", Acronym = "MB", ResourceString = "pos_button_label_payment_type_cash_machine", ButtonIcon = @"Icons\icon_pos_payment_type_cash_machine.png" }; configurationPaymentMethod3.Save();
                var configurationPaymentMethod4 = new fin_configurationpaymentmethod(_session) { Ord = 40, Code = 40, Token = "CREDIT_CARD", Designation = "Crédito", Acronym = "CR", ResourceString = "pos_button_label_payment_type_bank_credit_card", ButtonIcon = @"Icons\icon_pos_payment_type_credit_card.png" }; configurationPaymentMethod4.Save();
                var configurationPaymentMethod5 = new fin_configurationpaymentmethod(_session) { Ord = 50, Code = 50, Token = "VISA", Designation = "Pagamento via VISA", Acronym = "VS", ResourceString = "pos_button_label_payment_type_visa", ButtonIcon = @"Icons\icon_pos_payment_type_visa.png" }; configurationPaymentMethod5.Save();
                var configurationPaymentMethod6 = new fin_configurationpaymentmethod(_session) { Ord = 60, Code = 60, Token = "CURRENT_ACCOUNT", Designation = "Conta Corrent", Acronym = "CC", ResourceString = "pos_button_label_payment_type_current_account", ButtonIcon = @"Icons\icon_pos_payment_type_current_account.png" }; configurationPaymentMethod6.Save();
            };

            //ConfigurationPaymentMethod
            XPCollection xpcConfigurationKeyboard = new XPCollection(_session, typeof(pos_configurationkeyboard));
            if (xpcConfigurationKeyboard.Count == 0)
            {
                var configurationKeyboard1 = new pos_configurationkeyboard(_session) { Ord = 10, Code = 10, Designation = "Keyboard #1" }; configurationKeyboard1.Save();
                var configurationKeyboard2 = new pos_configurationkeyboard(_session) { Ord = 20, Code = 20, Designation = "Keyboard #2" }; configurationKeyboard2.Save();
                var configurationKeyboard3 = new pos_configurationkeyboard(_session) { Ord = 30, Code = 30, Designation = "Keyboard #3" }; configurationKeyboard3.Save();
                var configurationKeyboard4 = new pos_configurationkeyboard(_session) { Ord = 40, Code = 40, Designation = "Keyboard #4" }; configurationKeyboard4.Save();
                var configurationKeyboard5 = new pos_configurationkeyboard(_session) { Ord = 50, Code = 50, Designation = "Keyboard #5" }; configurationKeyboard5.Save();
            };

            //ConfigurationMaintenance
            XPCollection xpcConfigurationMaintenance = new XPCollection(_session, typeof(pos_configurationmaintenance));
            if (xpcConfigurationMaintenance.Count == 0)
            {
                var configurationMaintenance1 = new pos_configurationmaintenance(_session) { Ord = 10, Code = 10, Designation = "Keyboard #1" }; configurationMaintenance1.Save();
                var configurationMaintenance2 = new pos_configurationmaintenance(_session) { Ord = 20, Code = 20, Designation = "Keyboard #2" }; configurationMaintenance2.Save();
                var configurationMaintenance3 = new pos_configurationmaintenance(_session) { Ord = 30, Code = 30, Designation = "Keyboard #3" }; configurationMaintenance3.Save();
                var configurationMaintenance4 = new pos_configurationmaintenance(_session) { Ord = 40, Code = 40, Designation = "Keyboard #4" }; configurationMaintenance4.Save();
                var configurationMaintenance5 = new pos_configurationmaintenance(_session) { Ord = 50, Code = 50, Designation = "Keyboard #5" }; configurationMaintenance5.Save();
            };

            //ConfigurationCurrency
            XPCollection xpcConfigurationCurrency = new XPCollection(_session, typeof(cfg_configurationcurrency));
            if (xpcConfigurationCurrency.Count == 0)
            {
                var configurationCurrency1 = new cfg_configurationcurrency(_session) { Ord = 10, Code = 10, Acronym = "EUR", Designation = "Euro" }; configurationCurrency1.Save();
                var configurationCurrency2 = new cfg_configurationcurrency(_session) { Ord = 20, Code = 20, Acronym = "KWZ", Designation = "Kwanza" }; configurationCurrency2.Save();
                var configurationCurrency3 = new cfg_configurationcurrency(_session) { Ord = 30, Code = 30, Acronym = "USD", Designation = "US Dollar" }; configurationCurrency3.Save();
            };

            //ConfigurationCurrency
            XPCollection xpcConfigurationCountry = new XPCollection(_session, typeof(cfg_configurationcountry));
            if (xpcConfigurationCountry.Count == 0)
            {
                var configurationCountry1 = new cfg_configurationcountry(_session) { Ord = 10, Code = 10, Code2 = "AD", Code3 = "AND", Designation = "Andorra" }; configurationCountry1.Save();
                var configurationCountry2 = new cfg_configurationcountry(_session) { Ord = 20, Code = 20, Code2 = "AE", Code3 = "ARE", Designation = "Emiratos Árabes Unidos" }; configurationCountry2.Save();
                var configurationCountry3 = new cfg_configurationcountry(_session) { Ord = 30, Code = 30, Code2 = "AF", Code3 = "AFG", Designation = "Afeganistão" }; configurationCountry3.Save();
            };

            //CustomerType
            XPCollection xpcCustomerType = new XPCollection(_session, typeof(erp_customertype));
            if (xpcCustomerType.Count == 0)
            {
                var customerType1 = new erp_customertype(_session) { Ord = 10, Code = 10, Designation = "Normal" }; customerType1.Save();
                var customerType2 = new erp_customertype(_session) { Ord = 20, Code = 20, Designation = "Fraco" }; customerType2.Save();
                var customerType3 = new erp_customertype(_session) { Ord = 30, Code = 30, Designation = "Bom" }; customerType3.Save();
                var customerType4 = new erp_customertype(_session) { Ord = 40, Code = 40, Designation = "Muito Bom" }; customerType4.Save();
                var customerType5 = new erp_customertype(_session) { Ord = 50, Code = 50, Designation = "Empresa" }; customerType5.Save();
                var customerType6 = new erp_customertype(_session) { Ord = 60, Code = 60, Designation = "Fim-de-Semana" }; customerType6.Save();
            };

            //ConfigurationPriceType
            XPCollection xpcConfigurationPriceType = new XPCollection(_session, typeof(fin_configurationpricetype));
            if (xpcConfigurationPriceType.Count == 0)
            {
                var configurationPriceType1 = new fin_configurationpricetype(_session) { Ord = 10, Code = 10, Designation = "Normal", EnumValue = 0 }; configurationPriceType1.Save();
                var configurationPriceType2 = new fin_configurationpricetype(_session) { Ord = 20, Code = 20, Designation = "Balcão", EnumValue = 1 }; configurationPriceType2.Save();
                var configurationPriceType3 = new fin_configurationpricetype(_session) { Ord = 30, Code = 30, Designation = "Esplanada", EnumValue = 2 }; configurationPriceType3.Save();
                var configurationPriceType4 = new fin_configurationpricetype(_session) { Ord = 40, Code = 40, Designation = "Especial", EnumValue = 3 }; configurationPriceType4.Save();
                var configurationPriceType5 = new fin_configurationpricetype(_session) { Ord = 50, Code = 50, Designation = "Consumo", EnumValue = 4 }; configurationPriceType5.Save();

                //ConfigurationPlace
                XPCollection xpcConfigurationPlace = new XPCollection(_session, typeof(pos_configurationplace));
                if (xpcConfigurationPlace.Count == 0)
                {
                    var configurationPlace1 = new pos_configurationplace(_session) { Ord = 10, Code = 10, Designation = "Bar", PriceType = configurationPriceType1 }; configurationPlace1.Save();
                    var configurationPlace2 = new pos_configurationplace(_session) { Ord = 20, Code = 20, Designation = "Balcão", PriceType = configurationPriceType2 }; configurationPlace2.Save();
                    var configurationPlace3 = new pos_configurationplace(_session) { Ord = 30, Code = 30, Designation = "Sala", PriceType = configurationPriceType3 }; configurationPlace3.Save();
                    var configurationPlace4 = new pos_configurationplace(_session) { Ord = 40, Code = 40, Designation = "Esplanada", PriceType = configurationPriceType4 }; configurationPlace4.Save();
                    var configurationPlace5 = new pos_configurationplace(_session) { Ord = 50, Code = 50, Designation = "Take-Away", PriceType = configurationPriceType5 }; configurationPlace5.Save();
                    var configurationPlace6 = new pos_configurationplace(_session) { Ord = 60, Code = 60, Designation = "Piscina", PriceType = configurationPriceType5 }; configurationPlace6.Save();

                    //ConfigurationPlaceTable
                    XPCollection xpcConfigurationPlaceTable = new XPCollection(_session, typeof(pos_configurationplacetable));
                    if (xpcConfigurationPlaceTable.Count == 0)
                    {
                        //Zona1
                        var configurationPlaceTable1 = new pos_configurationplacetable(_session) { Ord = 10, Code = 10, Designation = "Table 1.1", Place = configurationPlace1 }; configurationPlaceTable1.Save();
                        var configurationPlaceTable2 = new pos_configurationplacetable(_session) { Ord = 20, Code = 20, Designation = "Table 1.2", Place = configurationPlace1, TableStatus = TableStatus.Open }; configurationPlaceTable2.Save();
                        var configurationPlaceTable3 = new pos_configurationplacetable(_session) { Ord = 30, Code = 30, Designation = "Table 1.3", Place = configurationPlace1, TableStatus = TableStatus.Reserved }; configurationPlaceTable3.Save();
                        var configurationPlaceTable4 = new pos_configurationplacetable(_session) { Ord = 40, Code = 40, Designation = "Table 1.4", Place = configurationPlace1 }; configurationPlaceTable4.Save();
                        var configurationPlaceTable5 = new pos_configurationplacetable(_session) { Ord = 50, Code = 50, Designation = "Table 1.5", Place = configurationPlace1 }; configurationPlaceTable5.Save();
                        //Zona2
                        var configurationPlaceTable6 = new pos_configurationplacetable(_session) { Ord = 60, Code = 60, Designation = "Table 2.1", Place = configurationPlace2, TableStatus = TableStatus.Open }; configurationPlaceTable6.Save();
                        var configurationPlaceTable7 = new pos_configurationplacetable(_session) { Ord = 70, Code = 70, Designation = "Table 2.2", Place = configurationPlace2, TableStatus = TableStatus.Reserved }; configurationPlaceTable7.Save();
                        var configurationPlaceTable8 = new pos_configurationplacetable(_session) { Ord = 80, Code = 80, Designation = "Table 2.3", Place = configurationPlace2, TableStatus = TableStatus.Open }; configurationPlaceTable8.Save();
                        var configurationPlaceTable9 = new pos_configurationplacetable(_session) { Ord = 90, Code = 90, Designation = "Table 2.4", Place = configurationPlace2, TableStatus = TableStatus.Free }; configurationPlaceTable9.Save();
                        var configurationPlaceTable10 = new pos_configurationplacetable(_session) { Ord = 100, Code = 100, Designation = "Table 2.5", Place = configurationPlace2 }; configurationPlaceTable10.Save();
                        //Zona3
                        var configurationPlaceTable11 = new pos_configurationplacetable(_session) { Ord = 110, Code = 110, Designation = "Table 3.1", Place = configurationPlace3, TableStatus = TableStatus.Open }; configurationPlaceTable11.Save();
                        var configurationPlaceTable12 = new pos_configurationplacetable(_session) { Ord = 120, Code = 120, Designation = "Table 3.2", Place = configurationPlace3, TableStatus = TableStatus.Reserved }; configurationPlaceTable12.Save();
                        var configurationPlaceTable13 = new pos_configurationplacetable(_session) { Ord = 130, Code = 130, Designation = "Table 3.3", Place = configurationPlace3, TableStatus = TableStatus.Free }; configurationPlaceTable13.Save();
                        var configurationPlaceTable14 = new pos_configurationplacetable(_session) { Ord = 140, Code = 140, Designation = "Table 3.4", Place = configurationPlace3, TableStatus = TableStatus.Reserved }; configurationPlaceTable14.Save();
                        var configurationPlaceTable15 = new pos_configurationplacetable(_session) { Ord = 150, Code = 150, Designation = "Table 3.5", Place = configurationPlace3, TableStatus = TableStatus.Free }; configurationPlaceTable15.Save();
                        //Zona4
                        var configurationPlaceTable16 = new pos_configurationplacetable(_session) { Ord = 160, Code = 160, Designation = "Table 4.1", Place = configurationPlace4, TableStatus = TableStatus.Reserved }; configurationPlaceTable16.Save();
                        var configurationPlaceTable17 = new pos_configurationplacetable(_session) { Ord = 170, Code = 170, Designation = "Table 4.2", Place = configurationPlace4, TableStatus = TableStatus.Open }; configurationPlaceTable17.Save();
                        var configurationPlaceTable18 = new pos_configurationplacetable(_session) { Ord = 180, Code = 180, Designation = "Table 4.3", Place = configurationPlace4, TableStatus = TableStatus.Reserved }; configurationPlaceTable18.Save();
                        var configurationPlaceTable19 = new pos_configurationplacetable(_session) { Ord = 190, Code = 190, Designation = "Table 4.4", Place = configurationPlace4, TableStatus = TableStatus.Open }; configurationPlaceTable19.Save();
                        var configurationPlaceTable20 = new pos_configurationplacetable(_session) { Ord = 200, Code = 200, Designation = "Table 4.5", Place = configurationPlace4, TableStatus = TableStatus.Reserved }; configurationPlaceTable20.Save();
                        //Zona5
                        var configurationPlaceTable21 = new pos_configurationplacetable(_session) { Ord = 210, Code = 210, Designation = "Table 5.1", Place = configurationPlace5, TableStatus = TableStatus.Reserved }; configurationPlaceTable21.Save();
                        var configurationPlaceTable22 = new pos_configurationplacetable(_session) { Ord = 220, Code = 220, Designation = "Table 5.2", Place = configurationPlace5, TableStatus = TableStatus.Reserved }; configurationPlaceTable22.Save();
                        var configurationPlaceTable23 = new pos_configurationplacetable(_session) { Ord = 230, Code = 230, Designation = "Table 5.3", Place = configurationPlace5, TableStatus = TableStatus.Free }; configurationPlaceTable23.Save();
                        var configurationPlaceTable24 = new pos_configurationplacetable(_session) { Ord = 240, Code = 240, Designation = "Table 5.4", Place = configurationPlace5, TableStatus = TableStatus.Free }; configurationPlaceTable24.Save();
                        var configurationPlaceTable25 = new pos_configurationplacetable(_session) { Ord = 250, Code = 250, Designation = "Table 5.5", Place = configurationPlace5, TableStatus = TableStatus.Free }; configurationPlaceTable25.Save();
                        //Zona6
                        var configurationPlaceTable26 = new pos_configurationplacetable(_session) { Ord = 260, Code = 260, Designation = "Table 6.1", Place = configurationPlace6, TableStatus = TableStatus.Open }; configurationPlaceTable26.Save();
                        var configurationPlaceTable27 = new pos_configurationplacetable(_session) { Ord = 270, Code = 270, Designation = "Table 6.2", Place = configurationPlace6, TableStatus = TableStatus.Free }; configurationPlaceTable27.Save();
                        var configurationPlaceTable28 = new pos_configurationplacetable(_session) { Ord = 280, Code = 280, Designation = "Table 6.3", Place = configurationPlace6, TableStatus = TableStatus.Free }; configurationPlaceTable28.Save();
                        var configurationPlaceTable29 = new pos_configurationplacetable(_session) { Ord = 290, Code = 290, Designation = "Table 6.4", Place = configurationPlace6, TableStatus = TableStatus.Open }; configurationPlaceTable29.Save();
                        var configurationPlaceTable30 = new pos_configurationplacetable(_session) { Ord = 300, Code = 300, Designation = "Table 6.5", Place = configurationPlace6, TableStatus = TableStatus.Free }; configurationPlaceTable30.Save();

                        //OrderMain
                        XPCollection xpcDocumentOrderMain = new XPCollection(_session, typeof(fin_documentordermain));
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
            XPCollection xpcReportType = new XPCollection(_session, typeof(rpt_reporttype));
            if (xpcReportType.Count == 0)
            {
                var reportType1 = new rpt_reporttype(_session) { Ord = 10, Code = 10, Designation = "ReportType #1", ResourceString = "reporttype_label_type1" }; reportType1.Save();
                var reportType2 = new rpt_reporttype(_session) { Ord = 20, Code = 20, Designation = "ReportType #2", ResourceString = "reporttype_label_type2" }; reportType2.Save();
                /* IN009072 */
                //var reportType3 = new rpt_reporttype(_session) { Ord = 30, Code = 30, Designation = "ReportType #3", ResourceString = "reporttype_label_type3" }; reportType3.Save();
                var reportType4 = new rpt_reporttype(_session) { Ord = 40, Code = 30, Designation = "ReportType #4", ResourceString = "reporttype_label_type4" }; reportType4.Save();
                var reportType5 = new rpt_reporttype(_session) { Ord = 50, Code = 30, Designation = "ReportType #5", ResourceString = "reporttype_label_type5" }; reportType5.Save();

                //Report
                XPCollection xpcReport = new XPCollection(_session, typeof(rpt_report));
                if (xpcReport.Count == 0)
                {
                    var report1 = new rpt_report(_session) { Ord = 10, Code = 10, Designation = "Relatório de Vendas por Familia", ReportType = reportType1, ResourceString = "report_label_sales_per_family", Token = "REPORT1" }; report1.Save();
                    var report2 = new rpt_report(_session) { Ord = 20, Code = 20, Designation = "Relatório de Total por Terminal", ReportType = reportType1, ResourceString = "report_label_total_per_terminal", Token = "REPORT2" }; report2.Save();
                    var report3 = new rpt_report(_session) { Ord = 30, Code = 30, Designation = "Relatório de Vendas por Dia", ReportType = reportType1, ResourceString = "report_label_sales_per_date", Token = "REPORT3" }; report3.Save();
                    var report4 = new rpt_report(_session) { Ord = 40, Code = 40, Designation = "Relatório de Vendas por Funcionário", ReportType = reportType1, ResourceString = "report_label_sales_per_user", Token = "REPORT4" }; report4.Save();
                    var report5 = new rpt_report(_session) { Ord = 50, Code = 50, Designation = "Relatório de Vendas por Tipo de Documento Fiscal", ReportType = reportType1, ResourceString = "report_label_sales_per_finance_document", Token = "REPORT5" }; report5.Save();
                };
            };

            //ConfigurationPreferenceParameter
            XPCollection xpcConfigurationPreferenceParameter = new XPCollection(_session, typeof(cfg_configurationpreferenceparameter));
            if (xpcConfigurationPreferenceParameter.Count == 0)
            {
                var configurationPreferenceParameter1 = new cfg_configurationpreferenceparameter(_session) { Ord = 10, Code = 10, Token = "COMPANY_NAME", ResourceString = "prefparam_company_name", Value = "LogicPulse" }; configurationPreferenceParameter1.Save();
                var configurationPreferenceParameter2 = new cfg_configurationpreferenceparameter(_session) { Ord = 20, Code = 20, Token = "COMPANY_ADDRESS", ResourceString = "prefparam_company_address", Value = "Rua da República, nº39 1º Esq" }; configurationPreferenceParameter2.Save();
                var configurationPreferenceParameter4 = new cfg_configurationpreferenceparameter(_session) { Ord = 30, Code = 30, Token = "COMPANY_ZIPCODE", ResourceString = "prefparam_company_zipcode", Value = "3080-035" }; configurationPreferenceParameter4.Save();
                var configurationPreferenceParameter5 = new cfg_configurationpreferenceparameter(_session) { Ord = 40, Code = 40, Token = "COMPANY_CITY", ResourceString = "prefparam_company_city", Value = "Figueira da Foz" }; configurationPreferenceParameter5.Save();
                var configurationPreferenceParameter6 = new cfg_configurationpreferenceparameter(_session) { Ord = 50, Code = 50, Token = "COMPANY_TELEPHONE", ResourceString = "prefparam_company_telephone", Value = "+351 233 042 347" }; configurationPreferenceParameter6.Save();
                var configurationPreferenceParameter7 = new cfg_configurationpreferenceparameter(_session) { Ord = 60, Code = 60, Token = "COMPANY_FAX", ResourceString = "prefparam_company_fax", Value = "" }; configurationPreferenceParameter7.Save();
                var configurationPreferenceParameter8 = new cfg_configurationpreferenceparameter(_session) { Ord = 70, Code = 70, Token = "COMPANY_EMAIL", ResourceString = "prefparam_company_email", Value = "mail@logicpulse.pt" }; configurationPreferenceParameter8.Save();
                var configurationPreferenceParameter9 = new cfg_configurationpreferenceparameter(_session) { Ord = 80, Code = 80, Token = "COMPANY_FISCALNUMBER", ResourceString = "prefparam_company_fiscalnumber", Value = "508 278 155" }; configurationPreferenceParameter9.Save();
            }

            //ConfigurationPreferenceParameter
            XPCollection xpcConfigurationPrintersTemplates = new XPCollection(_session, typeof(sys_configurationprinterstemplates));
            if (xpcConfigurationPrintersTemplates.Count == 0)
            {
                var configurationPrintersTemplates1 = new sys_configurationprinterstemplates(_session) { Ord = 10, Code = 1, Designation = "Modelo A4" }; configurationPrintersTemplates1.Save();
            }

            //PrinterType
            XPCollection xpcConfigurationPrintersType = new XPCollection(_session, typeof(sys_configurationprinterstype));
            if (xpcConfigurationPrintersType.Count == 0)
            {
                var configurationPrintersType1 = new sys_configurationprinterstype(_session) { Ord = 10, Code = 10, Designation = "Impressora SINOCAN em ambiente Windows", Token = "THERMAL_PRINTER_WINDOWS" }; configurationPrintersType1.Save();
                var configurationPrintersType2 = new sys_configurationprinterstype(_session) { Ord = 20, Code = 20, Designation = "Impressora SINOCAN em ambiente Linux", Token = "THERMAL_PRINTER_LINUX" }; configurationPrintersType2.Save();

                var configurationPrinters1 = new sys_configurationprinters(_session) { Ord = 10, Code = 10, Designation = "Impressora teste Windows", PrinterType = configurationPrintersType1 }; configurationPrinters1.Save();
                var configurationPrinters2 = new sys_configurationprinters(_session) { Ord = 20, Code = 20, Designation = "Impressora teste Linux", PrinterType = configurationPrintersType2 }; configurationPrinters2.Save();
            }

            //ConfigurationDevice
            pos_configurationdevice configurationDevice1 = new pos_configurationdevice(_session) { Ord = 10, Code = 10, Designation = "Table 3.1" }; configurationDevice1.Save();

            //sys_configurationinputreader
            sys_configurationinputreader configurationInputReader = new sys_configurationinputreader(_session) { Ord = 10, Code = 10, Designation = "InputReader1" }; configurationInputReader.Save();

            //sys_configurationpoledisplay
            sys_configurationpoledisplay configurationPoleDisplay = new sys_configurationpoledisplay(_session) { Ord = 10, Code = 10, Designation = "PoleDisplay1" }; configurationPoleDisplay.Save();

            //sys_configurationweighingmachine
            sys_configurationweighingmachine configurationWeighingMachine = new sys_configurationweighingmachine(_session) { Ord = 10, Code = 10, Designation = "WeighingMachine1" }; configurationWeighingMachine.Save();

            //ConfigurationHolidays
            cfg_configurationholidays configurationHolidays1 = new cfg_configurationholidays(_session) { Ord = 10, Code = 10, Designation = "Feriado", Day = 1, Month = 1, Year = 1, Fixed = true }; configurationHolidays1.Save();

            //SystemNotification      
            sys_systemnotification systemNotification1 = new sys_systemnotification(GlobalFramework.SessionXpo) { Message = "Welcome Message" };
            systemNotification1.Save();

            //SystemBackup
            XPCollection xpcSystemBackup = new XPCollection(_session, typeof(sys_systembackup));
            if (xpcSystemBackup.Count == 0)
            {
                var systemBackup1 = new sys_systembackup(_session) { FileName = "filename001.sql", FileNamePacked = "filename001.bak" }; systemBackup1.Save();
                var systemBackup2 = new sys_systembackup(_session) { FileName = "filename002.bak", FileNamePacked = "filename002.bak" }; systemBackup2.Save();
            }
        }

        public static void InitDocumentFinance(Session pSession)
        {
            //Init Parameters
            _session = pSession;

            var documentFinanceYear1 = new fin_documentfinanceyears(_session) { Designation = "2013", FiscalYear = 2013 }; documentFinanceYear1.Save();

            //DocumentFinanceType
            XPCollection xpcDocumentFinanceType = new XPCollection(_session, typeof(fin_documentfinancetype));
            if (xpcDocumentFinanceType.Count == 0)
            {
                var documentFinanceType1 = new fin_documentfinancetype(_session) { Ord = 1, Designation = "Fatura", Acronym = "FT", ResourceString = "global_documentfinance_type_title_ft" }; documentFinanceType1.Save();

                XPCollection xpcDocumentFinanceDetail = new XPCollection(_session, typeof(fin_documentfinancedetail));
                if (xpcDocumentFinanceDetail.Count == 0)
                {
                    var documentFinanceDetail1 = new fin_documentfinancedetail(_session) { }; documentFinanceDetail1.Save();
                }

                //DocumentFinanceDetailOrderReference
                var documentFinanceDetailOrderReference1 = new fin_documentfinancedetailorderreference(_session) { OriginatingON = "ORDER#001", OrderDate = System.DateTime.Now }; documentFinanceDetailOrderReference1.Save();

                //DocumentFinanceDetailReference
                var documentFinanceDetailReference1 = new fin_documentfinancedetailreference(_session) { Reference = "ORDER#001", Reason = "ORDER#001" }; documentFinanceDetailReference1.Save();

                //DocumentFinanceSeries
                var documentFinanceSeries1 = new fin_documentfinanceseries(_session) { Designation = "Fatura 2013", FiscalYear = documentFinanceYear1, DocumentType = documentFinanceType1, NextDocumentNumber = 1 }; documentFinanceType1.Save();

                //DocumentFinanceYearSerieTerminal
                var documentFinanceYearSerieTerminal1 = new fin_documentfinanceyearserieterminal(_session) { Designation = "Fatura 2013" }; documentFinanceType1.Save();
            }

            XPCollection xpcDocumentFinanceCommission = new XPCollection(_session, typeof(fin_documentfinancecommission));
            if (xpcDocumentFinanceCommission.Count == 0)
            {
                var documentFinanceCommission1 = new fin_documentfinancecommission(_session) { Ord = 1, Commission = 10, Total = 10 }; documentFinanceCommission1.Save();
            }
        }

        public static void InitWorkSession(Session pSession)
        {
            //Init Parameters
            _session = pSession;

            //WorkSession
            XPCollection xpcWorkSessionMovementType = new XPCollection(_session, typeof(pos_worksessionmovementtype));
            if (xpcWorkSessionMovementType.Count == 0)
            {
                var workSessionMovementType1 = new pos_worksessionmovementtype(_session) { Ord = 1, Code = 10, Token = "FINANCE_DOCUMENT", Designation = "Documento Fiscal" }; workSessionMovementType1.Save();
                var workSessionMovementType2 = new pos_worksessionmovementtype(_session) { Ord = 2, Code = 20, Token = "CASHDRAWER_OPEN", Designation = "Abertura de Caixa", ResourceString = "pos_button_label_cashdrawer_open", ButtonIcon = @"Icons\icon_pos_cashdrawer_open.png", CashDrawerMovement = true }; workSessionMovementType2.Save();
                var workSessionMovementType6 = new pos_worksessionmovementtype(_session) { Ord = 3, Code = 30, Token = "CASHDRAWER_CLOSE", Designation = "Fecho de Caixa", ResourceString = "pos_button_label_cashdrawer_close", ButtonIcon = @"Icons\icon_pos_cashdrawer_close.png", CashDrawerMovement = true }; workSessionMovementType6.Save();
                var workSessionMovementType3 = new pos_worksessionmovementtype(_session) { Ord = 4, Code = 40, Token = "CASHDRAWER_IN", Designation = "Entrada de Caixa", ResourceString = "pos_button_label_cashdrawer_in", ButtonIcon = @"Icons\icon_pos_cashdrawer_in.png", CashDrawerMovement = true }; workSessionMovementType3.Save();
                var workSessionMovementType4 = new pos_worksessionmovementtype(_session) { Ord = 5, Code = 50, Token = "CASHDRAWER_OUT", Designation = "Saida de Caixa", ResourceString = "pos_button_label_cashdrawer_out", ButtonIcon = @"Icons\icon_pos_cashdrawer_out.png", CashDrawerMovement = true }; workSessionMovementType4.Save();
                var workSessionMovementType5 = new pos_worksessionmovementtype(_session) { Ord = 6, Code = 60, Token = "CASHDRAWER_MONEY_OUT", Designation = "Sangria", ResourceString = "pos_button_label_cashdrawer_money_out", ButtonIcon = @"Icons\icon_pos_cashdrawer_money_out.png", CashDrawerMovement = true, Disabled = true }; workSessionMovementType5.Save();

                var workSessionPeriod1 = new pos_worksessionperiod(_session) { Designation = "WorkSessionPeriod No.1", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod1.Save();
                var workSessionPeriod2 = new pos_worksessionperiod(_session) { Designation = "WorkSessionPeriod No.2", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod2.Save();
                var workSessionPeriod3 = new pos_worksessionperiod(_session) { Designation = "WorkSessionPeriod No.3", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod3.Save();
                var workSessionPeriod4 = new pos_worksessionperiod(_session) { Designation = "WorkSessionPeriod No.4", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod4.Save();
                var workSessionPeriod5 = new pos_worksessionperiod(_session) { Designation = "WorkSessionPeriod No.5", DateStart = DateTime.Now, DateEnd = DateTime.Now, SessionStatus = WorkSessionPeriodStatus.Close }; workSessionPeriod5.Save();

                var workSessionMovement1 = new pos_worksessionmovement(_session) { Date = DateTime.Now, MovementAmount = 100.10m, WorkSessionPeriod = workSessionPeriod1, WorkSessionMovementType = workSessionMovementType1 }; workSessionMovement1.Save();
                var workSessionMovement2 = new pos_worksessionmovement(_session) { Date = DateTime.Now, MovementAmount = 200.20m, WorkSessionPeriod = workSessionPeriod2, WorkSessionMovementType = workSessionMovementType2 }; workSessionMovement2.Save();
                var workSessionMovement3 = new pos_worksessionmovement(_session) { Date = DateTime.Now, MovementAmount = 300.30m, WorkSessionPeriod = workSessionPeriod3, WorkSessionMovementType = workSessionMovementType3 }; workSessionMovement3.Save();
                var workSessionMovement4 = new pos_worksessionmovement(_session) { Date = DateTime.Now, MovementAmount = 400.40m, WorkSessionPeriod = workSessionPeriod4, WorkSessionMovementType = workSessionMovementType4 }; workSessionMovement4.Save();

                XPCollection xpcConfigurationPaymentMethod = new XPCollection(_session, typeof(fin_configurationpaymentmethod));
                decimal amount = 10.10m;
                foreach (fin_configurationpaymentmethod paymentMethod in xpcConfigurationPaymentMethod)
                {
                    var workSessionPeriodTotal = new pos_worksessionperiodtotal(_session) { PaymentMethod = paymentMethod, Total = amount, Period = workSessionPeriod1 }; workSessionPeriodTotal.Save();
                    amount = amount * 2;
                }
            }
        }
    }
}
