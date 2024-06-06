using LogicPOS.Reporting.Common;
using System;

/* Report Fields
SELECT 
	sauDate, sauDescription, usdCode, usdName, cptCode, cptDesignation
FROM 
	view_systemaudit
;*/

namespace LogicPOS.Reporting.Reports.System
{
    [Report(Entity = "view_systemaudit")]
    internal class SystemAuditViewReport : ReportData
    {
        // SystemAudit
        [Report(Field = "sauOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //sauOid AS Oid,

        [Report(Field = "sauDate")]
        public DateTime SystemAuditDate { get; set; }                       //sauDate AS SystemAuditDate,

        [Report(Field = "sauDateDay")]
        public string SystemAuditDateDay { get; set; }                      //sauDateDay AS SystemAuditDate,

        [Report(Field = "sauDescription")]
        public string SystemAuditDescription { get; set; }                  //sauDescription AS SystemAuditDateDay,

        // SystemAuditType
        [Report(Field = "satOid")]
        public string SystemAuditType { get; set; }                         //satOid AS SystemAuditType,

        [Report(Field = "satOrd")]
        public uint SystemAuditTypeOrd { get; set; }                        //satOrd AS SystemAuditTypeOrd,

        [Report(Field = "satCode")]
        public uint SystemAuditTypeCode { get; set; }                       //satCode AS SystemAuditTypeCode,

        [Report(Field = "satToken")]
        public string SystemAuditTypeToken { get; set; }                    //satToken AS SystemAuditTypeToken,

        [Report(Field = "satDesignation")]
        public string SystemAuditTypeDesignation { get; set; }              //satDesignation AS SystemAuditTypeDesignation,

        [Report(Field = "satResourceString")]
        public string SystemAuditTypeResourceString { get; set; }           //satResourceString AS SystemAuditTypeResourceString,

        // UserDetail
        [Report(Field = "usdOid")]
        public string UserDetail { get; set; }                              //usdOid AS UserDetail,

        [Report(Field = "usdOrd")]
        public uint UserDetailOrd { get; set; }                             //usdOrd AS UserDetailOrd,

        [Report(Field = "usdCode")]
        public uint UserDetailCode { get; set; }                            //usdCode AS UserDetailCode,

        [Report(Field = "usdName")]
        public string UserDetailName { get; set; }                          //usdName AS UserDetailName,

        // ConfigurationPlaceTerminal
        [Report(Field = "cptOid")]
        public string ConfigurationPlaceTerminal { get; set; }              //cptOid AS ConfigurationPlaceTerminal,

        [Report(Field = "cptOrd")]
        public uint ConfigurationPlaceTerminalOrd { get; set; }             //cptOrd AS ConfigurationPlaceTerminalOrd,

        [Report(Field = "cptCode")]
        public uint ConfigurationPlaceTerminalCode { get; set; }            //cptCode AS ConfigurationPlaceTerminalCode,

        [Report(Field = "cptDesignation")]
        public string ConfigurationPlaceTerminalDesignation { get; set; }   //cptDesignation AS ConfigurationPlaceTerminalDesignation
    }
}
