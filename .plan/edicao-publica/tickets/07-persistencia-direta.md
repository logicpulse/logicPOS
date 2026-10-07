---
type: grilling
blocked_by: []
status: resolved
---

# Persistência da base de dados direta

## Question

A edição pública abre um SQLite, mas o modelo que usa está no `logicpos-api`, o mesmo que o servidor da API usa. A cópia única muda para o `logicPOS` e o servidor passa a referenciá-la, ou o modelo parte-se?

## Answer

Não se parte o modelo. A edição pública usa SQLite, MySQL e SQL Server em cima da mesma persistência que a API. O arranque deixou de recusar o que não fosse SQLite e o projeto público referencia os três migradores. A mudança da pasta desse modelo para dentro do GitHub fica para a sessão que executa a separação.
