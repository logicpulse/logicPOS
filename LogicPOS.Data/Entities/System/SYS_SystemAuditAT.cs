using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class sys_systemauditat : XPGuidObject
    {
        public sys_systemauditat() : base() { }
        public sys_systemauditat(Session session) : base(session) { }

        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        private SystemAuditATWSType fType;
        public SystemAuditATWSType Type
        {
            get { return fType; }
            set { SetPropertyValue("Type", ref fType, value); }
        }

        private string fPostData;
        [Size(SizeAttribute.Unlimited)]
        public string PostData
        {
            get { return fPostData; }
            set { SetPropertyValue<string>("PostData", ref fPostData, value); }
        }

        //SoapResult
        private int fReturnCode;
        public int ReturnCode
        {
            get { return fReturnCode; }
            set { SetPropertyValue<int>("ReturnCode", ref fReturnCode, value); }
        }

        private string fReturnMessage;
        public string ReturnMessage
        {
            get { return fReturnMessage; }
            set { SetPropertyValue<string>("ReturnMessage", ref fReturnMessage, value); }
        }

        private string fReturnRaw;
        [Size(SizeAttribute.Unlimited)]
        public string ReturnRaw
        {
            get { return fReturnRaw; }
            set { SetPropertyValue<string>("ReturnRaw", ref fReturnRaw, value); }
        }

        private string fDocumentNumber;
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        private string fATDocCodeID;
        public string ATDocCodeID
        {
            get { return fATDocCodeID; }
            set { SetPropertyValue<string>("ATDocCodeID", ref fATDocCodeID, value); }
        }

        //DocumentFinanceMaster One <> Many SystemAuditATWS
        private fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesSystemAuditAT")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue("DocumentMaster", ref fDocumentMaster, value); }
        }
    }
}