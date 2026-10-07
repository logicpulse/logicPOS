# LogicPOS

O produto de ponto de venda e back-office. A edição pública é o que o GitHub publica. A edição LogicPulse vive fora desse repositório.

## Language

**Edição pública**:
A edição publicada no GitHub `logicPOS`: só base de dados direta, sem ligação à API, sem certificação e sem registo de licença, com um encaixe para uma certificação de terceiros.
_Avoid_: versão free, versão GitHub, open source

**Edição LogicPulse**:
O anfitrião fechado que usa a edição pública e acrescenta a ligação à API, a certificação LogicPulse e o registo de licença.
_Avoid_: versão paga, a mesma app com um DLL de cloud, versão interna

**Certificação**:
A comunicação com a AT e a AGT, e o que só existe por causa dela: ATCUD, séries certificadas, SAF-T, hash, QR e a linha do programa certificado no documento.
_Avoid_: licença, cloud, ligação à API, registo

**Número da fatura**:
A identificação do documento na edição pública: a sigla da série e o próximo número (`sigla/n`).
_Avoid_: ATCUD, código de série certificada, número local
