# LogicPOS

**LogicPOS** is a **Windows** point-of-sale and back-office **desktop client**. The current app is **Avalonia** on **.NET 10** (`LogicPOS.App`, `LogicPOS.Core`). Shared translations live in **LogicPOS.Globalization** (RESX, multiple locales).

This repository is licensed under the **GNU General Public License v3**; see [LICENSE](LICENSE).

For AI-assisted editing, see [AGENTS.md](AGENTS.md).

---

## Repository layout

| Path | Role |
|------|------|
| **`LogicPOS.sln`** | Avalonia solution: app, core, tests. |
| **`LogicPOS.App`** | Avalonia shell. Reads **`appsettings.json`**. |
| **`LogicPOS.Core`** | Startup, direct database, documents, fiscal plugin loader. |
| **`tests`** | Public-edition tests. |
| **`LogicPOS.Globalization`** | Localized strings (RESX), embedded by the Avalonia app. |
| **`src/`** | Direct-database model and the SQLite, MySQL, and SQL Server migrators. |
| **`db_seeds/`** | Seed data for a fresh database. |

---

## Build the Avalonia app

Requires the **.NET 10 SDK**.

```powershell
dotnet build LogicPOS.sln -c Debug
dotnet test tests/LogicPOS.Core.Tests/LogicPOS.Core.Tests.csproj -c Debug
```

Run from `LogicPOS.App/bin/Debug/net10.0` with `dotnet logicpos.dll` (the working directory must be that folder). Settings are `LogicPOS.App/appsettings.json`.

A fiscal plugin, when you have one, is `plugins\*Plugin.dll` or `LogicPOS.Fiscal.dll` next to the app. Without it the app starts on a direct database and prints no fiscal mark.
