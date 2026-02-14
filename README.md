# TailwindBlazor

Zero-config [Tailwind CSS v4](https://tailwindcss.com/) integration for Blazor.

`dotnet add package TailwindBlazor` — that's it. The CLI downloads automatically, CSS compiles at build time, and watch mode runs in development.

## Features

- **Auto CLI management** — Downloads the correct Tailwind CLI binary for your OS/arch and caches it at `~/.tailwindblazor/cli/`
- **MSBuild integration** — CSS is generated at build time, no manual steps
- **Watch mode** — Hosted service runs `--watch` during development for instant rebuilds
- **Zero config** — Sensible defaults work out of the box, everything is overridable

## Quick Start

### 1. Install

```bash
dotnet add package TailwindBlazor
```

### 2. Create your CSS entry point

```css
/* Styles/app.css */
@import "tailwindcss";
```

### 3. Register the service

```csharp
// Program.cs
using TailwindBlazor;

var builder = WebApplication.CreateBuilder(args);
builder.UseTailwind();
```

### 4. Reference the generated CSS

```html
<!-- App.razor or _Host.cshtml -->
<link rel="stylesheet" href="css/tailwind.css" />
```

### 5. Use Tailwind classes

```html
<h1 class="text-3xl font-bold text-gray-900">Hello, TailwindBlazor!</h1>
```

## Configuration

### MSBuild Properties

Override in your `.csproj`:

```xml
<PropertyGroup>
  <TailwindVersion>4.1.18</TailwindVersion>
  <TailwindInputFile>Styles/app.css</TailwindInputFile>
  <TailwindOutputFile>wwwroot/css/tailwind.css</TailwindOutputFile>
  <TailwindEnabled>true</TailwindEnabled>
  <TailwindMinify>false</TailwindMinify>
</PropertyGroup>
```

### Runtime Options

```csharp
builder.UseTailwind(options =>
{
    options.InputFile = "Styles/app.css";
    options.OutputFile = "wwwroot/css/tailwind.css";
    options.CliPath = "/custom/path/to/tailwindcss"; // optional override
});
```

Or via `appsettings.json`:

```json
{
  "Tailwind": {
    "InputFile": "Styles/app.css",
    "OutputFile": "wwwroot/css/tailwind.css"
  }
}
```

## How It Works

**At build time**, MSBuild targets:
1. Detect your OS and architecture
2. Download the Tailwind CLI to `~/.tailwindblazor/cli/<version>/` (cached)
3. Run `tailwindcss -i <input> -o <output>` to generate the CSS

**At dev time**, the hosted service:
1. Ensures the CLI is downloaded
2. Starts `tailwindcss --watch` as a background process
3. Logs output via `ILogger`
4. Kills the process tree on application shutdown

## Supported Platforms

| OS | Architecture |
|----|-------------|
| Windows | x64 |
| macOS | x64, ARM64 |
| Linux | x64, ARM64 |

## License

[MIT](LICENSE)
