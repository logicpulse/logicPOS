using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Keyboard;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPage8 : PagePadPage
    {
        private Session _session;
        private DocumentFinanceDialogPagePad _pagePad;
        private GenericCRUDWidgetListXPO _crudWidgetList;
        //UI EntryBox
        private EntryBoxValidation _entryBoxClient;
        private EntryBoxValidation _entryBoxClientValidation;
        private EntryBoxValidation _entryBoxFiscalNumber;
        private EntryBoxValidation _entryBoxAddress;
        private EntryBoxValidation _entryBoxLocality;
        private EntryBoxValidation _entryBoxZipCode;
        private EntryBoxValidation _entryBoxCity;
        private XPOEntryBoxSelectRecord<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectCountry;
        private XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry> _entryBoxSelectCountryValidation;
        private EntryBoxValidation _entryBoxNotes;
        //Public
        private erp_customer _valueCustomer;
        public erp_customer ValueCustomer
        {
            get { return _valueCustomer; }
            set { _valueCustomer = value; }
        }

        //Constructor
        public DocumentFinanceDialogPage8(Window pSourceWindow, String pPageName) : this(pSourceWindow, pPageName, "", null, true) { }
        public DocumentFinanceDialogPage8(Window pSourceWindow, String pPageName, Widget pWidget) : this(pSourceWindow, pPageName, "", pWidget, true) { }
        public DocumentFinanceDialogPage8(Window pSourceWindow, String pPageName, String pPageIcon, Widget pWidget, bool pEnabled = true)
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
            _valueCustomer = (erp_customer)FrameworkUtils.GetXPGuidObject(_session, typeof(erp_customer), customerGuid);

            //Client (Used in _crudWidgetList)
            _entryBoxClient = new EntryBoxValidation(_sourceWindow, string.Format("{0}/WL", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customer")), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxClient.EntryValidation.Changed += delegate { Validate(); };
            _entryBoxClientValidation = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_customer"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, true);
            _entryBoxClientValidation.EntryValidation.Changed += delegate { Validate(); };
            //FiscalNumber
            _entryBoxFiscalNumber = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), KeyboardMode.Alfa, _valueCustomer.Country.RegExFiscalNumber, true);
            _entryBoxFiscalNumber.EntryValidation.Changed += delegate { /*ValidateFiscalNumber();*/ Validate(); };
            //Address
            _entryBoxAddress = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_address"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            //_entryBoxAddress.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxAddress.EntryValidation.Changed += delegate { Validate(); };
            //Locality
            _entryBoxLocality = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_locality"), KeyboardMode.Alfa, SettingsApp.RegexAlfa, false);
            //_entryBoxLocality.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxLocality.EntryValidation.Changed += delegate { Validate(); };
            //ZipCode
            _entryBoxZipCode = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_zipcode"), KeyboardMode.Alfa, _valueCustomer.Country.RegExZipCode, false);
            _entryBoxZipCode.WidthRequest = 200;
            _entryBoxZipCode.EntryValidation.Changed += delegate { Validate(); };
            //City
            _entryBoxCity = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_city"), KeyboardMode.Alfa, SettingsApp.RegexAlfa, false);
            //_entryBoxCity.WidthRequest = _pagePad.EntryBoxMaxWidth - 200;
            _entryBoxCity.EntryValidation.Changed += delegate { Validate(); };

            //Country (Used in _crudWidgetList)
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse("CurrencyCode = 'EUR'");
            _entryBoxSelectCountry = new XPOEntryBoxSelectRecord<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, string.Format("{0}/WL", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country")), "Designation", "Oid", _valueCustomer.Country, criteriaOperator);
            //_entryBoxSelectCountry.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxSelectCountry.Entry.IsEditable = false;
            //CountryValidation
            _entryBoxSelectCountryValidation = new XPOEntryBoxSelectRecordValidation<cfg_configurationcountry, TreeViewConfigurationCountry>(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_country"), "Designation", "Oid", _valueCustomer.Country, criteriaOperator, SettingsApp.RegexGuid, true);
            //_entryBoxSelectCountryValidation.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxSelectCountryValidation.EntryValidation.IsEditable = false;
            //Test _selectedXPGuidObject :)
            //_entryBoxSelectCountryValidation.EntryValidation.Changed += delegate { 
            //  _log.Debug(string.Format("_entryBoxCountryValidation.Selected.Code3: [{0}]", _entryBoxSelectCountryValidation.Value.Code3));
            //};

            //Notes
            _entryBoxNotes = new EntryBoxValidation(_sourceWindow, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_notes"), KeyboardMode.Alfa, SettingsApp.RegexAlfaNumericExtended, false);
            //_entryBoxNotes.WidthRequest = _pagePad.EntryBoxMaxWidth;
            _entryBoxNotes.EntryValidation.Changed += delegate { Validate(); };

            ////OPTIONAL: REQUIRE Assign Values TO Work On NON XpoWidget Mode ELSE Leave Blanck and XPOWidget Init Will fill Initial Values
            //_entryBoxClient.Entry.Text = xCustomer.Name;
            _entryBoxClientValidation.EntryValidation.Text = _valueCustomer.Name;
            ////Assign Value First
            //_entryBoxCountry.Value = xCustomer.Country;
            //_entryBoxCountry.Entry.Text = xCustomer.Country.Designation;
            ////Assign Value First
            //_entryBoxCountryValidation.Value = xCustomer.Country;
            //_entryBoxCountryValidation.EntryValidation.Text = xCustomer.Country.Designation;

            //Using Labels ;)
            GenericCRUDWidget<XPGuidObject> crudWidgetClientName = new GenericCRUDWidgetXPO(_entryBoxClient, _entryBoxClient.Label, _valueCustomer, "Name", SettingsApp.RegexAlfaNumericExtended, true);
            GenericCRUDWidget<XPGuidObject> crudWidgetClientCountry = new GenericCRUDWidgetXPO(_entryBoxSelectCountry, _entryBoxSelectCountry.Label, _valueCustomer, "Country", SettingsApp.RegexGuid, true);
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
            //_log.Debug(string.Format("Validate: {0}", this.Name));
        }
    }
}
