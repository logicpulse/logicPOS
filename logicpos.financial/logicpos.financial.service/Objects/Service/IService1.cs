using logicpos.financial.service.Objects.Modules.AT;
using LogicPOS.DTOs.Common;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace logicpos.financial.servicewcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
        [OperationContract]
        [WebInvoke(BodyStyle=WebMessageBodyStyle.Bare, ResponseFormat=WebMessageFormat.Json)]
        ServicesATSoapResult SendDocument(Guid pDocumentMaster);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        private string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue { get; set; } = true;

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
