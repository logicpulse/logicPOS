---
type: grilling
blocked_by: [02]
Status: resolved
---

# Contrato do encaixe de certificação

## Question

Qual é o contrato mínimo do encaixe de certificação (`plugins\*Plugin.dll`) para um terceiro certificar sem o público conhecer a AT ou a AGT, e o que o público faz quando esse encaixe está vazio? A cloud e a licença não têm contrato no público.

## Answer

O contrato mínimo é o que já existe: uma DLL com `AddFiscal` que regista um `IFiscalModule` (`DescribePrint`, SAF-T, séries, anulação). O público não conhece AT nem AGT. Cloud e licença não têm contrato no público.

Com o encaixe vazio, o posto abre e fatura sem ATCUD, QR fiscal, hash nem rodapé de programa certificado. Os ecrãs de SAF-T e de comunicação de séries dizem que a certificação não está instalada. O arranque não bloqueia.

A LogicPulse põe a sua DLL ao lado da app, como `LogicPOS.Fiscal.dll`. O instalador 1.6 copia-a com o anfitrião.
