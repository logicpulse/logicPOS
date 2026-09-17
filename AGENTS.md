# AI agent instructions — LogicPOS

Use this together with [`.cursor/rules/`](.cursor/rules/). The [README.md](README.md) is the source of truth for **human** onboarding (build order, paths, tooling).

## What this codebase is

- **LogicPOS.UI**: **.NET Framework 4.8** **GTK#** desktop app, output **`logicpos.exe`**. Main UI is **not** WPF/WinForms; follow existing GTK patterns under `Components/` and `Application/`.
- **LogicPOS.Api**: **Client library only** (**netstandard2.0**). **MediatR** + **`HttpClient`** call a **remote REST API**. There is **no** in-repo web host, Kestrel, or controllers — only outbound HTTP. API base URL: **`apisettings.json`** → **`ApiSettings.Default.BaseAddress`**.
- **LogicPOS.Globalization**: **RESX** localization; keep culture files consistent when changing strings.

## Rules of engagement

1. **Minimal diffs** — Change only what the task requires; do not refactor unrelated code or delete comments.
2. **Match existing style** — Naming, partial classes (`*.Components.cs`, `*.Validation.cs`), folder layout, MediatR command/query naming in **`LogicPOS.Api/Features/<Area>/...`**.
3. **New API operations** — Add **Command** / **Query** + **Handler** using **`ErrorOr<T>`**, **`RequestHandler<,>`**, and **`IHttpClientFactory`** (`"Default"` client). Do not add ASP.NET Core types.
4. **Secrets** — **`apisettings.json`** is gitignored. Never commit real URLs or tokens; use **`LogicPOS.Api/apisettings.example.json`** as the documented template.
5. **Build toolchain** — Prefer **Visual Studio MSBuild** or **`.vscode/build-solution.ps1`**. Plain **`dotnet msbuild`** on the **full solution** can fail **`LogicPOS.Globalization`** (AL / linker). **`publish.ps1`** expects **`MSBuild`** on PATH (Developer shell).
6. **Do not** convert **`LogicPOS.UI`** to SDK-style or retarget without an explicit product decision.
7. **GTK / designer** — Treat **`*.Designer.cs`** and generated regions carefully; match how the team edits GTK surfaces (often Visual Studio).
8. **VS Code / Cursor debugging** — **`launch.json`** uses **`"type": "clr"`** (Framework). **Debug** builds use **`Prefer32Bit>false`** so the VS Code Desktop CLR debugger (**x64-only**) can attach. Do not suggest **`coreclr`** / **`dotnet`** launch types for **`logicpos.exe`**.

## Where to look

| Task | Location |
|------|-----------|
| Startup, logging, single-instance | `LogicPOS.UI/Program.cs` |
| UI DI | `LogicPOS.UI/DependencyInjection.cs` |
| API client registration | `LogicPOS.Api/DependencyInjection.cs` |
| HTTP handlers base | `LogicPOS.Api/Features/Common/Requests/` |
| UI JSON settings | `LogicPOS.UI/appsettings.json`, `LogicPOS.UI/Application/Settings/` |
| API URL binding | `LogicPOS.Api/ApiSettings.cs`, local `apisettings.json` |

## Testing

No test projects are in **`LogicPOS.sln`** by default — verify with a **Debug** build and manual runs unless tests exist in-repo.
