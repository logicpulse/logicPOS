# LogicPOS

O produto de ponto de venda e back-office. A edição pública é o que o GitHub publica. A edição LogicPulse vive fora desse repositório.

## Language

**Edição pública**:
A edição publicada no GitHub `logicPOS`: só base de dados direta, sem ligação à API, sem certificação e sem registo de licença, com um encaixe para uma certificação de terceiros. A build develop da lista 2.0.31 é esta edição: sem a DLL, séries por comunicar à AT e a faixa de módulo em falta são o estado esperado. O cartão de primeiro arranque não promete faturação certificada.
_Avoid_: versão free, versão GitHub, open source, Community

**Edição LogicPulse**:
O anfitrião fechado em `logicPOS-internal` que referencia a Avalonia pública e acrescenta a composição própria: modo local (licença + fiscal) ou modo LogicPulse (API). O instalador publica este anfitrião, com certificação em Community, Business e Cloud.
_Avoid_: versão paga, a mesma app com um DLL de cloud, edição pública

**Community**:
A escolha de primeiro arranque que deixa o posto em base local, sem download, sem Client ID e sem ficheiro de licença. Na edição pública não tem certificação. Na edição LogicPulse tem, porque a DLL fiscal vem com a app.
_Avoid_: edição pública, versão GitHub, versão grátis

**Certificação**:
A comunicação com a AT e a AGT, e o que só existe por causa dela: ATCUD, séries certificadas, SAF-T, hash, QR e a linha do programa certificado no documento. Está em todas as escolhas da edição LogicPulse. Não está na edição pública.
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
Substituir os programas e as seeds de uma 1.6 já instalada, mantendo o appsettings, o `logicpos.licence`, a base SQLite dessa pasta e os Logs. A edição não é inventada; a página de escolha aparece se ainda não existir.
_Avoid_: migração da 1.4

**Modo local**:
A instalação LogicPulse com base de dados direta e certificação neste posto.
_Avoid_: edição pública, sem certificação, API local

**Cloud**:
A ligação à API de produção da LogicPulse (`https://api.logic-pos.com/`) com Client ID. Não instala API local.
_Avoid_: modo API, API no PC do cliente

**Modo API**:
O posto Avalonia ligado a uma API local que já existe, sem Client ID. Não descarrega API nem webapp neste PC.
_Avoid_: Cloud, API de produção

**Modo LogicPulse**:
Termo antigo para a ligação à API de produção. Preferir Cloud.
_Avoid_: usar em texto novo; ver Cloud

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
A atualização do esquema da base. O modelo é um só, em `logicPOS/src`. Cada motor (SQLite, MySQL, SQL Server, PostgreSQL) tem o seu projeto de migrations, e a app e a API usam esses mesmos projetos. No modo local a app aplica-as ao arrancar. No modo LogicPulse correm na API. O instalador não as corre.
_Avoid_: migração da 1.4, migrador, modelo da API

**Migração da 1.4**:
A cópia dos dados de uma instalação 1.4 para uma base nova do modo local. A base antiga fica como origem. Não corre no modo LogicPulse.
_Avoid_: migração de esquema, API, escrever por cima da base 1.4

**Passagem da 1.5**:
Desinstalar a 1.5 sem mostrar o assistente. Idioma, cultura e modo de operação vêm dessa instalação. A base SQLite copia-se para a pasta raiz da instalação nova. SQL Server, MySQL e PostgreSQL ficam no servidor, com a mesma ligação.
_Avoid_: migração da 1.4, atualização da 1.6

**Base SQLite**:
O ficheiro da base fica sempre na pasta raiz da instalação, `C:\Program Files\Logicpulse\logicpos`.
_Avoid_: ProgramData, pasta da API

**Email (vários endereços)**:
No campo Email de Clientes/Utilizadores (e no envio de documentos) podem ir vários endereços na mesma string, separados por `;` ou `,` (com ou sem espaços). Ex.: `a@b.com; c@d.com`.
_Avoid_: um campo por email, só um endereço válido

**Cliente**:
A entidade comercial ligada a um documento: identificação, NIF, morada e contactos.
_Avoid_: account, buyer, client genérico

**PIN**:
O segredo numérico de entrada no posto.
_Avoid_: palavra-passe, password

**Palavra-passe do portal**:
O segredo do portal, distinto do PIN do posto.
_Avoid_: password, PIN

**Código-postal (auxiliar)**:
Registo de referência Código-postal → Localidade/Cidade dum País; serve para preencher a morada do Cliente, não substitui a linha de rua.
_Avoid_: catálogo de moradas, CTT completo, street directory

**Localidade (campo)**:
Texto da localidade na morada do Cliente; pode ser preenchido a partir do Código-postal auxiliar.
_Avoid_: cidade como sinónimo exclusivo; tabela de ruas

**Consumidor final**:
O cliente de sistema usado quando não há cliente comercial específico; não se edita nem apaga na listagem nem no documento.
_Avoid_: anónimo, cliente vazio

**Documento novo**:
O ecrã de emissão de um documento (Avalonia NewDocument / portal Novo Doc.).
_Avoid_: invoice wizard, formulário de fatura genérico

**Ordem aberta / Pedido de mesa**:
O ticket de cozinha (ou retalho) gravado numa mesa enquanto o serviço corre: linhas finalizadas com Fin.pedido, ainda sem documento fiscal. Em “Ver pedidos” lista-se e pode eliminar-se uma ou todas as ordens abertas da mesa.
_Avoid_: fatura, FS, FR, FT, conta paga, documento fiscal

**Documento fiscal**:
O documento com série que substitui a ordem aberta: fatura simplificada (FS), fatura-recibo (FR) ou fatura (FT). Não é a ordem aberta da mesa.
_Avoid_: pedido de mesa, ticket de cozinha, Fin.pedido, pagamento (a FT não é um pagamento)

**Fatura simplificada (FS)**:
Documento fiscal de venda imediata ao consumidor final, liquidado na emissão.
_Avoid_: fatura, fatura-recibo, conta corrente

**Fatura-recibo (FR)**:
Documento fiscal de venda imediata a um cliente com nome e NIF, liquidado na emissão.
_Avoid_: fatura, fatura simplificada, recibo à parte, conta corrente

**Fatura (FT)**:
Documento fiscal emitido sem liquidação. Fica em dívida até um recibo.
_Avoid_: fatura-recibo, fatura simplificada, pagamento efetuado

**Dívida**:
O valor que o cliente ainda deve numa fatura (FT) por liquidar.
_Avoid_: valor em débito, nota de crédito, troco

**Liquidação**:
O estado em que o documento fiscal fica pago. A FS e a FR nascem liquidadas. A FT só se liquida com um recibo.
_Avoid_: pagamento efetuado numa FT, valor pago a zero numa FS ou FR

**Pagamento parcial**:
A liquidação de só algumas quantidades da ordem aberta. O resto continua na ordem aberta. O documento é uma FS ou uma FR.
_Avoid_: adiantamento, fatura com dívida parcial

**Pagamento misto**:
Uma liquidação em que o mesmo documento fiscal soma mais do que um método de pagamento.
_Avoid_: um documento por método, vários recibos

**MB Way**:
Método de pagamento. Pode liquidar sozinho ou entrar num pagamento misto.
_Avoid_: referência multibanco, transferência

**Nota de crédito**:
Documento fiscal que reduz ou anula uma venda já emitida. Na liquidação entra como crédito e baixa a dívida da fatura. Na grelha de documentos não é dívida do cliente. A sigla de tipo é NC; o nome visível é nota de crédito.
_Avoid_: NC como único nome, valor em dívida na linha da nota, fatura, recibo

**Recibo**:
O documento que liquida uma fatura (FT). O número é a sigla da série e o sequencial. O método de pagamento por omissão é Numerário.
_Avoid_: carimbo de data/hora, cartão cliente por omissão, RG em Portugal

**Portal**:
A interface web do logicPOS no posto (pasta Portal). Corre como serviço Windows, não como consola. Em Cloud pode existir sem API local.
_Avoid_: webapp como nome de produto; consola do Portal

**Serviço Windows (API / Portal)**:
O processo em segundo plano que mantém a API local e/ou o Portal sempre disponíveis após o download pós-instalação. A desinstalação do produto remove estes serviços e as pastas correspondentes.
_Avoid_: consola visível, serviço de mesa (hospitalidade), um único serviço “stack”

**API local**:
A API instalada na pasta API do posto (Business servidor). Distinta da API de produção Cloud.
_Avoid_: Cloud, modo API (posto ligado a API que já existe noutro sítio)
