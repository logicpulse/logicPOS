using System;
using System.Runtime.Serialization;

namespace logicpos.financial.service.Objects.Modules.AT
{
    [DataContract]
    public class ServicesATSoapResult
    {
        protected string _returnRaw = string.Empty;
        [DataMember]
        public string ReturnRaw
        {
            get { return _returnRaw; }
            set { _returnRaw = value; }
        }
        protected string _returnCode = string.Empty;
        [DataMember]
        public string ReturnCode
        {
            get { return _returnCode; }
            set { _returnCode = value; }
        }
        protected string _returnMessage = string.Empty;
        [DataMember]
        public string ReturnMessage
        {
            get { return _returnMessage; }
            set { _returnMessage = value; }
        }
        private string _documentNumber;
        [DataMember]
        public string DocumentNumber
        {
            get { return _documentNumber; }
            set { _documentNumber = value; }
        }
        private string _aTDocCodeID;
        [DataMember]
        public string ATDocCodeID
        {
            get { return _aTDocCodeID; }
            set { _aTDocCodeID = value; }
        }

        //Parameterless Constructor
        public ServicesATSoapResult() { }

        //Overload
        public ServicesATSoapResult(string pReturnCode, string pReturnMessage)
            : this(pReturnCode, pReturnMessage, string.Empty)
        {
        }

        //Overload
        public ServicesATSoapResult(string pReturnCode, string pReturnMessage, string pReturnRaw)
            : this(pReturnCode, pReturnMessage, pReturnRaw, string.Empty, string.Empty)
        {
        }

        //Base Constructor
        public ServicesATSoapResult(string pReturnCode, string pReturnMessage, string pReturnRaw, string pDocumentNumber, string pATDocCodeID)
        {
            //DC & WB
            _returnCode = pReturnCode;
            _returnMessage = pReturnMessage;
            _returnRaw = pReturnRaw;
            //WB
            _documentNumber = pDocumentNumber;
            _aTDocCodeID = pATDocCodeID;
        }
    }
}
