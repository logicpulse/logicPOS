using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using System;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using logicpos.datalayer.Xpo;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPage8 : PagePadPage
    {
        private readonly Session _session;
        private readonly DocumentFinanceDialogPagePad _pagePad;
        private readonly GenericCRUDWidgetListXPO _crudWidgetList;
        //UI EntryBox
        private readonly EntryBoxValidation _entryBoxClient;
        private readonly EntryBoxValidation _entryBoxClientValidation;
        private readonly EntryBoxValidation _entryBoxFiscalNumber;
        private readonly EntryBoxValidation _entryBoxAddress;
        private readonly EntryBoxValidation _entryBoxLocality;
        private readonly EntryBoxValidation _entryBoxZipCode;
        private readonly EntryBoxValidation _entryBoxCity;
        private readonly XPOEntryBoxSelectRecord<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectCountry;
        private readonly XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectCountryValidation;
        private readonly EntryBoxValidation _entryBoxNotes;

        public erp_customer ValueCustomer { get; set; }

        //Constructor
        public DocumentFinanceDialogPage8(Window pSourceWindow, string pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage8(Window pSourceWindow, string pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage8(Window pSourceWindow, string pPageName, string pPageIcon, Widget pWidget, bool pEnabled = true)
            : base(pSourceWindow, pPageName, pPageIcon, pWidget, pEnabled)
        {
            //Init private vars
            _pagePad = (_sourceWindow as PosDocumentFinanceDialog).PagePad;
            _session = (_pagePad as DocumentFinanceDialogPagePad).Session;
            _crudWidgetList = new GenericCRUDWidgetListXPO(_session);

            //Get Target Object
            //Guid customerGuid = new Guid("0cf40622-578b-417d-b50f-e945fefb5d68");//Consumidor Final|0.0
            Guid customerGuid = new Guid("765859cc-29c2-4925-be89-0486d03684f2");//Carlos Fernandes|5.0
            //Guid customerGuid = new Guid("78c08879-6d08-4146-9cc9-914f427926c6");//Cristina Janeiro|12.5
            ValueCustomer = (erp_customer)XPOHelper.GetXPGuidObject(_session, typeof(erp_customer), customerGuid);

            //Client (Used in _crudWidgetList)
            _entryBoxClient = new EntryBoxValidation(_sourceWindow, string.Format("{0}/WL", CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_customer")), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true);
            _entryBoxClient.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxClientValidation = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_customer"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true);
            _entryBoxClientValidation.EntryValidation.Changed += delegate { Validate(); };
            //FiscalNumber
            _entryBoxFiscalNumber = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_fiscal_number"), KeyboardMode.Alfa, ValueCustomer.Country.RegExFiscalNumber, true);
            _entryBoxFiscalNumber.EntryValidation.Changed += delegate { /*ValidateFiscalNumber();*/ Validate(); };
            //Address
            _entryBoxAddress = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_address"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            //_entryBoxAddress.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxAddress.EntryValidation.Changed += delegate { Validate(); };
            //Locality
            _entryBoxLocality = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_locality"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfa, false);
            //_entryBoxLocality.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxLocality.EntryValidation.Changed += delegate { Validate(); };
            //ZipCode
            _entryBoxZipCode = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_zipcode"), KeyboardMode.Alfa, ValueCustomer.Country.RegExZipCode, false);
            _entryBoxZipCode.WidthRequest = 200;
            _entryBoxZipCode.EntryValidation.Changed += delegate { Validate(); };
            //City
            _entryBoxCity = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_city"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfa, false);
            //_entryBoxCity.WidthRequest = _pagePad.EntryBoxMaxWidth - 200;
            _entryBoxCity.EntryValidation.Changed += delegate { Validate(); };

            //Country (Used in _crudWidgetList)
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("CurrencyCode = 'EUR'");
            _entryBoxSelectCountry = new XPOEntryBoxSelectRecord<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, string.Format("{0}/WL", CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_country")), "Designation", "Oid", ValueCustomer.Country, criteriaOperator);
            //_entryBoxSelectCountry.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxSelectCountry.Entry.IsEditable = false;
            //CountryValidation
            _entryBoxSelectCountryValidation = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_country"), "Designation", "Oid", ValueCustomer.Country, criteriaOperator, LogicPOS.Utility.RegexUtils.RegexGuid, true);
            //_entryBoxSelectCountryValidation.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxSelectCountryValidation.EntryValidation.IsEditable = false;
            //Test _selectedXPGuidObject :)
            //_entryBoxSelectCountryValidation.EntryValidation.Changed += delegate { 
            //  _logger.Debug(string.Format("_entryBoxCountryValidation.Selected.Code3: [{0}]", _entryBoxSelectCountryValidation.Value.Code3));
            //};

            //Notes
            _entryBoxNotes = new EntryBoxValidation(_sourceWindow, CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "global_notes"), KeyboardMode.Alfa, LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false);
            //_entryBoxNotes.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxNotes.EntryValidation.Changed += delegate { Validate(); };

            ////OPTIONAL: REQUIRE Assign Values TO Work On NON XpoWidget Mode ELSE Leave Blanck and XPOWidget Init Will fill Initial Values
            //_entryBoxClient.Entry.Text = xCustomer.Name;
            _entryBoxClientValidation.EntryValidation.Text = ValueCustomer.Name;
            ////Assign Value First
            //_entryBoxCountry.Value = xCustomer.Country;
            //_entryBoxCountry.Entry.Text = xCustomer.Country.Designation;
            ////Assign Value First
            //_entryBoxCountryValidation.Value = xCustomer.Country;
            //_entryBoxCountryValidation.EntryValidation.Text = xCustomer.Country.Designation;

            //Using Labels ;)
            GenericCRUDWidget<XPGuidObject> crudWidgetClientName = new GenericCRUDWidgetXPO(_entryBoxClient, _entryBoxClient.Label, ValueCustomer, "Name", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true);
            GenericCRUDWidget<XPGuidObject> crudWidgetClientCountry = new GenericCRUDWidgetXPO(_entryBoxSelectCountry, _entryBoxSelectCountry.Label, ValueCustomer, "Country", LogicPOS.Utility.RegexUtils.RegexGuid, true);
            _crudWidgetList.Add(crudWidgetClientName);
            _crudWidgetList.Add(crudWidgetClientCountry);

            Button button = new Button("VALIDATE & UpdateRecord");
            button.Clicked += delegate
            {
                if (_crudWidgetList.ValidateRecord())
                {
                    //Using WidgetList
                    _crudWidgetList.UpdateRecord(DialogMode.Update);
                }
            };

            //Pack VBOX
            VBox vbox = new VBox(false, 2);
            vbox.PackStart(_entryBoxClient, false, false, 0);
            vbox.PackStart(_entryBoxClientValidation, false, false, 0);
            vbox.PackStart(_entryBoxSelectCountry, false, false, 0);
            vbox.PackStart(_entryBoxSelectCountryValidation, false, false, 0);
            vbox.PackStart(button, true, true, 0);
            PackStart(vbox);
        }

        //Override Base Validate
        public override void Validate()
        {
            //_logger.Debug(string.Format("Validate: {0}", this.Name));
        }
    }
}
