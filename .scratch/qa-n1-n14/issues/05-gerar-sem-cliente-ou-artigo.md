# O que faz Gerar sem cliente ou sem artigo?

Type: grilling
Status: resolved

## Question

Em «Saldo de clientes (detalhe)» sem cliente e em «Stock por artigo» sem artigo, o que deve fazer «Gerar»: recusar com aviso, ou emitir o relatório com todos os clientes ou todos os artigos?

## Decision

Gerar recusa quando o filtro fica vazio. O filtro por defeito já tem «Todos» seleccionado, e «Todos» não é vazio: Gerar com esse defeito emite o relatório para todos os clientes ou todos os artigos.
