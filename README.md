# File Converter Suite (Windows 11, .NET Framework 4.8)

## SECTION 1 — SYSTEM ARCHITECTURE OVERVIEW

### High-level design
- **Presentation Layer** (`FileConverterSuite.Presentation.Wpf`): WPF MVVM desktop shell, drag-and-drop, commands, theme switching.
- **Application Layer** (`FileConverterSuite.Application`): orchestration services, conversion engine, queue contracts, validation contracts.
- **Domain Layer** (`FileConverterSuite.Domain`): conversion entities, formats, plugin interfaces, category enums.
- **Infrastructure Layer** (`FileConverterSuite.Infrastructure`): DI wiring, external tool plugins, registry, queue implementation, logging, configuration.

### Technology decisions
- **Framework:** .NET Framework 4.8 for long-term Windows enterprise compatibility.
- **UI:** WPF + MVVM (CommunityToolkit.Mvvm) for maintainable data binding and command patterns.
- **Plugin system:** runtime registration of `IConversionPlugin` for modular extension.
- **Processing strategy:** asynchronous execution with `SemaphoreSlim` throttling to keep UI responsive and thread-safe.
- **Installer choice:** **Inno Setup** for deterministic EXE installer generation, runtime checks, and bundled tools support.

### Folder structure diagram
```text
FileConverterSuite/
├─ src/
│  ├─ FileConverterSuite.Domain/
│  ├─ FileConverterSuite.Application/
│  ├─ FileConverterSuite.Infrastructure/
│  └─ FileConverterSuite.Presentation.Wpf/
├─ installer/
│  └─ InnoSetup/
└─ docs/
```

## SECTION 2 — COMPLETE PROJECT STRUCTURE

- `FileConverterSuite.sln`
- `src/FileConverterSuite.Domain`
  - `Interfaces/IConversionPlugin.cs`
  - `Interfaces/IConversionRegistry.cs`
  - `Entities/ConversionTask.cs`
- `src/FileConverterSuite.Application`
  - `Services/ConversionEngine.cs`
  - `Abstractions/IConversionEngine.cs`
- `src/FileConverterSuite.Infrastructure`
  - `Conversion/ConversionCatalog.cs`
  - `Conversion/ExternalToolPlugin.cs`
  - `Hosting/PluginLoader.cs`
  - `Services.cs`
- `src/FileConverterSuite.Presentation.Wpf`
  - `Views/MainWindow.xaml`
  - `ViewModels/MainWindowViewModel.cs`
  - `Themes/LightTheme.xaml`
  - `Themes/DarkTheme.xaml`
  - `appsettings.json`
- `installer/InnoSetup/FileConverterSuite.iss`

## SECTION 3 — CORE ENGINE IMPLEMENTATION

- `ConversionEngine` resolves plugin by pair (`.src->.dst`), validates source/output, executes asynchronously, and logs telemetry.
- `ConversionCatalog` dynamically creates a matrix of format pairs for each tool family:
  - Document matrix: 10 formats → 90 utilities.
  - Image matrix: 10 formats → 90 utilities.
  - Archive matrix: 6 formats → 30 utilities.
  - Media matrix: 10 formats → 90 utilities.
  - **Total built-in conversion utilities = 300**.
- `ExternalToolPlugin` executes stable external engines:
  - LibreOffice (`MPL v2.0`) for office/document conversion.
  - ImageMagick (`Apache-2.0`) for image conversion.
  - 7-Zip (`LGPL`) for archive operations.
  - FFmpeg (`LGPL/GPL build selectable`) for media conversion.

## SECTION 4 — UI IMPLEMENTATION

- `MainWindow.xaml` includes:
  - command bar (`Add Files`, `Convert All`, `Toggle Theme`)
  - conversion queue grid
  - global progress bar
- `MainWindowViewModel` includes:
  - command binding via `RelayCommand`
  - batch processing loop
  - drag-and-drop integration via `HandleDroppedFilesAsync`
- Theme system via merged dictionaries (`LightTheme.xaml`, `DarkTheme.xaml`).

## SECTION 5 — INSTALLER CREATION

### Full installer config
- Inno Setup script is at `installer/InnoSetup/FileConverterSuite.iss`.
- Installer bundles app binaries plus third-party local tooling from `third_party/tools`.

### Packaging steps
1. Build solution in **Release | Any CPU**.
2. Ensure `third_party/tools` contains `ffmpeg`, `7zip`, `ImageMagick`, `LibreOffice` portable or deployed executables.
3. Open script in Inno Setup Compiler and build.

### Versioning
- Update `#define MyAppVersion "1.0.0"` in `.iss`.
- Keep assembly version synchronized in project properties.

### Final installer generation
```powershell
iscc installer\InnoSetup\FileConverterSuite.iss
```
Output: `artifacts/installer/FileConverterSuite-Setup-1.0.0.exe`

## SECTION 6 — BUILD & DEPLOYMENT GUIDE

### Build in Visual Studio
1. Open `FileConverterSuite.sln` with Visual Studio 2022.
2. Restore NuGet packages.
3. Build `Release` configuration.

### Publish steps
1. Verify `src/FileConverterSuite.Presentation.Wpf/bin/Release/net48` output.
2. Smoke-test conversion flows on clean Windows 11 VM.
3. Compile installer with Inno Setup.
4. Sign installer and executable with Authenticode certificate.

### Installer command
```powershell
iscc installer\InnoSetup\FileConverterSuite.iss
```

## SECTION 7 — SCALABILITY GUIDE

### Adding new conversion modules
1. Add new plugin definition in `ConversionCatalog.BuildDefinitions`.
2. Point to approved executable under `tools` root.
3. Supply conversion pair matrix.
4. Add argument strategy in `PluginLoader.BuildArguments` if tool requires custom flags.

### Architecture maintenance best practices
- Keep domain contracts tool-agnostic.
- Add one plugin per bounded conversion capability.
- Enforce extension-pair validation before execution.
- Route all long-running work through async engine methods.
- Keep tools local-first; remote conversion remains optional extension.

## NuGet packages
- `CommunityToolkit.Mvvm`
- `Microsoft.Xaml.Behaviors.Wpf`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Configuration.Json`
- `Microsoft.Extensions.Hosting`
- `Serilog`
- `Serilog.Sinks.File`

