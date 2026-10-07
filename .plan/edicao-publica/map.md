## Destination

Uma regra fechada: o que pode viver no repositório público `logicPOS`, o que fica nos repositórios privados, e como a edição LogicPulse junta cloud, certificação e licença sem ninguém limpar o público à mão.

## Notes

- O único repositório público é `logicPOS`. Privados, neste posto: `logicpos-api`, `logicpos-apiclient`, `logicpos-migrators`, `logicpos-webapp`. A pasta `logicpos-fiscal` ainda não é um repositório git.
- A regra cobre o repositório público inteiro, a app Avalonia e a app GTK.
- Cloud, certificação e licença entram da mesma forma: o contrato e o carregador ficam no público; a peça em si fica de fora. Sem a peça, a edição pública usa base de dados direta, não certifica e não pede registo de licença. A edição LogicPulse traz as peças e pode usar cloud ou base de dados direta.
- Certificação inclui as ligações à AT e à AGT e também o ATCUD, as séries certificadas e o que o documento mostra por causa delas. Não é a licença nem a cloud.
- Este mapa não implementa. Quando não houver tickets abertos, a regra está pronta para outra sessão a pôr no código.
- Skills: `wayfinder`, `domain-modeling`, `research`. Termos em `GLOSSARY.md`. Conversação em português.

## Decisions so far

<!-- the index — one line per resolved ticket: enough to judge relevance, then zoom the link for the detail the ticket holds -->

## Not yet specified

- **SAF-T, hash e QR.** Não está fechado se o ficheiro SAF-T, o hash do documento e o QR são certificação (saem do público) ou impressão genérica que a edição pública pode ter. <clears-with: 01>
- **Onde nasce a certificação se não couber num repositório que já existe.** A pasta `logicpos-fiscal` não é git. Se nenhuma casa privada servir, falta o sítio novo. <clears-with: 03>

## Out of scope

- Pôr a separação no código, nos projetos e no zip de release. O destino deste mapa é a regra, não a alteração.
- Fundir os repositórios privados, ou tornar públicos `logicpos-api`, `logicpos-apiclient`, `logicpos-migrators` e `logicpos-webapp`.
- Deixar a certificação só na API, com a app a nunca falar com a AT ou a AGT. Isso tirava a base de dados direta à edição LogicPulse.
