# O que é que o trabalho de hoje já fecha na lista N1–N14

Type: research
Status: resolved
Blocked by:

## Question

Quais dos itens N1–N14 já estão corrigidos no checkout `dev` (incluindo as alterações por gravar de 2026-11-23), em `integrate/tickets-01-25`, ou num ramo `ticket/*`, e quais continuam abertos?

Para cada item, indicar o commit, o ficheiro do diff local, ou o ramo que o trata, ou dizer que nenhum o cobre. Separar o que vive no repositório público (`C:\SVN\logicPOS-api-integration\logicPOS`) do que só existe no anfitrião LogicPulse (`C:\SVN\logicPOS-api-integration\logicPOS-internal`: instalador, certificação, atualização).

A lista foi testada numa build anterior ao trabalho local de hoje. O commit `e4616a04` já está em `dev`, e N1 foi visto nesse código. As junções de 2026-11-20 não contam, por si, como correção posterior ao teste.

### Lista

- N1 Bases novas em Portugal. `db_seeds/pt/preferenceparameters.json` tem `COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS` duas vezes, com o mesmo identificador, vindo da junção `e4616a04`. Base nova em Portugal deve falhar por chave repetida.
- N2 Primeiro arranque (Community). O texto diz «A Community inclui faturação local sem certificação AT integrada…» e já não diz «Faturação certificada». No manual, a CE aparece certificada. Regra deste mapa: sem certificação AT só na edição pública; na edição LogicPulse a Community tem certificação.
- N3 Pagamentos. «MB Way» ocupou o lugar do Multibanco (código interno «MB»). O FrontOffice já não tem Multibanco; numa base atualizada o BackOffice continua a mostrar «MultiBanco».
- N4 Emissão Recibos. O método por omissão passou a «Cartão de Crédito». O glossário e a alteração anunciada dizem Numerário.
- N5 Atualização de versões. Correções de textos e dados só chegam a bases novas. Quem atualiza mantém «Dinheiro Eletrônico» e artigos de demonstração sem acentos.
- N6 Pagamento misto. «Conta Corrente» aparece como método numa das parcelas e pode deixar essa parte dada como paga.
- N7 Dicas. A dica de «Pag. Parcial» não aparece, registada como «Pag.Parcial», sem espaço. A de «Reemb. Volta» diz só «Reembolso de volta».
- N8 Textos. «Nenhuma série activa…», «Day» em Sessões de Trab., «Imprimir» com maiúscula a meio da frase.
- N9 Instalador. A app que abre no fim da instalação corre como administrador.
- N10 Relatórios. Em «Saldo de clientes (detalhe)» e «Stock por artigo», sem cliente ou artigo escolhido, «Gerar» não faz nada e não avisa.
- N11 Relatórios. Um relatório sem dados mostra «No Content. Check the Filters».
- N12 Relatórios. O período por omissão nos filtros é o mês atual.
- N13 Sessões de Trab. Com a base cloud, a lista mostra 0 registos em 2026, apesar de haver vendas.
- N14 Sistema → Atualizar. Pergunta logo «Descarregar a atualização e reiniciar?», sem verificar se há versão nova nem dizer qual é.

## Answer

Nenhum dos N1–N14 fica fechado por completo. Os catorze continuam abertos. Não há correção só num ramo `ticket/*` ou em `integrate/tickets-01-25`: esses ramos têm 0 commits fora de `dev` (`9c774e15`). O detalhe, com ficheiro e commit, está em `research/qa-n1-n14-ja-feito`, ficheiro `docs/research/qa-n1-n14-ja-feito.md` (commit `0df3c302`, enviado para `origin`).

- N1 ainda aberto em `dev`. A chave `COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS` está duplicada, com o mesmo `Id`, em `db_seeds/pt/preferenceparameters.json`. Entrou em `e6eb94f0`, já antepassado de `e4616a04`.
- N2 ainda aberto em `dev`. `EditionSetupWindow.axaml` (`d5195946`) diz que a Community não tem certificação AT. O anfitrião não tem outro texto. O glossário separa a edição pública da Community LogicPulse.
- N3 ainda aberto. `72ad7b67` renomeia a designação do acrónimo `MB` para «MB Way» na semente. O FrontOffice escreve «MB Way» à mão. Uma base já criada não é reescrita: `ApplyNewSeed` não toca em métodos de pagamento. Decisão em [MB Way e Multibanco](03-mb-way-e-multibanco.md).
- N4 ainda aberto em `dev`. O recibo selecciona o primeiro método por `Order`, e esse é «Cartão de Crédito». O glossário já pede Numerário. Erro claro, sem ticket novo.
- N5 ainda aberto. `a1d1335f` corrige textos só nos JSON. A atualização chama `ApplyNewSeed`, que não reescreve linhas existentes. Decisão em [textos de semente](02-atualizacao-reescreve-textos-seed.md).
- N6 ainda aberto em `dev`. O misto oferece «Conta Corrente» e, com pagamento integral, o documento fica `Paid`. Decisão em [Conta Corrente no misto](04-conta-corrente-no-pagamento-misto.md).
- N7 ainda aberto em `dev`. A dica está em `Pag.Parcial` e o botão diz «Pag. Parcial». «Reemb. Volta» explica-se com «Reembolso de volta». A chave é erro claro. O texto da dica está em [Reemb. Volta](07-dica-reemb-volta.md).
- N8 ainda aberto. «série activa» e «deseja Imprimir» continuam no repositório público. «Day […]» é a designação que a API do anfitrião grava ao abrir o dia. Erros de cópia, sem ticket novo.
- N9 ainda aberto, fora deste repositório. O instalador em `logicPOS-internal/setup/logicPOS.Setup` pede administrador e arranca o POS com `Process.Start` sem baixar o privilégio. O manifesto da app pública não pede administrador.
- N10 ainda aberto no checkout com o diff de hoje. O aviso «Escolha um cliente.» está commitado. O diff de `IReportService.cs` põe `NeedsArticle` a falso, por isso «Stock por artigo» sem artigo deixa de avisar. Decisão em [Gerar sem cliente ou artigo](05-gerar-sem-cliente-ou-artigo.md).
- N11 ainda aberto. A página inglesa continua em `NoContentPdfModel.cs` no anfitrião e o posto local ainda a abre, salvo três stocks vazios que o diff de hoje troca pelo alerta em português. O interceptar no `CloudReportService.cs` do anfitrião está só por gravar, não commitado. Erro claro, sem ticket novo.
- N12 ainda aberto. O período inicial é o mês actual (`cb12112d` e o construtor). O diff de hoje acrescenta atalhos e não muda o valor. Decisão em [período por omissão](06-periodo-omissao-dos-relatorios.md).
- N13 a lista a zero na cloud tem causa: `GetOpenTerminalSessionsQuery` só devolve sessões de terminal abertas, e a grelha filtra depois pelo ano corrente. Vendas não entram nessa consulta. O que a lista deve mostrar está em [sessões cloud](08-lista-sessoes-de-trabalho-cloud.md).
- N14 ainda aberto em `dev`. `ShowUpdate` abre logo «Descarregar a atualização e reiniciar?» sem dizer a versão. A comparação só corre depois do Sim, em `ApplicationUpdater`. Erro claro, sem ticket novo.

O diff de hoje não fecha nenhum item. Nos relatórios de stock, por gravar e ainda sem commit, faz o seguinte:

- `IReportService.NeedsArticle` passa a `false`. «Stock por artigo» sem artigo deixa de mostrar «Escolha um artigo.»
- `LocalReportService`, quando `stock-article`, `stock-supplier` ou `stock-gain` não devolvem PDF, falha com `report-filter-no-records`. `ReportsWindow` mostra o alerta «Os filtros atuais não encontram registos…» em vez da página inglesa. Os outros relatórios vazios continuam na página inglesa.
- `ReportsWindow` troca as grelhas dos filtros por filas que só mostram os campos visíveis (`PackFilterRow`). O comentário no código diz que a grelha antiga deixava o diálogo branco e bloqueado.
- Atalhos «Dia», «Mês» e «Ano» por cima do período. O valor inicial continua a ser o mês actual.
- `ShowAsync` passa a apanhar a excepção, a escrever `ReportsWindow.ShowAsync` e a fechar a janela.
- No anfitrião, por gravar: `CloudReportService.cs` (+184) e `LocalQuestPdfReportService.cs` (+28). O ticket só tinha dito que o interceptar da cloud não estava commitado.

## Correção (fetch ao GitHub, 2026-11-23)

A resposta acima não viu o GitHub. O `git fetch` posterior mostrou ramos desse dia, ainda fora de `dev`, sem pull request.

No `logicpulse/logicPOS`:

- `fix/dados-2.0.35-r2` deixa uma só ocorrência de `COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS` em `db_seeds/pt/preferenceparameters.json` e acrescenta a migração `FixSeedTexts2035` para bases já criadas.
- `fix/ux-2.0.35-r3` troca o botão «Pag. Parcial» por «Pagamento Parcial» e «Reemb. Volta» por «Reembolso de depósito», com a dica «Reembolso de depósito / vasilhame». O texto de Community em `EditionSetupWindow.axaml` continua «sem certificação AT integrada». A emissão de recibos em `ReceiptEmissionView.axaml.cs` continua a seleccionar o primeiro método da lista.

No `logicpulse/logicPOS-internal`:

- `fix/instalador-2.0.35` arranca a app no fim com `UnelevatedProcess.TryStart` (`setup/logicPOS.Setup/UnelevatedProcess.cs`).
- `fix/grafia-ativa-internal` passa «série activa» a «série ativa» em pt-PT.
- `fix/textos-api-2.0.35` passa o TRV a «Talão de reembolso de depósito».
