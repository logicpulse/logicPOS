---
type: research
blocked_by: []
status: resolved
---

# Superfície certificada no repo público

## Question

No repositório público `logicPOS` de hoje, que tipos, campos, ecrãs e chamadas são certificação ou registo de licença, na app Avalonia e na app GTK, e quais já estão atrás do carregador da cloud?

## Answer

Nota: [Superfície certificada no repo público](../research/02-superficie-no-publico.md).

Na Avalonia, as chamadas à AT e à AGT passam por `IFiscalModule` e só no caminho da base de dados direta. O carregador da cloud não tem certificação nem licença; se a cloud carregar, o plugin fiscal nem é registado. Fora dos dois carregadores continuam o rodapé `2543/AT` e `221/AGT`, o QR, o hash local, a série `TESTELOCAL`, os menus SAF-T/AGT e o passo AT do assistente. A licença corre antes da cloud e não tem ecrã de registo. No GTK não há esses carregadores: SAF-T, séries, envio à AT, páginas AGT e o `RegisterModal` saem pelo cliente HTTP.
