# LogicPOS

**LogicPOS** is a **Windows** point-of-sale and back-office **desktop client**. The current app is **Avalonia** on **.NET 10** (`LogicPOS.Desktop` host, `LogicPOS.App` UI, `LogicPOS.Core`). Shared translations live in **LogicPOS.Globalization** (RESX, multiple locales).

This repository is licensed under the **GNU General Public License v3**; see [LICENSE](LICENSE).

For AI-assisted editing, see [AGENTS.md](AGENTS.md).

---

## Repository layout

| Path | Role |
|------|------|
| **`LogicPOS.sln`** | Avalonia solution: desktop host, app, core, tests. |
| **`LogicPOS.Desktop`** | WinExe host (`Program.cs`). Set this as the Visual Studio startup project. |
| **`LogicPOS.App`** | Avalonia UI library. |
| **`LogicPOS.Core`** | Startup, direct database, documents, fiscal plugin loader. |
| **`tests`** | Public-edition tests. |
| **`LogicPOS.Globalization`** | Localized strings (RESX), embedded by the Avalonia app. |
| **`src/`** | Direct-database model and the SQLite, MySQL, SQL Server, and PostgreSQL migrators. |
| **`db_seeds/`** | Seed data for a fresh database. |

---

## Build the Avalonia app

Requires the **.NET 10 SDK**.

```powershell
dotnet build LogicPOS.sln -c Debug
dotnet test tests/LogicPOS.Core.Tests/LogicPOS.Core.Tests.csproj -c Debug
```

Run the desktop host:

```powershell
dotnet run --project LogicPOS.Desktop -c Debug
```

Or open `LogicPOS.sln` in Visual Studio, set **LogicPOS.Desktop** as the startup project, and press F5. Settings are `LogicPOS.Desktop/appsettings.json` (copied to the output folder on build).

A fiscal plugin, when you have one, is `plugins\*Plugin.dll` or `LogicPOS.Fiscal.dll` next to the app. Without it the app starts on a direct database and prints no fiscal mark.
