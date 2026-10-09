---
type: grilling
blocked_by: [03, 05]
Status: resolved
---

# Montagem automática da edição LogicPulse

## Question

Como é que o anfitrião fechado referencia os projetos públicos e acrescenta a cloud, a certificação LogicPulse e a licença, sem esse código entrar no repositório público e sem um passo manual de limpeza?

## Answer

Em `logicPOS-internal` nasce um WinExe anfitrião que referencia a UI Avalonia pública (`LogicPOS.App`), o `LogicPOS.Core`, a licença e a cloud. Tem o seu `Program` e a sua composição. O instalador 1.6 publica este exe, não o da edição pública.

A composição do anfitrião lê o appsettings. Em modo local: base direta, `ILicenseModule` real (descarrega e grava `logicpos.licence`), e o encaixe fiscal com `LogicPOS.Fiscal.dll` ao lado. Em modo LogicPulse: `CloudComposition` (API). Não aplica o forço do `AppComposition` público (`UseCloud` falso, `UseSeed` true, `Module` default).

O repositório público mantém o `AppComposition` só para a edição pública. Cloud, licença e AT/AGT não entram no GitHub. O publish do instalador monta o anfitrião, o `LogicPOS.Fiscal.dll` e as seeds automaticamente.
