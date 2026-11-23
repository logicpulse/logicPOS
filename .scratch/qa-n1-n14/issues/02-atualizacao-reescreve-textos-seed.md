# A atualização reescreve textos de semente numa base já criada?

Type: grilling
Status: resolved

## Question

Quando sai uma versão nova, a app deve reescrever numa base já criada os textos e dados de semente (designação dos métodos de pagamento, artigos de demonstração, parâmetros), ou esses textos ficam só para bases novas?

## Decision

Numa base já criada, a atualização só reescreve as alterações dessa versão. Não repõe a semente toda. Designações, artigos de demonstração e parâmetros que a versão não muda ficam como estão. Uma base nova continua a receber a semente completa.
