# O que o trabalho de hoje já fecha em N1–N14

Pergunta: quais dos itens N1–N14 já estão corrigidos no checkout `dev` (incluindo o diff por gravar de 2026-11-23), em `integrate/tickets-01-25` ou num ramo `ticket/*`, e quais continuam abertos?

Leitura em 2026-11-23 sobre o checkout público `C:\SVN\logicPOS-api-integration\logicPOS`, `HEAD` `9c774e15` («Remove the Android retail host from the public repository»), ramo `dev`. O commit `e4616a04` é antepassado deste `HEAD`. As junções de 2026-11-20 estão nessa história: não contam, por si, como correção posterior ao teste. N1 foi visto nessa árvore.

O diff por gravar no checkout público (não commitado) é só: `GLOSSARY.md`, `LogicPOS.App/App.axaml.cs`, `LogicPOS.App/Styles.axaml`, `LogicPOS.App/Views/BackOfficeWindow.axaml.cs`, `LogicPOS.App/Views/DashboardView.axaml.cs`, `LogicPOS.App/Views/PosWindow.axaml.cs`, `LogicPOS.App/Views/ReportsWindow.axaml`, `LogicPOS.App/Views/ReportsWindow.axaml.cs`, `LogicPOS.Core/BackOffice/IReportService.cs`, `LogicPOS.Core/BackOffice/LocalReportService.cs`, `LogicPOS.Core/MoneyFormat.cs`, mais `docs/adr/0002-inscricao-posto-cloud.md` por rastrear. Esse diff não toca seeds, métodos de pagamento, recibos, dicas, textos «activa» / «Imprimir», nem o diálogo Sistema → Atualizar. As linhas novas de `PosWindow.axaml.cs`, `BackOfficeWindow.axaml.cs` e `App.axaml.cs` não alteram MB Way, Conta Corrente, «Pag. Parcial» nem o texto do pedido de atualização.

`git rev-list --count dev..<ramo>` deu 0 para `integrate/tickets-01-25` e para todos os `ticket/*` locais (`07`, `09`, `10`, `12`, `13`, `14-15`, `16-17`, `18-19`, `20-21`, `22`, `23-24`, `25`, `lei-pt-01`, `lei-pt-08`) e para `fix/anomalias-backoffice`, `fix/anomalias-desktop`, `fix/desempenho-bd-15`. `integrate/tickets-01-25` está em `7a4084b8`, que já está na história de `dev`. Não há correção N1–N14 só num ramo ainda fora de `dev`.

O anfitrião foi lido em `C:\SVN\logicPOS-api-integration\logicPOS-internal` e não foi alterado. Aí, `client/src/LogicPOS.Cloud/CloudReportService.cs` tem alterações por gravar (182 linhas). Esse diff não faz parte do checkout público e não está commitado no anfitrião: `git grep "No Content" HEAD -- client/src/LogicPOS.Cloud/CloudReportService.cs` não devolve nada.

## Contagem

Nenhum item N1–N14 fica fechado por completo. Os catorze continuam abertos no checkout que o tester veria mais o diff de hoje. Zero ficam só noutro ramo.

O diff de hoje mexe em dois sítios sem fechar o item: deixa de exigir artigo em «Stock por artigo», e mostra um alerta em português quando três relatórios de stock devolvem o token `report-filter-no-records`.

## N1 — chave repetida na semente de Portugal

Ainda aberto, commitado em `dev`.

`db_seeds/pt/preferenceparameters.json` tem o token `COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS` duas vezes, com o mesmo `Id` `b4e8c2a1-7f3d-4e91-9c6a-2d8f5e0b1a43` (linhas 458–504). `git grep` nesse ficheiro no `HEAD` só encontra essas duas linhas. Os outros `preferenceparameters.json` da pasta `db_seeds` não repetem o token.

O par entrou em `e6eb94f0` (`feat(pos): transport guide circulation requires AT code when company marked`, 2026-11-20): o commit acrescenta 48 linhas a esse ficheiro e, nesse commit, o token já ocorre duas vezes. `e6eb94f0` é antepassado de `e4616a04` e de `9c774e15`. Nenhum commit posterior apaga a segunda cópia. O diff de hoje não toca no ficheiro.

`PreferenceParametersSeeder.SeedData` faz `AddRange` da lista inteira e `SaveChanges` (`src/Persistence/LogicPOS.Persistence/Entities/System/PreferenceParameters/PreferenceParametersSeeder.cs`, linhas 31–32). A entidade base usa `Id` como identidade (`src/LogicPOS.Domain/Entities/Common/Entity/Entity.cs`, linha 5). Duas linhas com o mesmo `Id` não podem ser gravadas. O seeder apanha a exceção e devolve falso (linhas 34–37). Numa base nova, `DatabaseInitializer.Initialize` trata isso como falha da semente obrigatória (`src/Persistence/LogicPOS.Persistence/Services/DatabaseInitializer.cs`, linhas 59–63). `DatabaseStartup.EnsureSeed` também desiste se `ApplyRequiredSeed` devolve falso (`LogicPOS.Core/DatabaseStartup.cs`, linhas 324–327), quando ainda não há parâmetros.

## N2 — texto de primeiro arranque da Community

Ainda aberto, commitado em `dev`.

`LogicPOS.App/Views/EditionSetupWindow.axaml` diz, na linha 133, «A Community inclui faturação local sem certificação AT integrada; Business e Cloud acrescentam certificação LogicPulse.» A linha 166 repete «sem certificação AT integrada» no cartão Community. Esse texto entrou em `d5195946` (`fix(edition): Community first-run copy…`, 2026-11-20), já em `dev`. O diff de hoje não altera o ficheiro.

Procura em `C:\SVN\logicPOS-api-integration\logicPOS-internal` por «sem certificação AT», «Faturação certificada» e `EditionSetup`: sem ocorrências. O anfitrião não tem outro cartão. A mesma janela serve as duas edições. O glossário já fechado neste mapa diz que só a edição pública fica sem certificação AT e que, na LogicPulse, a Community tem certificação. O código não separa as duas frases.

## N3 — MB Way no lugar do Multibanco

Ainda aberto. A troca de nome está commitada em `dev` e não chega a uma base já criada.

`db_seeds/paymentmethods.json`, token `CASH_MACHINE`, acrónimo `MB`, designação «MB Way», `Id` `39ddd898-3be3-4d74-bfd2-5163ebeb8f21` (linhas 149–159). Não há segunda linha Multibanco. `72ad7b67` (`feat(pos): MB Way settlement…`, 2026-11-20) muda a designação de «MultiBanco» para «MB Way» e mantém o acrónimo `MB`.

O FrontOffice escreve o nome à mão: `LogicPOS.App/Views/PaymentWindow.axaml.cs`, linhas 58 e 154, `("CASH_MACHINE", "MB Way")`. O BackOffice lista a entidade `PaymentMethod` (`LogicPOS.Core/BackOffice/BackOfficeListingService.cs`, linha 4062) e mostra a designação gravada na base.

`PaymentMethodsSeeder.SeedData` só faz `AddRange` (`src/Persistence/LogicPOS.Persistence/Entities/Finance/PaymentMethods/PaymentMethodsSeeder.cs`, linhas 28–29). Não há atualização de designação. Numa base que já existia, `DatabaseInitializer` chama `ApplyNewSeed` (`DatabaseInitializer.cs`, linhas 45–48). `ApplyNewSeed` só acrescenta parâmetros de preferência, permissões e tipos de documento novos (`DatabaseSeeder.cs`, linhas 180–211). Não relê `paymentmethods.json`. Quem atualiza mantém «MultiBanco» no BackOffice; o FrontOffice mostra «MB Way» na mesma.

O que falta decidir (um método renomeado, ou Multibanco e MB Way como dois métodos) não está no glossário.

## N4 — método por omissão do recibo

Ainda aberto, commitado em `dev`. O glossário deste mapa já diz Numerário. O código escolhe outro.

`LocalReceiptEmission.CatalogAsync` ordena métodos por `Order` e depois por designação (`LogicPOS.Core/BackOffice/LocalReceiptEmission.cs`, linhas 110–114). Em `db_seeds/paymentmethods.json`, «Cartão de Crédito» tem `Order` 10 (linhas 3–10) e «Numerário» tem `Order` 90 (linhas 171–178). `ReceiptEmissionView.axaml.cs` seleciona o primeiro da lista (linhas 339–340). O diff de hoje não altera estes ficheiros. `4a21bcfe` («default Numerário») está em `dev` e não muda esta seleção.

## N5 — textos de semente numa base já criada

Ainda aberto. As correções de texto estão nos JSON commitados em `dev` e o caminho de atualização não as reescreve.

`a1d1335f` (`fix(ui): PT-PT copy, listing tooltips and demo seed accents`, 2026-11-20) altera `db_seeds/paymentmethods.json` e os artigos de `db_seeds/modules/cafe/articles.json` e `db_seeds/modules/restaurant/articles.json`. Exemplo no café: «Sopa De Peixe» passa a «Sopa de Peixe». A designação atual de dinheiro eletrónico na semente é «Dinheiro Eletrónico» (`db_seeds/paymentmethods.json`, linha 114). O mesmo commit é o que introduz essa grafia no ficheiro (`git log -S "Dinheiro Eletrónico"`).

`ArticlesSeeder.SeedData` só faz `AddRange` (`src/Persistence/LogicPOS.Persistence/Entities/Articles/Articles/ArticlesSeeder.cs`, linhas 28–29) e só corre em `ApplyAdditionalSeed`, que o inicializador reserva à base nova (`DatabaseInitializer.cs`, linhas 66–68). `ApplyNewSeed` não atualiza artigos nem métodos de pagamento. `PreferenceParametersSeeder.SeedNewData` insere tokens que ainda não existem e ignora os que já estão (`PreferenceParametersSeeder.cs`, linhas 67–73). Não reescreve o valor antigo.

Se uma atualização deve reescrever estes textos numa base já criada não está decidido no glossário.

## N6 — Conta Corrente no pagamento misto

Ainda aberto, commitado em `dev`.

O combo do misto inclui Conta Corrente: `PaymentWindow.axaml.cs`, linhas 55–62 (`CURRENT_ACCOUNT`) e 1077–1085 (a lista inteira vai para o `ComboBox`). Na confirmação, cada parcela, incluindo essa, segue como `PosDocumentPaymentMethod` (linhas 864–871). `PosDocumentService` grava todas as parcelas que têm valor (por volta das linhas 1095–1118) e, no misto, manda `TotalDelivery` nulo (linha 1188). `DocumentCreator` trata nulo como zero (`src/LogicPOS.Domain/Entities/Finance/Documents/Documents/Create/DocumentCreator.cs`, linha 54). `Document.CalculateTotals`, quando o tipo exige pagamento integral e `TotalDelivery` é 0, faz `Paid = true` (`Document.Behavior.cs`, linhas 36–46). `PosMixedPaymentValidation` só confere se a soma das parcelas iguala o total (`LogicPOS.Core/FrontOffice/PosMixedPaymentValidation.cs`, linhas 10–33). Não exclui Conta Corrente.

O glossário fecha a soma das parcelas. Não diz se Conta Corrente pode ser uma delas, nem se essa parcela conta como liquidada.

## N7 — dicas «Pag. Parcial» e «Reemb. Volta»

Ainda aberto, commitado em `dev`. `a1d1335f` é o commit das dicas e já está na árvore testada.

`ListingLabelTips` regista `["Pag.Parcial"] = "Pagamento parcial"` e `["Reemb. Volta"] = "Reembolso de volta"` (`LogicPOS.Core/BackOffice/ListingLabelTips.cs`, linhas 22–24). A procura é ordinal, com o texto exacto (`FullTerm`, linhas 40–47). O botão visível diz «Pag. Parcial», com espaço (`LogicPOS.App/Views/PaymentWindow.axaml`, linha 63). O resx PT diz «Pag.Parcial» (`LogicPOS.Globalization/Localization/Resx.pt-PT.resx`, linhas 981–982, `global_button_label_payment_dialog_partial_payment`). A chave não coincide com o rótulo do botão, por isso `Tooltip` devolve o próprio rótulo curto.

«Reemb. Volta» coincide com o botão (`PosWindow.axaml.cs`, linhas 791 e 809). No modo só ícone, a dica é `ListingLabelTips.Tooltip(text)` (linha 2011), ou seja «Reembolso de volta». O que essa dica deve explicar não está no glossário.

## N8 — três textos

Ainda aberto.

- «Nenhuma série activa para emissão de recibos foi encontrada.» em `LogicPOS.Core/BackOffice/LocalReceiptEmission.cs`, linha 205. A mesma grafia «série activa» está na linha 276 desse ficheiro e em `LogicPOS.App/Views/PaymentWindow.axaml.cs`, linha 909.
- «Pergunta se deseja Imprimir documento» em `LogicPOS.Globalization/Localization/Resx.pt-PT.resx`, linhas 1931–1932 (`global_print_request_confirmation`), e o mesmo texto em `BackOfficeListingService.cs`, linha 225.
- No posto local, a abertura do dia grava «Dia » mais a data (`BackOfficeListingService.cs`, linha 2864). No anfitrião, a API cloud grava `Day [{DateTime.Now}]` (`logicPOS-internal/api/src/LogicPOS.Api/Features/POS/WorkSessions/Periods/OpenDay/OpenDayCommandHandler.cs`, linha 37) e a sessão de terminal grava `Session [{DateTime.Now}]` (`…/OpenTerminal/OpenTerminalCommandHandler.cs`, linha 62). Procura de «Day [» no repositório público: não aparece. O «Day» que a lista cloud pode mostrar vem do anfitrião, e continua lá.

O diff de hoje não altera estes textos. São erros de cópia com o comportamento esperado já enunciado na lista (grafia e maiúscula). Não são uma decisão em aberto.

## N9 — a app que o instalador abre corre como administrador

Ainda aberto, fora do repositório público.

O manifesto da app pública `LogicPOS.App/app.manifest` não pede `requireAdministrator`. O instalador pede: `logicPOS-internal/setup/logicPOS.Setup/app.manifest`, linha 8, `level="requireAdministrator"`. No fim, `MainWindow.xaml.cs` arranca o executável do POS com `Process.Start` e `UseShellExecute = true`, sem baixar o privilégio (linhas 1968–1973 e, no arranque silencioso, linhas 2058–2063). O processo filho de um instalador elevado fica elevado. Não há ramo público que mude isto.

## N10 — Gerar sem cliente ou sem artigo

Ainda aberto no checkout com o diff de hoje. O aviso de cliente está commitado; o aviso de artigo o diff de hoje retira-o.

Catálogo (`LogicPOS.Core/BackOffice/ReportCatalog.cs`): «Saldo de clientes (detalhe)» é `customer-balance-details` com `DateRangeCustomerRequired` (linha 39); «Stock por artigo» é `stock-article` com `StockArticle` (linha 69).

No `HEAD` commitado, `ReportDefinition.NeedsCustomer` é verdadeiro para `DateRangeCustomerRequired`, e `NeedsArticle` é verdadeiro para `StockArticle` (`git diff` de `IReportService.cs` mostra a linha antiga `NeedsArticle => FilterProfile is ReportFilterProfile.StockArticle`). `LocalReportService.GenerateAsync` devolve «Escolha um cliente.» quando falta o cliente (linhas 49–52 no ficheiro de trabalho, já presentes antes do diff). O mesmo par de avisos está em `logicPOS-internal/client/src/LogicPOS.Cloud/CloudReportService.cs` nas linhas 42–49 da cópia de trabalho; a verificação de artigo depende de `NeedsArticle`.

O diff por gravar de `IReportService.cs` passa `NeedsArticle` a `false` (linha 56 do ficheiro de trabalho). Com isso, «Stock por artigo» sem artigo deixa de avisar. `LocalQuestPdfReportService.GetStockByArticleDataAsync` trata artigo vazio como todos os artigos (`logicPOS-internal/api/src/LogicPOS.Infrastructure/Features/Reports/Local/LocalQuestPdfReportService.cs`, linhas 1873–1880). Se a lista vier vazia, `TryGenerateAsync` devolve nulo (linhas 302–305) e o `LocalReportService` de hoje devolve o token `report-filter-no-records` (linhas 86–88). A janela mostra então o alerta em português (`ReportsWindow.axaml.cs`, `ShowNoRecordsAlert`). Se houver artigos, Gerar produz o PDF de todos.

O que Gerar deve fazer sem cliente ou sem artigo não está no glossário. O sintoma «não faz nada e não avisa» não descreve o aviso de cliente, que o código commitado já emite (`ReportsWindow.axaml.cs`, `SetActiveNotice` quando `GenerateAsync` falha).

## N11 — página vazia em inglês

Ainda aberto.

A página é `logicPOS-internal/api/src/LogicPOS.Infrastructure/Features/Reports/Common/NoContentPdfModel.cs`, linha 20: «No Content.\n Check the Filters». `CurrentAccountPdfGenerator.GeneratePdf` devolve essa página quando não há movimentos (linhas 11–13). O mesmo padrão está nos outros geradores de PDF de relatório dessa pasta, incluindo `StockByArticleReportPdfGenerator`.

No posto local, `LocalQuestPdfReportService.TryGenerateAsync` para `customer-balance-details` devolve sempre os bytes desse PDF (linhas 246–248). `LocalReportService` só substitui nulo pelo token `report-filter-no-records` nas chaves `stock-article`, `stock-supplier` e `stock-gain` (diff de hoje, linhas 86–88). Nos outros relatórios, os bytes seguem para um PDF temporário e a janela abre-o.

O diff por gravar de `ReportsWindow.axaml.cs` mostra um alerta em português quando o erro é exactamente esse token (`ShowNoRecordsAlert`). Não lê o PDF inglês. No anfitrião, o interceptar de «No Content» em `CloudReportService.cs` (por volta das linhas 86–132 da cópia de trabalho) está só no diff por gravar: não está no `HEAD` do anfitrião. A build testada, sem esse diff e sem o alerta novo da janela, abre a página inglesa.

## N12 — período por omissão dos relatórios

Ainda aberto. O mês actual está commitado em `dev` e o diff de hoje não o troca.

`ReportsWindow` arranca o período no dia 1 do mês até ao fim de hoje (`ReportsWindow.axaml.cs`, linhas 33–35; `git blame` aponta `5aeabd6c`, 2026-11-03). `ResetFilterDefaults` repete o mesmo intervalo (linhas 357–361), commit `cb12112d` de 2026-11-20 («Default report filters to Todos and the current month»), já em `dev`. O diff de hoje acrescenta botões «Dia», «Mês» e «Ano» (`ReportsWindow.axaml`, linhas 101–115) e marca «Mês» quando o intervalo já é o mês actual. O valor inicial continua a ser o mês actual. Qual deve ser o período por omissão não está no glossário.

## N13 — Sessões de Trab. com 0 registos em 2026 na cloud

Ainda aberto como comportamento. A causa da lista vazia está no código do anfitrião; não é um filtro de ano na consulta.

Na cloud, `CloudListingService.LoadAsync` para «Sessões de Trab.» chama só `GetOpenTerminalSessionsQuery` (`logicPOS-internal/client/src/LogicPOS.Cloud/CloudListingService.cs`, linha 848). O aviso da página é «Sessões de terminal abertas.» (linha 192). A API filtra `Status == Open` e `Type == Terminal` (`logicPOS-internal/api/src/LogicPOS.Api/Features/POS/WorkSessions/Periods/GetOpenTerminalSessions/GetOpenTerminalSessionsQueryHandler.cs`, linha 19). Vendas não entram nesta consulta. Sessões fechadas e o dia também não.

A grelha aplica depois o filtro de datas da listagem. «Sessões de Trab.» usa esse filtro (`LogicPOS.App/Views/EntityListing.axaml.cs`, linha 260). O intervalo por omissão é 1 de Janeiro do ano corrente até ao fim de hoje (linhas 337–341). Em 2026 isso é o ano de 2026. `InDateRange` (linhas 863–894) esconde a linha se uma data reconhecida cair fora do intervalo. Se a API não devolver sessões de terminal abertas, a lista fica a 0 antes desse filtro. O posto local, pelo contrário, materializa `WorkSessionPeriod` sem este recorte de «só abertas» (`BackOfficeListingService.QueryEntitiesAsync`).

O que a lista cloud deve mostrar (só terminais abertos, ou o período filtrado) não está no glossário. O «porque é que dá 0 havendo vendas» deixa de ser uma causa desconhecida: a consulta não lista vendas nem sessões fechadas.

## N14 — Sistema → Atualizar

Ainda aberto, no repositório público. O diff de hoje não altera o diálogo.

O menu chama `ShowUpdate` (`LogicPOS.App/Views/BackOfficeWindow.axaml.cs`, linhas 1054–1056 e 1746–1753), que torna visível o overlay sem ler `version.txt` nem comparar versões. O texto está em `BackOfficeWindow.axaml`, linha 250: «Descarregar a atualização e reiniciar? A configuração (appsettings) e a base de dados local mantêm-se.» Não nomeia uma versão. A comparação com a versão publicada só acontece depois do Sim, dentro de `ApplicationUpdater.DownloadAndScheduleAsync` (`LogicPOS.App/ApplicationUpdater.cs`, linhas 22–32), que lê `VersionUrl` já durante o download. O fluxo pedido pela lista (verificar se há versão nova e dizer qual é, antes da pergunta) não está neste caminho.

## O que não é ticket novo

Erros cujo comportamento esperado já está na lista ou no glossário, e que continuam no código: N1 (chave repetida), N2 (uma frase só, contra o glossário), N4 (recibo em Cartão de Crédito), N7 na chave `Pag.Parcial` sem espaço, N8 (activa, Imprimir, Day no anfitrião), N9 (filho do instalador elevado), N11 (página «No Content. Check the Filters»), N14 (pergunta sem versão). Ficam à espera do spec. Não são decisões por abrir.

Não há ticket quando o glossário e o código já dizem o mesmo. Aqui o glossário fecha Numerário no recibo, a soma das parcelas e MB Way sem confirmação no telemóvel. O recibo no código não está em Numerário (N4). A soma das parcelas está implementada e não se reabre. A confirmação MB Way no telemóvel não foi reaberta: o código de liquidação não pede telefone neste caminho, e o glossário já o fecha.

## Decisões que a leitura deixa em aberto

- Se a atualização reescreve textos de semente numa base já criada (N5).
- Se MB Way substituiu o Multibanco ou se são dois métodos (N3).
- Se Conta Corrente pode ser parcela de um pagamento misto e se essa parcela fica liquidada (N6).
- O que Gerar faz sem cliente ou sem artigo (N10).
- Qual é o período por omissão dos relatórios (N12).
- O que a dica «Reemb. Volta» deve explicar (N7).
- Se a lista cloud de Sessões de Trab. deve ser o período filtrado ou só sessões de terminal abertas (N13). A causa do zero está escrita acima.
