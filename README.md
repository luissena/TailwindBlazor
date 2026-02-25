<p align="center">
  <img src="samples/SampleBlazorApp/wwwroot/favicon.png" alt="TailwindBlazor" width="64" height="64" />
</p>

<h1 align="center">TailwindBlazor</h1>

<p align="center">
  Zero-config <a href="https://tailwindcss.com/">Tailwind CSS v4</a> integration for <a href="https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor">Blazor</a>. No Node.js, no npm — just <code>dotnet add package</code> and go.
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/TailwindBlazor"><img src="https://img.shields.io/nuget/v/TailwindBlazor?color=8b5cf6&label=NuGet" alt="NuGet" /></a>
  <a href="https://www.nuget.org/packages/TailwindBlazor"><img src="https://img.shields.io/nuget/dt/TailwindBlazor?color=8b5cf6&label=Downloads" alt="NuGet Downloads" /></a>
  <a href="https://tailwind-blazor.com"><img src="https://img.shields.io/badge/Docs-tailwind--blazor.com-8b5cf6" alt="Docs" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/github/license/luissena/TailwindBlazor?color=8b5cf6" alt="MIT License" /></a>
</p>

<p align="center">
  <a href="https://tailwind-blazor.com">Website</a> · <a href="https://tailwind-blazor.com/docs">Docs</a> · <a href="https://www.nuget.org/packages/TailwindBlazor">NuGet</a>
</p>

## Quick Start

```sh
dotnet add package TailwindBlazor
```

```css
/* Styles/app.css */
@import "tailwindcss";
```

```csharp
// Program.cs
using TailwindBlazor;

var builder = WebApplication.CreateBuilder(args);
builder.UseTailwind();
```

```html
<!-- App.razor or _Host.cshtml -->
<link rel="stylesheet" href="css/tailwind.css" />
```

```html
<h1 class="text-3xl font-bold text-gray-900">Hello, TailwindBlazor!</h1>
```

## Why TailwindBlazor

Tailwind CSS requires a CLI to scan your files and generate CSS. In JavaScript projects, npm handles this. In .NET, there's no npm. TailwindBlazor bridges this gap by:

- **Auto-downloading** the standalone Tailwind CLI binary (no Node.js required)
- **Integrating with MSBuild** so CSS is generated on every build
- **Running watch mode** during development via an `IHostedService`
- **Caching the CLI** at `~/.tailwindblazor/cli/` (shared across projects)

## Features

| Feature | Description |
|---------|-------------|
| Auto CLI management | Downloads the correct binary for your OS/arch automatically |
| MSBuild integration | CSS compiles at build time, minifies in Release |
| Watch mode | `--watch` runs during development for instant rebuilds |
| Zero config | Sensible defaults, everything overridable |
| Multi-platform | Windows x64, macOS x64/ARM64, Linux x64/ARM64 |

## Configuration

TailwindBlazor works with zero configuration. Override defaults when needed:

### MSBuild Properties (.csproj)

```xml
<PropertyGroup>
  <TailwindVersion>4.1.18</TailwindVersion>
  <TailwindInputFile>Styles/app.css</TailwindInputFile>
  <TailwindOutputFile>wwwroot/css/tailwind.css</TailwindOutputFile>
  <TailwindEnabled>true</TailwindEnabled>
  <TailwindMinify>false</TailwindMinify>
</PropertyGroup>
```

### C# Options (Program.cs)

```csharp
builder.UseTailwind(options =>
{
    options.InputFile = "Styles/app.css";
    options.OutputFile = "wwwroot/css/tailwind.css";
    options.CliPath = "/custom/path/to/tailwindcss";
    options.TailwindVersion = "4.1.18";
});
```

### appsettings.json

```json
{
  "Tailwind": {
    "InputFile": "Styles/app.css",
    "OutputFile": "wwwroot/css/tailwind.css",
    "TailwindVersion": "4.1.18"
  }
}
```

## How It Works

**Build time** — MSBuild targets run before compilation:
1. Detect OS and architecture
2. Download Tailwind CLI to `~/.tailwindblazor/cli/<version>/` (cached)
3. Run `tailwindcss -i <input> -o <output>`
4. Minify automatically in Release mode

**Dev time** — The hosted service (when `ASPNETCORE_ENVIRONMENT=Development`):
1. Ensure CLI is downloaded
2. Start `tailwindcss --watch` as a background process
3. Stream logs to `ILogger`
4. Kill the process on shutdown

> MSBuild targets and the hosted service work independently. You get CSS generation even without calling `UseTailwind()`.

## Common Patterns

### Blazor Server with Interactive Components

```csharp
using TailwindBlazor;

var builder = WebApplication.CreateBuilder(args);
builder.UseTailwind();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
```

### Custom Tailwind version

```csharp
builder.UseTailwind(o => o.TailwindVersion = "4.0.0");
```

### Pre-downloaded CLI (CI/CD, air-gapped)

```csharp
builder.UseTailwind(o => o.CliPath = "/usr/local/bin/tailwindcss");
```

### Disable Tailwind for a build

```sh
dotnet build -p:TailwindEnabled=false
```

## Anti-patterns

| Don't | Do Instead |
|-------|------------|
| Install Node.js/npm for Tailwind | Use TailwindBlazor — it downloads the standalone CLI |
| Create `tailwind.config.js` | Tailwind v4 detects content files automatically |
| Manually download the CLI | Let TailwindBlazor manage it (or set `CliPath` once) |
| Run `tailwindcss --watch` manually | Call `builder.UseTailwind()` — the hosted service handles it |

## Supported Platforms

| OS | Architecture |
|----|-------------|
| Windows | x64 |
| macOS | x64, ARM64 |
| Linux | x64, ARM64 |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| CLI download fails | Check firewall; set `CliPath` to a manually downloaded binary |
| CSS is empty | Ensure `.razor` files are in the project directory |
| Watch mode not starting | Verify `ASPNETCORE_ENVIRONMENT=Development` and `UseTailwind()` is called |
| Slow first build | CLI downloads once per version; subsequent builds are fast |

## Links

- [Website & Docs](https://tailwind-blazor.com)
- [NuGet Package](https://www.nuget.org/packages/TailwindBlazor)
- [GitHub](https://github.com/luissena/TailwindBlazor)
- [API Reference](docs/api-reference.md)

## License

[MIT](LICENSE)
