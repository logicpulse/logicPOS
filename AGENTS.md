# AI agent instructions — LogicPOS

Use this together with [`.cursor/rules/`](.cursor/rules/). The [README.md](README.md) is the source of truth for **human** onboarding (build order, paths, tooling).

## What this codebase is

- **LogicPOS.App** / **LogicPOS.Core**: Avalonia desktop (**net10.0**), solution `LogicPOS.sln`. Startup is `LogicPOS.Core/AppComposition.cs`. A fiscal plugin is `plugins\*Plugin.dll` beside the app, loaded by `LogicPOS.Core/Fiscal/FiscalModuleLoader.cs`.
- **LogicPOS.Globalization**: **RESX** localization; keep culture files consistent when changing strings.

## Rules of engagement

1. **Minimal diffs** — Change only what the task requires; do not refactor unrelated code or delete comments.
2. **Match existing style** — Naming, partial classes, folder layout.
3. **Secrets** — Never commit real database passwords, API URLs, or tokens.
4. **Build toolchain** — `dotnet build LogicPOS.sln -c Debug` and `dotnet test tests/LogicPOS.Core.Tests/LogicPOS.Core.Tests.csproj -c Debug`.
5. **Debug** — Launch `logicpos.dll` with the working directory `LogicPOS.App/bin/Debug/net10.0`.

## Where to look

| Task | Location |
|------|-----------|
| Avalonia startup | `LogicPOS.Core/AppComposition.cs` |
| Fiscal plugin | `LogicPOS.Core/Fiscal/FiscalModuleLoader.cs` |
| Avalonia PDF | `LogicPOS.Core/FrontOffice/Pdf/DocumentModel.cs` |
| Avalonia window | `LogicPOS.App` |
| Database model | `src/` |

## Testing

```powershell
dotnet test tests/LogicPOS.Core.Tests/LogicPOS.Core.Tests.csproj -c Debug
```
