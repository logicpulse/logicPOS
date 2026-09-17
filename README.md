# LogicPOS

**LogicPOS** is a **Windows** point-of-sale and back-office **desktop client**. The shell is **GTK#** (GtkSharp 2) on **.NET Framework 4.8** (`logicpos.exe`). It talks to a **remote HTTP API** through the **LogicPOS.Api** library (**MediatR** + **`HttpClient`** — not a web app hosted inside this repo). Shared translations live in **LogicPOS.Globalization** (RESX, multiple locales).

This repository is licensed under the **GNU General Public License v3**; see [LICENSE](LICENSE).

**New here?** Follow [First-time setup](#first-time-setup) and [Build the solution](#build-the-solution) so you can run the app locally. For AI-assisted editing, see [AGENTS.md](AGENTS.md) and [`.cursor/rules/`](.cursor/rules/).

---

## Repository layout

| Path | Role |
|------|------|
| **`LogicPOS.sln`** | Visual Studio solution (three projects). |
| **`LogicPOS.UI`** | **WinExe** — GTK UI, devices (printers, displays, scales), PDF/thermal printing, licensing, themes. Reads **`appsettings.json`**. |
| **`LogicPOS.Api`** | **.NET Standard 2.0** — API **client** only: MediatR handlers, `HttpClient`, JWT when logged in. Reads **`apisettings.json`** for API base URL. |
| **`LogicPOS.Globalization`** | Localized strings (RESX). |
| **`libs`** | Bundled assets (e.g. **GtkRuntime** copied to build output; Spire.PDF and other vendor bits). |

Server-side features are grouped under `LogicPOS.Api/Features/`: **Articles**, **Company**, **Finance**, **POS**, **Reports**, **System**, plus **Common**.

---

## Prerequisites

| Requirement | Notes |
|-------------|--------|
| **OS** | **Windows** (GTK# deployment targets Windows here). |
| **.NET Framework 4.8** | Install the **Developer Pack** so MSBuild can target `net48`. |
| **MSBuild (Visual Studio or Build Tools)** | Required for a **reliable full solution build**. The solution mixes classic **.NET Framework** projects and **netstandard2.0**; the **.NET SDK’s** `dotnet msbuild` can fail on **`LogicPOS.Globalization`** (assembly linker / **AL** task). Use the same MSBuild that ships with **Visual Studio 2022** or **Build Tools for Visual Studio**. |
| **PowerShell** | Used by **`.vscode/build-solution.ps1`** and **`publish.ps1`**. |
| **Backend API** | A compatible REST API must be running; default URL in the sample config is `http://localhost:5011/`. |

Optional: **Visual Studio Code** or **Cursor** with the **C#** extension for editing (see [Editing and debugging in VS Code / Cursor](#editing-and-debugging-in-vs-code--cursor)).

---

## First-time setup

1. **Clone** this repository.

2. **Create API settings** — `apisettings.json` is **not committed** (see `.gitignore`). Copy the example and adjust the API URL:

   ```powershell
   copy LogicPOS.Api\apisettings.example.json LogicPOS.Api\apisettings.json
   ```

   Edit `BaseAddress` to point at your backend.

3. **Restore NuGet packages** — from a **Developer PowerShell for VS** or any shell where **`msbuild`** resolves to Visual Studio’s MSBuild:

   ```bat
   msbuild LogicPOS.sln /t:Restore
   ```

   Or open **`LogicPOS.sln`** in Visual Studio and let it restore on load.

4. **Build** (see next section). The **`LogicPOS.Api`** project copies **`apisettings.json`** into the output folder next to **`logicpos.exe`** when you build the UI.

5. **Run the backend API**, then start **`logicpos.exe`** from the [build output folder](#where-build-output-goes).

---

## Build the solution

Always use **Visual Studio’s MSBuild** (not only the standalone .NET SDK) for the **full solution**.

### Option A — Visual Studio 2022 (simplest)

1. Open **`LogicPOS.sln`**.
2. Configuration: **Debug** (or **Release**), Platform: **Any CPU**.
3. **Build → Build Solution** (or Ctrl+Shift+B).

### Option B — Developer Command Prompt / PowerShell

Use **“Developer PowerShell for VS 2022”** or **“x64 Native Tools”** so **`msbuild`** and targets are on PATH:

```bat
msbuild LogicPOS.sln /restore /t:Build /p:Configuration=Debug /p:Platform="Any CPU" /verbosity:minimal
```

### Option C — VS Code / Cursor

- **Terminal → Run Build Task** (default **Ctrl+Shift+B**) runs **`.vscode/build-solution.ps1`**, which finds **`MSBuild.exe`** via **vswhere** (same toolchain as Visual Studio).
- Do **not** rely on **`dotnet msbuild`** alone for the full solution if **`LogicPOS.Globalization`** fails with **AL** / linker errors — use the script or Visual Studio.

---

## Where build output goes

**`LogicPOS.UI`** sets:

```text
..\..\artifacts\logicpos\Debug\
```

relative to the **`LogicPOS.UI`** project directory. That resolves to **next to the repo root**, not inside it:

| If the repo is… | Debug output example |
|-------------------|----------------------|
| `C:\...\logicpulse\logicpos\` | `C:\...\logicpulse\artifacts\logicpos\Debug\logicpos.exe` |

**Release** builds go to **`..\artifacts\logicpos\Release\`** (same layout).

After a successful build you should see **`logicpos.exe`**, **`GtkRuntime\`**, **`appsettings.json`**, **`LogicPOS.Api.dll`**, **`apisettings.json`** (if present under **`LogicPOS.Api`** at build time), and other dependencies in that folder.

---

## Run locally

1. Start your **backend API** (matching **`apisettings.json`**).
2. Run **`logicpos.exe`** from the **Debug** (or **Release**) output folder above.
3. **Logs** roll under **`Logs\log.txt`** next to the executable (see `Program.cs`).

If the app cannot reach the API, check **`BaseAddress`** and firewall/network.

---

## Configuration (quick reference)

| File | Project | Purpose |
|------|---------|---------|
| **`LogicPOS.UI/appsettings.json`** | UI | Themes, fonts, colors, paths, timeouts — copied to output. |
| **`LogicPOS.Api/apisettings.json`** | API client | **`BaseAddress`**, paging defaults — **gitignored**; create from **`apisettings.example.json`**. |
| **`LogicPOS.UI/App.config`** | UI | Licensing URLs, legacy service endpoints — adjust per environment. |

---

## Editing and debugging in VS Code / Cursor

- **Extension:** Use the **C#** extension (`ms-dotnettools.csharp`). **`dotnet.preferCSharpExtension`** is set in **`.vscode/settings.json`** so **C# Dev Kit** does not treat this repo as an SDK-style solution (Dev Kit targets modern .NET projects).
- **Build:** Use the **build** task (MSBuild via **`.vscode/build-solution.ps1`**), not raw **`dotnet build`** on the whole solution.
- **Debug:** **`launch.json`** uses **`"type": "clr"`** (**.NET Framework**). Do **not** use auto-generated **“.NET Core”** / **`dotnet`** profiles — they cause **`hostpolicy.dll`** / **`runtimeconfig.json`** errors.
- **64-bit:** The VS Code Desktop CLR debugger only supports **x64**. **Debug** builds set **`<Prefer32Bit>false</Prefer32Bit>`** in **`LogicPOS.UI.csproj`**. If native GTK binaries were **32-bit-only**, you could see load errors — use **Visual Studio** to debug **32-bit** managed apps, or restore **Prefer32Bit** for Debug after checking your **GtkRuntime** architecture.
- **Full-featured debugging** for this stack is often easier in **Visual Studio 2022**.

---

## Publish script

**`publish.ps1`** builds **`LogicPOS.UI`** with **`MSBuild`** and writes to **`..\..\artifacts\publish\pos\`** by default (relative to **`LogicPOS.UI`**, i.e. beside the repo under **`artifacts`**). Run from the repo root in a shell where **`MSBuild`** is on PATH (e.g. Developer PowerShell):

```powershell
.\publish.ps1 -Configuration Release
```

---

## Architecture (for contributors)

- **MediatR:** UI obtains **`ISender`** from **`DependencyInjection.Mediator`** and sends commands/queries defined in **`LogicPOS.Api`**.
- **Handlers** typically inherit **`RequestHandler<,>`** and return **`ErrorOr<T>`**; HTTP uses **`IHttpClientFactory`** client **`"Default"`** and **`AuthenticationData.Token`** when set.
- **Single instance:** a second **`logicpos`** process exits immediately (`Program.cs`).
- **Version check:** the app can warn if API and POS versions differ (`SystemVersionService`).

Third-party packages (GTK#, OxyPlot, PdfiumViewer, Serilog, ESC-POS, etc.) are listed in **`LogicPOS.UI.csproj`**.

---

## Troubleshooting builds

| Symptom | Likely cause | What to try |
|---------|----------------|-------------|
| **`LogicPOS.Globalization`** / **AL** / **ToolPath** errors with **`dotnet msbuild`** | SDK MSBuild vs full Framework toolchain | Build with **Visual Studio** or **`.vscode/build-solution.ps1`**. |
| **`apisettings.json` missing** at runtime | File never created under **`LogicPOS.Api`** | Copy from **`apisettings.example.json`** and rebuild the UI. |
| **`hostpolicy.dll`** / **`runtimeconfig.json`** when debugging in VS Code | Wrong debugger (**Core** host) | Use **`clr`** launch config; avoid **C# Dev Kit** taking **F5**. |
| **“Only x64 processes”** in VS Code | 32-bit EXE vs Desktop CLR limitation | Use **Debug** build (**Prefer32Bit** false) or debug in **Visual Studio**. |
