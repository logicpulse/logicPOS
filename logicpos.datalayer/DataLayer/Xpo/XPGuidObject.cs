using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.App;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [NonPersistent]
    [DeferredDeletion(false)]
    public abstract class XPGuidObject : XPCustomObject
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool debug = false;

        public XPGuidObject() : base() { }
        public XPGuidObject(Session pSession) : base(pSession) { }

        [Persistent("Oid"), Indexed(Unique = true), Key(true), MemberDesignTimeVisibility(false)]
        private Guid _Oid = Guid.Empty;

        [PersistentAlias("_Oid")]
        public Guid Oid { get { return _Oid; } }

        private bool _isNewRecord = false;

        private bool _isEncrypted = false;
        [NonPersistentAttribute]
        public bool IsEncrypted { get => _isEncrypted; set => _isEncrypted = value; }

        // Assigned by Childs and Store Properties References
        protected Dictionary<string, PropertyInfo> _encryptedAttributes;

        Boolean fDisabled;
        public Boolean Disabled
        {
            get { return fDisabled; }
            set { SetPropertyValue<Boolean>("Disabled", ref fDisabled, value); }
        }

        String fNotes;
        [Size(SizeAttribute.Unlimited)]
        public String Notes
        {
            get { return fNotes; }
            set { SetPropertyValue<String>("Notes", ref fNotes, value); }
        }

        DateTime fCreatedAt;
        public DateTime CreatedAt
        {
            get { return fCreatedAt; }
            set { SetPropertyValue<DateTime>("CreatedAt", ref fCreatedAt, value); }
        }

        sys_userdetail fCreatedBy;
        public sys_userdetail CreatedBy
        {
            get { return fCreatedBy; }
            set { SetPropertyValue<sys_userdetail>("CreatedBy", ref fCreatedBy, value); }
        }

        pos_configurationplaceterminal fCreatedWhere;
        public pos_configurationplaceterminal CreatedWhere
        {
            get { return fCreatedWhere; }
            set { SetPropertyValue<pos_configurationplaceterminal>("CreatedWhere", ref fCreatedWhere, value); }
        }

        DateTime fUpdatedAt;
        public DateTime UpdatedAt
        {
            get { return fUpdatedAt; }
            set { SetPropertyValue<DateTime>("UpdatedAt", ref fUpdatedAt, value); }
        }

        sys_userdetail fUpdatedBy;
        public sys_userdetail UpdatedBy
        {
            get { return fUpdatedBy; }
            set { SetPropertyValue<sys_userdetail>("UpdatedBy", ref fUpdatedBy, value); }
        }

        pos_configurationplaceterminal fUpdatedWhere;
        public pos_configurationplaceterminal UpdatedWhere
        {
            get { return fUpdatedWhere; }
            set { SetPropertyValue<pos_configurationplaceterminal>("UpdatedWhere", ref fUpdatedWhere, value); }
        }

        DateTime fDeletedAt;
        public DateTime DeletedAt
        {
            get { return fDeletedAt; }
            set { SetPropertyValue<DateTime>("DeletedAt", ref fDeletedAt, value); }
        }

        sys_userdetail fDeletedBy;
        public sys_userdetail DeletedBy
        {
            get { return fDeletedBy; }
            set { SetPropertyValue<sys_userdetail>("DeletedBy", ref fDeletedBy, value); }
        }

        pos_configurationplaceterminal fDeletedWhere;
        public pos_configurationplaceterminal DeletedWhere
        {
            get { return fDeletedWhere; }
            set { SetPropertyValue<pos_configurationplaceterminal>("DeletedWhere", ref fDeletedWhere, value); }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Override Methods 

        protected override void OnSaving()
        {
            base.OnSaving();

            if (!(Session is NestedUnitOfWork) && Session.IsNewObject(this))
            {
                _Oid = XpoDefault.NewGuid();
            }

            //Global Updates
            UpdatedAt = FrameworkUtils.CurrentDateTimeAtomic();
            if (GlobalFramework.LoggedUser != null)
            {
                UpdatedBy = this.Session.GetObjectByKey<sys_userdetail>(GlobalFramework.LoggedUser.Oid);
            }
            if (GlobalFramework.LoggedTerminal != null)
            {
                UpdatedWhere = this.Session.GetObjectByKey<pos_configurationplaceterminal>(GlobalFramework.LoggedTerminal.Oid);
            }

            if (_isNewRecord)
            {
                //Global Updates
                CreatedAt = FrameworkUtils.CurrentDateTimeAtomic();
                if (GlobalFramework.LoggedUser != null)
                {
                    CreatedBy = this.Session.GetObjectByKey<sys_userdetail>(GlobalFramework.LoggedUser.Oid);
                }
                if (GlobalFramework.LoggedTerminal != null)
                {
                    CreatedWhere = this.Session.GetObjectByKey<pos_configurationplaceterminal>(GlobalFramework.LoggedTerminal.Oid);
                }
                // Call EncryptProperties to be used when we create Objects outside BO, 
                // this will trigger Encrypted Automatically
                EncryptProperties();
                // Now we can Procced with Save with Encrypted Proporties
                OnNewRecordSaving();
            }
            else
            {
                OnRecordSaving();
            }
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            Disabled = false;
            DateTime dateTime = FrameworkUtils.CurrentDateTimeAtomic();
            CreatedAt = dateTime;
            UpdatedAt = dateTime;

            if (GlobalFramework.LoggedUser != null)
            {
                UpdatedBy = this.Session.GetObjectByKey<sys_userdetail>(GlobalFramework.LoggedUser.Oid);
                // This Prevent : DevExpress.Xpo.DB.Exceptions.LockingException: Cannot persist the object. It was modified or deleted (purged) by another application.
                // Created to prevent Creating Year Series problems
                UpdatedBy.Reload();
            }

            if (GlobalFramework.LoggedTerminal != null)
            {
                UpdatedWhere = this.Session.GetObjectByKey<pos_configurationplaceterminal>(GlobalFramework.LoggedTerminal.Oid);
                // This Prevent : DevExpress.Xpo.DB.Exceptions.LockingException: Cannot persist the object. It was modified or deleted (purged) by another application.
                // Created to prevent Creating Year Series problems
                UpdatedWhere.Reload();
            }

            // The Trick to Catch New Records is compare it with Guid.Empty
            _isNewRecord = (this.Oid == Guid.Empty);//true

            OnAfterConstruction();
        }

        //To be Override by SubClasses
        protected virtual void OnAfterConstruction()
        {
        }

        protected virtual void OnNewRecordSaving()
        {
            // This Only Occurs on BO Edits, and Not in Object Code Creation, else it Trigger Double Encryption
            if (!_isEncrypted && _encryptedAttributes != null && _encryptedAttributes.Count > 0)
            {
                // Call SharedEncryptedDecryptProperties
                EncryptProperties();
            }
        }

        protected virtual void OnRecordSaving()
        {
            if (!_isEncrypted && _encryptedAttributes != null && _encryptedAttributes.Count > 0)
            {
                // Call SharedEncryptedDecryptProperties
                // This will ReEncrypt "item.Key.EncryptProperties();" in genericcrudwidgetlistxpo.cs
                // EncryptProperties
                EncryptProperties();
            }
        }

        protected override void OnLoaded()
        //protected override void AfterLoad()
        {
            // Occurs on New Record, Refresh Tree etc

            if (_encryptedAttributes != null && _encryptedAttributes.Count > 0)
            {
                // Call SharedEncryptedDecryptProperties
                if (debug) _log.Debug($"OnLoaded: [{this.GetType().Name}]");
                // DecryptProperties
                DecryptProperties();
            }
        }

        protected override void OnSaved()
        {
            // This will do the Trick to Decrypt InMemory Values after Save Decrypted Values, and Will Show UI Decrypted Strings
            if (_isEncrypted && _encryptedAttributes != null && _encryptedAttributes.Count > 0)
            {
                // DecryptProperties
                DecryptProperties();
            }
        }

        // Other events that may be Usefull
        //protected override void OnChanged(string propertyName, object oldValue, object newValue)
        //{
        //    if (debug) _log.Debug($"OnChanged: [{this.GetType().Name}], propertyName: [{propertyName}]");
        //
        //    if (_encryptedAttributes != null && _encryptedAttributes.Count > 0)
        //    {
        //        // This will do the Trick to Decrypt InMemory Values after Save Decrypted Values, and Will Show UI Decrypted Strings
        //        // Using OptimisticLockFieldInDataLayer
        //
        //        // Call SharedEncryptedDecryptProperties
        //        if (propertyName.Equals("OptimisticLockFieldInDataLayer") && newValue.GetType() == typeof(string))
        //        {
        //            _log.Debug($"OnChanged: [{this.GetType().Name}]");
        //            SharedEncryptedDecryptProperties(false);
        //        }
        //    }
        //}

        // Other events that may be Usefull
        //protected override void FireChangedByXPPropertyDescriptor(string memberName)
        //{
        //}

        // Other events that may be Usefull
        //protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        //{
        //}

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Encrypted/ Decrypt Stuff

        // Encrypted Static Helper Method
        protected void InitEncryptedAttributes<T>()
        {
            _encryptedAttributes = new Dictionary<string, PropertyInfo>();
            object referenceValue;

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(typeof(XPGuidObjectAttribute), true);

                foreach (object attr in attrs)
                {
                    // Get XPGuidObjectAttributes
                    XPGuidObjectAttribute authAttr = attr as XPGuidObjectAttribute;
                    if (authAttr != null)
                    {
                        string propName = prop.Name;
                        bool useEncrypted = authAttr.Encrypted;
                        // Add to Dictionary
                        if (useEncrypted)
                        {
                            // Add to encryptedAttributes if Used
                            _encryptedAttributes.Add(propName, prop);

                            // Get Reference 
                            referenceValue = prop.GetValue(this, null);

                            if (referenceValue != null)
                            {
                                if (debug) _log.Debug(string.Format("Added Property: [{0}], PropertyValue: [{1}]", propName, referenceValue));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Public Encrypt Method, using shared method SharedEncryptedDecryptProperties
        /// </summary>
        public void EncryptProperties()
        {
            SharedEncryptedDecryptProperties(true);
        }

        /// <summary>
        /// Public Dencrypt Method, using shared method SharedEncryptedDecryptProperties
        /// </summary>
        public void DecryptProperties()
        {
            SharedEncryptedDecryptProperties(false);
        }

        /// <summary>
        /// Shared Method for Encrypt/Decrypt all encryptedAttributes Model Properties
        /// </summary>
        /// <param name="encrypt">Encrypt = True, Decrypt = false</param>
        private void SharedEncryptedDecryptProperties(bool encrypt)
        {
            string sourcePropertValue = string.Empty;
            object targetPropertValue = null;
            string modeString = (encrypt) ? "Encrypting" : "Decrypting";
            Type propertyType;

            // If has Model has encryptedAttributes and has valid PluginSoftwareVendor to Encrypt
            if (_encryptedAttributes != null && SettingsApp.PluginSoftwareVendor != null)
            {
                foreach (var attr in _encryptedAttributes)
                {
                    // Must Check if property and value Exists / Non Null
                    if (_encryptedAttributes[attr.Key] != null && _encryptedAttributes[attr.Key].GetValue(this, null) != null)
                    {
                        // Get value from Property (Plain or Encrypted value)
                        sourcePropertValue = _encryptedAttributes[attr.Key].GetValue(this, null).ToString();
                        // Get Type
                        propertyType = _encryptedAttributes[attr.Key].GetValue(this, null).GetType();

                        // Check if Value is Null or Empty and is type of String (We need a string field to Assign Encrypted Values)
                        if (!string.IsNullOrEmpty(sourcePropertValue) && propertyType == typeof(string))
                        {
                            try
                            {
                                // Encrypted
                                if (encrypt)
                                {
                                    // Encrypt Property Value
                                    targetPropertValue = SettingsApp.PluginSoftwareVendor.Encrypt(sourcePropertValue);
                                }
                                else
                                {
                                    // DeEncrypt Property Value
                                    targetPropertValue = SettingsApp.PluginSoftwareVendor.Decrypt(sourcePropertValue);
                                }

                                // Set Value value to PropertyInfo
                                _encryptedAttributes[attr.Key].SetValue(this, targetPropertValue);

                                // Change Object Property 
                                _isEncrypted = encrypt;

                                // Show Log
                                if (debug) _log.Debug(string.Format("{0} Property type :[{1}]: [{2}], value: [{3}] to [{4}]", modeString, propertyType, attr.Key, sourcePropertValue, targetPropertValue));
                            }
                            catch (Exception ex)
                            {
                                _log.Debug(ex.Message, ex);
                            }
                        }
                    }
                }
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Static Methods

        /// <summary>
        /// Get Object Attributes
        /// </summary>
        /// <param name="type"></param>
        /// <param name="forceLowerCaseKeys">Used with selectStatementResultMeta that has lowercase field names used in Selects, or fieldnames used in QUERY, this ways we guarantee that we can always use lowercase for detection</param>
        /// <param name="onlyEncrypted"></param>
        /// <returns></returns>
        public static Dictionary<string, PropertyInfo> GetXPGuidObjectAttributes(Type type, bool forceLowerCaseKeys, bool onlyEncrypted = true)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            Dictionary<string, PropertyInfo> result = new Dictionary<string, PropertyInfo>();
            string key = string.Empty;

            try
            {
                PropertyInfo[] props = type.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    object[] attribute = prop.GetCustomAttributes(typeof(XPGuidObjectAttribute), true);
                    // If Detect Attributes add It
                    if (attribute.Length > 0)
                    {
                        // Add Only Encrypted properties
                        if ((onlyEncrypted && (attribute[0] as XPGuidObjectAttribute).Encrypted) || !onlyEncrypted)
                        {
                            key = (forceLowerCaseKeys) ? prop.Name.ToLower() : prop.Name;
                            result.Add(key, prop);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Helper to Descrypt SelectStatementResultRows
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selectStatementResultMeta"></param>
        /// <param name="selectStatementResultData"></param>
        /// <returns></returns>
        public static SelectStatementResultRow[] DecryptSelectStatementResults(Type type, SelectStatementResultRow[] selectStatementResultMeta, SelectStatementResultRow[] selectStatementResultData)
        {
            return DecryptSelectStatementResults(type, selectStatementResultMeta, selectStatementResultData, null);
        }

        public static SelectStatementResultRow[] DecryptSelectStatementResults(Type type, SelectStatementResultRow[] selectStatementResultMeta, SelectStatementResultRow[] selectStatementResultData, string[] nonPropertyFields)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            SelectStatementResultRow[] result = selectStatementResultData;
            // Get Encrypted GetXPGuidObjectAttributes
            Dictionary<string, PropertyInfo> attributes = GetXPGuidObjectAttributes(type, true);
            string columnName = string.Empty;
            string columnValue = string.Empty;
            string columnValueDecrypted = string.Empty;

            try
            {
                // Loop Rows
                foreach (var row in selectStatementResultData)
                {
                    // Reset Column
                    int i = -1;

                    foreach (var column in selectStatementResultMeta)
                    {
                        i++;

                        columnName = column.Values[0].ToString();

                        // Detected Encrypted Field, or property that exists on nonPropertyFields, ex "label" from "....AS label"
                        if (attributes.ContainsKey(columnName) || (nonPropertyFields != null && nonPropertyFields.Any(x => x == columnName)))
                        {
                            if ((row.Values[i] != null))
                            {
                                columnValue = row.Values[i].ToString();
                                columnValueDecrypted = SettingsApp.PluginSoftwareVendor.Decrypt(columnValue);
                                if (debug) log.Debug($"Detected Encrypted Column ColumName: [{columnName}], ColumnValue: [{columnValue}], ColumnValueDecrypted: [{columnValueDecrypted}]");
                                // Replace Original Value
                                row.Values[i] = columnValueDecrypted;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }

        public static object DecryptIfNeeded(object source)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            object result = source;

            try
            {
                // Decrypt Properties
                if (SettingsApp.PluginSoftwareVendor != null && source != null)
                {
                    result = SettingsApp.PluginSoftwareVendor.Decrypt(source.ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
