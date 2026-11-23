# Conta Corrente pode ser parcela de um pagamento misto?

Type: grilling
Status: resolved

## Question

«Conta Corrente» pode ser uma parcela de um pagamento misto e, se pode, essa parcela conta como liquidada no documento?

## Decision

Sim. Conta Corrente pode ser uma parcela do pagamento misto, e essa parcela conta como liquidada no documento.

Análise no código actual: a lista do pagamento misto já inclui Conta Corrente. A validação só exige que a soma das parcelas iguale o total. A parcela fica gravada como método de pagamento, com o valor indicado. Em FS e FR o documento fica pago na totalidade, incluindo essa parcela. Não fica um resto em aberto só da Conta Corrente.
