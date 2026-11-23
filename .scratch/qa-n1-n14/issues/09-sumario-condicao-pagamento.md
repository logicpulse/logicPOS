# O que lista o sumário de vendas por condição de pagamento?

Type: grilling
Status: resolved

## Question

No separador «Financeiros (Sumário)», «Vendas por condição de pagamento» usa o mesmo corpo que «(Detalhado)»: uma linha por artigo, com código, designação e quantidade (`SalesByPaymentConditionReportPdfModel` percorre `sale.Items`, tal como o modelo detalhado).

O que deve listar o sumário, se deixa de repetir essa linha por artigo?

## Answer

Um sumário das linhas, agrupadas por condição de pagamento. Cada condição aparece uma vez, com os totais das linhas desse grupo. Não há linha por artigo, nem código, nem designação, nem quantidade de artigo.
