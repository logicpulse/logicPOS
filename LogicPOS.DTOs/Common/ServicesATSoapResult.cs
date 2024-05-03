using System.Runtime.Serialization;

namespace LogicPOS.DTOs.Common
{
    [DataContract]
    public class ServicesATSoapResult
    {
        [DataMember]
        public string ReturnRaw { get; set; } = string.Empty;
       

        [DataMember]
        public string ReturnCode { get; set; } = string.Empty;
       
        [DataMember]
        public string ReturnMessage { get; set; } = string.Empty;
        

        [DataMember]
        public string DocumentNumber { get; set; }


        [DataMember]
        public string ATDocCodeID { get; set; }
       

        public ServicesATSoapResult() { }

        public ServicesATSoapResult(string pReturnCode, string pReturnMessage)
            : this(pReturnCode, pReturnMessage, string.Empty)
        {
        }

        public ServicesATSoapResult(string pReturnCode, string pReturnMessage, string pReturnRaw)
            : this(pReturnCode, pReturnMessage, pReturnRaw, string.Empty, string.Empty)
        {
        }

        public ServicesATSoapResult(string pReturnCode, string pReturnMessage, string pReturnRaw, string pDocumentNumber, string pATDocCodeID)
        {
            //DC & WB
            ReturnCode = pReturnCode;
            ReturnMessage = pReturnMessage;
            ReturnRaw = pReturnRaw;

            //WB
            DocumentNumber = pDocumentNumber;
            ATDocCodeID = pATDocCodeID;
        }
    }
}
