using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class CFG_ConfigurationPreferenceParameter : XPGuidObject
    {
        public CFG_ConfigurationPreferenceParameter() : base() { }
        public CFG_ConfigurationPreferenceParameter(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(CFG_ConfigurationPreferenceParameter), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(CFG_ConfigurationPreferenceParameter), "Code");
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

        string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public String Token
        {
            get { return fToken; }
            set { SetPropertyValue<String>("Token", ref fToken, value); }
        }

        string fValue;
        //[Size(255)]
        [Size(SizeAttribute.Unlimited)]
        public String Value
        {
            get { return fValue; }
            set { SetPropertyValue<String>("Value", ref fValue, value); }
        }

        string fValueTip;
        public string ValueTip
        {
            get { return fValueTip; }
            set { SetPropertyValue<string>("ValueTip", ref fValueTip, value); }
        }

        Boolean fRequired;
        public Boolean Required
        {
            get { return fRequired; }
            set { SetPropertyValue<Boolean>("Required", ref fRequired, value); }
        }

        string fRegEx;
        [Size(255)]
        public string RegEx
        {
            get { return fRegEx; }
            set { SetPropertyValue<string>("RegEx", ref fRegEx, value); }
        }

        string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        string fResourceStringInfo;
        [Size(255)]
        public String ResourceStringInfo
        {
            get { return fResourceStringInfo; }
            set { SetPropertyValue<String>("ResourceStringInfo", ref fResourceStringInfo, value); }
        }
        int fFormType;
        public int FormType
        {
            get { return fFormType; }
            set { SetPropertyValue<int>("FormType", ref fFormType, value); }
        }

        int fFormPageNo;
        public int FormPageNo
        {
            get { return fFormPageNo; }
            set { SetPropertyValue<int>("FormPageNo", ref fFormPageNo, value); }
        }

        PreferenceParameterInputType fInputType;
        public PreferenceParameterInputType InputType
        {
            get { return fInputType; }
            set { SetPropertyValue<PreferenceParameterInputType>("InputType", ref fInputType, value); }
        }
    }
}