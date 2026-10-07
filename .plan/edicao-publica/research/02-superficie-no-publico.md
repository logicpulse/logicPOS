# Superfície certificada no repo público

## Pergunta

No repositório público `logicPOS` de hoje, que tipos, campos, ecrãs e chamadas são certificação ou registo de licença, na app Avalonia e na app GTK, e quais já estão atrás do carregador da cloud?

Fontes: working tree de `C:\SVN\logicPOS-api-integration\logicPOS`, incluindo ficheiros por gravar. Não há implementação de webservice da AT ou da AGT neste tree; o que se cita é o que o tree ainda contém.

## Como o arranque separa os três

`LogicPOS.Core\AppComposition.cs` faz três coisas por esta ordem:

1. `LicenseModuleLoader.Load` corre sempre, antes de olhar para a cloud. Se a DLL de licença recusar o arranque, a mensagem fica em `StartupError` e a composição termina.
2. Com `DatabaseSettings:UseCloud` verdadeiro, `CloudModuleLoader.TryRegister` carrega `LogicPOS.Cloud.dll` e chama `AddCloud`. Se isso sucede, `Configure` devolve o contentor da cloud e não regista o plugin fiscal, o hasher local, nem os serviços locais de documento e de back-office.
3. No caminho da base de dados directa, `FiscalModuleLoader.TryRegister` procura `plugins\*Plugin.dll` e depois `LogicPOS.Fiscal.dll` ao lado da aplicação. Sem plugin, regista `NullFiscalModule`.

`LogicPOS.Core\Cloud\CloudModuleLoader.cs` não contém tipos de certificação nem de licença. Só resolve o caminho da DLL (`LogicPOS:Cloud:AssemblyPath`, a DLL ao lado da aplicação, ou `logicpos-apiclient\src\LogicPOS.Cloud\bin`). `LogicPOS.App\appsettings.json` tem `DatabaseSettings:UseCloud` verdadeiro e `LogicPOS:Api:BaseAddress` / `ClientId`. Esses dois últimos são a ligação à API; não são certificação nem licença. Servem só para dizer se o caminho da cloud está ligado.

`LogicPOS.App\logicpos.csproj` (`AttachPrivatePieces`) copia, no publish, DLLs de `LogicPOS.Cloud`, `LogicPOS.Fiscal` e `LogicPOS.Licence` quando essas pastas de build existem ao lado do repo. É um passo de publicação, não um carregador em runtime.

`tests\LogicPOS.Core.Tests\PublicEditionStartupTests.cs` confirma o fallback: com `UseCloud` verdadeiro e sem a DLL, o arranque segue para a base directa, `IFiscalModule.IsAvailable` é falso e `ILicenseModule.RegistrationRequired` é falso.

## Certificação ainda na Avalonia

### Encaixe: atrás do carregador fiscal, fora do carregador da cloud

O carregador fiscal só entra no contentor da base directa. No sucesso da cloud, `AppComposition` nem o chama.

| Peça | Ficheiro | O que é |
| --- | --- | --- |
| `IFiscalModule` | `LogicPOS.Core\Fiscal\IFiscalModule.cs` | `IsAvailable`, `DescribePrint`, `ExportSaftAsync`, `RegisterSeriesAsync`, `RequestSeriesCodeAsync`, `NotifyCancellationAsync` |
| `FiscalDocument` / `FiscalPrint` | `LogicPOS.Core\Fiscal\FiscalDocument.cs` | Factos (`Hash`, `QrPayload`, `SeriesCode`, totais, NIF) e o que imprimir (`CodeLine`, `QrPayload`) |
| `FiscalModuleLoader` | `LogicPOS.Core\Fiscal\FiscalModuleLoader.cs` | Carrega o plugin. Sem referência de projecto à certificação |
| `NullFiscalModule` | `LogicPOS.Core\Fiscal\NullFiscalModule.cs` | `DescribePrint` vazio. SAF-T, registo de série e pedido de código devolvem «A certificação fiscal não está instalada neste posto.» A anulação devolve nulo |
| `FiscalMarks` | `LogicPOS.Core\Fiscal\FiscalMarks.cs` | Só chama `DescribePrint` quando `IsAvailable` é verdadeiro. Caso contrário devolve `FiscalPrint` vazio |

Não há neste tree um tipo ou campo com o nome ATCUD. A linha que o plugin pode acrescentar chama-se `FiscalCodeLine` / `FiscalPrint.CodeLine`. O QR da autoridade é `ATQRCode` / `AtQrCode` / `QrPayload`.

### Ecrãs e acções que delegam no módulo

Atrás do carregador fiscal. O serviço local que as executa não é registado quando a cloud carrega, por isso não estão atrás do carregador da cloud. Os ecrãs em `LogicPOS.App` continuam no executável e falam com `IBackOfficeListingService` / `IFiscalYearWizard`; quem responde no caminho da cloud é o que a DLL externa registar, e isso não está neste tree.

- `LogicPOS.Core\BackOffice\BackOfficeListingService.cs`: na página «Séries», acções `export-saft` (`ExportSaftAsync`), `register-at` (`RegisterSeriesAsync`), `request-agt` (`RequestSeriesCodeAsync`). Páginas «SAF-T ano», «SAF-T último mês» e «SAF-T período» chamam o mesmo `ExportSaftAsync`. `FiscalAsync` usa o `IFiscalModule` do contentor, ou um `NullFiscalModule` novo.
- `LogicPOS.App\Views\BackOfficeWindow.axaml.cs`: menu Exportar com «SAF-T ano», «SAF-T último mês» e «SAF-T período». Se o país for Angola, insere o grupo «AGT» com «Séries AGT» e «Documentos AGT».
- `LogicPOS.App\Views\EntityListing.axaml.cs`: `export-saft` / `saft-period` enviam o intervalo; `register-at` exige a série seleccionada; `request-agt` pede o tipo de documento (predefinição `FT`).
- `LogicPOS.Core\BackOffice\LocalFiscalYearWizard.cs`: com `CommunicateWithAt`, chama `RegisterSeriesAsync` por cada série criada. Sem módulo disponível, a nota é «As séries ficaram por comunicar.»
- `LogicPOS.Core\FrontOffice\PosDocumentService.cs`: `CancelDocumentAsync`, se o módulo está disponível, chama `NotifyCancellationAsync`. `CreateA4FileAsync` passa o documento a `FiscalMarks.Read` e, se o plugin devolver texto, escreve `FiscalCodeLine` e `AtQRCode` no modelo do PDF.
- `LogicPOS.Core\FrontOffice\LocalThermalPrintSource.cs`: o mesmo `FiscalMarks.Read` sobre `ATQRCode`, para o ticket térmico.

### Consulta AGT: ecrã público, chamada ausente dos dois carregadores

`BackOfficeListingService` tem as páginas «Documentos AGT» e «Séries AGT». O texto diz que a consulta online está disponível no modo cloud. A acção `agt-consult` devolve essa frase e não chama `IFiscalModule` nem `CloudModuleLoader`. «Séries AGT» não tem acção. O ecrã está na app; a consulta em si não está implementada neste tree.

### O que imprime sem plugin: nenhum dos dois carregadores

`LogicPOS.Core\FrontOffice\Pdf\DocumentModel.cs`, `DesignFooter`, em todas as páginas: os primeiros quatro caracteres de `Document.Hash`, depois «Processado por programa validado Nº 221/AGT/2024/LogicPOS» quando `IsAngola`, senão «Processado por programa certificado Nº 2543/AT». `AddQrCell` imprime `FiscalCodeLine` se vier preenchido e desenha um QR de `AtQRCode` ou, na falta dele, do número do documento. O QR angolano usa `Company.AgtLogo`. Os totais angolanos usam `ApplyAgtRounding`.

`LogicPOS.Core\FrontOffice\Pdf\ReceiptModel.cs`, `DesignFooter`: a mesma linha 221/AGT ou 2543/AT, conforme o país, sem o prefixo do hash. O QR do recibo usa `Receipt.QrCode` e, em Angola, `AgtLogo`.

`LogicPOS.App\Hardware\ThermalInvoiceRenderer.cs`: imprime `FiscalCodeLine` e um QR de `AtQrCode` ou do número, se `PrintQrCode` for verdadeiro. O rodapé do tipo de documento usa as chaves `global_documentfinance_type_report_invoice_footer_at` e `global_documentfinance_type_report_non_invoice_footer_at`. Em `LogicPOS.Globalization\Localization\Resx.pt-PT.resx` esses textos são a alínea do CIVA e «Este documento não serve de fatura», não o número 2543/AT.

`LogicPOS.Core\FrontOffice\LocalThermalPrintSource.cs` lê a preferência `PRINT_QRCODE`.

### Hash e série de teste: nenhum dos dois carregadores

`LogicPOS.Core\FrontOffice\LocalDocumentHasher.cs` implementa `IDocumentHasher` com SHA-256 sobre data, número, total e hash anterior. O comentário no ficheiro diz que a assinatura certificada do SAF-T com chave privada fica fora deste tree. `AppComposition` regista-o só no caminho da base directa. `PosDocumentService` passa-o a `Document.CreateAsync`. A listagem esconde `Hash`, `Hash4Code`, `HashControl` e `QrCode` (`BackOfficeListingService`, vector `Hidden`).

`PosDocumentService` (`LocalTestSeriesCode` = `TESTELOCAL`) grava `ATDocCodeValidationSeries` / `AtValidationCode` nas séries em falta, em `EnsureSeriesAsync` e `EnsureFiscalSetupAsync`, sem chamar o plugin. `Explain` traduz o erro «não foi comunicada» para uma frase sobre a série ainda não comunicada à AT.

### Ecrã da AT e separador SAF-T: nenhum dos dois carregadores

`LogicPOS.App\Views\FiscalYearWizard.axaml` tem o passo «AT» (só quando o rascunho é Portugal), a caixa «Comunicar as séries com a AT» e o botão «Testar». `FiscalYearWizard.axaml.cs` mostra os parâmetros de sistema cujo token casa com `service_at_production_mode_enabled`, `service_at_production_account_fiscal_number`, `service_at_production_account_password`, `service_at_send_documents`, `service_at_send_documents_waybill` e `service_at_waybill_agricultural_mode_enabled`, e grava-os na listagem. `LocalFiscalYearWizard.TestAtAsync` devolve «O teste em linha à AT não está disponível neste posto.» O pedido `CommunicateWithAt` é que entra no plugin, como acima.

`LogicPOS.App\Views\SettingsPage.axaml.cs` tem o separador «SAFT» e mete nele parâmetros cujo token contém `SAFT` ou começa por `SERVICE_AT` ou `AT_`.

`LogicPOS.Core\LogicPOS.Core.csproj` declara no comentário de topo que a certificação portuguesa da AT (webservice, certificados e comunicação de séries) não pertence a este repositório. O comentário não apaga as peças listadas acima.

## Registo de licença ainda na Avalonia

Nenhum dos dois carregadores fiscal ou da cloud. O carregador da licença corre antes da cloud e, se a DLL existir, o `ILicenseModule` já carregado é o que entra no contentor da cloud (`AppComposition`: `cloudServices.AddSingleton(license)`). A implementação do registo não está neste tree.

| Peça | Ficheiro | O que é |
| --- | --- | --- |
| `ILicenseModule` | `LogicPOS.Core\Licensing\ILicenseModule.cs` | Só `RegistrationRequired`. O comentário diz que o registo LogicPulse fica fora do repositório |
| `NullLicenseModule` | `LogicPOS.Core\Licensing\NullLicenseModule.cs` | `RegistrationRequired` falso, quando não há DLL |
| `LicenseModuleLoader` | `LogicPOS.Core\Licensing\LicenseModuleLoader.cs` | `LogicPOS:License:AssemblyPath` ou `LogicPOS.Licence.dll` ao lado da aplicação. Reflecte `LogicPOS.Licence.LicenseComposition.AllowsStartup` e `LogicPOS.Licence.HardwareIdProvider.GetHardwareId`. Se o arranque não for permitido: «É necessário registar a licença antes de usar a aplicação.» Com DLL presente, devolve `RequiredLicenseModule` (`RegistrationRequired` verdadeiro) |
| `MachineIdentity` | `LogicPOS.Core\MachineIdentity.cs` | Identidade local em `hardware.id`. O loader da licença substitui o fornecedor pelo da DLL |

Não há ecrã de registo de licença na app Avalonia. `RegistrationRequired` é lido no teste de arranque; não há outro uso na UI. O campo `HardwareId` dos terminais (`LoginWindow`, listagem de terminais) é a identidade da máquina, não o formulário de licença.

## Certificação e licença ainda no GTK

A árvore é `discontinued\LogicPOS.UI` e `discontinued\LogicPOS.Api`. Não há `FiscalModuleLoader`, `IFiscalModule` nem `CloudModuleLoader`. As chamadas saem pelo cliente HTTP: `discontinued\LogicPOS.Api\DependencyInjection.cs` põe `HttpClient.BaseAddress` a partir de `ApiSettings.BaseAddress` (`apisettings.example.json` documenta `http://localhost:5011/`). Isso é a ligação à API, não o carregador da cloud da Avalonia. Nenhum dos dois carregadores cobre esta árvore.

### Certificação

Ecrãs e fachadas, todos a passar pelo cliente HTTP:

- SAF-T: `BackOfficeWindow.Components.cs` / `BackOfficeWindow.EventHandlers.cs` — botões do ano, do último mês e de período. `ExportSaftByPeriod` envia `GetSaftQuery`. O handler em `discontinued\LogicPOS.Api\Features\Finance\Saft\GetSaft\GetSaftQueryHandler.cs` pede o ficheiro. O destino usa a preferência `PATH_SAFTPT` (`PreferenceParametersService.SaftExportPath`). Permissões `BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_*` em `UserProfilePermissions.cs`. O menu SAF-T é empacotado quando `UseAgtFe` é falso.
- Séries AT: `DocumentSerieModal` (`Components`, `Designer`, `EventHandlers`, `DocumentSerieModal.cs`). Campo `_txtATDocCodeValidationSerie`, rótulo `global_at_atdoccodeid` («Código de Identificação» em `Resx.pt-PT.resx`). Grava `AtValidationCode` e lê `ATDocCodeValidationSeries`. O botão chama `AtService.RegisterSeries`.
- Documentos AT: `DocumentsModal.EventHandler.Portugal.cs` chama `AtService.RegisterDocument` e mostra `AtDocCodeId`. `AtService.DocumentTypeRequiresAtRegistration` é verdadeiro para guias. `DocumentViewModel` tem `AtDocCodeId`, `AtResendDocument`, `IsAtDocument` e `GetAtStatus`.
- Tipos HTTP da AT, no cliente: `RegisterSeriesCommand` / `AtSeriesInfo` (`ValidationCode` e notas `ATDocCodeValidacaoSerie`) e `RegisterDocumentCommand` / `RegisterDocumentResponse.AtDocCodeId`. Entidade de série do cliente: `discontinued\LogicPOS.Api\Features\Finance\Documents\Series\DocumentSeries.cs`, campo `ATDocCodeValidationSeries`. Tipo SAF-T do documento: `SaftDocumentType`.
- AGT, quando `SystemInformationService.UseAgtFe` (Angola e `LicensingService.Data.AgtFeModule`): secção «AGT» com `AgtSeriesPage` («Séries») e `AgtDocumentsPage` («Documentos»); modais de filtro, `RequestSeriesModal`, `SeriesInfoModal`, `AgtDocumentInfoModal`, `AgtOnlineDocumentInfoModal`. Documentos e recibos ganham a coluna «AGT/Est. Validação» e os botões enviar, actualizar estado e ver informação. `AgtService` é a fachada. Os comandos estão em `discontinued\LogicPOS.Api\Features\Finance\Agt\` (registo, séries, consulta online, estado de validação, NIF). `DocumentViewModel.Agt` e `GetAgtStatus` / `IsAgtDocument`. O mesmo padrão nos recibos (`ReceiptsModal`, `ReceiptsPage.Columns.cs`).
- Impressão térmica: `discontinued\LogicPOS.UI\Printing\Thermal\Printers\InvoicePrinter.cs` escreve `FiscalCodeLine` e um QR de `ATQRCode` ou do número, se `PreferenceParametersService.PrintQrCode`. O modelo vem de `DocumentPrintingModel` (`ATQRCode`, `FiscalCodeLine`). O rodapé do tipo de documento usa as mesmas chaves CIVA / «não serve de fatura». Não há neste tree a linha «2543/AT» nem «221/AGT».
- Arredondamento de linha AGT: `AgtLineRounding.cs`, usado em `SaleItem` e `DocumentDetail`. Logótipo do QR: preferência `AGT_FE_QRCODE_LOGO`.

Não há nome de campo ATCUD nesta árvore. O código de série é `ATDocCodeValidationSeries` / `ValidationCode`; o código do documento comunicado é `AtDocCodeId`.

### Registo de licença

Também pelo cliente HTTP, não por `ILicenseModule` nem por `LogicPOS.Licence`.

- `discontinued\LogicPOS.UI\Program.cs`: se `LicensingService.Data.IsLicensed` é falso, tenta activar de ficheiro ou abre `RegisterModal`.
- `discontinued\LogicPOS.UI\Application\Licensing\RegisterModal\RegisterModal.cs`: campos nome, empresa, NIF, morada, email, telefone, país, `HardwareId`, versão do assembly e chave de software. Envia `ActivateLicenseCommand`.
- `LicensingService.cs`: `GetLicenseData`, `ActivateLicense`, `RefreshLicense`. O cliente HTTP correspondente está em `discontinued\LogicPOS.Api\Features\System\Licensing\` (`GetLicenseDataQuery`, `ActivateLicenseCommand`, `RefreshLicenseCommand`, `LicenseData` com `IsLicensed` e `Status`).
- O estado da licença também condiciona o POS (`POSWindow.cs`, `LoginWindow.Designer.cs`), a barra do back-office (`BackOfficeBaseWindow.cs`) e `UseAgtFe`.

## O que uma decisão posterior ainda encontra no público

Estas peças estão no tree e não estão dentro do carregador da cloud. As chamadas à autoridade, na Avalonia, só acontecem se o plugin fiscal estiver carregado no caminho da base directa.

- Rodapés 2543/AT e 221/AGT, QR e linha fiscal do PDF: `LogicPOS.Core\FrontOffice\Pdf\DocumentModel.cs`, `ReceiptModel.cs`.
- QR térmico e `FiscalCodeLine`: `LogicPOS.App\Hardware\ThermalInvoiceRenderer.cs`, `LogicPOS.Core\FrontOffice\LocalThermalPrintSource.cs`.
- Hash local SHA-256 e campos escondidos `Hash` / `Hash4Code` / `HashControl` / `QrCode`: `LocalDocumentHasher.cs`, `BackOfficeListingService.cs`.
- Série `TESTELOCAL` em `ATDocCodeValidationSeries`: `PosDocumentService.cs`.
- Menus SAF-T e AGT, acções de série, e a consulta AGT que só devolve a frase do modo cloud: `BackOfficeWindow.axaml.cs`, `EntityListing.axaml.cs`, `BackOfficeListingService.cs`.
- Assistente com passo AT, conta da AT e «Testar»: `FiscalYearWizard.axaml`, `FiscalYearWizard.axaml.cs`, `LocalFiscalYearWizard.cs`.
- Separador «SAFT» das definições: `SettingsPage.axaml.cs`.
- Textos `global_at_atdoccodeid` e rodapés `*_footer_at`: `LogicPOS.Globalization\Localization\Resx.pt-PT.resx`.
- Árvore GTK inteira sob `discontinued\`: ecrãs SAF-T, série (`ATDocCodeValidationSeries`), envio à AT (`AtDocCodeId`), páginas AGT, `InvoicePrinter`, e `RegisterModal` / `LicensingService`.
