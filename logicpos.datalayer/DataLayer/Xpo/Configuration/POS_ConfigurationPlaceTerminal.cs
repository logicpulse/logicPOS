using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_configurationplaceterminal : XPGuidObject
    {
        public pos_configurationplaceterminal() : base() { }
        public pos_configurationplaceterminal(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplaceterminal), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationplaceterminal), "Code");
            TemplateTicket = this.Session.GetObjectByKey<sys_configurationprinterstemplates>(SettingsApp.XpoOidConfigurationPrintersTemplateTicket);
            TemplateTablesConsult = this.Session.GetObjectByKey<sys_configurationprinterstemplates>(SettingsApp.XpoOidConfigurationPrintersTemplateTableConsult);
            InputReaderTimerInterval = 200;
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

        UInt32 fInputReaderTimerInterval;
        public UInt32 InputReaderTimerInterval
        {
            get { return fInputReaderTimerInterval; }
            set { SetPropertyValue<UInt32>("InputReaderTimerInterval", ref fInputReaderTimerInterval, value); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTerminal
        pos_configurationplace fPlace;
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTerminal")]
        public pos_configurationplace Place
        {
            get { return fPlace; }
            set { SetPropertyValue<pos_configurationplace>("Place", ref fPlace, value); }
        }

        //ConfigurationPrinters One <> Many ConfigurationPlaceTerminal
        sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesConfigurationPlaceTerminal")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //ConfigurationPrinters One <> Many ConfigurationPlaceTerminal
        sys_configurationprinters fThermalPrinter;
        [Association(@"ConfigurationThermalPrintersReferencesConfigurationPlaceTerminal")]
        public sys_configurationprinters ThermalPrinter
        {
            get { return fThermalPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("ThermalPrinter", ref fThermalPrinter, value); }
        }

        //BarcodeReader One <> Many ConfigurationPlaceTerminal
        sys_configurationinputreader fBarcodeReader;
        [Association(@"ConfigurationHardwareInputReader1ReferencesConfigurationPlaceTerminal")]
        public sys_configurationinputreader BarcodeReader
        {
            get { return fBarcodeReader; }
            set { SetPropertyValue<sys_configurationinputreader>("BarcodeReader", ref fBarcodeReader, value); }
        }

        //ConfigurationInputReader One <> Many ConfigurationPlaceTerminal
        sys_configurationinputreader fCardReader;
        [Association(@"ConfigurationHardwareInputReader2ReferencesConfigurationPlaceTerminal")]
        public sys_configurationinputreader CardReader
        {
            get { return fCardReader; }
            set { SetPropertyValue<sys_configurationinputreader>("CardReader", ref fCardReader, value); }
        }

        //ConfigurationPoleDisplay One <> Many ConfigurationPlaceTerminal
        sys_configurationpoledisplay fPoleDisplay;
        [Association(@"ConfigurationHardwarePoleDisplayReferencesConfigurationPlaceTerminal")]
        public sys_configurationpoledisplay PoleDisplay
        {
            get { return fPoleDisplay; }
            set { SetPropertyValue<sys_configurationpoledisplay>("PoleDisplay", ref fPoleDisplay, value); }
        }

        //ConfigurationWeighingMachine One <> Many ConfigurationPlaceTerminal
        sys_configurationweighingmachine fWeighingMachine;
        [Association(@"ConfigurationHardwareWeighingMachineReferencesConfigurationPlaceTerminal")]
        public sys_configurationweighingmachine WeighingMachine
        {
            get { return fWeighingMachine; }
            set { SetPropertyValue<sys_configurationweighingmachine>("WeighingMachine", ref fWeighingMachine, value); }
        }

        //ConfigurationPlaceTerminal One <> Many DocumentFinanceYearSerieTerminal
        [Association(@"ConfigurationPlaceTerminalReferencesDFYearSerieTerminal", typeof(fin_documentfinanceyearserieterminal))]
        public XPCollection<fin_documentfinanceyearserieterminal> YearSerieTerminal
        {
            get { return GetCollection<fin_documentfinanceyearserieterminal>("YearSerieTerminal"); }
        }

        //ConfigurationPrintersTemplates One <> Many ConfigurationPlaceTerminal
        sys_configurationprinterstemplates fTemplateTicket;
        [Association(@"ConfigurationPrintersTemplatesTemplateTicketReferencesTerminal")]
        public sys_configurationprinterstemplates TemplateTicket
        {
            get { return fTemplateTicket; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("TemplateTicket", ref fTemplateTicket, value); }
        }

        //ConfigurationPrintersTemplates One <> Many ConfigurationPlaceTerminal
        sys_configurationprinterstemplates fTemplateTablesConsult;
        [Association(@"ConfigurationPrintersTemplatesTemplateTablesConsultReferencesTerminal")]
        public sys_configurationprinterstemplates TemplateTablesConsult
        {
            get { return fTemplateTablesConsult; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("TemplateTablesConsult", ref fTemplateTablesConsult, value); }
        }
    }
}