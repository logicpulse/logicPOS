using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.service.App;
using logicpos.financial.service.Objects.Modules.AT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using System.Security;

namespace logicpos.financial.service.Objects
{
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public class Utils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static void Log(string pMessage)
        {
            Log(pMessage, false);
        }

        public static void Log(string pMessage, bool pEndWithBlankLine)
        {
            _log.Debug(pMessage);
            if (Environment.UserInteractive)
            {
                Console.WriteLine(pMessage);

                if (pEndWithBlankLine)
                {
                    Console.WriteLine();
                }
            }
        }
        
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public static string GetDocumentsQuery(bool pWayBillMode)
        {
            return GetDocumentsQuery(pWayBillMode, Guid.Empty);
        }

        public static string GetDocumentsQuery(bool pWayBillMode, Guid pDocumentMaster)
        {
            //Common : Require to Check if has Records with ReturnCode -3 (Já foi registado um documento idêntico.)
            string where = @"(
                ft.WsAtDocument = 1 AND (fm.ATValidAuditResult IS NULL OR fm.ATResendDocument = 1)  
                AND (SELECT COUNT(*) FROM sys_systemauditat WHERE DocumentMaster = fm.Oid AND ReturnCode = '-3') = 0
            ) AND";

            //Invoices
            //
            //1.4 – Tipo (InvoiceType)
            //Tipo de documento. Pode assumir os seguintes valores:
            //    FT – Fatura;
            //    FS – Fatura Simplificada;
            //    NC –.Nota de Crédito;
            //    ND – Nota de Débito;
            //
            //1.4 – Estado (InvoiceStatus)	
            //Estado de documento. Pode assumir os seguintes valores:
            //    N – Normal;
            //    A – Anulada;
            if (!pWayBillMode)
            {
                //Includes FR SAF-T v1.03
                where += @" (
                    ((ft.Acronym = 'FT' AND ft.WayBill <> 1) OR ft.Acronym = 'FS' OR ft.Acronym = 'NC' OR ft.Acronym = 'ND' OR ft.Acronym = 'FR') 
                    AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'A') 
                    AND ((SELECT COUNT(*) FROM sys_systemauditat WHERE DocumentMaster = fm.Oid AND ReturnCode = '0') = 0 ) 
                )";
                /* IN009083 - Premisses:
                 * - we do not cancel financial documents already received by AT, therefore, do not resend them (COUNT statement)
                 * - "fm.ATResendDocument = 1" is not settled to ft.WayBill <> 1 (non-WayBill documents)
                 */
            }
            //TransportDocuments/WayBill
            //
            //1.8 – Tipo do documento (MovementType)
            //Deve ser preenchido com:
            //    GR – Guia de remessa;
            //    GT – Guia de transporte;
            //    GA – Guia de movimentação de ativos próprios;
            //    GC – Guia de consignação;
            //    GD – Guia ou nota de devolução efetuada pelo cliente.
            //
            //1.6 - Estado atual do documento
            //Estado de documento. Pode assumir os seguintes
            //(MovementStatus)
            //valores:
            //    N – Normal;
            //    T – Por conta de terceiros;
            //    A – Anulada.
            //
            //1.3.4 – Pais (Country)
            //Preencher com <<PT>>.

            //1.12 – Endereço da Empresa Cliente (CustomerAddress)
            //1.12.4 – Pais (Country)
            //Preencher com <<PT>>.

            //1.14 – Local de carga (AddressFrom)
            //1.14.4 – Pais (Country)
            //Preencher com PT.

            //É obrigatório comunicar um documento de transporte à AT cujo destinatário seja um consumidor final?
            //Não. Estão excluídos das obrigações de comunicação os documentos de transporte em que o destinatário ou adquirente seja consumidor final.
            else
            {
                where += string.Format(@" (
                    (ft.Acronym = 'GR' OR ft.Acronym = 'GT' OR ft.Acronym = 'GA' OR ft.Acronym = 'GC' OR ft.Acronym = 'GD')
                    AND (fm.DocumentStatusStatus = 'N' OR fm.DocumentStatusStatus = 'T' OR fm.DocumentStatusStatus = 'A')
                    AND (fm.ShipToCountry = 'PT' AND fm.ShipFromCountry = 'PT')
                    AND (fm.EntityOid <> '{0}')
                )"
                // Skip FinalConsumer
                , SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity
                );
            }

            //Shared: If Has Target Document
            if (pDocumentMaster != Guid.Empty)
            {
                where = string.Format("{0} AND fm.Oid='{1}'", where, pDocumentMaster.ToString());
            }

            //Build Query
            string result = string.Format(@"
                SELECT 
                    fm.Oid AS Oid
                FROM
                    (
                        fin_documentfinancemaster fm
                        LEFT JOIN fin_documentfinancetype ft ON (fm.DocumentType = ft.Oid)
                    )
                WHERE
                    {0}
                ORDER BY 
                    fm.DocumentDate DESC;
                ",
                where
            );

            return result;
        }

        public static void ParseSoapSample()
        {
            //<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
            //  <s:Head>
            //    <h:talkId s:mustknow="1" xmlns:h="urn:schemas-test:testgate:hotel:2012-06">
            //      sfasfasfasfsfsf</h:talkId>
            //    </s:Head>
            //  <s:Body>
            //    <bookHotelResponse xmlns="urn:schemas-test:testgate:hotel:2012-06" xmlns:d="http://someURL" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
            //      <d:bookingReference>123456</d:bookingReference>
            //      <d:bookingStatus>successful</d:bookingStatus>
            //      <d:price xmlns:p="moreURL">
            //        <d:total>105</d:total>
            //      </d:price>
            //    </bookHotelResponse>
            //  </s:Body>
            //</s:Envelope>

            XElement doc = XElement.Load(@"Temp\soapsample.xml");
            XNamespace s = "http://schemas.xmlsoap.org/soap/envelope/";//Envelop namespace s
            XNamespace bhr = "urn:schemas-test:testgate:hotel:2012-06";//bookHotelResponse namespace
            XNamespace d = "http://someURL";//d namespace

            foreach (var itm in doc.Descendants(s + "Body").Descendants(bhr + "bookHotelResponse"))
            {
                //your bookingStatus value
                var elStatus = itm.Element(d + "bookingStatus").Value;
                var elReference = itm.Element(d + "bookingReference").Value;
                var elPrice = itm.Element(d + "price").Value;

                _log.Debug(String.Format("elStatus: [{0}]", elStatus));
                _log.Debug(String.Format("elReference: [{0}]", elReference));
                _log.Debug(String.Format("elPrice: [{0}]", elPrice));

                Console.WriteLine(elStatus);
                Console.WriteLine(elReference);
                Console.WriteLine(elPrice);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Invoices

        /// <summary>
        /// Increase DocumentNumber, used to Fake It, to always be valid
        /// </summary>
        /// <param name="DocumentNumber"></param>
        /// <returns></returns>
        public static string IncreaseDocumentNumber(fin_documentfinancemaster pDocumentMaster)
        {
            string result = string.Empty;

            try
            {
                string documentNumber = pDocumentMaster.DocumentNumber;
                string[] split = documentNumber.Split('/');
                int number = Convert.ToInt16(split[1]) + 1;
                pDocumentMaster.DocumentNumber = string.Format("{0}/{1}", split[0], number);
                pDocumentMaster.Save();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        /// <summary>
        /// Get Document <Line>s Splitted by Tax and <DocumentTotals> Content
        /// 	Linhas do Documento por Taxa (Line)
        /// 	Resumo das linhas da fatura por taxa de imposto, e motivo de isenção ou não liquidação.
        /// 	Deve existir uma, e uma só linha, por cada taxa (TaxType, TaxCountryRegion, TaxCode) e motivo de isenção ou não liquidação (TaxExemptionReason)
        /// </summary>
        /// <param name="DocumentMaster"></param>
        /// <returns></returns>
        public static string GetDocumentContentLinesAndDocumentTotals(fin_documentfinancemaster pDocumentMaster)
        {
            _log.Debug($"string GetDocumentContentLinesAndDocumentTotals(fin_documentfinancemaster pDocumentMaster) :: {pDocumentMaster.DocumentNumber}");

            //Init Locals Vars
            string result = string.Empty;
            decimal taxPayable = 0.0m;
            decimal netTotal = 0.0m;
            decimal grossTotal = 0.0m;
            //Prepare Node Name
            string nodeName = (pDocumentMaster.DocumentType.Credit) ? "CreditAmount" : "DebitAmount";

            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    fdVat AS Vat,
	                    cvTaxType AS TaxType,
	                    cvTaxCode AS TaxCode,
	                    cvTaxCountryRegion AS TaxCountryRegion,
	                    SUM(fdTotalNet) AS TotalNet,
	                    SUM(fdTotalGross) AS TotalGross,
                        SUM(fdTotalDiscount) AS TotalDiscount,
	                    SUM(fdTotalTax) AS TotalTax,
	                    SUM(fdTotalFinal) AS TotalFinal,
	                    cxAcronym AS VatExemptionReasonAcronym
                    FROM
	                    view_documentfinance
                    WHERE 
	                    fmOid = '{0}'
                    GROUP BY
	                    fdVat,cvTaxType,cvTaxCode,cvTaxCountryRegion,cxAcronym
                    ORDER BY
	                    fdVat, VatExemptionReasonAcronym
                    ;"
                    , pDocumentMaster.Oid
                );

                DataTable dtResult = FrameworkUtils.GetDataTableFromQuery(sql);

                //Init StringBuilder
                StringBuilder sb = new StringBuilder();

                foreach (DataRow item in dtResult.Rows)
                {
                    string taxExemptionReason = (!string.IsNullOrEmpty(item["VatExemptionReasonAcronym"].ToString()))
                        ? string.Format("{0}      <ns2:TaxExemptionReason>{1}</ns2:TaxExemptionReason>", Environment.NewLine, item["VatExemptionReasonAcronym"])
                        : string.Empty
                    ;

                    sb.Append(string.Format(@"    <Line>
      <ns2:{0}>{1}</ns2:{0}>
      <ns2:Tax>
        <ns2:TaxType>{2}</ns2:TaxType>
        <ns2:TaxCountryRegion>{3}</ns2:TaxCountryRegion>
        <ns2:TaxPercentage>{4}</ns2:TaxPercentage>
      </ns2:Tax>{5}
    </Line>
"
                        , nodeName
                        , FrameworkUtils.DecimalToString(Convert.ToDecimal(item["TotalNet"]), GlobalFramework.CurrentCultureNumberFormat)
                        , item["TaxType"]
                        , item["TaxCountryRegion"]
                        , FrameworkUtils.DecimalToString(Convert.ToDecimal(item["Vat"]), GlobalFramework.CurrentCultureNumberFormat)
                        , taxExemptionReason
                    ));

                    //Sum DocumentTotals
                    taxPayable += Convert.ToDecimal(item["TotalTax"]);
                    netTotal += Convert.ToDecimal(item["TotalNet"]);
                    //Is TotalFinal not db TotalGross
                    grossTotal += Convert.ToDecimal(item["TotalFinal"]);
                }

                //Add DocumentTotals
                sb.Append(string.Format(@"    <DocumentTotals>
      <ns2:TaxPayable>{0}</ns2:TaxPayable>
      <ns2:NetTotal>{1}</ns2:NetTotal>
      <ns2:GrossTotal>{2}</ns2:GrossTotal>
    </DocumentTotals>"
                    , FrameworkUtils.DecimalToString(taxPayable, GlobalFramework.CurrentCultureNumberFormat)
                    , FrameworkUtils.DecimalToString(netTotal, GlobalFramework.CurrentCultureNumberFormat)
                    , FrameworkUtils.DecimalToString(grossTotal, GlobalFramework.CurrentCultureNumberFormat)
                ));

                result = sb.ToString();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //WayBill

        /// <summary>
        /// Get Document <Line>s
        /// 	Linhas do Documento(Line)
        /// </summary>
        /// <param name="DocumentMaster"></param>
        /// <returns></returns>
        public static string GetDocumentWayBillContentLines(fin_documentfinancemaster pDocumentMaster)
        {
            /* IN007016 - escaping the following list of chars using "System.Security.SecurityElement.Escape()" method:
             * 
             *      "   &quot;
             *      '   &apos;
             *      <   &lt;
             *      >   &gt;
             *      &   &amp;
             */
            _log.Debug($"string GetDocumentWayBillContentLines(fin_documentfinancemaster pDocumentMaster) :: {pDocumentMaster.DocumentNumber}");

            //Init Locals Vars
            string result = string.Empty;

            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    Designation AS ProductDescription, Quantity, UnitMeasure AS UnitOfMeasure, Price AS UnitPrice,
	                    (SELECT rf.OriginatingON FROM fin_documentfinancedetailorderreference AS rf WHERE rf.DocumentDetail = fd.Oid) AS OrderReferences
                    FROM 
	                    fin_documentfinancedetail AS fd 
                    WHERE 
	                    DocumentMaster = '{0}' 
                    ORDER BY 
	                    Ord
                    ;"
                    , pDocumentMaster.Oid
                );

                DataTable dtResult = FrameworkUtils.GetDataTableFromQuery(sql);

                //Init StringBuilder
                StringBuilder sb = new StringBuilder();

                foreach (DataRow item in dtResult.Rows)
                {
                    string orderReferences = (!string.IsNullOrEmpty(item["OrderReferences"].ToString()))
                        ? string.Format(@"{0}      <OrderReferences>
        <OriginatingON>{1}</OriginatingON>
      </OrderReferences>", Environment.NewLine, item["OrderReferences"])
                        : string.Empty
                    ;

                    sb.Append(string.Format(@"    <Line>{0}
      <ProductDescription>{1}</ProductDescription>
      <Quantity>{2}</Quantity>
      <UnitOfMeasure>{3}</UnitOfMeasure>
      <UnitPrice>{4}</UnitPrice>
    </Line>
"
                        , orderReferences
                        , SecurityElement.Escape(item["ProductDescription"].ToString())
                        , FrameworkUtils.DecimalToString(Convert.ToDecimal(item["Quantity"]), GlobalFramework.CurrentCultureNumberFormat)
                        , item["UnitOfMeasure"]
                        , FrameworkUtils.DecimalToString(Convert.ToDecimal(item["UnitPrice"]), GlobalFramework.CurrentCultureNumberFormat)
                    ));
                }

                result = sb.ToString();
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        //If can launch command, to register urlacl is because VS is not running in Admin Mode, launch ConsoleApplicationServer.exe with Admin to Register urlacl
        //Required else error occurs when runnig without admin privileges ex inside VS or Running Console exe
        //An unhandled exception of type 'System.ServiceModel.AddressAccessDeniedException' occurred in System.ServiceModel.dll
        //Additional information: O HTTP não conseguiu registar o URL http://+:8733/hello/. O processo não tem direitos de acesso a este espaço de nomes (consulte http://go.microsoft.com/fwlink/?LinkId=70353 para obter detalhes).
        public static void ModifyHttpSettings()
        {
            //The SID "S-1-1-0" is a wellknown SID and stands for the "Everyone" account. 
            //The SID is the same for all localizations of windows. The method Translate of SecurityIdentifier class returns the localized name of the Everyone account.
            string everyone = new System.Security.Principal.SecurityIdentifier(
                "S-1-1-0").Translate(typeof(System.Security.Principal.NTAccount)).ToString();

            string parameter = string.Format(@"http add urlacl url=http://+:{0}/ user=\{1}", Program.ServicePort, everyone);

            _log.Debug(string.Format("ModifyHttpSettings: 'netsh {0}'", parameter));
            ProcessStartInfo psi = new ProcessStartInfo("netsh", parameter);

            psi.Verb = "runas";
            psi.RedirectStandardOutput = false;
            psi.CreateNoWindow = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Timer Service 

        public static Dictionary<fin_documentfinancemaster, ServicesATSoapResult> ServiceSendPendentDocuments()
        {
            _log.Debug("Dictionary<fin_documentfinancemaster, ServicesATSoapResult> Utils.ServiceSendPendentDocuments()");
            Dictionary<fin_documentfinancemaster, ServicesATSoapResult> result = new Dictionary<fin_documentfinancemaster, ServicesATSoapResult>();
            Guid key = Guid.Empty;
            fin_documentfinancemaster documentMaster = null;
            ServicesATSoapResult soapResult = new ServicesATSoapResult();

            /* IN009083 - #TODO apply same validation for when sending documents to AT */
            try
            {
                //Invoice Documents
                if (SettingsApp.ServiceATSendDocuments)
                {
                    string sqlDocuments = GetDocumentsQuery(false);
                    //_log.Debug(String.Format("sqlDocuments: [{0}]", FrameworkUtils.RemoveCarriageReturnAndExtraWhiteSpaces(sqlDocuments)));

                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sqlDocuments);
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        key = new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString());
                        documentMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), key);
                        //SendDocument
                        soapResult = SendDocument(documentMaster);
                        
                        //Helper to Detect Documents
                        //Detect if Document is Already in AT System / -3 = "Já foi registado um documento idêntico."
                        //200 = Detect The operation has timed out | The remote name could not be resolved: 'servicos.portaldasfinancas.gov.pt'
                        //if (soapResult.ReturnCode.Equals("-3"))
                        //{
                        //    _log.Debug("BREAK");
                        //}

                        result.Add(documentMaster, soapResult);
                    }
                }

                //WayBill Documents
                if (SettingsApp.ServiceATSendDocumentsWayBill)
                {
                    string sqlDocumentsWayBill = GetDocumentsQuery(true);
                    //_log.Debug(String.Format("sqlDocumentsWayBill: [{0}]", FrameworkUtils.RemoveCarriageReturnAndExtraWhiteSpaces(sqlDocumentsWayBill)));

                    XPSelectData xPSelectData = FrameworkUtils.GetSelectedDataFromQuery(sqlDocumentsWayBill);
                    foreach (SelectStatementResultRow row in xPSelectData.Data)
                    {
                        key = new Guid(row.Values[xPSelectData.GetFieldIndex("Oid")].ToString());
                        documentMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), key);
                        //SendDocument
                        soapResult = SendDocument(documentMaster);
                        result.Add(documentMaster, soapResult);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                Console.Write($"Error: [{ex.Message}]");
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Send Document : Used from Tests and From Timer Service

        public static ServicesATSoapResult SendDocument(fin_documentfinancemaster documentMaster)
        {
            //Send Document
            ServicesAT sendDocument = new ServicesAT(documentMaster);
            //Get Result from SendDocument Object
            string resultSend = sendDocument.Send();
            //Post
            ServicesATSoapResult soapResult = sendDocument.SoapResult;

            //Show Result
            //Log(string.Format("SendDocument DocumentMaster: [{0}]", documentMaster.DocumentNumber));
            //Log(string.Format("SendDocument ResultRaw: [{0}]", resultSend));
            //if (sendDocument.SoapResult != null) {
            //    Log(string.Format(@"{0}ResultCode: [{1}]{0}ResultMessage: [{2}]"
            //        , Environment.NewLine
            //        , sendDocument.SoapResult.ReturnCode
            //        , sendDocument.SoapResult.ReturnMessage
            //    ));
            //    //WB
            //    if (!string.IsNullOrEmpty(sendDocument.SoapResult.ATDocCodeID))
            //    {
            //        Log(string.Format(@"ATDocCodeID: [{0}]", sendDocument.SoapResult.ATDocCodeID));
            //    }
            //}

            return soapResult;
        }
    }
}
