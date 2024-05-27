-- US Holidays;

DELETE FROM cfg_configurationholidays;

INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('9b7e0366-5ba1-4412-8294-bdedac08441a',  10,  10,    0,  1,  1, 'New Year''s Day', 'New Year''s Day', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('179cbbc1-d51b-4550-894b-da5d66305775',  20,  20, 2019,  1, 21, 'Martin Luther King Jr. Day', 'Birthday of Martin Luther King Jr. It helds on the third Monday of January', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('96772f39-a7ab-40d4-a891-7216934e61a0',  30,  30, 2019,  2, 18, 'President''s Day', 'George Washington''s birthday,  and its holiday is celebrated on the third Monday of February', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('f2c2811a-dea1-46f8-9fed-488b45c1c2d7',  40,  40, 2019,  5, 27, 'Memorial Day', 'It is currently observed every year on the last Monday of May', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('7d805a71-0a74-4ed6-92e4-85eb6e709935',  50,  50,    0,  7,  4, 'Independence Day', 'The Fourth of July', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('6fc02b38-107c-47c2-aaa2-a99df969eefa',  60,  60, 2019,  9,  2, 'Labor Day', 'It is a public holiday celebrated on the first Monday in September', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('cb18f266-5590-48a3-9dee-6dba3a8d4962',  70,  70, 2019, 10, 14, 'Columbus Day', 'Columbus Day is on the second Monday of October', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('c51f03f3-28a5-4e23-8254-18e14e6da2ce',  80,  80,    0, 11, 11, 'Veterans Day', 'Is an official US public holiday observed annually on November 11th', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('2cb59423-2b6e-423b-bacf-11b5974a3dea',  90,  90, 2019, 11, 28, 'Thanksgiving', 'Thanksgiving is celebrated on the on the fourth Thursday of November in the US', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('e25d71ff-7ab5-4f17-94b0-ed28f6d65913', 100, 100,    0, 12, 25, 'Christmas', 'Christmas Day', 1);

UPDATE cfg_configurationholidays SET UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
