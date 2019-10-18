-- NOTE : Comments must be Terminated by SEPARATOR else we have problems loading Scripts SEPARATOR >>>> ;

-- Old data clean up;
DELETE FROM fin_article;
DELETE FROM fin_articlesubfamily;
DELETE FROM fin_articlefamily;

-- Default values;
INSERT INTO fin_article (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);
INSERT INTO fin_articlefamily (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);
INSERT INTO fin_articlesubfamily (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '-- Selecione --', 1);

-- Inserts for fin_articlefamily;
INSERT INTO fin_articlefamily (Oid, Ord, Code, Designation, ButtonImage, Disabled) VALUES ('60c79f04-418c-42b8-98ac-7dae714f2ced', 60, 60, 'Serviços', 'Assets/Images/Icons/Families/Parking/parking_category_icon_10.png', 0);

-- Inserts for fin_articlesubfamily;																						     
INSERT INTO fin_articlesubfamily (Oid, Ord, Code, Designation, ButtonImage, Family) VALUES ('d0c8169b-a5bc-46cb-b8ff-186b0ba39929', 310, 310, 'Parqueamento', 'Assets/Images/Icons/SubFamilies/Parking/parking_subcategory_icon_10.png', '60c79f04-418c-42b8-98ac-7dae714f2ced');

-- Inserts for fin_article;
INSERT INTO fin_article (Oid, Ord, Code, Designation, ButtonImage, Favorite, Family, SubFamily, UnitMeasure, UnitSize, Class, Type, VATOnTable, VATDirectSelling) VALUES ('f4c6294d-0a57-4f36-951d-87ab2e076ef1', 3190, 3190, 'Bilhete De Parqueamento', 'Assets/Images/Products/Parking/parking_ticket.png', 1, '60c79f04-418c-42b8-98ac-7dae714f2ced', 'd0c8169b-a5bc-46cb-b8ff-186b0ba39929', '4c81aa20-98ec-4497-b740-165cdb5fa395', '18f564aa-7da5-4a1c-9091-8014638b818c', '7622e5d2-2d52-4be9-bb8b-e5efae5ec791', 'edf4841e-e451-4c7b-9bd0-ee02860ba937', 'cee00590-7317-41b8-af46-66560401096b', 'cee00590-7317-41b8-af46-66560401096b');
INSERT INTO fin_article (Oid, Ord, Code, Designation, ButtonImage, Favorite, Family, SubFamily, UnitMeasure, UnitSize, Class, Type, VATOnTable, VATDirectSelling) VALUES ('32829702-33fa-48d5-917c-4c1db8720777', 3200, 3200, 'Cartão De Parqueamento', 'Assets/Images/Products/Parking/parking_card.png', 1, '60c79f04-418c-42b8-98ac-7dae714f2ced', 'd0c8169b-a5bc-46cb-b8ff-186b0ba39929', '4c81aa20-98ec-4497-b740-165cdb5fa395', '18f564aa-7da5-4a1c-9091-8014638b818c', '7622e5d2-2d52-4be9-bb8b-e5efae5ec791', 'edf4841e-e451-4c7b-9bd0-ee02860ba937', 'cee00590-7317-41b8-af46-66560401096b', 'cee00590-7317-41b8-af46-66560401096b');

-- Updates;
UPDATE fin_articlefamily SET CreatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
UPDATE fin_articlesubfamily SET CreatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
UPDATE fin_article SET CreatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';