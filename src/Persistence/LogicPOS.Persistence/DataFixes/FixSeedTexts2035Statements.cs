﻿using System;
using System.Collections.Generic;

namespace LogicPOS.Persistence.DataFixes;

/// <summary>
/// Conditional SQL used by the FixSeedTexts2035 EF migrations (SQLite / MySQL / SQL Server / PostgreSQL).
/// Designation UPDATEs match seed Id and the exact previous value (binary / case-sensitive).
/// </summary>
public static class FixSeedTexts2035Statements
{
    public static readonly (Guid Id, string Old, string New)[] ArticleDesignations =
    [
        (Guid.Parse("826536e1-8e66-4c54-a91d-4c7c7a832de9"), "Agua tonica schweppes", "Agua Tonica Schweppes"),
        (Guid.Parse("c2199878-63fb-48ba-bbb3-5e81ea4c9b8a"), "Aguardente aldeia velha", "Aguardente Aldeia Velha"),
        (Guid.Parse("d1198983-9d22-4091-a8ba-0dd3d686672e"), "Aliança particular", "Aliança Particular"),
        (Guid.Parse("de61ec41-992b-4a4a-9e2b-12c385a0b5ba"), "Alvarinho couto de frades", "Alvarinho Couto de Frades"),
        (Guid.Parse("ee55f865-f706-441f-bee6-a2e6285f62ae"), "Alvarinho de moção", "Alvarinho de Moção"),
        (Guid.Parse("f2b4d141-2129-4e2e-ae89-9936b9f912d7"), "Amêndoa amarga", "Amêndoa Amarga"),
        (Guid.Parse("0b7b91ad-defa-4497-9baa-415f9b69c677"), "Anis doce domuz", "Anis Doce Domuz"),
        (Guid.Parse("1d366402-0acc-49f1-ac74-4bf59d19c971"), "Aplauso bical", "Aplauso Bical"),
        (Guid.Parse("929aa226-b7cc-468e-8923-7099f9f30606"), "Arroz de Lulas", "Arroz de lulas"),
        (Guid.Parse("31bda5be-b38b-461f-b21e-295d081ba076"), "Arroz de Marisco", "Arroz de marisco"),
        (Guid.Parse("1a58af75-5a76-4ef9-bbed-90fe0a80e941"), "Arroz de Polvo", "Arroz de polvo"),
        (Guid.Parse("da2bf54d-d231-482b-bc23-da74ad156d79"), "Arroz de Tamboril", "Arroz de tamboril"),
        (Guid.Parse("93cded1f-a383-4584-bc2e-950fbb59481e"), "Asti gancia doce", "Asti Gancia Doce"),
        (Guid.Parse("8369619a-c9f6-44f9-aae8-91abf2df8ffe"), "Ataíde semedo cuvée bruto 2011", "Ataíde Semedo Cuvée Bruto 2011"),
        (Guid.Parse("9fc538d9-7d2a-4017-a015-30b47d449a8a"), "Baba de Camelo", "Baba de camelo"),
        (Guid.Parse("212cadbd-7708-43fb-9152-612eb190a8a3"), "Bacalhau Assado Na Brasa", "Bacalhau assado na brasa"),
        (Guid.Parse("a04c4e4b-e6eb-462a-9b5a-e11f0ccbae15"), "Bacalhau Na Cataplana", "Bacalhau na cataplana"),
        (Guid.Parse("c45cba9b-716a-4275-b9a5-29a0f6fb896f"), "Bacalhau Recheado", "Bacalhau recheado"),
        (Guid.Parse("4a249eca-549b-4381-bbdc-860a19afb473"), "Bacalhau À Brás", "Bacalhau à Brás"),
        (Guid.Parse("4a249eca-549b-4381-bbdc-860a19afb473"), "Bacalhau à brás", "Bacalhau à Brás"),
        (Guid.Parse("af9e705e-a572-460c-bcb6-60b25b33f0b8"), "Bacalhau À Casa", "Bacalhau à casa"),
        (Guid.Parse("af9e705e-a572-460c-bcb6-60b25b33f0b8"), "Bacalhau à Casa", "Bacalhau à casa"),
        (Guid.Parse("9c7a9bd2-3886-486d-8355-d79a33d44de0"), "Bacalhau À Gomes Sá", "Bacalhau à Gomes Sá"),
        (Guid.Parse("9c7a9bd2-3886-486d-8355-d79a33d44de0"), "Bacalhau à gomes sá", "Bacalhau à Gomes Sá"),
        (Guid.Parse("c5a0a6f7-d506-4312-a1b4-16999ae72915"), "Bacalhau À Lagareiro", "Bacalhau à lagareiro"),
        (Guid.Parse("c5a0a6f7-d506-4312-a1b4-16999ae72915"), "Bacalhau à Lagareiro", "Bacalhau à lagareiro"),
        (Guid.Parse("804963f6-772b-4ef6-bef5-ca0e10e1b7e7"), "Bacardi pina colada", "Bacardi Pina Colada"),
        (Guid.Parse("970276fe-b377-4646-a522-74cbda27912c"), "Bacon Grelhado", "Bacon grelhado"),
        (Guid.Parse("90b1d603-07df-4b95-a6b2-095cbce0b5b2"), "Ballantines 12 anos", "Ballantines 12 Anos"),
        (Guid.Parse("fbb83ae7-a8ce-4e78-979c-a06090181566"), "Batata Cozida", "Batata cozida"),
        (Guid.Parse("0bd6022b-28ce-4cfd-a809-8245f2e7020f"), "Batata Frita", "Batata frita"),
        (Guid.Parse("5cdc8661-9701-425e-8fa6-1a06233f8f70"), "Bifana no Pão", "Bifana no pão"),
        (Guid.Parse("de501923-2b66-4ca5-81e3-8a798f1c5aca"), "Bife Cortador", "Bife cortador"),
        (Guid.Parse("662db2ae-2d97-432d-912f-7a7b48f4b3e3"), "Bife de Pimenta", "Bife de pimenta"),
        (Guid.Parse("eacbf6cf-e3d4-456e-98da-38d23ade0ea6"), "Bife Na Pedra", "Bife na pedra"),
        (Guid.Parse("b44fc3b5-bc4f-4755-b5ef-c7d86366b633"), "Bitoque de Porco", "Bitoque de porco"),
        (Guid.Parse("9d7d65a5-f7c8-415d-a1f6-4445ea417750"), "Bitoque de Vaca", "Bitoque de vaca"),
        (Guid.Parse("dded3b26-5ca1-4379-886a-3991b748b4d3"), "Bola de berlim", "Bola de Berlim"),
        (Guid.Parse("d98b9b71-2cee-42ab-902c-d1e751755f14"), "Bolo de Arroz", "Bolo de arroz"),
        (Guid.Parse("c1ad389f-f607-43d2-aab7-e0529d9a13e9"), "Bolo de Bolacha", "Bolo de bolacha"),
        (Guid.Parse("7576c8fb-981f-463a-bc81-12a07eba1979"), "Borlido meio seco", "Borlido Meio Seco"),
        (Guid.Parse("a3223bd5-8f01-4f07-bef2-3d4e5a7e4e3a"), "Brandy croft", "Brandy Croft"),
        (Guid.Parse("a109edee-9529-44e8-b091-368ccae614c1"), "Brandy macieira", "Brandy Macieira"),
        (Guid.Parse("1586c128-814f-4bea-9d5e-52346102277b"), "Cachaca milbar", "Cachaca Milbar"),
        (Guid.Parse("48803622-3376-44df-9bbf-708c809c788d"), "Cachorro Completo", "Cachorro completo"),
        (Guid.Parse("4e235be6-47a5-4558-b0d5-81168a7c1ef5"), "Cachorro Simples", "Cachorro simples"),
        (Guid.Parse("87a9cf78-d31e-43fc-9a3c-72f152041dd7"), "Caldo Verde", "Caldo verde"),
        (Guid.Parse("142dfaba-99bc-4630-8310-9cd08ce53bc7"), "Camarão C Molho Vinagrete", "Camarão com molho vinagrete"),
        (Guid.Parse("07532292-468c-4dab-a4f4-30951f02186f"), "Campolargo rosé", "Campolargo Rosé"),
        (Guid.Parse("2386eb83-89fe-428f-b9c7-7e48cd3fd06b"), "Carapau Grelhado", "Carapau grelhado"),
        (Guid.Parse("8a51def3-bcc2-47b7-bf8b-1ad705fb05e6"), "Carioca de Café", "Carioca de café"),
        (Guid.Parse("78ddc3ba-58b4-44f7-aae5-9772e62d65b5"), "Carioca de Limão", "Carioca de limão"),
        (Guid.Parse("a3e52eab-8d05-488d-993f-4f56073f6edc"), "Carne Porco Alentejana", "Carne porco alentejana"),
        (Guid.Parse("0217cfa5-1503-41ce-a24a-8ca4fbe8db7f"), "Carregar Cartão de Cliente", "Carregar cartão de cliente"),
        (Guid.Parse("c3176d15-14da-4cd6-9eac-78344512d745"), "Casal garcia", "Casal Garcia"),
        (Guid.Parse("1575b3cd-9b0b-48d8-b20f-3899d34b984b"), "Castelar baga", "Castelar Baga"),
        (Guid.Parse("d76de305-41fb-45ae-bfd7-9d07cab954af"), "Castelo dalba", "Castelo Dalba"),
        (Guid.Parse("6fe883ad-2a5f-42fa-a98c-c5e6da9ce300"), "Caves são joão reserva", "Caves São João Reserva"),
        (Guid.Parse("9a38f1a3-1fe3-4fff-85d3-e20afaeaa102"), "Cerveja de Pressão 33cl", "Cerveja de pressão 33cl"),
        (Guid.Parse("866dfb92-cd64-4051-a8ac-7731fdc37f3a"), "Cerveja de Pressão 50cl", "Cerveja de pressão 50cl"),
        (Guid.Parse("d4759e25-7b1c-472c-9ca5-ec479afa759f"), "Chocolate Quente", "Chocolate quente"),
        (Guid.Parse("2a2e5a62-22ac-4e1f-9545-0f1acb54e4c7"), "Chá Branco", "Chá branco"),
        (Guid.Parse("0aa1da2b-9ce1-47fe-98e7-66d913cc3689"), "Chá Preto", "Chá preto"),
        (Guid.Parse("2c1ca0c9-5721-4f95-a0fb-1584304fe065"), "Chá Tisana", "Chá tisana"),
        (Guid.Parse("3a86d7c0-f508-4ebf-9721-df94ca578fde"), "Chá Verde", "Chá verde"),
        (Guid.Parse("e4ddfa02-1cd3-46a7-8e54-04418425a17a"), "Chá Vermelho", "Chá vermelho"),
        (Guid.Parse("8284b17d-7c0b-4b35-b6b5-a4b8e1e164fd"), "Cocktail de Camarão", "Cocktail de camarão"),
        (Guid.Parse("66ef0cfc-6f30-41f8-9d2a-ed26535178ed"), "Conde de villar", "Conde de Villar"),
        (Guid.Parse("8f368c15-16cf-4df7-a3ba-b84d65613000"), "Constantino brandy", "Constantino Brandy"),
        (Guid.Parse("2db1b49b-a1f7-4338-b13f-cbeaf69035ab"), "Contos Da Terra", "Contos da Terra"),
        (Guid.Parse("2db1b49b-a1f7-4338-b13f-cbeaf69035ab"), "Contos da terra", "Contos da Terra"),
        (Guid.Parse("50c60450-3245-45e9-8f2a-d9919f35e969"), "Copo de Leite", "Copo de leite"),
        (Guid.Parse("e77bc7d4-bdeb-46a7-9468-0b7581b96c36"), "Costeleta de Novilho Grelhada", "Costeleta de novilho grelhada"),
        (Guid.Parse("e1e48020-af12-4a68-9049-e7f1a91de968"), "Costeleta de Porco Grelhada", "Costeleta de porco grelhada"),
        (Guid.Parse("4d6ad77b-4f39-42ad-8c0a-6a38e89d2162"), "Croissant de Chocolate", "Croissant de chocolate"),
        (Guid.Parse("8693a78f-3834-4719-a9cb-229326831096"), "Croissant Misto", "Croissant misto"),
        (Guid.Parse("05428f1c-cbbc-4084-8b84-dadc0b653f39"), "Croissant Simples", "Croissant simples"),
        (Guid.Parse("b96c3a94-4226-4ff6-90a5-14b6860a2c63"), "Croissants de Galinha", "Croissants de galinha"),
        (Guid.Parse("aa2c009c-d242-4eed-8bba-e255d3141a76"), "Croissants de Ovos", "Croissants de ovos"),
        (Guid.Parse("f2825591-776a-4c84-84ed-5768ba96cc90"), "Cuca mini", "Cuca Mini"),
        (Guid.Parse("1657a1e3-73af-4a22-865d-0096273c3f0e"), "Cutty sark", "Cutty Sark"),
        (Guid.Parse("4e7f0f30-c0ae-4727-b8df-88f7f4a2d3ad"), "Doce Da Casa", "Doce da casa"),
        (Guid.Parse("fb0bda9f-bb50-4a25-a70f-b9c67347cb5c"), "Dona maria", "Dona Maria"),
        (Guid.Parse("34fae12b-a3ac-46dc-a977-016c6cc539ce"), "Duas quintas", "Duas Quintas"),
        (Guid.Parse("5f171104-d515-434e-920a-4a1934876a65"), "Duas quintas branco", "Duas Quintas Branco"),
        (Guid.Parse("93a8b723-9dbb-4c22-9d2e-956a7b202622"), "Eclair de Chocolate", "Eclair de chocolate"),
        (Guid.Parse("af9adeb5-3116-44c3-bd2f-938857e4d832"), "Ementa Turistica", "Ementa turistica"),
        (Guid.Parse("00a2ff1c-b828-4fdd-bcc8-98e1425eeaf5"), "Encontro bruto 2010", "Encontro Bruto 2010"),
        (Guid.Parse("43c3f6fd-42ad-45ca-944a-4a97433fd055"), "Entrecosto Porco Grelhado", "Entrecosto porco grelhado"),
        (Guid.Parse("7b8913e0-11fe-47f6-8c50-5bf7b07249c3"), "Escalopes de Peru", "Escalopes de peru"),
        (Guid.Parse("bfdae5d6-94a3-4cb2-bf5a-8e404a74ad86"), "Espetada de Tamboril C Camarão", "Espetada de tamboril com camarão"),
        (Guid.Parse("1c165df6-25cb-435a-988f-af6bb160fba9"), "Espetada Mista", "Espetada mista"),
        (Guid.Parse("2e43763b-7a23-4758-afaf-6d00d4b49d8e"), "Feijoada de Marisco", "Feijoada de marisco"),
        (Guid.Parse("21cddc71-bd67-47fe-8590-72b5c4596446"), "Frango de Caril", "Frango de caril"),
        (Guid.Parse("340d5e4f-34eb-45c8-891b-7542c21c1a4d"), "Frango de Churrasco", "Frango de churrasco"),
        (Guid.Parse("cb12767e-6ac0-4e9f-9969-ff4117c4a497"), "Frango no Forno", "Frango no forno"),
        (Guid.Parse("07933390-424c-4abe-83ef-cc3b77cd4a09"), "Frei joão", "Frei João"),
        (Guid.Parse("4f8d95de-1413-40fc-80dc-008e035452fa"), "Freixenet bruto", "Freixenet Bruto"),
        (Guid.Parse("b210f082-caec-4e14-abe8-ecd3e9184b0a"), "Frize sabores", "Frize Sabores"),
        (Guid.Parse("5865cb65-23e9-4e37-af60-c260f0111f19"), "Galão Cevada", "Galão cevada"),
        (Guid.Parse("62ab6b17-e225-4b53-8427-777a42c843ee"), "Galão Descafeinado", "Galão descafeinado"),
        (Guid.Parse("21e154c1-64a2-4812-b987-37e6f331fbdc"), "Galão Normal", "Galão normal"),
        (Guid.Parse("d72caba7-65e3-43bf-97a7-12f0e4d5fe54"), "Gelado Baunilha", "Gelado baunilha"),
        (Guid.Parse("8e8f69d8-bff8-4939-9e1a-7d40ab625f98"), "Gelado Chocolate", "Gelado chocolate"),
        (Guid.Parse("03673673-1c9a-47e5-8dbd-4a485d1e4809"), "Gelado Limão", "Gelado limão"),
        (Guid.Parse("0f2f9c4c-cb79-4e45-8e75-389498b16846"), "Gelado Morango", "Gelado morango"),
        (Guid.Parse("d9f24485-e60e-489a-bde8-2aaae0c72168"), "Gelado Natas", "Gelado natas"),
        (Guid.Parse("2f1436ba-90be-45a7-b5d8-731d55e69ee5"), "Gin N1 the london", "Gin N1 The London"),
        (Guid.Parse("b2bbfb27-a181-427d-99e2-f1ba000d992a"), "Grelhada Mista", "Grelhada mista"),
        (Guid.Parse("29dbad6a-d707-400c-ad27-fbbb4a1b3625"), "Grão vasco", "Grão Vasco"),
        (Guid.Parse("3d8427b8-2b36-4b72-8d81-76883b01d5ae"), "Hambúrguer Completo", "Hambúrguer completo"),
        (Guid.Parse("a79e3038-db10-4f6d-9b9e-ce5c4da5f58c"), "Hambúrguer Simples", "Hambúrguer simples"),
        (Guid.Parse("930a9d9d-876c-4c5c-b7b6-d866eb8bfa70"), "Herdade Da Comporta", "Herdade da Comporta"),
        (Guid.Parse("930a9d9d-876c-4c5c-b7b6-d866eb8bfa70"), "Herdade da comporta", "Herdade da Comporta"),
        (Guid.Parse("90ce4de1-ed19-408a-ba26-1043e2b39985"), "Herdade Do Sobroso", "Herdade do Sobroso"),
        (Guid.Parse("90ce4de1-ed19-408a-ba26-1043e2b39985"), "Herdade do sobroso", "Herdade do Sobroso"),
        (Guid.Parse("395fe6f8-72d7-4b89-8a0e-353a38fa4912"), "Jack daniels old n 7", "Jack Daniels Old N 7"),
        (Guid.Parse("54754e60-a77b-4987-a43d-694efc06e46a"), "Lambrusco rosé", "Lambrusco Rosé"),
        (Guid.Parse("ddc49d2d-b9a7-4edd-b167-e255f26c3d69"), "Lambrusco Tinto", "Lambrusco tinto"),
        (Guid.Parse("6d0d21eb-d95a-48f9-87c3-36ab43756cd1"), "Lapa Dos Gaviões", "Lapa dos Gaviões"),
        (Guid.Parse("6d0d21eb-d95a-48f9-87c3-36ab43756cd1"), "Lapa dos gaviões", "Lapa dos Gaviões"),
        (Guid.Parse("a8679965-43e1-4817-8026-56d810f23118"), "Legumes Salteados", "Legumes salteados"),
        (Guid.Parse("1beceefc-649e-442d-a2f7-f922fd40e6f2"), "Leite Creme", "Leite creme"),
        (Guid.Parse("041266de-4966-455c-9af2-6a02352ce8a9"), "Licor baileys", "Licor Baileys"),
        (Guid.Parse("0ffcae14-fed2-4e97-815b-b7a6741f0b70"), "Licor malibu", "Licor Malibu"),
        (Guid.Parse("09ffd315-979e-4273-820d-af4cb908af41"), "Licor sheridans", "Licor Sheridans"),
        (Guid.Parse("1bb89940-5e01-4071-8723-16f0613663f8"), "Lombinhos C Cogumelos", "Lombinhos com cogumelos"),
        (Guid.Parse("23126bf7-d541-43d1-9a37-f5c9481fe2b5"), "Lulas Recheadas", "Lulas recheadas"),
        (Guid.Parse("eed3ac7e-2641-4a8f-a3d2-55c995252fce"), "Luso sabores", "Luso Sabores"),
        (Guid.Parse("8f5f8277-d9b5-4017-bc54-f5aff1893ca4"), "Maria mansa", "Maria Mansa"),
        (Guid.Parse("c7166b1c-2e6a-43e9-89d1-8fa30db05ac1"), "Marquês de marialva bruto reserva", "Marquês de Marialva Bruto Reserva"),
        (Guid.Parse("091b34cd-a14c-4814-92f3-ddd291681979"), "Martini bianco", "Martini Bianco"),
        (Guid.Parse("3c6e589f-e339-40f8-89ce-7fc84acea2a4"), "Martini rosso", "Martini Rosso"),
        (Guid.Parse("0d88b0b1-d0b9-48dc-b303-3a886ba22d10"), "Massada de Marisco", "Massada de marisco"),
        (Guid.Parse("a5cf312f-a717-4017-b013-e35ce56a8f7c"), "Mata fidalga rosé", "Mata Fidalga Rosé"),
        (Guid.Parse("0c652176-f8e4-4987-adbf-8e1b87b81ec4"), "Mateus rosé", "Mateus Rosé"),
        (Guid.Parse("ea2d8eba-2c3a-42ea-ab4c-f0fa69a40310"), "Maçã Assada", "Maçã assada"),
        (Guid.Parse("eb57e2fc-2e60-4373-b64a-221d99ae26bf"), "Meia de Leite", "Meia de leite"),
        (Guid.Parse("5656a622-7682-4d47-97cb-ae392d3030a9"), "Meia de Leite Descafeinado", "Meia de leite descafeinado"),
        (Guid.Parse("1c39d3e9-4cd7-41c0-801c-6319f9c942ea"), "Melão com Presunto", "Melão com presunto"),
        (Guid.Parse("0c7ebd05-76c1-4a34-baec-ad41bcbd0787"), "Menu de Carne", "Menu de carne"),
        (Guid.Parse("d495ca1c-824a-4db0-b847-43e67d784f4f"), "Menu de Dieta", "Menu de dieta"),
        (Guid.Parse("afd97cb4-9c22-4275-8bca-9415d9f06d21"), "Menu de Peixe", "Menu de peixe"),
        (Guid.Parse("e52515bf-2dc2-4eb7-99d3-d3d5f56c0c14"), "Menu Vegetariano", "Menu vegetariano"),
        (Guid.Parse("79e95e0f-6123-4489-b14b-09f5afd666de"), "Mil Folhas", "Mil folhas"),
        (Guid.Parse("70c88548-4d17-405d-8e9f-6b3f0cbcf163"), "Monsaraz branco", "Monsaraz Branco"),
        (Guid.Parse("456d5577-f521-448a-b29c-6b652090a25a"), "Montanha chardonnay", "Montanha Chardonnay"),
        (Guid.Parse("bcfe9014-de33-484f-8254-2b1c937dd05a"), "Montanha real", "Montanha Real"),
        (Guid.Parse("8441351c-702f-4bd1-9a61-ef39b25cbc00"), "Monte velho", "Monte Velho"),
        (Guid.Parse("815e4066-d28f-4113-8231-5940afbe5aa3"), "Monte velho branco", "Monte Velho Branco"),
        (Guid.Parse("fdcf43e3-79d9-4de7-b9dd-bd8d70823597"), "Mousse de Chocolate Caseira", "Mousse de chocolate caseira"),
        (Guid.Parse("2592aba9-f1b9-4c84-95a3-99b21b30bae0"), "Mousse de Chocolate C Chantily", "Mousse de chocolate com chantilly"),
        (Guid.Parse("2592aba9-f1b9-4c84-95a3-99b21b30bae0"), "Mousse de Chocolate com Chantilly", "Mousse de chocolate com chantilly"),
        (Guid.Parse("fcdf7d41-955f-49d0-907c-127f8a906b9a"), "Murganheira meio seco", "Murganheira Meio Seco"),
        (Guid.Parse("dbb13f87-9fe4-441e-ba4d-bca2475894d0"), "Ngola preta", "Ngola Preta"),
        (Guid.Parse("2add3507-7423-411f-9e19-b9651c8e6cbc"), "Ninfa touriga nacional syrah", "Ninfa Touriga Nacional Syrah"),
        (Guid.Parse("4df18856-9c52-4a32-8486-713c67d5de42"), "Omolete de Presunto", "Omolete de presunto"),
        (Guid.Parse("3dfdd75b-421f-4e97-9d34-c79b392a6aef"), "Omolete Queijo e Fiambre", "Omolete queijo e fiambre"),
        (Guid.Parse("1aa6c397-545b-45ce-a81a-bc802ed99744"), "Omolete Simples", "Omolete simples"),
        (Guid.Parse("7f8e8d15-e65a-467b-a859-00980a9d7b09"), "Ovos Estrelados C Bacon", "Ovos estrelados com bacon"),
        (Guid.Parse("74e9ccf6-7961-4f56-8c8c-f89d36193840"), "Palmier Recheado", "Palmier recheado"),
        (Guid.Parse("7346c936-6a50-42a7-8c3d-b0cdbb049405"), "Pastel de Côco", "Pastel de côco"),
        (Guid.Parse("5a4566c8-9fa7-4aa0-8e77-92ff867ef84e"), "Pastel de Feijão", "Pastel de feijão"),
        (Guid.Parse("4f8f2692-cd8e-4212-8ac8-b324ea900f47"), "Pastel de Nata", "Pastel de nata"),
        (Guid.Parse("c121133f-f23d-4962-809d-3d3c22c6dbc2"), "Pata de Veado", "Pata de veado"),
        (Guid.Parse("653f6588-0834-4f8c-ba79-91d6a57838b6"), "Paté de Sardinha", "Paté de sardinha"),
        (Guid.Parse("6567acc2-e948-4607-97c9-75abbda91531"), "Pescada Cozida", "Pescada cozida"),
        (Guid.Parse("d9aa08f1-968a-4a30-a2d0-6ed5f6baf7ef"), "Piornos reserva", "Piornos Reserva"),
        (Guid.Parse("93271679-aa26-4586-8f62-801f18cbeb93"), "Pizza Doce", "Pizza doce"),
        (Guid.Parse("771b5611-7679-45de-9dc8-2c7e3ac885c1"), "Pizza Especial", "Pizza especial"),
        (Guid.Parse("4b82adca-4c1b-4ba1-999d-56405ee7e7ba"), "Pizza Light", "Pizza light"),
        (Guid.Parse("37c15d52-47e4-4d54-b21d-8ba739a969ab"), "Pizza Tradicional", "Pizza tradicional"),
        (Guid.Parse("f3958333-43f3-4e94-916a-7a8db56cd35c"), "Polvo À Lagareiro", "Polvo à lagareiro"),
        (Guid.Parse("f3958333-43f3-4e94-916a-7a8db56cd35c"), "Polvo à Lagareiro", "Polvo à lagareiro"),
        (Guid.Parse("3f8e6ddc-9545-4f2b-9568-8166d469acb2"), "Ponte de lima", "Ponte de Lima"),
        (Guid.Parse("7574e1a8-0e4b-4d83-be24-9efb63ce165b"), "Porca de murça", "Porca de Murça"),
        (Guid.Parse("006fd7d9-af52-4a60-9f72-76b3cb28de65"), "Presunto Pata Negra", "Presunto pata negra"),
        (Guid.Parse("f9049b84-9a45-488c-bef9-b1533b3b7253"), "Presunto Serrano", "Presunto serrano"),
        (Guid.Parse("c9c956bf-ae99-4c70-b65c-4de63268a8ec"), "Primavera baga", "Primavera Baga"),
        (Guid.Parse("ab0d522f-5a0d-46e0-a5e3-ff09975d14b9"), "Pão de Alho", "Pão de alho"),
        (Guid.Parse("505bf8e1-0528-43f3-99af-f65dd8f36eb9"), "Qta Das Bageiras", "Qta das Bageiras"),
        (Guid.Parse("505bf8e1-0528-43f3-99af-f65dd8f36eb9"), "Qta das bageiras", "Qta das Bageiras"),
        (Guid.Parse("fc714b92-9bc2-41fc-99ab-fbf87bcb8046"), "Queijo Azeitão", "Queijo azeitão"),
        (Guid.Parse("413550d8-4e1e-428a-ad8d-300773d59e1f"), "Queijo Ovelha Curado", "Queijo ovelha curado"),
        (Guid.Parse("35374024-ec94-468f-b8cd-b0878061db28"), "Queijo Saloio", "Queijo saloio"),
        (Guid.Parse("d33ef167-a9e7-439d-9eda-4d26c88280a9"), "Queijo Serra", "Queijo serra"),
        (Guid.Parse("353485f7-0a0b-4cca-b403-beabfe0a5f37"), "Quinta d aveleda", "Quinta D Aveleda"),
        (Guid.Parse("7aa8c0b8-4492-46b5-8858-f50f1af3dce4"), "Quinta Das Apegadas", "Quinta das Apegadas"),
        (Guid.Parse("7aa8c0b8-4492-46b5-8858-f50f1af3dce4"), "Quinta das apegadas", "Quinta das Apegadas"),
        (Guid.Parse("9983c630-c735-4685-a8ff-3f852764f05b"), "Quinta de cabriz", "Quinta de Cabriz"),
        (Guid.Parse("b51d313d-3931-43e1-bdc3-118b8728c659"), "Quinta de cabriz branco", "Quinta de Cabriz Branco"),
        (Guid.Parse("bbdfbb80-fee6-46b0-aff2-cf5a006615e2"), "Quinta Do Ortigão Baga", "Quinta do Ortigão Baga"),
        (Guid.Parse("bbdfbb80-fee6-46b0-aff2-cf5a006615e2"), "Quinta do ortigão baga", "Quinta do Ortigão Baga"),
        (Guid.Parse("0a4faaa4-f210-4a69-a02a-8e42fe6ff7cf"), "Quintela reserva", "Quintela Reserva"),
        (Guid.Parse("0d30bf31-ecc4-452e-9b43-ee9d5c1d7fb6"), "Red bull", "Red Bull"),
        (Guid.Parse("0ff63619-916e-4291-82fd-47ee965c43ed"), "Sagres bohemia", "Sagres Bohemia"),
        (Guid.Parse("735d38e5-28b0-4b9b-b840-20b0cc58d006"), "Sagres mini", "Sagres Mini"),
        (Guid.Parse("86ad446e-3c18-4fee-9d5a-d778c0d84551"), "Sagres preta 33cl", "Sagres Preta 33cl"),
        (Guid.Parse("126892a7-df23-4b2b-8b19-9af7b2f91196"), "Sagres preta mini", "Sagres Preta Mini"),
        (Guid.Parse("638636ba-5a93-4c2d-8897-706143393474"), "Sagres s.a", "Sagres S.A"),
        (Guid.Parse("47867cb6-70b1-498f-82a8-3471cef54792"), "Salada de Atum com Mayonese", "Salada de atum com mayonese"),
        (Guid.Parse("eae3491c-0c6d-4a12-a069-3c5bb77bdc00"), "Salada de Fruta", "Salada de fruta"),
        (Guid.Parse("c8d51cb3-2fbc-44aa-8395-5880c6338da8"), "Salmão Grelhado", "Salmão grelhado"),
        (Guid.Parse("882751a4-16b7-40ab-aef3-326e27464e02"), "Sande Americana", "Sande americana"),
        (Guid.Parse("2463828b-5494-47f0-a617-ed3542ca3b1e"), "Sande de Fiambre", "Sande de fiambre"),
        (Guid.Parse("488c6271-bed5-4394-890f-17d1724b50a9"), "Sande de Queijo", "Sande de queijo"),
        (Guid.Parse("a617ca80-834b-4c94-9768-c8dbeaef4d65"), "Sande Mista", "Sande mista"),
        (Guid.Parse("ac2cc59b-8508-407e-8f7c-df3c90d2c451"), "Sanduíche de Atum", "Sanduíche de atum"),
        (Guid.Parse("8f96aab5-46d5-4e49-9ea4-39014b5547b7"), "Sanduíche de Frango", "Sanduíche de frango"),
        (Guid.Parse("7b45a01d-50ee-42d3-a4af-0dcde9397e93"), "Santàl nectar", "Santàl Nectar"),
        (Guid.Parse("7c7a47d4-c7eb-4e54-9cd1-22ba67cb9aba"), "Sardinha Assada", "Sardinha assada"),
        (Guid.Parse("d02a3d1f-c08b-433a-9a61-2aa3d042c568"), "Secretos de Porco Preto", "Secretos de porco preto"),
        (Guid.Parse("6f7821a5-902f-41c2-bc47-0f1cc758de1b"), "Sopa Da Pedra", "Sopa da pedra"),
        (Guid.Parse("b79fc237-caab-4907-ae12-a25cd4e0fb4b"), "Sopa de Legumes", "Sopa de legumes"),
        (Guid.Parse("975a37fd-06d7-4adb-a3ea-fc9a0cacd880"), "Sopa de Marisco", "Sopa de marisco"),
        (Guid.Parse("f8ab342b-5702-414d-87a6-13dc62f31f98"), "Sopa de Peixe", "Sopa de peixe"),
        (Guid.Parse("07dd6f13-9e5a-4461-ac7c-1ff77018aef4"), "Sta valha reserva", "Sta Valha Reserva"),
        (Guid.Parse("f71b3648-bb41-4952-ac75-ee93ccf0ec66"), "Sumo Natural", "Sumo natural"),
        (Guid.Parse("a0604c04-4413-427e-962b-3433e4c7678a"), "Super bock 33cl", "Super Bock 33cl"),
        (Guid.Parse("ab4ccfe1-4877-4158-9986-1f64098c3ad8"), "Super bock abadia", "Super Bock Abadia"),
        (Guid.Parse("d7d209c3-6dab-4439-b69c-5cdda55e324a"), "Super bock mini", "Super Bock Mini"),
        (Guid.Parse("70b503e1-5433-4465-ab23-251ddc720b46"), "Super bock mini stout", "Super Bock Mini Stout"),
        (Guid.Parse("678b0690-33ca-485b-a3b6-4555c76b0c94"), "Super bock s.a", "Super Bock S.A"),
        (Guid.Parse("edc1f935-fbbd-41c1-8c9e-8b8bf4e08b39"), "São domingos rosé bruto", "São Domingos Rosé Bruto"),
        (Guid.Parse("9cc54f01-65c7-4dc7-b47b-8ee8059023d0"), "Talismã reserva", "Talismã Reserva"),
        (Guid.Parse("1b9234c9-a762-405c-87e8-90f3dcb508f5"), "Tapada Dos Monges", "Tapada dos Monges"),
        (Guid.Parse("1b9234c9-a762-405c-87e8-90f3dcb508f5"), "Tapada dos monges", "Tapada dos Monges"),
        (Guid.Parse("b90be65a-983e-4e1b-a46d-cdb3716c3c47"), "Tarte de Amêndoa", "Tarte de amêndoa"),
        (Guid.Parse("bf155f62-9508-4215-b64d-4eb9cbee01e6"), "Tarte de Chila", "Tarte de chila"),
        (Guid.Parse("be42c2c5-83a9-4387-b3cd-1249e6e4c68c"), "Tarte de Maçã", "Tarte de maçã"),
        (Guid.Parse("82ffc0d2-efba-428c-ac74-b2d88112b7cd"), "Terra quente", "Terra Quente"),
        (Guid.Parse("89d868ec-eb47-43c3-92f8-8be9a1366189"), "Terras Do Demo Bruto", "Terras do Demo bruto"),
        (Guid.Parse("89d868ec-eb47-43c3-92f8-8be9a1366189"), "Terras do Demo Bruto", "Terras do Demo bruto"),
        (Guid.Parse("89d868ec-eb47-43c3-92f8-8be9a1366189"), "Terras do demo bruto", "Terras do Demo bruto"),
        (Guid.Parse("66cf37fe-f82f-4659-8ad5-ff115dc4badb"), "Torta de Noz", "Torta de noz"),
        (Guid.Parse("6602ffeb-3af4-4c6c-b0b1-2f680428bb3e"), "Tosta de Fiambre", "Tosta de fiambre"),
        (Guid.Parse("a803569c-edef-48e3-b275-e1df72e7ba86"), "Tosta de Queijo", "Tosta de queijo"),
        (Guid.Parse("608d6a7b-378a-4cb2-b3d9-8afc13f560fe"), "Tosta Mista", "Tosta mista"),
        (Guid.Parse("0ebdf4ad-f628-45b4-b4ff-7b2ea55c2bb5"), "Toucinho Do Ceu", "Toucinho do Céu"),
        (Guid.Parse("77109198-6d71-4f41-a6b7-3c2a2e071ddf"), "Vila régia", "Vila Régia"),
        (Guid.Parse("4324c0bf-d3ae-4678-95d3-6f31b689a1a3"), "Vila régia b", "Vila Régia B"),
        (Guid.Parse("fd59d1ef-1a04-4d60-a274-7ebf334259fe"), "Vinhas dalém", "Vinhas Dalém"),
        (Guid.Parse("ba65b05b-5d52-4195-96b8-991fc8d894b1"), "Vodka eristoff", "Vodka Eristoff"),
        (Guid.Parse("703902b4-6c10-47b9-8975-e815fe55c77f"), "Vodka smirnoff", "Vodka Smirnoff"),
        (Guid.Parse("5b8f420c-36bd-4efc-8013-4929d4412a9c"), "Água castelo", "Água Castelo"),
        (Guid.Parse("df10b18d-2746-4abf-9a22-ea8b3cfab71e"), "Água castelo sabores", "Água Castelo Sabores"),
        (Guid.Parse("d12258dc-7f7d-4b05-8f3b-94c306c1a2ea"), "Água luso 1.5l", "Água Luso 1.5l"),
        (Guid.Parse("010faa02-c13e-4119-ba0d-4cf1acb3e49f"), "Água luso 1l", "Água Luso 1l"),
        (Guid.Parse("9b54c6df-dc8e-4a0b-8937-433f141fd77c"), "Água luso 33cl", "Água Luso 33cl"),
        (Guid.Parse("a2aab7f5-7e0b-4333-8ab8-e120214d67dc"), "Água luso 50cl", "Água Luso 50cl"),
        (Guid.Parse("1a2e615c-743e-4c24-b0c1-830696299afd"), "Água pedras sabores", "Água Pedras Sabores"),
        (Guid.Parse("c52defdf-3e29-483c-9e60-ddc2cb1286e2"), "Água pedras salgadas", "Água Pedras Salgadas"),
    ];

    public static readonly (Guid Id, string Old, string New)[] ArticleSubfamilyDesignations =
    [
        (Guid.Parse("3c8e7913-3245-47e5-a975-0bfe00e56b5a"), "Café E Leite", "Café e leite"),
        (Guid.Parse("42e26888-36e0-4642-bd5b-afbf589c1aea"), "Cartão De Cliente", "Cartão de cliente"),
        (Guid.Parse("1c427e91-55f8-4b10-bc5e-96e3016c58e0"), "Outras Caf", "Outros de cafetaria"),
        (Guid.Parse("1c427e91-55f8-4b10-bc5e-96e3016c58e0"), "Outras cafetaria", "Outros de cafetaria"),
        (Guid.Parse("f954af89-fec4-409a-b152-3bb66a7473c2"), "Outras Ent", "Outras entradas"),
        (Guid.Parse("838c5a3d-484c-473a-91c4-a6702da615ab"), "Ref Rápidas", "Refeições rápidas"),
        (Guid.Parse("3c1aee38-8dca-4be4-8937-762c68097ad4"), "Sandes E Tostas", "Sandes e tostas"),
        (Guid.Parse("a2d7fb46-3c8a-4c3b-a661-859f2b0d4840"), "Vinhos Brancos", "Vinhos brancos"),
        (Guid.Parse("665adc99-6f00-4b2a-acb8-9f4be44f699d"), "Vinhos Tintos", "Vinhos tintos"),
        (Guid.Parse("2473a21e-e372-4020-afc7-93182c42a5f2"), "Vinhos Verdes", "Vinhos verdes"),
    ];

    public static readonly Guid VirtualMoneyPaymentMethodId = Guid.Parse("178a2e72-60d8-429b-ae25-7558ed40f84e");
    public static readonly Guid StockGainPermissionItemId = Guid.Parse("3fb11872-bbd4-496e-bb3f-bba1e93edffc");

    public static IEnumerable<string> ForSqlite() => Build(SqlDialect.Sqlite);

    public static IEnumerable<string> ForMySql() => Build(SqlDialect.MySql);

    public static IEnumerable<string> ForSqlServer() => Build(SqlDialect.SqlServer);

    public static IEnumerable<string> ForPostgreSql() => Build(SqlDialect.PostgreSql);

    private enum SqlDialect
    {
        Sqlite,
        MySql,
        SqlServer,
        PostgreSql
    }

    private static IEnumerable<string> Build(SqlDialect dialect)
    {
        yield return DeleteDuplicateTransport(dialect);

        foreach (var sql in ClearSmtpExamples(dialect))
        {
            yield return sql;
        }

        yield return UpdateDesignationById(
            dialect,
            "PaymentMethods",
            VirtualMoneyPaymentMethodId,
            "Dinheiro Eletrônico",
            "Dinheiro Eletrónico");
        yield return UpdateDesignationById(
            dialect,
            "PermissionItems",
            StockGainPermissionItemId,
            "__designation[4040]__",
            "Relatório :: Stock por valorização de artigo");

        foreach (var (id, oldValue, newValue) in ArticleDesignations)
        {
            yield return UpdateDesignationById(dialect, "Articles", id, oldValue, newValue);
        }

        foreach (var (id, oldValue, newValue) in ArticleSubfamilyDesignations)
        {
            yield return UpdateDesignationById(dialect, "ArticleSubfamilies", id, oldValue, newValue);
        }
    }

    private static IEnumerable<string> ClearSmtpExamples(SqlDialect dialect)
    {
        string[] userExamples = ["YOUR-EMAIL@gmail.com", "exemplo@empresa.pt"];
        string[] passwordExamples = ["YOUR-PASSWORD"];

        foreach (var example in userExamples)
        {
            yield return UpdateTokenValue(dialect, "SEND_MAIL_SMTP_USERNAME", example, string.Empty);
        }

        foreach (var example in passwordExamples)
        {
            yield return UpdateTokenValue(dialect, "SEND_MAIL_SMTP_PASSWORD", example, string.Empty);
        }
    }

    private static string DeleteDuplicateTransport(SqlDialect dialect) =>
        dialect switch
        {
            SqlDialect.Sqlite =>
                """
DELETE FROM PreferenceParameters
WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  AND rowid NOT IN (
    SELECT MIN(rowid) FROM PreferenceParameters
    WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  );
""",
            SqlDialect.MySql =>
                """
DELETE FROM PreferenceParameters
WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  AND Id NOT IN (
    SELECT Id FROM (
      SELECT Id
      FROM PreferenceParameters
      WHERE Token = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
      ORDER BY CreatedAt ASC, Id ASC
      LIMIT 1
    ) AS KeepTransport
  );
""",
            SqlDialect.PostgreSql =>
                """
DELETE FROM "PreferenceParameters" AS pp
USING (
    SELECT "Id",
           ROW_NUMBER() OVER (ORDER BY "CreatedAt" ASC, "Id" ASC) AS "RowNumber"
    FROM "PreferenceParameters"
    WHERE "Token" = 'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
) AS ranked
WHERE pp."Id" = ranked."Id"
  AND ranked."RowNumber" > 1;
""",
            _ =>
                """
WITH RankedTransport AS (
    SELECT Id,
           ROW_NUMBER() OVER (ORDER BY CreatedAt ASC, Id ASC) AS RowNumber
    FROM PreferenceParameters
    WHERE Token = N'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
)
DELETE FROM PreferenceParameters
WHERE Token = N'COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS'
  AND Id IN (SELECT Id FROM RankedTransport WHERE RowNumber > 1);
"""
        };

    private static string UpdateTokenValue(SqlDialect dialect, string token, string oldValue, string newValue)
    {
        var table = Quote(dialect, "PreferenceParameters");
        var valueColumn = Quote(dialect, "Value");
        var tokenColumn = Quote(dialect, "Token");
        return $"UPDATE {table} SET {valueColumn} = {Literal(dialect, newValue)} WHERE {tokenColumn} = {Literal(dialect, token)} AND {valueColumn} = {Literal(dialect, oldValue)};";
    }

    private static string UpdateDesignationById(SqlDialect dialect, string table, Guid id, string oldValue, string newValue)
    {
        var quotedTable = Quote(dialect, table);
        var designation = Quote(dialect, "Designation");
        var idColumn = Quote(dialect, "Id");
        var idLiteral = GuidLiteral(dialect, id);
        var oldLiteral = BinaryStringLiteral(dialect, oldValue);
        var newLiteral = Literal(dialect, newValue);
        return $"UPDATE {quotedTable} SET {designation} = {newLiteral} WHERE {idColumn} = {idLiteral} AND {designation} = {oldLiteral};";
    }

    private static string GuidLiteral(SqlDialect dialect, Guid id) =>
        dialect switch
        {
            // EF Core SQLite stores GUIDs as uppercase TEXT (8-4-4-4-12).
            SqlDialect.Sqlite => $"'{id.ToString().ToUpperInvariant()}'",
            SqlDialect.PostgreSql => $"'{id}'::uuid",
            SqlDialect.SqlServer => $"'{id}'",
            _ => $"'{id}'"
        };

    private static string BinaryStringLiteral(SqlDialect dialect, string value)
    {
        var literal = Literal(dialect, value);
        return dialect switch
        {
            SqlDialect.MySql => $"{literal} COLLATE utf8mb4_bin",
            SqlDialect.SqlServer => $"{literal} COLLATE Latin1_General_100_BIN2",
            // SQLite string equality is binary by default (no NOCASE on Designation).
            _ => literal
        };
    }

    private static string Quote(SqlDialect dialect, string identifier) =>
        dialect == SqlDialect.PostgreSql ? $"\"{identifier}\"" : identifier;

    private static string Literal(SqlDialect dialect, string value)
    {
        var escaped = value.Replace("'", "''");
        return dialect == SqlDialect.SqlServer ? $"N'{escaped}'" : $"'{escaped}'";
    }
}
