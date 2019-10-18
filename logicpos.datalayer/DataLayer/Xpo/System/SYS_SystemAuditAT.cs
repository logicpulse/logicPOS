using System;
using DevExpress.Xpo;
using logicpos.datalayer.Enums;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systemauditat : XPGuidObject
    {
        public sys_systemauditat() : base() { }
        public sys_systemauditat(Session session) : base(session) { }

        DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        SystemAuditATWSType fType;
        public SystemAuditATWSType Type
        {
            get { return fType; }
            set { SetPropertyValue<SystemAuditATWSType>("Type", ref fType, value); }
        }

        string fPostData;
        [Size(SizeAttribute.Unlimited)]
        public string PostData
        {
            get { return fPostData; }
            set { SetPropertyValue<string>("PostData", ref fPostData, value); }
        }

        //SoapResult
        int fReturnCode;
        public int ReturnCode
        {
            get { return fReturnCode; }
            set { SetPropertyValue<int>("ReturnCode", ref fReturnCode, value); }
        }

        string fReturnMessage;
        public string ReturnMessage
        {
            get { return fReturnMessage; }
            set { SetPropertyValue<string>("ReturnMessage", ref fReturnMessage, value); }
        }

        string fReturnRaw;
        [Size(SizeAttribute.Unlimited)]
        public string ReturnRaw
        {
            get { return fReturnRaw; }
            set { SetPropertyValue<string>("ReturnRaw", ref fReturnRaw, value); }
        }

        string fDocumentNumber;
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        string fATDocCodeID;
        public string ATDocCodeID
        {
            get { return fATDocCodeID; }
            set { SetPropertyValue<string>("ATDocCodeID", ref fATDocCodeID, value); }
        }

        //DocumentFinanceMaster One <> Many SystemAuditATWS
        fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesSystemAuditAT")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}