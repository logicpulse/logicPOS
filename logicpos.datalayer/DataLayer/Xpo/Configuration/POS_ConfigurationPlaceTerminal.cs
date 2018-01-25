using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_ConfigurationPlaceTerminal : XPGuidObject
    {
        public POS_ConfigurationPlaceTerminal() : base() { }
        public POS_ConfigurationPlaceTerminal(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlaceTerminal", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("POS_ConfigurationPlaceTerminal", "Code");
            TemplateTicket = this.Session.GetObjectByKey<SYS_ConfigurationPrintersTemplates>(SettingsApp.XpoOidConfigurationPrintersTemplateTicket);
            TemplateTablesConsult = this.Session.GetObjectByKey<SYS_ConfigurationPrintersTemplates>(SettingsApp.XpoOidConfigurationPrintersTemplateTableConsult);
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fHardwareId;
        [Indexed(Unique = true), Size(30)]
        public string HardwareId
        {
            get { return fHardwareId; }
            set { SetPropertyValue<string>("HardwareId", ref fHardwareId, value); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTerminal
        POS_ConfigurationPlace fPlace;
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTerminal")]
        public POS_ConfigurationPlace Place
        {
            get { return fPlace; }
            set { SetPropertyValue<POS_ConfigurationPlace>("Place", ref fPlace, value); }
        }

        //ConfigurationPrinters One <> Many ConfigurationPlaceTerminal
        SYS_ConfigurationPrinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesConfigurationPlaceTerminal")]
        public SYS_ConfigurationPrinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<SYS_ConfigurationPrinters>("Printer", ref fPrinter, value); }
        }

        //ConfigurationPlaceTerminal One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPlaceTerminalReferencesDFYearSerieTerminal", typeof(FIN_DocumentFinanceYearSerieTerminal))]
        public XPCollection<FIN_DocumentFinanceYearSerieTerminal> YearSerieTerminal
        {
            get { return GetCollection<FIN_DocumentFinanceYearSerieTerminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrintersTemplates One <> Many ConfigurationPlaceTerminal
        SYS_ConfigurationPrintersTemplates fTemplateTicket;
        [Association(@"ConfigurationPrintersTemplatesTemplateTicketReferencesTerminal")]
        public SYS_ConfigurationPrintersTemplates TemplateTicket
        {
            get { return fTemplateTicket; }
            set { SetPropertyValue<SYS_ConfigurationPrintersTemplates>("TemplateTicket", ref fTemplateTicket, value); }
        }

        //ConfigurationPrintersTemplates One <> Many ConfigurationPlaceTerminal
        SYS_ConfigurationPrintersTemplates fTemplateTablesConsult;
        [Association(@"ConfigurationPrintersTemplatesTemplateTablesConsultReferencesTerminal")]
        public SYS_ConfigurationPrintersTemplates TemplateTablesConsult
        {
            get { return fTemplateTablesConsult; }
            set { SetPropertyValue<SYS_ConfigurationPrintersTemplates>("TemplateTablesConsult", ref fTemplateTablesConsult, value); }
        }
    }
}