UPDATE cfg_configurationcurrency SET ExchangeRate = 1.0000;
-- Exchange Rate based on Google Finance exchange rates: 2019-03-11;
UPDATE cfg_configurationcurrency SET ExchangeRate = 81.3600 WHERE Acronym = 'AOA';
UPDATE cfg_configurationcurrency SET ExchangeRate =  1.0000 WHERE Acronym = 'BRL';
UPDATE cfg_configurationcurrency SET ExchangeRate =  0.2300 WHERE Acronym = 'EUR';
UPDATE cfg_configurationcurrency SET ExchangeRate =  0.2000 WHERE Acronym = 'GBP';
UPDATE cfg_configurationcurrency SET ExchangeRate = 16.2300 WHERE Acronym = 'MZN';
UPDATE cfg_configurationcurrency SET ExchangeRate =  0.2600 WHERE Acronym = 'USD';

UPDATE cfg_configurationcurrency SET UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
