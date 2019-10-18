--Deprecated : now its Inside DataLayer in sqlConfigurationPreferenceParameter;
--UPDATE cfg_configurationpreferenceparameter SET Value = NULL WHERE FormPageNo = 1 AND FormType = 1;

UPDATE cfg_configurationpreferenceparameter SET Value = NULL WHERE Token = 'COMPANY_COUNTRY_CODE2';
