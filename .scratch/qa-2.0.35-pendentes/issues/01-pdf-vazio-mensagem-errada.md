# 01: PDF vazio com mensagem errada no desktop

Type: bug
Status: open

## Problema

Quando um relatório no desktop não tem registos, o PDF gerado mostra a linha «Sem dados no intervalo indicado.». O texto esperado é o da API: «Sem registos. Verifique os filtros.» (em `NoContentPdfModel`).

## Onde está

- Desktop (lógica local): `LogicPOS.Core/BackOffice/LocalReportService.cs`, método `BuildTablePdfAsync`, ~linha 119–122 — quando `rows.Count == 0`, adiciona `Row(("Informação", "Sem dados no intervalo indicado."))` e escreve o PDF na mesma.
- Referência API (mensagem correta): repositório `logicpulse/logicPOS-internal`, ficheiro `api/src/LogicPOS.Infrastructure/Features/Reports/Common/NoContentPdfModel.cs`, constante `Message = "Sem registos.\nVerifique os filtros."`. Este tipo **não** existe no repositório público `logicPOS`.
- Nota: os relatórios `stock-article`, `stock-supplier` e `stock-gain` sem linhas não passam por este PDF; devolvem `LocalReportService.NoRecordsError` (`report-filter-no-records`) e abrem o aviso de UI (ver ticket 02).

## Passos para reproduzir

1. Abrir Relatórios no BackOffice.
2. Escolher um relatório que não seja stock-article / stock-supplier / stock-gain e que, com os filtros actuais, não tenha linhas (ex.: período sem movimento).
3. Gerar o PDF e abri-lo.

## Resultado esperado

O PDF vazio usa a mesma mensagem que a API (`NoContentPdfModel`): «Sem registos. Verifique os filtros.»

## Capturas (Teste Local)

Pasta `/workspace/lp-ux-r4/capturas/` — prefixos `2bbacab2`, `609a171f`, `818db800`.
