/* Report Fields
SELECT
	DateDay,UserOid,UserOrd,UserName,ArticleOid,Code,Designation,UnitMeasure,Price,Vat,Discount,
	SUM(Quantity) AS Quantity, 
	Price * SUM(Quantity) AS Total,
	CommissionValue, 
	SUM(TotalCommission) AS TotalCommission
FROM 
	view_usercommission
WHERE 
	DateDay = '2018-08-30'
GROUP BY 
	DateDay,UserOid,UserOrd,UserName,ArticleOid,Code,Designation,UnitMeasure,Price,Vat,Discount,CommissionValue
ORDER BY 
	DateDay, UserOrd, CommissionValue
;*/

namespace logicpos.financial.library.Classes.Reports.BOs.Users
{
    [FRBO(Entity = "view_usercommission",
        Group = "DateDay,UserOid,UserOrd,UserCode,UserName,ArticleOid,Code,Designation,UnitMeasure,Price,Vat,Discount,CommissionValue",
        Order = "DateDay,UserName,Designation")]
    class FRBOUserCommission : FRBOBaseObject
    {
        [FRBO(Field = "UserOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }            //UserOid AS Oid,

        public string DateDay { get; set; }
        public uint UserOrd { get; set; }
        public uint UserCode { get; set; }
        public string UserName { get; set; }
        public string ArticleOid { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public string UnitMeasure { get; set; }
        public decimal Price { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }

        [FRBO(Field = "SUM(Quantity)")]
        public decimal Quantity { get; set; }

        [FRBO(Field = "Price * SUM(Quantity)")]
        public decimal Total { get; set; }

        public decimal CommissionValue { get; set; }

        [FRBO(Field = "SUM(TotalCommission)")]
        public decimal TotalCommission { get; set; }
    }
}
