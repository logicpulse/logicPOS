/****** 
NOTES: DONT REPLACE ABOVE BOTTOM > "Required Changes for XPO Defaults" 
RESPECT "BOF GENERATED SCRIPT" to "EOF GENERATED SCRIPT" to paste GENERATED SCRIPT from Sql Server Management
******/

/****** BOF GENERATED SCRIPT ******/

/****** Object:  Table [dbo].[cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_configurationcountry](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Code2] [nvarchar](5) NULL,
	[Code3] [nvarchar](6) NULL,
	[Designation] [nvarchar](100) NULL,
	[Capital] [nvarchar](100) NULL,
	[TLD] [nvarchar](10) NULL,
	[Currency] [nvarchar](20) NULL,
	[CurrencyCode] [nvarchar](3) NULL,
	[RegExFiscalNumber] [nvarchar](255) NULL,
	[RegExZipCode] [nvarchar](255) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_cfg_configurationcountry] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_configurationcurrency](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Acronym] [nvarchar](100) NULL,
	[Symbol] [nvarchar](10) NULL,
	[Entity] [nvarchar](512) NULL,
	[ExchangeRate] [money] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_cfg_configurationcurrency] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_configurationholidays](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Description] [nvarchar](255) NULL,
	[Year] [int] NULL,
	[Month] [int] NULL,
	[Day] [int] NULL,
	[Fixed] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_cfg_configurationholidays] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_configurationpreferenceparameter](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Token] [nvarchar](100) NULL,
	[Value] [nvarchar](max) NULL,
	[ValueTip] [nvarchar](100) NULL,
	[Required] [bit] NULL,
	[RegEx] [nvarchar](255) NULL,
	[ResourceString] [nvarchar](100) NULL,
	[ResourceStringInfo] [nvarchar](255) NULL,
	[FormType] [int] NULL,
	[FormPageNo] [int] NULL,
	[InputType] [int] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_cfg_configurationpreferenceparameter] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_configurationunitmeasure](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Acronym] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_cfg_configurationunitmeasure] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[cfg_configurationunitsize](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_cfg_configurationunitsize] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[erp_customer](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[CodeInternal] [nvarchar](30) NULL,
	[Name] [nvarchar](512) NULL,
	[Address] [nvarchar](512) NULL,
	[Locality] [nvarchar](255) NULL,
	[ZipCode] [nvarchar](100) NULL,
	[City] [nvarchar](255) NULL,
	[DateOfBirth] [nvarchar](100) NULL,
	[Phone] [nvarchar](255) NULL,
	[Fax] [nvarchar](255) NULL,
	[MobilePhone] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[WebSite] [nvarchar](255) NULL,
	[FiscalNumber] [nvarchar](100) NULL,
	[CardNumber] [nvarchar](100) NULL,
	[DiscountType] [nvarchar](100) NULL,
	[Discount] [money] NULL,
	[CardCredit] [money] NULL,
	[TotalDebt] [money] NULL,
	[TotalCredit] [money] NULL,
	[CurrentBalance] [money] NULL,
	[CreditLine] [nvarchar](100) NULL,
	[Remarks] [nvarchar](100) NULL,
	[Supplier] [bit] NULL,
	[Hidden] [bit] NULL,
	[CustomerType] [uniqueidentifier] NULL,
	[DiscountGroup] [uniqueidentifier] NULL,
	[PriceType] [uniqueidentifier] NULL,
	[Country] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_erp_customer] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[erp_customerdiscountgroup](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_erp_customerdiscountgroup] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[erp_customertype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_erp_customertype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_article]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_article](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [nvarchar](25) NULL,
	[CodeDealer] [nvarchar](25) NULL,
	[IsComposed] [bit] NULL,
	[Designation] [nvarchar](100) NULL,
	[ButtonLabel] [nvarchar](35) NULL,
	[ButtonLabelHide] [bit] NULL,
	[ButtonImage] [nvarchar](255) NULL,
	[ButtonIcon] [nvarchar](255) NULL,
	[Price1] [money] NULL,
	[Price2] [money] NULL,
	[Price3] [money] NULL,
	[Price4] [money] NULL,
	[Price5] [money] NULL,
	[Price1Promotion] [money] NULL,
	[Price2Promotion] [money] NULL,
	[Price3Promotion] [money] NULL,
	[Price4Promotion] [money] NULL,
	[Price5Promotion] [money] NULL,
	[Price1UsePromotionPrice] [bit] NULL,
	[Price2UsePromotionPrice] [bit] NULL,
	[Price3UsePromotionPrice] [bit] NULL,
	[Price4UsePromotionPrice] [bit] NULL,
	[Price5UsePromotionPrice] [bit] NULL,
	[PriceWithVat] [bit] NULL,
	[Discount] [money] NULL,
	[DefaultQuantity] [money] NULL,
	[Accounting] [money] NULL,
	[MinimumStock] [money] NULL,
	[Tare] [money] NULL,
	[Weight] [money] NULL,
	[BarCode] [nvarchar](100) NULL,
	[PVPVariable] [bit] NULL,
	[Favorite] [bit] NULL,
	[UseWeighingBalance] [bit] NULL,
	[Token1] [nvarchar](255) NULL,
	[Token2] [nvarchar](255) NULL,
	[Family] [uniqueidentifier] NULL,
	[SubFamily] [uniqueidentifier] NULL,
	[Type] [uniqueidentifier] NULL,
	[Class] [uniqueidentifier] NULL,
	[UnitMeasure] [uniqueidentifier] NULL,
	[UnitSize] [uniqueidentifier] NULL,
	[CommissionGroup] [uniqueidentifier] NULL,
	[DiscountGroup] [uniqueidentifier] NULL,
	[VatOnTable] [uniqueidentifier] NULL,
	[VatDirectSelling] [uniqueidentifier] NULL,
	[VatExemptionReason] [uniqueidentifier] NULL,
	[Printer] [uniqueidentifier] NULL,
	[Template] [uniqueidentifier] NULL,
	[TemplateBarCode] [uniqueidentifier] NULL,
	[UniqueArticles] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_article] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articlecomposition]    Script Date: 14/01/2021 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articlecomposition](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Article] [uniqueidentifier] NULL,
	[ArticleChild] [uniqueidentifier] NULL,
	[Quantity] [decimal] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articlecomposition] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articleserialnumber]  Script Date: 01/03/2021 12:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articleserialnumber](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Article] [uniqueidentifier] NULL,
	[SerialNumber] [nvarchar](max) NULL,
	[ArticleWarehouse] [uniqueidentifier] NULL,
	[StockMovimentIn] [uniqueidentifier] NULL,
	[StockMovimentOut] [uniqueidentifier] NULL,
	[IsSold] [bit] NULL,
	[Status] [int] NULL,
	[Quantity] [decimal] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articleserialnumber] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articlewarehouse]  Script Date: 01/03/2021 12:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articlewarehouse](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Article] [uniqueidentifier] NULL,
	[Warehouse] [uniqueidentifier] NULL,
	[Location] [uniqueidentifier] NULL,
	[ArticleSerialNumber] [uniqueidentifier] NULL,
	[Quantity] [decimal] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articlewarehouse] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articlecompositionserialnumber]  Script Date: 01/03/2021 12:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articlecompositionserialnumber](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,	
	[Ord] [numeric](10, 0) NULL,
	[Code] [nvarchar](25) NULL,	
	[ArticleSerialNumber] [uniqueidentifier] NULL,
	[ArticleSerialNumberChild] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articlecompositionserialnumber] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_warehouse]  Script Date: 01/03/2021 12:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_warehouse](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [nvarchar](25) NULL,
	[Designation] [nvarchar](100) NULL,
	[Location] [uniqueidentifier] NULL,
	[Quantity] [decimal] NULL,
	[IsDefault] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_warehouse] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_warehouselocation]  Script Date: 01/03/2021 12:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_warehouselocation](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [nvarchar](25) NULL,
	[Designation] [nvarchar](100) NULL,
	[Warehouse] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_warehouselocation] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articleclass](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Acronym] [nvarchar](1) NULL,
	[WorkInStock] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articleclass] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articlefamily](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ButtonLabel] [nvarchar](35) NULL,
	[ButtonLabelHide] [bit] NULL,
	[ButtonImage] [nvarchar](255) NULL,
	[ButtonIcon] [nvarchar](255) NULL,
	[CommissionGroup] [uniqueidentifier] NULL,
	[DiscountGroup] [uniqueidentifier] NULL,
	[Printer] [uniqueidentifier] NULL,
	[Template] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articlefamily] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articlestock](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Date] [datetime] NULL,
	[Customer] [uniqueidentifier] NULL,
	[DocumentNumber] [nvarchar](50) NULL,
	[Article] [uniqueidentifier] NULL,
	[Quantity] [money] NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[DocumentDetail] [uniqueidentifier] NULL,
	[ArticleSerialNumber] [uniqueidentifier] NULL,
	[PurchasePrice] [money] NULL, 
	[AttachedFile] [varbinary](MAX) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articlestock] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articlesubfamily](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ButtonLabel] [nvarchar](35) NULL,
	[ButtonLabelHide] [bit] NULL,
	[ButtonImage] [nvarchar](255) NULL,
	[ButtonIcon] [nvarchar](255) NULL,
	[Family] [uniqueidentifier] NULL,
	[CommissionGroup] [uniqueidentifier] NULL,
	[DiscountGroup] [uniqueidentifier] NULL,
	[VatOnTable] [uniqueidentifier] NULL,
	[VatDirectSelling] [uniqueidentifier] NULL,
	[Printer] [uniqueidentifier] NULL,
	[Template] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articlesubfamily] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_articletype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[HavePrice] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_articletype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_configurationpaymentcondition](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Acronym] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_configurationpaymentcondition] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_configurationpaymentmethod](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Token] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[ResourceString] [nvarchar](100) NULL,
	[ButtonIcon] [nvarchar](255) NULL,
	[Acronym] [nvarchar](100) NULL,
	[AllowPayback] [nvarchar](100) NULL,
	[Symbol] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_configurationpaymentmethod] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_configurationpricetype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[EnumValue] [int] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_configurationpricetype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_configurationvatexemptionreason](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](60) NULL,
	[Acronym] [nvarchar](3) NULL,
	[StandardApplicable] [nvarchar](512) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_configurationvatexemptionreason] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_configurationvatrate](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Value] [money] NULL,
	[ReasonCode] [nvarchar](100) NULL,
	[TaxType] [nvarchar](3) NULL,
	[TaxCode] [nvarchar](10) NULL,
	[TaxCountryRegion] [nvarchar](5) NULL,
	[TaxExpirationDate] [datetime] NULL,
	[TaxDescription] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_configurationvatrate] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancecommission](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Date] [datetime] NULL,
	[Commission] [money] NULL,
	[Total] [money] NULL,
	[CommissionGroup] [uniqueidentifier] NULL,
	[FinanceMaster] [uniqueidentifier] NULL,
	[FinanceDetail] [uniqueidentifier] NULL,
	[UserDetail] [uniqueidentifier] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancecommission] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancedetail](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[Quantity] [money] NULL,
	[UnitMeasure] [nvarchar](35) NULL,
	[Price] [money] NULL,
	[Vat] [money] NULL,
	[VatExemptionReasonDesignation] [nvarchar](255) NULL,
	[Discount] [money] NULL,
	[TotalNet] [money] NULL,
	[TotalGross] [money] NULL,
	[TotalDiscount] [money] NULL,
	[TotalTax] [money] NULL,
	[TotalFinal] [money] NULL,
	[PriceType] [int] NULL,
	[PriceFinal] [money] NULL,
	[Token1] [nvarchar](255) NULL,
	[Token2] [nvarchar](255) NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[Article] [uniqueidentifier] NULL,
	[VatRate] [uniqueidentifier] NULL,
	[VatExemptionReason] [uniqueidentifier] NULL,
	[SerialNumber] [nvarchar](MAX) NULL,
	[Warehouse] [nvarchar](MAX) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancedetail] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancedetailorderreference](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[OriginatingON] [nvarchar](60) NULL,
	[OrderDate] [datetime] NULL,
	[DocumentDetail] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancedetailorderreference] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancedetailreference](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[Reference] [nvarchar](60) NULL,
	[Reason] [nvarchar](max) NULL,
	[DocumentDetail] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancedetailreference] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancemaster](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Date] [datetime] NULL,
	[DocumentNumber] [nvarchar](50) NULL,
	[DocumentStatusStatus] [nvarchar](1) NULL,
	[DocumentStatusDate] [nvarchar](19) NULL,
	[DocumentStatusReason] [nvarchar](50) NULL,
	[DocumentStatusUser] [nvarchar](30) NULL,
	[SourceBilling] [nvarchar](1) NULL,
	[Hash] [nvarchar](172) NULL,
	[HashControl] [nvarchar](40) NULL,
	[DocumentDate] [nvarchar](19) NULL,
	[SelfBillingIndicator] [int] NULL,
	[CashVatSchemeIndicator] [int] NULL,
	[ThirdPartiesBillingIndicator] [int] NULL,
	[DocumentCreatorUser] [nvarchar](30) NULL,
	[EACCode] [nvarchar](5) NULL,
	[SystemEntryDate] [nvarchar](50) NULL,
	[TransactionID] [nvarchar](70) NULL,
	[ShipToDeliveryID] [nvarchar](255) NULL,
	[ShipToDeliveryDate] [datetime] NULL,
	[ShipToWarehouseID] [nvarchar](50) NULL,
	[ShipToLocationID] [nvarchar](30) NULL,
	[ShipToBuildingNumber] [nvarchar](10) NULL,
	[ShipToStreetName] [nvarchar](90) NULL,
	[ShipToAddressDetail] [nvarchar](100) NULL,
	[ShipToCity] [nvarchar](50) NULL,
	[ShipToPostalCode] [nvarchar](20) NULL,
	[ShipToRegion] [nvarchar](50) NULL,
	[ShipToCountry] [nvarchar](5) NULL,
	[ShipFromDeliveryID] [nvarchar](255) NULL,
	[ShipFromDeliveryDate] [datetime] NULL,
	[ShipFromWarehouseID] [nvarchar](50) NULL,
	[ShipFromLocationID] [nvarchar](30) NULL,
	[ShipFromBuildingNumber] [nvarchar](10) NULL,
	[ShipFromStreetName] [nvarchar](90) NULL,
	[ShipFromAddressDetail] [nvarchar](100) NULL,
	[ShipFromCity] [nvarchar](50) NULL,
	[ShipFromPostalCode] [nvarchar](20) NULL,
	[ShipFromRegion] [nvarchar](50) NULL,
	[ShipFromCountry] [nvarchar](5) NULL,
	[MovementStartTime] [datetime] NULL,
	[MovementEndTime] [datetime] NULL,
	[TotalNet] [money] NULL,
	[TotalGross] [money] NULL,
	[TotalDiscount] [money] NULL,
	[TotalTax] [money] NULL,
	[TotalFinal] [money] NULL,
	[TotalFinalRound] [money] NULL,
	[TotalDelivery] [money] NULL,
	[TotalChange] [money] NULL,
	[ExternalDocument] [nvarchar](50) NULL,
	[Discount] [money] NULL,
	[DiscountFinancial] [money] NULL,
	[ExchangeRate] [money] NULL,
	[EntityOid] [uniqueidentifier] NULL,
	[EntityInternalCode] [nvarchar](30) NULL,
	[EntityName] [nvarchar](512) NULL,
	[EntityAddress] [nvarchar](512) NULL,
	[EntityLocality] [nvarchar](255) NULL,
	[EntityZipCode] [nvarchar](100) NULL,
	[EntityCity] [nvarchar](255) NULL,
	[EntityCountry] [nvarchar](5) NULL,
	[EntityCountryOid] [uniqueidentifier] NULL,
	[EntityFiscalNumber] [nvarchar](100) NULL,
	[Payed] [bit] NULL,
	[PayedDate] [datetime] NULL,
	[Printed] [bit] NULL,
	[SourceOrderMain] [uniqueidentifier] NULL,
	[DocumentParent] [uniqueidentifier] NULL,
	[DocumentChild] [uniqueidentifier] NULL,
	[ATDocCodeID] [nvarchar](200) NULL,
	[ATValidAuditResult] [uniqueidentifier] NULL,
	[ATResendDocument] [bit] NULL,
	[ATCUD] [nvarchar](200) NULL,
	[ATDocQRCode] [nvarchar](MAX) NULL,
	[DocumentType] [uniqueidentifier] NULL,
	[DocumentSerie] [uniqueidentifier] NULL,
	[PaymentMethod] [uniqueidentifier] NULL,
	[PaymentCondition] [uniqueidentifier] NULL,
	[Currency] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancemaster] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancemasterpayment](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[CreditAmount] [money] NULL,
	[DebitAmount] [money] NULL,
	[DocumentFinanceMaster] [uniqueidentifier] NULL,
	[DocumentFinancePayment] [uniqueidentifier] NULL,
	[DocumentFinanceMasterCreditNote] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancemasterpayment] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancemastertotal](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Value] [money] NULL,
	[Total] [money] NULL,
	[TotalBase] [money] NULL,
	[TotalType] [int] NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancemastertotal] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancepayment](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[PaymentRefNo] [nvarchar](60) NULL,
	[TransactionID] [nvarchar](70) NULL,
	[TransactionDate] [nvarchar](19) NULL,
	[PaymentType] [nvarchar](2) NULL,
	[PaymentStatus] [nvarchar](1) NULL,
	[PaymentStatusDate] [nvarchar](50) NULL,
	[Reason] [nvarchar](50) NULL,
	[DocumentStatusSourceID] [nvarchar](30) NULL,
	[SourcePayment] [nvarchar](1) NULL,
	[PaymentMechanism] [nvarchar](2) NULL,
	[PaymentAmount] [money] NULL,
	[PaymentDate] [nvarchar](19) NULL,
	[SourceID] [nvarchar](30) NULL,
	[SystemEntryDate] [nvarchar](50) NULL,
	[TaxPayable] [money] NULL,
	[NetTotal] [money] NULL,
	[GrossTotal] [money] NULL,
	[SettlementAmount] [money] NULL,
	[CurrencyCode] [nvarchar](3) NULL,
	[CurrencyAmount] [money] NULL,
	[ExchangeRate] [money] NULL,
	[WithholdingTaxAmount] [money] NULL,
	[EntityOid] [uniqueidentifier] NULL,
	[EntityInternalCode] [nvarchar](30) NULL,
	[DocumentDate] [nvarchar](19) NULL,
	[ExtendedValue] [nvarchar](1024) NULL,
	[DocumentType] [uniqueidentifier] NULL,
	[DocumentSerie] [uniqueidentifier] NULL,
	[PaymentMethod] [uniqueidentifier] NULL,
	[Currency] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancepayment] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinanceseries](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[NextDocumentNumber] [int] NULL,
	[DocumentNumberRangeBegin] [int] NULL,
	[DocumentNumberRangeEnd] [int] NULL,
	[Acronym] [nvarchar](100) NULL,
	[DocumentType] [uniqueidentifier] NULL,
	[FiscalYear] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinanceseries] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinancetype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Acronym] [nvarchar](4) NULL,
	[AcronymLastSerie] [int] NULL,
	[ResourceString] [nvarchar](100) NULL,
	[ResourceStringReport] [nvarchar](100) NULL,
	[Payed] [bit] NULL,
	[Credit] [bit] NULL,
	[CreditDebit] [int] NULL,
	[PrintCopies] [int] NULL,
	[PrintRequestMotive] [bit] NULL,
	[PrintRequestConfirmation] [bit] NULL,
	[PrintOpenDrawer] [bit] NULL,
	[WayBill] [bit] NULL,
	[WsAtDocument] [bit] NULL,
	[SaftAuditFile] [bit] NULL,
	[SaftDocumentType] [int] NULL,
	[StockMode] [int] NULL,
	[Printer] [uniqueidentifier] NULL,
	[Template] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinancetype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinanceyears](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[FiscalYear] [int] NULL,
	[Acronym] [nvarchar](100) NULL,
	[SeriesForEachTerminal] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinanceyears] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentfinanceyearserieterminal](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[FiscalYear] [uniqueidentifier] NULL,
	[DocumentType] [uniqueidentifier] NULL,
	[Serie] [uniqueidentifier] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[Printer] [uniqueidentifier] NULL,
	[Template] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentfinanceyearserieterminal] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentorderdetail](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[Quantity] [money] NULL,
	[UnitMeasure] [nvarchar](35) NULL,
	[Price] [money] NULL,
	[Discount] [money] NULL,
	[Vat] [money] NULL,
	[VatExemptionReason] [uniqueidentifier] NULL,
	[TotalGross] [money] NULL,
	[TotalDiscount] [money] NULL,
	[TotalTax] [money] NULL,
	[TotalFinal] [money] NULL,
	[Token1] [nvarchar](255) NULL,
	[Token2] [nvarchar](255) NULL,
	[OrderTicket] [uniqueidentifier] NULL,
	[Article] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentorderdetail] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentordermain](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[DateStart] [datetime] NULL,
	[OrderStatus] [int] NULL,
	[PlaceTable] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentordermain] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fin_documentorderticket](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[TicketId] [int] NULL,
	[DateStart] [datetime] NULL,
	[PriceType] [int] NULL,
	[Discount] [money] NULL,
	[OrderMain] [uniqueidentifier] NULL,
	[PlaceTable] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_fin_documentorderticket] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationcashregister](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Printer] [nvarchar](100) NULL,
	[Drawer] [nvarchar](100) NULL,
	[AutomaticDrawer] [nvarchar](100) NULL,
	[ActiveSales] [nvarchar](100) NULL,
	[AllowChargeBacks] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationcashregister] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationdevice](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Type] [nvarchar](100) NULL,
	[Properties] [nvarchar](max) NULL,
	[PlaceTerminal] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationdevice] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationkeyboard](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Language] [nvarchar](100) NULL,
	[VirtualKeyboard] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationkeyboard] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationmaintenance](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Date] [nvarchar](100) NULL,
	[Time] [nvarchar](100) NULL,
	[PasswordAccess] [nvarchar](100) NULL,
	[Remarks] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationmaintenance] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationplace](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ButtonImage] [nvarchar](255) NULL,
	[TypeSubtotal] [nvarchar](100) NULL,
	[AccountType] [nvarchar](100) NULL,
	[OrderPrintMode] [int] NULL,
	[PriceType] [uniqueidentifier] NULL,
	[MovementType] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationplace] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationplacemovementtype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[VatDirectSelling] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationplacemovementtype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationplacetable](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ButtonImage] [nvarchar](255) NULL,
	[Discount] [money] NULL,
	[TableStatus] [int] NULL,
	[TotalOpen] [money] NULL,
	[DateTableOpen] [datetime] NULL,
	[DateTableClosed] [datetime] NULL,
	[Place] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationplacetable] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_configurationplaceterminal](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[HardwareId] [nvarchar](120) NULL,
	[InputReaderTimerInterval] [numeric](10, 0) NULL,
	[Place] [uniqueidentifier] NULL,
	[Printer] [uniqueidentifier] NULL,
	[ThermalPrinter] [uniqueidentifier] NULL,
	[BarcodeReader] [uniqueidentifier] NULL,
	[CardReader] [uniqueidentifier] NULL,
	[PoleDisplay] [uniqueidentifier] NULL,
	[WeighingMachine] [uniqueidentifier] NULL,
	[TemplateTicket] [uniqueidentifier] NULL,
	[TemplateTablesConsult] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_configurationplaceterminal] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_usercommissiongroup](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Commission] [money] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_usercommissiongroup] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_worksessionmovement](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Date] [datetime] NULL,
	[MovementAmount] [money] NULL,
	[Description] [nvarchar](255) NULL,
	[UserDetail] [uniqueidentifier] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[DocumentFinanceMaster] [uniqueidentifier] NULL,
	[DocumentFinancePayment] [uniqueidentifier] NULL,
	[DocumentFinanceType] [uniqueidentifier] NULL,
	[PaymentMethod] [uniqueidentifier] NULL,
	[WorkSessionPeriod] [uniqueidentifier] NULL,
	[WorkSessionMovementType] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_worksessionmovement] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_worksessionmovementtype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Token] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[ResourceString] [nvarchar](100) NULL,
	[ButtonIcon] [nvarchar](255) NULL,
	[CashDrawerMovement] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_worksessionmovementtype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_worksessionperiod](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[PeriodType] [int] NULL,
	[SessionStatus] [int] NULL,
	[Designation] [nvarchar](100) NULL,
	[DateStart] [datetime] NULL,
	[DateEnd] [datetime] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[Parent] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_worksessionperiod] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[pos_worksessionperiodtotal](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[PaymentMethod] [uniqueidentifier] NULL,
	[Total] [money] NULL,
	[Period] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_pos_worksessionperiodtotal] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rpt_report](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ResourceString] [nvarchar](100) NULL,
	[Token] [nvarchar](100) NULL,
	[FileName] [nvarchar](100) NULL,
	[ParameterFields] [nvarchar](100) NULL,
	[AuthorType] [int] NULL,
	[ReportType] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_rpt_report] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[rpt_reporttype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ResourceString] [nvarchar](100) NULL,
	[MenuIcon] [nvarchar](255) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_rpt_reporttype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_configurationinputreader](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[ReaderSizes] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_configurationinputreader] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_configurationpoledisplay](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[VID] [nvarchar](100) NULL,
	[PID] [nvarchar](100) NULL,
	[EndPoint] [nvarchar](100) NULL,
	[CodeTable] [nvarchar](100) NULL,
	[COM] [nvarchar](10) NULL,
	[DisplayCharactersPerLine] [numeric](10, 0) NULL,
	[GoToStandByInSeconds] [numeric](10, 0) NULL,
	[StandByLine1] [nvarchar](100) NULL,
	[StandByLine2] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_configurationpoledisplay] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_configurationprinters](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[NetworkName] [nvarchar](100) NULL,
	[ThermalEncoding] [nvarchar](100) NULL,
	[ThermalPrintLogo] [bit] NULL,
	[ThermalImageCompanyLogo] [nvarchar](100) NULL,
	[ThermalMaxCharsPerLineNormal] [int] NULL,
	[ThermalMaxCharsPerLineNormalBold] [int] NULL,
	[ThermalMaxCharsPerLineSmall] [int] NULL,
	[ThermalCutCommand] [nvarchar](100) NULL,
	[ThermalOpenDrawerValueM] [int] NULL,
	[ThermalOpenDrawerValueT1] [int] NULL,
	[ThermalOpenDrawerValueT2] [int] NULL,
	[ShowInDialog] [bit] NULL,
	[PrinterType] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_configurationprinters] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_configurationprinterstemplates](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[FileTemplate] [nvarchar](100) NULL,
	[FinancialTemplate] [bit] NULL,
	[IsBarCode] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_configurationprinterstemplates] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_configurationprinterstype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Token] [nvarchar](100) NULL,
	[ThermalPrinter] [bit] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_configurationprinterstype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_configurationweighingmachine](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[PortName] [nvarchar](4) NULL,
	[BaudRate] [numeric](10, 0) NULL,
	[Parity] [nvarchar](5) NULL,
	[StopBits] [nvarchar](12) NULL,
	[DataBits] [numeric](10, 0) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_configurationweighingmachine] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemaudit](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Date] [datetime] NULL,
	[Description] [nvarchar](255) NULL,
	[UserDetail] [uniqueidentifier] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[AuditType] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemaudit] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemauditat](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Date] [datetime] NULL,
	[Type] [int] NULL,
	[PostData] [nvarchar](max) NULL,
	[ReturnCode] [int] NULL,
	[ReturnMessage] [nvarchar](max) NULL,
	[ReturnRaw] [nvarchar](max) NULL,
	[DocumentNumber] [nvarchar](100) NULL,
	[ATDocCodeID] [nvarchar](100) NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemauditat] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemaudittype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Token] [nvarchar](100) NULL,
	[Designation] [nvarchar](100) NULL,
	[ResourceString] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemaudittype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systembackup](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[DataBaseType] [int] NULL,
	[Version] [numeric](10, 0) NULL,
	[FileName] [nvarchar](255) NULL,
	[FileNamePacked] [nvarchar](255) NULL,
	[FilePath] [nvarchar](100) NULL,
	[FileHash] [nvarchar](255) NULL,
	[User] [uniqueidentifier] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systembackup] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemnotification](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Message] [nvarchar](max) NULL,
	[Readed] [bit] NULL,
	[DateRead] [datetime] NULL,
	[UserTarget] [uniqueidentifier] NULL,
	[TerminalTarget] [uniqueidentifier] NULL,
	[UserLastRead] [uniqueidentifier] NULL,
	[TerminalLastRead] [uniqueidentifier] NULL,
	[NotificationType] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemnotification] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemnotificationdocumentmaster](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Notification] [uniqueidentifier] NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemnotificationdocumentmaster] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemnotificationtype](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[Message] [nvarchar](512) NULL,
	[WarnDaysBefore] [int] NULL,
	[UserTarget] [uniqueidentifier] NULL,
	[TerminalTarget] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemnotificationtype] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_systemprint](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Date] [datetime] NULL,
	[Designation] [nvarchar](100) NULL,
	[CopyNames] [nvarchar](50) NULL,
	[PrintCopies] [int] NULL,
	[PrintMotive] [nvarchar](255) NULL,
	[SecondPrint] [bit] NULL,
	[UserDetail] [uniqueidentifier] NULL,
	[Terminal] [uniqueidentifier] NULL,
	[DocumentMaster] [uniqueidentifier] NULL,
	[DocumentPayment] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_systemprint] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_userdetail](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[CodeInternal] [nvarchar](30) NULL,
	[Name] [nvarchar](512) NULL,
	[Residence] [nvarchar](512) NULL,
	[Locality] [nvarchar](255) NULL,
	[ZipCode] [nvarchar](100) NULL,
	[City] [nvarchar](255) NULL,
	[DateOfContract] [nvarchar](100) NULL,
	[Phone] [nvarchar](255) NULL,
	[MobilePhone] [nvarchar](255) NULL,
	[Email] [nvarchar](255) NULL,
	[FiscalNumber] [nvarchar](100) NULL,
	[Language] [nvarchar](100) NULL,
	[AssignedSeating] [nvarchar](100) NULL,
	[AccessPin] [nvarchar](255) NULL,
	[AccessCardNumber] [nvarchar](100) NULL,
	[Login] [nvarchar](100) NULL,
	[Password] [nvarchar](255) NULL,
	[PasswordReset] [bit] NULL,
	[PasswordResetDate] [datetime] NULL,
	[BaseConsumption] [nvarchar](100) NULL,
	[BaseOffers] [nvarchar](100) NULL,
	[PVPOffers] [nvarchar](100) NULL,
	[Remarks] [nvarchar](100) NULL,
	[ButtonImage] [nvarchar](255) NULL,
	[Profile] [uniqueidentifier] NULL,
	[CommissionGroup] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_userdetail] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_userpermissiongroup](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_userpermissiongroup] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_userpermissionitem](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Token] [nvarchar](100) NULL,
	[Designation] [nvarchar](200) NULL,
	[PermissionGroup] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_userpermissionitem] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_userpermissionprofile](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Granted] [bit] NULL,
	[UserProfile] [uniqueidentifier] NULL,
	[PermissionItem] [uniqueidentifier] NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_userpermissionprofile] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_userprofile](
	[Oid] [uniqueidentifier] ROWGUIDCOL  NOT NULL,
	[Disabled] [bit] NULL,
	[Notes] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[CreatedBy] [uniqueidentifier] NULL,
	[CreatedWhere] [uniqueidentifier] NULL,
	[UpdatedAt] [datetime] NULL,
	[UpdatedBy] [uniqueidentifier] NULL,
	[UpdatedWhere] [uniqueidentifier] NULL,
	[DeletedAt] [datetime] NULL,
	[DeletedBy] [uniqueidentifier] NULL,
	[DeletedWhere] [uniqueidentifier] NULL,
	[Ord] [numeric](10, 0) NULL,
	[Code] [numeric](10, 0) NULL,
	[Designation] [nvarchar](100) NULL,
	[AccessPassword] [nvarchar](50) NULL,
	[OptimisticLockField] [int] NULL,
 CONSTRAINT [PK_sys_userprofile] PRIMARY KEY CLUSTERED 
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Index [iCode_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iCode2_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode2_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[Code2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iCode3_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode3_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[Code3] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_cfg_configurationcountry]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_cfg_configurationcountry] ON [dbo].[cfg_configurationcountry]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_cfg_configurationcurrency]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_cfg_configurationcurrency] ON [dbo].[cfg_configurationcurrency]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_cfg_configurationholidays]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_cfg_configurationholidays] ON [dbo].[cfg_configurationholidays]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iResourceString_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iResourceString_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[ResourceString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_cfg_configurationpreferenceparameter]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_cfg_configurationpreferenceparameter] ON [dbo].[cfg_configurationpreferenceparameter]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iAcronym_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iAcronym_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[Acronym] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_cfg_configurationunitmeasure]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_cfg_configurationunitmeasure] ON [dbo].[cfg_configurationunitmeasure]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_cfg_configurationunitsize]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_cfg_configurationunitsize] ON [dbo].[cfg_configurationunitsize]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iCardNumber_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCardNumber_erp_customer] ON [dbo].[erp_customer]
(
	[CardNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iCodeInternal_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCodeInternal_erp_customer] ON [dbo].[erp_customer]
(
	[CodeInternal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCountry_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCountry_erp_customer] ON [dbo].[erp_customer]
(
	[Country] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_erp_customer] ON [dbo].[erp_customer]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_erp_customer] ON [dbo].[erp_customer]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCustomerType_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCustomerType_erp_customer] ON [dbo].[erp_customer]
(
	[CustomerType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_erp_customer] ON [dbo].[erp_customer]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_erp_customer] ON [dbo].[erp_customer]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDiscountGroup_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDiscountGroup_erp_customer] ON [dbo].[erp_customer]
(
	[DiscountGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_erp_customer] ON [dbo].[erp_customer]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPriceType_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPriceType_erp_customer] ON [dbo].[erp_customer]
(
	[PriceType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_erp_customer] ON [dbo].[erp_customer]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_erp_customer]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_erp_customer] ON [dbo].[erp_customer]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_erp_customerdiscountgroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_erp_customerdiscountgroup] ON [dbo].[erp_customerdiscountgroup]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_erp_customertype] ON [dbo].[erp_customertype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_erp_customertype] ON [dbo].[erp_customertype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_erp_customertype] ON [dbo].[erp_customertype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_erp_customertype] ON [dbo].[erp_customertype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_erp_customertype] ON [dbo].[erp_customertype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_erp_customertype] ON [dbo].[erp_customertype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_erp_customertype] ON [dbo].[erp_customertype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_erp_customertype] ON [dbo].[erp_customertype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_erp_customertype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_erp_customertype] ON [dbo].[erp_customertype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iBarCode_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iBarCode_fin_article] ON [dbo].[fin_article]
(
	[BarCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iClass_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iClass_fin_article] ON [dbo].[fin_article]
(
	[Class] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iCode_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_article] ON [dbo].[fin_article]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCommissionGroup_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCommissionGroup_fin_article] ON [dbo].[fin_article]
(
	[CommissionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_article] ON [dbo].[fin_article]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_article] ON [dbo].[fin_article]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_article] ON [dbo].[fin_article]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_article] ON [dbo].[fin_article]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_article] ON [dbo].[fin_article]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDiscountGroup_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDiscountGroup_fin_article] ON [dbo].[fin_article]
(
	[DiscountGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFamily_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFamily_fin_article] ON [dbo].[fin_article]
(
	[Family] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_article] ON [dbo].[fin_article]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinter_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinter_fin_article] ON [dbo].[fin_article]
(
	[Printer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iSubFamily_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iSubFamily_fin_article] ON [dbo].[fin_article]
(
	[SubFamily] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplate_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplate_fin_article] ON [dbo].[fin_article]
(
	[Template] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iType_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iType_fin_article] ON [dbo].[fin_article]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUnitMeasure_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUnitMeasure_fin_article] ON [dbo].[fin_article]
(
	[UnitMeasure] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUnitSize_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUnitSize_fin_article] ON [dbo].[fin_article]
(
	[UnitSize] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_article] ON [dbo].[fin_article]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_article] ON [dbo].[fin_article]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatDirectSelling_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatDirectSelling_fin_article] ON [dbo].[fin_article]
(
	[VatDirectSelling] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatExemptionReason_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatExemptionReason_fin_article] ON [dbo].[fin_article]
(
	[VatExemptionReason] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatOnTable_fin_article]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatOnTable_fin_article] ON [dbo].[fin_article]
(
	[VatOnTable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iAcronym_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iAcronym_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[Acronym] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_articleclass]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_articleclass] ON [dbo].[fin_articleclass]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCommissionGroup_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCommissionGroup_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[CommissionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDiscountGroup_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDiscountGroup_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[DiscountGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinter_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinter_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[Printer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplate_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplate_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[Template] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_articlefamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_articlefamily] ON [dbo].[fin_articlefamily]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iArticle_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iArticle_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[Article] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCustomer_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCustomer_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[Customer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentDetail_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentDetail_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[DocumentDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_articlestock]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_articlestock] ON [dbo].[fin_articlestock]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCommissionGroup_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCommissionGroup_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[CommissionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDiscountGroup_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDiscountGroup_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[DiscountGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFamily_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFamily_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[Family] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinter_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinter_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[Printer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplate_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplate_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[Template] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatDirectSelling_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatDirectSelling_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[VatDirectSelling] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatOnTable_fin_articlesubfamily]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatOnTable_fin_articlesubfamily] ON [dbo].[fin_articlesubfamily]
(
	[VatOnTable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_articletype] ON [dbo].[fin_articletype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_articletype] ON [dbo].[fin_articletype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_articletype] ON [dbo].[fin_articletype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_articletype] ON [dbo].[fin_articletype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_articletype] ON [dbo].[fin_articletype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_articletype] ON [dbo].[fin_articletype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_articletype] ON [dbo].[fin_articletype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_articletype] ON [dbo].[fin_articletype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_articletype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_articletype] ON [dbo].[fin_articletype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_configurationpaymentcondition]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_configurationpaymentcondition] ON [dbo].[fin_configurationpaymentcondition]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iResourceString_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iResourceString_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[ResourceString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_configurationpaymentmethod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_configurationpaymentmethod] ON [dbo].[fin_configurationpaymentmethod]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iEnumValue_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iEnumValue_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[EnumValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_configurationpricetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_configurationpricetype] ON [dbo].[fin_configurationpricetype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_configurationvatexemptionreason]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_configurationvatexemptionreason] ON [dbo].[fin_configurationvatexemptionreason]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_configurationvatrate]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_configurationvatrate] ON [dbo].[fin_configurationvatrate]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCommissionGroup_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCommissionGroup_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[CommissionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFinanceDetail_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFinanceDetail_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[FinanceDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFinanceMaster_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFinanceMaster_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[FinanceMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserDetail_fin_documentfinancecommission]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserDetail_fin_documentfinancecommission] ON [dbo].[fin_documentfinancecommission]
(
	[UserDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iArticle_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iArticle_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[Article] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatExemptionReason_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatExemptionReason_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[VatExemptionReason] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iVatRate_fin_documentfinancedetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iVatRate_fin_documentfinancedetail] ON [dbo].[fin_documentfinancedetail]
(
	[VatRate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentDetail_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentDetail_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[DocumentDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancedetailorderreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancedetailorderreference] ON [dbo].[fin_documentfinancedetailorderreference]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentDetail_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentDetail_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[DocumentDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancedetailreference]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancedetailreference] ON [dbo].[fin_documentfinancedetailreference]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iATValidAuditResult_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iATValidAuditResult_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[ATValidAuditResult] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCurrency_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCurrency_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[Currency] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentChild_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentChild_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DocumentChild] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDocumentNumber_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDocumentNumber_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DocumentNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentParent_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentParent_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DocumentParent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentSerie_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentSerie_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DocumentSerie] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentType_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentType_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[DocumentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPaymentCondition_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPaymentCondition_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[PaymentCondition] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPaymentMethod_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPaymentMethod_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[PaymentMethod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iSourceOrderMain_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iSourceOrderMain_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[SourceOrderMain] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancemaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancemaster] ON [dbo].[fin_documentfinancemaster]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentFinanceMaster_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentFinanceMaster_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[DocumentFinanceMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentFinanceMasterCreditNote_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentFinanceMasterCreditNote_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[DocumentFinanceMasterCreditNote] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentFinancePayment_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentFinancePayment_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[DocumentFinancePayment] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancemasterpayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancemasterpayment] ON [dbo].[fin_documentfinancemasterpayment]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancemastertotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancemastertotal] ON [dbo].[fin_documentfinancemastertotal]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCurrency_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCurrency_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[Currency] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentSerie_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentSerie_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[DocumentSerie] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentType_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentType_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[DocumentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPaymentMethod_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPaymentMethod_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[PaymentMethod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancepayment]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancepayment] ON [dbo].[fin_documentfinancepayment]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iAcronym_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iAcronym_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[Acronym] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentType_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentType_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[DocumentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFiscalYear_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFiscalYear_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[FiscalYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinanceseries]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinanceseries] ON [dbo].[fin_documentfinanceseries]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinter_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinter_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[Printer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplate_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplate_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[Template] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinancetype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinancetype] ON [dbo].[fin_documentfinancetype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iAcronym_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iAcronym_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[Acronym] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFiscalYear_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFiscalYear_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[FiscalYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinanceyears]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinanceyears] ON [dbo].[fin_documentfinanceyears]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentType_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentType_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[DocumentType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iFiscalYear_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iFiscalYear_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[FiscalYear] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinter_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinter_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[Printer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iSerie_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iSerie_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[Serie] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplate_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplate_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[Template] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentfinanceyearserieterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentfinanceyearserieterminal] ON [dbo].[fin_documentfinanceyearserieterminal]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iArticle_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iArticle_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[Article] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOrderTicket_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iOrderTicket_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[OrderTicket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentorderdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentorderdetail] ON [dbo].[fin_documentorderdetail]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPlaceTable_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPlaceTable_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[PlaceTable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentordermain]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentordermain] ON [dbo].[fin_documentordermain]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOrderMain_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iOrderMain_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[OrderMain] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPlaceTable_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPlaceTable_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[PlaceTable] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_fin_documentorderticket]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_fin_documentorderticket] ON [dbo].[fin_documentorderticket]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationcashregister]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationcashregister] ON [dbo].[pos_configurationcashregister]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPlaceTerminal_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPlaceTerminal_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[PlaceTerminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationdevice]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationdevice] ON [dbo].[pos_configurationdevice]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationkeyboard]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationkeyboard] ON [dbo].[pos_configurationkeyboard]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationmaintenance]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationmaintenance] ON [dbo].[pos_configurationmaintenance]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iMovementType_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iMovementType_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[MovementType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPriceType_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPriceType_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[PriceType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationplace]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationplace] ON [dbo].[pos_configurationplace]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationplacemovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationplacemovementtype] ON [dbo].[pos_configurationplacemovementtype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPlace_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPlace_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[Place] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationplacetable]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationplacetable] ON [dbo].[pos_configurationplacetable]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iBarcodeReader_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iBarcodeReader_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[BarcodeReader] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCardReader_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCardReader_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[CardReader] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iHardwareId_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iHardwareId_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[HardwareId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPlace_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPlace_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[Place] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPoleDisplay_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPoleDisplay_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[PoleDisplay] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinter_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinter_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[Printer] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplateTablesConsult_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplateTablesConsult_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[TemplateTablesConsult] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTemplateTicket_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTemplateTicket_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[TemplateTicket] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iWeighingMachine_pos_configurationplaceterminal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iWeighingMachine_pos_configurationplaceterminal] ON [dbo].[pos_configurationplaceterminal]
(
	[WeighingMachine] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_usercommissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_usercommissiongroup] ON [dbo].[pos_usercommissiongroup]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentFinanceMaster_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentFinanceMaster_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[DocumentFinanceMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentFinancePayment_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentFinancePayment_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[DocumentFinancePayment] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentFinanceType_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentFinanceType_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[DocumentFinanceType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPaymentMethod_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPaymentMethod_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[PaymentMethod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserDetail_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserDetail_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[UserDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iWorkSessionMovementType_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iWorkSessionMovementType_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[WorkSessionMovementType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iWorkSessionPeriod_pos_worksessionmovement]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iWorkSessionPeriod_pos_worksessionmovement] ON [dbo].[pos_worksessionmovement]
(
	[WorkSessionPeriod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iResourceString_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iResourceString_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[ResourceString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_worksessionmovementtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_worksessionmovementtype] ON [dbo].[pos_worksessionmovementtype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iParent_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iParent_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[Parent] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_worksessionperiod]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_worksessionperiod] ON [dbo].[pos_worksessionperiod]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPaymentMethod_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPaymentMethod_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[PaymentMethod] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPeriod_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPeriod_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[Period] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_pos_worksessionperiodtotal]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_pos_worksessionperiodtotal] ON [dbo].[pos_worksessionperiodtotal]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_rpt_report] ON [dbo].[rpt_report]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_rpt_report] ON [dbo].[rpt_report]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_rpt_report] ON [dbo].[rpt_report]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_rpt_report] ON [dbo].[rpt_report]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_rpt_report] ON [dbo].[rpt_report]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_rpt_report] ON [dbo].[rpt_report]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_rpt_report] ON [dbo].[rpt_report]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iReportType_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iReportType_rpt_report] ON [dbo].[rpt_report]
(
	[ReportType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iResourceString_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iResourceString_rpt_report] ON [dbo].[rpt_report]
(
	[ResourceString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_rpt_report] ON [dbo].[rpt_report]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_rpt_report] ON [dbo].[rpt_report]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_rpt_report]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_rpt_report] ON [dbo].[rpt_report]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iResourceString_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iResourceString_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[ResourceString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_rpt_reporttype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_rpt_reporttype] ON [dbo].[rpt_reporttype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_configurationinputreader]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_configurationinputreader] ON [dbo].[sys_configurationinputreader]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_configurationpoledisplay]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_configurationpoledisplay] ON [dbo].[sys_configurationpoledisplay]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPrinterType_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPrinterType_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[PrinterType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_configurationprinters]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_configurationprinters] ON [dbo].[sys_configurationprinters]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_configurationprinterstemplates]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_configurationprinterstemplates] ON [dbo].[sys_configurationprinterstemplates]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_configurationprinterstype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_configurationprinterstype] ON [dbo].[sys_configurationprinterstype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_configurationweighingmachine]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_configurationweighingmachine] ON [dbo].[sys_configurationweighingmachine]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iAuditType_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iAuditType_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[AuditType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserDetail_sys_systemaudit]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserDetail_sys_systemaudit] ON [dbo].[sys_systemaudit]
(
	[UserDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemauditat]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemauditat] ON [dbo].[sys_systemauditat]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iResourceString_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iResourceString_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[ResourceString] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemaudittype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemaudittype] ON [dbo].[sys_systemaudittype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iFileName_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iFileName_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[FileName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iFileNamePacked_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iFileNamePacked_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[FileNamePacked] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUser_sys_systembackup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUser_sys_systembackup] ON [dbo].[sys_systembackup]
(
	[User] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iNotificationType_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iNotificationType_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[NotificationType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminalLastRead_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminalLastRead_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[TerminalLastRead] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminalTarget_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminalTarget_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[TerminalTarget] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserLastRead_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserLastRead_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[UserLastRead] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserTarget_sys_systemnotification]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserTarget_sys_systemnotification] ON [dbo].[sys_systemnotification]
(
	[UserTarget] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iNotification_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iNotification_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[Notification] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemnotificationdocumentmaster]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemnotificationdocumentmaster] ON [dbo].[sys_systemnotificationdocumentmaster]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iMessage_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iMessage_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[Message] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminalTarget_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminalTarget_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[TerminalTarget] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserTarget_sys_systemnotificationtype]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserTarget_sys_systemnotificationtype] ON [dbo].[sys_systemnotificationtype]
(
	[UserTarget] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentMaster_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentMaster_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[DocumentMaster] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDocumentPayment_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDocumentPayment_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[DocumentPayment] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iTerminal_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iTerminal_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[Terminal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserDetail_sys_systemprint]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserDetail_sys_systemprint] ON [dbo].[sys_systemprint]
(
	[UserDetail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iAccessPin_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iAccessPin_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[AccessPin] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iCodeInternal_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCodeInternal_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[CodeInternal] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCommissionGroup_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCommissionGroup_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[CommissionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iProfile_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iProfile_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[Profile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_userdetail]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_userdetail] ON [dbo].[sys_userdetail]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_userpermissiongroup]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_userpermissiongroup] ON [dbo].[sys_userpermissiongroup]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPermissionGroup_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPermissionGroup_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[PermissionGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iToken_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iToken_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[Token] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_userpermissionitem]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_userpermissionitem] ON [dbo].[sys_userpermissionitem]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iPermissionItem_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iPermissionItem_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[PermissionItem] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUserProfile_sys_userpermissionprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUserProfile_sys_userpermissionprofile] ON [dbo].[sys_userpermissionprofile]
(
	[UserProfile] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCode_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iCode_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedBy_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedBy_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iCreatedWhere_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iCreatedWhere_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[CreatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedBy_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedBy_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[DeletedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iDeletedWhere_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iDeletedWhere_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[DeletedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [iDesignation_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iDesignation_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[Designation] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iOid_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iOid_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[Oid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedBy_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedBy_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[UpdatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [iUpdatedWhere_sys_userprofile]    Script Date: 26/06/2018 16:07:51 ******/
CREATE NONCLUSTERED INDEX [iUpdatedWhere_sys_userprofile] ON [dbo].[sys_userprofile]
(
	[UpdatedWhere] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[cfg_configurationcountry]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcountry_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcountry] CHECK CONSTRAINT [FK_cfg_configurationcountry_CreatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationcountry]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcountry_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcountry] CHECK CONSTRAINT [FK_cfg_configurationcountry_CreatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationcountry]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcountry_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcountry] CHECK CONSTRAINT [FK_cfg_configurationcountry_DeletedBy]
GO
ALTER TABLE [dbo].[cfg_configurationcountry]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcountry_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcountry] CHECK CONSTRAINT [FK_cfg_configurationcountry_DeletedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationcountry]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcountry_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcountry] CHECK CONSTRAINT [FK_cfg_configurationcountry_UpdatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationcountry]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcountry_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcountry] CHECK CONSTRAINT [FK_cfg_configurationcountry_UpdatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationcurrency]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcurrency_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcurrency] CHECK CONSTRAINT [FK_cfg_configurationcurrency_CreatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationcurrency]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcurrency_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcurrency] CHECK CONSTRAINT [FK_cfg_configurationcurrency_CreatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationcurrency]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcurrency_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcurrency] CHECK CONSTRAINT [FK_cfg_configurationcurrency_DeletedBy]
GO
ALTER TABLE [dbo].[cfg_configurationcurrency]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcurrency_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcurrency] CHECK CONSTRAINT [FK_cfg_configurationcurrency_DeletedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationcurrency]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcurrency_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcurrency] CHECK CONSTRAINT [FK_cfg_configurationcurrency_UpdatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationcurrency]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationcurrency_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationcurrency] CHECK CONSTRAINT [FK_cfg_configurationcurrency_UpdatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationholidays]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationholidays_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationholidays] CHECK CONSTRAINT [FK_cfg_configurationholidays_CreatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationholidays]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationholidays_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationholidays] CHECK CONSTRAINT [FK_cfg_configurationholidays_CreatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationholidays]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationholidays_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationholidays] CHECK CONSTRAINT [FK_cfg_configurationholidays_DeletedBy]
GO
ALTER TABLE [dbo].[cfg_configurationholidays]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationholidays_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationholidays] CHECK CONSTRAINT [FK_cfg_configurationholidays_DeletedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationholidays]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationholidays_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationholidays] CHECK CONSTRAINT [FK_cfg_configurationholidays_UpdatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationholidays]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationholidays_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationholidays] CHECK CONSTRAINT [FK_cfg_configurationholidays_UpdatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationpreferenceparameter_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter] CHECK CONSTRAINT [FK_cfg_configurationpreferenceparameter_CreatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationpreferenceparameter_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter] CHECK CONSTRAINT [FK_cfg_configurationpreferenceparameter_CreatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationpreferenceparameter_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter] CHECK CONSTRAINT [FK_cfg_configurationpreferenceparameter_DeletedBy]
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationpreferenceparameter_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter] CHECK CONSTRAINT [FK_cfg_configurationpreferenceparameter_DeletedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationpreferenceparameter_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter] CHECK CONSTRAINT [FK_cfg_configurationpreferenceparameter_UpdatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationpreferenceparameter_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationpreferenceparameter] CHECK CONSTRAINT [FK_cfg_configurationpreferenceparameter_UpdatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitmeasure_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure] CHECK CONSTRAINT [FK_cfg_configurationunitmeasure_CreatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitmeasure_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure] CHECK CONSTRAINT [FK_cfg_configurationunitmeasure_CreatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitmeasure_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure] CHECK CONSTRAINT [FK_cfg_configurationunitmeasure_DeletedBy]
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitmeasure_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure] CHECK CONSTRAINT [FK_cfg_configurationunitmeasure_DeletedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitmeasure_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure] CHECK CONSTRAINT [FK_cfg_configurationunitmeasure_UpdatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitmeasure_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitmeasure] CHECK CONSTRAINT [FK_cfg_configurationunitmeasure_UpdatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationunitsize]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitsize_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitsize] CHECK CONSTRAINT [FK_cfg_configurationunitsize_CreatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationunitsize]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitsize_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitsize] CHECK CONSTRAINT [FK_cfg_configurationunitsize_CreatedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationunitsize]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitsize_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitsize] CHECK CONSTRAINT [FK_cfg_configurationunitsize_DeletedBy]
GO
ALTER TABLE [dbo].[cfg_configurationunitsize]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitsize_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitsize] CHECK CONSTRAINT [FK_cfg_configurationunitsize_DeletedWhere]
GO
ALTER TABLE [dbo].[cfg_configurationunitsize]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitsize_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitsize] CHECK CONSTRAINT [FK_cfg_configurationunitsize_UpdatedBy]
GO
ALTER TABLE [dbo].[cfg_configurationunitsize]  WITH NOCHECK ADD  CONSTRAINT [FK_cfg_configurationunitsize_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[cfg_configurationunitsize] CHECK CONSTRAINT [FK_cfg_configurationunitsize_UpdatedWhere]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_Country] FOREIGN KEY([Country])
REFERENCES [dbo].[cfg_configurationcountry] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_Country]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_CreatedBy]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_CreatedWhere]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_CustomerType] FOREIGN KEY([CustomerType])
REFERENCES [dbo].[erp_customertype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_CustomerType]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_DeletedBy]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_DeletedWhere]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_DiscountGroup] FOREIGN KEY([DiscountGroup])
REFERENCES [dbo].[erp_customerdiscountgroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_DiscountGroup]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_PriceType] FOREIGN KEY([PriceType])
REFERENCES [dbo].[fin_configurationpricetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_PriceType]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_UpdatedBy]
GO
ALTER TABLE [dbo].[erp_customer]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customer_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customer] CHECK CONSTRAINT [FK_erp_customer_UpdatedWhere]
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customerdiscountgroup_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup] CHECK CONSTRAINT [FK_erp_customerdiscountgroup_CreatedBy]
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customerdiscountgroup_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup] CHECK CONSTRAINT [FK_erp_customerdiscountgroup_CreatedWhere]
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customerdiscountgroup_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup] CHECK CONSTRAINT [FK_erp_customerdiscountgroup_DeletedBy]
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customerdiscountgroup_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup] CHECK CONSTRAINT [FK_erp_customerdiscountgroup_DeletedWhere]
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customerdiscountgroup_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup] CHECK CONSTRAINT [FK_erp_customerdiscountgroup_UpdatedBy]
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customerdiscountgroup_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customerdiscountgroup] CHECK CONSTRAINT [FK_erp_customerdiscountgroup_UpdatedWhere]
GO
ALTER TABLE [dbo].[erp_customertype]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customertype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customertype] CHECK CONSTRAINT [FK_erp_customertype_CreatedBy]
GO
ALTER TABLE [dbo].[erp_customertype]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customertype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customertype] CHECK CONSTRAINT [FK_erp_customertype_CreatedWhere]
GO
ALTER TABLE [dbo].[erp_customertype]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customertype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customertype] CHECK CONSTRAINT [FK_erp_customertype_DeletedBy]
GO
ALTER TABLE [dbo].[erp_customertype]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customertype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customertype] CHECK CONSTRAINT [FK_erp_customertype_DeletedWhere]
GO
ALTER TABLE [dbo].[erp_customertype]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customertype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customertype] CHECK CONSTRAINT [FK_erp_customertype_UpdatedBy]
GO
ALTER TABLE [dbo].[erp_customertype]  WITH NOCHECK ADD  CONSTRAINT [FK_erp_customertype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[erp_customertype] CHECK CONSTRAINT [FK_erp_customertype_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_Class] FOREIGN KEY([Class])
REFERENCES [dbo].[fin_articleclass] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_Class]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_CommissionGroup] FOREIGN KEY([CommissionGroup])
REFERENCES [dbo].[pos_usercommissiongroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_CommissionGroup]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_CreatedBy]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_DeletedBy]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_DiscountGroup] FOREIGN KEY([DiscountGroup])
REFERENCES [dbo].[erp_customerdiscountgroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_DiscountGroup]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_Family] FOREIGN KEY([Family])
REFERENCES [dbo].[fin_articlefamily] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_Family]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_Printer] FOREIGN KEY([Printer])
REFERENCES [dbo].[sys_configurationprinters] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_Printer]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_SubFamily] FOREIGN KEY([SubFamily])
REFERENCES [dbo].[fin_articlesubfamily] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_SubFamily]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_Template] FOREIGN KEY([Template])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_Template]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_Type] FOREIGN KEY([Type])
REFERENCES [dbo].[fin_articletype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_Type]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_UnitMeasure] FOREIGN KEY([UnitMeasure])
REFERENCES [dbo].[cfg_configurationunitmeasure] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_UnitMeasure]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_UnitSize] FOREIGN KEY([UnitSize])
REFERENCES [dbo].[cfg_configurationunitsize] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_UnitSize]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_VatDirectSelling] FOREIGN KEY([VatDirectSelling])
REFERENCES [dbo].[fin_configurationvatrate] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_VatDirectSelling]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_VatExemptionReason] FOREIGN KEY([VatExemptionReason])
REFERENCES [dbo].[fin_configurationvatexemptionreason] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_VatExemptionReason]
GO
ALTER TABLE [dbo].[fin_article]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_article_VatOnTable] FOREIGN KEY([VatOnTable])
REFERENCES [dbo].[fin_configurationvatrate] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_article] CHECK CONSTRAINT [FK_fin_article_VatOnTable]
GO
ALTER TABLE [dbo].[fin_articleclass]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articleclass_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articleclass] CHECK CONSTRAINT [FK_fin_articleclass_CreatedBy]
GO
ALTER TABLE [dbo].[fin_articleclass]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articleclass_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articleclass] CHECK CONSTRAINT [FK_fin_articleclass_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_articleclass]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articleclass_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articleclass] CHECK CONSTRAINT [FK_fin_articleclass_DeletedBy]
GO
ALTER TABLE [dbo].[fin_articleclass]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articleclass_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articleclass] CHECK CONSTRAINT [FK_fin_articleclass_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_articleclass]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articleclass_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articleclass] CHECK CONSTRAINT [FK_fin_articleclass_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_articleclass]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articleclass_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articleclass] CHECK CONSTRAINT [FK_fin_articleclass_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_CommissionGroup] FOREIGN KEY([CommissionGroup])
REFERENCES [dbo].[pos_usercommissiongroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_CommissionGroup]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_CreatedBy]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_DeletedBy]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_DiscountGroup] FOREIGN KEY([DiscountGroup])
REFERENCES [dbo].[erp_customerdiscountgroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_DiscountGroup]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_Printer] FOREIGN KEY([Printer])
REFERENCES [dbo].[sys_configurationprinters] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_Printer]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_Template] FOREIGN KEY([Template])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_Template]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_articlefamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlefamily_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlefamily] CHECK CONSTRAINT [FK_fin_articlefamily_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_Article] FOREIGN KEY([Article])
REFERENCES [dbo].[fin_article] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_Article]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_CreatedBy]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_Customer] FOREIGN KEY([Customer])
REFERENCES [dbo].[erp_customer] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_Customer]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_DeletedBy]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_DocumentDetail] FOREIGN KEY([DocumentDetail])
REFERENCES [dbo].[fin_documentfinancedetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_DocumentDetail]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_DocumentMaster]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_articlestock]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlestock_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlestock] CHECK CONSTRAINT [FK_fin_articlestock_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_CommissionGroup] FOREIGN KEY([CommissionGroup])
REFERENCES [dbo].[pos_usercommissiongroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_CommissionGroup]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_CreatedBy]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_DeletedBy]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_DiscountGroup] FOREIGN KEY([DiscountGroup])
REFERENCES [dbo].[erp_customerdiscountgroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_DiscountGroup]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_Family] FOREIGN KEY([Family])
REFERENCES [dbo].[fin_articlefamily] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_Family]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_Printer] FOREIGN KEY([Printer])
REFERENCES [dbo].[sys_configurationprinters] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_Printer]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_Template] FOREIGN KEY([Template])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_Template]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_VatDirectSelling] FOREIGN KEY([VatDirectSelling])
REFERENCES [dbo].[fin_configurationvatrate] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_VatDirectSelling]
GO
ALTER TABLE [dbo].[fin_articlesubfamily]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articlesubfamily_VatOnTable] FOREIGN KEY([VatOnTable])
REFERENCES [dbo].[fin_configurationvatrate] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articlesubfamily] CHECK CONSTRAINT [FK_fin_articlesubfamily_VatOnTable]
GO
ALTER TABLE [dbo].[fin_articletype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articletype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articletype] CHECK CONSTRAINT [FK_fin_articletype_CreatedBy]
GO
ALTER TABLE [dbo].[fin_articletype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articletype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articletype] CHECK CONSTRAINT [FK_fin_articletype_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_articletype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articletype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articletype] CHECK CONSTRAINT [FK_fin_articletype_DeletedBy]
GO
ALTER TABLE [dbo].[fin_articletype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articletype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articletype] CHECK CONSTRAINT [FK_fin_articletype_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_articletype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articletype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articletype] CHECK CONSTRAINT [FK_fin_articletype_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_articletype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_articletype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_articletype] CHECK CONSTRAINT [FK_fin_articletype_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentcondition_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition] CHECK CONSTRAINT [FK_fin_configurationpaymentcondition_CreatedBy]
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentcondition_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition] CHECK CONSTRAINT [FK_fin_configurationpaymentcondition_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentcondition_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition] CHECK CONSTRAINT [FK_fin_configurationpaymentcondition_DeletedBy]
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentcondition_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition] CHECK CONSTRAINT [FK_fin_configurationpaymentcondition_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentcondition_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition] CHECK CONSTRAINT [FK_fin_configurationpaymentcondition_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentcondition_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentcondition] CHECK CONSTRAINT [FK_fin_configurationpaymentcondition_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentmethod_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod] CHECK CONSTRAINT [FK_fin_configurationpaymentmethod_CreatedBy]
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentmethod_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod] CHECK CONSTRAINT [FK_fin_configurationpaymentmethod_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentmethod_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod] CHECK CONSTRAINT [FK_fin_configurationpaymentmethod_DeletedBy]
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentmethod_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod] CHECK CONSTRAINT [FK_fin_configurationpaymentmethod_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentmethod_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod] CHECK CONSTRAINT [FK_fin_configurationpaymentmethod_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpaymentmethod_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpaymentmethod] CHECK CONSTRAINT [FK_fin_configurationpaymentmethod_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpricetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpricetype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpricetype] CHECK CONSTRAINT [FK_fin_configurationpricetype_CreatedBy]
GO
ALTER TABLE [dbo].[fin_configurationpricetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpricetype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpricetype] CHECK CONSTRAINT [FK_fin_configurationpricetype_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpricetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpricetype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpricetype] CHECK CONSTRAINT [FK_fin_configurationpricetype_DeletedBy]
GO
ALTER TABLE [dbo].[fin_configurationpricetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpricetype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpricetype] CHECK CONSTRAINT [FK_fin_configurationpricetype_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_configurationpricetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpricetype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpricetype] CHECK CONSTRAINT [FK_fin_configurationpricetype_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_configurationpricetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationpricetype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationpricetype] CHECK CONSTRAINT [FK_fin_configurationpricetype_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatexemptionreason_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason] CHECK CONSTRAINT [FK_fin_configurationvatexemptionreason_CreatedBy]
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatexemptionreason_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason] CHECK CONSTRAINT [FK_fin_configurationvatexemptionreason_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatexemptionreason_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason] CHECK CONSTRAINT [FK_fin_configurationvatexemptionreason_DeletedBy]
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatexemptionreason_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason] CHECK CONSTRAINT [FK_fin_configurationvatexemptionreason_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatexemptionreason_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason] CHECK CONSTRAINT [FK_fin_configurationvatexemptionreason_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatexemptionreason_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatexemptionreason] CHECK CONSTRAINT [FK_fin_configurationvatexemptionreason_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationvatrate]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatrate_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatrate] CHECK CONSTRAINT [FK_fin_configurationvatrate_CreatedBy]
GO
ALTER TABLE [dbo].[fin_configurationvatrate]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatrate_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatrate] CHECK CONSTRAINT [FK_fin_configurationvatrate_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_configurationvatrate]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatrate_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatrate] CHECK CONSTRAINT [FK_fin_configurationvatrate_DeletedBy]
GO
ALTER TABLE [dbo].[fin_configurationvatrate]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatrate_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatrate] CHECK CONSTRAINT [FK_fin_configurationvatrate_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_configurationvatrate]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatrate_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatrate] CHECK CONSTRAINT [FK_fin_configurationvatrate_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_configurationvatrate]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_configurationvatrate_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_configurationvatrate] CHECK CONSTRAINT [FK_fin_configurationvatrate_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_CommissionGroup] FOREIGN KEY([CommissionGroup])
REFERENCES [dbo].[pos_usercommissiongroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_CommissionGroup]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_FinanceDetail] FOREIGN KEY([FinanceDetail])
REFERENCES [dbo].[fin_documentfinancedetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_FinanceDetail]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_FinanceMaster] FOREIGN KEY([FinanceMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_FinanceMaster]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_Terminal]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancecommission]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancecommission_UserDetail] FOREIGN KEY([UserDetail])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancecommission] CHECK CONSTRAINT [FK_fin_documentfinancecommission_UserDetail]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_Article] FOREIGN KEY([Article])
REFERENCES [dbo].[fin_article] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_Article]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_DocumentMaster]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_VatExemptionReason] FOREIGN KEY([VatExemptionReason])
REFERENCES [dbo].[fin_configurationvatexemptionreason] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_VatExemptionReason]
GO
ALTER TABLE [dbo].[fin_documentfinancedetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetail_VatRate] FOREIGN KEY([VatRate])
REFERENCES [dbo].[fin_configurationvatrate] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetail] CHECK CONSTRAINT [FK_fin_documentfinancedetail_VatRate]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_DocumentDetail] FOREIGN KEY([DocumentDetail])
REFERENCES [dbo].[fin_documentfinancedetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_DocumentDetail]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_DocumentMaster]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailorderreference_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailorderreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailorderreference_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_DocumentDetail] FOREIGN KEY([DocumentDetail])
REFERENCES [dbo].[fin_documentfinancedetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_DocumentDetail]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_DocumentMaster]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancedetailreference_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancedetailreference] CHECK CONSTRAINT [FK_fin_documentfinancedetailreference_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_ATValidAuditResult] FOREIGN KEY([ATValidAuditResult])
REFERENCES [dbo].[sys_systemauditat] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_ATValidAuditResult]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_Currency] FOREIGN KEY([Currency])
REFERENCES [dbo].[cfg_configurationcurrency] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_Currency]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_DocumentChild] FOREIGN KEY([DocumentChild])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_DocumentChild]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_DocumentParent] FOREIGN KEY([DocumentParent])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_DocumentParent]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_DocumentSerie] FOREIGN KEY([DocumentSerie])
REFERENCES [dbo].[fin_documentfinanceseries] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_DocumentSerie]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_DocumentType] FOREIGN KEY([DocumentType])
REFERENCES [dbo].[fin_documentfinancetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_DocumentType]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_PaymentCondition] FOREIGN KEY([PaymentCondition])
REFERENCES [dbo].[fin_configurationpaymentcondition] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_PaymentCondition]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_PaymentMethod] FOREIGN KEY([PaymentMethod])
REFERENCES [dbo].[fin_configurationpaymentmethod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_PaymentMethod]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_SourceOrderMain] FOREIGN KEY([SourceOrderMain])
REFERENCES [dbo].[fin_documentordermain] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_SourceOrderMain]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemaster]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemaster_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemaster] CHECK CONSTRAINT [FK_fin_documentfinancemaster_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_DocumentFinanceMaster] FOREIGN KEY([DocumentFinanceMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_DocumentFinanceMaster]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_DocumentFinanceMasterCreditNote] FOREIGN KEY([DocumentFinanceMasterCreditNote])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_DocumentFinanceMasterCreditNote]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_DocumentFinancePayment] FOREIGN KEY([DocumentFinancePayment])
REFERENCES [dbo].[fin_documentfinancepayment] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_DocumentFinancePayment]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemasterpayment_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemasterpayment] CHECK CONSTRAINT [FK_fin_documentfinancemasterpayment_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_DocumentMaster]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancemastertotal_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancemastertotal] CHECK CONSTRAINT [FK_fin_documentfinancemastertotal_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_Currency] FOREIGN KEY([Currency])
REFERENCES [dbo].[cfg_configurationcurrency] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_Currency]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_DocumentSerie] FOREIGN KEY([DocumentSerie])
REFERENCES [dbo].[fin_documentfinanceseries] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_DocumentSerie]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_DocumentType] FOREIGN KEY([DocumentType])
REFERENCES [dbo].[fin_documentfinancetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_DocumentType]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_PaymentMethod] FOREIGN KEY([PaymentMethod])
REFERENCES [dbo].[fin_configurationpaymentmethod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_PaymentMethod]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancepayment]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancepayment_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancepayment] CHECK CONSTRAINT [FK_fin_documentfinancepayment_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_DocumentType] FOREIGN KEY([DocumentType])
REFERENCES [dbo].[fin_documentfinancetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_DocumentType]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_FiscalYear] FOREIGN KEY([FiscalYear])
REFERENCES [dbo].[fin_documentfinanceyears] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_FiscalYear]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceseries]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceseries_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceseries] CHECK CONSTRAINT [FK_fin_documentfinanceseries_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_Printer] FOREIGN KEY([Printer])
REFERENCES [dbo].[sys_configurationprinters] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_Printer]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_Template] FOREIGN KEY([Template])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_Template]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinancetype]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinancetype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinancetype] CHECK CONSTRAINT [FK_fin_documentfinancetype_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceyears]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyears_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyears] CHECK CONSTRAINT [FK_fin_documentfinanceyears_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceyears]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyears_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyears] CHECK CONSTRAINT [FK_fin_documentfinanceyears_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceyears]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyears_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyears] CHECK CONSTRAINT [FK_fin_documentfinanceyears_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceyears]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyears_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyears] CHECK CONSTRAINT [FK_fin_documentfinanceyears_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceyears]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyears_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyears] CHECK CONSTRAINT [FK_fin_documentfinanceyears_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceyears]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyears_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyears] CHECK CONSTRAINT [FK_fin_documentfinanceyears_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_DocumentType] FOREIGN KEY([DocumentType])
REFERENCES [dbo].[fin_documentfinancetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_DocumentType]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_FiscalYear] FOREIGN KEY([FiscalYear])
REFERENCES [dbo].[fin_documentfinanceyears] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_FiscalYear]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Printer] FOREIGN KEY([Printer])
REFERENCES [dbo].[sys_configurationprinters] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Printer]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Serie] FOREIGN KEY([Serie])
REFERENCES [dbo].[fin_documentfinanceseries] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Serie]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Template] FOREIGN KEY([Template])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Template]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_Terminal]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentfinanceyearserieterminal_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentfinanceyearserieterminal] CHECK CONSTRAINT [FK_fin_documentfinanceyearserieterminal_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_Article] FOREIGN KEY([Article])
REFERENCES [dbo].[fin_article] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_Article]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_OrderTicket] FOREIGN KEY([OrderTicket])
REFERENCES [dbo].[fin_documentorderticket] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_OrderTicket]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentorderdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderdetail_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderdetail] CHECK CONSTRAINT [FK_fin_documentorderdetail_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_PlaceTable] FOREIGN KEY([PlaceTable])
REFERENCES [dbo].[pos_configurationplacetable] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_PlaceTable]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentordermain]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentordermain_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentordermain] CHECK CONSTRAINT [FK_fin_documentordermain_UpdatedWhere]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_CreatedBy]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_CreatedWhere]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_DeletedBy]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_DeletedWhere]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_OrderMain] FOREIGN KEY([OrderMain])
REFERENCES [dbo].[fin_documentordermain] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_OrderMain]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_PlaceTable] FOREIGN KEY([PlaceTable])
REFERENCES [dbo].[pos_configurationplacetable] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_PlaceTable]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_UpdatedBy]
GO
ALTER TABLE [dbo].[fin_documentorderticket]  WITH NOCHECK ADD  CONSTRAINT [FK_fin_documentorderticket_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[fin_documentorderticket] CHECK CONSTRAINT [FK_fin_documentorderticket_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationcashregister]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationcashregister_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationcashregister] CHECK CONSTRAINT [FK_pos_configurationcashregister_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationcashregister]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationcashregister_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationcashregister] CHECK CONSTRAINT [FK_pos_configurationcashregister_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationcashregister]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationcashregister_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationcashregister] CHECK CONSTRAINT [FK_pos_configurationcashregister_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationcashregister]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationcashregister_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationcashregister] CHECK CONSTRAINT [FK_pos_configurationcashregister_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationcashregister]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationcashregister_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationcashregister] CHECK CONSTRAINT [FK_pos_configurationcashregister_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationcashregister]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationcashregister_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationcashregister] CHECK CONSTRAINT [FK_pos_configurationcashregister_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_PlaceTerminal] FOREIGN KEY([PlaceTerminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_PlaceTerminal]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationdevice]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationdevice_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationdevice] CHECK CONSTRAINT [FK_pos_configurationdevice_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationkeyboard]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationkeyboard_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationkeyboard] CHECK CONSTRAINT [FK_pos_configurationkeyboard_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationkeyboard]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationkeyboard_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationkeyboard] CHECK CONSTRAINT [FK_pos_configurationkeyboard_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationkeyboard]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationkeyboard_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationkeyboard] CHECK CONSTRAINT [FK_pos_configurationkeyboard_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationkeyboard]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationkeyboard_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationkeyboard] CHECK CONSTRAINT [FK_pos_configurationkeyboard_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationkeyboard]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationkeyboard_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationkeyboard] CHECK CONSTRAINT [FK_pos_configurationkeyboard_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationkeyboard]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationkeyboard_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationkeyboard] CHECK CONSTRAINT [FK_pos_configurationkeyboard_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationmaintenance]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationmaintenance_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationmaintenance] CHECK CONSTRAINT [FK_pos_configurationmaintenance_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationmaintenance]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationmaintenance_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationmaintenance] CHECK CONSTRAINT [FK_pos_configurationmaintenance_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationmaintenance]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationmaintenance_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationmaintenance] CHECK CONSTRAINT [FK_pos_configurationmaintenance_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationmaintenance]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationmaintenance_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationmaintenance] CHECK CONSTRAINT [FK_pos_configurationmaintenance_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationmaintenance]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationmaintenance_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationmaintenance] CHECK CONSTRAINT [FK_pos_configurationmaintenance_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationmaintenance]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationmaintenance_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationmaintenance] CHECK CONSTRAINT [FK_pos_configurationmaintenance_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_MovementType] FOREIGN KEY([MovementType])
REFERENCES [dbo].[pos_configurationplacemovementtype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_MovementType]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_PriceType] FOREIGN KEY([PriceType])
REFERENCES [dbo].[fin_configurationpricetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_PriceType]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplace]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplace_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplace] CHECK CONSTRAINT [FK_pos_configurationplace_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacemovementtype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype] CHECK CONSTRAINT [FK_pos_configurationplacemovementtype_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacemovementtype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype] CHECK CONSTRAINT [FK_pos_configurationplacemovementtype_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacemovementtype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype] CHECK CONSTRAINT [FK_pos_configurationplacemovementtype_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacemovementtype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype] CHECK CONSTRAINT [FK_pos_configurationplacemovementtype_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacemovementtype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype] CHECK CONSTRAINT [FK_pos_configurationplacemovementtype_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacemovementtype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacemovementtype] CHECK CONSTRAINT [FK_pos_configurationplacemovementtype_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_Place] FOREIGN KEY([Place])
REFERENCES [dbo].[pos_configurationplace] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_Place]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplacetable]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplacetable_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplacetable] CHECK CONSTRAINT [FK_pos_configurationplacetable_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_BarcodeReader] FOREIGN KEY([BarcodeReader])
REFERENCES [dbo].[sys_configurationinputreader] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_BarcodeReader]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_CardReader] FOREIGN KEY([CardReader])
REFERENCES [dbo].[sys_configurationinputreader] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_CardReader]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_CreatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_DeletedBy]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_Place] FOREIGN KEY([Place])
REFERENCES [dbo].[pos_configurationplace] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_Place]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_PoleDisplay] FOREIGN KEY([PoleDisplay])
REFERENCES [dbo].[sys_configurationpoledisplay] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_PoleDisplay]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_Printer] FOREIGN KEY([Printer])
REFERENCES [dbo].[sys_configurationprinters] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_Printer]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_TemplateTablesConsult] FOREIGN KEY([TemplateTablesConsult])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_TemplateTablesConsult]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_TemplateTicket] FOREIGN KEY([TemplateTicket])
REFERENCES [dbo].[sys_configurationprinterstemplates] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_TemplateTicket]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_configurationplaceterminal_WeighingMachine] FOREIGN KEY([WeighingMachine])
REFERENCES [dbo].[sys_configurationweighingmachine] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_configurationplaceterminal] CHECK CONSTRAINT [FK_pos_configurationplaceterminal_WeighingMachine]
GO
ALTER TABLE [dbo].[pos_usercommissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_usercommissiongroup_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_usercommissiongroup] CHECK CONSTRAINT [FK_pos_usercommissiongroup_CreatedBy]
GO
ALTER TABLE [dbo].[pos_usercommissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_usercommissiongroup_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_usercommissiongroup] CHECK CONSTRAINT [FK_pos_usercommissiongroup_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_usercommissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_usercommissiongroup_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_usercommissiongroup] CHECK CONSTRAINT [FK_pos_usercommissiongroup_DeletedBy]
GO
ALTER TABLE [dbo].[pos_usercommissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_usercommissiongroup_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_usercommissiongroup] CHECK CONSTRAINT [FK_pos_usercommissiongroup_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_usercommissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_usercommissiongroup_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_usercommissiongroup] CHECK CONSTRAINT [FK_pos_usercommissiongroup_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_usercommissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_usercommissiongroup_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_usercommissiongroup] CHECK CONSTRAINT [FK_pos_usercommissiongroup_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_CreatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_DeletedBy]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_DocumentFinanceMaster] FOREIGN KEY([DocumentFinanceMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_DocumentFinanceMaster]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_DocumentFinancePayment] FOREIGN KEY([DocumentFinancePayment])
REFERENCES [dbo].[fin_documentfinancepayment] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_DocumentFinancePayment]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_DocumentFinanceType] FOREIGN KEY([DocumentFinanceType])
REFERENCES [dbo].[fin_documentfinancetype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_DocumentFinanceType]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_PaymentMethod] FOREIGN KEY([PaymentMethod])
REFERENCES [dbo].[fin_configurationpaymentmethod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_PaymentMethod]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_Terminal]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_UserDetail] FOREIGN KEY([UserDetail])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_UserDetail]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_WorkSessionMovementType] FOREIGN KEY([WorkSessionMovementType])
REFERENCES [dbo].[pos_worksessionmovementtype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_WorkSessionMovementType]
GO
ALTER TABLE [dbo].[pos_worksessionmovement]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovement_WorkSessionPeriod] FOREIGN KEY([WorkSessionPeriod])
REFERENCES [dbo].[pos_worksessionperiod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovement] CHECK CONSTRAINT [FK_pos_worksessionmovement_WorkSessionPeriod]
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovementtype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype] CHECK CONSTRAINT [FK_pos_worksessionmovementtype_CreatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovementtype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype] CHECK CONSTRAINT [FK_pos_worksessionmovementtype_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovementtype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype] CHECK CONSTRAINT [FK_pos_worksessionmovementtype_DeletedBy]
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovementtype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype] CHECK CONSTRAINT [FK_pos_worksessionmovementtype_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovementtype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype] CHECK CONSTRAINT [FK_pos_worksessionmovementtype_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionmovementtype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionmovementtype] CHECK CONSTRAINT [FK_pos_worksessionmovementtype_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_CreatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_DeletedBy]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_Parent] FOREIGN KEY([Parent])
REFERENCES [dbo].[pos_worksessionperiod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_Parent]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_Terminal]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionperiod]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiod_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiod] CHECK CONSTRAINT [FK_pos_worksessionperiod_UpdatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_CreatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_CreatedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_DeletedBy]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_DeletedWhere]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_PaymentMethod] FOREIGN KEY([PaymentMethod])
REFERENCES [dbo].[fin_configurationpaymentmethod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_PaymentMethod]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_Period] FOREIGN KEY([Period])
REFERENCES [dbo].[pos_worksessionperiod] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_Period]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_UpdatedBy]
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal]  WITH NOCHECK ADD  CONSTRAINT [FK_pos_worksessionperiodtotal_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[pos_worksessionperiodtotal] CHECK CONSTRAINT [FK_pos_worksessionperiodtotal_UpdatedWhere]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_CreatedBy]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_CreatedWhere]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_DeletedBy]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_DeletedWhere]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_ReportType] FOREIGN KEY([ReportType])
REFERENCES [dbo].[rpt_reporttype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_ReportType]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_UpdatedBy]
GO
ALTER TABLE [dbo].[rpt_report]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_report_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_report] CHECK CONSTRAINT [FK_rpt_report_UpdatedWhere]
GO
ALTER TABLE [dbo].[rpt_reporttype]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_reporttype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_reporttype] CHECK CONSTRAINT [FK_rpt_reporttype_CreatedBy]
GO
ALTER TABLE [dbo].[rpt_reporttype]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_reporttype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_reporttype] CHECK CONSTRAINT [FK_rpt_reporttype_CreatedWhere]
GO
ALTER TABLE [dbo].[rpt_reporttype]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_reporttype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_reporttype] CHECK CONSTRAINT [FK_rpt_reporttype_DeletedBy]
GO
ALTER TABLE [dbo].[rpt_reporttype]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_reporttype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_reporttype] CHECK CONSTRAINT [FK_rpt_reporttype_DeletedWhere]
GO
ALTER TABLE [dbo].[rpt_reporttype]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_reporttype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_reporttype] CHECK CONSTRAINT [FK_rpt_reporttype_UpdatedBy]
GO
ALTER TABLE [dbo].[rpt_reporttype]  WITH NOCHECK ADD  CONSTRAINT [FK_rpt_reporttype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[rpt_reporttype] CHECK CONSTRAINT [FK_rpt_reporttype_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationinputreader]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationinputreader_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationinputreader] CHECK CONSTRAINT [FK_sys_configurationinputreader_CreatedBy]
GO
ALTER TABLE [dbo].[sys_configurationinputreader]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationinputreader_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationinputreader] CHECK CONSTRAINT [FK_sys_configurationinputreader_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationinputreader]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationinputreader_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationinputreader] CHECK CONSTRAINT [FK_sys_configurationinputreader_DeletedBy]
GO
ALTER TABLE [dbo].[sys_configurationinputreader]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationinputreader_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationinputreader] CHECK CONSTRAINT [FK_sys_configurationinputreader_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_configurationinputreader]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationinputreader_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationinputreader] CHECK CONSTRAINT [FK_sys_configurationinputreader_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_configurationinputreader]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationinputreader_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationinputreader] CHECK CONSTRAINT [FK_sys_configurationinputreader_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationpoledisplay_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay] CHECK CONSTRAINT [FK_sys_configurationpoledisplay_CreatedBy]
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationpoledisplay_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay] CHECK CONSTRAINT [FK_sys_configurationpoledisplay_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationpoledisplay_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay] CHECK CONSTRAINT [FK_sys_configurationpoledisplay_DeletedBy]
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationpoledisplay_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay] CHECK CONSTRAINT [FK_sys_configurationpoledisplay_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationpoledisplay_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay] CHECK CONSTRAINT [FK_sys_configurationpoledisplay_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationpoledisplay_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationpoledisplay] CHECK CONSTRAINT [FK_sys_configurationpoledisplay_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_CreatedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_DeletedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_PrinterType] FOREIGN KEY([PrinterType])
REFERENCES [dbo].[sys_configurationprinterstype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_PrinterType]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinters]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinters_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinters] CHECK CONSTRAINT [FK_sys_configurationprinters_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstemplates_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates] CHECK CONSTRAINT [FK_sys_configurationprinterstemplates_CreatedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstemplates_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates] CHECK CONSTRAINT [FK_sys_configurationprinterstemplates_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstemplates_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates] CHECK CONSTRAINT [FK_sys_configurationprinterstemplates_DeletedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstemplates_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates] CHECK CONSTRAINT [FK_sys_configurationprinterstemplates_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstemplates_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates] CHECK CONSTRAINT [FK_sys_configurationprinterstemplates_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstemplates_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstemplates] CHECK CONSTRAINT [FK_sys_configurationprinterstemplates_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinterstype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstype] CHECK CONSTRAINT [FK_sys_configurationprinterstype_CreatedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinterstype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstype] CHECK CONSTRAINT [FK_sys_configurationprinterstype_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinterstype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstype] CHECK CONSTRAINT [FK_sys_configurationprinterstype_DeletedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinterstype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstype] CHECK CONSTRAINT [FK_sys_configurationprinterstype_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_configurationprinterstype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstype] CHECK CONSTRAINT [FK_sys_configurationprinterstype_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_configurationprinterstype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationprinterstype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationprinterstype] CHECK CONSTRAINT [FK_sys_configurationprinterstype_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationweighingmachine_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine] CHECK CONSTRAINT [FK_sys_configurationweighingmachine_CreatedBy]
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationweighingmachine_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine] CHECK CONSTRAINT [FK_sys_configurationweighingmachine_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationweighingmachine_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine] CHECK CONSTRAINT [FK_sys_configurationweighingmachine_DeletedBy]
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationweighingmachine_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine] CHECK CONSTRAINT [FK_sys_configurationweighingmachine_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationweighingmachine_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine] CHECK CONSTRAINT [FK_sys_configurationweighingmachine_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_configurationweighingmachine_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_configurationweighingmachine] CHECK CONSTRAINT [FK_sys_configurationweighingmachine_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_AuditType] FOREIGN KEY([AuditType])
REFERENCES [dbo].[sys_systemaudittype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_AuditType]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_Terminal]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudit]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudit_UserDetail] FOREIGN KEY([UserDetail])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudit] CHECK CONSTRAINT [FK_sys_systemaudit_UserDetail]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_DocumentMaster]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemauditat]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemauditat_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemauditat] CHECK CONSTRAINT [FK_sys_systemauditat_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudittype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudittype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudittype] CHECK CONSTRAINT [FK_sys_systemaudittype_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemaudittype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudittype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudittype] CHECK CONSTRAINT [FK_sys_systemaudittype_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudittype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudittype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudittype] CHECK CONSTRAINT [FK_sys_systemaudittype_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemaudittype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudittype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudittype] CHECK CONSTRAINT [FK_sys_systemaudittype_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemaudittype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudittype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudittype] CHECK CONSTRAINT [FK_sys_systemaudittype_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemaudittype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemaudittype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemaudittype] CHECK CONSTRAINT [FK_sys_systemaudittype_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_Terminal]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systembackup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systembackup_User] FOREIGN KEY([User])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systembackup] CHECK CONSTRAINT [FK_sys_systembackup_User]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_NotificationType] FOREIGN KEY([NotificationType])
REFERENCES [dbo].[sys_systemnotificationtype] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_NotificationType]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_TerminalLastRead] FOREIGN KEY([TerminalLastRead])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_TerminalLastRead]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_TerminalTarget] FOREIGN KEY([TerminalTarget])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_TerminalTarget]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_UserLastRead] FOREIGN KEY([UserLastRead])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_UserLastRead]
GO
ALTER TABLE [dbo].[sys_systemnotification]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotification_UserTarget] FOREIGN KEY([UserTarget])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotification] CHECK CONSTRAINT [FK_sys_systemnotification_UserTarget]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_DocumentMaster]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_Notification] FOREIGN KEY([Notification])
REFERENCES [dbo].[sys_systemnotification] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_Notification]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationdocumentmaster_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationdocumentmaster] CHECK CONSTRAINT [FK_sys_systemnotificationdocumentmaster_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_TerminalTarget] FOREIGN KEY([TerminalTarget])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_TerminalTarget]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemnotificationtype]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemnotificationtype_UserTarget] FOREIGN KEY([UserTarget])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemnotificationtype] CHECK CONSTRAINT [FK_sys_systemnotificationtype_UserTarget]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_CreatedBy]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_DeletedBy]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_DocumentMaster] FOREIGN KEY([DocumentMaster])
REFERENCES [dbo].[fin_documentfinancemaster] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_DocumentMaster]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_DocumentPayment] FOREIGN KEY([DocumentPayment])
REFERENCES [dbo].[fin_documentfinancepayment] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_DocumentPayment]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_Terminal] FOREIGN KEY([Terminal])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_Terminal]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_systemprint]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_systemprint_UserDetail] FOREIGN KEY([UserDetail])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_systemprint] CHECK CONSTRAINT [FK_sys_systemprint_UserDetail]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_CommissionGroup] FOREIGN KEY([CommissionGroup])
REFERENCES [dbo].[pos_usercommissiongroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_CommissionGroup]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_CreatedBy]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_DeletedBy]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_Profile] FOREIGN KEY([Profile])
REFERENCES [dbo].[sys_userprofile] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_Profile]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_userdetail]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userdetail_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userdetail] CHECK CONSTRAINT [FK_sys_userdetail_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissiongroup_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissiongroup] CHECK CONSTRAINT [FK_sys_userpermissiongroup_CreatedBy]
GO
ALTER TABLE [dbo].[sys_userpermissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissiongroup_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissiongroup] CHECK CONSTRAINT [FK_sys_userpermissiongroup_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissiongroup_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissiongroup] CHECK CONSTRAINT [FK_sys_userpermissiongroup_DeletedBy]
GO
ALTER TABLE [dbo].[sys_userpermissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissiongroup_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissiongroup] CHECK CONSTRAINT [FK_sys_userpermissiongroup_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissiongroup_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissiongroup] CHECK CONSTRAINT [FK_sys_userpermissiongroup_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_userpermissiongroup]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissiongroup_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissiongroup] CHECK CONSTRAINT [FK_sys_userpermissiongroup_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_CreatedBy]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_DeletedBy]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_PermissionGroup] FOREIGN KEY([PermissionGroup])
REFERENCES [dbo].[sys_userpermissiongroup] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_PermissionGroup]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_userpermissionitem]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionitem_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionitem] CHECK CONSTRAINT [FK_sys_userpermissionitem_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_CreatedBy]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_DeletedBy]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_PermissionItem] FOREIGN KEY([PermissionItem])
REFERENCES [dbo].[sys_userpermissionitem] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_PermissionItem]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_UpdatedWhere]
GO
ALTER TABLE [dbo].[sys_userpermissionprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userpermissionprofile_UserProfile] FOREIGN KEY([UserProfile])
REFERENCES [dbo].[sys_userprofile] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userpermissionprofile] CHECK CONSTRAINT [FK_sys_userpermissionprofile_UserProfile]
GO
ALTER TABLE [dbo].[sys_userprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userprofile_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userprofile] CHECK CONSTRAINT [FK_sys_userprofile_CreatedBy]
GO
ALTER TABLE [dbo].[sys_userprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userprofile_CreatedWhere] FOREIGN KEY([CreatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userprofile] CHECK CONSTRAINT [FK_sys_userprofile_CreatedWhere]
GO
ALTER TABLE [dbo].[sys_userprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userprofile_DeletedBy] FOREIGN KEY([DeletedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userprofile] CHECK CONSTRAINT [FK_sys_userprofile_DeletedBy]
GO
ALTER TABLE [dbo].[sys_userprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userprofile_DeletedWhere] FOREIGN KEY([DeletedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userprofile] CHECK CONSTRAINT [FK_sys_userprofile_DeletedWhere]
GO
ALTER TABLE [dbo].[sys_userprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userprofile_UpdatedBy] FOREIGN KEY([UpdatedBy])
REFERENCES [dbo].[sys_userdetail] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userprofile] CHECK CONSTRAINT [FK_sys_userprofile_UpdatedBy]
GO
ALTER TABLE [dbo].[sys_userprofile]  WITH NOCHECK ADD  CONSTRAINT [FK_sys_userprofile_UpdatedWhere] FOREIGN KEY([UpdatedWhere])
REFERENCES [dbo].[pos_configurationplaceterminal] ([Oid])
NOT FOR REPLICATION 
GO
ALTER TABLE [dbo].[sys_userprofile] CHECK CONSTRAINT [FK_sys_userprofile_UpdatedWhere]
GO
/****** EOF GENERATED SCRIPT ******/

/****** Required Changes for XPO Defaults ******/

/****** Drop Index, else must have a CardNumber for every Customer ******/
DROP INDEX [iCardNumber_erp_customer] ON [dbo].[erp_customer]
GO

/****** Drop Index, else must have a BarCode for every Article ******/
DROP INDEX [iBarCode_fin_article] ON [dbo].[fin_article]
GO

/****** Object:  Index [[iFiscalNumber_erp_customer]]    Script Date: 16/04/2019 16:07:51 Related to: IN009061 ******/
CREATE UNIQUE NONCLUSTERED INDEX [iFiscalNumber_erp_customer] ON [dbo].[erp_customer]
(
	[FiscalNumber] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE TABLE [sys_databaseversion](
	[Version] [varchar](20) NOT NULL
) ON [PRIMARY]

GO