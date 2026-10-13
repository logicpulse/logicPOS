# LogicPOS

O produto de ponto de venda e back-office. A edição pública é o que o GitHub publica. A edição LogicPulse vive fora desse repositório.

## Language

**Edição pública**:
A edição publicada no GitHub `logicPOS`: só base de dados direta, sem ligação à API, sem certificação e sem registo de licença, com um encaixe para uma certificação de terceiros.
_Avoid_: versão free, versão GitHub, open source

**Edição LogicPulse**:
O anfitrião fechado em `logicPOS-internal` que referencia a Avalonia pública e acrescenta a composição própria: modo local (licença + fiscal) ou modo LogicPulse (API). O instalador 1.6 publica este anfitrião.
_Avoid_: versão paga, a mesma app com um DLL de cloud, edição pública

**Certificação**:
A comunicação com a AT e a AGT, e o que só existe por causa dela: ATCUD, séries certificadas, SAF-T, hash, QR e a linha do programa certificado no documento.
_Avoid_: licença, cloud, ligação à API, registo

**Encaixe de certificação**:
O contrato público para um terceiro (ou a LogicPulse) trazer a certificação: uma DLL com `AddFiscal` que regista um `IFiscalModule`. Sem DLL, o posto fatura sem marcas fiscais. A LogicPulse leva `LogicPOS.Fiscal.dll` ao lado da app.
_Avoid_: cloud, licença, plugin de cloud

**Número da fatura**:
A identificação do documento na edição pública: a sigla da série e o próximo número (`sigla/n`).
_Avoid_: ATCUD, código de série certificada, número local

**Instalador**:
O instalador Windows da edição LogicPulse, com certificação. A edição pública não tem instalador. Por defeito instala em `C:\Program Files\Logicpulse\logicpos`.
_Avoid_: setup opensource, instalador do GitHub

**Atualização da 1.6**:
Substituir os programas e as seeds de uma 1.6 já instalada, mantendo o appsettings, o `logicpos.licence`, a base SQLite dessa pasta e os Logs. O assistente não volta a perguntar.
_Avoid_: migração da 1.4

**Modo local**:
A instalação LogicPulse com base de dados direta e certificação neste posto.
_Avoid_: edição pública, sem certificação, API local

**Modo LogicPulse**:
A instalação LogicPulse ligada à API de produção. A certificação fica nessa API.
_Avoid_: cloud, API local, catálogo SQL de contas

**Modo de operação**:
O ramo de negócio da instalação local, como restauração, café ou padaria. No modo LogicPulse vem da API e o instalador não pergunta.
_Avoid_: vertical, cloud, módulo da API

**Base de dados de demonstração**:
Os dados iniciais do modo de operação. Só entram se quem instala os aceitar.
_Avoid_: migração da 1.4, base vazia

**Client ID**:
O identificador com que o posto se apresenta à API no modo LogicPulse. Vem preenchido e pode ficar vazio.
_Avoid_: licença, número de série

**Ficheiro de licença**:
O `logicpos.licence` que a instalação local descarrega do ERP quando já existe uma licença válida para o hardware id desta máquina. Sem ele a app abre, mas a impressão e a geração de documentos ficam inativas.
_Avoid_: DLL, logicpos.license

**Hardware id**:
O identificador da máquina no formato `XXXX-XXXX-XXXX-XXXX-XXXX-XXXX`, gerado como o GTK já pedia à API.
_Avoid_: IntelliLock, MachineGuid

**Migrations**:
A atualização do esquema da base. O modelo é um só, em `logicPOS/src`. Cada motor (SQLite, MySQL, SQL Server) tem o seu projeto de migrations, e a app e a API usam esses mesmos projetos. No modo local a app aplica-as ao arrancar. No modo LogicPulse correm na API. O instalador não as corre.
_Avoid_: migração da 1.4, migrador, modelo da API

**Migração da 1.4**:
A cópia dos dados de uma instalação 1.4 para uma base nova do modo local. A base antiga fica como origem. Não corre no modo LogicPulse.
_Avoid_: migração de esquema, API, escrever por cima da base 1.4

**Passagem da 1.5**:
Desinstalar a 1.5 sem mostrar o assistente. Idioma, cultura e modo de operação vêm dessa instalação. A base SQLite copia-se para a pasta raiz da instalação nova. SQL Server e MySQL ficam no servidor, com a mesma ligação.
_Avoid_: migração da 1.4, atualização da 1.6

**Base SQLite**:
O ficheiro da base fica sempre na pasta raiz da instalação, `C:\Program Files\Logicpulse\logicpos`.
_Avoid_: ProgramData, pasta da API
