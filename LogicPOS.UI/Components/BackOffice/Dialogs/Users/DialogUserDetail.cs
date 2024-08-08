using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Utility;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogUserDetail : EditDialog
    {
        //Private UI References
        private CheckButton _checkButtonPasswordReset;
        //Reference to UserDetail
        private readonly sys_userdetail _userDetail;
        //Old Values before Changes
        private Guid _currentUserPermissionProfileGuid;
        private readonly bool _currentUserPasswordReset;
        private readonly bool _currentUserDisabled;
        private readonly bool _isLoggedUser = false;

        public DialogUserDetail(Window parentWindow, XpoGridView pTreeView, DialogFlags pFlags, DialogMode pDialogMode, Entity pXPGuidObject)
            : base(parentWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_edit_user"));
            SetSizeRequest(500, 469);

            //Store References
            _userDetail = (Entity as sys_userdetail);
            _isLoggedUser = (_userDetail == XPOSettings.LoggedUser);
            
            //Store Current User Unchanged Profile
            _currentUserPermissionProfileGuid = (_userDetail.Profile != null && _dialogMode == DialogMode.Update)
                ? _userDetail.Profile.Oid
                : Guid.Empty;

            //Store Current User Unchanged PasswordReset
            _currentUserPasswordReset = _userDetail.PasswordReset;
            //Store Current User Unchanged Disabled
            _currentUserDisabled = _userDetail.Disabled;

            //Init UI
            InitUI();
            InitNotes();
            ShowAll();
        }

        private void InitUI()
        {
            try
            {
                //Tab1
                VBox vboxTab1 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Ord
                Entry entryOrd = new Entry();
                BOWidgetBox boxLabel = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLabel, Entity, "Ord", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCode, Entity, "Code", RegexUtils.RegexIntegerGreaterThanZero, true));

                //Name
                Entry entryName = new Entry();
                BOWidgetBox boxName = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_name"), entryName);
                vboxTab1.PackStart(boxName, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxName, Entity, "Name", RegexUtils.RegexAlfaNumericExtended, true));

                //Profile
                XPOComboBox xpoComboBoxProfile = new XPOComboBox(Entity.Session, typeof(sys_userprofile), (Entity as sys_userdetail).Profile, "Designation", null);
                BOWidgetBox boxProfile = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_user_profiles"), xpoComboBoxProfile);
                vboxTab1.PackStart(boxProfile, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxProfile, Entity, "Profile", RegexUtils.RegexGuid, true));

                //CommissionGroup
                XPOComboBox xpoComboBoxCommissionGroup = new XPOComboBox(Entity.Session, typeof(pos_usercommissiongroup), (Entity as sys_userdetail).CommissionGroup, "Designation", null);
                BOWidgetBox boxCommissionGroup = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_commission_group"), xpoComboBoxCommissionGroup);
                vboxTab1.PackStart(boxCommissionGroup, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCommissionGroup, Entity, "CommissionGroup", RegexUtils.RegexGuid, false));

                //DateOfContract
                Entry entryDateOfContract = new Entry();
                BOWidgetBox boxDateOfContract = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_contract_date"), entryDateOfContract);
                vboxTab1.PackStart(boxDateOfContract, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxDateOfContract, Entity, "DateOfContract", RegexUtils.RegexDate, false));

                //PasswordReset: Force Reset Password on Next Login
                _checkButtonPasswordReset = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_reset_password"));
                vboxTab1.PackStart(_checkButtonPasswordReset, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(_checkButtonPasswordReset, Entity, "PasswordReset"));
                if (_dialogMode == DialogMode.Insert || _userDetail.PasswordReset || _isLoggedUser) _checkButtonPasswordReset.Sensitive = false;

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, Entity, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Residence
                Entry entryResidence = new Entry();
                BOWidgetBox boxResidence = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_address"), entryResidence);
                vboxTab2.PackStart(boxResidence, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxResidence, Entity, "Residence", RegexUtils.RegexAlfaNumericExtended, false));

                //Locality
                Entry entryLocality = new Entry();
                BOWidgetBox boxLocality = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_locality"), entryLocality);
                vboxTab2.PackStart(boxLocality, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLocality, Entity, "Locality", RegexUtils.RegexAlfa, false));

                //City
                Entry entryCity = new Entry();
                BOWidgetBox boxCity = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_city"), entryCity);
                vboxTab2.PackStart(boxCity, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxCity, Entity, "City", RegexUtils.RegexAlfa, false));

                //ZipCode
                Entry entryZipCode = new Entry();
                BOWidgetBox boxZipCode = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_postal_code"), entryZipCode);
                vboxTab2.PackStart(boxZipCode, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxZipCode, Entity, "ZipCode", XPOSettings.ConfigurationSystemCountry.RegExZipCode, false));

                //Phone
                Entry entryPhone = new Entry();
                BOWidgetBox boxPhone = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_phone"), entryPhone);
                vboxTab2.PackStart(boxPhone, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPhone, Entity, "Phone", RegexUtils.RegexIntegerGreaterThanZero, false));

                //MobilePhone
                Entry entryMobilePhone = new Entry();
                BOWidgetBox boxMobilePhone = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_mobile_phone"), entryMobilePhone);
                vboxTab2.PackStart(boxMobilePhone, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxMobilePhone, Entity, "MobilePhone", RegexUtils.RegexIntegerGreaterThanZero, false));

                //Email
                Entry entryEmail = new Entry();
                BOWidgetBox boxEmail = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_email_separator"), entryEmail);
                vboxTab2.PackStart(boxEmail, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxEmail, Entity, "Email", RegexUtils.RegexEmail, false));

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_contacts")));
                
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //FiscalNumber
                Entry entryFiscalNumber = new Entry();
                BOWidgetBox boxFiscalNumber = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_fiscal_number"), entryFiscalNumber);
                vboxTab3.PackStart(boxFiscalNumber, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxFiscalNumber, Entity, "FiscalNumber", XPOSettings.ConfigurationSystemCountry.RegExFiscalNumber, false));

                //Language
                Entry entryLanguage = new Entry();
                BOWidgetBox boxLanguage = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_language"), entryLanguage);
                vboxTab3.PackStart(boxLanguage, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxLanguage, Entity, "Language", RegexUtils.RegexAlfa, false));

                //AssignedSeating
                Entry entryAssignedSeating = new Entry();
                BOWidgetBox boxAssignedSeating = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_assigned_seating"), entryAssignedSeating);
                vboxTab3.PackStart(boxAssignedSeating, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAssignedSeating, Entity, "AssignedSeating", RegexUtils.RegexIntegerColonSeparated, false));

                //AccessCardNumber
                Entry entryAccessCardNumber = new Entry();
                BOWidgetBox boxAccessCardNumber = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_access_card_number"), entryAccessCardNumber);
                vboxTab3.PackStart(boxAccessCardNumber, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxAccessCardNumber, Entity, "AccessCardNumber", RegexUtils.RegexIntegerGreaterThanZero, false));

                //BaseConsumption
                Entry entryBaseConsumption = new Entry();
                BOWidgetBox boxBaseConsumption = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_base_consumption"), entryBaseConsumption);
                vboxTab3.PackStart(boxBaseConsumption, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxBaseConsumption, Entity, "BaseConsumption", RegexUtils.RegexIntegerGreaterThanZero, false));

                //BaseOffers
                Entry entryBaseOffers = new Entry();
                BOWidgetBox boxBaseOffers = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_base_offers"), entryBaseOffers);
                vboxTab3.PackStart(boxBaseOffers, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxBaseOffers, Entity, "BaseOffers", RegexUtils.RegexIntegerGreaterThanZero, false));

                //PVPOffers
                Entry entryPVPOffers = new Entry();
                BOWidgetBox boxPVPOffers = new BOWidgetBox(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_price_offers"), entryPVPOffers);
                vboxTab3.PackStart(boxPVPOffers, false, false, 0);
                InputFields.Add(new GenericCRUDWidgetXPO(boxPVPOffers, Entity, "PVPOffers", RegexUtils.RegexIntegerGreaterThanZero, false));

                //Append Tab
                _notebook.AppendPage(vboxTab3, new Label(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_others")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Protect to prevent disabled Current Logged in User
                if (_isLoggedUser) checkButtonDisabled.Sensitive = false;

                //Capture Events
                InputFields.BeforeUpdate += _crudWidgetList_BeforeUpdate;
                InputFields.AfterUpdate += _crudWidgetList_AfterUpdate;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        private void _crudWidgetList_BeforeUpdate(object sender, System.EventArgs e)
        {
            //here we must Catch Widget value not XPGuidObject Value, it appeans Before Update
            //If _currentUserPasswordReset = true dont Reset, if _currentUserPasswordReset = false and we change true ResetPassword
            if (!_currentUserPasswordReset && _checkButtonPasswordReset.Active)
            {
                ResetPassword();
            }
        }

        private void _crudWidgetList_AfterUpdate(object sender, EventArgs e)
        {
            //Detected Change in Logged User Profile
            if (_dialogMode == DialogMode.Update && _isLoggedUser && _currentUserPermissionProfileGuid != _userDetail.Profile.Oid)
            {
                _logger.Debug(string.Format("Detected Change Logged User Profile: [{0}]", _userDetail.Profile.Designation));
                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_information"), CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "dialog_message_user_profile_changed_request_loggeroff"));
            }

            //Detected Change in User Disabled: If user was Disabled, Force Logout User
            if (!_isLoggedUser && _currentUserDisabled && !_userDetail.Disabled)
            {
                GlobalApp.StartupWindow.LogOutUser(false, _userDetail);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Helper Methods

        private void ResetPassword()
        {
            try
            {
                //UserDetail userDetail = (this._crudWidgetList.GetFieldWidget("PasswordReset").DataSourceRow as UserDetail);
                _userDetail.AccessPin = CryptographyUtils.GenerateSaltedString(XPOSettings.DefaultValueUserDetailAccessPin);
                _userDetail.PasswordReset = false;
                _userDetail.PasswordResetDate = XPOUtility.CurrentDateTimeAtomic();
                //Force LogOut User
                GlobalApp.StartupWindow.LogOutUser(false, _userDetail);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
    }
}
