using LogicPOS.Reporting.Data.Common;
using System;

/* Report Fields
SELECT 
	sauDate, sauDescription, usdCode, usdName, cptCode, cptDesignation
FROM 
	view_systemaudit
;*/

namespace LogicPOS.Reporting.Reports.System
{
    [ReportData(Entity = "view_systemaudit")]
    internal class SystemAuditViewReport : ReportData
    {
        // SystemAudit
        [ReportData(Field = "sauOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //sauOid AS Oid,

        [ReportData(Field = "sauDate")]
        public DateTime SystemAuditDate { get; set; }                       //sauDate AS SystemAuditDate,

        [ReportData(Field = "sauDateDay")]
        public string SystemAuditDateDay { get; set; }                      //sauDateDay AS SystemAuditDate,

        [ReportData(Field = "sauDescription")]
        public string SystemAuditDescription { get; set; }                  //sauDescription AS SystemAuditDateDay,

        // SystemAuditType
        [ReportData(Field = "satOid")]
        public string SystemAuditType { get; set; }                         //satOid AS SystemAuditType,

        [ReportData(Field = "satOrd")]
        public uint SystemAuditTypeOrd { get; set; }                        //satOrd AS SystemAuditTypeOrd,

        [ReportData(Field = "satCode")]
        public uint SystemAuditTypeCode { get; set; }                       //satCode AS SystemAuditTypeCode,

        [ReportData(Field = "satToken")]
        public string SystemAuditTypeToken { get; set; }                    //satToken AS SystemAuditTypeToken,

        [ReportData(Field = "satDesignation")]
        public string SystemAuditTypeDesignation { get; set; }              //satDesignation AS SystemAuditTypeDesignation,

        [ReportData(Field = "satResourceString")]
        public string SystemAuditTypeResourceString { get; set; }           //satResourceString AS SystemAuditTypeResourceString,

        // UserDetail
        [ReportData(Field = "usdOid")]
        public string UserDetail { get; set; }                              //usdOid AS UserDetail,

        [ReportData(Field = "usdOrd")]
        public uint UserDetailOrd { get; set; }                             //usdOrd AS UserDetailOrd,

        [ReportData(Field = "usdCode")]
        public uint UserDetailCode { get; set; }                            //usdCode AS UserDetailCode,

        [ReportData(Field = "usdName")]
        public string UserDetailName { get; set; }                          //usdName AS UserDetailName,

        // ConfigurationPlaceTerminal
        [ReportData(Field = "cptOid")]
        public string ConfigurationPlaceTerminal { get; set; }              //cptOid AS ConfigurationPlaceTerminal,

        [ReportData(Field = "cptOrd")]
        public uint ConfigurationPlaceTerminalOrd { get; set; }             //cptOrd AS ConfigurationPlaceTerminalOrd,

        [ReportData(Field = "cptCode")]
        public uint ConfigurationPlaceTerminalCode { get; set; }            //cptCode AS ConfigurationPlaceTerminalCode,

        [ReportData(Field = "cptDesignation")]
        public string ConfigurationPlaceTerminalDesignation { get; set; }   //cptDesignation AS ConfigurationPlaceTerminalDesignation
    }
}
