INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('b9c3c308-8898-4179-b835-38377ba27703', 4100, 4100,'BACKOFFICE_MAN_WAREHOUSE_CREATE','BackOffice :: Gestão de Armazéns :: Criação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('9bf1d39f-8f9a-45fb-b58c-4f45987395e7', 4110, 4110,'BACKOFFICE_MAN_WAREHOUSE_DELETE','BackOffice ::Gestão de Armazéns :: Eliminação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('a0b3e944-c9f8-459c-8b4b-dfd24b15f160', 4120, 4120,'BACKOFFICE_MAN_WAREHOUSE_EDIT','BackOffice :: Gestão de Armazéns :: Editar','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('ae2ab865-365c-4bc0-82f7-79505f517ffe', 4130, 4130,'BACKOFFICE_MAN_WAREHOUSE_MENU','BackOffice :: Gestão de Armazéns :: Menu','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('083d7c91-72c0-482d-83e0-7c0b327fe8e8', 4140, 4140,'BACKOFFICE_MAN_WAREHOUSE_VIEW','BackOffice :: Gestão de Armazéns :: Ver','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('7bdcdfdc-6bca-4789-aaff-7287f656e3a4',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','b9c3c308-8898-4179-b835-38377ba27703');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('8f75f053-ff2f-47f5-845b-09768fad4fe7',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','9bf1d39f-8f9a-45fb-b58c-4f45987395e7');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('a082b301-be75-42df-badb-d0460ff58356',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','a0b3e944-c9f8-459c-8b4b-dfd24b15f160');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('0acb1ff2-7901-4f5a-abd0-d0517860b8cd',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','ae2ab865-365c-4bc0-82f7-79505f517ffe');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('97b6fbce-53ba-45b9-ae1c-08805338d16f',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','083d7c91-72c0-482d-83e0-7c0b327fe8e8');

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('ae3dd7d2-4ea5-4da6-b16a-0c820a6ea274', 4200, 4200,'BACKOFFICE_MAN_ARTICLEWAREHOUSE_CREATE','BackOffice :: Gestão de Armazéns/Artigos :: Criação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('56ea98eb-e0f9-4d10-9bd7-bb37c9b9ac65', 4210, 4210,'BACKOFFICE_MAN_ARTICLEWAREHOUSE_DELETE','BackOffice ::Gestão de Armazéns/Artigos :: Eliminação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('e9b23452-976f-48c1-a324-94681bed1868', 4220, 4220,'BACKOFFICE_MAN_ARTICLEWAREHOUSE_EDIT','BackOffice :: Gestão de Armazéns/Artigos :: Editar','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('8028840b-5e4b-4f04-8816-23ea67b2c2f4', 4230, 4230,'BACKOFFICE_MAN_ARTICLEWAREHOUSE_MENU','BackOffice :: Gestão de Armazéns/Artigos :: Menu','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('e1ab4485-0909-4c0b-9e62-b148febeedb4', 4240, 4240,'BACKOFFICE_MAN_ARTICLEWAREHOUSE_VIEW','BackOffice :: Gestão de Armazéns/Artigos :: Ver','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('5632906e-e0fd-44b5-8b05-a39cb5b67121',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','ae3dd7d2-4ea5-4da6-b16a-0c820a6ea274');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('2d77ada6-e406-4e30-ad94-e85e984cbceb',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','56ea98eb-e0f9-4d10-9bd7-bb37c9b9ac65');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('0698adc9-2f35-47ec-ac7b-eedf43b21998',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','e9b23452-976f-48c1-a324-94681bed1868');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('95cbf173-c5bf-4424-8234-ac1f47f96fce',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','8028840b-5e4b-4f04-8816-23ea67b2c2f4');
INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('18d65246-60c1-4b27-9240-41bf4d0bf7a3',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','e1ab4485-0909-4c0b-9e62-b148febeedb4');

alter table [fin_articlestock] ADD [ArticleSerialNumber] char(36) NULL;
alter table [fin_articlestock] ADD [PurchasePrice] money NULL;
alter table [fin_articlestock] ADD [AttachedFile] varbinary NULL;
alter table [fin_article] ADD [UniqueArticles] bit;
alter table [fin_documentfinancedetail] ADD [SerialNumber] TEXT;
alter table [fin_documentfinancedetail] ADD [Warehouse] TEXT;



CREATE TABLE [fin_warehouse] ([Oid] char(36) NOT NULL, [Disabled] bit, [Notes] text NULL, [CreatedAt] datetime NULL, [CreatedBy] char(36) NULL, [CreatedWhere] char(36) NULL, [UpdatedAt] datetime NULL, [UpdatedBy] char(36) NULL, [UpdatedWhere] char(36) NULL, [DeletedAt] datetime NULL, [DeletedBy] char(36) NULL, [DeletedWhere] char(36) NULL, [Ord] numeric(10,0) NULL, [Code] numeric(10,0) NULL,[Designation] char(36) NULL, [IsDefault] bit, [OptimisticLockField] int NULL, primary key ([Oid]));
CREATE INDEX [iDesignation_fin_warehouse] on [fin_warehouse]([Designation]);
CREATE INDEX [iCode_fin_warehouse] on [fin_warehouse]([Code]);
CREATE INDEX [iOrd_fin_warehouse] on [fin_warehouse]([Ord]);
CREATE INDEX [iCreatedBy_fin_warehouse] on [fin_warehouse]([CreatedBy]);
CREATE INDEX [iCreatedWhere_fin_warehouse] on [fin_warehouse]([CreatedWhere]);
CREATE INDEX [iUpdatedBy_fin_warehouse] on [fin_warehouse]([UpdatedBy]);
CREATE INDEX [iUpdatedWhere_fin_warehouse] on [fin_warehouse]([UpdatedWhere]);
CREATE INDEX [iDeletedBy_fin_warehouse] on [fin_warehouse]([DeletedBy]);
CREATE INDEX [iDeletedWhere_fin_warehouse] on [fin_warehouse]([DeletedWhere]);

CREATE TABLE [fin_warehouselocation] ([Oid] char(36) NOT NULL, [Disabled] bit, [Notes] text NULL, [CreatedAt] datetime NULL, [CreatedBy] char(36) NULL, [CreatedWhere] char(36) NULL, [UpdatedAt] datetime NULL, [UpdatedBy] char(36) NULL, [UpdatedWhere] char(36) NULL, [DeletedAt] datetime NULL, [DeletedBy] char(36) NULL, [DeletedWhere] char(36) NULL, [Ord] numeric(10,0) NULL, [Code] numeric(10,0) NULL,[Designation] char(36) NULL, [Warehouse] char(36) NULL, [OptimisticLockField] int NULL, primary key ([Oid]));
CREATE INDEX [iDesignation_fin_warehouselocation] on [fin_warehouselocation]([Designation]);
CREATE INDEX [iWarehouse_fin_warehouselocation] on [fin_warehouselocation]([Warehouse]);
CREATE INDEX [iCode_fin_warehouselocation] on [fin_warehouselocation]([Code]);
CREATE INDEX [iOrd_fin_warehouselocation] on [fin_warehouselocation]([Ord]);
CREATE INDEX [iCreatedBy_fin_warehouselocation] on [fin_warehouselocation]([CreatedBy]);
CREATE INDEX [iCreatedWhere_fin_warehouselocation] on [fin_warehouselocation]([CreatedWhere]);
CREATE INDEX [iUpdatedBy_fin_warehouselocation] on [fin_warehouselocation]([UpdatedBy]);
CREATE INDEX [iUpdatedWhere_fin_warehouselocation] on [fin_warehouselocation]([UpdatedWhere]);
CREATE INDEX [iDeletedBy_fin_warehouselocation] on [fin_warehouselocation]([DeletedBy]);
CREATE INDEX [iDeletedWhere_fin_warehouselocation] on [fin_warehouselocation]([DeletedWhere]);

CREATE TABLE [fin_articlewarehouse] ([Oid] char(36) NOT NULL, [Disabled] bit, [Notes] text NULL, [CreatedAt] datetime NULL, [CreatedBy] char(36) NULL, [CreatedWhere] char(36) NULL, [UpdatedAt] datetime NULL, [UpdatedBy] char(36) NULL, [UpdatedWhere] char(36) NULL, [DeletedAt] datetime NULL, [DeletedBy] char(36) NULL, [DeletedWhere] char(36) NULL, [Warehouse] char(36) NULL, [Location] char(36) NULL, [Article] char(36) NULL, [ArticleSerialNumber] char(36) NULL, [Quantity] money NULL, [OptimisticLockField] int NULL, primary key ([Oid]));
CREATE INDEX [iArticle_fin_articlewarehouse] on [fin_articlewarehouse]([Article]);
CREATE INDEX [iWarehouse_fin_articlewarehouse] on [fin_articlewarehouse]([Warehouse]);
CREATE INDEX [iLocation_fin_articlewarehouse] on [fin_articlewarehouse]([Warehouse]);
CREATE INDEX [iSerialNumber_fin_articlewarehouse] on [fin_articlewarehouse]([ArticleSerialNumber]);
CREATE INDEX [iQuantity_fin_articlewarehouse] on [fin_articlewarehouse]([Quantity]);
CREATE INDEX [iCreatedBy_fin_articlewarehouse] on [fin_articlewarehouse]([CreatedBy]);
CREATE INDEX [iCreatedWhere_fin_articlewarehouse] on [fin_articlewarehouse]([CreatedWhere]);
CREATE INDEX [iUpdatedBy_fin_articlewarehouse] on [fin_articlewarehouse]([UpdatedBy]);
CREATE INDEX [iUpdatedWhere_fin_articlewarehouse] on [fin_articlewarehouse]([UpdatedWhere]);
CREATE INDEX [iDeletedBy_fin_articlewarehouse] on [fin_articlewarehouse]([DeletedBy]);
CREATE INDEX [iDeletedWhere_fin_articlewarehouse] on [fin_articlewarehouse]([DeletedWhere]);

CREATE TABLE [fin_articleserialnumber] ([Oid] char (36) NOT NULL, [Disabled] bit, [Notes] text, [CreatedAt] datetime, [CreatedBy] char (36), [CreatedWhere] char (36), [UpdatedAt] datetime NULL, [UpdatedBy] char(36) NULL, [UpdatedWhere] char(36) NULL, [DeletedAt] datetime NULL, [DeletedBy] char(36) NULL, [DeletedWhere] char (36), [Article] char (36), [SerialNumber] char (36) NOT NULL UNIQUE, [OptimisticLockField] int, [ArticleWarehouse] char (36), [StockMovimentIn] char (36), [StockMovimentOut] char (36), [IsSold] boolean Default null, PRIMARY KEY ([Oid]));
CREATE INDEX [iArticle_fin_articleserialnumber] ON [fin_articleserialnumber] ([Article]);
CREATE INDEX [iSerialNumber_fin_articleserialnumber] ON [fin_articleserialnumber] ([SerialNumber]);
CREATE INDEX [iCreatedBy_fin_articleserialnumber] ON [fin_articleserialnumber] ([CreatedBy]);
CREATE INDEX [iCreatedWhere_fin_articleserialnumber] ON [fin_articleserialnumber] ([CreatedWhere]);
CREATE INDEX [iUpdatedBy_fin_articleserialnumber] ON fin_articleserialnumber ([UpdatedBy]);
CREATE INDEX [iUpdatedWhere_fin_articleserialnumber] ON [fin_articleserialnumber] ([UpdatedWhere]);
CREATE INDEX [iDeletedBy_fin_articleserialnumber] ON [fin_articleserialnumber] ([DeletedBy]);
CREATE INDEX [iDeletedWhere_fin_articleserialnumber] ON [fin_articleserialnumber] ([DeletedWhere]);

CREATE TABLE [fin_articlecompositionserialnumber] ([Oid] char(36) NOT NULL, [Disabled] bit, [Notes] text NULL, [CreatedAt] datetime NULL, [CreatedBy] char(36) NULL, [CreatedWhere] char(36) NULL, [UpdatedAt] datetime NULL, [UpdatedBy] char(36) NULL, [UpdatedWhere] char(36) NULL, [DeletedAt] datetime NULL, [DeletedBy] char(36) NULL, [DeletedWhere] char(36) NULL, [Ord] numeric(10,0) NULL, [Code] numeric(10,0) NULL,[ArticleSerialNumber] char(36) NULL, [ArticleSerialNumberChild] char(36) NULL, [OptimisticLockField] int NULL, primary key ([Oid]));
CREATE INDEX [iArticleSerialNumber_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([ArticleSerialNumber]);
CREATE INDEX [iArticleSerialNumberChild_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([ArticleSerialNumberChild]);
CREATE INDEX [iCode_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([Code]);
CREATE INDEX [iOrd_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([Ord]);
CREATE INDEX [iCreatedBy_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([CreatedBy]);
CREATE INDEX [iCreatedWhere_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([CreatedWhere]);
CREATE INDEX [iUpdatedBy_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([UpdatedBy]);
CREATE INDEX [iUpdatedWhere_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([UpdatedWhere]);
CREATE INDEX [iDeletedBy_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([DeletedBy]);
CREATE INDEX [iDeletedWhere_fin_articlecompositionserialnumber] on [fin_articlecompositionserialnumber]([DeletedWhere]);

alter table [fin_articlewarehouse] ADD [ArticleSerialNumber] char(36) NULL;
alter table [fin_articleserialnumber] ADD [ArticleWarehouse] char(36) NULL;

INSERT INTO fin_warehouse (Oid, Ord, Code, Designation, IsDefault, Disabled) VALUES ('4f5f33a7-7717-49cd-8687-5dc302a0c3cb',10,10, 'Armazém 1', 1, 0);
INSERT INTO fin_warehouselocation (Oid, Ord, Code, Designation, Warehouse, Disabled) VALUES ('a12a472b-5f32-4c7a-a648-03072560ffc8',10,10, 'Localização 1','4f5f33a7-7717-49cd-8687-5dc302a0c3cb', 0);

INSERT INTO cfg_configurationpreferenceparameter (Oid,Disabled,Ord,Code,Token,Value,ValueTip,Required,RegEx,ResourceString,ResourceStringInfo,FormType,FormPageNo,InputType) VALUES ('213aa95f-8e67-498a-a5a4-0fc9ec48f88c',NULL,800,800,'CHECK_STOCKS','True','true | false',1,'RegexBoolean','prefparam_check_stocks',NULL,2,1,3);

INSERT INTO cfg_configurationpreferenceparameter (Oid,Disabled,Ord,Code,Token,Value,ValueTip,Required,RegEx,ResourceString,ResourceStringInfo,FormType,FormPageNo,InputType) VALUES ('a318a646-19fe-468a-9325-fa88eadeb22d','true',810,810,'CHECK_STOCKS_MESSAGE','True','true | false',1,'RegexBoolean','prefparam_check_stocks_message',NULL,2,1,3);

alter table [fin_article] ADD [MinimumStock] money;

INSERT INTO erp_customer (Oid,Ord,Code,CodeInternal,Name,FiscalNumber,CardNumber,Address,Locality,ZipCode,City,Country,Discount,PriceType,CustomerType,Disabled,Supplier) VALUES ('00000000-0000-0000-0000-000000000002',01,01,'00000000-0000-0000-0000-000002',(Select Value from cfg_configurationpreferenceparameter where Token = 'COMPANY_NAME'),(Select Value from cfg_configurationpreferenceparameter where Token = 'COMPANY_FISCALNUMBER'),NULL,'Desconhecido','Desconhecido','1000-000','Indefinido','e7e8c325-a0d4-4908-b148-508ed750676a',0,'cf17a218-b687-4b82-a8f4-0905594ac1f5','a4b3811f-9851-430d-810e-f8be7ac3f392',1,1);

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('f07fbdec-8a54-4d0f-836c-1a4fc05b1c1c', 3400, 3400,'BACKOFFICE_MAN_ARTICLESTOCK_CREATE','BackOffice :: Gestão de Movimentos de Stock :: Criação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('6cbb4f99-a43b-4da3-907b-8941152b72b3',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','f07fbdec-8a54-4d0f-836c-1a4fc05b1c1c');

alter table [fin_article] ADD [IsComposed] bit;

INSERT INTO fin_articleserialnumber (Oid, SerialNumber, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);
INSERT INTO fin_warehouse (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);

CREATE TABLE [fin_articlecomposition] ([Oid] char(36) NOT NULL, [Disabled] bit, [Notes] text NULL, [CreatedAt] datetime NULL, [CreatedBy] char(36) NULL, [CreatedWhere] char(36) NULL, [UpdatedAt] datetime NULL, [UpdatedBy] char(36) NULL, [UpdatedWhere] char(36) NULL, [DeletedAt] datetime NULL, [DeletedBy] char(36) NULL, [DeletedWhere] char(36) NULL, [Article] char(36) NULL, [ArticleChild] char(36) NULL, [Quantity] money NULL, [OptimisticLockField] int NULL, primary key ([Oid]));
CREATE INDEX [iArticle_articlecomposition] on [fin_articlecomposition]([Article]);
CREATE INDEX [iArticleChild_articlecomposition] on [fin_articlecomposition]([ArticleChild]);
CREATE INDEX [iQuantity_articlecomposition] on [fin_articlecomposition]([Quantity]);
CREATE INDEX [iCreatedBy_fin_articlecomposition] on [fin_articlecomposition]([CreatedBy]);
CREATE INDEX [iCreatedWhere_fin_articlecomposition] on [fin_articlecomposition]([CreatedWhere]);
CREATE INDEX [iUpdatedBy_fin_articlecomposition] on [fin_articlecomposition]([UpdatedBy]);
CREATE INDEX [iUpdatedWhere_fin_articlecomposition] on [fin_articlecomposition]([UpdatedWhere]);
CREATE INDEX [iDeletedBy_fin_articlecomposition] on [fin_articlecomposition]([DeletedBy]);
CREATE INDEX [iDeletedWhere_fin_articlecomposition] on [fin_articlecomposition]([DeletedWhere]);

INSERT INTO cfg_configurationpreferenceparameter (Oid,Disabled,Ord,Code,Token,Value,ValueTip,Required,RegEx,ResourceString,ResourceStringInfo,FormType,FormPageNo,InputType) VALUES ('f19d91c5-b042-4cca-9025-6045f76dfb41',NULL,700,700,'PRINT_QRCODE','True','true | false',1,'RegexBoolean','prefparam_print_qrcode',NULL,2,1,3);
alter table [fin_documentfinancemaster] ADD [ATDocQRCode] nvarchar(200);
alter table [fin_documentfinancemaster] ADD [ATCUD] nvarchar(200);

DROP VIEW view_documentfinance;

CREATE VIEW view_documentfinance AS SELECT        ft.Oid AS ftOid, ft.Ord AS ftDocumentTypeOrd, ft.Code AS ftDocumentTypeCode, ft.Designation AS ftDocumentTypeDesignation, ft.ResourceString AS ftDocumentTypeResourceString, ft.Acronym AS ftDocumentTypeAcronym, 
                         ft.ResourceStringReport AS ftDocumentTypeResourceStringReport, ft.Credit AS ftCredit, ft.WayBill AS ftWayBill, ft.SaftAuditFile AS ftSaftAuditFile, ft.SaftDocumentType AS ftSaftDocumentType, fm.Oid AS fmOid, fm.Date AS fmDate,
                          fm.DocumentNumber AS fmDocumentNumber, fm.DocumentDate AS fmDocumentDate, fm.SystemEntryDate AS fmSystemEntryDate, fm.TransactionID AS fmTransactionID, fm.ShipToDeliveryID AS fmShipToDeliveryID, 
                         fm.ShipToDeliveryDate AS fmShipToDeliveryDate, fm.ShipToWarehouseID AS fmShipToWarehouseID, fm.ShipToLocationID AS fmShipToLocationID, fm.ShipToAddressDetail AS fmShipToAddressDetail, 
                         fm.ShipToCity AS fmShipToCity, fm.ShipToPostalCode AS fmShipToPostalCode, fm.ShipToRegion AS fmShipToRegion, fm.ShipToCountry AS fmShipToCountry, fm.ShipFromDeliveryID AS fmShipFromDeliveryID, 
                         fm.ShipFromDeliveryDate AS fmShipFromDeliveryDate, fm.ShipFromWarehouseID AS fmShipFromWarehouseID, fm.ShipFromLocationID AS fmShipFromLocationID, fm.ShipFromAddressDetail AS fmShipFromAddressDetail, 
                         fm.ShipFromCity AS fmShipFromCity, fm.ShipFromPostalCode AS fmShipFromPostalCode, fm.ShipFromRegion AS fmShipFromRegion, fm.ShipFromCountry AS fmShipFromCountry, 
                         fm.MovementStartTime AS fmMovementStartTime, fm.MovementEndTime AS fmMovementEndTime, fm.ATDocCodeID AS fmATDocCodeID, fm.DocumentCreatorUser AS fmDocumentCreatorUser, 
                         fm.DocumentStatusStatus AS fmDocumentStatusStatus, fm.Payed AS fmPayed, fm.PayedDate AS fmPayedDate, fm.TotalNet AS fmTotalNet, fm.TotalGross AS fmTotalGross, fm.TotalDiscount AS fmTotalDiscount, 
                         fm.TotalTax AS fmTotalTax, fm.TotalFinal AS fmTotalFinal, fm.TotalFinalRound AS fmTotalFinalRound, fm.TotalDelivery AS fmTotalDelivery, fm.TotalChange AS fmTotalChange, fm.Discount AS fmDiscount, 
                         fm.DiscountFinancial AS fmDiscountFinancial, fm.ExchangeRate AS fmExchangeRate, fm.EntityOid AS fmEntity, fm.EntityInternalCode AS fmEntityInternalCode, fm.ATDocQRCode AS fmATDocQRCode, cu.Ord AS cuEntityOrd, 
                         cu.Code AS cuEntityCode, cu.Hidden AS cuEntityHidden, fm.EntityName AS fmEntityName, fm.EntityAddress AS fmEntityAddress, fm.EntityZipCode AS fmEntityZipCode, fm.EntityLocality AS fmEntityLocality, 
                         fm.EntityCity AS fmEntityCity, fm.EntityCountry AS fmEntityCountryCode2, fm.EntityFiscalNumber AS fmEntityFiscalNumber, fm.Notes AS fmNotes, fm.SourceOrderMain AS fmSourceOrderMain, 
                         fm.DocumentParent AS fmDocumentParent, fm.DocumentType AS fmDocumentType, fm.DocumentSerie AS fmDocumentSerie, pm.Oid AS fmPaymentMethod, pm.Ord AS pmPaymentMethodOrd, 
                         pm.Code AS pmPaymentMethodCode, pm.Designation AS pmPaymentMethodDesignation, pm.Token AS pmPaymentMethodToken, fm.PaymentCondition AS fmPaymentCondition, pc.Ord AS pcPaymentConditionOrd, 
                         pc.Code AS pcPaymentConditionCode, pc.Designation AS pcPaymentConditionDesignation, pc.Acronym AS pcPaymentConditionAcronym, cc.Oid AS ccCountry, cc.Ord AS ccCountryOrd, cc.Code AS ccCountryCode, 
                         cc.Designation AS ccCountryDesignation, fm.Currency AS fmCurrency, cr.Ord AS crCurrencyOrd, cr.Code AS crCurrencyCode, cr.Designation AS crCurrencyDesignation, cr.Acronym AS crCurrencyAcronym, af.Oid AS afFamily, 
                         af.Ord AS afFamilyOrd, af.Code AS afFamilyCode, af.Designation AS afFamilyDesignation, sf.Oid AS sfSubFamily, sf.Ord AS sfSubFamilyOrd, sf.Code AS sfSubFamilyCode, sf.Designation AS sfSubFamilyDesignation, 
                         fd.Oid AS fdOid, fd.Article AS fdArticle, fd.Ord AS fdOrd, fd.Code AS fdCode, fd.Designation AS fdDesignation, fd.Quantity AS fdQuantity, fd.UnitMeasure AS fdUnitMeasure, fd.Price AS fdPrice, (fd.Price - ((fd.Price * fd.Discount) 
                         / 100)) AS fdPriceWithDiscount, fd.Vat AS fdVat, fd.Discount AS fdDiscount, ar.PriceWithVat AS arPriceWithVat, fd.TotalNet AS fdTotalNet, fd.TotalGross AS fdTotalGross, fd.TotalDiscount AS fdTotalDiscount, 
                         fd.TotalTax AS fdTotalTax, fd.TotalFinal AS fdTotalFinal, fd.VatExemptionReason AS fdVatExemptionReason, fd.VatExemptionReasonDesignation AS fdVatExemptionReasonDesignation, cx.Acronym AS cxAcronym, 
                         fd.Token1 AS fdToken1, fd.Token2 AS fdToken2, cv.Ord AS cfVatOrd, cv.Code AS cfVatCode, cv.Designation AS cfVatDesignation, cv.TaxType AS cvTaxType, cv.TaxCode AS cvTaxCode, 
                         cv.TaxCountryRegion AS cvTaxCountryRegion, cv.TaxExpirationDate AS cvTaxExpirationDate, cv.TaxDescription AS cvTaxDescription, ud.Oid AS udUserDetail, ud.Ord AS udUserDetailOrd, ud.Code AS udUserDetailCode, 
                         ud.Name AS udUserDetailName, tr.Oid AS trTerminal, tr.Ord AS trTerminalOrd, tr.Code AS trTerminalCode, tr.Designation AS trTerminalDesignation, cp.Oid AS cpPlace, cp.Ord AS cpPlaceOrd, cp.Code AS cpPlaceCode, 
                         cp.Designation AS cpPlaceDesignation, dm.PlaceTable AS dmPlaceTable, ct.Ord AS ctPlaceTableOrd, ct.Code AS ctPlaceTableCode, ct.Designation AS ctPlaceTableDesignation, cg.Designation AS cgDesignation, 
                         cg.Commission AS cgCommission, cv.Oid AS cfOid, ar.Class AS arClass, ac.Designation as acDesignation, ac.Acronym as acAcronym
FROM            (((((((((((((((((((fin_documentfinancemaster fm LEFT JOIN
                         fin_documentfinancedetail fd ON ((fm.Oid = fd.DocumentMaster))) LEFT JOIN
                         fin_documentfinancetype ft ON ((fm.DocumentType = ft.Oid))) LEFT JOIN
                         fin_article ar ON ((ar.Oid = fd.Article))) LEFT JOIN
                         fin_articlefamily af ON ((af.Oid = ar.Family))) LEFT JOIN
                         fin_articlesubfamily sf ON ((sf.Oid = ar.SubFamily))) LEFT JOIN
						            fin_articleclass ac ON ((ac.Oid = ar.Class))) LEFT JOIN
                         fin_configurationvatrate cv ON ((cv.Oid = fd.VatRate))) LEFT JOIN
                         fin_configurationvatexemptionreason cx ON ((cx.Oid = fd.VatExemptionReason))) LEFT JOIN
                         fin_documentordermain dm ON ((fm.SourceOrderMain = dm.Oid))) LEFT JOIN
                         pos_configurationplacetable ct ON ((dm.PlaceTable = ct.Oid))) LEFT JOIN
                         pos_configurationplace cp ON ((ct.Place = cp.Oid))) LEFT JOIN
                         fin_configurationpaymentmethod pm ON ((fm.PaymentMethod = pm.Oid))) LEFT JOIN
                         fin_configurationpaymentcondition pc ON ((pc.Oid = fm.PaymentCondition))) LEFT JOIN
                         cfg_configurationcountry cc ON ((fm.EntityCountry = cc.Code2))) LEFT JOIN
                         cfg_configurationcurrency cr ON ((fm.Currency = cr.Oid))) LEFT JOIN
                         sys_userdetail ud ON ((fm.UpdatedBy = ud.Oid))) LEFT JOIN
                         pos_usercommissiongroup cg ON ((cg.Oid = ud.CommissionGroup))) LEFT JOIN
                         pos_configurationplaceterminal tr ON ((fm.UpdatedWhere = tr.Oid))) LEFT JOIN
                         erp_customer cu ON ((fm.EntityOid = cu.Oid)));;

INSERT INTO cfg_configurationpreferenceparameter (Oid,Disabled,Ord,Code,Token,Value,ValueTip,Required,RegEx,ResourceString,ResourceStringInfo,FormType,FormPageNo,InputType) VALUES ('a8813e64-b20a-4bb4-b666-1cf81695e7e7',NULL,600,600,'TICKET_PRINT_TICKET','True','true | false',1,'RegexBoolean','prefparam_ticket_print_ticket',NULL,2,1,3);
						 
UPDATE cfg_configurationpreferenceparameter
SET Disabled = True, Value = True
WHERE Oid = 'c383e97a-c90b-46a5-9298-cc2436a30e5d';

UPDATE cfg_configurationpreferenceparameter
SET Disabled = True
WHERE Oid = 'cf1b6200-8213-4a56-b211-0377ca06cdbc' OR Oid = '428ece74-22db-44b6-864a-f426a77f77ea' OR Oid = 'c383e97a-c90b-46a5-9298-cc2436a30e5d' OR Oid = 'e744d01c-0f8a-4680-a86c-1d3f5815d7f1';

alter table [sys_configurationpoledisplay] ADD [COM] nvarchar(10);

alter table [sys_configurationprinterstemplates] ADD [IsBarCode] bit;

INSERT INTO [sys_configurationprinterstemplates] (Oid, CreatedAt, UpdatedAt, Ord, Code, Designation, FileTemplate, FinancialTemplate, IsBarCode) VALUES ('b931ef9c-75cf-4e2e-94ed-b67a22f50cda','2021-03-15 12:00:00.000','2021-03-15 12:00:00.000', 300, 300, 'Modelo para impressão de Código de Barras 40*30', 'Resources/Reports/UserReports/BarCodeTemplate_40x30.frx',0,1);

INSERT INTO [sys_configurationprinterstemplates] (Oid, CreatedAt, UpdatedAt, Ord, Code, Designation, FileTemplate, FinancialTemplate, IsBarCode) VALUES ('577c83c4-2469-4d15-aae8-770f34782d57','2021-03-15 12:00:00.000','2021-03-15 12:00:00.000', 310, 310, 'Modelo para impressão de Código de Barras 100*50', 'Resources/Reports/UserReports/BarCodeTemplate_100x50.frx',0,1);

alter table [fin_article] ADD [TemplateBarCode] char(36);

alter table [fin_articleserialnumber] ADD [Status] int;



INSERT INTO rpt_reporttype (Oid,Ord,Code,Designation,ResourceString,MenuIcon,Disabled) VALUES ('751c9e56-26bb-4fc8-8110-6c1e3b7c84e6',60,60,'ReportType #6','reporttype_label_type6','report_other_reports.png',NULL);

INSERT INTO rpt_report (Oid,Ord,Code,Designation,ResourceString,Token,ReportType,Disabled) VALUES ('8bda2d73-43ad-41ee-9c92-febbcd86ffab',940,940,'Relatório :: Stock por Armazém','report_list_stock_warehouse','REPORT_LIST_STOCK_WAREHOUSE','751c9e56-26bb-4fc8-8110-6c1e3b7c84e6',NULL);

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('061e7ff2-418c-4270-9292-e3b0b457f077', 5000,5000,'REPORT_LIST_STOCK_WAREHOUSE','Relatório :: Armazém :: Stocks','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('fe5dc800-cbb7-4e7b-8bf6-9825adc7889b',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','061e7ff2-418c-4270-9292-e3b0b457f077');

CREATE VIEW view_articlestockwarehouse AS
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
        art.Oid AS Article,
        art.Ord AS artOrd,
        art.Code AS artCode,
        art.CodeDealer AS artCodeDealer,
        art.Designation AS artDesignation,
        art.Disabled AS artDisabled,
        strftime('%Y-%m-%d', stk.CreatedAt) AS stkDateDay,
        stk.Oid AS stkOid,
        stk.CreatedAt AS Date,
        stk.Quantity AS stkQuantity,
        aum.Acronym AS aumAcronym,
        aum.Designation AS aumDesignation,
        stk.CreatedWhere AS afaTerminal,
        stk.CreatedBy AS afaUserDetail,
        wrh.Designation AS wrhDesignation,
        whl.Designation AS whlDesignation,
        asn.SerialNumber AS asnSerialNumber,
        wrh.Oid AS Warehouse,
        whl.Oid AS Location,
        asn.Oid AS ArticleSerialNumber
    FROM
        (((((((fin_articlewarehouse stk
        LEFT JOIN fin_article art ON ((art.Oid = stk.Article)))
        LEFT JOIN fin_articlefamily afa ON ((afa.Oid = art.Family)))
        LEFT JOIN fin_articlesubfamily asf ON ((asf.Oid = art.SubFamily)))
        LEFT JOIN cfg_configurationunitmeasure aum ON ((aum.Oid = art.UnitMeasure)))
        LEFT JOIN fin_warehouse wrh ON ((wrh.Oid = stk.Warehouse)))
        LEFT JOIN fin_warehouselocation whl ON ((whl.Oid = stk.Location)))
        LEFT JOIN fin_articleserialnumber asn ON ((asn.Oid = stk.ArticleSerialNumber)))
;  

INSERT INTO rpt_report (Oid,Ord,Code,Designation,ResourceString,Token,ReportType,Disabled) VALUES ('e0576081-c809-4a3c-9a81-13a44c8f1009',950,950,'Relatório :: Stock por Artigo','report_list_stock_article','REPORT_LIST_STOCK_ARTICLE','751c9e56-26bb-4fc8-8110-6c1e3b7c84e6',NULL);
        
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('2e71e645-4a96-4673-9680-eff3aeedf5d3', 5010, 5010,'REPORT_LIST_STOCK_ARTICLE','Relatório :: Stocks por Artigo','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('f524fc77-238a-446a-86bf-c08fecd7c8d4',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','2e71e645-4a96-4673-9680-eff3aeedf5d3');

CREATE VIEW view_articlestock AS
SELECT  afa.Oid AS afaOid,
        afa.Ord AS afaOrd,
        afa.Code AS afaCode,
        afa.Designation AS afaDesignation,
        afa.Disabled AS afaDisabled,
        asf.Oid AS asfOid,
        asf.Ord AS asfOrd,
        asf.Code AS asfCode,
        asf.Designation AS asfDesignation,
        asf.Disabled AS asfDisabled,
        art.Oid AS Article,
        art.Designation as artDesignation,
        art.Ord AS artOrd,
        art.Code AS artCode,
        art.MinimumStock AS artMinStock,
        strftime('%Y-%m-%d', stm.Date) AS stmDateDay,
        stm.Date AS Date,
        (SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE Article = art.Oid AND (Disabled = 0 OR Disabled is NULL)) as artStk,
        aum.Acronym AS aumAcronym,
        aum.Designation AS aumDesignation
        FROM 
        ((((fin_articlestock stm
        LEFT JOIN fin_article art ON ((stm.Article = art.Oid)))   
        LEFT JOIN fin_articlefamily afa ON ((afa.Oid = art.Family)))
        LEFT JOIN fin_articlesubfamily asf ON ((asf.Oid = art.SubFamily)))
        LEFT JOIN cfg_configurationunitmeasure aum ON ((aum.Oid = art.UnitMeasure)))
        GROUP BY Article
     
;        

INSERT INTO rpt_report (Oid,Ord,Code,Designation,ResourceString,Token,ReportType,Disabled) VALUES ('8c45f62b-c3aa-4c4e-84eb-c8402837dead',960,960,'Relatório :: Stock por Fornecedor','report_list_stock_supplier','REPORT_LIST_STOCK_SUPPLIER','751c9e56-26bb-4fc8-8110-6c1e3b7c84e6',NULL);
        
INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('d59f992c-310e-4fb6-afaa-47c6ad8c0b27', 5020, 5020,'REPORT_LIST_STOCK_SUPPLIER','Relatório :: Stocks por Fornecedor','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('4f22a693-ca3d-42ca-b590-1addc91857f6',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','d59f992c-310e-4fb6-afaa-47c6ad8c0b27'); 

CREATE VIEW view_articlestocksupplier AS
  SELECT stm.Oid as stmOid,
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
        art.Oid AS Article,
        art.Ord AS artOrd,
        art.Code AS artCode,
        art.CodeDealer AS artCodeDealer,
        art.Designation AS artDesignation,
        art.Disabled AS artDisabled,        
        strftime('%Y-%m-%d', stm.Date) AS stmDateDay,
        stm.Date AS Date,
        aum.Acronym AS aumAcronym,
        aum.Designation AS aumDesignation,
        stm.CreatedWhere AS afaTerminal,
        stm.CreatedBy AS afaUserDetail,
        wrh.Designation AS wrhDesignation,
        whl.Designation AS whlDesignation,
        asn.SerialNumber AS asnSerialNumber,
        wrh.Oid AS Warehouse,
        whl.Oid AS Location,
        asn.Oid AS ArticleSerialNumber,
        stm.Customer AS EntityOid,
        stm.PurchasePrice AS stmPrice,
        stm.Quantity AS stmQtd,
        ctm.Name as ctmName,
        (Select Oid FROM fin_documentfinancemaster f Where Oid = stm.DocumentMaster) as fdmOid,
        stm.DocumentNumber AS stmDocumentNumber,
        (Select Acronym FROM cfg_configurationcurrency Where Oid = fdm.Currency) AS Currency
    FROM
        ((((((((((fin_articlestock stm
        LEFT JOIN fin_article art ON ((art.Oid = stm.Article)))
        LEFT JOIN fin_articlefamily afa ON ((afa.Oid = art.Family)))
        LEFT JOIN fin_articlesubfamily asf ON ((asf.Oid = art.SubFamily)))
        LEFT JOIN cfg_configurationunitmeasure aum ON ((aum.Oid = art.UnitMeasure)))
        LEFT JOIN fin_articleserialnumber asn ON ((asn.StockMovimentIn = stm.Oid)))
        LEFT JOIN fin_articlewarehouse awh ON ((awh.Oid = asn.ArticleWarehouse)))
        LEFT JOIN fin_warehouse wrh ON ((wrh.Oid = awh.Warehouse)))
        LEFT JOIN fin_warehouselocation whl ON ((whl.Oid = awh.Location)))
        LEFT JOIN erp_customer ctm ON ((ctm.Oid = stm.Customer)))
        LEFT JOIN fin_documentfinancemaster fdm ON ((fdm.Oid = stm.DocumentMaster)))
        ;
        
INSERT INTO fin_articlestock(Oid, DocumentNumber, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);

UPDATE rpt_report SET ReportType = '751c9e56-26bb-4fc8-8110-6c1e3b7c84e6' WHERE Oid = 'c721aee9-cb5e-49c7-85ea-0dfef2540638';

INSERT INTO fin_articleserialnumber(Oid, SerialNumber, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);

INSERT INTO fin_warehouse (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);

INSERT INTO rpt_report (Oid,Ord,Code,Designation,ResourceString,Token,ReportType,Disabled) VALUES ('3af51597-d69b-4095-9115-363ebb2b0677',1000,1000,'Relatório :: Vendas Detalhadas por Taxa de IVA','report_sales_detail_group_per_vat','REPORT_SALES_DETAIL_GROUP_PER_VAT','b0992600-83c4-4932-8fae-3c491abf2a18',NULL);

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('6599822b-ce40-4bc8-983c-d060ce6809d9',4030,4030,'REPORT_SALES_DETAIL_GROUP_PER_VAT','Relatório :: Vendas Detalhadas/Agrupadas por Taxa de IVA','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('b48adb5e-98ab-488d-80ec-c51b8a9cc0e5',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','6599822b-ce40-4bc8-983c-d060ce6809d9');

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('cfe9b20c-fea1-42b5-911f-9eca5eda9332',4040,4040,'BACKOFFICE_MAN_CONFIGURATIONPRICETYPE_CREATE','Gestão de Tipo de Preço :: Criação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a', 0);

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('5b4babce-72f4-4ac5-a786-325b8c0ef6ae',4050,4050,'BACKOFFICE_MAN_CONFIGURATIONPRICETYPE_DELETE','Gestão de Tipo de Preço :: Eliminação','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a', 0);


INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('c1bf1b90-1d99-4fb8-8e96-ef860773168e',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','cfe9b20c-fea1-42b5-911f-9eca5eda9332');


INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('f1d375b8-23cb-4682-9d6d-d0fef7bc2b09',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','5b4babce-72f4-4ac5-a786-325b8c0ef6ae');


INSERT INTO rpt_report (Oid,Ord,Code,Designation,ResourceString,Token,ReportType,Disabled) VALUES ('507f5075-3b7d-4c0d-824c-0fc740b5c721',1010,1010,'Relatório :: Vendas por Taxa de IVA','report_sales_per_vat','REPORT_SALES_PER_VAT','eeb05008-d250-4898-bae0-b36980d08543',NULL);

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('6fbebe95-1e90-43ae-99f8-1bb3479ebf11', 1397,1397,'REPORT_SALES_PER_VAT','Relatório :: Vendas por Taxa IVA','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('afabdd4a-b9a8-4ca8-b1b7-6ae1190813e5',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','6fbebe95-1e90-43ae-99f8-1bb3479ebf11');


INSERT INTO rpt_report (Oid,Ord,Code,Designation,ResourceString,Token,ReportType,Disabled) VALUES ('f11e3fbe-c21a-47b5-afe6-c5a394f04cfd',1020,1020,'Relatório :: Vendas por Taxa de IVA / Classe de artigos','report_sales_per_vat_by_article_class','REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS','eeb05008-d250-4898-bae0-b36980d08543',NULL);

INSERT INTO sys_userpermissionitem (Oid,Ord,Code,Token,Designation,PermissionGroup,Disabled) VALUES ('efe21943-3448-45e5-8f8f-0c4e59a89874', 1398,1398,'REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS','Relatório :: Vendas por Taxa IVA / Classe de artigo','4c047b35-8fe5-4a4b-ac6e-59c87e0f760a',NULL);

INSERT INTO sys_userpermissionprofile (Oid,Granted,userprofile,PermissionItem) VALUES ('8eca6316-3179-4e67-b173-3991068e6c7f',1,'1626e21f-75e6-429e-b0ac-edb755e733c2','efe21943-3448-45e5-8f8f-0c4e59a89874');

INSERT INTO cfg_configurationpreferenceparameter (Oid,Disabled,Ord,Code,Token,Value,ValueTip,Required,RegEx,ResourceString,ResourceStringInfo,FormType,FormPageNo,InputType) VALUES ('a3313e64-b20a-4bb4-b626-1cf81695e7e7',NULL,610,610,'TICKET_PRINT_COMERCIAL_NAME','True','true | false',1,'RegexBoolean','prefparam_ticket_print_company_name',NULL,2,1,3);