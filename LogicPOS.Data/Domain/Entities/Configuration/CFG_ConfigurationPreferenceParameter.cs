using DevExpress.Xpo;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class cfg_configurationpreferenceparameter : Entity
    {
        public cfg_configurationpreferenceparameter() : base() { }
        public cfg_configurationpreferenceparameter(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(cfg_configurationpreferenceparameter), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(cfg_configurationpreferenceparameter), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        private string fValue;
        //[Size(255)]
        [Size(SizeAttribute.Unlimited)]
        public string Value
        {
            get { return fValue; }
            set { SetPropertyValue<string>("Value", ref fValue, value); }
        }

        private string fValueTip;
        public string ValueTip
        {
            get { return fValueTip; }
            set { SetPropertyValue<string>("ValueTip", ref fValueTip, value); }
        }

        private bool fRequired;
        public bool Required
        {
            get { return fRequired; }
            set { SetPropertyValue<bool>("Required", ref fRequired, value); }
        }

        private string fRegEx;
        [Size(255)]
        public string RegEx
        {
            get { return fRegEx; }
            set { SetPropertyValue<string>("RegEx", ref fRegEx, value); }
        }

        private string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        private string fResourceStringInfo;
        [Size(255)]
        public string ResourceStringInfo
        {
            get { return fResourceStringInfo; }
            set { SetPropertyValue<string>("ResourceStringInfo", ref fResourceStringInfo, value); }
        }

        private int fFormType;
        public int FormType
        {
            get { return fFormType; }
            set { SetPropertyValue<int>("FormType", ref fFormType, value); }
        }

        private int fFormPageNo;
        public int FormPageNo
        {
            get { return fFormPageNo; }
            set { SetPropertyValue<int>("FormPageNo", ref fFormPageNo, value); }
        }

        private PreferenceParameterInputType fInputType;
        public PreferenceParameterInputType InputType
        {
            get { return fInputType; }
            set { SetPropertyValue("InputType", ref fInputType, value); }
        }
        //IN:009268 Use Euro VAT Info 
        public static object GetCountryCode2
        {
            get
            {
                string sql = "SELECT Value FROM cfg_configurationpreferenceparameter WHERE (Disabled IS NULL OR Disabled  <> 1) AND (Token = 'COMPANY_COUNTRY_CODE2')";
                var getCountryCode2 = XPOSettings.Session.ExecuteScalar(sql);
                return getCountryCode2;
            }
        }//IN:009268 ENDS
    }
}