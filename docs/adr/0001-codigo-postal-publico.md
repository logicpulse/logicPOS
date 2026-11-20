# Código postal só de Portugal, via API pública

A edição pública tem de sugerir moradas sem levar um catálogo no repositório nem na base. A sugestão de Portugal usa a GEO API PT. Angola e Moçambique não sugerem. Sem rede, com código desconhecido ou fora de Portugal, não há mensagem e a morada escreve-se à mão.

O código completo (`XXXX-XXX`) preenche localidade e cidade, e a rua só se a morada estiver vazia. Um prefixo de quatro dígitos não preenche sozinho: abre uma lista quando alguém a pede. A consulta não corre a cada letra.

Ficaram de fora o catálogo local (`postallocalities.json`, `postalstreets.json` e a tabela `PostalLocalities`), uma API com chave para os três países, e manter os JSON de Angola e Moçambique à espera de uma fonte. Angola não tem sistema público de código postal. Moçambique tem códigos nos Correios, sem API pública sem chave que devolva rua e localidade. A GEO API PT não pede chave e limita a 900 pedidos por IP em 15 minutos; por isso a consulta espera pelo código completo ou por um pedido explícito.
