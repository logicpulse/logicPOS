# Correções N1–N20

## Destination

Um spec de correção para a lista N1–N20: o comportamento esperado de cada item, na edição pública e na edição LogicPulse, em bases novas e em bases já criadas. O mapa fecha decisões. Não altera código de produto.

## Notes

- Edições no âmbito: as duas. A certificação não é igual. Só a edição pública fica sem certificação AT. Na edição LogicPulse, a escolha Community tem certificação, porque a DLL fiscal vem com a app. Confirma o glossário (`GLOSSARY.md`, termos Community e Certificação).
- A lista foi testada numa build anterior ao trabalho local de hoje (2026-11-23) neste checkout `dev`. O commit `e4616a04` (junção do ticket 22), citado em N1, já está em `dev` (`9c774e15`). Não tratar as junções de 2026-11-20 como posteriores ao teste.
- A auditoria de 2026-11-23 (`research/qa-n1-n14-ja-feito`, commit `0df3c302`) não fez fetch. Só viu `dev` local, o diff por gravar e os ramos `ticket/*` já presentes. Esses não têm commits fora de `dev`. O diff local não fecha nenhum item.
- No GitHub, depois do fetch, há ramos de 2026-11-23 ainda fora de `dev`. No público `logicpulse/logicPOS`: `fix/dados-2.0.35-r2`, `fix/dados-textos-2.0.35`, `fix/ux-2.0.35-r3`, `fix/ux-2.0.35-r2`, `fix/guids-marcas-fiscais`, `cursor/textos-pt-2.0.35-0ee5`, `cursor/ux-items-9-18-price-pad-c4c7`, `cursor/fix-fo-shell-restore-race-237a`. No `logicpulse/logicPOS-internal`: `fix/dados-2.0.35-r2`, `fix/dados-textos-2.0.35`, `fix/textos-api-2.0.35`, `fix/instalador-2.0.35`, `fix/grafia-ativa-internal`, `cursor/textos-pt-2.0.35-0ee5`. Nenhum está merged em `dev`. Não há pull requests novos.
- Comportamento já determinado, ainda por escrever no spec (sem ticket de decisão): N1 chave duplicada, N2 texto de Community por edição, N4 recibo em Numerário, a chave «Pag. Parcial» de N7, N8 cópia, N9 app elevada no fim da instalação, N11 página vazia em português, N14 verificar a versão antes de perguntar pelo download. O detalhe está na resposta de [O que é que o trabalho de hoje já fecha na lista N1–N14](issues/01-o-que-ja-esta-feito.md).
- N15–N20, vistos no código em 2026-11-23. Não estão no diff local dos relatórios de stock.
  - N15. O botão `sales-table-detail` diz «Vendas por mesa (Detalhado)». O PDF `SalesByTableDetailedReportPdfModel` titula «Vendas por Zona (Detalhado)», o mesmo título do relatório de zona. O título do PDF da mesa passa a ser o do botão.
  - N16. O título do PDF e o agrupamento seguem o botão. «Vendas por taxa IVA e tipo de artigo» não pode sair como «Vendas por Taxa IVA» nem agrupar por tipo de documento (`SalesByTaxByArticleTypeReportPdfModel` agrupa por `DocumentType` e a coluna chama-se «Tipo Documento»). «Ganho por artigo» não pode sair como «Stock por valorização de artigo». «Auditoria do sistema» não pode sair como «Tabelas de Auditoria».
  - N19. Em `dev`, o estado da mesa mostra `TableStatus.Free` como «Free». Em `fix/ux-2.0.35-r3`, por integrar, `ListingEnumDisplay` mostra «Livre», «Ocupada» e «Reservada» no editor. Ainda não está em `dev`.
  - N20. «Modo do cartão» mostra `CardMode.Debit` como «Debit». Esse ramo não traduz `CardMode`. O editor mostra Débito e Crédito.
- O instalador e o anfitrião LogicPulse vivem em `C:\SVN\logicPOS-api-integration\logicPOS-internal`, fora do repositório público.
- Spec publicado: [spec.md](spec.md). Tickets 11–18 e as decisões 02–08 estão gravados em `dev`. O 04 já era o comportamento do pagamento misto. O que já existia em `dev` ficou de fora da spec.
- Skills: grilling, domain-modeling, research. Tracker: markdown em `.scratch/qa-n1-n14/` (a pasta `.scratch/` está no `.gitignore`).
- Glossário já fechado, e que este spec não reabre: recibo com método por omissão Numerário; pagamento misto em que as parcelas somam o total a liquidar; MB Way liquida sem confirmação no telemóvel.

## Decisions so far

- [O que é que o trabalho de hoje já fecha na lista N1–N14](issues/01-o-que-ja-esta-feito.md): em `dev` nenhum fecha; no GitHub de 2026-11-23, por integrar, há ramos que tratam N1, N5, N7, N8 e N9. N2 e N4 continuam como no `dev` nesses ramos.
- [O que lista o sumário de vendas por condição de pagamento?](issues/09-sumario-condicao-pagamento.md): um sumário das linhas, agrupadas por condição de pagamento, sem linha por artigo.
- [O que abre «Ver relatório completo» no Top 10 artigos?](issues/10-ver-relatorio-completo-top-artigos.md): o relatório «Total vendido por artigo» (`articles-sold`), chamado aqui «Vendas por artigo Resumido».
- [A atualização reescreve textos de semente numa base já criada?](issues/02-atualizacao-reescreve-textos-seed.md): só as alterações dessa versão. A semente que a versão não muda fica como está.
- [MB Way substituiu o Multibanco ou são dois métodos?](issues/03-mb-way-e-multibanco.md): são dois métodos. MB Way só numa base de Portugal.
- [Conta Corrente pode ser parcela de um pagamento misto?](issues/04-conta-corrente-no-pagamento-misto.md): sim, e essa parcela conta como liquidada no documento.
- [O que faz Gerar sem cliente ou sem artigo?](issues/05-gerar-sem-cliente-ou-artigo.md): recusa se o filtro estiver vazio. «Todos», que já vem seleccionado, emite o relatório para todos.
- [Qual é o período por omissão dos relatórios?](issues/06-periodo-omissao-dos-relatorios.md): do primeiro ao último dia do mês actual nos relatórios, e do ano actual nas listas que abrem no ano.
- [O que deve explicar a dica Reemb. Volta?](issues/07-dica-reemb-volta.md): emite o talão de reembolso do depósito de vasilhame e imprime o documento `TRV`.
- [A lista cloud de Sessões de Trab. mostra o período ou só terminais abertos?](issues/08-lista-sessoes-de-trabalho-cloud.md): só as sessões abertas.

## Not yet specified

## Out of scope

- Implementar as correções neste mapa. O destino é o spec.
- Chamar «Community» à edição pública. O glossário reserva Community à escolha de primeiro arranque.
