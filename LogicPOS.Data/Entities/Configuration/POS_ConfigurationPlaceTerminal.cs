using DevExpress.Xpo;
using logicpos.datalayer.Xpo;
using LogicPOS.Data.XPO.Settings;
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
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_configurationplaceterminal), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_configurationplaceterminal), "Code");
            TemplateTicket = this.Session.GetObjectByKey<sys_configurationprinterstemplates>(XPOSettings.XpoOidConfigurationPrintersTemplateTicket);
            TemplateTablesConsult = this.Session.GetObjectByKey<sys_configurationprinterstemplates>(XPOSettings.XpoOidConfigurationPrintersTemplateTableConsult);
            InputReaderTimerInterval = 200;
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fHardwareId;
        [Indexed(Unique = true), Size(30)]
        public string HardwareId
        {
            get { return fHardwareId; }
            set { SetPropertyValue<string>("HardwareId", ref fHardwareId, value); }
        }

        private uint fInputReaderTimerInterval;
        public uint InputReaderTimerInterval
        {
            get { return fInputReaderTimerInterval; }
            set { SetPropertyValue<UInt32>("InputReaderTimerInterval", ref fInputReaderTimerInterval, value); }
        }

        //ConfigurationPlace One <> Many ConfigurationPlaceTerminal
        private pos_configurationplace fPlace;
        [Association(@"ConfigurationPlaceReferencesConfigurationPlaceTerminal")]
        public pos_configurationplace Place
        {
            get { return fPlace; }
            set { SetPropertyValue<pos_configurationplace>("Place", ref fPlace, value); }
        }

        //ConfigurationPrinters One <> Many ConfigurationPlaceTerminal
        private sys_configurationprinters fPrinter;
        [Association(@"ConfigurationPrintersReferencesConfigurationPlaceTerminal")]
        public sys_configurationprinters Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("Printer", ref fPrinter, value); }
        }

        //ConfigurationPrinters One <> Many ConfigurationPlaceTerminal
        private sys_configurationprinters fThermalPrinter;
        [Association(@"ConfigurationThermalPrintersReferencesConfigurationPlaceTerminal")]
        public sys_configurationprinters ThermalPrinter
        {
            get { return fThermalPrinter; }
            set { SetPropertyValue<sys_configurationprinters>("ThermalPrinter", ref fThermalPrinter, value); }
        }

        //BarcodeReader One <> Many ConfigurationPlaceTerminal
        private sys_configurationinputreader fBarcodeReader;
        [Association(@"ConfigurationHardwareInputReader1ReferencesConfigurationPlaceTerminal")]
        public sys_configurationinputreader BarcodeReader
        {
            get { return fBarcodeReader; }
            set { SetPropertyValue<sys_configurationinputreader>("BarcodeReader", ref fBarcodeReader, value); }
        }

        //ConfigurationInputReader One <> Many ConfigurationPlaceTerminal
        private sys_configurationinputreader fCardReader;
        [Association(@"ConfigurationHardwareInputReader2ReferencesConfigurationPlaceTerminal")]
        public sys_configurationinputreader CardReader
        {
            get { return fCardReader; }
            set { SetPropertyValue<sys_configurationinputreader>("CardReader", ref fCardReader, value); }
        }

        //ConfigurationPoleDisplay One <> Many ConfigurationPlaceTerminal
        private sys_configurationpoledisplay fPoleDisplay;
        [Association(@"ConfigurationHardwarePoleDisplayReferencesConfigurationPlaceTerminal")]
        public sys_configurationpoledisplay PoleDisplay
        {
            get { return fPoleDisplay; }
            set { SetPropertyValue<sys_configurationpoledisplay>("PoleDisplay", ref fPoleDisplay, value); }
        }

        //ConfigurationWeighingMachine One <> Many ConfigurationPlaceTerminal
        private sys_configurationweighingmachine fWeighingMachine;
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
        private sys_configurationprinterstemplates fTemplateTicket;
        [Association(@"ConfigurationPrintersTemplatesTemplateTicketReferencesTerminal")]
        public sys_configurationprinterstemplates TemplateTicket
        {
            get { return fTemplateTicket; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("TemplateTicket", ref fTemplateTicket, value); }
        }

        //ConfigurationPrintersTemplates One <> Many ConfigurationPlaceTerminal
        private sys_configurationprinterstemplates fTemplateTablesConsult;
        [Association(@"ConfigurationPrintersTemplatesTemplateTablesConsultReferencesTerminal")]
        public sys_configurationprinterstemplates TemplateTablesConsult
        {
            get { return fTemplateTablesConsult; }
            set { SetPropertyValue<sys_configurationprinterstemplates>("TemplateTablesConsult", ref fTemplateTablesConsult, value); }
        }
    }
}