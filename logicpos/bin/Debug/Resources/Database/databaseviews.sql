CREATE VIEW view_article AS
	SELECT 
		art.Oid AS artOid,
		art.Disabled AS artDisabled,
		art.Ord AS artOrd,
		art.Code AS artCode,
		art.CodeDealer AS artCodeDealer,
		art.Designation AS artDesignation,
		art.Price1 AS artPrice1,
		art.Price2 AS artPrice2,
		art.Price3 AS artPrice3,
		art.Price4 AS artPrice4,
		art.Price5 AS artPrice5,
		art.Price1Promotion AS artPrice1Promotion,
		art.Price2Promotion AS artPrice2Promotion,
		art.Price3Promotion AS artPrice3Promotion,
		art.Price4Promotion AS artPrice4Promotion,
		art.Price5Promotion AS artPrice5Promotion,
		art.Price1UsePromotionPrice AS artPrice1UsePromotionPrice,
		art.Price2UsePromotionPrice AS artPrice2UsePromotionPrice,
		art.Price3UsePromotionPrice AS artPrice3UsePromotionPrice,
		art.Price4UsePromotionPrice AS artPrice4UsePromotionPrice,
		art.Price5UsePromotionPrice AS artPrice5UsePromotionPrice,
		art.PriceWithVat AS artPriceWithVAT,
		art.Discount AS artDiscount,
		art.PVPVariable AS artPVPVariable,
		art.Favorite AS artFavorite,
		acl.Oid AS aclOid,
		acl.Designation AS aclDesignation,
		acl.WorkInStock AS aclWorkInStock,
		cum.Designation AS cumDesignation,
		cum.Acronym AS cumAcronym,
		vot.Oid AS votOid,
		vot.Value AS votValue,
		vot.Designation AS votDesignation,
		vds.Oid AS vdsOid,
		vds.Value AS vdsValue,
		vds.Designation AS vdsDesignation,
		cer.Oid AS cerOid,
		cer.Designation AS cerDesignation
	FROM
		(((((fin_article art
		left join fin_articleclass acl ON ((art.Class = acl.Oid)))
		left join fin_configurationvatrate vot ON ((art.VatOnTable = vot.Oid)))
		left join fin_configurationvatrate vds ON ((art.VatOnTable = vds.Oid)))
		left join fin_configurationvatexemptionreason cer ON ((art.VatExemptionReason = cer.Oid)))
		left join cfg_configurationunitmeasure cum ON ((art.UnitMeasure = cum.Oid)))
;

CREATE VIEW view_articlecommision AS
    SELECT 
        art.Oid AS artOid,
        art.Disabled AS artDisabled,
        art.Ord AS artOrd,
        art.Code AS artCode,
        art.Designation AS artDesignation,
        art.Price1 AS artPrice1,
        art.Discount AS artDiscount,
        art.CommissionGroup AS artCommissionGroup,
        cga.Designation AS cgaDesignation,
        cga.Commission AS cgaCommission,
        afm.Oid AS afmOid,
        afm.Ord AS afmOrd,
        afm.Code AS afmCode,
        afm.CommissionGroup AS afmCommissionGroup,
        afm.Designation AS afmDesignation,
        cgf.Designation AS cgfDesignation,
        cgf.Commission AS cgfCommission,
        asf.Oid AS asfOid,
        asf.Ord AS asfOrd,
        asf.Code AS asfCode,
        asf.CommissionGroup AS asfCommissionGroup,
        asf.Designation AS asfDesignation,
        cgs.Designation AS cgsDesignation,
        cgs.Commission AS cgsCommission,
        vot.Oid AS votOid,
        vot.Value AS votValue,
        vot.Designation AS votDesignation,
        vds.Oid AS vdsOid,
        vds.Value AS vdsValue,
        vds.Designation AS vdsDesignation
    FROM
        (((((((fin_article art
        LEFT JOIN fin_articlefamily afm ON ((art.Family = afm.Oid)))
        LEFT JOIN fin_articlesubfamily asf ON ((art.SubFamily = asf.Oid)))
        LEFT JOIN fin_configurationvatrate vot ON ((art.VatOnTable = vot.Oid)))
        LEFT JOIN fin_configurationvatrate vds ON ((art.VatOnTable = vds.Oid)))
        LEFT JOIN pos_usercommissiongroup cga ON ((art.CommissionGroup = cga.Oid)))
        LEFT JOIN pos_usercommissiongroup cgf ON ((afm.CommissionGroup = cgf.Oid)))
        LEFT JOIN pos_usercommissiongroup cgs ON ((asf.CommissionGroup = cgs.Oid)))
;

CREATE VIEW view_articlestock AS
	SELECT
		afa.Oid AS afaOid,
		afa.Ord AS afaOrd,
		afa.Code AS afaCode,
		afa.Designation AS afaDesignation,
		afa.Disabled AS afaDisabled,
		asf.Oid AS asfOid,
		asf.Ord AS asfOrd,
		asf.Code AS asfCode,
		asf.Designation AS asfDesignation,
		asf.Disabled AS asfDisabled,
		art.Oid AS artOid,
		art.Ord AS artOrd,
		art.Code AS artCode,
		art.CodeDealer AS CodeDealer,
		art.Designation AS artDesignation,
		art.Disabled AS artDisabled,
		stk.Oid AS stkOid,
		stk.Date AS stkDate,
		stk.DocumentNumber AS stkDocumentNumber,
		stk.Quantity AS stkQuantity,
		aum.Designation AS aumDesignation
	FROM
		(fin_articlestock stk
		left join fin_article art ON (art.Oid = stk.Article)
		left join fin_articlefamily afa ON (afa.Oid = art.Family)
		left join fin_articlesubfamily asf ON (asf.Oid = art.SubFamily)
		left join cfg_configurationunitmeasure aum ON (aum.Oid = art.UnitMeasure))
;

CREATE VIEW view_country AS
	SELECT 
		cou.Oid AS couOid,
		cou.Ord AS couOrd,
		cou.Code AS couCode,
		cou.Designation AS couDesignation,
		cou.Currency AS couCurrency,
		cou.CurrencyCode AS couCurrencyCode,
		cur.Oid AS curOid,
		cur.Ord AS curOrd,
		cur.Code AS curCode,
		cur.Designation AS curToken,
		cur.Acronym AS curAcronym,
		cur.Symbol AS curSymbol,
		cur.Entity AS curEntity,
		cur.ExchangeRate AS curExchangeRate
	FROM
		(cfg_configurationcountry cou
		left join cfg_configurationcurrency cur ON (cur.Acronym = cou.CurrencyCode)
	)
;

CREATE VIEW view_documentfinance AS
	SELECT 
		ft.Oid AS ftOid,
		ft.Code AS ftDocumentTypeCode,
		ft.Designation AS ftDocumentTypeDesignation,
		ft.ResourceString AS ftDocumentTypeResourceString,
		ft.Acronym AS ftDocumentTypeAcronym,
		ft.ResourceStringReport AS ftDocumentTypeResourceStringReport,
		ft.Credit AS ftCredit,
		ft.WayBill AS ftWayBill,
		ft.SaftAuditFile AS ftSaftAuditFile,
		ft.SaftDocumentType AS ftSaftDocumentType,
		fm.Oid AS fmOid,
		fm.Date AS fmDate,
		fm.DocumentNumber AS fmDocumentNumber,
		fm.DocumentDate AS fmDocumentDate,
		fm.SystemEntryDate AS fmSystemEntryDate,
		fm.TransactionID AS fmTransactionID,
		fm.ShipToDeliveryID AS fmShipToDeliveryID,
		fm.ShipToDeliveryDate AS fmShipToDeliveryDate,
		fm.ShipToWarehouseID AS fmShipToWarehouseID,
		fm.ShipToLocationID AS fmShipToLocationID,
		fm.ShipToAddressDetail AS fmShipToAddressDetail,
		fm.ShipToCity AS fmShipToCity,
		fm.ShipToPostalCode AS fmShipToPostalCode,
		fm.ShipToRegion AS fmShipToRegion,
		fm.ShipToCountry AS fmShipToCountry,
		fm.ShipFromDeliveryID AS fmShipFromDeliveryID,
		fm.ShipFromDeliveryDate AS fmShipFromDeliveryDate,
		fm.ShipFromWarehouseID AS fmShipFromWarehouseID,
		fm.ShipFromLocationID AS fmShipFromLocationID,
		fm.ShipFromAddressDetail AS fmShipFromAddressDetail,
		fm.ShipFromCity AS fmShipFromCity,
		fm.ShipFromPostalCode AS fmShipFromPostalCode,
		fm.ShipFromRegion AS fmShipFromRegion,
		fm.ShipFromCountry AS fmShipFromCountry,
		fm.MovementStartTime AS fmMovementStartTime,
		fm.MovementEndTime AS fmMovementEndTime,
		fm.ATDocCodeID AS fmATDocCodeID,
		fm.DocumentCreatorUser AS fmDocumentCreatorUser,
		fm.DocumentStatusStatus AS fmDocumentStatusStatus,
		fm.Payed AS fmPayed,
		fm.PayedDate AS fmPayedDate,
		fm.TotalNet AS fmTotalNet,
		fm.TotalGross AS fmTotalGross,
		fm.TotalDiscount AS fmTotalDiscount,
		fm.TotalTax AS fmTotalTax,
		fm.TotalFinal AS fmTotalFinal,
		fm.TotalFinalRound AS fmTotalFinalRound,
		fm.TotalDelivery AS fmTotalDelivery,
		fm.TotalChange AS fmTotalChange,
		fm.Discount AS fmDiscount,
		fm.DiscountFinancial AS fmDiscountFinancial,
		fm.ExchangeRate AS fmExchangeRate,
		fm.EntityOid AS fmEntity,
		fm.EntityInternalCode AS fmEntityInternalCode,
		cu.Code AS cuEntityCode,
		cu.Hidden AS cuEntityHidden,
		fm.EntityName AS fmEntityName,
		fm.EntityAddress AS fmEntityAddress,
		fm.EntityZipCode AS fmEntityZipCode,
		fm.EntityCity AS fmEntityCity,
		fm.EntityCountry AS fmEntityCountryCode2,
		cc.Designation AS ccCountryDesignation,
		fm.EntityFiscalNumber AS fmEntityFiscalNumber,
		fm.Notes AS fmNotes,
		fm.SourceOrderMain AS fmSourceOrderMain,
		fm.DocumentParent AS fmDocumentParent,
		fm.DocumentType AS fmDocumentType,
		fm.DocumentSerie AS fmDocumentSerie,
		pm.Oid AS fmPaymentMethod,
		pm.Code AS fmPaymentMethodCode,
		pm.Designation AS pmPaymentMethodDesignation,
		pm.Token AS pmPaymentMethodToken,
		fm.PaymentCondition AS fmPaymentCondition,
		pc.Code AS pcPaymentConditionCode,
		pc.Designation AS pcPaymentConditionDesignation,
		pc.Acronym AS pcPaymentConditionAcronym,
		fm.Currency AS fmCurrency,
		cr.Designation AS crCurrencyDesignation,
		cr.Acronym AS crCurrencyAcronym,
		af.Oid AS afFamily,
		af.Code AS afFamilyCode,
		af.Designation AS afFamilyDesignation,
		sf.Oid AS sfSubFamily,
		sf.Code AS sfSubFamilyCode,
		sf.Designation AS sfSubFamilyDesignation,
		fd.Oid AS fdOid,
		fd.Article AS fdArticle,
		fd.Ord AS fdOrd,
		fd.Code AS fdCode,
		fd.Designation AS fdDesignation,
		fd.Quantity AS fdQuantity,
		fd.UnitMeasure AS fdUnitMeasure,
		fd.Price AS fdPrice,
		(fd.Price - ((fd.Price * fd.Discount) / 100)) AS fdPriceWithDiscount,
		fd.Vat AS fdVat,
		fd.Discount AS fdDiscount,
		ar.PriceWithVAT AS arPriceWithVat,
		fd.TotalNet AS fdTotalNet,
		fd.TotalGross AS fdTotalGross,
		fd.TotalDiscount AS fdTotalDiscount,
		fd.TotalTax AS fdTotalTax,
		fd.TotalFinal AS fdTotalFinal,
		fd.VatExemptionReason AS fdVatExemptionReason,
		fd.VatExemptionReasonDesignation AS fdVatExemptionReasonDesignation,
		cx.Acronym AS cxAcronym,
		fd.Token1 AS fdToken1,
		fd.Token2 AS fdToken2,
		cv.Code AS cfVatCode,
		cv.Designation AS cfVatDesignation,
		cv.TaxType AS cvTaxType,
		cv.TaxCode AS cvTaxCode,
		cv.TaxCountryRegion AS cvTaxCountryRegion,
		cv.TaxExpirationDate AS cvTaxExpirationDate,
		cv.TaxDescription AS cvTaxDescription,
		ud.Oid AS udUserDetail,
		ud.Code AS udUserDetailCode,
		ud.Name AS udUserDetailName,
		dm.PlaceTable AS dmPlaceTable,
		ct.Designation AS ctPlaceTableDesignation
	FROM
		(((((((((((((((fin_documentfinancemaster fm
		left join fin_documentfinancedetail fd ON ((fm.Oid = fd.DocumentMaster)))
		left join fin_documentfinancetype ft ON ((fm.DocumentType = ft.Oid)))
		left join fin_article ar ON ((ar.Oid = fd.Article)))
		left join fin_articlefamily af ON ((af.Oid = ar.Family)))
		left join fin_articlesubfamily sf ON ((sf.Oid = ar.SubFamily)))
		left join fin_configurationvatrate cv ON ((cv.Oid = fd.VatRate)))
		left join fin_configurationvatexemptionreason cx ON ((cx.Oid = fd.VatExemptionReason)))
		left join fin_documentordermain dm ON ((fm.SourceOrderMain = dm.Oid)))
		left join pos_configurationplacetable ct ON ((dm.PlaceTable = ct.Oid)))
		left join fin_configurationpaymentmethod pm ON ((fm.PaymentMethod = pm.Oid)))
		left join fin_configurationpaymentcondition pc ON ((pc.Oid = fm.PaymentCondition)))
		left join cfg_configurationcountry cc ON ((fm.EntityCountry = cc.Code2)))
		left join cfg_configurationcurrency cr ON ((fm.Currency = cr.Oid)))
		left join sys_userdetail ud ON ((fm.UpdatedBy = ud.Oid)))
		left join erp_customer cu ON ((fm.EntityOid = cu.Oid)))
;

CREATE VIEW view_documentfinancecommision AS
    SELECT 
        dfm.Oid AS dfmOid,
        dfm.DocumentNumber AS dfmDocumentNumber,
        dfm.Date AS dfmDate,
        dfc.Ord AS dfcOrd,
        dfc.Date AS dfcDate,
        dfc.Commission AS dfcCommission,
        dfc.Total AS dfcTotal,
        dfd.Oid AS dfdOid,
        dfd.Ord AS dfdOrd,
        dfd.Code AS dfdCode,
        dfd.Designation AS dfdDesignation,
        dfd.Quantity AS dfdQuantity,
        dfd.UnitMeasure AS dfdUnitMeasure,
        dfd.Price AS dfdPrice,
        dfd.Vat AS dfdVat,
        dfd.VatExemptionReasonDesignation AS dfdVatExemptionReasonDesignation,
        dfd.Discount AS dfdDiscount,
        dfd.TotalNet AS dfdTotalNet,
        dfd.TotalGross AS dfdTotalGross,
        dfd.TotalDiscount AS dfdTotalDiscount,
        dfd.TotalTax AS dfdTotalTax,
        dfd.TotalFinal AS dfdTotalFinal,
        dfd.PriceType AS dfdPriceType,
        dfd.PriceFinal AS dfdPriceFinal,
        usd.Oid AS usdOid,
        usd.Name AS usdName,
        cpt.Oid AS cptOid,
        cpt.Designation AS cptDesignation
    FROM
        ((((fin_documentfinancecommission dfc
        LEFT JOIN fin_documentfinancemaster dfm ON ((dfc.FinanceMaster = dfm.Oid)))
        LEFT JOIN fin_documentfinancedetail dfd ON ((dfc.FinanceDetail = dfd.Oid)))
        LEFT JOIN sys_userdetail usd ON ((dfc.UserDetail = usd.Oid)))
        LEFT JOIN pos_configurationplaceterminal cpt ON ((dfc.Terminal = cpt.Oid)))
;

CREATE VIEW view_documentfinancemastertotal AS
	SELECT 
		fmt.Oid AS fmtOid,
		fmt.Value AS fmtValue,
		fmt.Total AS fmtTotal,
		fmt.TotalBase AS fmtTotalBase,
		fmt.TotalType AS fmtTotalType,
		fmt.DocumentMaster AS fmtDocumentMaster,
		cvr.Code AS cvrCode,
		cvr.Designation AS cvrDesignation,
		cvr.TaxType AS cvrTaxType,
		cvr.TaxCode AS cvrTaxCode,
		cvr.TaxCountryRegion AS cvrTaxCountryRegion
	FROM
		(fin_documentfinancemastertotal fmt
		left join fin_configurationvatrate cvr ON ((fmt.Value = cvr.Value)))
;

CREATE VIEW view_documentfinancepayment AS
	SELECT 
		ftp.Oid AS ftpOid,
		ftp.Code AS ftpCode,
		ftp.Designation AS ftpDesignation,
		ftp.ResourceString AS ftpResourceString,
		ftp.ResourceStringReport AS ftpResourceStringReport,
		ftm.Oid AS ftmOid,
		ftm.Code AS ftmCode,
		ftm.Designation AS ftmDesignation,
		ftm.Credit AS ftmCredit,
		fma.Oid AS fmaOid,
		fma.CreatedAt AS fmaCreatedAt,
		fma.DocumentNumber AS fmaDocumentNumber,
		fma.DocumentDate AS fmaDocumentDate,
		fma.TotalGross AS fmaTotalGross,
		fma.TotalDiscount AS fmaTotalDiscount,
		fma.TotalTax AS fmaTotalTax,
		fma.TotalFinal AS fmaTotalFinal,
		fma.TotalFinalRound AS fmaTotalFinalRound,
		fma.Payed AS fmaPayed,
		fma.PayedDate AS fmaPayedDate,
		fma.EntityOid AS fmaEntityOid,
		fma.Notes AS fmaNotes,
		fpa.Oid AS fpaOid,
		fpa.PaymentRefNo AS fpaPaymentRefNo,
		fpa.PaymentType AS fpaPaymentType,
		fpa.PaymentStatus AS fpaPaymentStatus,
		fpa.PaymentStatusDate AS fpaPaymentStatusDate,
		fpa.PaymentMechanism AS fpaPaymentMechanism,
		fpa.PaymentAmount AS fpaPaymentAmount,
		fmp.Ord AS fmpOrd,
		fmp.CreditAmount AS fmpCreditAmount,
		fmp.DebitAmount AS fmpDebitAmount,
		fpa.PaymentDate AS fpaPaymentDate,
		fpa.ExtendedValue AS fpaExtendedValue,
		fpa.DocumentDate AS fpaDocumentDate,
		fpa.ExchangeRate AS fpaExchangeRate,
		fpa.Notes AS fpaNotes,
		cus.Oid AS cusOid,
		cus.Code AS cusCode,
		cus.Name AS cusName,
		cus.Address AS cusAddress,
		cus.ZipCode AS cusZipCode,
		cus.City AS cusCity,
		cco.Designation AS ccoDesignation,
		cus.FiscalNumber AS cusFiscalNumber,
		cpm.Oid AS cpmOid,
		cpm.Code AS cpmCode,
		cpm.Designation AS cpmDesignation,
		cpm.Acronym AS cpmAcronym,
		cur.Designation AS curDesignation,
		cur.Acronym AS curAcronym,
		cur.Symbol AS curSymbol
	FROM
		((((((((fin_documentfinancemaster fma
		left join fin_documentfinancemasterpayment fmp ON ((fmp.DocumentFinanceMaster = fma.Oid)))
		left join fin_documentfinancepayment fpa ON ((fpa.Oid = fmp.DocumentFinancePayment)))
		left join fin_documentfinancetype ftp ON ((ftp.Oid = fpa.DocumentType)))
		left join fin_documentfinancetype ftm ON ((ftm.Oid = fma.DocumentType)))
		left join fin_configurationpaymentmethod cpm ON ((cpm.Oid = fpa.PaymentMethod)))
		left join erp_customer cus ON ((cus.Oid = fpa.EntityOid)))
		left join cfg_configurationcurrency cur ON ((cur.Oid = fpa.Currency)))
		left join cfg_configurationcountry cco ON ((cco.Oid = cus.Country)))
;

CREATE VIEW view_documentfinanceseries AS
	SELECT 
		dfy.Oid AS dfyOid,
		dfy.Ord AS dfyOrd,
		dfy.Code AS dfyCode,
		dfy.Designation AS dfyDesignation,
		dfy.FiscalYear AS dfyFiscalYear,
		dfy.Acronym AS dfyAcronym,
		dfs.Oid AS dfsOid,
		dfs.Ord AS dfsOrd,
		dfs.Code AS dfsCode,
		dfs.Designation AS dfsDesignation,
		dfs.NextDocumentNumber AS dfsNextDocumentNumber,
		dfs.DocumentNumberRangeBegin AS dfsDocumentNumberRangeBegin,
		dfs.DocumentNumberRangeEnd AS dfsDocumentNumberRangeEnd,
		dfs.Acronym AS dfsAcronym,
		dft.Oid AS dftOid,
		dft.Ord AS dftOrd,
		dft.Code AS dftCode,
		dft.Designation AS dftDesignation,
		dft.DocumentType AS dftDocumentTypeOid,
		dtp.Designation AS dftDocumentTypeDesignation,
		dft.Terminal AS dftTerminalOid,
		cpt.Designation AS dftTerminalDesignation
	FROM
		((((fin_documentfinanceyears dfy
		join fin_documentfinanceseries dfs ON ((dfs.FiscalYear = dfy.Oid)))
		join fin_documentfinanceyearserieterminal dft ON ((dft.Serie = dfs.Oid)))
		join fin_documentfinancetype dtp ON ((dtp.Oid = dft.DocumentType)))
		join pos_configurationplaceterminal cpt ON ((cpt.Oid = dft.Terminal)))
;

CREATE VIEW view_orders AS
	SELECT 
		dm.Oid AS dmOid,
		dm.DateStart AS dmDateStart,
		dm.OrderStatus AS dmOrderStatus,
		dm.PlaceTable AS dmPlaceTable,
		ct.Oid AS ctOid,
		ct.Designation AS ctDesignation,
		cp.Oid AS cpOid,
		cp.Designation AS cpDesignation,
		dm.UpdatedBy AS dmUpdatedBy,
		dm.UpdatedWhere AS dmUpdatedWhere,
		dt.Oid AS dtOid,
		dt.TicketId AS dtTicketId,
		dt.DateStart AS dtDateStart,
		dt.PriceType AS dtPriceType,
		dt.Discount AS dtDiscount,
		dt.PlaceTable AS dtPlaceTable,
		dt.UpdatedBy AS dtUpdatedBy,
		dt.UpdatedWhere AS dtUpdatedWhere,
		dd.Oid AS ddOid,
		dd.Article AS ddArticle,
		dd.Ord AS ddOrd,
		dd.Code AS ddCode,
		dd.Designation AS ddDesignation,
		dd.Price AS ddPrice,
		dd.Quantity AS ddQuantity,
		dd.UnitMeasure AS ddUnitMeasure,
		dd.Discount AS ddDiscount,
		dd.Vat AS ddVat,
		dd.VatExemptionReason AS ddVatExemptionReason,
		dd.TotalGross AS ddTotalGross,
		dd.TotalDiscount AS ddTotalDiscount,
		dd.TotalTax AS ddTotalTax,
		dd.TotalFinal AS ddTotalFinal,
		dd.Token1 AS ddToken1,
		dd.Token2 AS ddToken2,
		dd.UpdatedAt AS ddUpdatedAt,
		dd.UpdatedBy AS ddUpdatedBy,
		dd.UpdatedWhere AS ddUpdatedWhere
	FROM
		((((fin_documentordermain dm
		join fin_documentorderticket dt ON ((dm.Oid = dt.OrderMain)))
		join fin_documentorderdetail dd ON ((dt.Oid = dd.OrderTicket)))
		join pos_configurationplacetable ct ON ((ct.Oid = dt.PlaceTable)))
		join pos_configurationplace cp ON ((cp.Oid = ct.Place)))
;

CREATE VIEW view_systemaudit AS
	SELECT 
		sau.Date AS Date, 
		sat.Token AS Token,
		sau.Description AS Description,
		usd.Name AS UserName,
		cpt.Designation AS Terminal
	FROM
		sys_systemaudit sau
		left join sys_systemaudittype sat ON (sat.Oid = sau.AuditType)
		left join sys_userdetail usd ON (usd.Oid = sau.UserDetail)
		left join pos_configurationplaceterminal cpt ON (cpt.Oid = sau.Terminal)
;

CREATE VIEW view_tables AS
	SELECT 
		cpl.Oid AS cplOid,
		cpl.Ord AS cplOrd,
		cpl.Designation AS cplDesignation,
		cpt.Oid AS cptOid,
		cpt.Ord AS cptOrd,
		cpt.Designation AS cptDesignation,
		cpt.Discount AS cptDiscount,
		cpt.TableStatus AS cptTableStatus,
		cpt.TotalOpen AS cptTotalOpen,
		cpt.DateTableOpen AS cptDateTableOpen,
		cpt.DateTableClosed AS cptDateTableClosed,
		cpr.Oid AS cprOid,
		cpr.Ord AS cprOrd,
		cpr.Designation AS cprDesignation,
		cpr.EnumValue AS cprEnumValue
	FROM
		((pos_configurationplace cpl
		join pos_configurationplacetable cpt ON ((cpt.Place = cpl.Oid)))
		join fin_configurationpricetype cpr ON ((cpr.Oid = cpl.PriceType)))
;

CREATE VIEW view_userprofile AS
	SELECT 
		upr.Oid AS uprOid,
		upr.Designation AS uprDesignation,
		upp.Granted AS uppGranted,
		upi.Oid AS upiOid,
		upi.Code AS upiCode,
		upi.Token AS upiToken,
		upi.Designation AS upiDesignation
	FROM
		((sys_userprofile upr
		join sys_userpermissionprofile upp ON ((upr.Oid = upp.UserProfile)))
		join sys_userpermissionitem upi ON ((upi.Oid = upp.PermissionItem)))
;

CREATE VIEW view_worksessionmovement AS
	SELECT
		wsp.Oid AS wspPeriod,
		wsp.PeriodType AS wspPeriodType,
		wsp.SessionStatus AS wspPeriodStatus,
		wsp.Parent AS wspPeriodParent,
		wmt.Oid AS wmtMovementType,
		wmt.Code AS wmtMovementTypeCode,
		wmt.Token AS wmtMovementTypeToken,
		wsm.Oid AS wsmOid,
		wsm.Ord AS wsmOrd,
		wsm.Date AS wsmDate,
		wsm.Description AS wsmDescription,
		wsm.MovementAmount AS wsmMovementAmount,
		wsm.DocumentFinanceType AS wsmDocumentFinanceType,
		dft.Designation AS dftCode,
		dft.Designation AS dftDesignation,
		dfm.Oid AS dfmDocument,
		dfm.DocumentNumber AS dfmDocumentNumber,
		dfm.DocumentStatusStatus AS dfmDocumentStatusStatus,
		dfm.SourceOrderMain AS dfmDocumentSourceOrder,
		dfp.Oid AS dfpOid,
		dfp.PaymentRefNo AS dfpPaymentRefNo,
		dfp.PaymentStatus AS dfpPaymentStatus,
		dfp.SourcePayment AS dfpSourcePayment,
		wsm.PaymentMethod AS wsmPaymentMethod,
		cpm.Ord AS cpmPaymentMethodOrd,
		cpm.Code AS cpmPaymentMethodCode,
		cpm.Designation AS cpmPaymentMethodDesignation,
		cpm.Token AS cpmPaymentMethodToken,
		cpt.Oid AS cptTerminal,
		cpt.Ord AS cptTerminalOrd,
		cpt.Code AS cptTerminalCode,
		cpt.Designation AS cptTerminalDesignation,
		usd.Oid AS usdUserDetail,
		usd.Ord AS usdUserDetailOrd,
		usd.Code AS usdUserDetailCode,
		usd.Name AS usdUserDetailName
	FROM
		((((((((pos_worksessionmovement wsm
		left join pos_worksessionperiod wsp ON ((wsm.WorkSessionPeriod = wsp.Oid)))
		left join pos_worksessionmovementtype wmt ON ((wsm.WorkSessionMovementType = wmt.Oid)))
		left join fin_documentfinancetype dft ON ((dft.Oid = wsm.DocumentFinanceType)))
		left join fin_documentfinancemaster dfm ON ((dfm.Oid = wsm.DocumentFinanceMaster)))
		left join fin_documentfinancepayment dfp ON ((dfp.Oid = wsm.DocumentFinancePayment)))
		left join fin_configurationpaymentmethod cpm ON ((wsm.PaymentMethod = cpm.Oid)))
		left join pos_configurationplaceterminal cpt ON ((wsm.Terminal = cpt.Oid)))
		left join sys_userdetail usd ON ((wsm.UserDetail = usd.Oid)))
;

CREATE VIEW view_worksessionmovementresume AS
	SELECT
		vwm.wspPeriod AS Period,
		vwm.wspPeriodType AS PeriodType,
		vwm.wspPeriodStatus AS PeriodStatus,
		vwm.wspPeriodParent AS PeriodParent,
		vwm.cptTerminal AS Terminal,
		vwm.cptTerminalCode AS TerminalCode,
		vwm.cptTerminalDesignation AS TerminalDesignation,
		vwm.usdUserDetail AS UserDetail,
		vwm.usdUserDetailCode AS UserDetailCode,
		vwm.usdUserDetailName AS UserDetailName,
		vwm.wsmDate AS MovementDate,
		vwm.wsmDescription AS MovementDescription,
		vwm.wsmMovementAmount AS MovementAmount,
		vwm.wmtMovementType AS MovementType,
		vwm.wmtMovementTypeCode AS MovementTypeCode,
		vwm.wmtMovementTypeToken AS MovementTypeToken,
		vwm.wsmDocumentFinanceType AS DocumentType,
		vwm.dftCode AS DocumentTypeCode,
		vwm.dftDesignation AS DocumentTypeDesignation,
		vwm.dfmDocument AS Document,
		vwm.dfmDocumentNumber AS DocumentNumber,
		vwm.dfmDocumentStatusStatus AS DocumentStatusStatus,
		vwm.dfmDocumentSourceOrder AS DocumentOrderMain,
		vwm.dfpOid AS Payment,
		vwm.dfpPaymentRefNo AS PaymentRefNo,
		vwm.dfpPaymentStatus AS PaymentStatus,
		vwm.wsmPaymentMethod AS PaymentMethod,
		vwm.cpmPaymentMethodCode AS PaymentMethodCode,
		vwm.cpmPaymentMethodDesignation AS PaymentMethodDesignation,
		vwm.cpmPaymentMethodToken AS PaymentMethodToken,
		vdf.afFamily AS Family,
		vdf.afFamilyCode AS FamilyCode,
		vdf.afFamilyDesignation AS FamilyDesignation,
		vdf.sfSubFamily AS SubFamily,
		vdf.sfSubFamilyCode AS SubFamilyCode,
		vdf.sfSubFamilyDesignation AS SubFamilyDesignation,
		vdf.fdArticle AS Article,
		vdf.fdCode AS Code,
		vdf.fdDesignation AS Designation,
		vdf.fdQuantity AS Quantity,
		vdf.fdUnitMeasure AS UnitMeasure,
		vdf.fdPrice AS Price,
		vdf.cfVatCode AS VatCode,
		vdf.cfVatDesignation AS VatDesignation,
		vdf.fdVat AS VatValue,
		vdf.fdDiscount AS Discount,
		vdf.fdTotalGross AS TotalGross,
		vdf.fdTotalTax AS TotalTax,
		vdf.fdTotalFinal AS TotalFinal
	FROM
		(view_worksessionmovement vwm
		left join view_documentfinance vdf ON ((vwm.dfmDocument = vdf.fmOid)))
;
				
CREATE VIEW view_worksessionperiodtotal AS
	SELECT 
		wpt.Ord AS wptOrd,
		cpm.Token AS cpmPaymentMethodToken,
		wpt.Total AS wptTotal,
		wpt.PaymentMethod AS wptPaymentMethod,
		wpt.Period AS wptPeriod,
		wsp.Oid AS wspOid,
		wsp.DateEnd AS wspDateEnd,
		wsp.Designation AS wspDesignation,
		wsp.Terminal AS wspTerminal,
		wsp.PeriodType AS wspPeriodType,
		wsp.Parent AS wspParent
	FROM
		((pos_worksessionperiodtotal wpt
		left join pos_worksessionperiod wsp ON ((wpt.Period = wsp.Oid)))
		left join fin_configurationpaymentmethod cpm ON ((wpt.PaymentMethod = cpm.Oid)))
;
