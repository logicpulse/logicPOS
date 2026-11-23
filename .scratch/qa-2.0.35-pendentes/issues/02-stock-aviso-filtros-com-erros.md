# 02: Aviso de stock sem registos com erros de escrita

Type: bug
Status: open

## Problema

Em «Stock por artigo» (e nos outros stock filtrados), com «Todos» e sem registos, o desktop mostra o aviso:

> Os filtros atuais não encontram registos com essas condições, reformule os fitros e tente novamente com outra condições!

Há erros de escrita («fitros», «outra condições»). O texto esperado é o mesmo da mensagem vazia da API: «Sem registos. Verifique os filtros.»

## Onde está

- UI: `LogicPOS.App/Views/ReportsWindow.axaml.cs`, método `ShowNoRecordsAlert` (~linhas 561–568) — lê a chave Resx `dialog_message_report_filter_no_records_with_criteria` via `PreferenceLabels.Text`.
- Disparo: `LocalReportService.GenerateAsync` (~linhas 86–88) — para `stock-article`, `stock-supplier` e `stock-gain`, se o PDF Quest branded não tiver bytes, devolve `Fail(NoRecordsError)` (`report-filter-no-records`); `GenerateSelectedAsync` compara com `LocalReportService.NoRecordsError` e chama `ShowNoRecordsAlert`.
- Chave Resx: `dialog_message_report_filter_no_records_with_criteria` (constante `ResourceNames.DIALOG_MESSAGE_REPORT_FILTER_NO_RECORDS_WITH_CRITERIA`).
- Culturas com a mesma chave (valores actuais):
  - `LogicPOS.Globalization/Localization/Resx.pt-PT.resx` — texto com «fitros» / «outra condições»
  - `Resx.pt-AO.resx` — idem (registos)
  - `Resx.pt-MZ.resx` — idem (registos)
  - `Resx.pt-BR.resx` — idem (registros)
  - `Resx.resx` / `Resx.en-GB.resx` / `Resx.en-US.resx` — EN «Current filter has no records…»
  - `Resx.es-ES.resx` — ES
  - `Resx.fr-FR.resx` — FR
  - `Resx.de-DE.resx` / `Resx.de-CH.resx` — DE (de-CH tem ainda `dialog_message_report_filter_no_records_with_criteria1`)

## Passos para reproduzir

1. Abrir Relatórios → «Stock por artigo».
2. Deixar artigo em «Todos» (ou filtros que não devolvam linhas).
3. Gerar — aparece o diálogo de informação com a mensagem longa.

## Resultado esperado

O aviso usa «Sem registos. Verifique os filtros.» (alinhado a `NoContentPdfModel` na API), sem os erros de escrita do Resx actual.

## Capturas (Teste Local)

Pasta `/workspace/lp-ux-r4/capturas/` — prefixos `2bbacab2`, `609a171f`, `818db800`.
