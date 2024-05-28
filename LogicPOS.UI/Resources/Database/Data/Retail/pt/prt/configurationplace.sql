DELETE FROM pos_configurationplacetable WHERE Place='99afc739-7828-4fa6-83c9-6e8b71987909' OR Place='08f60493-2823-4279-920d-003ab1696eda' OR Place='0ae5cbc0-73de-4d28-a9b6-f415b21217d3' OR Place='7c0dda6f-54f4-45f0-836a-71cb52aff52b';

UPDATE pos_configurationplacetable SET Ord=60, Code=60, Designation=6 WHERE Oid='ece81d96-52ee-4ec7-ab58-9d1e51b2b037'; -- ('ece81d96-52ee-4ec7-ab58-9d1e51b2b037',160,160,16,'0ae5cbc0-73de-4d28-a9b6-f415b21217d3'); -- Hotel;
UPDATE pos_configurationplacetable SET Ord=70, Code=70, Designation=7 WHERE Oid='5f67eecd-844c-46b4-905c-67164b8ec9f8'; -- ('5f67eecd-844c-46b4-905c-67164b8ec9f8',170,170,17,'0ae5cbc0-73de-4d28-a9b6-f415b21217d3'); -- Hotel;
UPDATE pos_configurationplacetable SET Ord=80, Code=80, Designation=8 WHERE Oid='eacbf751-c639-430c-9871-6e2bd5f16394'; -- ('eacbf751-c639-430c-9871-6e2bd5f16394',180,180,18,'0ae5cbc0-73de-4d28-a9b6-f415b21217d3'); -- Hotel;
UPDATE pos_configurationplacetable SET Ord=90, Code=90, Designation=9 WHERE Oid='9520fcd4-b6f3-4597-ac26-eb004e2f7b17'; -- ('9520fcd4-b6f3-4597-ac26-eb004e2f7b17',190,190,19,'0ae5cbc0-73de-4d28-a9b6-f415b21217d3'); -- Hotel;
UPDATE pos_configurationplacetable SET Ord=100, Code=100, Designation=10 WHERE Oid='1be5366a-e611-4622-b6a2-424946df960f'; -- ('1be5366a-e611-4622-b6a2-424946df960f',200,200,20,'0ae5cbc0-73de-4d28-a9b6-f415b21217d3'); -- Hotel;
UPDATE pos_configurationplacetable SET Place = '5d1f314a-9f86-4cb8-95b7-73149a1b2ab9';

DELETE FROM pos_configurationplace WHERE Oid='dd5a3869-db52-42d4-bbed-dec4adfaf62b'; -- Bar Counter;
DELETE FROM pos_configurationplace WHERE Oid='99afc739-7828-4fa6-83c9-6e8b71987909'; -- Dinning Room;
DELETE FROM pos_configurationplace WHERE Oid='08f60493-2823-4279-920d-003ab1696eda'; -- Patio;
DELETE FROM pos_configurationplace WHERE Oid='0ae5cbc0-73de-4d28-a9b6-f415b21217d3'; -- Hotel;
DELETE FROM pos_configurationplace WHERE Oid='7c0dda6f-54f4-45f0-836a-71cb52aff52b'; -- Disco;
UPDATE pos_configurationplace SET Ord=10, Code=10, Designation='Normal',MovementType='378cef43-82a0-4c2e-a157-4907e52806ef' WHERE Oid='5d1f314a-9f86-4cb8-95b7-73149a1b2ab9';

DELETE FROM pos_configurationplacemovementtype WHERE Oid='9e3d68b5-5aae-459b-969d-dd039b33e8cd'; -- Staff Consumption;
DELETE FROM pos_configurationplacemovementtype WHERE Oid='88e92456-cea4-4a32-ad43-9c3aafb3033b'; -- Take-Out;
DELETE FROM pos_configurationplacemovementtype WHERE Oid='ab506a3c-4aec-4427-9b71-ce9f0269ce7c'; -- Delivery;
UPDATE pos_configurationplacemovementtype SET VatDirectSelling=1 WHERE Oid='378cef43-82a0-4c2e-a157-4907e52806ef'; -- Regular;
