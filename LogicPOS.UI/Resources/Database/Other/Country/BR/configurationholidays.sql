-- Feriados BR;

DELETE FROM cfg_configurationholidays;

INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('e0aa67fd-0e0f-4afb-bcfd-0463a4ebcd2f',  10,  10,    0,  1,  1, 'Confraternização Universal', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('2f153129-5b87-4dc5-b5e3-7519bfcce49d',  20,  20, 2019,  3,  4, 'Segunda-Feira de Carnaval', '(ponto facultativo)', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('61c82cdb-2f70-4c16-aa44-cd711dddfa55',  30,  30, 2019,  3,  5, 'Carnaval', '(ponto facultativo)', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('d048c1f5-4a81-4d76-afb4-4d608194ac4c',  40,  40, 2019,  3,  6, 'Quarta-Feira de Cinzas', '(ponto facultativo até as 14 horas)', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('5ffd5a26-6bae-43f9-983c-88a3f46afb2d',  50,  50, 2019,  4, 19, 'Paixão de Cristo', '(feriado nacional)', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('53ab02e6-fb4c-4e52-8bea-f51247d509b7',  60,  60,    0,  4, 21, 'Tiradentes', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('05b9771d-e6cd-4af2-87df-9f1a59d44a00',  70,  70, 2019,  4, 21, 'Páscoa', '(feriado nacional)', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('0e970796-5d89-42a5-a39c-11cd65dfce97',  80,  80,    0,  5,  1, 'Dia Mundial do Trabalho', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('70099b3c-0b4a-4d79-b53a-4aa7c7571209',  90,  90, 2019,  6, 20, 'Corpus Christi', '(ponto facultativo)', 0);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('e9fb6b83-2e94-4ad0-92e6-9957b2205d07', 100, 100,    0,  9,  7, 'Independência do Brasil', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('356d7b40-7d36-4117-894d-f1eb2da7c54d', 110, 110,    0, 10, 12, 'Nossa Senhora Aparecida', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('f3baeb7a-49d9-44c0-8b31-df6146e1a698', 120, 120,    0, 11,  2, 'Finados', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('9cfac610-6c02-4e35-b30c-c61dfe3943d4', 130, 130,    0, 11, 15, 'Proclamação da República', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('3eb95637-24a6-4ceb-8d9e-65081df6ae28', 140, 140,    0, 12, 24, 'Véspera de Natal', '(ponto facultativo após as 14 horas)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('26480354-665d-46a5-ba7f-df5314b4c383', 150, 150,    0, 12, 25, 'Natal', '(feriado nacional)', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('f2728a39-f81e-4fa2-815a-8b89a149ddfd', 160, 160,    0, 12, 31, 'Véspera de Ano Novo', '(ponto facultativo após as 14 horas)', 1);

UPDATE cfg_configurationholidays SET UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
