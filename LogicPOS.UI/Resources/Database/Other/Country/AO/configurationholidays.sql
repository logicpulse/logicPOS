-- Feriados AO;

DELETE FROM cfg_configurationholidays;

INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('e0aa67fd-0e0f-4afb-bcfd-0463a4ebcd2f',  10,  10, 0,  1,  1, 'Ano Novo', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('2f153129-5b87-4dc5-b5e3-7519bfcce49d',  20,  20, 0,  1,  4, 'Dia dos Mártires da Repressão Colonial', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('61c82cdb-2f70-4c16-aa44-cd711dddfa55',  30,  30, 0,  1, 25, 'Dia da Cidade de Luanda', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('d048c1f5-4a81-4d76-afb4-4d608194ac4c',  40,  40, 0,  2,  4, 'Dia Nacional do Esforço Armado', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('5ffd5a26-6bae-43f9-983c-88a3f46afb2d',  50,  50, 0,  3,  8, 'Dia Internacional da Mulher', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('53ab02e6-fb4c-4e52-8bea-f51247d509b7',  60,  60, 0,  4,  4, 'Dia da Paz', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('0e970796-5d89-42a5-a39c-11cd65dfce97',  70,  70, 0,  5,  1, 'Dia do Trabalhador', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('70099b3c-0b4a-4d79-b53a-4aa7c7571209',  80,  80, 0,  5, 25, 'Dia de África', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('e9fb6b83-2e94-4ad0-92e6-9957b2205d07',  90,  90, 0,  6,  1, 'Dia Internacional da Criança', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('356d7b40-7d36-4117-894d-f1eb2da7c54d', 100, 100, 0,  9, 17, 'Fundador da Nação e Dia dos Heróis Nacionais', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('f3baeb7a-49d9-44c0-8b31-df6146e1a698', 110, 110, 0, 11,  2, 'Dia de Finados', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('9cfac610-6c02-4e35-b30c-c61dfe3943d4', 120, 120, 0, 11, 11, 'Dia da Independência', '', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('3eb95637-24a6-4ceb-8d9e-65081df6ae28', 130, 130, 0, 12, 25, 'Natal', 'Noite da Consoada', 1);
INSERT INTO cfg_configurationholidays (Oid, Ord, Code, Year, Month, Day, Designation, Description, Fixed) VALUES ('f2728a39-f81e-4fa2-815a-8b89a149ddfd', 140, 140, 0, 12, 31, 'Último dia do ano', '', 1);

UPDATE cfg_configurationholidays SET UpdatedBy = '090c5684-52ba-4d7a-8bc3-a00320ef503d', CreatedAt = '2014-02-28 14:02:28', UpdatedAt = '2019-03-11 23:16:18';
