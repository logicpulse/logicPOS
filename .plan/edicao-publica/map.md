## Destination

O repositório público `logicPOS` publica só a edição pública: base de dados direta, sem ligação à API, sem certificação e sem registo de licença, com um encaixe para quem quiser a sua própria certificação. A edição LogicPulse é um anfitrião fechado que referencia esses projetos e acrescenta a cloud, a certificação LogicPulse e a licença. O GTK está descontinuado e não fica no repositório.

## Notes

- O único repositório público é `logicPOS`. Privados, neste posto: `logicpos-api`, `logicpos-apiclient`, `logicpos-migrators`, `logicpos-webapp`. A pasta `logicpos-fiscal` ainda não é um repositório git.
- O público não tem cliente HTTP, `BaseAddress`, `ClientId` nem carregador de cloud. A base de dados direta aceita SQLite, MySQL e SQL Server.
- O único encaixe público é o de certificação (`plugins\*Plugin.dll`). Sem plugin, o documento imprime sem ATCUD, SAF-T, hash, QR nem linha de programa certificado.
- A persistência é uma cópia única, partilhada com a API. Os três migradores entram na edição pública. Mudar a pasta desse modelo para dentro do GitHub fica para a sessão que executa a separação.
- Este mapa não implementa. Quando não houver tickets abertos, a regra está pronta para outra sessão a pôr no código.
- Skills: `wayfinder`, `domain-modeling`, `research`. Termos em `GLOSSARY.md`. Conversação em português.

## Decisions so far

- [Fronteira do opensource antigo](tickets/01-fronteira-legado.md): o legado público já continha a certificação (AT, ATCUD, hash, QR, SAF-T); fora ficavam os segredos LogicPulse, a licença e os stocks.
- [Persistência da base de dados direta](tickets/07-persistencia-direta.md): a edição pública usa SQLite, MySQL e SQL Server sobre o mesmo modelo; o arranque deixa de recusar os servidores.
- [Superfície certificada no repo público](tickets/02-superficie-no-publico.md): na Avalonia as chamadas à autoridade estão no plugin fiscal da base direta; rodapé, QR, hash, série de teste e ecrãs SAF-T/AT continuam no público. No GTK isso sai pelo cliente HTTP.

## Not yet specified

- **Onde nasce o anfitrião LogicPulse** se nenhum repositório privado atual servir. O GTK não tem arquivo. <clears-with: 03>

## Out of scope

- Pôr a separação no código, nos projetos e no zip de release. O destino deste mapa é a regra, não a alteração.
- Fundir os repositórios privados, ou tornar públicos `logicpos-api`, `logicpos-apiclient`, `logicpos-migrators` e `logicpos-webapp`.
- Deixar a certificação só na API, com a app a nunca falar com a AT ou a AGT. Isso tirava a base de dados direta à edição LogicPulse.
- Um slot de cloud ou qualquer cliente HTTP no repositório público.
- Apagar o GTK. A app GTK e o cliente HTTP estão descontinuados e saem do repositório.
