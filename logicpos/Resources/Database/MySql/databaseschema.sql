/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table cfg_configurationcountry
--

DROP TABLE IF EXISTS cfg_configurationcountry;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE cfg_configurationcountry (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Code2 varchar(5) DEFAULT NULL,
  Code3 varchar(6) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Capital varchar(100) DEFAULT NULL,
  TLD varchar(10) DEFAULT NULL,
  Currency varchar(20) DEFAULT NULL,
  CurrencyCode varchar(3) DEFAULT NULL,
  RegExFiscalNumber varchar(255) DEFAULT NULL,
  RegExZipCode varchar(255) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_cfg_configurationcountry (Oid),
  UNIQUE KEY iCode_cfg_configurationcountry (Code),
  UNIQUE KEY iCode2_cfg_configurationcountry (Code2),
  UNIQUE KEY iCode3_cfg_configurationcountry (Code3),
  UNIQUE KEY iDesignation_cfg_configurationcountry (Designation),
  KEY iCreatedBy_cfg_configurationcountry (CreatedBy),
  KEY iCreatedWhere_cfg_configurationcountry (CreatedWhere),
  KEY iUpdatedBy_cfg_configurationcountry (UpdatedBy),
  KEY iUpdatedWhere_cfg_configurationcountry (UpdatedWhere),
  KEY iDeletedBy_cfg_configurationcountry (DeletedBy),
  KEY iDeletedWhere_cfg_configurationcountry (DeletedWhere),
  CONSTRAINT FK_cfg_configurationcountry_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationcountry_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationcountry_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationcountry_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationcountry_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationcountry_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table cfg_configurationcurrency
--

DROP TABLE IF EXISTS cfg_configurationcurrency;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE cfg_configurationcurrency (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Acronym varchar(100) DEFAULT NULL,
  Symbol varchar(10) DEFAULT NULL,
  Entity text,
  ExchangeRate double DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_cfg_configurationcurrency (Oid),
  UNIQUE KEY iCode_cfg_configurationcurrency (Code),
  UNIQUE KEY iDesignation_cfg_configurationcurrency (Designation),
  KEY iCreatedBy_cfg_configurationcurrency (CreatedBy),
  KEY iCreatedWhere_cfg_configurationcurrency (CreatedWhere),
  KEY iUpdatedBy_cfg_configurationcurrency (UpdatedBy),
  KEY iUpdatedWhere_cfg_configurationcurrency (UpdatedWhere),
  KEY iDeletedBy_cfg_configurationcurrency (DeletedBy),
  KEY iDeletedWhere_cfg_configurationcurrency (DeletedWhere),
  CONSTRAINT FK_cfg_configurationcurrency_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationcurrency_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationcurrency_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationcurrency_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationcurrency_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationcurrency_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table cfg_configurationholidays
--

DROP TABLE IF EXISTS cfg_configurationholidays;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE cfg_configurationholidays (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Description varchar(255) DEFAULT NULL,
  Year int(11) DEFAULT NULL,
  Month int(11) DEFAULT NULL,
  Day int(11) DEFAULT NULL,
  Fixed bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_cfg_configurationholidays (Oid),
  UNIQUE KEY iCode_cfg_configurationholidays (Code),
  UNIQUE KEY iDesignation_cfg_configurationholidays (Designation),
  KEY iCreatedBy_cfg_configurationholidays (CreatedBy),
  KEY iCreatedWhere_cfg_configurationholidays (CreatedWhere),
  KEY iUpdatedBy_cfg_configurationholidays (UpdatedBy),
  KEY iUpdatedWhere_cfg_configurationholidays (UpdatedWhere),
  KEY iDeletedBy_cfg_configurationholidays (DeletedBy),
  KEY iDeletedWhere_cfg_configurationholidays (DeletedWhere),
  CONSTRAINT FK_cfg_configurationholidays_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationholidays_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationholidays_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationholidays_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationholidays_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationholidays_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table cfg_configurationpreferenceparameter
--

DROP TABLE IF EXISTS cfg_configurationpreferenceparameter;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE cfg_configurationpreferenceparameter (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  Value longtext DEFAULT NULL,
  ValueTip varchar(100) DEFAULT NULL,
  Required bit(1) DEFAULT NULL,
  RegEx varchar(255) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  ResourceStringInfo varchar(255) DEFAULT NULL,
  FormType int(11) DEFAULT NULL,
  FormPageNo int(11) DEFAULT NULL,
  InputType int(11) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_cfg_configurationpreferenceparameter (Oid),
  UNIQUE KEY iCode_cfg_configurationpreferenceparameter (Code),
  UNIQUE KEY iToken_cfg_configurationpreferenceparameter (Token),
  UNIQUE KEY iResourceString_cfg_configurationpreferenceparameter (ResourceString),
  KEY iCreatedBy_cfg_configurationpreferenceparameter (CreatedBy),
  KEY iCreatedWhere_cfg_configurationpreferenceparameter (CreatedWhere),
  KEY iUpdatedBy_cfg_configurationpreferenceparameter (UpdatedBy),
  KEY iUpdatedWhere_cfg_configurationpreferenceparameter (UpdatedWhere),
  KEY iDeletedBy_cfg_configurationpreferenceparameter (DeletedBy),
  KEY iDeletedWhere_cfg_configurationpreferenceparameter (DeletedWhere),
  CONSTRAINT FK_cfg_configurationpreferenceparameter_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationpreferenceparameter_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationpreferenceparameter_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationpreferenceparameter_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationpreferenceparameter_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationpreferenceparameter_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table cfg_configurationunitmeasure
--

DROP TABLE IF EXISTS cfg_configurationunitmeasure;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE cfg_configurationunitmeasure (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Acronym varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_cfg_configurationunitmeasure (Oid),
  UNIQUE KEY iCode_cfg_configurationunitmeasure (Code),
  UNIQUE KEY iDesignation_cfg_configurationunitmeasure (Designation),
  UNIQUE KEY iAcronym_cfg_configurationunitmeasure (Acronym),
  KEY iCreatedBy_cfg_configurationunitmeasure (CreatedBy),
  KEY iCreatedWhere_cfg_configurationunitmeasure (CreatedWhere),
  KEY iUpdatedBy_cfg_configurationunitmeasure (UpdatedBy),
  KEY iUpdatedWhere_cfg_configurationunitmeasure (UpdatedWhere),
  KEY iDeletedBy_cfg_configurationunitmeasure (DeletedBy),
  KEY iDeletedWhere_cfg_configurationunitmeasure (DeletedWhere),
  CONSTRAINT FK_cfg_configurationunitmeasure_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationunitmeasure_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationunitmeasure_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationunitmeasure_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationunitmeasure_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationunitmeasure_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table cfg_configurationunitsize
--

DROP TABLE IF EXISTS cfg_configurationunitsize;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE cfg_configurationunitsize (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_cfg_configurationunitsize (Oid),
  UNIQUE KEY iCode_cfg_configurationunitsize (Code),
  UNIQUE KEY iDesignation_cfg_configurationunitsize (Designation),
  KEY iCreatedBy_cfg_configurationunitsize (CreatedBy),
  KEY iCreatedWhere_cfg_configurationunitsize (CreatedWhere),
  KEY iUpdatedBy_cfg_configurationunitsize (UpdatedBy),
  KEY iUpdatedWhere_cfg_configurationunitsize (UpdatedWhere),
  KEY iDeletedBy_cfg_configurationunitsize (DeletedBy),
  KEY iDeletedWhere_cfg_configurationunitsize (DeletedWhere),
  CONSTRAINT FK_cfg_configurationunitsize_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationunitsize_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationunitsize_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationunitsize_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_cfg_configurationunitsize_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_cfg_configurationunitsize_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table erp_customer
--

DROP TABLE IF EXISTS erp_customer;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE erp_customer (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  CodeInternal varchar(30) DEFAULT NULL,
  Name text,
  Address text,
  Locality varchar(255) DEFAULT NULL,
  ZipCode varchar(100) DEFAULT NULL,
  City varchar(255) DEFAULT NULL,
  DateOfBirth varchar(100) DEFAULT NULL,
  Phone varchar(255) DEFAULT NULL,
  Fax varchar(255) DEFAULT NULL,
  MobilePhone varchar(255) DEFAULT NULL,
  Email varchar(255) DEFAULT NULL,
  WebSite varchar(255) DEFAULT NULL,
  FiscalNumber varchar(100) DEFAULT NULL,
  CardNumber varchar(100) DEFAULT NULL,
  DiscountType varchar(100) DEFAULT NULL,
  Discount double DEFAULT NULL,
  CardCredit double DEFAULT NULL,
  TotalDebt double DEFAULT NULL,
  TotalCredit double DEFAULT NULL,
  CurrentBalance double DEFAULT NULL,
  CreditLine varchar(100) DEFAULT NULL,
  Remarks varchar(100) DEFAULT NULL,
  Supplier bit(1) DEFAULT NULL,
  Hidden bit(1) DEFAULT NULL,
  CustomerType char(38) DEFAULT NULL,
  DiscountGroup char(38) DEFAULT NULL,
  PriceType char(38) DEFAULT NULL,
  Country char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_erp_customer (Oid),
  UNIQUE KEY iCodeInternal_erp_customer (CodeInternal),
  UNIQUE KEY iCardNumber_erp_customer (CardNumber),
  UNIQUE KEY iFiscalNumber_erp_customer (FiscalNumber),
  KEY iCreatedBy_erp_customer (CreatedBy),
  KEY iCreatedWhere_erp_customer (CreatedWhere),
  KEY iUpdatedBy_erp_customer (UpdatedBy),
  KEY iUpdatedWhere_erp_customer (UpdatedWhere),
  KEY iDeletedBy_erp_customer (DeletedBy),
  KEY iDeletedWhere_erp_customer (DeletedWhere),
  KEY iCustomerType_erp_customer (CustomerType),
  KEY iDiscountGroup_erp_customer (DiscountGroup),
  KEY iPriceType_erp_customer (PriceType),
  KEY iCountry_erp_customer (Country),
  CONSTRAINT FK_erp_customer_Country FOREIGN KEY (Country) REFERENCES cfg_configurationcountry (Oid),
  CONSTRAINT FK_erp_customer_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customer_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_erp_customer_CustomerType FOREIGN KEY (CustomerType) REFERENCES erp_customertype (Oid),
  CONSTRAINT FK_erp_customer_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customer_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_erp_customer_DiscountGroup FOREIGN KEY (DiscountGroup) REFERENCES erp_customerdiscountgroup (Oid),
  CONSTRAINT FK_erp_customer_PriceType FOREIGN KEY (PriceType) REFERENCES fin_configurationpricetype (Oid),
  CONSTRAINT FK_erp_customer_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customer_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table erp_customerdiscountgroup
--

DROP TABLE IF EXISTS erp_customerdiscountgroup;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE erp_customerdiscountgroup (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_erp_customerdiscountgroup (Oid),
  UNIQUE KEY iCode_erp_customerdiscountgroup (Code),
  UNIQUE KEY iDesignation_erp_customerdiscountgroup (Designation),
  KEY iCreatedBy_erp_customerdiscountgroup (CreatedBy),
  KEY iCreatedWhere_erp_customerdiscountgroup (CreatedWhere),
  KEY iUpdatedBy_erp_customerdiscountgroup (UpdatedBy),
  KEY iUpdatedWhere_erp_customerdiscountgroup (UpdatedWhere),
  KEY iDeletedBy_erp_customerdiscountgroup (DeletedBy),
  KEY iDeletedWhere_erp_customerdiscountgroup (DeletedWhere),
  CONSTRAINT FK_erp_customerdiscountgroup_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customerdiscountgroup_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_erp_customerdiscountgroup_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customerdiscountgroup_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_erp_customerdiscountgroup_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customerdiscountgroup_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table erp_customertype
--

DROP TABLE IF EXISTS erp_customertype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE erp_customertype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_erp_customertype (Oid),
  UNIQUE KEY iCode_erp_customertype (Code),
  UNIQUE KEY iDesignation_erp_customertype (Designation),
  KEY iCreatedBy_erp_customertype (CreatedBy),
  KEY iCreatedWhere_erp_customertype (CreatedWhere),
  KEY iUpdatedBy_erp_customertype (UpdatedBy),
  KEY iUpdatedWhere_erp_customertype (UpdatedWhere),
  KEY iDeletedBy_erp_customertype (DeletedBy),
  KEY iDeletedWhere_erp_customertype (DeletedWhere),
  CONSTRAINT FK_erp_customertype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customertype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_erp_customertype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customertype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_erp_customertype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_erp_customertype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_article
--

DROP TABLE IF EXISTS fin_article;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_article (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code varchar(25) DEFAULT NULL,
  CodeDealer varchar(25) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ButtonLabel varchar(35) DEFAULT NULL,
  ButtonLabelHide bit(1) DEFAULT NULL,
  ButtonImage varchar(255) DEFAULT NULL,
  ButtonIcon varchar(255) DEFAULT NULL,
  Price1 double DEFAULT NULL,
  Price2 double DEFAULT NULL,
  Price3 double DEFAULT NULL,
  Price4 double DEFAULT NULL,
  Price5 double DEFAULT NULL,
  Price1Promotion double DEFAULT NULL,
  Price2Promotion double DEFAULT NULL,
  Price3Promotion double DEFAULT NULL,
  Price4Promotion double DEFAULT NULL,
  Price5Promotion double DEFAULT NULL,
  Price1UsePromotionPrice bit(1) DEFAULT NULL,
  Price2UsePromotionPrice bit(1) DEFAULT NULL,
  Price3UsePromotionPrice bit(1) DEFAULT NULL,
  Price4UsePromotionPrice bit(1) DEFAULT NULL,
  Price5UsePromotionPrice bit(1) DEFAULT NULL,
  PriceWithVat bit(1) DEFAULT NULL,
  Discount double DEFAULT NULL,
  DefaultQuantity double DEFAULT NULL,
  Accounting double DEFAULT NULL,
  Tare double DEFAULT NULL,
  Weight double DEFAULT NULL,
  BarCode varchar(100) DEFAULT NULL,
  PVPVariable bit(1) DEFAULT NULL,
  Favorite bit(1) DEFAULT NULL,
  UseWeighingBalance bit(1) DEFAULT NULL,
  Token1 varchar(255) DEFAULT NULL,
  Token2 varchar(255) DEFAULT NULL,
  Family char(38) DEFAULT NULL,
  SubFamily char(38) DEFAULT NULL,
  Type char(38) DEFAULT NULL,
  Class char(38) DEFAULT NULL,
  UnitMeasure char(38) DEFAULT NULL,
  UnitSize char(38) DEFAULT NULL,
  CommissionGroup char(38) DEFAULT NULL,
  DiscountGroup char(38) DEFAULT NULL,
  VatOnTable char(38) DEFAULT NULL,
  VatDirectSelling char(38) DEFAULT NULL,
  VatExemptionReason char(38) DEFAULT NULL,
  Printer char(38) DEFAULT NULL,
  Template char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_article (Oid),
  UNIQUE KEY iCode_fin_article (Code),
  UNIQUE KEY iDesignation_fin_article (Designation),
  UNIQUE KEY iBarCode_fin_article (BarCode),
  KEY iCreatedBy_fin_article (CreatedBy),
  KEY iCreatedWhere_fin_article (CreatedWhere),
  KEY iUpdatedBy_fin_article (UpdatedBy),
  KEY iUpdatedWhere_fin_article (UpdatedWhere),
  KEY iDeletedBy_fin_article (DeletedBy),
  KEY iDeletedWhere_fin_article (DeletedWhere),
  KEY iFamily_fin_article (Family),
  KEY iSubFamily_fin_article (SubFamily),
  KEY iType_fin_article (Type),
  KEY iClass_fin_article (Class),
  KEY iUnitMeasure_fin_article (UnitMeasure),
  KEY iUnitSize_fin_article (UnitSize),
  KEY iCommissionGroup_fin_article (CommissionGroup),
  KEY iDiscountGroup_fin_article (DiscountGroup),
  KEY iVatOnTable_fin_article (VatOnTable),
  KEY iVatDirectSelling_fin_article (VatDirectSelling),
  KEY iVatExemptionReason_fin_article (VatExemptionReason),
  KEY iPrinter_fin_article (Printer),
  KEY iTemplate_fin_article (Template),
  CONSTRAINT FK_fin_article_Class FOREIGN KEY (Class) REFERENCES fin_articleclass (Oid),
  CONSTRAINT FK_fin_article_CommissionGroup FOREIGN KEY (CommissionGroup) REFERENCES pos_usercommissiongroup (Oid),
  CONSTRAINT FK_fin_article_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_article_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_article_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_article_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_article_DiscountGroup FOREIGN KEY (DiscountGroup) REFERENCES erp_customerdiscountgroup (Oid),
  CONSTRAINT FK_fin_article_Family FOREIGN KEY (Family) REFERENCES fin_articlefamily (Oid),
  CONSTRAINT FK_fin_article_Printer FOREIGN KEY (Printer) REFERENCES sys_configurationprinters (Oid),
  CONSTRAINT FK_fin_article_SubFamily FOREIGN KEY (SubFamily) REFERENCES fin_articlesubfamily (Oid),
  CONSTRAINT FK_fin_article_Template FOREIGN KEY (Template) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_fin_article_Type FOREIGN KEY (Type) REFERENCES fin_articletype (Oid),
  CONSTRAINT FK_fin_article_UnitMeasure FOREIGN KEY (UnitMeasure) REFERENCES cfg_configurationunitmeasure (Oid),
  CONSTRAINT FK_fin_article_UnitSize FOREIGN KEY (UnitSize) REFERENCES cfg_configurationunitsize (Oid),
  CONSTRAINT FK_fin_article_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_article_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_article_VatDirectSelling FOREIGN KEY (VatDirectSelling) REFERENCES fin_configurationvatrate (Oid),
  CONSTRAINT FK_fin_article_VatExemptionReason FOREIGN KEY (VatExemptionReason) REFERENCES fin_configurationvatexemptionreason (Oid),
  CONSTRAINT FK_fin_article_VatOnTable FOREIGN KEY (VatOnTable) REFERENCES fin_configurationvatrate (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_articleclass
--

DROP TABLE IF EXISTS fin_articleclass;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_articleclass (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Acronym varchar(1) DEFAULT NULL,
  WorkInStock bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_articleclass (Oid),
  UNIQUE KEY iCode_fin_articleclass (Code),
  UNIQUE KEY iDesignation_fin_articleclass (Designation),
  UNIQUE KEY iAcronym_fin_articleclass (Acronym),
  KEY iCreatedBy_fin_articleclass (CreatedBy),
  KEY iCreatedWhere_fin_articleclass (CreatedWhere),
  KEY iUpdatedBy_fin_articleclass (UpdatedBy),
  KEY iUpdatedWhere_fin_articleclass (UpdatedWhere),
  KEY iDeletedBy_fin_articleclass (DeletedBy),
  KEY iDeletedWhere_fin_articleclass (DeletedWhere),
  CONSTRAINT FK_fin_articleclass_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articleclass_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articleclass_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articleclass_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articleclass_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articleclass_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_articlefamily
--

DROP TABLE IF EXISTS fin_articlefamily;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_articlefamily (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ButtonLabel varchar(35) DEFAULT NULL,
  ButtonLabelHide bit(1) DEFAULT NULL,
  ButtonImage varchar(255) DEFAULT NULL,
  ButtonIcon varchar(255) DEFAULT NULL,
  CommissionGroup char(38) DEFAULT NULL,
  DiscountGroup char(38) DEFAULT NULL,
  Printer char(38) DEFAULT NULL,
  Template char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_articlefamily (Oid),
  UNIQUE KEY iCode_fin_articlefamily (Code),
  UNIQUE KEY iDesignation_fin_articlefamily (Designation),
  KEY iCreatedBy_fin_articlefamily (CreatedBy),
  KEY iCreatedWhere_fin_articlefamily (CreatedWhere),
  KEY iUpdatedBy_fin_articlefamily (UpdatedBy),
  KEY iUpdatedWhere_fin_articlefamily (UpdatedWhere),
  KEY iDeletedBy_fin_articlefamily (DeletedBy),
  KEY iDeletedWhere_fin_articlefamily (DeletedWhere),
  KEY iCommissionGroup_fin_articlefamily (CommissionGroup),
  KEY iDiscountGroup_fin_articlefamily (DiscountGroup),
  KEY iPrinter_fin_articlefamily (Printer),
  KEY iTemplate_fin_articlefamily (Template),
  CONSTRAINT FK_fin_articlefamily_CommissionGroup FOREIGN KEY (CommissionGroup) REFERENCES pos_usercommissiongroup (Oid),
  CONSTRAINT FK_fin_articlefamily_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlefamily_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlefamily_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlefamily_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlefamily_DiscountGroup FOREIGN KEY (DiscountGroup) REFERENCES erp_customerdiscountgroup (Oid),
  CONSTRAINT FK_fin_articlefamily_Printer FOREIGN KEY (Printer) REFERENCES sys_configurationprinters (Oid),
  CONSTRAINT FK_fin_articlefamily_Template FOREIGN KEY (Template) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_fin_articlefamily_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlefamily_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_articlestock
--

DROP TABLE IF EXISTS fin_articlestock;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_articlestock (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Date datetime DEFAULT NULL,
  Customer char(38) DEFAULT NULL,
  DocumentNumber varchar(50) DEFAULT NULL,
  Article char(38) DEFAULT NULL,
  Quantity double DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  DocumentDetail char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_articlestock (Oid),
  KEY iCreatedBy_fin_articlestock (CreatedBy),
  KEY iCreatedWhere_fin_articlestock (CreatedWhere),
  KEY iUpdatedBy_fin_articlestock (UpdatedBy),
  KEY iUpdatedWhere_fin_articlestock (UpdatedWhere),
  KEY iDeletedBy_fin_articlestock (DeletedBy),
  KEY iDeletedWhere_fin_articlestock (DeletedWhere),
  KEY iCustomer_fin_articlestock (Customer),
  KEY iArticle_fin_articlestock (Article),
  KEY iDocumentMaster_fin_articlestock (DocumentMaster),
  KEY iDocumentDetail_fin_articlestock (DocumentDetail),
  CONSTRAINT FK_fin_articlestock_Article FOREIGN KEY (Article) REFERENCES fin_article (Oid),
  CONSTRAINT FK_fin_articlestock_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlestock_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlestock_Customer FOREIGN KEY (Customer) REFERENCES erp_customer (Oid),
  CONSTRAINT FK_fin_articlestock_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlestock_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlestock_DocumentDetail FOREIGN KEY (DocumentDetail) REFERENCES fin_documentfinancedetail (Oid),
  CONSTRAINT FK_fin_articlestock_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_articlestock_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlestock_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_articlesubfamily
--

DROP TABLE IF EXISTS fin_articlesubfamily;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_articlesubfamily (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ButtonLabel varchar(35) DEFAULT NULL,
  ButtonLabelHide bit(1) DEFAULT NULL,
  ButtonImage varchar(255) DEFAULT NULL,
  ButtonIcon varchar(255) DEFAULT NULL,
  Family char(38) DEFAULT NULL,
  CommissionGroup char(38) DEFAULT NULL,
  DiscountGroup char(38) DEFAULT NULL,
  VatOnTable char(38) DEFAULT NULL,
  VatDirectSelling char(38) DEFAULT NULL,
  Printer char(38) DEFAULT NULL,
  Template char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_articlesubfamily (Oid),
  UNIQUE KEY iCode_fin_articlesubfamily (Code),
  KEY iCreatedBy_fin_articlesubfamily (CreatedBy),
  KEY iCreatedWhere_fin_articlesubfamily (CreatedWhere),
  KEY iUpdatedBy_fin_articlesubfamily (UpdatedBy),
  KEY iUpdatedWhere_fin_articlesubfamily (UpdatedWhere),
  KEY iDeletedBy_fin_articlesubfamily (DeletedBy),
  KEY iDeletedWhere_fin_articlesubfamily (DeletedWhere),
  KEY iFamily_fin_articlesubfamily (Family),
  KEY iCommissionGroup_fin_articlesubfamily (CommissionGroup),
  KEY iDiscountGroup_fin_articlesubfamily (DiscountGroup),
  KEY iVatOnTable_fin_articlesubfamily (VatOnTable),
  KEY iVatDirectSelling_fin_articlesubfamily (VatDirectSelling),
  KEY iPrinter_fin_articlesubfamily (Printer),
  KEY iTemplate_fin_articlesubfamily (Template),
  CONSTRAINT FK_fin_articlesubfamily_CommissionGroup FOREIGN KEY (CommissionGroup) REFERENCES pos_usercommissiongroup (Oid),
  CONSTRAINT FK_fin_articlesubfamily_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlesubfamily_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlesubfamily_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlesubfamily_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlesubfamily_DiscountGroup FOREIGN KEY (DiscountGroup) REFERENCES erp_customerdiscountgroup (Oid),
  CONSTRAINT FK_fin_articlesubfamily_Family FOREIGN KEY (Family) REFERENCES fin_articlefamily (Oid),
  CONSTRAINT FK_fin_articlesubfamily_Printer FOREIGN KEY (Printer) REFERENCES sys_configurationprinters (Oid),
  CONSTRAINT FK_fin_articlesubfamily_Template FOREIGN KEY (Template) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_fin_articlesubfamily_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articlesubfamily_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articlesubfamily_VatDirectSelling FOREIGN KEY (VatDirectSelling) REFERENCES fin_configurationvatrate (Oid),
  CONSTRAINT FK_fin_articlesubfamily_VatOnTable FOREIGN KEY (VatOnTable) REFERENCES fin_configurationvatrate (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_articletype
--

DROP TABLE IF EXISTS fin_articletype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_articletype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  HavePrice bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_articletype (Oid),
  UNIQUE KEY iCode_fin_articletype (Code),
  UNIQUE KEY iDesignation_fin_articletype (Designation),
  KEY iCreatedBy_fin_articletype (CreatedBy),
  KEY iCreatedWhere_fin_articletype (CreatedWhere),
  KEY iUpdatedBy_fin_articletype (UpdatedBy),
  KEY iUpdatedWhere_fin_articletype (UpdatedWhere),
  KEY iDeletedBy_fin_articletype (DeletedBy),
  KEY iDeletedWhere_fin_articletype (DeletedWhere),
  CONSTRAINT FK_fin_articletype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articletype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articletype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articletype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_articletype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_articletype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_configurationpaymentcondition
--

DROP TABLE IF EXISTS fin_configurationpaymentcondition;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_configurationpaymentcondition (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Acronym varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_configurationpaymentcondition (Oid),
  UNIQUE KEY iCode_fin_configurationpaymentcondition (Code),
  UNIQUE KEY iDesignation_fin_configurationpaymentcondition (Designation),
  KEY iCreatedBy_fin_configurationpaymentcondition (CreatedBy),
  KEY iCreatedWhere_fin_configurationpaymentcondition (CreatedWhere),
  KEY iUpdatedBy_fin_configurationpaymentcondition (UpdatedBy),
  KEY iUpdatedWhere_fin_configurationpaymentcondition (UpdatedWhere),
  KEY iDeletedBy_fin_configurationpaymentcondition (DeletedBy),
  KEY iDeletedWhere_fin_configurationpaymentcondition (DeletedWhere),
  CONSTRAINT FK_fin_configurationpaymentcondition_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpaymentcondition_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationpaymentcondition_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpaymentcondition_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationpaymentcondition_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpaymentcondition_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_configurationpaymentmethod
--

DROP TABLE IF EXISTS fin_configurationpaymentmethod;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_configurationpaymentmethod (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  ButtonIcon varchar(255) DEFAULT NULL,
  Acronym varchar(100) DEFAULT NULL,
  AllowPayback varchar(100) DEFAULT NULL,
  Symbol varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_configurationpaymentmethod (Oid),
  UNIQUE KEY iCode_fin_configurationpaymentmethod (Code),
  UNIQUE KEY iToken_fin_configurationpaymentmethod (Token),
  UNIQUE KEY iDesignation_fin_configurationpaymentmethod (Designation),
  UNIQUE KEY iResourceString_fin_configurationpaymentmethod (ResourceString),
  KEY iCreatedBy_fin_configurationpaymentmethod (CreatedBy),
  KEY iCreatedWhere_fin_configurationpaymentmethod (CreatedWhere),
  KEY iUpdatedBy_fin_configurationpaymentmethod (UpdatedBy),
  KEY iUpdatedWhere_fin_configurationpaymentmethod (UpdatedWhere),
  KEY iDeletedBy_fin_configurationpaymentmethod (DeletedBy),
  KEY iDeletedWhere_fin_configurationpaymentmethod (DeletedWhere),
  CONSTRAINT FK_fin_configurationpaymentmethod_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpaymentmethod_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationpaymentmethod_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpaymentmethod_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationpaymentmethod_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpaymentmethod_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_configurationpricetype
--

DROP TABLE IF EXISTS fin_configurationpricetype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_configurationpricetype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  EnumValue int(11) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_configurationpricetype (Oid),
  UNIQUE KEY iCode_fin_configurationpricetype (Code),
  UNIQUE KEY iDesignation_fin_configurationpricetype (Designation),
  UNIQUE KEY iEnumValue_fin_configurationpricetype (EnumValue),
  KEY iCreatedBy_fin_configurationpricetype (CreatedBy),
  KEY iCreatedWhere_fin_configurationpricetype (CreatedWhere),
  KEY iUpdatedBy_fin_configurationpricetype (UpdatedBy),
  KEY iUpdatedWhere_fin_configurationpricetype (UpdatedWhere),
  KEY iDeletedBy_fin_configurationpricetype (DeletedBy),
  KEY iDeletedWhere_fin_configurationpricetype (DeletedWhere),
  CONSTRAINT FK_fin_configurationpricetype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpricetype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationpricetype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpricetype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationpricetype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationpricetype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_configurationvatexemptionreason
--

DROP TABLE IF EXISTS fin_configurationvatexemptionreason;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_configurationvatexemptionreason (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(60) DEFAULT NULL,
  Acronym varchar(3) DEFAULT NULL,
  StandardApplicable text,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_configurationvatexemptionreason (Oid),
  UNIQUE KEY iCode_fin_configurationvatexemptionreason (Code),
  KEY iCreatedBy_fin_configurationvatexemptionreason (CreatedBy),
  KEY iCreatedWhere_fin_configurationvatexemptionreason (CreatedWhere),
  KEY iUpdatedBy_fin_configurationvatexemptionreason (UpdatedBy),
  KEY iUpdatedWhere_fin_configurationvatexemptionreason (UpdatedWhere),
  KEY iDeletedBy_fin_configurationvatexemptionreason (DeletedBy),
  KEY iDeletedWhere_fin_configurationvatexemptionreason (DeletedWhere),
  CONSTRAINT FK_fin_configurationvatexemptionreason_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationvatexemptionreason_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationvatexemptionreason_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationvatexemptionreason_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationvatexemptionreason_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationvatexemptionreason_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_configurationvatrate
--

DROP TABLE IF EXISTS fin_configurationvatrate;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_configurationvatrate (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Value double DEFAULT NULL,
  ReasonCode varchar(100) DEFAULT NULL,
  TaxType varchar(3) DEFAULT NULL,
  TaxCode varchar(10) DEFAULT NULL,
  TaxCountryRegion varchar(5) DEFAULT NULL,
  TaxExpirationDate datetime DEFAULT NULL,
  TaxDescription varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_configurationvatrate (Oid),
  UNIQUE KEY iCode_fin_configurationvatrate (Code),
  UNIQUE KEY iDesignation_fin_configurationvatrate (Designation),
  KEY iCreatedBy_fin_configurationvatrate (CreatedBy),
  KEY iCreatedWhere_fin_configurationvatrate (CreatedWhere),
  KEY iUpdatedBy_fin_configurationvatrate (UpdatedBy),
  KEY iUpdatedWhere_fin_configurationvatrate (UpdatedWhere),
  KEY iDeletedBy_fin_configurationvatrate (DeletedBy),
  KEY iDeletedWhere_fin_configurationvatrate (DeletedWhere),
  CONSTRAINT FK_fin_configurationvatrate_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationvatrate_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationvatrate_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationvatrate_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_configurationvatrate_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_configurationvatrate_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancecommission
--

DROP TABLE IF EXISTS fin_documentfinancecommission;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancecommission (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Date datetime DEFAULT NULL,
  Commission double DEFAULT NULL,
  Total double DEFAULT NULL,
  CommissionGroup char(38) DEFAULT NULL,
  FinanceMaster char(38) DEFAULT NULL,
  FinanceDetail char(38) DEFAULT NULL,
  UserDetail char(38) DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancecommission (Oid),
  KEY iCreatedBy_fin_documentfinancecommission (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancecommission (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancecommission (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancecommission (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancecommission (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancecommission (DeletedWhere),
  KEY iCommissionGroup_fin_documentfinancecommission (CommissionGroup),
  KEY iFinanceMaster_fin_documentfinancecommission (FinanceMaster),
  KEY iFinanceDetail_fin_documentfinancecommission (FinanceDetail),
  KEY iUserDetail_fin_documentfinancecommission (UserDetail),
  KEY iTerminal_fin_documentfinancecommission (Terminal),
  CONSTRAINT FK_fin_documentfinancecommission_CommissionGroup FOREIGN KEY (CommissionGroup) REFERENCES pos_usercommissiongroup (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_FinanceDetail FOREIGN KEY (FinanceDetail) REFERENCES fin_documentfinancedetail (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_FinanceMaster FOREIGN KEY (FinanceMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancecommission_UserDetail FOREIGN KEY (UserDetail) REFERENCES sys_userdetail (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancedetail
--

DROP TABLE IF EXISTS fin_documentfinancedetail;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancedetail (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code varchar(100) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Quantity double DEFAULT NULL,
  UnitMeasure varchar(35) DEFAULT NULL,
  Price double DEFAULT NULL,
  Vat double DEFAULT NULL,
  VatExemptionReasonDesignation varchar(255) DEFAULT NULL,
  Discount double DEFAULT NULL,
  TotalNet double DEFAULT NULL,
  TotalGross double DEFAULT NULL,
  TotalDiscount double DEFAULT NULL,
  TotalTax double DEFAULT NULL,
  TotalFinal double DEFAULT NULL,
  PriceType int(11) DEFAULT NULL,
  PriceFinal double DEFAULT NULL,
  Token1 varchar(255) DEFAULT NULL,
  Token2 varchar(255) DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  Article char(38) DEFAULT NULL,
  VatRate char(38) DEFAULT NULL,
  VatExemptionReason char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancedetail (Oid),
  KEY iCreatedBy_fin_documentfinancedetail (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancedetail (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancedetail (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancedetail (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancedetail (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancedetail (DeletedWhere),
  KEY iDocumentMaster_fin_documentfinancedetail (DocumentMaster),
  KEY iArticle_fin_documentfinancedetail (Article),
  KEY iVatRate_fin_documentfinancedetail (VatRate),
  KEY iVatExemptionReason_fin_documentfinancedetail (VatExemptionReason),
  CONSTRAINT FK_fin_documentfinancedetail_Article FOREIGN KEY (Article) REFERENCES fin_article (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_VatExemptionReason FOREIGN KEY (VatExemptionReason) REFERENCES fin_configurationvatexemptionreason (Oid),
  CONSTRAINT FK_fin_documentfinancedetail_VatRate FOREIGN KEY (VatRate) REFERENCES fin_configurationvatrate (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancedetailorderreference
--

DROP TABLE IF EXISTS fin_documentfinancedetailorderreference;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancedetailorderreference (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  OriginatingON varchar(60) DEFAULT NULL,
  OrderDate datetime DEFAULT NULL,
  DocumentDetail char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancedetailorderreference (Oid),
  KEY iCreatedBy_fin_documentfinancedetailorderreference (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancedetailorderreference (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancedetailorderreference (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancedetailorderreference (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancedetailorderreference (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancedetailorderreference (DeletedWhere),
  KEY iDocumentMaster_fin_documentfinancedetailorderreference (DocumentMaster),
  KEY iDocumentDetail_fin_documentfinancedetailorderreference (DocumentDetail),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_DocumentDetail FOREIGN KEY (DocumentDetail) REFERENCES fin_documentfinancedetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailorderreference_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancedetailreference
--

DROP TABLE IF EXISTS fin_documentfinancedetailreference;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancedetailreference (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  Reference varchar(60) DEFAULT NULL,
  Reason longtext DEFAULT NULL,
  DocumentDetail char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancedetailreference (Oid),
  KEY iCreatedBy_fin_documentfinancedetailreference (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancedetailreference (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancedetailreference (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancedetailreference (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancedetailreference (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancedetailreference (DeletedWhere),
  KEY iDocumentMaster_fin_documentfinancedetailreference (DocumentMaster),
  KEY iDocumentDetail_fin_documentfinancedetailreference (DocumentDetail),
  CONSTRAINT FK_fin_documentfinancedetailreference_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_DocumentDetail FOREIGN KEY (DocumentDetail) REFERENCES fin_documentfinancedetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancedetailreference_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancemaster
--

DROP TABLE IF EXISTS fin_documentfinancemaster;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancemaster (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Date datetime DEFAULT NULL,
  DocumentNumber varchar(50) DEFAULT NULL,
  DocumentStatusStatus varchar(1) DEFAULT NULL,
  DocumentStatusDate varchar(19) DEFAULT NULL,
  DocumentStatusReason varchar(50) DEFAULT NULL,
  DocumentStatusUser varchar(30) DEFAULT NULL,
  SourceBilling varchar(1) DEFAULT NULL,
  Hash varchar(172) DEFAULT NULL,
  HashControl varchar(40) DEFAULT NULL,
  DocumentDate varchar(19) DEFAULT NULL,
  SelfBillingIndicator int(11) DEFAULT NULL,
  CashVatSchemeIndicator int(11) DEFAULT NULL,
  ThirdPartiesBillingIndicator int(11) DEFAULT NULL,
  DocumentCreatorUser varchar(30) DEFAULT NULL,
  EACCode varchar(5) DEFAULT NULL,
  SystemEntryDate varchar(50) DEFAULT NULL,
  TransactionID varchar(70) DEFAULT NULL,
  ShipToDeliveryID varchar(255) DEFAULT NULL,
  ShipToDeliveryDate datetime DEFAULT NULL,
  ShipToWarehouseID varchar(50) DEFAULT NULL,
  ShipToLocationID varchar(30) DEFAULT NULL,
  ShipToBuildingNumber varchar(10) DEFAULT NULL,
  ShipToStreetName varchar(90) DEFAULT NULL,
  ShipToAddressDetail varchar(100) DEFAULT NULL,
  ShipToCity varchar(50) DEFAULT NULL,
  ShipToPostalCode varchar(20) DEFAULT NULL,
  ShipToRegion varchar(50) DEFAULT NULL,
  ShipToCountry varchar(5) DEFAULT NULL,
  ShipFromDeliveryID varchar(255) DEFAULT NULL,
  ShipFromDeliveryDate datetime DEFAULT NULL,
  ShipFromWarehouseID varchar(50) DEFAULT NULL,
  ShipFromLocationID varchar(30) DEFAULT NULL,
  ShipFromBuildingNumber varchar(10) DEFAULT NULL,
  ShipFromStreetName varchar(90) DEFAULT NULL,
  ShipFromAddressDetail varchar(100) DEFAULT NULL,
  ShipFromCity varchar(50) DEFAULT NULL,
  ShipFromPostalCode varchar(20) DEFAULT NULL,
  ShipFromRegion varchar(50) DEFAULT NULL,
  ShipFromCountry varchar(5) DEFAULT NULL,
  MovementStartTime datetime DEFAULT NULL,
  MovementEndTime datetime DEFAULT NULL,
  TotalNet double DEFAULT NULL,
  TotalGross double DEFAULT NULL,
  TotalDiscount double DEFAULT NULL,
  TotalTax double DEFAULT NULL,
  TotalFinal double DEFAULT NULL,
  TotalFinalRound double DEFAULT NULL,
  TotalDelivery double DEFAULT NULL,
  TotalChange double DEFAULT NULL,
  ExternalDocument varchar(50) DEFAULT NULL,
  Discount double DEFAULT NULL,
  DiscountFinancial double DEFAULT NULL,
  ExchangeRate double DEFAULT NULL,
  EntityOid char(38) DEFAULT NULL,
  EntityInternalCode varchar(30) DEFAULT NULL,
  EntityName varchar(512) DEFAULT NULL,
  EntityAddress varchar(512) DEFAULT NULL,
  EntityLocality varchar(255) DEFAULT NULL,
  EntityZipCode varchar(100) DEFAULT NULL,
  EntityCity varchar(255) DEFAULT NULL,
  EntityCountry varchar(5) DEFAULT NULL,
  EntityCountryOid char(38) DEFAULT NULL,
  EntityFiscalNumber varchar(100) DEFAULT NULL,
  Payed bit(1) DEFAULT NULL,
  PayedDate datetime DEFAULT NULL,
  Printed bit(1) DEFAULT NULL,
  SourceOrderMain char(38) DEFAULT NULL,
  DocumentParent char(38) DEFAULT NULL,
  DocumentChild char(38) DEFAULT NULL,
  ATDocCodeID varchar(200) DEFAULT NULL,
  ATValidAuditResult char(38) DEFAULT NULL,
  ATResendDocument bit(1) DEFAULT NULL,
  DocumentType char(38) DEFAULT NULL,
  DocumentSerie char(38) DEFAULT NULL,
  PaymentMethod char(38) DEFAULT NULL,
  PaymentCondition char(38) DEFAULT NULL,
  Currency char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancemaster (Oid),
  UNIQUE KEY iDocumentNumber_fin_documentfinancemaster (DocumentNumber),
  KEY iCreatedBy_fin_documentfinancemaster (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancemaster (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancemaster (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancemaster (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancemaster (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancemaster (DeletedWhere),
  KEY iSourceOrderMain_fin_documentfinancemaster (SourceOrderMain),
  KEY iDocumentParent_fin_documentfinancemaster (DocumentParent),
  KEY iDocumentChild_fin_documentfinancemaster (DocumentChild),
  KEY iATValidAuditResult_fin_documentfinancemaster (ATValidAuditResult),
  KEY iDocumentType_fin_documentfinancemaster (DocumentType),
  KEY iDocumentSerie_fin_documentfinancemaster (DocumentSerie),
  KEY iPaymentMethod_fin_documentfinancemaster (PaymentMethod),
  KEY iPaymentCondition_fin_documentfinancemaster (PaymentCondition),
  KEY iCurrency_fin_documentfinancemaster (Currency),
  CONSTRAINT FK_fin_documentfinancemaster_ATValidAuditResult FOREIGN KEY (ATValidAuditResult) REFERENCES sys_systemauditat (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_Currency FOREIGN KEY (Currency) REFERENCES cfg_configurationcurrency (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_DocumentChild FOREIGN KEY (DocumentChild) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_DocumentParent FOREIGN KEY (DocumentParent) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_DocumentSerie FOREIGN KEY (DocumentSerie) REFERENCES fin_documentfinanceseries (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_DocumentType FOREIGN KEY (DocumentType) REFERENCES fin_documentfinancetype (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_PaymentCondition FOREIGN KEY (PaymentCondition) REFERENCES fin_configurationpaymentcondition (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_PaymentMethod FOREIGN KEY (PaymentMethod) REFERENCES fin_configurationpaymentmethod (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_SourceOrderMain FOREIGN KEY (SourceOrderMain) REFERENCES fin_documentordermain (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemaster_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancemasterpayment
--

DROP TABLE IF EXISTS fin_documentfinancemasterpayment;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancemasterpayment (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  CreditAmount double DEFAULT NULL,
  DebitAmount double DEFAULT NULL,
  DocumentFinanceMaster char(38) DEFAULT NULL,
  DocumentFinancePayment char(38) DEFAULT NULL,
  DocumentFinanceMasterCreditNote char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancemasterpayment (Oid),
  KEY iCreatedBy_fin_documentfinancemasterpayment (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancemasterpayment (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancemasterpayment (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancemasterpayment (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancemasterpayment (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancemasterpayment (DeletedWhere),
  KEY iDocumentFinanceMaster_fin_documentfinancemasterpayment (DocumentFinanceMaster),
  KEY iDocumentFinancePayment_fin_documentfinancemasterpayment (DocumentFinancePayment),
  KEY iDocumentFinanceMasterCreditNote_FIN_DocumentFinanceMas_69586DDB (DocumentFinanceMasterCreditNote),
  CONSTRAINT FK_fin_documentfinancemasterpayment_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_DocumentFinanceMast_A8C68AC8 FOREIGN KEY (DocumentFinanceMasterCreditNote) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_DocumentFinanceMaster FOREIGN KEY (DocumentFinanceMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_DocumentFinancePayment FOREIGN KEY (DocumentFinancePayment) REFERENCES fin_documentfinancepayment (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemasterpayment_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancemastertotal
--

DROP TABLE IF EXISTS fin_documentfinancemastertotal;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancemastertotal (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Value double DEFAULT NULL,
  Total double DEFAULT NULL,
  TotalBase double DEFAULT NULL,
  TotalType int(11) DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancemastertotal (Oid),
  KEY iCreatedBy_fin_documentfinancemastertotal (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancemastertotal (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancemastertotal (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancemastertotal (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancemastertotal (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancemastertotal (DeletedWhere),
  KEY iDocumentMaster_fin_documentfinancemastertotal (DocumentMaster),
  CONSTRAINT FK_fin_documentfinancemastertotal_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemastertotal_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancemastertotal_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemastertotal_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancemastertotal_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_fin_documentfinancemastertotal_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancemastertotal_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancepayment
--

DROP TABLE IF EXISTS fin_documentfinancepayment;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancepayment (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  PaymentRefNo varchar(60) DEFAULT NULL,
  TransactionID varchar(70) DEFAULT NULL,
  TransactionDate varchar(19) DEFAULT NULL,
  PaymentType varchar(2) DEFAULT NULL,
  PaymentStatus varchar(1) DEFAULT NULL,
  PaymentStatusDate varchar(50) DEFAULT NULL,
  Reason varchar(50) DEFAULT NULL,
  DocumentStatusSourceID varchar(30) DEFAULT NULL,
  SourcePayment varchar(1) DEFAULT NULL,
  PaymentMechanism varchar(2) DEFAULT NULL,
  PaymentAmount double DEFAULT NULL,
  PaymentDate varchar(19) DEFAULT NULL,
  SourceID varchar(30) DEFAULT NULL,
  SystemEntryDate varchar(50) DEFAULT NULL,
  TaxPayable double DEFAULT NULL,
  NetTotal double DEFAULT NULL,
  GrossTotal double DEFAULT NULL,
  SettlementAmount double DEFAULT NULL,
  CurrencyCode varchar(3) DEFAULT NULL,
  CurrencyAmount double DEFAULT NULL,
  ExchangeRate double DEFAULT NULL,
  WithholdingTaxAmount double DEFAULT NULL,
  EntityOid char(38) DEFAULT NULL,
  EntityInternalCode varchar(30) DEFAULT NULL,
  DocumentDate varchar(19) DEFAULT NULL,
  ExtendedValue text,
  DocumentType char(38) DEFAULT NULL,
  DocumentSerie char(38) DEFAULT NULL,
  PaymentMethod char(38) DEFAULT NULL,
  Currency char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancepayment (Oid),
  KEY iCreatedBy_fin_documentfinancepayment (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancepayment (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancepayment (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancepayment (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancepayment (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancepayment (DeletedWhere),
  KEY iDocumentType_fin_documentfinancepayment (DocumentType),
  KEY iDocumentSerie_fin_documentfinancepayment (DocumentSerie),
  KEY iPaymentMethod_fin_documentfinancepayment (PaymentMethod),
  KEY iCurrency_fin_documentfinancepayment (Currency),
  CONSTRAINT FK_fin_documentfinancepayment_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_Currency FOREIGN KEY (Currency) REFERENCES cfg_configurationcurrency (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_DocumentSerie FOREIGN KEY (DocumentSerie) REFERENCES fin_documentfinanceseries (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_DocumentType FOREIGN KEY (DocumentType) REFERENCES fin_documentfinancetype (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_PaymentMethod FOREIGN KEY (PaymentMethod) REFERENCES fin_configurationpaymentmethod (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancepayment_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinanceseries
--

DROP TABLE IF EXISTS fin_documentfinanceseries;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinanceseries (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  NextDocumentNumber int(11) DEFAULT NULL,
  DocumentNumberRangeBegin int(11) DEFAULT NULL,
  DocumentNumberRangeEnd int(11) DEFAULT NULL,
  Acronym varchar(100) DEFAULT NULL,
  DocumentType char(38) DEFAULT NULL,
  FiscalYear char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinanceseries (Oid),
  UNIQUE KEY iDesignation_fin_documentfinanceseries (Designation),
  UNIQUE KEY iAcronym_fin_documentfinanceseries (Acronym),
  KEY iCreatedBy_fin_documentfinanceseries (CreatedBy),
  KEY iCreatedWhere_fin_documentfinanceseries (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinanceseries (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinanceseries (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinanceseries (DeletedBy),
  KEY iDeletedWhere_fin_documentfinanceseries (DeletedWhere),
  KEY iDocumentType_fin_documentfinanceseries (DocumentType),
  KEY iFiscalYear_fin_documentfinanceseries (FiscalYear),
  CONSTRAINT FK_fin_documentfinanceseries_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_DocumentType FOREIGN KEY (DocumentType) REFERENCES fin_documentfinancetype (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_FiscalYear FOREIGN KEY (FiscalYear) REFERENCES fin_documentfinanceyears (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceseries_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinancetype
--

DROP TABLE IF EXISTS fin_documentfinancetype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinancetype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Acronym varchar(4) DEFAULT NULL,
  AcronymLastSerie int(11) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  ResourceStringReport varchar(100) DEFAULT NULL,
  Payed bit(1) DEFAULT NULL,
  Credit bit(1) DEFAULT NULL,
  CreditDebit int(11) DEFAULT NULL,
  PrintCopies int(11) DEFAULT NULL,
  PrintRequestMotive bit(1) DEFAULT NULL,
  PrintRequestConfirmation bit(1) DEFAULT NULL,
  PrintOpenDrawer bit(1) DEFAULT NULL,
  WayBill bit(1) DEFAULT NULL,
  WsAtDocument bit(1) DEFAULT NULL,
  SaftAuditFile bit(1) DEFAULT NULL,
  SaftDocumentType int(11) DEFAULT NULL,
  StockMode int(11) DEFAULT NULL,
  Printer char(38) DEFAULT NULL,
  Template char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinancetype (Oid),
  UNIQUE KEY iCode_fin_documentfinancetype (Code),
  UNIQUE KEY iDesignation_fin_documentfinancetype (Designation),
  KEY iCreatedBy_fin_documentfinancetype (CreatedBy),
  KEY iCreatedWhere_fin_documentfinancetype (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinancetype (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinancetype (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinancetype (DeletedBy),
  KEY iDeletedWhere_fin_documentfinancetype (DeletedWhere),
  KEY iPrinter_fin_documentfinancetype (Printer),
  KEY iTemplate_fin_documentfinancetype (Template),
  CONSTRAINT FK_fin_documentfinancetype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancetype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancetype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancetype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinancetype_Printer FOREIGN KEY (Printer) REFERENCES sys_configurationprinters (Oid),
  CONSTRAINT FK_fin_documentfinancetype_Template FOREIGN KEY (Template) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_fin_documentfinancetype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinancetype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinanceyears
--

DROP TABLE IF EXISTS fin_documentfinanceyears;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinanceyears (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  FiscalYear int(11) DEFAULT NULL,
  Acronym varchar(100) DEFAULT NULL,
  SeriesForEachTerminal bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinanceyears (Oid),
  UNIQUE KEY iCode_fin_documentfinanceyears (Code),
  UNIQUE KEY iDesignation_fin_documentfinanceyears (Designation),
  UNIQUE KEY iAcronym_fin_documentfinanceyears (Acronym),
  KEY iFiscalYear_fin_documentfinanceyears (FiscalYear),
  KEY iCreatedBy_fin_documentfinanceyears (CreatedBy),
  KEY iCreatedWhere_fin_documentfinanceyears (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinanceyears (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinanceyears (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinanceyears (DeletedBy),
  KEY iDeletedWhere_fin_documentfinanceyears (DeletedWhere),
  CONSTRAINT FK_fin_documentfinanceyears_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceyears_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceyears_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceyears_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceyears_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceyears_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentfinanceyearserieterminal
--

DROP TABLE IF EXISTS fin_documentfinanceyearserieterminal;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentfinanceyearserieterminal (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  FiscalYear char(38) DEFAULT NULL,
  DocumentType char(38) DEFAULT NULL,
  Serie char(38) DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  Printer char(38) DEFAULT NULL,
  Template char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentfinanceyearserieterminal (Oid),
  UNIQUE KEY iDesignation_fin_documentfinanceyearserieterminal (Designation),
  KEY iCreatedBy_fin_documentfinanceyearserieterminal (CreatedBy),
  KEY iCreatedWhere_fin_documentfinanceyearserieterminal (CreatedWhere),
  KEY iUpdatedBy_fin_documentfinanceyearserieterminal (UpdatedBy),
  KEY iUpdatedWhere_fin_documentfinanceyearserieterminal (UpdatedWhere),
  KEY iDeletedBy_fin_documentfinanceyearserieterminal (DeletedBy),
  KEY iDeletedWhere_fin_documentfinanceyearserieterminal (DeletedWhere),
  KEY iFiscalYear_fin_documentfinanceyearserieterminal (FiscalYear),
  KEY iDocumentType_fin_documentfinanceyearserieterminal (DocumentType),
  KEY iSerie_fin_documentfinanceyearserieterminal (Serie),
  KEY iTerminal_fin_documentfinanceyearserieterminal (Terminal),
  KEY iPrinter_fin_documentfinanceyearserieterminal (Printer),
  KEY iTemplate_fin_documentfinanceyearserieterminal (Template),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_DocumentType FOREIGN KEY (DocumentType) REFERENCES fin_documentfinancetype (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_FiscalYear FOREIGN KEY (FiscalYear) REFERENCES fin_documentfinanceyears (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_Printer FOREIGN KEY (Printer) REFERENCES sys_configurationprinters (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_Serie FOREIGN KEY (Serie) REFERENCES fin_documentfinanceseries (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_Template FOREIGN KEY (Template) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentfinanceyearserieterminal_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentorderdetail
--

DROP TABLE IF EXISTS fin_documentorderdetail;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentorderdetail (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code varchar(100) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Quantity double DEFAULT NULL,
  UnitMeasure varchar(35) DEFAULT NULL,
  Price double DEFAULT NULL,
  Discount double DEFAULT NULL,
  Vat double DEFAULT NULL,
  VatExemptionReason char(38) DEFAULT NULL,
  TotalGross double DEFAULT NULL,
  TotalDiscount double DEFAULT NULL,
  TotalTax double DEFAULT NULL,
  TotalFinal double DEFAULT NULL,
  Token1 varchar(255) DEFAULT NULL,
  Token2 varchar(255) DEFAULT NULL,
  OrderTicket char(38) DEFAULT NULL,
  Article char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentorderdetail (Oid),
  KEY iCreatedBy_fin_documentorderdetail (CreatedBy),
  KEY iCreatedWhere_fin_documentorderdetail (CreatedWhere),
  KEY iUpdatedBy_fin_documentorderdetail (UpdatedBy),
  KEY iUpdatedWhere_fin_documentorderdetail (UpdatedWhere),
  KEY iDeletedBy_fin_documentorderdetail (DeletedBy),
  KEY iDeletedWhere_fin_documentorderdetail (DeletedWhere),
  KEY iOrderTicket_fin_documentorderdetail (OrderTicket),
  KEY iArticle_fin_documentorderdetail (Article),
  CONSTRAINT FK_fin_documentorderdetail_Article FOREIGN KEY (Article) REFERENCES fin_article (Oid),
  CONSTRAINT FK_fin_documentorderdetail_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentorderdetail_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentorderdetail_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentorderdetail_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentorderdetail_OrderTicket FOREIGN KEY (OrderTicket) REFERENCES fin_documentorderticket (Oid),
  CONSTRAINT FK_fin_documentorderdetail_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentorderdetail_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentordermain
--

DROP TABLE IF EXISTS fin_documentordermain;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentordermain (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  DateStart datetime DEFAULT NULL,
  OrderStatus int(11) DEFAULT NULL,
  PlaceTable char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentordermain (Oid),
  KEY iCreatedBy_fin_documentordermain (CreatedBy),
  KEY iCreatedWhere_fin_documentordermain (CreatedWhere),
  KEY iUpdatedBy_fin_documentordermain (UpdatedBy),
  KEY iUpdatedWhere_fin_documentordermain (UpdatedWhere),
  KEY iDeletedBy_fin_documentordermain (DeletedBy),
  KEY iDeletedWhere_fin_documentordermain (DeletedWhere),
  KEY iPlaceTable_fin_documentordermain (PlaceTable),
  CONSTRAINT FK_fin_documentordermain_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentordermain_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentordermain_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentordermain_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentordermain_PlaceTable FOREIGN KEY (PlaceTable) REFERENCES pos_configurationplacetable (Oid),
  CONSTRAINT FK_fin_documentordermain_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentordermain_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table fin_documentorderticket
--

DROP TABLE IF EXISTS fin_documentorderticket;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE fin_documentorderticket (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  TicketId int(11) DEFAULT NULL,
  DateStart datetime DEFAULT NULL,
  PriceType int(11) DEFAULT NULL,
  Discount double DEFAULT NULL,
  OrderMain char(38) DEFAULT NULL,
  PlaceTable char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_fin_documentorderticket (Oid),
  KEY iCreatedBy_fin_documentorderticket (CreatedBy),
  KEY iCreatedWhere_fin_documentorderticket (CreatedWhere),
  KEY iUpdatedBy_fin_documentorderticket (UpdatedBy),
  KEY iUpdatedWhere_fin_documentorderticket (UpdatedWhere),
  KEY iDeletedBy_fin_documentorderticket (DeletedBy),
  KEY iDeletedWhere_fin_documentorderticket (DeletedWhere),
  KEY iOrderMain_fin_documentorderticket (OrderMain),
  KEY iPlaceTable_fin_documentorderticket (PlaceTable),
  CONSTRAINT FK_fin_documentorderticket_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentorderticket_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentorderticket_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentorderticket_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_fin_documentorderticket_OrderMain FOREIGN KEY (OrderMain) REFERENCES fin_documentordermain (Oid),
  CONSTRAINT FK_fin_documentorderticket_PlaceTable FOREIGN KEY (PlaceTable) REFERENCES pos_configurationplacetable (Oid),
  CONSTRAINT FK_fin_documentorderticket_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_fin_documentorderticket_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationcashregister
--

DROP TABLE IF EXISTS pos_configurationcashregister;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationcashregister (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Printer varchar(100) DEFAULT NULL,
  Drawer varchar(100) DEFAULT NULL,
  AutomaticDrawer varchar(100) DEFAULT NULL,
  ActiveSales varchar(100) DEFAULT NULL,
  AllowChargeBacks varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationcashregister (Oid),
  UNIQUE KEY iCode_pos_configurationcashregister (Code),
  UNIQUE KEY iDesignation_pos_configurationcashregister (Designation),
  KEY iCreatedBy_pos_configurationcashregister (CreatedBy),
  KEY iCreatedWhere_pos_configurationcashregister (CreatedWhere),
  KEY iUpdatedBy_pos_configurationcashregister (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationcashregister (UpdatedWhere),
  KEY iDeletedBy_pos_configurationcashregister (DeletedBy),
  KEY iDeletedWhere_pos_configurationcashregister (DeletedWhere),
  CONSTRAINT FK_pos_configurationcashregister_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationcashregister_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationcashregister_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationcashregister_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationcashregister_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationcashregister_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationdevice
--

DROP TABLE IF EXISTS pos_configurationdevice;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationdevice (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Type varchar(100) DEFAULT NULL,
  Properties longtext,
  PlaceTerminal char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationdevice (Oid),
  UNIQUE KEY iCode_pos_configurationdevice (Code),
  UNIQUE KEY iDesignation_pos_configurationdevice (Designation),
  KEY iCreatedBy_pos_configurationdevice (CreatedBy),
  KEY iCreatedWhere_pos_configurationdevice (CreatedWhere),
  KEY iUpdatedBy_pos_configurationdevice (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationdevice (UpdatedWhere),
  KEY iDeletedBy_pos_configurationdevice (DeletedBy),
  KEY iDeletedWhere_pos_configurationdevice (DeletedWhere),
  KEY iPlaceTerminal_pos_configurationdevice (PlaceTerminal),
  CONSTRAINT FK_pos_configurationdevice_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationdevice_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationdevice_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationdevice_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationdevice_PlaceTerminal FOREIGN KEY (PlaceTerminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationdevice_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationdevice_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationkeyboard
--

DROP TABLE IF EXISTS pos_configurationkeyboard;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationkeyboard (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Language varchar(100) DEFAULT NULL,
  VirtualKeyboard varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationkeyboard (Oid),
  UNIQUE KEY iCode_pos_configurationkeyboard (Code),
  UNIQUE KEY iDesignation_pos_configurationkeyboard (Designation),
  KEY iCreatedBy_pos_configurationkeyboard (CreatedBy),
  KEY iCreatedWhere_pos_configurationkeyboard (CreatedWhere),
  KEY iUpdatedBy_pos_configurationkeyboard (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationkeyboard (UpdatedWhere),
  KEY iDeletedBy_pos_configurationkeyboard (DeletedBy),
  KEY iDeletedWhere_pos_configurationkeyboard (DeletedWhere),
  CONSTRAINT FK_pos_configurationkeyboard_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationkeyboard_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationkeyboard_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationkeyboard_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationkeyboard_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationkeyboard_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationmaintenance
--

DROP TABLE IF EXISTS pos_configurationmaintenance;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationmaintenance (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Date varchar(100) DEFAULT NULL,
  Time varchar(100) DEFAULT NULL,
  PasswordAccess varchar(100) DEFAULT NULL,
  Remarks varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationmaintenance (Oid),
  UNIQUE KEY iCode_pos_configurationmaintenance (Code),
  UNIQUE KEY iDesignation_pos_configurationmaintenance (Designation),
  KEY iCreatedBy_pos_configurationmaintenance (CreatedBy),
  KEY iCreatedWhere_pos_configurationmaintenance (CreatedWhere),
  KEY iUpdatedBy_pos_configurationmaintenance (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationmaintenance (UpdatedWhere),
  KEY iDeletedBy_pos_configurationmaintenance (DeletedBy),
  KEY iDeletedWhere_pos_configurationmaintenance (DeletedWhere),
  CONSTRAINT FK_pos_configurationmaintenance_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationmaintenance_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationmaintenance_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationmaintenance_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationmaintenance_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationmaintenance_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationplace
--

DROP TABLE IF EXISTS pos_configurationplace;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationplace (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ButtonImage varchar(255) DEFAULT NULL,
  TypeSubtotal varchar(100) DEFAULT NULL,
  AccountType varchar(100) DEFAULT NULL,
  OrderPrintMode int(11) DEFAULT NULL,
  PriceType char(38) DEFAULT NULL,
  MovementType char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationplace (Oid),
  UNIQUE KEY iCode_pos_configurationplace (Code),
  UNIQUE KEY iDesignation_pos_configurationplace (Designation),
  KEY iCreatedBy_pos_configurationplace (CreatedBy),
  KEY iCreatedWhere_pos_configurationplace (CreatedWhere),
  KEY iUpdatedBy_pos_configurationplace (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationplace (UpdatedWhere),
  KEY iDeletedBy_pos_configurationplace (DeletedBy),
  KEY iDeletedWhere_pos_configurationplace (DeletedWhere),
  KEY iPriceType_pos_configurationplace (PriceType),
  KEY iMovementType_pos_configurationplace (MovementType),
  CONSTRAINT FK_pos_configurationplace_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplace_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplace_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplace_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplace_MovementType FOREIGN KEY (MovementType) REFERENCES pos_configurationplacemovementtype (Oid),
  CONSTRAINT FK_pos_configurationplace_PriceType FOREIGN KEY (PriceType) REFERENCES fin_configurationpricetype (Oid),
  CONSTRAINT FK_pos_configurationplace_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplace_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationplacemovementtype
--

DROP TABLE IF EXISTS pos_configurationplacemovementtype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationplacemovementtype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  VatDirectSelling bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationplacemovementtype (Oid),
  UNIQUE KEY iCode_pos_configurationplacemovementtype (Code),
  UNIQUE KEY iDesignation_pos_configurationplacemovementtype (Designation),
  KEY iCreatedBy_pos_configurationplacemovementtype (CreatedBy),
  KEY iCreatedWhere_pos_configurationplacemovementtype (CreatedWhere),
  KEY iUpdatedBy_pos_configurationplacemovementtype (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationplacemovementtype (UpdatedWhere),
  KEY iDeletedBy_pos_configurationplacemovementtype (DeletedBy),
  KEY iDeletedWhere_pos_configurationplacemovementtype (DeletedWhere),
  CONSTRAINT FK_pos_configurationplacemovementtype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplacemovementtype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplacemovementtype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplacemovementtype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplacemovementtype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplacemovementtype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationplacetable
--

DROP TABLE IF EXISTS pos_configurationplacetable;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationplacetable (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ButtonImage varchar(255) DEFAULT NULL,
  Discount double DEFAULT NULL,
  TableStatus int(11) DEFAULT NULL,
  TotalOpen double DEFAULT NULL,
  DateTableOpen datetime DEFAULT NULL,
  DateTableClosed datetime DEFAULT NULL,
  Place char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationplacetable (Oid),
  UNIQUE KEY iCode_pos_configurationplacetable (Code),
  UNIQUE KEY iDesignation_pos_configurationplacetable (Designation),
  KEY iCreatedBy_pos_configurationplacetable (CreatedBy),
  KEY iCreatedWhere_pos_configurationplacetable (CreatedWhere),
  KEY iUpdatedBy_pos_configurationplacetable (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationplacetable (UpdatedWhere),
  KEY iDeletedBy_pos_configurationplacetable (DeletedBy),
  KEY iDeletedWhere_pos_configurationplacetable (DeletedWhere),
  KEY iPlace_pos_configurationplacetable (Place),
  CONSTRAINT FK_pos_configurationplacetable_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplacetable_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplacetable_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplacetable_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplacetable_Place FOREIGN KEY (Place) REFERENCES pos_configurationplace (Oid),
  CONSTRAINT FK_pos_configurationplacetable_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplacetable_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_configurationplaceterminal
--

DROP TABLE IF EXISTS pos_configurationplaceterminal;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_configurationplaceterminal (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  HardwareId varchar(120) DEFAULT NULL,
  InputReaderTimerInterval int(10) unsigned DEFAULT NULL,
  Place char(38) DEFAULT NULL,
  Printer char(38) DEFAULT NULL,  
  ThermalPrinter char(38) DEFAULT NULL,
  BarcodeReader char(38) DEFAULT NULL,
  CardReader char(38) DEFAULT NULL,
  PoleDisplay char(38) DEFAULT NULL,
  WeighingMachine char(38) DEFAULT NULL,
  TemplateTicket char(38) DEFAULT NULL,
  TemplateTablesConsult char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_configurationplaceterminal (Oid),
  UNIQUE KEY iCode_pos_configurationplaceterminal (Code),
  UNIQUE KEY iDesignation_pos_configurationplaceterminal (Designation),
  UNIQUE KEY iHardwareId_pos_configurationplaceterminal (HardwareId),
  KEY iCreatedBy_pos_configurationplaceterminal (CreatedBy),
  KEY iCreatedWhere_pos_configurationplaceterminal (CreatedWhere),
  KEY iUpdatedBy_pos_configurationplaceterminal (UpdatedBy),
  KEY iUpdatedWhere_pos_configurationplaceterminal (UpdatedWhere),
  KEY iDeletedBy_pos_configurationplaceterminal (DeletedBy),
  KEY iDeletedWhere_pos_configurationplaceterminal (DeletedWhere),
  KEY iPlace_pos_configurationplaceterminal (Place),
  KEY iPrinter_pos_configurationplaceterminal (Printer),
  KEY iBarcodeReader_pos_configurationplaceterminal (BarcodeReader),
  KEY iCardReader_pos_configurationplaceterminal (CardReader),
  KEY iPoleDisplay_pos_configurationplaceterminal (PoleDisplay),
  KEY iWeighingMachine_pos_configurationplaceterminal (WeighingMachine),
  KEY iTemplateTicket_pos_configurationplaceterminal (TemplateTicket),
  KEY iTemplateTablesConsult_pos_configurationplaceterminal (TemplateTablesConsult),
  CONSTRAINT FK_pos_configurationplaceterminal_BarcodeReader FOREIGN KEY (BarcodeReader) REFERENCES sys_configurationinputreader (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_CardReader FOREIGN KEY (CardReader) REFERENCES sys_configurationinputreader (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_Place FOREIGN KEY (Place) REFERENCES pos_configurationplace (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_PoleDisplay FOREIGN KEY (PoleDisplay) REFERENCES sys_configurationpoledisplay (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_Printer FOREIGN KEY (Printer) REFERENCES sys_configurationprinters (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_TemplateTablesConsult FOREIGN KEY (TemplateTablesConsult) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_TemplateTicket FOREIGN KEY (TemplateTicket) REFERENCES sys_configurationprinterstemplates (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_configurationplaceterminal_WeighingMachine FOREIGN KEY (WeighingMachine) REFERENCES sys_configurationweighingmachine (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_usercommissiongroup
--

DROP TABLE IF EXISTS pos_usercommissiongroup;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_usercommissiongroup (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Commission double DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_usercommissiongroup (Oid),
  UNIQUE KEY iCode_pos_usercommissiongroup (Code),
  UNIQUE KEY iDesignation_pos_usercommissiongroup (Designation),
  KEY iCreatedBy_pos_usercommissiongroup (CreatedBy),
  KEY iCreatedWhere_pos_usercommissiongroup (CreatedWhere),
  KEY iUpdatedBy_pos_usercommissiongroup (UpdatedBy),
  KEY iUpdatedWhere_pos_usercommissiongroup (UpdatedWhere),
  KEY iDeletedBy_pos_usercommissiongroup (DeletedBy),
  KEY iDeletedWhere_pos_usercommissiongroup (DeletedWhere),
  CONSTRAINT FK_pos_usercommissiongroup_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_usercommissiongroup_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_usercommissiongroup_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_usercommissiongroup_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_usercommissiongroup_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_usercommissiongroup_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_worksessionmovement
--

DROP TABLE IF EXISTS pos_worksessionmovement;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_worksessionmovement (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Date datetime DEFAULT NULL,
  MovementAmount double DEFAULT NULL,
  Description varchar(255) DEFAULT NULL,
  UserDetail char(38) DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  DocumentFinanceMaster char(38) DEFAULT NULL,
  DocumentFinancePayment char(38) DEFAULT NULL,
  DocumentFinanceType char(38) DEFAULT NULL,
  PaymentMethod char(38) DEFAULT NULL,
  WorkSessionPeriod char(38) DEFAULT NULL,
  WorkSessionMovementType char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_worksessionmovement (Oid),
  KEY iCreatedBy_pos_worksessionmovement (CreatedBy),
  KEY iCreatedWhere_pos_worksessionmovement (CreatedWhere),
  KEY iUpdatedBy_pos_worksessionmovement (UpdatedBy),
  KEY iUpdatedWhere_pos_worksessionmovement (UpdatedWhere),
  KEY iDeletedBy_pos_worksessionmovement (DeletedBy),
  KEY iDeletedWhere_pos_worksessionmovement (DeletedWhere),
  KEY iUserDetail_pos_worksessionmovement (UserDetail),
  KEY iTerminal_pos_worksessionmovement (Terminal),
  KEY iDocumentFinanceMaster_pos_worksessionmovement (DocumentFinanceMaster),
  KEY iDocumentFinancePayment_pos_worksessionmovement (DocumentFinancePayment),
  KEY iDocumentFinanceType_pos_worksessionmovement (DocumentFinanceType),
  KEY iPaymentMethod_pos_worksessionmovement (PaymentMethod),
  KEY iWorkSessionPeriod_pos_worksessionmovement (WorkSessionPeriod),
  KEY iWorkSessionMovementType_pos_worksessionmovement (WorkSessionMovementType),
  CONSTRAINT FK_pos_worksessionmovement_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovement_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionmovement_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovement_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionmovement_DocumentFinanceMaster FOREIGN KEY (DocumentFinanceMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_pos_worksessionmovement_DocumentFinancePayment FOREIGN KEY (DocumentFinancePayment) REFERENCES fin_documentfinancepayment (Oid),
  CONSTRAINT FK_pos_worksessionmovement_DocumentFinanceType FOREIGN KEY (DocumentFinanceType) REFERENCES fin_documentfinancetype (Oid),
  CONSTRAINT FK_pos_worksessionmovement_PaymentMethod FOREIGN KEY (PaymentMethod) REFERENCES fin_configurationpaymentmethod (Oid),
  CONSTRAINT FK_pos_worksessionmovement_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionmovement_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovement_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionmovement_UserDetail FOREIGN KEY (UserDetail) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovement_WorkSessionMovementType FOREIGN KEY (WorkSessionMovementType) REFERENCES pos_worksessionmovementtype (Oid),
  CONSTRAINT FK_pos_worksessionmovement_WorkSessionPeriod FOREIGN KEY (WorkSessionPeriod) REFERENCES pos_worksessionperiod (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_worksessionmovementtype
--

DROP TABLE IF EXISTS pos_worksessionmovementtype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_worksessionmovementtype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  ButtonIcon varchar(255) DEFAULT NULL,
  CashDrawerMovement bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_worksessionmovementtype (Oid),
  UNIQUE KEY iCode_pos_worksessionmovementtype (Code),
  UNIQUE KEY iToken_pos_worksessionmovementtype (Token),
  UNIQUE KEY iDesignation_pos_worksessionmovementtype (Designation),
  UNIQUE KEY iResourceString_pos_worksessionmovementtype (ResourceString),
  KEY iCreatedBy_pos_worksessionmovementtype (CreatedBy),
  KEY iCreatedWhere_pos_worksessionmovementtype (CreatedWhere),
  KEY iUpdatedBy_pos_worksessionmovementtype (UpdatedBy),
  KEY iUpdatedWhere_pos_worksessionmovementtype (UpdatedWhere),
  KEY iDeletedBy_pos_worksessionmovementtype (DeletedBy),
  KEY iDeletedWhere_pos_worksessionmovementtype (DeletedWhere),
  CONSTRAINT FK_pos_worksessionmovementtype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovementtype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionmovementtype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovementtype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionmovementtype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionmovementtype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_worksessionperiod
--

DROP TABLE IF EXISTS pos_worksessionperiod;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_worksessionperiod (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  PeriodType int(11) DEFAULT NULL,
  SessionStatus int(11) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  DateStart datetime DEFAULT NULL,
  DateEnd datetime DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  Parent char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_worksessionperiod (Oid),
  UNIQUE KEY iDesignation_pos_worksessionperiod (Designation),
  KEY iCreatedBy_pos_worksessionperiod (CreatedBy),
  KEY iCreatedWhere_pos_worksessionperiod (CreatedWhere),
  KEY iUpdatedBy_pos_worksessionperiod (UpdatedBy),
  KEY iUpdatedWhere_pos_worksessionperiod (UpdatedWhere),
  KEY iDeletedBy_pos_worksessionperiod (DeletedBy),
  KEY iDeletedWhere_pos_worksessionperiod (DeletedWhere),
  KEY iTerminal_pos_worksessionperiod (Terminal),
  KEY iParent_pos_worksessionperiod (Parent),
  CONSTRAINT FK_pos_worksessionperiod_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionperiod_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionperiod_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionperiod_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionperiod_Parent FOREIGN KEY (Parent) REFERENCES pos_worksessionperiod (Oid),
  CONSTRAINT FK_pos_worksessionperiod_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionperiod_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionperiod_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table pos_worksessionperiodtotal
--

DROP TABLE IF EXISTS pos_worksessionperiodtotal;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE pos_worksessionperiodtotal (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  PaymentMethod char(38) DEFAULT NULL,
  Total double DEFAULT NULL,
  Period char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_pos_worksessionperiodtotal (Oid),
  KEY iCreatedBy_pos_worksessionperiodtotal (CreatedBy),
  KEY iCreatedWhere_pos_worksessionperiodtotal (CreatedWhere),
  KEY iUpdatedBy_pos_worksessionperiodtotal (UpdatedBy),
  KEY iUpdatedWhere_pos_worksessionperiodtotal (UpdatedWhere),
  KEY iDeletedBy_pos_worksessionperiodtotal (DeletedBy),
  KEY iDeletedWhere_pos_worksessionperiodtotal (DeletedWhere),
  KEY iPaymentMethod_pos_worksessionperiodtotal (PaymentMethod),
  KEY iPeriod_pos_worksessionperiodtotal (Period),
  CONSTRAINT FK_pos_worksessionperiodtotal_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_PaymentMethod FOREIGN KEY (PaymentMethod) REFERENCES fin_configurationpaymentmethod (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_Period FOREIGN KEY (Period) REFERENCES pos_worksessionperiod (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_pos_worksessionperiodtotal_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table rpt_report
--

DROP TABLE IF EXISTS rpt_report;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE rpt_report (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  FileName varchar(100) DEFAULT NULL,
  ParameterFields varchar(100) DEFAULT NULL,
  AuthorType int(11) DEFAULT NULL,
  ReportType char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_rpt_report (Oid),
  UNIQUE KEY iCode_rpt_report (Code),
  UNIQUE KEY iDesignation_rpt_report (Designation),
  UNIQUE KEY iResourceString_rpt_report (ResourceString),
  UNIQUE KEY iToken_rpt_report (Token),
  KEY iCreatedBy_rpt_report (CreatedBy),
  KEY iCreatedWhere_rpt_report (CreatedWhere),
  KEY iUpdatedBy_rpt_report (UpdatedBy),
  KEY iUpdatedWhere_rpt_report (UpdatedWhere),
  KEY iDeletedBy_rpt_report (DeletedBy),
  KEY iDeletedWhere_rpt_report (DeletedWhere),
  KEY iReportType_rpt_report (ReportType),
  CONSTRAINT FK_rpt_report_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_rpt_report_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_rpt_report_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_rpt_report_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_rpt_report_ReportType FOREIGN KEY (ReportType) REFERENCES rpt_reporttype (Oid),
  CONSTRAINT FK_rpt_report_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_rpt_report_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table rpt_reporttype
--

DROP TABLE IF EXISTS rpt_reporttype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE rpt_reporttype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  MenuIcon varchar(255) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_rpt_reporttype (Oid),
  UNIQUE KEY iDesignation_rpt_reporttype (Designation),
  UNIQUE KEY iResourceString_rpt_reporttype (ResourceString),
  KEY iCreatedBy_rpt_reporttype (CreatedBy),
  KEY iCreatedWhere_rpt_reporttype (CreatedWhere),
  KEY iUpdatedBy_rpt_reporttype (UpdatedBy),
  KEY iUpdatedWhere_rpt_reporttype (UpdatedWhere),
  KEY iDeletedBy_rpt_reporttype (DeletedBy),
  KEY iDeletedWhere_rpt_reporttype (DeletedWhere),
  CONSTRAINT FK_rpt_reporttype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_rpt_reporttype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_rpt_reporttype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_rpt_reporttype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_rpt_reporttype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_rpt_reporttype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_configurationinputreader
--

DROP TABLE IF EXISTS sys_configurationinputreader;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_configurationinputreader (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ReaderSizes varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_configurationinputreader (Oid),
  UNIQUE KEY iCode_sys_configurationinputreader (Code),
  KEY iCreatedBy_sys_configurationinputreader (CreatedBy),
  KEY iCreatedWhere_sys_configurationinputreader (CreatedWhere),
  KEY iUpdatedBy_sys_configurationinputreader (UpdatedBy),
  KEY iUpdatedWhere_sys_configurationinputreader (UpdatedWhere),
  KEY iDeletedBy_sys_configurationinputreader (DeletedBy),
  KEY iDeletedWhere_sys_configurationinputreader (DeletedWhere),
  CONSTRAINT FK_sys_configurationinputreader_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationinputreader_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationinputreader_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationinputreader_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationinputreader_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationinputreader_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_configurationpoledisplay
--

DROP TABLE IF EXISTS sys_configurationpoledisplay;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_configurationpoledisplay (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  VID varchar(100) DEFAULT NULL,
  PID varchar(100) DEFAULT NULL,
  EndPoint varchar(100) DEFAULT NULL,
  CodeTable varchar(100) DEFAULT NULL,
  COM varchar(10) DEFAULT NULL,
  DisplayCharactersPerLine int(10) unsigned DEFAULT NULL,
  GoToStandByInSeconds int(10) unsigned DEFAULT NULL,
  StandByLine1 varchar(100) DEFAULT NULL,
  StandByLine2 varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_configurationpoledisplay (Oid),
  UNIQUE KEY iCode_sys_configurationpoledisplay (Code),
  KEY iCreatedBy_sys_configurationpoledisplay (CreatedBy),
  KEY iCreatedWhere_sys_configurationpoledisplay (CreatedWhere),
  KEY iUpdatedBy_sys_configurationpoledisplay (UpdatedBy),
  KEY iUpdatedWhere_sys_configurationpoledisplay (UpdatedWhere),
  KEY iDeletedBy_sys_configurationpoledisplay (DeletedBy),
  KEY iDeletedWhere_sys_configurationpoledisplay (DeletedWhere),
  CONSTRAINT FK_sys_configurationpoledisplay_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationpoledisplay_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationpoledisplay_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationpoledisplay_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationpoledisplay_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationpoledisplay_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_configurationprinters
--

DROP TABLE IF EXISTS sys_configurationprinters;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_configurationprinters (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  NetworkName varchar(100) DEFAULT NULL,
  ThermalEncoding varchar(100) DEFAULT NULL,
  ThermalPrintLogo bit(1) DEFAULT NULL,
  ThermalImageCompanyLogo varchar(100) DEFAULT NULL,
  ThermalMaxCharsPerLineNormal int(11) DEFAULT NULL,
  ThermalMaxCharsPerLineNormalBold int(11) DEFAULT NULL,
  ThermalMaxCharsPerLineSmall int(11) DEFAULT NULL,
  ThermalCutCommand varchar(100) DEFAULT NULL,
  ThermalOpenDrawerValueM int(11) DEFAULT NULL,
  ThermalOpenDrawerValueT1 int(11) DEFAULT NULL,
  ThermalOpenDrawerValueT2 int(11) DEFAULT NULL,
  ShowInDialog bit(1) DEFAULT NULL,
  PrinterType char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_configurationprinters (Oid),
  UNIQUE KEY iCode_sys_configurationprinters (Code),
  KEY iCreatedBy_sys_configurationprinters (CreatedBy),
  KEY iCreatedWhere_sys_configurationprinters (CreatedWhere),
  KEY iUpdatedBy_sys_configurationprinters (UpdatedBy),
  KEY iUpdatedWhere_sys_configurationprinters (UpdatedWhere),
  KEY iDeletedBy_sys_configurationprinters (DeletedBy),
  KEY iDeletedWhere_sys_configurationprinters (DeletedWhere),
  KEY iPrinterType_sys_configurationprinters (PrinterType),
  CONSTRAINT FK_sys_configurationprinters_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinters_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationprinters_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinters_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationprinters_PrinterType FOREIGN KEY (PrinterType) REFERENCES sys_configurationprinterstype (Oid),
  CONSTRAINT FK_sys_configurationprinters_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinters_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_configurationprinterstemplates
--

DROP TABLE IF EXISTS sys_configurationprinterstemplates;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_configurationprinterstemplates (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  FileTemplate varchar(100) DEFAULT NULL,
  FinancialTemplate bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_configurationprinterstemplates (Oid),
  UNIQUE KEY iDesignation_sys_configurationprinterstemplates (Designation),
  KEY iCreatedBy_sys_configurationprinterstemplates (CreatedBy),
  KEY iCreatedWhere_sys_configurationprinterstemplates (CreatedWhere),
  KEY iUpdatedBy_sys_configurationprinterstemplates (UpdatedBy),
  KEY iUpdatedWhere_sys_configurationprinterstemplates (UpdatedWhere),
  KEY iDeletedBy_sys_configurationprinterstemplates (DeletedBy),
  KEY iDeletedWhere_sys_configurationprinterstemplates (DeletedWhere),
  CONSTRAINT FK_sys_configurationprinterstemplates_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinterstemplates_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationprinterstemplates_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinterstemplates_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationprinterstemplates_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinterstemplates_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_configurationprinterstype
--

DROP TABLE IF EXISTS sys_configurationprinterstype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_configurationprinterstype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  ThermalPrinter bit(1) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_configurationprinterstype (Oid),
  UNIQUE KEY iCode_sys_configurationprinterstype (Code),
  UNIQUE KEY iDesignation_sys_configurationprinterstype (Designation),
  UNIQUE KEY iToken_sys_configurationprinterstype (Token),
  KEY iCreatedBy_sys_configurationprinterstype (CreatedBy),
  KEY iCreatedWhere_sys_configurationprinterstype (CreatedWhere),
  KEY iUpdatedBy_sys_configurationprinterstype (UpdatedBy),
  KEY iUpdatedWhere_sys_configurationprinterstype (UpdatedWhere),
  KEY iDeletedBy_sys_configurationprinterstype (DeletedBy),
  KEY iDeletedWhere_sys_configurationprinterstype (DeletedWhere),
  CONSTRAINT FK_sys_configurationprinterstype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinterstype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationprinterstype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinterstype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationprinterstype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationprinterstype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_configurationweighingmachine
--

DROP TABLE IF EXISTS sys_configurationweighingmachine;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_configurationweighingmachine (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  PortName varchar(4) DEFAULT NULL,
  BaudRate int(10) unsigned DEFAULT NULL,
  Parity varchar(5) DEFAULT NULL,
  StopBits varchar(12) DEFAULT NULL,
  DataBits int(10) unsigned DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_configurationweighingmachine (Oid),
  UNIQUE KEY iCode_sys_configurationweighingmachine (Code),
  KEY iCreatedBy_sys_configurationweighingmachine (CreatedBy),
  KEY iCreatedWhere_sys_configurationweighingmachine (CreatedWhere),
  KEY iUpdatedBy_sys_configurationweighingmachine (UpdatedBy),
  KEY iUpdatedWhere_sys_configurationweighingmachine (UpdatedWhere),
  KEY iDeletedBy_sys_configurationweighingmachine (DeletedBy),
  KEY iDeletedWhere_sys_configurationweighingmachine (DeletedWhere),
  CONSTRAINT FK_sys_configurationweighingmachine_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationweighingmachine_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationweighingmachine_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationweighingmachine_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_configurationweighingmachine_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_configurationweighingmachine_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemaudit
--

DROP TABLE IF EXISTS sys_systemaudit;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemaudit (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Date datetime DEFAULT NULL,
  Description varchar(255) DEFAULT NULL,
  UserDetail char(38) DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  AuditType char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemaudit (Oid),
  KEY iCreatedBy_sys_systemaudit (CreatedBy),
  KEY iCreatedWhere_sys_systemaudit (CreatedWhere),
  KEY iUpdatedBy_sys_systemaudit (UpdatedBy),
  KEY iUpdatedWhere_sys_systemaudit (UpdatedWhere),
  KEY iDeletedBy_sys_systemaudit (DeletedBy),
  KEY iDeletedWhere_sys_systemaudit (DeletedWhere),
  KEY iUserDetail_sys_systemaudit (UserDetail),
  KEY iTerminal_sys_systemaudit (Terminal),
  KEY iAuditType_sys_systemaudit (AuditType),
  CONSTRAINT FK_sys_systemaudit_AuditType FOREIGN KEY (AuditType) REFERENCES sys_systemaudittype (Oid),
  CONSTRAINT FK_sys_systemaudit_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemaudit_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemaudit_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemaudit_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemaudit_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemaudit_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemaudit_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemaudit_UserDetail FOREIGN KEY (UserDetail) REFERENCES sys_userdetail (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemauditat
--

DROP TABLE IF EXISTS sys_systemauditat;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemauditat (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Date datetime DEFAULT NULL,
  Type int(11) DEFAULT NULL,
  PostData longtext,
  ReturnCode int(11) DEFAULT NULL,
  ReturnMessage longtext DEFAULT NULL,
  ReturnRaw longtext,
  DocumentNumber varchar(100) DEFAULT NULL,
  ATDocCodeID varchar(100) DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemauditat (Oid),
  KEY iCreatedBy_sys_systemauditat (CreatedBy),
  KEY iCreatedWhere_sys_systemauditat (CreatedWhere),
  KEY iUpdatedBy_sys_systemauditat (UpdatedBy),
  KEY iUpdatedWhere_sys_systemauditat (UpdatedWhere),
  KEY iDeletedBy_sys_systemauditat (DeletedBy),
  KEY iDeletedWhere_sys_systemauditat (DeletedWhere),
  KEY iDocumentMaster_sys_systemauditat (DocumentMaster),
  CONSTRAINT FK_sys_systemauditat_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemauditat_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemauditat_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemauditat_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemauditat_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_sys_systemauditat_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemauditat_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemaudittype
--

DROP TABLE IF EXISTS sys_systemaudittype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemaudittype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  ResourceString varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemaudittype (Oid),
  UNIQUE KEY iCode_sys_systemaudittype (Code),
  UNIQUE KEY iToken_sys_systemaudittype (Token),
  UNIQUE KEY iDesignation_sys_systemaudittype (Designation),
  UNIQUE KEY iResourceString_sys_systemaudittype (ResourceString),
  KEY iCreatedBy_sys_systemaudittype (CreatedBy),
  KEY iCreatedWhere_sys_systemaudittype (CreatedWhere),
  KEY iUpdatedBy_sys_systemaudittype (UpdatedBy),
  KEY iUpdatedWhere_sys_systemaudittype (UpdatedWhere),
  KEY iDeletedBy_sys_systemaudittype (DeletedBy),
  KEY iDeletedWhere_sys_systemaudittype (DeletedWhere),
  CONSTRAINT FK_sys_systemaudittype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemaudittype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemaudittype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemaudittype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemaudittype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemaudittype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systembackup
--

DROP TABLE IF EXISTS sys_systembackup;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systembackup (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  DataBaseType int(11) DEFAULT NULL,
  Version int(10) unsigned DEFAULT NULL,
  FileName varchar(255) DEFAULT NULL,
  FileNamePacked varchar(255) DEFAULT NULL,
  FilePath varchar(100) DEFAULT NULL,
  FileHash varchar(255) DEFAULT NULL,
  User char(38) DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systembackup (Oid),
  UNIQUE KEY iFileName_sys_systembackup (FileName),
  UNIQUE KEY iFileNamePacked_sys_systembackup (FileNamePacked),
  KEY iCreatedBy_sys_systembackup (CreatedBy),
  KEY iCreatedWhere_sys_systembackup (CreatedWhere),
  KEY iUpdatedBy_sys_systembackup (UpdatedBy),
  KEY iUpdatedWhere_sys_systembackup (UpdatedWhere),
  KEY iDeletedBy_sys_systembackup (DeletedBy),
  KEY iDeletedWhere_sys_systembackup (DeletedWhere),
  KEY iUser_sys_systembackup (User),
  KEY iTerminal_sys_systembackup (Terminal),
  CONSTRAINT FK_sys_systembackup_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systembackup_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systembackup_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systembackup_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systembackup_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systembackup_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systembackup_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systembackup_User FOREIGN KEY (User) REFERENCES sys_userdetail (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemnotification
--

DROP TABLE IF EXISTS sys_systemnotification;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemnotification (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Message longtext,
  Readed bit(1) DEFAULT NULL,
  DateRead datetime DEFAULT NULL,
  UserTarget char(38) DEFAULT NULL,
  TerminalTarget char(38) DEFAULT NULL,
  UserLastRead char(38) DEFAULT NULL,
  TerminalLastRead char(38) DEFAULT NULL,
  NotificationType char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemnotification (Oid),
  KEY iCreatedBy_sys_systemnotification (CreatedBy),
  KEY iCreatedWhere_sys_systemnotification (CreatedWhere),
  KEY iUpdatedBy_sys_systemnotification (UpdatedBy),
  KEY iUpdatedWhere_sys_systemnotification (UpdatedWhere),
  KEY iDeletedBy_sys_systemnotification (DeletedBy),
  KEY iDeletedWhere_sys_systemnotification (DeletedWhere),
  KEY iUserTarget_sys_systemnotification (UserTarget),
  KEY iTerminalTarget_sys_systemnotification (TerminalTarget),
  KEY iUserLastRead_sys_systemnotification (UserLastRead),
  KEY iTerminalLastRead_sys_systemnotification (TerminalLastRead),
  KEY iNotificationType_sys_systemnotification (NotificationType),
  CONSTRAINT FK_sys_systemnotification_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotification_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotification_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotification_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotification_NotificationType FOREIGN KEY (NotificationType) REFERENCES sys_systemnotificationtype (Oid),
  CONSTRAINT FK_sys_systemnotification_TerminalLastRead FOREIGN KEY (TerminalLastRead) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotification_TerminalTarget FOREIGN KEY (TerminalTarget) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotification_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotification_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotification_UserLastRead FOREIGN KEY (UserLastRead) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotification_UserTarget FOREIGN KEY (UserTarget) REFERENCES sys_userdetail (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemnotificationdocumentmaster
--

DROP TABLE IF EXISTS sys_systemnotificationdocumentmaster;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemnotificationdocumentmaster (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Notification char(38) DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemnotificationdocumentmaster (Oid),
  KEY iCreatedBy_sys_systemnotificationdocumentmaster (CreatedBy),
  KEY iCreatedWhere_sys_systemnotificationdocumentmaster (CreatedWhere),
  KEY iUpdatedBy_sys_systemnotificationdocumentmaster (UpdatedBy),
  KEY iUpdatedWhere_sys_systemnotificationdocumentmaster (UpdatedWhere),
  KEY iDeletedBy_sys_systemnotificationdocumentmaster (DeletedBy),
  KEY iDeletedWhere_sys_systemnotificationdocumentmaster (DeletedWhere),
  KEY iNotification_sys_systemnotificationdocumentmaster (Notification),
  KEY iDocumentMaster_sys_systemnotificationdocumentmaster (DocumentMaster),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_Notification FOREIGN KEY (Notification) REFERENCES sys_systemnotification (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotificationdocumentmaster_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemnotificationtype
--

DROP TABLE IF EXISTS sys_systemnotificationtype;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemnotificationtype (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  Message varchar(512) DEFAULT NULL,
  WarnDaysBefore int(11) DEFAULT NULL,
  UserTarget char(38) DEFAULT NULL,
  TerminalTarget char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemnotificationtype (Oid),
  UNIQUE KEY iCode_sys_systemnotificationtype (Code),
  UNIQUE KEY iDesignation_sys_systemnotificationtype (Designation),
  UNIQUE KEY iMessage_sys_systemnotificationtype (Message),
  KEY iCreatedBy_sys_systemnotificationtype (CreatedBy),
  KEY iCreatedWhere_sys_systemnotificationtype (CreatedWhere),
  KEY iUpdatedBy_sys_systemnotificationtype (UpdatedBy),
  KEY iUpdatedWhere_sys_systemnotificationtype (UpdatedWhere),
  KEY iDeletedBy_sys_systemnotificationtype (DeletedBy),
  KEY iDeletedWhere_sys_systemnotificationtype (DeletedWhere),
  KEY iUserTarget_sys_systemnotificationtype (UserTarget),
  KEY iTerminalTarget_sys_systemnotificationtype (TerminalTarget),
  CONSTRAINT FK_sys_systemnotificationtype_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_TerminalTarget FOREIGN KEY (TerminalTarget) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemnotificationtype_UserTarget FOREIGN KEY (UserTarget) REFERENCES sys_userdetail (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_systemprint
--

DROP TABLE IF EXISTS sys_systemprint;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_systemprint (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Date datetime DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  CopyNames varchar(50) DEFAULT NULL,
  PrintCopies int(11) DEFAULT NULL,
  PrintMotive varchar(255) DEFAULT NULL,
  SecondPrint bit(1) DEFAULT NULL,
  UserDetail char(38) DEFAULT NULL,
  Terminal char(38) DEFAULT NULL,
  DocumentMaster char(38) DEFAULT NULL,
  DocumentPayment char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_systemprint (Oid),
  KEY iCreatedBy_sys_systemprint (CreatedBy),
  KEY iCreatedWhere_sys_systemprint (CreatedWhere),
  KEY iUpdatedBy_sys_systemprint (UpdatedBy),
  KEY iUpdatedWhere_sys_systemprint (UpdatedWhere),
  KEY iDeletedBy_sys_systemprint (DeletedBy),
  KEY iDeletedWhere_sys_systemprint (DeletedWhere),
  KEY iUserDetail_sys_systemprint (UserDetail),
  KEY iTerminal_sys_systemprint (Terminal),
  KEY iDocumentMaster_sys_systemprint (DocumentMaster),
  KEY iDocumentPayment_sys_systemprint (DocumentPayment),
  CONSTRAINT FK_sys_systemprint_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemprint_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemprint_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemprint_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemprint_DocumentMaster FOREIGN KEY (DocumentMaster) REFERENCES fin_documentfinancemaster (Oid),
  CONSTRAINT FK_sys_systemprint_DocumentPayment FOREIGN KEY (DocumentPayment) REFERENCES fin_documentfinancepayment (Oid),
  CONSTRAINT FK_sys_systemprint_Terminal FOREIGN KEY (Terminal) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemprint_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_systemprint_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_systemprint_UserDetail FOREIGN KEY (UserDetail) REFERENCES sys_userdetail (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_userdetail
--

DROP TABLE IF EXISTS sys_userdetail;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_userdetail (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  CodeInternal varchar(30) DEFAULT NULL,
  Name text,
  Residence text,
  Locality varchar(255) DEFAULT NULL,
  ZipCode varchar(100) DEFAULT NULL,
  City varchar(255) DEFAULT NULL,
  DateOfContract varchar(100) DEFAULT NULL,
  Phone varchar(255) DEFAULT NULL,
  MobilePhone varchar(255) DEFAULT NULL,
  Email varchar(255) DEFAULT NULL,
  FiscalNumber varchar(100) DEFAULT NULL,
  Language varchar(100) DEFAULT NULL,
  AssignedSeating varchar(100) DEFAULT NULL,
  AccessPin varchar(255) DEFAULT NULL,
  AccessCardNumber varchar(100) DEFAULT NULL,
  Login varchar(100) DEFAULT NULL,
  Password varchar(255) DEFAULT NULL,
  PasswordReset bit(1) DEFAULT NULL,
  PasswordResetDate datetime DEFAULT NULL,
  BaseConsumption varchar(100) DEFAULT NULL,
  BaseOffers varchar(100) DEFAULT NULL,
  PVPOffers varchar(100) DEFAULT NULL,
  Remarks varchar(100) DEFAULT NULL,
  ButtonImage varchar(255) DEFAULT NULL,
  Profile char(38) DEFAULT NULL,
  CommissionGroup char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_userdetail (Oid),
  UNIQUE KEY iCode_sys_userdetail (Code),
  UNIQUE KEY iCodeInternal_sys_userdetail (CodeInternal),
  UNIQUE KEY iAccessPin_sys_userdetail (AccessPin),
  KEY iCreatedBy_sys_userdetail (CreatedBy),
  KEY iCreatedWhere_sys_userdetail (CreatedWhere),
  KEY iUpdatedBy_sys_userdetail (UpdatedBy),
  KEY iUpdatedWhere_sys_userdetail (UpdatedWhere),
  KEY iDeletedBy_sys_userdetail (DeletedBy),
  KEY iDeletedWhere_sys_userdetail (DeletedWhere),
  KEY iProfile_sys_userdetail (Profile),
  KEY iCommissionGroup_sys_userdetail (CommissionGroup),
  CONSTRAINT FK_sys_userdetail_CommissionGroup FOREIGN KEY (CommissionGroup) REFERENCES pos_usercommissiongroup (Oid),
  CONSTRAINT FK_sys_userdetail_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userdetail_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userdetail_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userdetail_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userdetail_Profile FOREIGN KEY (Profile) REFERENCES sys_userprofile (Oid),
  CONSTRAINT FK_sys_userdetail_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userdetail_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_userpermissiongroup
--

DROP TABLE IF EXISTS sys_userpermissiongroup;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_userpermissiongroup (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_userpermissiongroup (Oid),
  UNIQUE KEY iCode_sys_userpermissiongroup (Code),
  UNIQUE KEY iDesignation_sys_userpermissiongroup (Designation),
  KEY iCreatedBy_sys_userpermissiongroup (CreatedBy),
  KEY iCreatedWhere_sys_userpermissiongroup (CreatedWhere),
  KEY iUpdatedBy_sys_userpermissiongroup (UpdatedBy),
  KEY iUpdatedWhere_sys_userpermissiongroup (UpdatedWhere),
  KEY iDeletedBy_sys_userpermissiongroup (DeletedBy),
  KEY iDeletedWhere_sys_userpermissiongroup (DeletedWhere),
  CONSTRAINT FK_sys_userpermissiongroup_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissiongroup_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissiongroup_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissiongroup_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissiongroup_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissiongroup_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_userpermissionitem
--

DROP TABLE IF EXISTS sys_userpermissionitem;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_userpermissionitem (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Token varchar(100) DEFAULT NULL,
  Designation varchar(200) DEFAULT NULL,
  PermissionGroup char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_userpermissionitem (Oid),
  UNIQUE KEY iCode_sys_userpermissionitem (Code),
  UNIQUE KEY iToken_sys_userpermissionitem (Token),
  UNIQUE KEY iDesignation_sys_userpermissionitem (Designation),
  KEY iCreatedBy_sys_userpermissionitem (CreatedBy),
  KEY iCreatedWhere_sys_userpermissionitem (CreatedWhere),
  KEY iUpdatedBy_sys_userpermissionitem (UpdatedBy),
  KEY iUpdatedWhere_sys_userpermissionitem (UpdatedWhere),
  KEY iDeletedBy_sys_userpermissionitem (DeletedBy),
  KEY iDeletedWhere_sys_userpermissionitem (DeletedWhere),
  KEY iPermissionGroup_sys_userpermissionitem (PermissionGroup),
  CONSTRAINT FK_sys_userpermissionitem_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissionitem_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissionitem_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissionitem_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissionitem_PermissionGroup FOREIGN KEY (PermissionGroup) REFERENCES sys_userpermissiongroup (Oid),
  CONSTRAINT FK_sys_userpermissionitem_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissionitem_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_userpermissionprofile
--

DROP TABLE IF EXISTS sys_userpermissionprofile;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_userpermissionprofile (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Granted bit(1) DEFAULT NULL,
  UserProfile char(38) DEFAULT NULL,
  PermissionItem char(38) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_userpermissionprofile (Oid),
  KEY iCreatedBy_sys_userpermissionprofile (CreatedBy),
  KEY iCreatedWhere_sys_userpermissionprofile (CreatedWhere),
  KEY iUpdatedBy_sys_userpermissionprofile (UpdatedBy),
  KEY iUpdatedWhere_sys_userpermissionprofile (UpdatedWhere),
  KEY iDeletedBy_sys_userpermissionprofile (DeletedBy),
  KEY iDeletedWhere_sys_userpermissionprofile (DeletedWhere),
  KEY iUserProfile_sys_userpermissionprofile (UserProfile),
  KEY iPermissionItem_sys_userpermissionprofile (PermissionItem),
  CONSTRAINT FK_sys_userpermissionprofile_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_PermissionItem FOREIGN KEY (PermissionItem) REFERENCES sys_userpermissionitem (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userpermissionprofile_UserProfile FOREIGN KEY (UserProfile) REFERENCES sys_userprofile (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table sys_userprofile
--

DROP TABLE IF EXISTS sys_userprofile;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE sys_userprofile (
  Oid char(38) NOT NULL,
  Disabled bit(1) DEFAULT NULL,
  Notes longtext,
  CreatedAt datetime DEFAULT NULL,
  CreatedBy char(38) DEFAULT NULL,
  CreatedWhere char(38) DEFAULT NULL,
  UpdatedAt datetime DEFAULT NULL,
  UpdatedBy char(38) DEFAULT NULL,
  UpdatedWhere char(38) DEFAULT NULL,
  DeletedAt datetime DEFAULT NULL,
  DeletedBy char(38) DEFAULT NULL,
  DeletedWhere char(38) DEFAULT NULL,
  Ord int(10) unsigned DEFAULT NULL,
  Code int(10) unsigned DEFAULT NULL,
  Designation varchar(100) DEFAULT NULL,
  AccessPassword varchar(50) DEFAULT NULL,
  OptimisticLockField int(11) DEFAULT NULL,
  PRIMARY KEY (Oid),
  UNIQUE KEY iOid_sys_userprofile (Oid),
  UNIQUE KEY iCode_sys_userprofile (Code),
  UNIQUE KEY iDesignation_sys_userprofile (Designation),
  KEY iCreatedBy_sys_userprofile (CreatedBy),
  KEY iCreatedWhere_sys_userprofile (CreatedWhere),
  KEY iUpdatedBy_sys_userprofile (UpdatedBy),
  KEY iUpdatedWhere_sys_userprofile (UpdatedWhere),
  KEY iDeletedBy_sys_userprofile (DeletedBy),
  KEY iDeletedWhere_sys_userprofile (DeletedWhere),
  CONSTRAINT FK_sys_userprofile_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userprofile_CreatedWhere FOREIGN KEY (CreatedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userprofile_DeletedBy FOREIGN KEY (DeletedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userprofile_DeletedWhere FOREIGN KEY (DeletedWhere) REFERENCES pos_configurationplaceterminal (Oid),
  CONSTRAINT FK_sys_userprofile_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES sys_userdetail (Oid),
  CONSTRAINT FK_sys_userprofile_UpdatedWhere FOREIGN KEY (UpdatedWhere) REFERENCES pos_configurationplaceterminal (Oid)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

CREATE TABLE sys_databaseversion (Version varchar(10) DEFAULT NULL) ENGINE=InnoDB DEFAULT CHARSET=utf8;