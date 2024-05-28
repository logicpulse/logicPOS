-- NOTE : Comments must be Terminated by SEPARATOR else we have problems loading Scripts SEPARATOR >>>> ;

-- Old data clean up;
DELETE FROM fin_article;
DELETE FROM fin_articlesubfamily;
DELETE FROM fin_articlefamily;

-- Undefined values;
INSERT INTO fin_article (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '--- Undefined ---', 1);
INSERT INTO fin_articlefamily (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '--- Undefined ---', 1);
INSERT INTO fin_articlesubfamily (Oid, Designation, Disabled) VALUES ('00000000-0000-0000-0000-000000000001', '--- Undefined ---', 1);

-- Inserts for fin_articlefamily;
INSERT INTO fin_articlefamily (Oid, Ord, Code, ButtonImage, Designation, Disabled) VALUES ('60C79F04-418C-42B8-98AC-7DAE714F2CED', 60, 60, 'Assets/Images/Icons/Families/Parking/parking_category_icon_10.jpg', 'Serviços', 0);

-- Inserts for fin_articlesubfamily;																						     
INSERT INTO fin_articlesubfamily (Oid, Ord, Code, ButtonImage, Designation, Family) VALUES ('D0C8169B-A5BC-46CB-B8FF-186B0BA39929', 310, 310, 'Assets/Images/Icons/SubFamilies/Parking/parking_subcategory_icon_10.jpg', 'Parqueamento', '60C79F04-418C-42B8-98AC-7DAE714F2CED');

-- Inserts for fin_article;
INSERT INTO fin_article (Oid, Ord, Code, ButtonImage, Designation, Favorite, Family, SubFamily, UnitMeasure, UnitSize, Class, Type, VATOnTable, VATDirectSelling) VALUES ('F4C6294D-0A57-4F36-951D-87AB2E076EF1', 3190, 3190, 'Assets/Images/Products/Parking/noimage.jpg', 'Bilhete de Parqueamento', 1, '60C79F04-418C-42B8-98AC-7DAE714F2CED', 'D0C8169B-A5BC-46CB-B8FF-186B0BA39929', '4C81AA20-98EC-4497-B740-165CDB5FA395', '18F564AA-7DA5-4A1C-9091-8014638B818C', '7622E5D2-2D52-4BE9-BB8B-E5EFAE5EC791', 'EDF4841E-E451-4C7B-9BD0-EE02860BA937', 'CEE00590-7317-41B8-AF46-66560401096B', 'CEE00590-7317-41B8-AF46-66560401096B');
INSERT INTO fin_article (Oid, Ord, Code, ButtonImage, Designation, Favorite, Family, SubFamily, UnitMeasure, UnitSize, Class, Type, VATOnTable, VATDirectSelling) VALUES ('32829702-33FA-48D5-917C-4C1DB8720777', 3200, 3200, 'Assets/Images/Products/Parking/noimage.jpg', 'Cartão de Parqueamento', 1, '60C79F04-418C-42B8-98AC-7DAE714F2CED', 'D0C8169B-A5BC-46CB-B8FF-186B0BA39929', '4C81AA20-98EC-4497-B740-165CDB5FA395', '18F564AA-7DA5-4A1C-9091-8014638B818C', '7622E5D2-2D52-4BE9-BB8B-E5EFAE5EC791', 'EDF4841E-E451-4C7B-9BD0-EE02860BA937', 'CEE00590-7317-41B8-AF46-66560401096B', 'CEE00590-7317-41B8-AF46-66560401096B');

-- Updates;
UPDATE fin_articlefamily SET CreatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
UPDATE fin_articlesubfamily SET CreatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
UPDATE fin_article SET CreatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';