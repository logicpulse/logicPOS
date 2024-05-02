﻿using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.Widgets.BackOffice;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using System;
using logicpos.Classes.Enums.Dialogs;
using logicpos.shared.App;
using logicpos.datalayer.App;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class DialogUserDetail : BOBaseDialog
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

        public DialogUserDetail(Window pSourceWindow, GenericTreeViewXPO pTreeView, DialogFlags pFlags, DialogMode pDialogMode, XPGuidObject pXPGuidObject)
            : base(pSourceWindow, pTreeView, pFlags, pDialogMode, pXPGuidObject)
        {
            this.Title = logicpos.Utils.GetWindowTitle(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "window_title_edit_user"));
            SetSizeRequest(500, 469);

            //Store References
            _userDetail = (DataSourceRow as sys_userdetail);
            _isLoggedUser = (_userDetail == DataLayerFramework.LoggedUser);
            
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
                BOWidgetBox boxLabel = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_order"), entryOrd);
                vboxTab1.PackStart(boxLabel, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLabel, _dataSourceRow, "Ord", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Code
                Entry entryCode = new Entry();
                BOWidgetBox boxCode = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_code"), entryCode);
                vboxTab1.PackStart(boxCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCode, _dataSourceRow, "Code", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, true));

                //Name
                Entry entryName = new Entry();
                BOWidgetBox boxName = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_name"), entryName);
                vboxTab1.PackStart(boxName, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxName, _dataSourceRow, "Name", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, true));

                //Profile
                XPOComboBox xpoComboBoxProfile = new XPOComboBox(DataSourceRow.Session, typeof(sys_userprofile), (DataSourceRow as sys_userdetail).Profile, "Designation", null);
                BOWidgetBox boxProfile = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_user_profiles"), xpoComboBoxProfile);
                vboxTab1.PackStart(boxProfile, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxProfile, DataSourceRow, "Profile", LogicPOS.Utility.RegexUtils.RegexGuid, true));

                //CommissionGroup
                XPOComboBox xpoComboBoxCommissionGroup = new XPOComboBox(DataSourceRow.Session, typeof(pos_usercommissiongroup), (DataSourceRow as sys_userdetail).CommissionGroup, "Designation", null);
                BOWidgetBox boxCommissionGroup = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_commission_group"), xpoComboBoxCommissionGroup);
                vboxTab1.PackStart(boxCommissionGroup, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCommissionGroup, DataSourceRow, "CommissionGroup", LogicPOS.Utility.RegexUtils.RegexGuid, false));

                //DateOfContract
                Entry entryDateOfContract = new Entry();
                BOWidgetBox boxDateOfContract = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_contract_date"), entryDateOfContract);
                vboxTab1.PackStart(boxDateOfContract, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxDateOfContract, _dataSourceRow, "DateOfContract", LogicPOS.Utility.RegexUtils.RegexDate, false));

                //PasswordReset: Force Reset Password on Next Login
                _checkButtonPasswordReset = new CheckButton(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_reset_password"));
                vboxTab1.PackStart(_checkButtonPasswordReset, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(_checkButtonPasswordReset, _dataSourceRow, "PasswordReset"));
                if (_dialogMode == DialogMode.Insert || _userDetail.PasswordReset || _isLoggedUser) _checkButtonPasswordReset.Sensitive = false;

                //Disabled
                CheckButton checkButtonDisabled = new CheckButton(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_disabled"));
                if (_dialogMode == DialogMode.Insert) checkButtonDisabled.Active = POSSettings.BOXPOObjectsStartDisabled;
                vboxTab1.PackStart(checkButtonDisabled, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(checkButtonDisabled, _dataSourceRow, "Disabled"));

                //Append Tab
                _notebook.AppendPage(vboxTab1, new Label(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_record_main_detail")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab2 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //Residence
                Entry entryResidence = new Entry();
                BOWidgetBox boxResidence = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_address"), entryResidence);
                vboxTab2.PackStart(boxResidence, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxResidence, _dataSourceRow, "Residence", LogicPOS.Utility.RegexUtils.RegexAlfaNumericExtended, false));

                //Locality
                Entry entryLocality = new Entry();
                BOWidgetBox boxLocality = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_locality"), entryLocality);
                vboxTab2.PackStart(boxLocality, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLocality, _dataSourceRow, "Locality", LogicPOS.Utility.RegexUtils.RegexAlfa, false));

                //City
                Entry entryCity = new Entry();
                BOWidgetBox boxCity = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_city"), entryCity);
                vboxTab2.PackStart(boxCity, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxCity, _dataSourceRow, "City", LogicPOS.Utility.RegexUtils.RegexAlfa, false));

                //ZipCode
                Entry entryZipCode = new Entry();
                BOWidgetBox boxZipCode = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_postal_code"), entryZipCode);
                vboxTab2.PackStart(boxZipCode, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxZipCode, _dataSourceRow, "ZipCode", DataLayerSettings.ConfigurationSystemCountry.RegExZipCode, false));

                //Phone
                Entry entryPhone = new Entry();
                BOWidgetBox boxPhone = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_phone"), entryPhone);
                vboxTab2.PackStart(boxPhone, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPhone, _dataSourceRow, "Phone", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //MobilePhone
                Entry entryMobilePhone = new Entry();
                BOWidgetBox boxMobilePhone = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_mobile_phone"), entryMobilePhone);
                vboxTab2.PackStart(boxMobilePhone, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxMobilePhone, _dataSourceRow, "MobilePhone", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //Email
                Entry entryEmail = new Entry();
                BOWidgetBox boxEmail = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_email_separator"), entryEmail);
                vboxTab2.PackStart(boxEmail, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxEmail, _dataSourceRow, "Email", LogicPOS.Utility.RegexUtils.RegexEmail, false));

                //Append Tab
                _notebook.AppendPage(vboxTab2, new Label(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_contacts")));
                
                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Tab2
                VBox vboxTab3 = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

                //FiscalNumber
                Entry entryFiscalNumber = new Entry();
                BOWidgetBox boxFiscalNumber = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_fiscal_number"), entryFiscalNumber);
                vboxTab3.PackStart(boxFiscalNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxFiscalNumber, _dataSourceRow, "FiscalNumber", DataLayerSettings.ConfigurationSystemCountry.RegExFiscalNumber, false));

                //Language
                Entry entryLanguage = new Entry();
                BOWidgetBox boxLanguage = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_language"), entryLanguage);
                vboxTab3.PackStart(boxLanguage, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxLanguage, _dataSourceRow, "Language", LogicPOS.Utility.RegexUtils.RegexAlfa, false));

                //AssignedSeating
                Entry entryAssignedSeating = new Entry();
                BOWidgetBox boxAssignedSeating = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_assigned_seating"), entryAssignedSeating);
                vboxTab3.PackStart(boxAssignedSeating, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAssignedSeating, _dataSourceRow, "AssignedSeating", LogicPOS.Utility.RegexUtils.RegexIntegerColonSeparated, false));

                //AccessCardNumber
                Entry entryAccessCardNumber = new Entry();
                BOWidgetBox boxAccessCardNumber = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_access_card_number"), entryAccessCardNumber);
                vboxTab3.PackStart(boxAccessCardNumber, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxAccessCardNumber, _dataSourceRow, "AccessCardNumber", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //BaseConsumption
                Entry entryBaseConsumption = new Entry();
                BOWidgetBox boxBaseConsumption = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_base_consumption"), entryBaseConsumption);
                vboxTab3.PackStart(boxBaseConsumption, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBaseConsumption, _dataSourceRow, "BaseConsumption", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //BaseOffers
                Entry entryBaseOffers = new Entry();
                BOWidgetBox boxBaseOffers = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_base_offers"), entryBaseOffers);
                vboxTab3.PackStart(boxBaseOffers, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxBaseOffers, _dataSourceRow, "BaseOffers", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //PVPOffers
                Entry entryPVPOffers = new Entry();
                BOWidgetBox boxPVPOffers = new BOWidgetBox(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_price_offers"), entryPVPOffers);
                vboxTab3.PackStart(boxPVPOffers, false, false, 0);
                _crudWidgetList.Add(new GenericCRUDWidgetXPO(boxPVPOffers, _dataSourceRow, "PVPOffers", LogicPOS.Utility.RegexUtils.RegexIntegerGreaterThanZero, false));

                //Append Tab
                _notebook.AppendPage(vboxTab3, new Label(resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_others")));

                //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                //Protect to prevent disabled Current Logged in User
                if (_isLoggedUser) checkButtonDisabled.Sensitive = false;

                //Capture Events
                _crudWidgetList.BeforeUpdate += _crudWidgetList_BeforeUpdate;
                _crudWidgetList.AfterUpdate += _crudWidgetList_AfterUpdate;
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
                logicpos.Utils.ShowMessageTouch(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_information"), resources.CustomResources.GetCustomResource(DataLayerFramework.Settings["customCultureResourceDefinition"], "dialog_message_user_profile_changed_request_loggeroff"));
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
                _userDetail.AccessPin = CryptographyUtils.SaltedString.GenerateSaltedString(DataLayerSettings.DefaultValueUserDetailAccessPin);
                _userDetail.PasswordReset = false;
                _userDetail.PasswordResetDate = DataLayerUtils.CurrentDateTimeAtomic();
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
