# Fronteira do opensource antigo

Fonte primária: `C:\SVN\logicpos\trunk\src\logicpos` (`logicpos_pos_opensource` e `logicpos_pos_internal`). Sem alteração de código.

## Pergunta

No legado, o que ficava fora da árvore pública (ligação à AT, ligação à AGT, ATCUD, séries, licença, plugins) e o que é que essa árvore ainda fazia sozinha no arranque e ao emitir e imprimir um documento?

## O que ficava fora da árvore pública

A solução interna não é uma segunda aplicação. `logicpos_pos_internal\logicpos.sln` referencia o executável, a biblioteca partilhada, os contratos e o carregador de plugins da árvore pública, e troca três projetos: os plugins LogicPulse, a biblioteca financeira e o serviço financeiro. A biblioteca financeira interna compila a maior parte dos `.cs` públicos por `Link` e substitui só três ficheiros locais (`logicpos_pos_internal\logicpos.financial\logicpos.financial.library\logicpos.financial.library.csproj`, linhas 314–316).

### Plugin de fabricante LogicPulse (certificação)

`logicpos_pos_internal\logicpos.plugins\plugins\logicpulse\logicpulse.softwarevendor.plugin\`

- Implementa `ISoftwareVendor` com a identidade LogicPulse: nome `LogicPos`, empresa LogicPulse Technologies, número de certificado SAF-T(PT) `2543`, NIF do produtor `508278155`, versão SAF-T `1.04_01` (`App\SettingsApp.cs`).
- Guarda a palavra-passe de produção do certificado da AT, a chave RSA de assinatura, a chave secreta emparelhada com a biblioteca financeira, e a palavra-passe de backup. O comentário em `logicpos_pos_internal\logicpos.financial\logicpos.financial.library\App\SettingsApp.cs` diz que essa chave secreta tem de ficar fora do projeto opensource.
- Acrescenta o bloco SAF-T(AO) que a amostra pública não tem: certificado `221`, NIF `5171164380`, moeda `AOA`, versão `1.01_01`, segunda chave RSA (`GetSaftSoftwareCertificateNumberAO`, `GetRsaPrivateKeyAO` em `LogicpulseSoftwareVendorPlugin.cs`).
- Traz dados de arranque cifrados em `Resources\Database\Other\Plugins\SoftwareVendor\`.
- `SignDataToSHA1Base64` só assina se a chave secreta recebida for a da própria plugin (`LogicpulseSoftwareVendorPlugin.cs`).

A chave secreta da biblioteca financeira interna é a mesma da plugin LogicPulse, e é diferente da chave da árvore pública (comparação dos literais, sem os reproduzir).

### Registo de licença

`logicpos_pos_internal\logicpos.plugins\plugins\logicpulse\logicpulse.licencemanager.plugin\`

- Única implementação de `ILicenceManager`. O contrato está na árvore pública (`logicpos_pos_opensource\logicpos.plugins\logicpos.plugin.contracts\ILicenceManager.cs`): hardware id, ficheiro de licença, `IsLicensed`, `ConnectToWS`, `ActivateLicense`.
- Liga ao serviço IntelliLock `http://licence.logicpulse.pt/ws/activationservice.asmx` (referência `Web References\WSIntellilock`).
- `NOTES.md` da plugin exige o executável passado pelo IntelliLock para a licença funcionar.

Isto é registo de licença. Não é a ligação à AT nem à AGT.

### Stocks

- A interface `IStockManagementModule` só existe como ficheiro em `logicpos_pos_internal\logicpos.financial\logicpos.financial.library\Classes\Stocks\IStockManagementModule.cs` e só entra na compilação da biblioteca financeira interna.
- A implementação `LogicpulseStockManagementPlugin` está em `logicpos_pos_internal\logicpos.plugins\plugins\logicpulse\logicpulse.stockmanagement.plugin\`.
- A árvore pública chama o tipo (`logicpos\Main.cs`, `ProcessFinanceDocument.cs`) e, na ausência da plugin, cai em `ProcessArticleStock.Add` (`ProcessFinanceDocument.cs`, por volta da linha 737).

### SAF-T de Angola substituído na build interna

A build interna compila `logicpos_pos_internal\logicpos.financial\logicpos.financial.library\Classes\Finance\SaftAo.cs` no lugar do `SaftAo.cs` público. O ficheiro interno é mais longo (cerca de 2106 linhas contra 1849): `TaxEntity` fixo `GLOBAL`, pós-processamento do XML e reescrita de hash. Os dois ficheiros escrevem `SoftwareValidationNumber` `221/AGT/2019` e `ProductID` `LogicPOS/LOGICPULSE ANGOLA`.

Não há cliente HTTP da AGT em nenhuma das árvores. A comunicação com a AGT, neste legado, é a exportação do ficheiro SAF-T(AO).

### O que a solução interna continua a compilar a partir do público

O serviço financeiro interno faz `Link` para o `ServicesAT.cs` e o `ServicesATSoapResult.cs` públicos (`logicpos_pos_internal\logicpos.financial\logicpos.financial.service\logicpos.financial.service.csproj`). Há uma cópia local de `ServicesATSoapResult.cs` na pasta interna, sem `ATDocCodeValidacaoSerie`, e essa cópia não entra no `Compile`. A ligação à AT que a build interna usa é a do código público.

`logicpos.hardwareid` também está na árvore pública (`logicpos_pos_opensource\logicpos.hardware\logicpos.hardwareid\`). A solução interna aponta para esse projeto. A pasta `logicpos_pos_internal\logicpos.hardwareid\` é outra cópia e não é a que a solução referencia.

## O que a árvore pública fazia sozinha no arranque

`logicpos_pos_opensource\logicpos\Main.cs`, `FirstSteps` e `StartApp`:

1. Lê `pathPlugins` de `logicpos\App.config` (valor `.`) e cria `PluginContainer` sobre essa pasta.
2. `GenericPluginLoader<T>` carrega cada `*Plugin.dll` e fica com os tipos que implementam a interface pedida (`logicpos.plugins\logicpos.plugin.library\GenericPluginLoader.cs`).
3. Procura o primeiro `ISoftwareVendor`. Se existir, chama `SettingsApp.InitSoftwareVendorPluginSettings()` e `ValidateEmbeddedResources()`. Se não existir, regista o erro e segue. Não termina o processo nesse ponto.
4. Procura `IStockManagementModule`. Pode ficar nulo.
5. Procura `ILicenceManager`. `forceShowPluginLicenceWithDebugger` está a `true`, por isso a procura também corre com o depurador ligado.
6. Com plugin de licença, arranca `LicenseRouter`. Sem plugin, arranca o POS sem passar pelo router (`StartApp`, ramo `else`).

`LicenseRouter` (`logicpos\Classes\Logic\License\LicenseRouter.cs`): em `DEBUG` preenche uma licença de demonstração e não chama o serviço; fora de `DEBUG`, se a plugin estiver registada, pede hardware id, compara a licença local com a do serviço e pode abrir o diálogo de registo. Se a plugin não estiver no contentor, escreve «Skip License Manager» e segue.

`logicpos\App\SettingsApp.cs`: em `DEBUG`, `LicenceRegistered` nasce `true` e sobrepõe o IntelliLock; fora de `DEBUG` nasce `false`. `LicenceManagement.IsLicensed` (`logicpos\App\LicenceManagement.cs`) usa essa flag e, se for falsa, `GlobalFramework.LicenceRegistered`.

Depois, `logicpos\logicpos.cs` (`Init`) mostra um diálogo de erro se não houver fabricante ou se `IsValidSecretKey` falhar contra `SettingsApp.SecretKey`. O diálogo não faz `Environment.Exit`.

A amostra pública é `acme.softwarevendor.plugin` (`logicpos.sln`). Identidade Acme, certificado SAF-T `0000`, NIF `000000000`, palavra-passe de produção `YOUR_PASSWORD_HERE`, palavra-passe de teste da AT `TESTEwebservice`, chave RSA e chave secreta próprias (`App\SettingsApp.cs`). Essa chave secreta é a mesma de `logicpos_pos_opensource\logicpos.financial\logicpos.financial.library\App\SettingsApp.cs`. `SignDataToSHA1Base64` assina com a chave RSA da amostra quando a chave secreta coincide (`AcmeSoftwareVendorPlugin.cs`).

A amostra não implementa os métodos SAF-T(AO) de `ISoftwareVendor.cs` (zero métodos `*AO` em `AcmeSoftwareVendorPlugin.cs`; o contrato declara-os). `InitSoftwareVendorPluginSettings` (`logicpos.shared\App\SettingsApp.cs`) pede por reflexão `GetFileFormatSaftAO` e `GetSaftSoftwareCertificateNumberAO`. Com a amostra ACME, essa chamada parte a meio do `FirstSteps` (a exceção é apanhada) e o arranque da licença, que está a seguir no mesmo `try`, não chega a correr.

## O que a árvore pública fazia sozinha num documento

Tudo abaixo está em `logicpos_pos_opensource`. A build interna, para hash, QR, ATCUD e séries, compila estes mesmos ficheiros.

### Emissão

`ProcessFinanceDocument.PersistFinanceDocument` (`logicpos.financial.library\Classes\Finance\ProcessFinanceDocument.cs`):

- Número no formato `{acrónimo de série}/{número}`.
- `GenDocumentHash`: cadeia `data;data-hora de sistema;número;total;hash anterior` e, se houver `ISoftwareVendor`, `SignDataToSHA1Base64`. Sem fabricante, o hash fica `null`. O mesmo padrão vale para recibos em `GenDocumentHashPayment`.
- `HashControl` vem das definições preenchidas pela plugin.
- ATCUD: `ATDocCodeValidacaoSerie` da série, ou vazio, mais `-` mais `NextDocumentNumber - 1`. A fórmula corre com ou sem plugin. O mesmo em `PersistFinanceDocumentPayment`.
- QR: `GenDocumentQRCode` monta os campos A–R (NIF emitente, NIF adquirente decifrado pela plugin, país, tipo, estado, data, número, ATCUD, espaço fiscal `PT`, bases e imposto, quatro caracteres do hash, número de certificado). Sem fabricante devolve `null`, mas lê `PluginSoftwareVendor.Decrypt` antes desse teste, por isso a ausência da plugin rebenta na decifragem em vez de seguir o ramo nulo.
- `GenDocumentHash4Chars` tira o 1.º, 11.º, 21.º e 31.º caráter do hash. Hash vazio lança a excepção de documento com hash inválido.
- Stock: plugin de stocks se a licença tiver o módulo e a plugin estiver carregada; senão `ProcessArticleStock.Add`.

### Séries e ligação à AT

- `ProcessFinanceDocumentSeries` cria a série (prefixo aleatório ou numérico, conforme a plugin) e, com `cultureFinancialRules == pt-PT`, `SendToAT` chama `new ServicesAT(serie, modo)` e grava `ATDocCodeValidacaoSerie` (`ProcessFinanceDocumentSeries.cs`, `SendToAT`).
- O diálogo público `FrameworkCalls.SendSeriesToATWSDialog` (`logicpos\Classes\Utils\FrameworkCalls.cs`) é o que `DialogDocumentFinanceSeries` usa para obter o código de validação.
- `ServicesAT` (`logicpos.financial.service\Objects\Modules\AT\ServicesAT.cs`) fala com `servicos.portaldasfinancas.gov.pt`: faturas (portas 700/400), séries (722/422), documentos de transporte (701/401) e guias agrícolas (702/402). A palavra-passe do certificado de produção vem de `ISoftwareVendor.GetAppSoftwareATWSProdModeCertificatePassword()`. A de teste está no próprio `ServicesAT` (`TESTEwebservice`). Credenciais de subutilizador de produção vêm das preferências `SERVICE_AT_PRODUCTION_ACCOUNT_*`.
- Depois de gravar o documento, `FrameworkCalls.PersistFinanceDocument` só chama `SendDocumentToATWSDialog` para documento de transporte com destino `PT`. A fatura normal não é enviada à AT nesse passo.

### SAF-T

- SAF-T(PT): `SaftPt.cs`, incluindo o elemento `ATCUD` e `SoftwareCertificateNumber` das definições. `FrameworkCalls.ExportSaft` escolhe `SaftPt` ou `SaftAo` pelo país.
- SAF-T(AO) público: `SaftAo.cs` escreve `SoftwareValidationNumber` `221/AGT/2019` e `ProductID` `LogicPOS/LOGICPULSE ANGOLA`. Não abre ligação à AGT.

### Impressão e linha do programa certificado

- `FrameworkCalls.PrintFinanceDocument` recusa a impressão quando `LicenceManagement.IsLicensed` ou `CanPrint` é falso. Fora de `DEBUG`, sem a plugin de licença a ter posto `LicenceRegistered`, a impressão fica fechada. Em `DEBUG` a flag pública nasce verdadeira.
- Talão (`ThermalPrinterFinanceDocumentMaster.cs`): para Portugal e Angola escreve `ATCUD:` e, em Portugal, imprime o QR a partir de `ATDocQRCode`.
- Linha de certificação (`ThermalPrinterBaseFinanceTemplate.PrintCertificationText` e `CustomReport.cs`): em Portugal, texto de processado/emitido com o número de certificado SAF-T(PT) e os quatro caracteres do hash; em Angola, texto com `SaftSoftwareCertificateNumberAO` e `SaftProductIDAO`; noutros países, só «processado por computador». Esses números só ficam preenchidos se `InitSoftwareVendorPluginSettings` tiver corrido contra uma plugin que exponha os getters.

## Factos em que uma decisão posterior se pode apoiar

1. A certificação (cliente SOAP da AT, registo de séries, fórmula do ATCUD, montagem do QR, orquestração do hash, SAF-T(PT) e SAF-T(AO)) está na árvore pública. A build interna reutiliza esses ficheiros por `Link`, exceto o `SaftAo.cs`, que substitui. (`logicpos.financial.service.csproj` interno; `ProcessFinanceDocument.cs`; `ServicesAT.cs`; `SaftPt.cs`; `SaftAo.cs`.)
2. Não existe cliente da AGT. O que há é exportação SAF-T(AO), e o número `221/AGT/2019` já está escrito no `SaftAo.cs` público.
3. Fora da árvore pública ficam a identidade e os segredos LogicPulse: chave RSA, chave secreta, palavra-passe do certificado de produção da AT, números de certificado `2543` (PT) e `221` (AO), a plugin de licença IntelliLock, a plugin de stocks e a interface `IStockManagementModule`.
4. A árvore pública traz uma amostra ACME com certificado `0000` e a mesma chave secreta que a biblioteca financeira pública. Essa amostra não implementa o contrato SAF-T(AO), e o arranque público já pede esses getters por reflexão.
5. Sem `ISoftwareVendor`, o documento grava hash `null` e a impressão dos quatro caracteres do hash falha. O ATCUD é calculado na mesma, com o código de validação vazio se a série não tiver sido comunicada.
6. A plugin de licença é opcional no arranque: sem ela o POS abre. Fora de `DEBUG`, a impressão exige licença registada, e esse estado só fica verdadeiro pelo `LicenseRouter` ou pela flag de `DEBUG`.
7. A comunicação da série à AT está no código público e só corre com `cultureFinancialRules == pt-PT`. O envio do documento à AT, no fecho da venda, está limitado ao transporte com destino Portugal.
