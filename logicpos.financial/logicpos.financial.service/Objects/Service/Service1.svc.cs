using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.service.App;
using logicpos.financial.service.Objects;
using logicpos.financial.service.Objects.Modules.AT;
using logicpos.shared;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace logicpos.financial.servicewcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        //Log4Net
        protected log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //AT WebServices

        public ServicesATSoapResult SendDocument(Guid pDocumentMaster)
        {
            ServicesATSoapResult result = null;

            try
            {
                fin_documentfinancemaster documentMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), pDocumentMaster);

                if (documentMaster != null)
                {
                    //Send Document
                    ServicesAT servicesAT = new ServicesAT(documentMaster);
                    //Get Result from SendDocument Object
                    string resultSend = servicesAT.Send();
                    //Get SoapResult
                    result = servicesAT.SoapResult;

                    if (
                        //Error: Não foi possível resolver o nome remoto: 'servicos.portaldasfinancas.gov.pt'
                        result == null 
                        //Error: <faultcode>33</faultcode>
                        ||
                        result != null && string.IsNullOrEmpty(result.ReturnCode)
                        )
                    {
                        result = new ServicesATSoapResult("200", resultSend);
                        servicesAT.PersistResult(result);
                        Utils.Log(string.Format("Error {0}: [{1}]", result.ReturnCode, result.ReturnMessage));
                    }
                    else
                    {
                        //Output in ServiceAT With Log, here is optional
                        //Utils.Log(string.Format("SendDocument Result: [{0}]:[{1}]:[{2}]", result.ReturnCode, result.ReturnMessage, result.ReturnRaw));
                    }
                }
                else
                {
                    //All messages are in PT, from ATWS, dont required translation here
                    string errorMsg = string.Format("Documento Inválido: {0}", pDocumentMaster);
                    result = new ServicesATSoapResult("202", errorMsg);
                    Utils.Log(string.Format("Error {0}: [{1}]", result.ReturnCode, result.ReturnMessage));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                if (Environment.UserInteractive) { Utils.Log(ex.Message); }
                //Send Error Message : 210 is All Exceptions Errors
                result = new ServicesATSoapResult("210", ex.Message);
            }

            //Dont Send null ServicesATSoapResult here, else triggers erros outside
            return result;
        }
    }
}
