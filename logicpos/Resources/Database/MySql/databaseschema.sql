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
-- Table structure for table `cfg_configurationcountry`
--

DROP TABLE IF EXISTS `cfg_configurationcountry`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cfg_configurationcountry` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Code2` varchar(5) DEFAULT NULL,
  `Code3` varchar(6) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Capital` varchar(100) DEFAULT NULL,
  `TLD` varchar(10) DEFAULT NULL,
  `Currency` varchar(20) DEFAULT NULL,
  `CurrencyCode` varchar(3) DEFAULT NULL,
  `RegExFiscalNumber` varchar(255) DEFAULT NULL,
  `RegExZipCode` varchar(255) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_CFG_ConfigurationCountry` (`Oid`),
  UNIQUE KEY `iCode_CFG_ConfigurationCountry` (`Code`),
  UNIQUE KEY `iCode2_CFG_ConfigurationCountry` (`Code2`),
  UNIQUE KEY `iCode3_CFG_ConfigurationCountry` (`Code3`),
  UNIQUE KEY `iDesignation_CFG_ConfigurationCountry` (`Designation`),
  KEY `iCreatedBy_CFG_ConfigurationCountry` (`CreatedBy`),
  KEY `iCreatedWhere_CFG_ConfigurationCountry` (`CreatedWhere`),
  KEY `iUpdatedBy_CFG_ConfigurationCountry` (`UpdatedBy`),
  KEY `iUpdatedWhere_CFG_ConfigurationCountry` (`UpdatedWhere`),
  KEY `iDeletedBy_CFG_ConfigurationCountry` (`DeletedBy`),
  KEY `iDeletedWhere_CFG_ConfigurationCountry` (`DeletedWhere`),
  CONSTRAINT `FK_CFG_ConfigurationCountry_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCountry_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCountry_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCountry_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCountry_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCountry_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cfg_configurationcurrency`
--

DROP TABLE IF EXISTS `cfg_configurationcurrency`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cfg_configurationcurrency` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Acronym` varchar(100) DEFAULT NULL,
  `Symbol` varchar(10) DEFAULT NULL,
  `Entity` text,
  `ExchangeRate` double DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_CFG_ConfigurationCurrency` (`Oid`),
  UNIQUE KEY `iCode_CFG_ConfigurationCurrency` (`Code`),
  UNIQUE KEY `iDesignation_CFG_ConfigurationCurrency` (`Designation`),
  KEY `iCreatedBy_CFG_ConfigurationCurrency` (`CreatedBy`),
  KEY `iCreatedWhere_CFG_ConfigurationCurrency` (`CreatedWhere`),
  KEY `iUpdatedBy_CFG_ConfigurationCurrency` (`UpdatedBy`),
  KEY `iUpdatedWhere_CFG_ConfigurationCurrency` (`UpdatedWhere`),
  KEY `iDeletedBy_CFG_ConfigurationCurrency` (`DeletedBy`),
  KEY `iDeletedWhere_CFG_ConfigurationCurrency` (`DeletedWhere`),
  CONSTRAINT `FK_CFG_ConfigurationCurrency_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCurrency_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCurrency_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCurrency_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCurrency_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationCurrency_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cfg_configurationholidays`
--

DROP TABLE IF EXISTS `cfg_configurationholidays`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cfg_configurationholidays` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Year` int(11) DEFAULT NULL,
  `Month` int(11) DEFAULT NULL,
  `Day` int(11) DEFAULT NULL,
  `Fixed` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_CFG_ConfigurationHolidays` (`Oid`),
  UNIQUE KEY `iCode_CFG_ConfigurationHolidays` (`Code`),
  UNIQUE KEY `iDesignation_CFG_ConfigurationHolidays` (`Designation`),
  KEY `iCreatedBy_CFG_ConfigurationHolidays` (`CreatedBy`),
  KEY `iCreatedWhere_CFG_ConfigurationHolidays` (`CreatedWhere`),
  KEY `iUpdatedBy_CFG_ConfigurationHolidays` (`UpdatedBy`),
  KEY `iUpdatedWhere_CFG_ConfigurationHolidays` (`UpdatedWhere`),
  KEY `iDeletedBy_CFG_ConfigurationHolidays` (`DeletedBy`),
  KEY `iDeletedWhere_CFG_ConfigurationHolidays` (`DeletedWhere`),
  CONSTRAINT `FK_CFG_ConfigurationHolidays_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationHolidays_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationHolidays_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationHolidays_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationHolidays_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationHolidays_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cfg_configurationpreferenceparameter`
--

DROP TABLE IF EXISTS `cfg_configurationpreferenceparameter`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cfg_configurationpreferenceparameter` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `ValueTip` varchar(100) DEFAULT NULL,
  `Required` bit(1) DEFAULT NULL,
  `RegEx` varchar(255) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `ResourceStringInfo` varchar(255) DEFAULT NULL,
  `FormType` int(11) DEFAULT NULL,
  `FormPageNo` int(11) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_CFG_ConfigurationPreferenceParameter` (`Oid`),
  UNIQUE KEY `iCode_CFG_ConfigurationPreferenceParameter` (`Code`),
  UNIQUE KEY `iToken_CFG_ConfigurationPreferenceParameter` (`Token`),
  UNIQUE KEY `iResourceString_CFG_ConfigurationPreferenceParameter` (`ResourceString`),
  KEY `iCreatedBy_CFG_ConfigurationPreferenceParameter` (`CreatedBy`),
  KEY `iCreatedWhere_CFG_ConfigurationPreferenceParameter` (`CreatedWhere`),
  KEY `iUpdatedBy_CFG_ConfigurationPreferenceParameter` (`UpdatedBy`),
  KEY `iUpdatedWhere_CFG_ConfigurationPreferenceParameter` (`UpdatedWhere`),
  KEY `iDeletedBy_CFG_ConfigurationPreferenceParameter` (`DeletedBy`),
  KEY `iDeletedWhere_CFG_ConfigurationPreferenceParameter` (`DeletedWhere`),
  CONSTRAINT `FK_CFG_ConfigurationPreferenceParameter_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationPreferenceParameter_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationPreferenceParameter_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationPreferenceParameter_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationPreferenceParameter_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationPreferenceParameter_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cfg_configurationunitmeasure`
--

DROP TABLE IF EXISTS `cfg_configurationunitmeasure`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cfg_configurationunitmeasure` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Acronym` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_CFG_ConfigurationUnitMeasure` (`Oid`),
  UNIQUE KEY `iCode_CFG_ConfigurationUnitMeasure` (`Code`),
  UNIQUE KEY `iDesignation_CFG_ConfigurationUnitMeasure` (`Designation`),
  UNIQUE KEY `iAcronym_CFG_ConfigurationUnitMeasure` (`Acronym`),
  KEY `iCreatedBy_CFG_ConfigurationUnitMeasure` (`CreatedBy`),
  KEY `iCreatedWhere_CFG_ConfigurationUnitMeasure` (`CreatedWhere`),
  KEY `iUpdatedBy_CFG_ConfigurationUnitMeasure` (`UpdatedBy`),
  KEY `iUpdatedWhere_CFG_ConfigurationUnitMeasure` (`UpdatedWhere`),
  KEY `iDeletedBy_CFG_ConfigurationUnitMeasure` (`DeletedBy`),
  KEY `iDeletedWhere_CFG_ConfigurationUnitMeasure` (`DeletedWhere`),
  CONSTRAINT `FK_CFG_ConfigurationUnitMeasure_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitMeasure_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitMeasure_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitMeasure_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitMeasure_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitMeasure_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cfg_configurationunitsize`
--

DROP TABLE IF EXISTS `cfg_configurationunitsize`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cfg_configurationunitsize` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_CFG_ConfigurationUnitSize` (`Oid`),
  UNIQUE KEY `iCode_CFG_ConfigurationUnitSize` (`Code`),
  UNIQUE KEY `iDesignation_CFG_ConfigurationUnitSize` (`Designation`),
  KEY `iCreatedBy_CFG_ConfigurationUnitSize` (`CreatedBy`),
  KEY `iCreatedWhere_CFG_ConfigurationUnitSize` (`CreatedWhere`),
  KEY `iUpdatedBy_CFG_ConfigurationUnitSize` (`UpdatedBy`),
  KEY `iUpdatedWhere_CFG_ConfigurationUnitSize` (`UpdatedWhere`),
  KEY `iDeletedBy_CFG_ConfigurationUnitSize` (`DeletedBy`),
  KEY `iDeletedWhere_CFG_ConfigurationUnitSize` (`DeletedWhere`),
  CONSTRAINT `FK_CFG_ConfigurationUnitSize_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitSize_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitSize_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitSize_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitSize_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_CFG_ConfigurationUnitSize_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `erp_customer`
--

DROP TABLE IF EXISTS `erp_customer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `erp_customer` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `CodeInternal` varchar(30) DEFAULT NULL,
`Name` varchar(512) DEFAULT NULL,
`Address` varchar(512) DEFAULT NULL,
`Locality` varchar(255) DEFAULT NULL,
  `ZipCode` varchar(100) DEFAULT NULL,
`City` varchar(255) DEFAULT NULL,
  `DateOfBirth` varchar(100) DEFAULT NULL,
`Phone` varchar(255) DEFAULT NULL,
`Fax` varchar(255) DEFAULT NULL,
`MobilePhone` varchar(255) DEFAULT NULL,
`Email` varchar(255) DEFAULT NULL,
  `WebSite` varchar(255) DEFAULT NULL,
  `FiscalNumber` varchar(100) DEFAULT NULL,
  `CardNumber` varchar(100) DEFAULT NULL,
  `DiscountType` varchar(100) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `CardCredit` double DEFAULT NULL,
  `TotalDebt` double DEFAULT NULL,
  `TotalCredit` double DEFAULT NULL,
  `CurrentBalance` double DEFAULT NULL,
  `CreditLine` varchar(100) DEFAULT NULL,
  `Remarks` varchar(100) DEFAULT NULL,
  `Supplier` bit(1) DEFAULT NULL,
  `Hidden` bit(1) DEFAULT NULL,
  `CustomerType` char(38) DEFAULT NULL,
  `DiscountGroup` char(38) DEFAULT NULL,
  `PriceType` char(38) DEFAULT NULL,
  `Country` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_ERP_Customer` (`Oid`),
  UNIQUE KEY `iCodeInternal_ERP_Customer` (`CodeInternal`),
  UNIQUE KEY `iCardNumber_ERP_Customer` (`CardNumber`),
  KEY `iCreatedBy_ERP_Customer` (`CreatedBy`),
  KEY `iCreatedWhere_ERP_Customer` (`CreatedWhere`),
  KEY `iUpdatedBy_ERP_Customer` (`UpdatedBy`),
  KEY `iUpdatedWhere_ERP_Customer` (`UpdatedWhere`),
  KEY `iDeletedBy_ERP_Customer` (`DeletedBy`),
  KEY `iDeletedWhere_ERP_Customer` (`DeletedWhere`),
  KEY `iCustomerType_ERP_Customer` (`CustomerType`),
  KEY `iDiscountGroup_ERP_Customer` (`DiscountGroup`),
  KEY `iPriceType_ERP_Customer` (`PriceType`),
  KEY `iCountry_ERP_Customer` (`Country`),
  CONSTRAINT `FK_ERP_Customer_Country` FOREIGN KEY (`Country`) REFERENCES `cfg_configurationcountry` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_CustomerType` FOREIGN KEY (`CustomerType`) REFERENCES `erp_customertype` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_DiscountGroup` FOREIGN KEY (`DiscountGroup`) REFERENCES `erp_customerdiscountgroup` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_PriceType` FOREIGN KEY (`PriceType`) REFERENCES `fin_configurationpricetype` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_Customer_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `erp_customerdiscountgroup`
--

DROP TABLE IF EXISTS `erp_customerdiscountgroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `erp_customerdiscountgroup` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_ERP_CustomerDiscountGroup` (`Oid`),
  UNIQUE KEY `iCode_ERP_CustomerDiscountGroup` (`Code`),
  UNIQUE KEY `iDesignation_ERP_CustomerDiscountGroup` (`Designation`),
  KEY `iCreatedBy_ERP_CustomerDiscountGroup` (`CreatedBy`),
  KEY `iCreatedWhere_ERP_CustomerDiscountGroup` (`CreatedWhere`),
  KEY `iUpdatedBy_ERP_CustomerDiscountGroup` (`UpdatedBy`),
  KEY `iUpdatedWhere_ERP_CustomerDiscountGroup` (`UpdatedWhere`),
  KEY `iDeletedBy_ERP_CustomerDiscountGroup` (`DeletedBy`),
  KEY `iDeletedWhere_ERP_CustomerDiscountGroup` (`DeletedWhere`),
  CONSTRAINT `FK_ERP_CustomerDiscountGroup_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerDiscountGroup_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerDiscountGroup_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerDiscountGroup_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerDiscountGroup_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerDiscountGroup_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `erp_customertype`
--

DROP TABLE IF EXISTS `erp_customertype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `erp_customertype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_ERP_CustomerType` (`Oid`),
  UNIQUE KEY `iCode_ERP_CustomerType` (`Code`),
  UNIQUE KEY `iDesignation_ERP_CustomerType` (`Designation`),
  KEY `iCreatedBy_ERP_CustomerType` (`CreatedBy`),
  KEY `iCreatedWhere_ERP_CustomerType` (`CreatedWhere`),
  KEY `iUpdatedBy_ERP_CustomerType` (`UpdatedBy`),
  KEY `iUpdatedWhere_ERP_CustomerType` (`UpdatedWhere`),
  KEY `iDeletedBy_ERP_CustomerType` (`DeletedBy`),
  KEY `iDeletedWhere_ERP_CustomerType` (`DeletedWhere`),
  CONSTRAINT `FK_ERP_CustomerType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_ERP_CustomerType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_article`
--

DROP TABLE IF EXISTS `fin_article`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_article` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` varchar(25) DEFAULT NULL,
  `CodeDealer` varchar(25) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ButtonLabel` varchar(35) DEFAULT NULL,
  `ButtonLabelHide` bit(1) DEFAULT NULL,
  `ButtonImage` varchar(255) DEFAULT NULL,
  `ButtonIcon` varchar(255) DEFAULT NULL,
  `Price1` double DEFAULT NULL,
  `Price2` double DEFAULT NULL,
  `Price3` double DEFAULT NULL,
  `Price4` double DEFAULT NULL,
  `Price5` double DEFAULT NULL,
  `Price1Promotion` double DEFAULT NULL,
  `Price2Promotion` double DEFAULT NULL,
  `Price3Promotion` double DEFAULT NULL,
  `Price4Promotion` double DEFAULT NULL,
  `Price5Promotion` double DEFAULT NULL,
  `Price1UsePromotionPrice` bit(1) DEFAULT NULL,
  `Price2UsePromotionPrice` bit(1) DEFAULT NULL,
  `Price3UsePromotionPrice` bit(1) DEFAULT NULL,
  `Price4UsePromotionPrice` bit(1) DEFAULT NULL,
  `Price5UsePromotionPrice` bit(1) DEFAULT NULL,
  `PriceWithVat` bit(1) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `DefaultQuantity` double DEFAULT NULL,
  `Accounting` double DEFAULT NULL,
  `Tare` double DEFAULT NULL,
  `Weight` double DEFAULT NULL,
  `BarCode` varchar(100) DEFAULT NULL,
  `PVPVariable` bit(1) DEFAULT NULL,
  `Favorite` bit(1) DEFAULT NULL,
  `UseWeighingBalance` bit(1) DEFAULT NULL,
  `Token1` varchar(255) DEFAULT NULL,
  `Token2` varchar(255) DEFAULT NULL,
  `Family` char(38) DEFAULT NULL,
  `SubFamily` char(38) DEFAULT NULL,
  `Type` char(38) DEFAULT NULL,
  `Class` char(38) DEFAULT NULL,
  `UnitMeasure` char(38) DEFAULT NULL,
  `UnitSize` char(38) DEFAULT NULL,
  `CommissionGroup` char(38) DEFAULT NULL,
  `DiscountGroup` char(38) DEFAULT NULL,
  `VatOnTable` char(38) DEFAULT NULL,
  `VatDirectSelling` char(38) DEFAULT NULL,
  `VatExemptionReason` char(38) DEFAULT NULL,
  `Printer` char(38) DEFAULT NULL,
  `Template` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_Article` (`Oid`),
  UNIQUE KEY `iCode_FIN_Article` (`Code`),
  UNIQUE KEY `iDesignation_FIN_Article` (`Designation`),
  UNIQUE KEY `iBarCode_FIN_Article` (`BarCode`),
  KEY `iCreatedBy_FIN_Article` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_Article` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_Article` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_Article` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_Article` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_Article` (`DeletedWhere`),
  KEY `iFamily_FIN_Article` (`Family`),
  KEY `iSubFamily_FIN_Article` (`SubFamily`),
  KEY `iType_FIN_Article` (`Type`),
  KEY `iClass_FIN_Article` (`Class`),
  KEY `iUnitMeasure_FIN_Article` (`UnitMeasure`),
  KEY `iUnitSize_FIN_Article` (`UnitSize`),
  KEY `iCommissionGroup_FIN_Article` (`CommissionGroup`),
  KEY `iDiscountGroup_FIN_Article` (`DiscountGroup`),
  KEY `iVatOnTable_FIN_Article` (`VatOnTable`),
  KEY `iVatDirectSelling_FIN_Article` (`VatDirectSelling`),
  KEY `iVatExemptionReason_FIN_Article` (`VatExemptionReason`),
  KEY `iPrinter_FIN_Article` (`Printer`),
  KEY `iTemplate_FIN_Article` (`Template`),
  CONSTRAINT `FK_FIN_Article_Class` FOREIGN KEY (`Class`) REFERENCES `fin_articleclass` (`Oid`),
  CONSTRAINT `FK_FIN_Article_CommissionGroup` FOREIGN KEY (`CommissionGroup`) REFERENCES `pos_usercommissiongroup` (`Oid`),
  CONSTRAINT `FK_FIN_Article_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_Article_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_Article_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_Article_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_Article_DiscountGroup` FOREIGN KEY (`DiscountGroup`) REFERENCES `erp_customerdiscountgroup` (`Oid`),
  CONSTRAINT `FK_FIN_Article_Family` FOREIGN KEY (`Family`) REFERENCES `fin_articlefamily` (`Oid`),
  CONSTRAINT `FK_FIN_Article_Printer` FOREIGN KEY (`Printer`) REFERENCES `sys_configurationprinters` (`Oid`),
  CONSTRAINT `FK_FIN_Article_SubFamily` FOREIGN KEY (`SubFamily`) REFERENCES `fin_articlesubfamily` (`Oid`),
  CONSTRAINT `FK_FIN_Article_Template` FOREIGN KEY (`Template`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_FIN_Article_Type` FOREIGN KEY (`Type`) REFERENCES `fin_articletype` (`Oid`),
  CONSTRAINT `FK_FIN_Article_UnitMeasure` FOREIGN KEY (`UnitMeasure`) REFERENCES `cfg_configurationunitmeasure` (`Oid`),
  CONSTRAINT `FK_FIN_Article_UnitSize` FOREIGN KEY (`UnitSize`) REFERENCES `cfg_configurationunitsize` (`Oid`),
  CONSTRAINT `FK_FIN_Article_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_Article_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_Article_VatDirectSelling` FOREIGN KEY (`VatDirectSelling`) REFERENCES `fin_configurationvatrate` (`Oid`),
  CONSTRAINT `FK_FIN_Article_VatExemptionReason` FOREIGN KEY (`VatExemptionReason`) REFERENCES `fin_configurationvatexemptionreason` (`Oid`),
  CONSTRAINT `FK_FIN_Article_VatOnTable` FOREIGN KEY (`VatOnTable`) REFERENCES `fin_configurationvatrate` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_articleclass`
--

DROP TABLE IF EXISTS `fin_articleclass`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_articleclass` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Acronym` varchar(1) DEFAULT NULL,
  `WorkInStock` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ArticleClass` (`Oid`),
  UNIQUE KEY `iCode_FIN_ArticleClass` (`Code`),
  UNIQUE KEY `iDesignation_FIN_ArticleClass` (`Designation`),
  UNIQUE KEY `iAcronym_FIN_ArticleClass` (`Acronym`),
  KEY `iCreatedBy_FIN_ArticleClass` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ArticleClass` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ArticleClass` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ArticleClass` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ArticleClass` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ArticleClass` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ArticleClass_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleClass_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleClass_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleClass_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleClass_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleClass_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_articlefamily`
--

DROP TABLE IF EXISTS `fin_articlefamily`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_articlefamily` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ButtonLabel` varchar(35) DEFAULT NULL,
  `ButtonLabelHide` bit(1) DEFAULT NULL,
  `ButtonImage` varchar(255) DEFAULT NULL,
  `ButtonIcon` varchar(255) DEFAULT NULL,
  `CommissionGroup` char(38) DEFAULT NULL,
  `DiscountGroup` char(38) DEFAULT NULL,
  `Printer` char(38) DEFAULT NULL,
  `Template` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ArticleFamily` (`Oid`),
  UNIQUE KEY `iCode_FIN_ArticleFamily` (`Code`),
  UNIQUE KEY `iDesignation_FIN_ArticleFamily` (`Designation`),
  KEY `iCreatedBy_FIN_ArticleFamily` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ArticleFamily` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ArticleFamily` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ArticleFamily` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ArticleFamily` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ArticleFamily` (`DeletedWhere`),
  KEY `iCommissionGroup_FIN_ArticleFamily` (`CommissionGroup`),
  KEY `iDiscountGroup_FIN_ArticleFamily` (`DiscountGroup`),
  KEY `iPrinter_FIN_ArticleFamily` (`Printer`),
  KEY `iTemplate_FIN_ArticleFamily` (`Template`),
  CONSTRAINT `FK_FIN_ArticleFamily_CommissionGroup` FOREIGN KEY (`CommissionGroup`) REFERENCES `pos_usercommissiongroup` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_DiscountGroup` FOREIGN KEY (`DiscountGroup`) REFERENCES `erp_customerdiscountgroup` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_Printer` FOREIGN KEY (`Printer`) REFERENCES `sys_configurationprinters` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_Template` FOREIGN KEY (`Template`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleFamily_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_articlestock`
--

DROP TABLE IF EXISTS `fin_articlestock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_articlestock` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `Customer` char(38) DEFAULT NULL,
  `DocumentNumber` varchar(50) DEFAULT NULL,
  `Article` char(38) DEFAULT NULL,
  `Quantity` double DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `DocumentDetail` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ArticleStock` (`Oid`),
  KEY `iCreatedBy_FIN_ArticleStock` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ArticleStock` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ArticleStock` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ArticleStock` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ArticleStock` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ArticleStock` (`DeletedWhere`),
  KEY `iCustomer_FIN_ArticleStock` (`Customer`),
  KEY `iArticle_FIN_ArticleStock` (`Article`),
  KEY `iDocumentMaster_FIN_ArticleStock` (`DocumentMaster`),
  KEY `iDocumentDetail_FIN_ArticleStock` (`DocumentDetail`),
  CONSTRAINT `FK_FIN_ArticleStock_Article` FOREIGN KEY (`Article`) REFERENCES `fin_article` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_Customer` FOREIGN KEY (`Customer`) REFERENCES `erp_customer` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_DocumentDetail` FOREIGN KEY (`DocumentDetail`) REFERENCES `fin_documentfinancedetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleStock_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_articlesubfamily`
--

DROP TABLE IF EXISTS `fin_articlesubfamily`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_articlesubfamily` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ButtonLabel` varchar(35) DEFAULT NULL,
  `ButtonLabelHide` bit(1) DEFAULT NULL,
  `ButtonImage` varchar(255) DEFAULT NULL,
  `ButtonIcon` varchar(255) DEFAULT NULL,
  `Family` char(38) DEFAULT NULL,
  `CommissionGroup` char(38) DEFAULT NULL,
  `DiscountGroup` char(38) DEFAULT NULL,
  `VatOnTable` char(38) DEFAULT NULL,
  `VatDirectSelling` char(38) DEFAULT NULL,
  `Printer` char(38) DEFAULT NULL,
  `Template` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ArticleSubFamily` (`Oid`),
  UNIQUE KEY `iCode_FIN_ArticleSubFamily` (`Code`),
  KEY `iCreatedBy_FIN_ArticleSubFamily` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ArticleSubFamily` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ArticleSubFamily` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ArticleSubFamily` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ArticleSubFamily` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ArticleSubFamily` (`DeletedWhere`),
  KEY `iFamily_FIN_ArticleSubFamily` (`Family`),
  KEY `iCommissionGroup_FIN_ArticleSubFamily` (`CommissionGroup`),
  KEY `iDiscountGroup_FIN_ArticleSubFamily` (`DiscountGroup`),
  KEY `iVatOnTable_FIN_ArticleSubFamily` (`VatOnTable`),
  KEY `iVatDirectSelling_FIN_ArticleSubFamily` (`VatDirectSelling`),
  KEY `iPrinter_FIN_ArticleSubFamily` (`Printer`),
  KEY `iTemplate_FIN_ArticleSubFamily` (`Template`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_CommissionGroup` FOREIGN KEY (`CommissionGroup`) REFERENCES `pos_usercommissiongroup` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_DiscountGroup` FOREIGN KEY (`DiscountGroup`) REFERENCES `erp_customerdiscountgroup` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_Family` FOREIGN KEY (`Family`) REFERENCES `fin_articlefamily` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_Printer` FOREIGN KEY (`Printer`) REFERENCES `sys_configurationprinters` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_Template` FOREIGN KEY (`Template`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_VatDirectSelling` FOREIGN KEY (`VatDirectSelling`) REFERENCES `fin_configurationvatrate` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleSubFamily_VatOnTable` FOREIGN KEY (`VatOnTable`) REFERENCES `fin_configurationvatrate` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_articletype`
--

DROP TABLE IF EXISTS `fin_articletype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_articletype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `HavePrice` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ArticleType` (`Oid`),
  UNIQUE KEY `iCode_FIN_ArticleType` (`Code`),
  UNIQUE KEY `iDesignation_FIN_ArticleType` (`Designation`),
  KEY `iCreatedBy_FIN_ArticleType` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ArticleType` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ArticleType` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ArticleType` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ArticleType` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ArticleType` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ArticleType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ArticleType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_configurationpaymentcondition`
--

DROP TABLE IF EXISTS `fin_configurationpaymentcondition`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_configurationpaymentcondition` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Acronym` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ConfigurationPaymentCondition` (`Oid`),
  UNIQUE KEY `iCode_FIN_ConfigurationPaymentCondition` (`Code`),
  UNIQUE KEY `iDesignation_FIN_ConfigurationPaymentCondition` (`Designation`),
  KEY `iCreatedBy_FIN_ConfigurationPaymentCondition` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ConfigurationPaymentCondition` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ConfigurationPaymentCondition` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ConfigurationPaymentCondition` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ConfigurationPaymentCondition` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ConfigurationPaymentCondition` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentCondition_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentCondition_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentCondition_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentCondition_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentCondition_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentCondition_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_configurationpaymentmethod`
--

DROP TABLE IF EXISTS `fin_configurationpaymentmethod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_configurationpaymentmethod` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `ButtonIcon` varchar(255) DEFAULT NULL,
  `Acronym` varchar(100) DEFAULT NULL,
  `AllowPayback` varchar(100) DEFAULT NULL,
  `Symbol` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ConfigurationPaymentMethod` (`Oid`),
  UNIQUE KEY `iCode_FIN_ConfigurationPaymentMethod` (`Code`),
  UNIQUE KEY `iToken_FIN_ConfigurationPaymentMethod` (`Token`),
  UNIQUE KEY `iDesignation_FIN_ConfigurationPaymentMethod` (`Designation`),
  UNIQUE KEY `iResourceString_FIN_ConfigurationPaymentMethod` (`ResourceString`),
  KEY `iCreatedBy_FIN_ConfigurationPaymentMethod` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ConfigurationPaymentMethod` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ConfigurationPaymentMethod` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ConfigurationPaymentMethod` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ConfigurationPaymentMethod` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ConfigurationPaymentMethod` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentMethod_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentMethod_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentMethod_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentMethod_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentMethod_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPaymentMethod_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_configurationpricetype`
--

DROP TABLE IF EXISTS `fin_configurationpricetype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_configurationpricetype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `EnumValue` int(11) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ConfigurationPriceType` (`Oid`),
  UNIQUE KEY `iCode_FIN_ConfigurationPriceType` (`Code`),
  UNIQUE KEY `iDesignation_FIN_ConfigurationPriceType` (`Designation`),
  UNIQUE KEY `iEnumValue_FIN_ConfigurationPriceType` (`EnumValue`),
  KEY `iCreatedBy_FIN_ConfigurationPriceType` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ConfigurationPriceType` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ConfigurationPriceType` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ConfigurationPriceType` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ConfigurationPriceType` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ConfigurationPriceType` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ConfigurationPriceType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPriceType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPriceType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPriceType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPriceType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationPriceType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_configurationvatexemptionreason`
--

DROP TABLE IF EXISTS `fin_configurationvatexemptionreason`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_configurationvatexemptionreason` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(60) DEFAULT NULL,
  `Acronym` varchar(3) DEFAULT NULL,
  `StandardApplicable` text,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ConfigurationVatExemptionReason` (`Oid`),
  UNIQUE KEY `iCode_FIN_ConfigurationVatExemptionReason` (`Code`),
  KEY `iCreatedBy_FIN_ConfigurationVatExemptionReason` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ConfigurationVatExemptionReason` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ConfigurationVatExemptionReason` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ConfigurationVatExemptionReason` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ConfigurationVatExemptionReason` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ConfigurationVatExemptionReason` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ConfigurationVatExemptionReason_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatExemptionReason_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatExemptionReason_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatExemptionReason_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatExemptionReason_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatExemptionReason_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_configurationvatrate`
--

DROP TABLE IF EXISTS `fin_configurationvatrate`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_configurationvatrate` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Value` double DEFAULT NULL,
  `ReasonCode` varchar(100) DEFAULT NULL,
  `TaxType` varchar(3) DEFAULT NULL,
  `TaxCode` varchar(10) DEFAULT NULL,
  `TaxCountryRegion` varchar(5) DEFAULT NULL,
  `TaxExpirationDate` datetime DEFAULT NULL,
  `TaxDescription` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_ConfigurationVatRate` (`Oid`),
  UNIQUE KEY `iCode_FIN_ConfigurationVatRate` (`Code`),
  UNIQUE KEY `iDesignation_FIN_ConfigurationVatRate` (`Designation`),
  KEY `iCreatedBy_FIN_ConfigurationVatRate` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_ConfigurationVatRate` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_ConfigurationVatRate` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_ConfigurationVatRate` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_ConfigurationVatRate` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_ConfigurationVatRate` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_ConfigurationVatRate_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatRate_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatRate_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatRate_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatRate_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_ConfigurationVatRate_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancecommission`
--

DROP TABLE IF EXISTS `fin_documentfinancecommission`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancecommission` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `Commission` double DEFAULT NULL,
  `Total` double DEFAULT NULL,
  `CommissionGroup` char(38) DEFAULT NULL,
  `FinanceMaster` char(38) DEFAULT NULL,
  `FinanceDetail` char(38) DEFAULT NULL,
  `UserDetail` char(38) DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceCommission` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinanceCommission` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceCommission` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceCommission` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceCommission` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceCommission` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceCommission` (`DeletedWhere`),
  KEY `iCommissionGroup_FIN_DocumentFinanceCommission` (`CommissionGroup`),
  KEY `iFinanceMaster_FIN_DocumentFinanceCommission` (`FinanceMaster`),
  KEY `iFinanceDetail_FIN_DocumentFinanceCommission` (`FinanceDetail`),
  KEY `iUserDetail_FIN_DocumentFinanceCommission` (`UserDetail`),
  KEY `iTerminal_FIN_DocumentFinanceCommission` (`Terminal`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_CommissionGroup` FOREIGN KEY (`CommissionGroup`) REFERENCES `pos_usercommissiongroup` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_FinanceDetail` FOREIGN KEY (`FinanceDetail`) REFERENCES `fin_documentfinancedetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_FinanceMaster` FOREIGN KEY (`FinanceMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceCommission_UserDetail` FOREIGN KEY (`UserDetail`) REFERENCES `sys_userdetail` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancedetail`
--

DROP TABLE IF EXISTS `fin_documentfinancedetail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancedetail` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` varchar(100) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Quantity` double DEFAULT NULL,
  `UnitMeasure` varchar(35) DEFAULT NULL,
  `Price` double DEFAULT NULL,
  `Vat` double DEFAULT NULL,
  `VatExemptionReasonDesignation` varchar(255) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `TotalNet` double DEFAULT NULL,
  `TotalGross` double DEFAULT NULL,
  `TotalDiscount` double DEFAULT NULL,
  `TotalTax` double DEFAULT NULL,
  `TotalFinal` double DEFAULT NULL,
  `PriceType` int(11) DEFAULT NULL,
  `PriceFinal` double DEFAULT NULL,
  `Token1` varchar(255) DEFAULT NULL,
  `Token2` varchar(255) DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `Article` char(38) DEFAULT NULL,
  `VatRate` char(38) DEFAULT NULL,
  `VatExemptionReason` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceDetail` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinanceDetail` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceDetail` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceDetail` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceDetail` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceDetail` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceDetail` (`DeletedWhere`),
  KEY `iDocumentMaster_FIN_DocumentFinanceDetail` (`DocumentMaster`),
  KEY `iArticle_FIN_DocumentFinanceDetail` (`Article`),
  KEY `iVatRate_FIN_DocumentFinanceDetail` (`VatRate`),
  KEY `iVatExemptionReason_FIN_DocumentFinanceDetail` (`VatExemptionReason`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_Article` FOREIGN KEY (`Article`) REFERENCES `fin_article` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_VatExemptionReason` FOREIGN KEY (`VatExemptionReason`) REFERENCES `fin_configurationvatexemptionreason` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetail_VatRate` FOREIGN KEY (`VatRate`) REFERENCES `fin_configurationvatrate` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancedetailorderreference`
--

DROP TABLE IF EXISTS `fin_documentfinancedetailorderreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancedetailorderreference` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `OriginatingON` varchar(60) DEFAULT NULL,
  `OrderDate` datetime DEFAULT NULL,
  `DocumentDetail` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceDetailOrderReference` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinanceDetailOrderReference` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceDetailOrderReference` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceDetailOrderReference` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceDetailOrderReference` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceDetailOrderReference` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceDetailOrderReference` (`DeletedWhere`),
  KEY `iDocumentMaster_FIN_DocumentFinanceDetailOrderReference` (`DocumentMaster`),
  KEY `iDocumentDetail_FIN_DocumentFinanceDetailOrderReference` (`DocumentDetail`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_DocumentDetail` FOREIGN KEY (`DocumentDetail`) REFERENCES `fin_documentfinancedetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailOrderReference_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancedetailreference`
--

DROP TABLE IF EXISTS `fin_documentfinancedetailreference`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancedetailreference` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `Reference` varchar(60) DEFAULT NULL,
  `Reason` varchar(50) DEFAULT NULL,
  `DocumentDetail` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceDetailReference` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinanceDetailReference` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceDetailReference` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceDetailReference` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceDetailReference` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceDetailReference` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceDetailReference` (`DeletedWhere`),
  KEY `iDocumentMaster_FIN_DocumentFinanceDetailReference` (`DocumentMaster`),
  KEY `iDocumentDetail_FIN_DocumentFinanceDetailReference` (`DocumentDetail`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_DocumentDetail` FOREIGN KEY (`DocumentDetail`) REFERENCES `fin_documentfinancedetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceDetailReference_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancemaster`
--

DROP TABLE IF EXISTS `fin_documentfinancemaster`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancemaster` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `DocumentNumber` varchar(50) DEFAULT NULL,
  `DocumentStatusStatus` varchar(1) DEFAULT NULL,
  `DocumentStatusDate` varchar(19) DEFAULT NULL,
  `DocumentStatusReason` varchar(50) DEFAULT NULL,
  `DocumentStatusUser` varchar(30) DEFAULT NULL,
  `SourceBilling` varchar(1) DEFAULT NULL,
  `Hash` varchar(172) DEFAULT NULL,
  `HashControl` varchar(40) DEFAULT NULL,
  `DocumentDate` varchar(19) DEFAULT NULL,
  `SelfBillingIndicator` int(11) DEFAULT NULL,
  `CashVatSchemeIndicator` int(11) DEFAULT NULL,
  `ThirdPartiesBillingIndicator` int(11) DEFAULT NULL,
  `DocumentCreatorUser` varchar(30) DEFAULT NULL,
  `EACCode` varchar(5) DEFAULT NULL,
  `SystemEntryDate` varchar(50) DEFAULT NULL,
  `TransactionID` varchar(70) DEFAULT NULL,
  `ShipToDeliveryID` varchar(255) DEFAULT NULL,
  `ShipToDeliveryDate` datetime DEFAULT NULL,
  `ShipToWarehouseID` varchar(50) DEFAULT NULL,
  `ShipToLocationID` varchar(30) DEFAULT NULL,
  `ShipToBuildingNumber` varchar(10) DEFAULT NULL,
  `ShipToStreetName` varchar(90) DEFAULT NULL,
  `ShipToAddressDetail` varchar(100) DEFAULT NULL,
  `ShipToCity` varchar(50) DEFAULT NULL,
  `ShipToPostalCode` varchar(20) DEFAULT NULL,
  `ShipToRegion` varchar(50) DEFAULT NULL,
  `ShipToCountry` varchar(5) DEFAULT NULL,
  `ShipFromDeliveryID` varchar(255) DEFAULT NULL,
  `ShipFromDeliveryDate` datetime DEFAULT NULL,
  `ShipFromWarehouseID` varchar(50) DEFAULT NULL,
  `ShipFromLocationID` varchar(30) DEFAULT NULL,
  `ShipFromBuildingNumber` varchar(10) DEFAULT NULL,
  `ShipFromStreetName` varchar(90) DEFAULT NULL,
  `ShipFromAddressDetail` varchar(100) DEFAULT NULL,
  `ShipFromCity` varchar(50) DEFAULT NULL,
  `ShipFromPostalCode` varchar(20) DEFAULT NULL,
  `ShipFromRegion` varchar(50) DEFAULT NULL,
  `ShipFromCountry` varchar(5) DEFAULT NULL,
  `MovementStartTime` datetime DEFAULT NULL,
  `MovementEndTime` datetime DEFAULT NULL,
  `TotalNet` double DEFAULT NULL,
  `TotalGross` double DEFAULT NULL,
  `TotalDiscount` double DEFAULT NULL,
  `TotalTax` double DEFAULT NULL,
  `TotalFinal` double DEFAULT NULL,
  `TotalFinalRound` double DEFAULT NULL,
  `TotalDelivery` double DEFAULT NULL,
  `TotalChange` double DEFAULT NULL,
  `ExternalDocument` varchar(50) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `DiscountFinancial` double DEFAULT NULL,
  `ExchangeRate` double DEFAULT NULL,
  `EntityOid` char(38) DEFAULT NULL,
  `EntityInternalCode` varchar(30) DEFAULT NULL,
  `EntityName` varchar(100) DEFAULT NULL,
  `EntityAddress` varchar(100) DEFAULT NULL,
  `EntityLocality` varchar(100) DEFAULT NULL,
  `EntityZipCode` varchar(10) DEFAULT NULL,
  `EntityCity` varchar(100) DEFAULT NULL,
  `EntityCountry` varchar(5) DEFAULT NULL,
  `EntityCountryOid` char(38) DEFAULT NULL,
  `EntityFiscalNumber` varchar(100) DEFAULT NULL,
  `Payed` bit(1) DEFAULT NULL,
  `PayedDate` datetime DEFAULT NULL,
  `Printed` bit(1) DEFAULT NULL,
  `SourceOrderMain` char(38) DEFAULT NULL,
  `DocumentParent` char(38) DEFAULT NULL,
  `DocumentChild` char(38) DEFAULT NULL,
  `ATDocCodeID` varchar(200) DEFAULT NULL,
  `ATValidAuditResult` char(38) DEFAULT NULL,
  `ATResendDocument` bit(1) DEFAULT NULL,
  `DocumentType` char(38) DEFAULT NULL,
  `DocumentSerie` char(38) DEFAULT NULL,
  `PaymentMethod` char(38) DEFAULT NULL,
  `PaymentCondition` char(38) DEFAULT NULL,
  `Currency` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceMaster` (`Oid`),
  UNIQUE KEY `iDocumentNumber_FIN_DocumentFinanceMaster` (`DocumentNumber`),
  KEY `iCreatedBy_FIN_DocumentFinanceMaster` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceMaster` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceMaster` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceMaster` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceMaster` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceMaster` (`DeletedWhere`),
  KEY `iSourceOrderMain_FIN_DocumentFinanceMaster` (`SourceOrderMain`),
  KEY `iDocumentParent_FIN_DocumentFinanceMaster` (`DocumentParent`),
  KEY `iDocumentChild_FIN_DocumentFinanceMaster` (`DocumentChild`),
  KEY `iATValidAuditResult_FIN_DocumentFinanceMaster` (`ATValidAuditResult`),
  KEY `iDocumentType_FIN_DocumentFinanceMaster` (`DocumentType`),
  KEY `iDocumentSerie_FIN_DocumentFinanceMaster` (`DocumentSerie`),
  KEY `iPaymentMethod_FIN_DocumentFinanceMaster` (`PaymentMethod`),
  KEY `iPaymentCondition_FIN_DocumentFinanceMaster` (`PaymentCondition`),
  KEY `iCurrency_FIN_DocumentFinanceMaster` (`Currency`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_ATValidAuditResult` FOREIGN KEY (`ATValidAuditResult`) REFERENCES `sys_systemauditat` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_Currency` FOREIGN KEY (`Currency`) REFERENCES `cfg_configurationcurrency` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_DocumentChild` FOREIGN KEY (`DocumentChild`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_DocumentParent` FOREIGN KEY (`DocumentParent`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_DocumentSerie` FOREIGN KEY (`DocumentSerie`) REFERENCES `fin_documentfinanceseries` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_DocumentType` FOREIGN KEY (`DocumentType`) REFERENCES `fin_documentfinancetype` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_PaymentCondition` FOREIGN KEY (`PaymentCondition`) REFERENCES `fin_configurationpaymentcondition` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_PaymentMethod` FOREIGN KEY (`PaymentMethod`) REFERENCES `fin_configurationpaymentmethod` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_SourceOrderMain` FOREIGN KEY (`SourceOrderMain`) REFERENCES `fin_documentordermain` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMaster_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancemasterpayment`
--

DROP TABLE IF EXISTS `fin_documentfinancemasterpayment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancemasterpayment` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `CreditAmount` double DEFAULT NULL,
  `DebitAmount` double DEFAULT NULL,
  `DocumentFinanceMaster` char(38) DEFAULT NULL,
  `DocumentFinancePayment` char(38) DEFAULT NULL,
  `DocumentFinanceMasterCreditNote` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceMasterPayment` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinanceMasterPayment` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceMasterPayment` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceMasterPayment` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceMasterPayment` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceMasterPayment` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceMasterPayment` (`DeletedWhere`),
  KEY `iDocumentFinanceMaster_FIN_DocumentFinanceMasterPayment` (`DocumentFinanceMaster`),
  KEY `iDocumentFinancePayment_FIN_DocumentFinanceMasterPayment` (`DocumentFinancePayment`),
  KEY `iDocumentFinanceMasterCreditNote_FIN_DocumentFinanceMas_69586DDB` (`DocumentFinanceMasterCreditNote`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_DocumentFinanceMast_A8C68AC8` FOREIGN KEY (`DocumentFinanceMasterCreditNote`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_DocumentFinanceMaster` FOREIGN KEY (`DocumentFinanceMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_DocumentFinancePayment` FOREIGN KEY (`DocumentFinancePayment`) REFERENCES `fin_documentfinancepayment` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterPayment_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancemastertotal`
--

DROP TABLE IF EXISTS `fin_documentfinancemastertotal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancemastertotal` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Value` double DEFAULT NULL,
  `Total` double DEFAULT NULL,
  `TotalBase` double DEFAULT NULL,
  `TotalType` int(11) DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceMasterTotal` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinanceMasterTotal` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceMasterTotal` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceMasterTotal` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceMasterTotal` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceMasterTotal` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceMasterTotal` (`DeletedWhere`),
  KEY `iDocumentMaster_FIN_DocumentFinanceMasterTotal` (`DocumentMaster`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceMasterTotal_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancepayment`
--

DROP TABLE IF EXISTS `fin_documentfinancepayment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancepayment` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `PaymentRefNo` varchar(60) DEFAULT NULL,
  `TransactionID` varchar(70) DEFAULT NULL,
  `TransactionDate` varchar(19) DEFAULT NULL,
  `PaymentType` varchar(2) DEFAULT NULL,
  `PaymentStatus` varchar(1) DEFAULT NULL,
  `PaymentStatusDate` varchar(50) DEFAULT NULL,
  `Reason` varchar(50) DEFAULT NULL,
  `DocumentStatusSourceID` varchar(30) DEFAULT NULL,
  `SourcePayment` varchar(1) DEFAULT NULL,
  `PaymentMechanism` varchar(2) DEFAULT NULL,
  `PaymentAmount` double DEFAULT NULL,
  `PaymentDate` varchar(19) DEFAULT NULL,
  `SourceID` varchar(30) DEFAULT NULL,
  `SystemEntryDate` varchar(50) DEFAULT NULL,
  `TaxPayable` double DEFAULT NULL,
  `NetTotal` double DEFAULT NULL,
  `GrossTotal` double DEFAULT NULL,
  `SettlementAmount` double DEFAULT NULL,
  `CurrencyCode` varchar(3) DEFAULT NULL,
  `CurrencyAmount` double DEFAULT NULL,
  `ExchangeRate` double DEFAULT NULL,
  `WithholdingTaxAmount` double DEFAULT NULL,
  `EntityOid` char(38) DEFAULT NULL,
  `EntityInternalCode` varchar(30) DEFAULT NULL,
  `DocumentDate` varchar(19) DEFAULT NULL,
  `ExtendedValue` text,
  `DocumentType` char(38) DEFAULT NULL,
  `DocumentSerie` char(38) DEFAULT NULL,
  `PaymentMethod` char(38) DEFAULT NULL,
  `Currency` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinancePayment` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentFinancePayment` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinancePayment` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinancePayment` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinancePayment` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinancePayment` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinancePayment` (`DeletedWhere`),
  KEY `iDocumentType_FIN_DocumentFinancePayment` (`DocumentType`),
  KEY `iDocumentSerie_FIN_DocumentFinancePayment` (`DocumentSerie`),
  KEY `iPaymentMethod_FIN_DocumentFinancePayment` (`PaymentMethod`),
  KEY `iCurrency_FIN_DocumentFinancePayment` (`Currency`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_Currency` FOREIGN KEY (`Currency`) REFERENCES `cfg_configurationcurrency` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_DocumentSerie` FOREIGN KEY (`DocumentSerie`) REFERENCES `fin_documentfinanceseries` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_DocumentType` FOREIGN KEY (`DocumentType`) REFERENCES `fin_documentfinancetype` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_PaymentMethod` FOREIGN KEY (`PaymentMethod`) REFERENCES `fin_configurationpaymentmethod` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinancePayment_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinanceseries`
--

DROP TABLE IF EXISTS `fin_documentfinanceseries`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinanceseries` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `NextDocumentNumber` int(11) DEFAULT NULL,
  `DocumentNumberRangeBegin` int(11) DEFAULT NULL,
  `DocumentNumberRangeEnd` int(11) DEFAULT NULL,
  `Acronym` varchar(100) DEFAULT NULL,
  `DocumentType` char(38) DEFAULT NULL,
  `FiscalYear` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceSeries` (`Oid`),
  UNIQUE KEY `iDesignation_FIN_DocumentFinanceSeries` (`Designation`),
  UNIQUE KEY `iAcronym_FIN_DocumentFinanceSeries` (`Acronym`),
  KEY `iCreatedBy_FIN_DocumentFinanceSeries` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceSeries` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceSeries` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceSeries` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceSeries` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceSeries` (`DeletedWhere`),
  KEY `iDocumentType_FIN_DocumentFinanceSeries` (`DocumentType`),
  KEY `iFiscalYear_FIN_DocumentFinanceSeries` (`FiscalYear`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_DocumentType` FOREIGN KEY (`DocumentType`) REFERENCES `fin_documentfinancetype` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_FiscalYear` FOREIGN KEY (`FiscalYear`) REFERENCES `fin_documentfinanceyears` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceSeries_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinancetype`
--

DROP TABLE IF EXISTS `fin_documentfinancetype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinancetype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Acronym` varchar(4) DEFAULT NULL,
  `AcronymLastSerie` int(11) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `ResourceStringReport` varchar(100) DEFAULT NULL,
  `Payed` bit(1) DEFAULT NULL,
  `Credit` bit(1) DEFAULT NULL,
  `PrintCopies` int(11) DEFAULT NULL,
  `PrintRequestMotive` bit(1) DEFAULT NULL,
  `PrintRequestConfirmation` bit(1) DEFAULT NULL,
  `PrintOpenDrawer` bit(1) DEFAULT NULL,
  `WayBill` bit(1) DEFAULT NULL,
  `WsAtDocument` bit(1) DEFAULT NULL,
  `SaftAuditFile` bit(1) DEFAULT NULL,
  `SaftDocumentType` int(11) DEFAULT NULL,
  `StockMode` int(11) DEFAULT NULL,
  `Printer` char(38) DEFAULT NULL,
  `Template` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceType` (`Oid`),
  UNIQUE KEY `iCode_FIN_DocumentFinanceType` (`Code`),
  UNIQUE KEY `iDesignation_FIN_DocumentFinanceType` (`Designation`),
  KEY `iCreatedBy_FIN_DocumentFinanceType` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceType` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceType` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceType` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceType` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceType` (`DeletedWhere`),
  KEY `iPrinter_FIN_DocumentFinanceType` (`Printer`),
  KEY `iTemplate_FIN_DocumentFinanceType` (`Template`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_Printer` FOREIGN KEY (`Printer`) REFERENCES `sys_configurationprinters` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_Template` FOREIGN KEY (`Template`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinanceyears`
--

DROP TABLE IF EXISTS `fin_documentfinanceyears`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinanceyears` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `FiscalYear` int(11) DEFAULT NULL,
  `Acronym` varchar(100) DEFAULT NULL,
  `SeriesForEachTerminal` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceYears` (`Oid`),
  UNIQUE KEY `iCode_FIN_DocumentFinanceYears` (`Code`),
  UNIQUE KEY `iDesignation_FIN_DocumentFinanceYears` (`Designation`),
  UNIQUE KEY `iAcronym_FIN_DocumentFinanceYears` (`Acronym`),
  KEY `iFiscalYear_FIN_DocumentFinanceYears` (`FiscalYear`),
  KEY `iCreatedBy_FIN_DocumentFinanceYears` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceYears` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceYears` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceYears` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceYears` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceYears` (`DeletedWhere`),
  CONSTRAINT `FK_FIN_DocumentFinanceYears_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYears_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYears_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYears_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYears_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYears_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentfinanceyearserieterminal`
--

DROP TABLE IF EXISTS `fin_documentfinanceyearserieterminal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentfinanceyearserieterminal` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `FiscalYear` char(38) DEFAULT NULL,
  `DocumentType` char(38) DEFAULT NULL,
  `Serie` char(38) DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `Printer` char(38) DEFAULT NULL,
  `Template` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentFinanceYearSerieTerminal` (`Oid`),
  UNIQUE KEY `iDesignation_FIN_DocumentFinanceYearSerieTerminal` (`Designation`),
  KEY `iCreatedBy_FIN_DocumentFinanceYearSerieTerminal` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentFinanceYearSerieTerminal` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentFinanceYearSerieTerminal` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentFinanceYearSerieTerminal` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentFinanceYearSerieTerminal` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentFinanceYearSerieTerminal` (`DeletedWhere`),
  KEY `iFiscalYear_FIN_DocumentFinanceYearSerieTerminal` (`FiscalYear`),
  KEY `iDocumentType_FIN_DocumentFinanceYearSerieTerminal` (`DocumentType`),
  KEY `iSerie_FIN_DocumentFinanceYearSerieTerminal` (`Serie`),
  KEY `iTerminal_FIN_DocumentFinanceYearSerieTerminal` (`Terminal`),
  KEY `iPrinter_FIN_DocumentFinanceYearSerieTerminal` (`Printer`),
  KEY `iTemplate_FIN_DocumentFinanceYearSerieTerminal` (`Template`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_DocumentType` FOREIGN KEY (`DocumentType`) REFERENCES `fin_documentfinancetype` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_FiscalYear` FOREIGN KEY (`FiscalYear`) REFERENCES `fin_documentfinanceyears` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_Printer` FOREIGN KEY (`Printer`) REFERENCES `sys_configurationprinters` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_Serie` FOREIGN KEY (`Serie`) REFERENCES `fin_documentfinanceseries` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_Template` FOREIGN KEY (`Template`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentFinanceYearSerieTerminal_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentorderdetail`
--

DROP TABLE IF EXISTS `fin_documentorderdetail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentorderdetail` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` varchar(100) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Quantity` double DEFAULT NULL,
  `UnitMeasure` varchar(35) DEFAULT NULL,
  `Price` double DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `Vat` double DEFAULT NULL,
  `VatExemptionReason` char(38) DEFAULT NULL,
  `TotalGross` double DEFAULT NULL,
  `TotalDiscount` double DEFAULT NULL,
  `TotalTax` double DEFAULT NULL,
  `TotalFinal` double DEFAULT NULL,
  `Token1` varchar(255) DEFAULT NULL,
  `Token2` varchar(255) DEFAULT NULL,
  `OrderTicket` char(38) DEFAULT NULL,
  `Article` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentOrderDetail` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentOrderDetail` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentOrderDetail` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentOrderDetail` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentOrderDetail` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentOrderDetail` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentOrderDetail` (`DeletedWhere`),
  KEY `iOrderTicket_FIN_DocumentOrderDetail` (`OrderTicket`),
  KEY `iArticle_FIN_DocumentOrderDetail` (`Article`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_Article` FOREIGN KEY (`Article`) REFERENCES `fin_article` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_OrderTicket` FOREIGN KEY (`OrderTicket`) REFERENCES `fin_documentorderticket` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderDetail_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentordermain`
--

DROP TABLE IF EXISTS `fin_documentordermain`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentordermain` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `DateStart` datetime DEFAULT NULL,
  `OrderStatus` int(11) DEFAULT NULL,
  `PlaceTable` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentOrderMain` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentOrderMain` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentOrderMain` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentOrderMain` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentOrderMain` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentOrderMain` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentOrderMain` (`DeletedWhere`),
  KEY `iPlaceTable_FIN_DocumentOrderMain` (`PlaceTable`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_PlaceTable` FOREIGN KEY (`PlaceTable`) REFERENCES `pos_configurationplacetable` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderMain_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `fin_documentorderticket`
--

DROP TABLE IF EXISTS `fin_documentorderticket`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `fin_documentorderticket` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `TicketId` int(11) DEFAULT NULL,
  `DateStart` datetime DEFAULT NULL,
  `PriceType` int(11) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `OrderMain` char(38) DEFAULT NULL,
  `PlaceTable` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_FIN_DocumentOrderTicket` (`Oid`),
  KEY `iCreatedBy_FIN_DocumentOrderTicket` (`CreatedBy`),
  KEY `iCreatedWhere_FIN_DocumentOrderTicket` (`CreatedWhere`),
  KEY `iUpdatedBy_FIN_DocumentOrderTicket` (`UpdatedBy`),
  KEY `iUpdatedWhere_FIN_DocumentOrderTicket` (`UpdatedWhere`),
  KEY `iDeletedBy_FIN_DocumentOrderTicket` (`DeletedBy`),
  KEY `iDeletedWhere_FIN_DocumentOrderTicket` (`DeletedWhere`),
  KEY `iOrderMain_FIN_DocumentOrderTicket` (`OrderMain`),
  KEY `iPlaceTable_FIN_DocumentOrderTicket` (`PlaceTable`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_OrderMain` FOREIGN KEY (`OrderMain`) REFERENCES `fin_documentordermain` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_PlaceTable` FOREIGN KEY (`PlaceTable`) REFERENCES `pos_configurationplacetable` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_FIN_DocumentOrderTicket_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationcashregister`
--

DROP TABLE IF EXISTS `pos_configurationcashregister`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationcashregister` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Printer` varchar(100) DEFAULT NULL,
  `Drawer` varchar(100) DEFAULT NULL,
  `AutomaticDrawer` varchar(100) DEFAULT NULL,
  `ActiveSales` varchar(100) DEFAULT NULL,
  `AllowChargeBacks` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationCashRegister` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationCashRegister` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationCashRegister` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationCashRegister` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationCashRegister` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationCashRegister` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationCashRegister` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationCashRegister` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationCashRegister` (`DeletedWhere`),
  CONSTRAINT `FK_POS_ConfigurationCashRegister_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationCashRegister_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationCashRegister_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationCashRegister_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationCashRegister_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationCashRegister_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationdevice`
--

DROP TABLE IF EXISTS `pos_configurationdevice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationdevice` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Type` varchar(100) DEFAULT NULL,
  `Properties` longtext,
  `PlaceTerminal` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationDevice` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationDevice` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationDevice` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationDevice` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationDevice` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationDevice` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationDevice` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationDevice` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationDevice` (`DeletedWhere`),
  KEY `iPlaceTerminal_POS_ConfigurationDevice` (`PlaceTerminal`),
  CONSTRAINT `FK_POS_ConfigurationDevice_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationDevice_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationDevice_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationDevice_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationDevice_PlaceTerminal` FOREIGN KEY (`PlaceTerminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationDevice_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationDevice_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationkeyboard`
--

DROP TABLE IF EXISTS `pos_configurationkeyboard`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationkeyboard` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Language` varchar(100) DEFAULT NULL,
  `VirtualKeyboard` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationKeyboard` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationKeyboard` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationKeyboard` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationKeyboard` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationKeyboard` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationKeyboard` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationKeyboard` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationKeyboard` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationKeyboard` (`DeletedWhere`),
  CONSTRAINT `FK_POS_ConfigurationKeyboard_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationKeyboard_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationKeyboard_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationKeyboard_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationKeyboard_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationKeyboard_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationmaintenance`
--

DROP TABLE IF EXISTS `pos_configurationmaintenance`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationmaintenance` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Date` varchar(100) DEFAULT NULL,
  `Time` varchar(100) DEFAULT NULL,
  `PasswordAccess` varchar(100) DEFAULT NULL,
  `Remarks` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationMaintenance` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationMaintenance` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationMaintenance` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationMaintenance` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationMaintenance` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationMaintenance` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationMaintenance` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationMaintenance` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationMaintenance` (`DeletedWhere`),
  CONSTRAINT `FK_POS_ConfigurationMaintenance_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationMaintenance_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationMaintenance_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationMaintenance_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationMaintenance_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationMaintenance_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationplace`
--

DROP TABLE IF EXISTS `pos_configurationplace`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationplace` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ButtonImage` varchar(255) DEFAULT NULL,
  `TypeSubtotal` varchar(100) DEFAULT NULL,
  `AccountType` varchar(100) DEFAULT NULL,
  `OrderPrintMode` int(11) DEFAULT NULL,
  `PriceType` char(38) DEFAULT NULL,
  `MovementType` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationPlace` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationPlace` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationPlace` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationPlace` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationPlace` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationPlace` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationPlace` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationPlace` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationPlace` (`DeletedWhere`),
  KEY `iPriceType_POS_ConfigurationPlace` (`PriceType`),
  KEY `iMovementType_POS_ConfigurationPlace` (`MovementType`),
  CONSTRAINT `FK_POS_ConfigurationPlace_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_MovementType` FOREIGN KEY (`MovementType`) REFERENCES `pos_configurationplacemovementtype` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_PriceType` FOREIGN KEY (`PriceType`) REFERENCES `fin_configurationpricetype` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlace_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationplacemovementtype`
--

DROP TABLE IF EXISTS `pos_configurationplacemovementtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationplacemovementtype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `VatDirectSelling` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationPlaceMovementType` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationPlaceMovementType` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationPlaceMovementType` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationPlaceMovementType` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationPlaceMovementType` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationPlaceMovementType` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationPlaceMovementType` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationPlaceMovementType` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationPlaceMovementType` (`DeletedWhere`),
  CONSTRAINT `FK_POS_ConfigurationPlaceMovementType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceMovementType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceMovementType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceMovementType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceMovementType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceMovementType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationplacetable`
--

DROP TABLE IF EXISTS `pos_configurationplacetable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationplacetable` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ButtonImage` varchar(255) DEFAULT NULL,
  `Discount` double DEFAULT NULL,
  `TableStatus` int(11) DEFAULT NULL,
  `TotalOpen` double DEFAULT NULL,
  `DateTableOpen` datetime DEFAULT NULL,
  `DateTableClosed` datetime DEFAULT NULL,
  `Place` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationPlaceTable` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationPlaceTable` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationPlaceTable` (`Designation`),
  KEY `iCreatedBy_POS_ConfigurationPlaceTable` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationPlaceTable` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationPlaceTable` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationPlaceTable` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationPlaceTable` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationPlaceTable` (`DeletedWhere`),
  KEY `iPlace_POS_ConfigurationPlaceTable` (`Place`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_Place` FOREIGN KEY (`Place`) REFERENCES `pos_configurationplace` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTable_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_configurationplaceterminal`
--

DROP TABLE IF EXISTS `pos_configurationplaceterminal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_configurationplaceterminal` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `HardwareId` varchar(30) DEFAULT NULL,
  `InputReaderTimerInterval` int(10) unsigned DEFAULT NULL,
  `Place` char(38) DEFAULT NULL,
  `Printer` char(38) DEFAULT NULL,
  `BarcodeReader` char(38) DEFAULT NULL,
  `CardReader` char(38) DEFAULT NULL,
  `PoleDisplay` char(38) DEFAULT NULL,
  `WeighingMachine` char(38) DEFAULT NULL,
  `TemplateTicket` char(38) DEFAULT NULL,
  `TemplateTablesConsult` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_ConfigurationPlaceTerminal` (`Oid`),
  UNIQUE KEY `iCode_POS_ConfigurationPlaceTerminal` (`Code`),
  UNIQUE KEY `iDesignation_POS_ConfigurationPlaceTerminal` (`Designation`),
  UNIQUE KEY `iHardwareId_POS_ConfigurationPlaceTerminal` (`HardwareId`),
  KEY `iCreatedBy_POS_ConfigurationPlaceTerminal` (`CreatedBy`),
  KEY `iCreatedWhere_POS_ConfigurationPlaceTerminal` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_ConfigurationPlaceTerminal` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_ConfigurationPlaceTerminal` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_ConfigurationPlaceTerminal` (`DeletedBy`),
  KEY `iDeletedWhere_POS_ConfigurationPlaceTerminal` (`DeletedWhere`),
  KEY `iPlace_POS_ConfigurationPlaceTerminal` (`Place`),
  KEY `iPrinter_POS_ConfigurationPlaceTerminal` (`Printer`),
  KEY `iBarcodeReader_POS_ConfigurationPlaceTerminal` (`BarcodeReader`),
  KEY `iCardReader_POS_ConfigurationPlaceTerminal` (`CardReader`),
  KEY `iPoleDisplay_POS_ConfigurationPlaceTerminal` (`PoleDisplay`),
  KEY `iWeighingMachine_POS_ConfigurationPlaceTerminal` (`WeighingMachine`),
  KEY `iTemplateTicket_POS_ConfigurationPlaceTerminal` (`TemplateTicket`),
  KEY `iTemplateTablesConsult_POS_ConfigurationPlaceTerminal` (`TemplateTablesConsult`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_BarcodeReader` FOREIGN KEY (`BarcodeReader`) REFERENCES `sys_configurationinputreader` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_CardReader` FOREIGN KEY (`CardReader`) REFERENCES `sys_configurationinputreader` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_Place` FOREIGN KEY (`Place`) REFERENCES `pos_configurationplace` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_PoleDisplay` FOREIGN KEY (`PoleDisplay`) REFERENCES `sys_configurationpoledisplay` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_Printer` FOREIGN KEY (`Printer`) REFERENCES `sys_configurationprinters` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_TemplateTablesConsult` FOREIGN KEY (`TemplateTablesConsult`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_TemplateTicket` FOREIGN KEY (`TemplateTicket`) REFERENCES `sys_configurationprinterstemplates` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_ConfigurationPlaceTerminal_WeighingMachine` FOREIGN KEY (`WeighingMachine`) REFERENCES `sys_configurationweighingmachine` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_usercommissiongroup`
--

DROP TABLE IF EXISTS `pos_usercommissiongroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_usercommissiongroup` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Commission` double DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_UserCommissionGroup` (`Oid`),
  UNIQUE KEY `iCode_POS_UserCommissionGroup` (`Code`),
  UNIQUE KEY `iDesignation_POS_UserCommissionGroup` (`Designation`),
  KEY `iCreatedBy_POS_UserCommissionGroup` (`CreatedBy`),
  KEY `iCreatedWhere_POS_UserCommissionGroup` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_UserCommissionGroup` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_UserCommissionGroup` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_UserCommissionGroup` (`DeletedBy`),
  KEY `iDeletedWhere_POS_UserCommissionGroup` (`DeletedWhere`),
  CONSTRAINT `FK_POS_UserCommissionGroup_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_UserCommissionGroup_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_UserCommissionGroup_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_UserCommissionGroup_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_UserCommissionGroup_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_UserCommissionGroup_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_worksessionmovement`
--

DROP TABLE IF EXISTS `pos_worksessionmovement`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_worksessionmovement` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `MovementAmount` double DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `UserDetail` char(38) DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `DocumentFinanceMaster` char(38) DEFAULT NULL,
  `DocumentFinancePayment` char(38) DEFAULT NULL,
  `DocumentFinanceType` char(38) DEFAULT NULL,
  `PaymentMethod` char(38) DEFAULT NULL,
  `WorkSessionPeriod` char(38) DEFAULT NULL,
  `WorkSessionMovementType` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_WorkSessionMovement` (`Oid`),
  KEY `iCreatedBy_POS_WorkSessionMovement` (`CreatedBy`),
  KEY `iCreatedWhere_POS_WorkSessionMovement` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_WorkSessionMovement` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_WorkSessionMovement` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_WorkSessionMovement` (`DeletedBy`),
  KEY `iDeletedWhere_POS_WorkSessionMovement` (`DeletedWhere`),
  KEY `iUserDetail_POS_WorkSessionMovement` (`UserDetail`),
  KEY `iTerminal_POS_WorkSessionMovement` (`Terminal`),
  KEY `iDocumentFinanceMaster_POS_WorkSessionMovement` (`DocumentFinanceMaster`),
  KEY `iDocumentFinancePayment_POS_WorkSessionMovement` (`DocumentFinancePayment`),
  KEY `iDocumentFinanceType_POS_WorkSessionMovement` (`DocumentFinanceType`),
  KEY `iPaymentMethod_POS_WorkSessionMovement` (`PaymentMethod`),
  KEY `iWorkSessionPeriod_POS_WorkSessionMovement` (`WorkSessionPeriod`),
  KEY `iWorkSessionMovementType_POS_WorkSessionMovement` (`WorkSessionMovementType`),
  CONSTRAINT `FK_POS_WorkSessionMovement_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_DocumentFinanceMaster` FOREIGN KEY (`DocumentFinanceMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_DocumentFinancePayment` FOREIGN KEY (`DocumentFinancePayment`) REFERENCES `fin_documentfinancepayment` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_DocumentFinanceType` FOREIGN KEY (`DocumentFinanceType`) REFERENCES `fin_documentfinancetype` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_PaymentMethod` FOREIGN KEY (`PaymentMethod`) REFERENCES `fin_configurationpaymentmethod` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_UserDetail` FOREIGN KEY (`UserDetail`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_WorkSessionMovementType` FOREIGN KEY (`WorkSessionMovementType`) REFERENCES `pos_worksessionmovementtype` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovement_WorkSessionPeriod` FOREIGN KEY (`WorkSessionPeriod`) REFERENCES `pos_worksessionperiod` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_worksessionmovementtype`
--

DROP TABLE IF EXISTS `pos_worksessionmovementtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_worksessionmovementtype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `ButtonIcon` varchar(255) DEFAULT NULL,
  `CashDrawerMovement` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_WorkSessionMovementType` (`Oid`),
  UNIQUE KEY `iCode_POS_WorkSessionMovementType` (`Code`),
  UNIQUE KEY `iToken_POS_WorkSessionMovementType` (`Token`),
  UNIQUE KEY `iDesignation_POS_WorkSessionMovementType` (`Designation`),
  UNIQUE KEY `iResourceString_POS_WorkSessionMovementType` (`ResourceString`),
  KEY `iCreatedBy_POS_WorkSessionMovementType` (`CreatedBy`),
  KEY `iCreatedWhere_POS_WorkSessionMovementType` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_WorkSessionMovementType` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_WorkSessionMovementType` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_WorkSessionMovementType` (`DeletedBy`),
  KEY `iDeletedWhere_POS_WorkSessionMovementType` (`DeletedWhere`),
  CONSTRAINT `FK_POS_WorkSessionMovementType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovementType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovementType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovementType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovementType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionMovementType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_worksessionperiod`
--

DROP TABLE IF EXISTS `pos_worksessionperiod`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_worksessionperiod` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `PeriodType` int(11) DEFAULT NULL,
  `SessionStatus` int(11) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `DateStart` datetime DEFAULT NULL,
  `DateEnd` datetime DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `Parent` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_WorkSessionPeriod` (`Oid`),
  UNIQUE KEY `iDesignation_POS_WorkSessionPeriod` (`Designation`),
  KEY `iCreatedBy_POS_WorkSessionPeriod` (`CreatedBy`),
  KEY `iCreatedWhere_POS_WorkSessionPeriod` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_WorkSessionPeriod` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_WorkSessionPeriod` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_WorkSessionPeriod` (`DeletedBy`),
  KEY `iDeletedWhere_POS_WorkSessionPeriod` (`DeletedWhere`),
  KEY `iTerminal_POS_WorkSessionPeriod` (`Terminal`),
  KEY `iParent_POS_WorkSessionPeriod` (`Parent`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_Parent` FOREIGN KEY (`Parent`) REFERENCES `pos_worksessionperiod` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriod_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `pos_worksessionperiodtotal`
--

DROP TABLE IF EXISTS `pos_worksessionperiodtotal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `pos_worksessionperiodtotal` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `PaymentMethod` char(38) DEFAULT NULL,
  `Total` double DEFAULT NULL,
  `Period` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_POS_WorkSessionPeriodTotal` (`Oid`),
  KEY `iCreatedBy_POS_WorkSessionPeriodTotal` (`CreatedBy`),
  KEY `iCreatedWhere_POS_WorkSessionPeriodTotal` (`CreatedWhere`),
  KEY `iUpdatedBy_POS_WorkSessionPeriodTotal` (`UpdatedBy`),
  KEY `iUpdatedWhere_POS_WorkSessionPeriodTotal` (`UpdatedWhere`),
  KEY `iDeletedBy_POS_WorkSessionPeriodTotal` (`DeletedBy`),
  KEY `iDeletedWhere_POS_WorkSessionPeriodTotal` (`DeletedWhere`),
  KEY `iPaymentMethod_POS_WorkSessionPeriodTotal` (`PaymentMethod`),
  KEY `iPeriod_POS_WorkSessionPeriodTotal` (`Period`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_PaymentMethod` FOREIGN KEY (`PaymentMethod`) REFERENCES `fin_configurationpaymentmethod` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_Period` FOREIGN KEY (`Period`) REFERENCES `pos_worksessionperiod` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_POS_WorkSessionPeriodTotal_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rpt_report`
--

DROP TABLE IF EXISTS `rpt_report`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `rpt_report` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `FileName` varchar(100) DEFAULT NULL,
  `ParameterFields` varchar(100) DEFAULT NULL,
  `AuthorType` int(11) DEFAULT NULL,
  `ReportType` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_RPT_Report` (`Oid`),
  UNIQUE KEY `iCode_RPT_Report` (`Code`),
  UNIQUE KEY `iDesignation_RPT_Report` (`Designation`),
  UNIQUE KEY `iResourceString_RPT_Report` (`ResourceString`),
  UNIQUE KEY `iToken_RPT_Report` (`Token`),
  KEY `iCreatedBy_RPT_Report` (`CreatedBy`),
  KEY `iCreatedWhere_RPT_Report` (`CreatedWhere`),
  KEY `iUpdatedBy_RPT_Report` (`UpdatedBy`),
  KEY `iUpdatedWhere_RPT_Report` (`UpdatedWhere`),
  KEY `iDeletedBy_RPT_Report` (`DeletedBy`),
  KEY `iDeletedWhere_RPT_Report` (`DeletedWhere`),
  KEY `iReportType_RPT_Report` (`ReportType`),
  CONSTRAINT `FK_RPT_Report_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_RPT_Report_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_RPT_Report_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_RPT_Report_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_RPT_Report_ReportType` FOREIGN KEY (`ReportType`) REFERENCES `rpt_reporttype` (`Oid`),
  CONSTRAINT `FK_RPT_Report_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_RPT_Report_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `rpt_reporttype`
--

DROP TABLE IF EXISTS `rpt_reporttype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `rpt_reporttype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `MenuIcon` varchar(255) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_RPT_ReportType` (`Oid`),
  UNIQUE KEY `iDesignation_RPT_ReportType` (`Designation`),
  UNIQUE KEY `iResourceString_RPT_ReportType` (`ResourceString`),
  KEY `iCreatedBy_RPT_ReportType` (`CreatedBy`),
  KEY `iCreatedWhere_RPT_ReportType` (`CreatedWhere`),
  KEY `iUpdatedBy_RPT_ReportType` (`UpdatedBy`),
  KEY `iUpdatedWhere_RPT_ReportType` (`UpdatedWhere`),
  KEY `iDeletedBy_RPT_ReportType` (`DeletedBy`),
  KEY `iDeletedWhere_RPT_ReportType` (`DeletedWhere`),
  CONSTRAINT `FK_RPT_ReportType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_RPT_ReportType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_RPT_ReportType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_RPT_ReportType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_RPT_ReportType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_RPT_ReportType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_configurationinputreader`
--

DROP TABLE IF EXISTS `sys_configurationinputreader`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_configurationinputreader` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ReaderSizes` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_ConfigurationInputReader` (`Oid`),
  UNIQUE KEY `iCode_SYS_ConfigurationInputReader` (`Code`),
  KEY `iCreatedBy_SYS_ConfigurationInputReader` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_ConfigurationInputReader` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_ConfigurationInputReader` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_ConfigurationInputReader` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_ConfigurationInputReader` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_ConfigurationInputReader` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_ConfigurationInputReader_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationInputReader_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationInputReader_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationInputReader_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationInputReader_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationInputReader_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_configurationpoledisplay`
--

DROP TABLE IF EXISTS `sys_configurationpoledisplay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_configurationpoledisplay` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `VID` varchar(100) DEFAULT NULL,
  `PID` varchar(100) DEFAULT NULL,
  `EndPoint` varchar(100) DEFAULT NULL,
  `CodeTable` varchar(100) DEFAULT NULL,
  `DisplayCharactersPerLine` int(10) unsigned DEFAULT NULL,
  `GoToStandByInSeconds` int(10) unsigned DEFAULT NULL,
  `StandByLine1` varchar(100) DEFAULT NULL,
  `StandByLine2` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_ConfigurationPoleDisplay` (`Oid`),
  UNIQUE KEY `iCode_SYS_ConfigurationPoleDisplay` (`Code`),
  KEY `iCreatedBy_SYS_ConfigurationPoleDisplay` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_ConfigurationPoleDisplay` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_ConfigurationPoleDisplay` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_ConfigurationPoleDisplay` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_ConfigurationPoleDisplay` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_ConfigurationPoleDisplay` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_ConfigurationPoleDisplay_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPoleDisplay_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPoleDisplay_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPoleDisplay_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPoleDisplay_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPoleDisplay_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_configurationweighingmachine`
--

DROP TABLE IF EXISTS `sys_configurationweighingmachine`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_configurationweighingmachine` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `PortName` varchar(4) NOT NULL,
  `BaudRate` int(11) NOT NULL,
  `Parity` varchar(5) NOT NULL,
  `StopBits` varchar(12) NOT NULL,
  `DataBits` int(11) NOT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_ConfigurationWeighingMachine` (`Oid`),
  UNIQUE KEY `iCode_SYS_ConfigurationWeighingMachine` (`Code`),
  KEY `iCreatedBy_SYS_ConfigurationWeighingMachine` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_ConfigurationWeighingMachine` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_ConfigurationWeighingMachine` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_ConfigurationWeighingMachine` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_ConfigurationWeighingMachine` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_ConfigurationWeighingMachine` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_ConfigurationWeighingMachine_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationWeighingMachine_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationWeighingMachine_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationWeighingMachine_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationWeighingMachine_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationWeighingMachine_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_configurationprinters`
--

DROP TABLE IF EXISTS `sys_configurationprinters`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_configurationprinters` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `NetworkName` varchar(100) DEFAULT NULL,
  `ThermalEncoding` varchar(100) DEFAULT NULL,
  `ThermalPrintLogo` bit(1) DEFAULT NULL,
  `ThermalImageCompanyLogo` varchar(100) DEFAULT NULL,
  `ThermalMaxCharsPerLineNormal` int(11) DEFAULT NULL,
  `ThermalMaxCharsPerLineNormalBold` int(11) DEFAULT NULL,
  `ThermalMaxCharsPerLineSmall` int(11) DEFAULT NULL,
  `ThermalCutCommand` varchar(100) DEFAULT NULL,
  `ThermalOpenDrawerValueM` int(11) DEFAULT NULL,
  `ThermalOpenDrawerValueT1` int(11) DEFAULT NULL,
  `ThermalOpenDrawerValueT2` int(11) DEFAULT NULL,
  `ShowInDialog` bit(1) DEFAULT NULL,
  `PrinterType` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_ConfigurationPrinters` (`Oid`),
  UNIQUE KEY `iCode_SYS_ConfigurationPrinters` (`Code`),
  KEY `iCreatedBy_SYS_ConfigurationPrinters` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_ConfigurationPrinters` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_ConfigurationPrinters` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_ConfigurationPrinters` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_ConfigurationPrinters` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_ConfigurationPrinters` (`DeletedWhere`),
  KEY `iPrinterType_SYS_ConfigurationPrinters` (`PrinterType`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_PrinterType` FOREIGN KEY (`PrinterType`) REFERENCES `sys_configurationprinterstype` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrinters_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_configurationprinterstemplates`
--

DROP TABLE IF EXISTS `sys_configurationprinterstemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_configurationprinterstemplates` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `FileTemplate` varchar(100) DEFAULT NULL,
  `FinancialTemplate` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_ConfigurationPrintersTemplates` (`Oid`),
  UNIQUE KEY `iDesignation_SYS_ConfigurationPrintersTemplates` (`Designation`),
  KEY `iCreatedBy_SYS_ConfigurationPrintersTemplates` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_ConfigurationPrintersTemplates` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_ConfigurationPrintersTemplates` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_ConfigurationPrintersTemplates` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_ConfigurationPrintersTemplates` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_ConfigurationPrintersTemplates` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersTemplates_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersTemplates_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersTemplates_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersTemplates_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersTemplates_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersTemplates_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_configurationprinterstype`
--

DROP TABLE IF EXISTS `sys_configurationprinterstype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_configurationprinterstype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `ThermalPrinter` bit(1) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_ConfigurationPrintersType` (`Oid`),
  UNIQUE KEY `iCode_SYS_ConfigurationPrintersType` (`Code`),
  UNIQUE KEY `iDesignation_SYS_ConfigurationPrintersType` (`Designation`),
  UNIQUE KEY `iToken_SYS_ConfigurationPrintersType` (`Token`),
  KEY `iCreatedBy_SYS_ConfigurationPrintersType` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_ConfigurationPrintersType` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_ConfigurationPrintersType` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_ConfigurationPrintersType` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_ConfigurationPrintersType` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_ConfigurationPrintersType` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_ConfigurationPrintersType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systemaudit`
--

DROP TABLE IF EXISTS `sys_systemaudit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systemaudit` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `UserDetail` char(38) DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `AuditType` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemAudit` (`Oid`),
  KEY `iCreatedBy_SYS_SystemAudit` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemAudit` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemAudit` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemAudit` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemAudit` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemAudit` (`DeletedWhere`),
  KEY `iUserDetail_SYS_SystemAudit` (`UserDetail`),
  KEY `iTerminal_SYS_SystemAudit` (`Terminal`),
  KEY `iAuditType_SYS_SystemAudit` (`AuditType`),
  CONSTRAINT `FK_SYS_SystemAudit_AuditType` FOREIGN KEY (`AuditType`) REFERENCES `sys_systemaudittype` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAudit_UserDetail` FOREIGN KEY (`UserDetail`) REFERENCES `sys_userdetail` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systemauditat`
--

DROP TABLE IF EXISTS `sys_systemauditat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systemauditat` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `Type` int(11) DEFAULT NULL,
  `PostData` longtext,
  `ReturnCode` int(11) DEFAULT NULL,
  `ReturnMessage` varchar(100) DEFAULT NULL,
  `ReturnRaw` longtext,
  `DocumentNumber` varchar(100) DEFAULT NULL,
  `ATDocCodeID` varchar(100) DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemAuditAT` (`Oid`),
  KEY `iCreatedBy_SYS_SystemAuditAT` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemAuditAT` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemAuditAT` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemAuditAT` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemAuditAT` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemAuditAT` (`DeletedWhere`),
  KEY `iDocumentMaster_SYS_SystemAuditAT` (`DocumentMaster`),
  CONSTRAINT `FK_SYS_SystemAuditAT_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditAT_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditAT_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditAT_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditAT_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditAT_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditAT_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systemaudittype`
--

DROP TABLE IF EXISTS `sys_systemaudittype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systemaudittype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `ResourceString` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemAuditType` (`Oid`),
  UNIQUE KEY `iCode_SYS_SystemAuditType` (`Code`),
  UNIQUE KEY `iToken_SYS_SystemAuditType` (`Token`),
  UNIQUE KEY `iDesignation_SYS_SystemAuditType` (`Designation`),
  UNIQUE KEY `iResourceString_SYS_SystemAuditType` (`ResourceString`),
  KEY `iCreatedBy_SYS_SystemAuditType` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemAuditType` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemAuditType` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemAuditType` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemAuditType` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemAuditType` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_SystemAuditType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemAuditType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systembackup`
--

DROP TABLE IF EXISTS `sys_systembackup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systembackup` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `DataBaseType` int(11) DEFAULT NULL,
  `Version` int(10) unsigned DEFAULT NULL,
  `FileName` varchar(255) DEFAULT NULL,
  `FileNamePacked` varchar(255) DEFAULT NULL,
  `FilePath` varchar(100) DEFAULT NULL,
  `FileHash` varchar(255) DEFAULT NULL,
  `User` char(38) DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemBackup` (`Oid`),
  UNIQUE KEY `iFileName_SYS_SystemBackup` (`FileName`),
  UNIQUE KEY `iFileNamePacked_SYS_SystemBackup` (`FileNamePacked`),
  KEY `iCreatedBy_SYS_SystemBackup` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemBackup` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemBackup` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemBackup` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemBackup` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemBackup` (`DeletedWhere`),
  KEY `iUser_SYS_SystemBackup` (`User`),
  KEY `iTerminal_SYS_SystemBackup` (`Terminal`),
  CONSTRAINT `FK_SYS_SystemBackup_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemBackup_User` FOREIGN KEY (`User`) REFERENCES `sys_userdetail` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systemnotification`
--

DROP TABLE IF EXISTS `sys_systemnotification`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systemnotification` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Message` longtext,
  `Readed` bit(1) DEFAULT NULL,
  `DateRead` datetime DEFAULT NULL,
  `UserTarget` char(38) DEFAULT NULL,
  `TerminalTarget` char(38) DEFAULT NULL,
  `UserLastRead` char(38) DEFAULT NULL,
  `TerminalLastRead` char(38) DEFAULT NULL,
  `NotificationType` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemNotification` (`Oid`),
  KEY `iCreatedBy_SYS_SystemNotification` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemNotification` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemNotification` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemNotification` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemNotification` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemNotification` (`DeletedWhere`),
  KEY `iUserTarget_SYS_SystemNotification` (`UserTarget`),
  KEY `iTerminalTarget_SYS_SystemNotification` (`TerminalTarget`),
  KEY `iUserLastRead_SYS_SystemNotification` (`UserLastRead`),
  KEY `iTerminalLastRead_SYS_SystemNotification` (`TerminalLastRead`),
  KEY `iNotificationType_SYS_SystemNotification` (`NotificationType`),
  CONSTRAINT `FK_SYS_SystemNotification_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_NotificationType` FOREIGN KEY (`NotificationType`) REFERENCES `sys_systemnotificationtype` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_TerminalLastRead` FOREIGN KEY (`TerminalLastRead`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_TerminalTarget` FOREIGN KEY (`TerminalTarget`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_UserLastRead` FOREIGN KEY (`UserLastRead`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotification_UserTarget` FOREIGN KEY (`UserTarget`) REFERENCES `sys_userdetail` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systemnotificationtype`
--

DROP TABLE IF EXISTS `sys_systemnotificationtype`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systemnotificationtype` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `Message` varchar(255) DEFAULT NULL,
  `WarnDaysBefore` int(11) DEFAULT NULL,
  `UserTarget` char(38) DEFAULT NULL,
  `TerminalTarget` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemNotificationType` (`Oid`),
  UNIQUE KEY `iCode_SYS_SystemNotificationType` (`Code`),
  UNIQUE KEY `iDesignation_SYS_SystemNotificationType` (`Designation`),
  UNIQUE KEY `iMessage_SYS_SystemNotificationType` (`Message`),
  KEY `iCreatedBy_SYS_SystemNotificationType` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemNotificationType` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemNotificationType` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemNotificationType` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemNotificationType` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemNotificationType` (`DeletedWhere`),
  KEY `iUserTarget_SYS_SystemNotificationType` (`UserTarget`),
  KEY `iTerminalTarget_SYS_SystemNotificationType` (`TerminalTarget`),
  CONSTRAINT `FK_SYS_SystemNotificationType_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_TerminalTarget` FOREIGN KEY (`TerminalTarget`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemNotificationType_UserTarget` FOREIGN KEY (`UserTarget`) REFERENCES `sys_userdetail` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_systemprint`
--

DROP TABLE IF EXISTS `sys_systemprint`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_systemprint` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Date` datetime DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `CopyNames` varchar(50) DEFAULT NULL,
  `PrintCopies` int(11) DEFAULT NULL,
  `PrintMotive` varchar(255) DEFAULT NULL,
  `SecondPrint` bit(1) DEFAULT NULL,
  `UserDetail` char(38) DEFAULT NULL,
  `Terminal` char(38) DEFAULT NULL,
  `DocumentMaster` char(38) DEFAULT NULL,
  `DocumentPayment` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_SystemPrint` (`Oid`),
  KEY `iCreatedBy_SYS_SystemPrint` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_SystemPrint` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_SystemPrint` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_SystemPrint` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_SystemPrint` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_SystemPrint` (`DeletedWhere`),
  KEY `iUserDetail_SYS_SystemPrint` (`UserDetail`),
  KEY `iTerminal_SYS_SystemPrint` (`Terminal`),
  KEY `iDocumentMaster_SYS_SystemPrint` (`DocumentMaster`),
  KEY `iDocumentPayment_SYS_SystemPrint` (`DocumentPayment`),
  CONSTRAINT `FK_SYS_SystemPrint_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_DocumentMaster` FOREIGN KEY (`DocumentMaster`) REFERENCES `fin_documentfinancemaster` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_DocumentPayment` FOREIGN KEY (`DocumentPayment`) REFERENCES `fin_documentfinancepayment` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_Terminal` FOREIGN KEY (`Terminal`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_SystemPrint_UserDetail` FOREIGN KEY (`UserDetail`) REFERENCES `sys_userdetail` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_userdetail`
--

DROP TABLE IF EXISTS `sys_userdetail`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_userdetail` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `CodeInternal` varchar(30) DEFAULT NULL,
  `Name` varchar(512) DEFAULT NULL,
  `Residence` varchar(512) DEFAULT NULL,
  `Locality` varchar(255) DEFAULT NULL,
  `ZipCode` varchar(100) DEFAULT NULL,
  `City` varchar(255) DEFAULT NULL,
  `DateOfContract` varchar(100) DEFAULT NULL,
  `Phone` varchar(255) DEFAULT NULL,
  `MobilePhone` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `FiscalNumber` varchar(100) DEFAULT NULL,
  `Language` varchar(100) DEFAULT NULL,
  `AssignedSeating` varchar(100) DEFAULT NULL,
  `AccessPin` varchar(255) DEFAULT NULL,
  `AccessCardNumber` varchar(100) DEFAULT NULL,
  `Login` varchar(100) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL,
  `PasswordReset` bit(1) DEFAULT NULL,
  `PasswordResetDate` datetime DEFAULT NULL,
  `BaseConsumption` varchar(100) DEFAULT NULL,
  `BaseOffers` varchar(100) DEFAULT NULL,
  `PVPOffers` varchar(100) DEFAULT NULL,
  `Remarks` varchar(100) DEFAULT NULL,
  `ButtonImage` varchar(255) DEFAULT NULL,
  `Profile` char(38) DEFAULT NULL,
  `CommissionGroup` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_UserDetail` (`Oid`),
  UNIQUE KEY `iCode_SYS_UserDetail` (`Code`),
  UNIQUE KEY `iCodeInternal_SYS_UserDetail` (`CodeInternal`),
  UNIQUE KEY `iAccessPin_SYS_UserDetail` (`AccessPin`),
  KEY `iCreatedBy_SYS_UserDetail` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_UserDetail` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_UserDetail` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_UserDetail` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_UserDetail` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_UserDetail` (`DeletedWhere`),
  KEY `iProfile_SYS_UserDetail` (`Profile`),
  KEY `iCommissionGroup_SYS_UserDetail` (`CommissionGroup`),
  CONSTRAINT `FK_SYS_UserDetail_CommissionGroup` FOREIGN KEY (`CommissionGroup`) REFERENCES `pos_usercommissiongroup` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_Profile` FOREIGN KEY (`Profile`) REFERENCES `sys_userprofile` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserDetail_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_userpermissiongroup`
--

DROP TABLE IF EXISTS `sys_userpermissiongroup`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_userpermissiongroup` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_UserPermissionGroup` (`Oid`),
  UNIQUE KEY `iCode_SYS_UserPermissionGroup` (`Code`),
  UNIQUE KEY `iDesignation_SYS_UserPermissionGroup` (`Designation`),
  KEY `iCreatedBy_SYS_UserPermissionGroup` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_UserPermissionGroup` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_UserPermissionGroup` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_UserPermissionGroup` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_UserPermissionGroup` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_UserPermissionGroup` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_UserPermissionGroup_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionGroup_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionGroup_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionGroup_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionGroup_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionGroup_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_userpermissionitem`
--

DROP TABLE IF EXISTS `sys_userpermissionitem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_userpermissionitem` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Token` varchar(100) DEFAULT NULL,
  `Designation` varchar(200) DEFAULT NULL,
  `PermissionGroup` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_UserPermissionItem` (`Oid`),
  UNIQUE KEY `iCode_SYS_UserPermissionItem` (`Code`),
  UNIQUE KEY `iToken_SYS_UserPermissionItem` (`Token`),
  UNIQUE KEY `iDesignation_SYS_UserPermissionItem` (`Designation`),
  KEY `iCreatedBy_SYS_UserPermissionItem` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_UserPermissionItem` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_UserPermissionItem` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_UserPermissionItem` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_UserPermissionItem` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_UserPermissionItem` (`DeletedWhere`),
  KEY `iPermissionGroup_SYS_UserPermissionItem` (`PermissionGroup`),
  CONSTRAINT `FK_SYS_UserPermissionItem_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionItem_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionItem_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionItem_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionItem_PermissionGroup` FOREIGN KEY (`PermissionGroup`) REFERENCES `sys_userpermissiongroup` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionItem_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionItem_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_userpermissionprofile`
--

DROP TABLE IF EXISTS `sys_userpermissionprofile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_userpermissionprofile` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Granted` bit(1) DEFAULT NULL,
  `UserProfile` char(38) DEFAULT NULL,
  `PermissionItem` char(38) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_UserPermissionProfile` (`Oid`),
  KEY `iCreatedBy_SYS_UserPermissionProfile` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_UserPermissionProfile` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_UserPermissionProfile` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_UserPermissionProfile` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_UserPermissionProfile` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_UserPermissionProfile` (`DeletedWhere`),
  KEY `iUserProfile_SYS_UserPermissionProfile` (`UserProfile`),
  KEY `iPermissionItem_SYS_UserPermissionProfile` (`PermissionItem`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_PermissionItem` FOREIGN KEY (`PermissionItem`) REFERENCES `sys_userpermissionitem` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserPermissionProfile_UserProfile` FOREIGN KEY (`UserProfile`) REFERENCES `sys_userprofile` (`Oid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `sys_userprofile`
--

DROP TABLE IF EXISTS `sys_userprofile`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `sys_userprofile` (
  `Oid` char(38) NOT NULL,
  `Disabled` bit(1) DEFAULT NULL,
  `Notes` longtext,
  `CreatedAt` datetime DEFAULT NULL,
  `CreatedBy` char(38) DEFAULT NULL,
  `CreatedWhere` char(38) DEFAULT NULL,
  `UpdatedAt` datetime DEFAULT NULL,
  `UpdatedBy` char(38) DEFAULT NULL,
  `UpdatedWhere` char(38) DEFAULT NULL,
  `DeletedAt` datetime DEFAULT NULL,
  `DeletedBy` char(38) DEFAULT NULL,
  `DeletedWhere` char(38) DEFAULT NULL,
  `Ord` int(10) unsigned DEFAULT NULL,
  `Code` int(10) unsigned DEFAULT NULL,
  `Designation` varchar(100) DEFAULT NULL,
  `AccessPassword` varchar(50) DEFAULT NULL,
  `OptimisticLockField` int(11) DEFAULT NULL,
  PRIMARY KEY (`Oid`),
  UNIQUE KEY `iOid_SYS_UserProfile` (`Oid`),
  UNIQUE KEY `iCode_SYS_UserProfile` (`Code`),
  UNIQUE KEY `iDesignation_SYS_UserProfile` (`Designation`),
  KEY `iCreatedBy_SYS_UserProfile` (`CreatedBy`),
  KEY `iCreatedWhere_SYS_UserProfile` (`CreatedWhere`),
  KEY `iUpdatedBy_SYS_UserProfile` (`UpdatedBy`),
  KEY `iUpdatedWhere_SYS_UserProfile` (`UpdatedWhere`),
  KEY `iDeletedBy_SYS_UserProfile` (`DeletedBy`),
  KEY `iDeletedWhere_SYS_UserProfile` (`DeletedWhere`),
  CONSTRAINT `FK_SYS_UserProfile_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserProfile_CreatedWhere` FOREIGN KEY (`CreatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserProfile_DeletedBy` FOREIGN KEY (`DeletedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserProfile_DeletedWhere` FOREIGN KEY (`DeletedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`),
  CONSTRAINT `FK_SYS_UserProfile_UpdatedBy` FOREIGN KEY (`UpdatedBy`) REFERENCES `sys_userdetail` (`Oid`),
  CONSTRAINT `FK_SYS_UserProfile_UpdatedWhere` FOREIGN KEY (`UpdatedWhere`) REFERENCES `pos_configurationplaceterminal` (`Oid`)
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