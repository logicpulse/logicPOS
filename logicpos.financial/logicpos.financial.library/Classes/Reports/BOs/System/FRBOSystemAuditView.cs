using System;

/* Report Fields
SELECT 
	sauDate, sauDescription, usdCode, usdName, cptCode, cptDesignation
FROM 
	view_systemaudit
;*/

namespace logicpos.financial.library.Classes.Reports.BOs.System
{
    [FRBO(Entity = "view_systemaudit")]
    class FRBOSystemAuditView : FRBOBaseObject
    {
        // SystemAudit
        [FRBO(Field = "sauOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //sauOid AS Oid,

        [FRBO(Field = "sauDate")]
        public DateTime SystemAuditDate { get; set; }                       //sauDate AS SystemAuditDate,

        [FRBO(Field = "sauDateDay")]
        public string SystemAuditDateDay { get; set; }                      //sauDateDay AS SystemAuditDate,

        [FRBO(Field = "sauDescription")]
        public string SystemAuditDescription { get; set; }                  //sauDescription AS SystemAuditDateDay,
        
        // SystemAuditType
        [FRBO(Field = "satOid")]
        public string SystemAuditType { get; set; }                         //satOid AS SystemAuditType,
        
        [FRBO(Field = "satOrd")]
        public uint SystemAuditTypeOrd { get; set; }                        //satOrd AS SystemAuditTypeOrd,
        
        [FRBO(Field = "satCode")]
        public uint SystemAuditTypeCode { get; set; }                       //satCode AS SystemAuditTypeCode,
        
        [FRBO(Field = "satToken")]
        public string SystemAuditTypeToken { get; set; }                    //satToken AS SystemAuditTypeToken,
        
        [FRBO(Field = "satDesignation")]
        public string SystemAuditTypeDesignation { get; set; }              //satDesignation AS SystemAuditTypeDesignation,
        
        [FRBO(Field = "satResourceString")]
        public string SystemAuditTypeResourceString { get; set; }           //satResourceString AS SystemAuditTypeResourceString,
        
        // UserDetail
        [FRBO(Field = "usdOid")]
        public string UserDetail { get; set; }                              //usdOid AS UserDetail,
        
        [FRBO(Field = "usdOrd")]
        public uint UserDetailOrd { get; set; }                             //usdOrd AS UserDetailOrd,
        
        [FRBO(Field = "usdCode")]
        public uint UserDetailCode { get; set; }                            //usdCode AS UserDetailCode,
        
        [FRBO(Field = "usdName")]
        public string UserDetailName { get; set; }                          //usdName AS UserDetailName,
        
        // ConfigurationPlaceTerminal
        [FRBO(Field = "cptOid")]
        public string ConfigurationPlaceTerminal { get; set; }              //cptOid AS ConfigurationPlaceTerminal,
        
        [FRBO(Field = "cptOrd")]
        public uint ConfigurationPlaceTerminalOrd { get; set; }             //cptOrd AS ConfigurationPlaceTerminalOrd,
        
        [FRBO(Field = "cptCode")]
        public uint ConfigurationPlaceTerminalCode { get; set; }            //cptCode AS ConfigurationPlaceTerminalCode,
        
        [FRBO(Field = "cptDesignation")]
        public string ConfigurationPlaceTerminalDesignation { get; set; }   //cptDesignation AS ConfigurationPlaceTerminalDesignation
    }
}
