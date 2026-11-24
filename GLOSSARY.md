# LogicPOS

O produto de ponto de venda e back-office. A edição pública é o que o GitHub publica. A edição LogicPulse vive fora desse repositório.

## Language

**Edição pública**:
O repositório `logicPOS` publicado no GitHub, sozinho: não tem instalador e não inclui `logicPOS-internal`. Só base de dados direta, sem ligação à API, sem certificação e sem registo de licença, com um encaixe para uma certificação de terceiros. A build develop da lista 2.0.31 é esta edição: sem a DLL, séries por comunicar à AT e a faixa de módulo em falta são o estado esperado.
_Avoid_: versão free, open source, Community

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
O instalador Windows da edição LogicPulse, com certificação. A edição pública não tem instalador. Numa instalação nova cria a base com esquema e administrador, pede o modo de operação, a base de dados de demonstração e os dados da empresa, e essa escolha só fica feita quando a app a confirma.
_Avoid_: setup opensource, instalador do GitHub, conta cloud

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
O ramo de negócio, como restauração, café ou padaria. Numa instalação nova, local ou Cloud, escolhe-se na primeira entrada, no Avalonia ou na webapp. No local, depois da confirmação, fica esse modo. Na conta cloud é um só: se a conta já tiver um, vem selecionado, e depois da primeira entrada é o que a conta disser, mesmo que a ficha mude mais tarde.
_Avoid_: vertical, módulo da API, conta em modo demo

**Primeira entrada**:
A confirmação, numa instalação nova local ou Cloud, do modo de operação, de aceitar ou não a base de dados de demonstração, e dos dados da empresa. Não acontece no modo API nem numa atualização. No local o instalador deixa a escolha pronta e a app é que a confirma. Na Cloud só a app pergunta, depois do login; se a conta já tiver modo, esse vem selecionado, e a demonstração vem por aceitar. Antes de confirmar pode mudar-se o modo, a demonstração e a empresa; a amostra é a do modo confirmado. Fechar a app sem confirmar não grava a confirmação. Outra confirmação ainda aberta não a substitui. Se a base de dados de demonstração aceite não chegar a entrar, a confirmação não fica feita.
_Avoid_: modo API, atualização, conta em modo demo, ano fiscal

**Dados da empresa**:
O país, a moeda e o NIF da primeira entrada. Os três são precisos para a concluir. Na conta cloud o país vem preenchido. A pessoa pode mudá-los antes de confirmar. Com o país confirmado entram o IVA, os meios de pagamento e os tipos de documento, sem pergunta própria.
_Avoid_: ano fiscal, séries, base de dados de demonstração

**Base da conta cloud**:
A base do POS dessa conta cloud. Nasce com o administrador da ficha, antes da primeira entrada. A palavra-passe é a que a conta tinha nesse momento; uma alteração posterior na conta não a muda.
_Avoid_: base de dados de demonstração, base sem administrador

**Base de dados de demonstração**:
Os artigos, as mesas e os clientes de exemplo do modo de operação. Entram na primeira entrada, se a pessoa aceitar, e são os do modo confirmado. Uma mudança posterior do modo não os tira nem os troca.
_Avoid_: IVA, meios de pagamento, tipos de documento, conta em modo demo

**Conta em modo demo**:
Marca comercial da conta no logicERP. Não escolhe o modo de operação nem mete a base de dados de demonstração.
_Avoid_: base de dados de demonstração, seeds, modo de operação

**Client ID**:
O identificador com que o posto se apresenta à API no modo LogicPulse. Vem preenchido e pode ficar vazio.
_Avoid_: licença, número de série

**Ficheiro de licença**:
O `logicpos.licence` que a instalação local descarrega do ERP quando já existe uma licença válida para o hardware id desta máquina. Sem ele a app abre, mas a impressão e a geração de documentos ficam inativas.
_Avoid_: DLL, logicpos.license

**Hardware id**:
O identificador da máquina no formato `XXXX-XXXX-XXXX-XXXX-XXXX-XXXX`, gerado como o GTK já pedia à API.
_Avoid_: IntelliLock, MachineGuid

**Conta Cloud**:
A instalação logicPOS na API de produção, identificada pelo Client ID. No logicERP agrupa só licenças da edição Cloud, e cada licença está no máximo numa conta. A licença não guarda o Client ID.
_Avoid_: Cliente, cliente do ERP, licença Business, conta de outro produto

**Licença Cloud do posto**:
Uma licença da edição Cloud no logicERP, com o hardware id desta máquina, agrupada numa só conta Cloud. Confirma-se na inscrição e em cada arranque, antes do PIN. Sem resposta do logicERP um posto já inscrito mantém-se 24 horas após a última confirmação; sem a API não há PIN nem cópia local dos segredos.
_Avoid_: Cliente, ficheiro de licença, lugares em simultâneo, licença do servidor da API, licença Business

**Inscrição do posto**:
A autorização deste posto Avalonia em Cloud, feita por um utilizador activo com o Client ID, o seu login e a palavra-passe do portal. Se o portal exige código, a inscrição também o exige. Sem a licença Cloud do posto não existe. A entrada do dia, depois disto, é o PIN. O modo local, o modo API e o portal não a usam.
_Avoid_: sessão, login com PIN, registo de licença

**Bloqueio de inscrição**:
Cinco palavras-passe do portal erradas impedem nova inscrição neste posto durante cinco minutos. Não mexe no PIN dos outros utilizadores.
_Avoid_: bloqueio de PIN, bloquear a conta

**Bloqueio de PIN**:
Cinco introduções erradas seguidas do PIN impedem esse utilizador durante cinco minutos neste posto, no ecrã de entrada e na troca de utilizador. Os outros utilizadores deste posto continuam, e este utilizador noutro posto da mesma conta também.
_Avoid_: bloquear o posto, bloquear a conta, palavra-passe do portal

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
O segredo numérico de entrada no posto, depois da inscrição. Não é a entrada na base.
_Avoid_: palavra-passe, password, entrada na base

**Entrada na base**:
O administrador e a palavra-passe da base, no Avalonia e na webapp. A palavra-passe é a que a conta, ou a instalação, tinha quando a base nasceu.
_Avoid_: PIN, inscrição do posto, palavra-passe do portal

**Perfil**:
O papel de um utilizador. Um utilizador novo não tem perfil até alguém o escolher.
_Avoid_: Barman por omissão, Proprietário em todos os novos

**Palavra-passe do portal**:
O segredo do portal, distinto do PIN do posto.
_Avoid_: password, PIN

**Código do artigo**:
O identificador do artigo, atribuído pelo sistema e já visível antes de gravar.
_Avoid_: código vazio, código obrigatório escrito à mão

**Valor entregue**:
O montante em numerário que o cliente entrega no pagamento.
_Avoid_: dinheiro requisitado, dinheiro pedido

**Predefinido**:
O registo usado quando ninguém escolhe outro.
_Avoid_: por padrão, por omissão, default

**Código do país**:
As duas letras do país, como PT ou AO.
_Avoid_: Código2 do País, código2

**Consulta VIES**:
A leitura do NIF no VIES, só quando quem edita o cliente a pede. Não corre ao sair do campo.
_Avoid_: validação automática, erro ao sair do NIF

**Total do documento**:
O único total em dinheiro de um documento fiscal. Listas, resumo e pagamento mostram os mesmos cêntimos.
_Avoid_: total por ecrã, diferença de um cêntimo aceite

**Código-postal (auxiliar)**:
Consulta pública só de Portugal. O código completo preenche localidade e cidade, e a rua apenas se a morada estiver vazia; um prefixo de quatro dígitos só abre uma lista que alguém pediu. Sem rede, com código desconhecido, ou em Angola e Moçambique, não há sugestão nem erro, e a morada escreve-se à mão.
_Avoid_: postallocalities.json, postalstreets.json, tabela PostalLocalities, catálogo local, consulta a cada letra

**Localidade (campo)**:
Texto da localidade na morada do Cliente; pode ser preenchido a partir do Código-postal auxiliar.
_Avoid_: cidade como sinónimo exclusivo; tabela de ruas

**Consumidor final**:
O cliente de sistema usado quando não há cliente comercial específico; não se edita nem apaga na listagem nem no documento. Na face da fatura não leva NIF, salvo se o pedir. O 999999990 não se imprime: serve só o SAF-T e o QR.
_Avoid_: anónimo, cliente vazio, 999999990 impresso, NIF obrigatório

**Documento novo**:
O ecrã de emissão de um documento (Avalonia NewDocument / portal Novo Doc.).
_Avoid_: invoice wizard, formulário de fatura genérico

**Ordem aberta / Pedido de mesa**:
O ticket de cozinha (ou retalho) gravado numa mesa enquanto o serviço corre: linhas finalizadas com Fin.pedido, ainda sem documento fiscal. Em “Ver pedidos” lista-se e pode eliminar-se uma ou todas as ordens abertas da mesa.
_Avoid_: fatura, FS, FR, FT, conta paga, documento fiscal

**Documento fiscal**:
O documento com série que substitui a ordem aberta: fatura simplificada (FS), fatura-recibo (FR) ou fatura (FT). Não é a ordem aberta da mesa.
_Avoid_: pedido de mesa, ticket de cozinha, Fin.pedido, pagamento (a FT não é um pagamento)

**NIF na fatura**:
Campo opcional no pagamento e no documento novo. Não cria ficha de cliente. Vazio: a face da fatura não leva NIF. NIF de pessoa singular imprime-se e o teto mantém-se. NIF iniciado por 5 ou por 6 trata o adquirente como sujeito passivo: até 100 € sai fatura simplificada só com esse NIF; acima do teto, a liquidação espera um cliente com nome, morada e o mesmo NIF, e só então sai a fatura-recibo. No regime do artigo 53.º a simplificada com esse NIF chega em qualquer valor.
_Avoid_: ficha de cliente obrigatória abaixo de 100 €, fatura-recibo só com NIF de empresa, 999999990 na face

**Vendedor ambulante**:
Marcação na ficha da empresa. Fora da restauração, do café e do estacionamento, uma venda só de Produtos a quem não é sujeito passivo pode ir até 1000 € na fatura simplificada. Sem a marcação, essa venda fica nos 100 €. Restauração, café e estacionamento ficam nos 100 € mesmo com a marcação.
_Avoid_: modo de retalho como sinónimo, 1000 € na restauração

**Anulação (fatura)**:
Estado de uma FT, FS ou FR já emitida. Exige motivo. Só cabe se não houver recibo, nota de crédito nem estado faturado. O número mantém-se na série. Devolução ou desconto faz-se por nota de crédito. O rascunho apaga-se.
_Avoid_: apagar o número, anular com recibo, nota de crédito se a certificação o exigir

**Guia de transporte**:
Documento que acompanha bens em circulação. Na ficha da empresa, quem teve volume de negócios acima de 100 000 € no ano anterior marca essa obrigação. Com a marcação, a guia só circula com o código da AT e com hora de início posterior à comunicação; sem rede, os bens não saem. Sem a marcação, a guia imprime-se sem esperar pelo código.
_Avoid_: código AT em toda a empresa, guia válida depois da hora de início, contingência do 5.º dia útil no uso normal

**Série comunicada (ATCUD)**:
Com o módulo fiscal presente, em Portugal, a FT, a FS, a FR, a nota de crédito, o recibo e a guia só se emitem se a série tiver código AT. O ATCUD é esse código mais o número. Sem o módulo, a emissão local mantém-se e o arranque avisa.
_Avoid_: ATCUD vazio com o módulo presente, recusar a versão GitHub

**Estacionamento (venda)**:
O talão de pagamento fecha a venda e dispensa fatura. A fatura simplificada só sai se alguém a pedir, e fica sujeita ao teto de 100 €.
_Avoid_: fatura obrigatória em cada saída, venda sem comprovativo

**Fatura simplificada (FS)**:
Documento fiscal de venda liquidada na emissão, só dentro do artigo 40.º do CIVA. O teto é o total da fatura. Restauração, café e estacionamento ficam nos 100 €, mesmo que o artigo esteja como Produto. Os 1000 € só existem num modo de retalho (loja, padaria, talho, roupa, ferragens, sapatos, peixe), com todas as linhas na classe Produtos, a um cliente que não é sujeito passivo. Uma linha que não seja Produto, ou um sujeito passivo, baixa essa fatura para 100 €. Na dúvida, 100 €. Com o regime do artigo 53.º marcado na ficha da empresa, não há teto. Acima do teto que se aplique, a venda liquidada sai em fatura-recibo, mesmo ao consumidor final.
_Avoid_: teto por artigo, 1000 € em restauração, 1000 € com uma linha de serviço, motivo de isenção da linha como regime da empresa

**Sujeito passivo (adquirente)**:
Quem compra com NIF iniciado por 5 ou por 6. A fatura simplificada dessa venda só vai até 100 € e o NIF vai na face. Consumidor final e NIF de pessoa singular não o são. O empresário em nome individual não se distingue pelo NIF.
_Avoid_: tipo de cliente Empresa, NIF singular como sujeito passivo

**Regime de isenção (artigo 53.º)**:
Regime marcado na ficha da empresa. Enquanto estiver marcado, a fatura simplificada não tem teto de 100 € nem de 1000 €. Enquanto não estiver marcado, aplicam-se os tetos normais. O NIF do sujeito passivo continua obrigatório na face, e o do consumidor final só se ele o pedir.
_Avoid_: motivo M10, isenção de IVA de uma linha

**Fatura-recibo (FR)**:
Documento fiscal de venda liquidada na emissão, com os elementos da fatura completa. Pode sair para o consumidor final sem NIF. O NIF desse cliente só entra se ele o pedir. Se o adquirente for sujeito passivo, o NIF é obrigatório.
_Avoid_: exigir NIF ao consumidor final, 999999990 impresso na fatura, conta corrente

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
Uma liquidação em que o mesmo documento fiscal soma mais do que um método de pagamento. As parcelas somam o total a liquidar.
_Avoid_: um documento por método, vários recibos

**MB Way**:
Método de pagamento. Escolhe-se e a venda liquida, sozinho ou num pagamento misto, sem espera de confirmação no telemóvel.
_Avoid_: referência multibanco, transferência, confirmação no telemóvel

**Nota de crédito**:
Documento fiscal que reduz ou anula uma venda já emitida, sempre a partir do documento de origem e com motivo. Usa o cliente dessa fatura, incluindo o consumidor final. Na liquidação entra como crédito e baixa a dívida da fatura. Na grelha de documentos não é dívida do cliente. A sigla de tipo é NC; o nome visível é nota de crédito. Se a origem foi talão, imprime-se em talão: o cliente leva o original e o estabelecimento fica com a segunda via, que tem a linha de assinatura. Os valores no papel são positivos. Não leva a menção de documento que não serve de fatura.
_Avoid_: NC como único nome, valor em dívida na linha da nota, valores negativos no papel, exigir NIF ao consumidor final, fatura, recibo

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
