# API Reference

TailwindBlazor exposes a minimal public API surface: one extension method, one options class, one hosted service, and one static utility class.

## TailwindServiceExtensions

**Namespace**: `TailwindBlazor`

Extension method to register TailwindBlazor services.

### UseTailwind

```csharp
public static WebApplicationBuilder UseTailwind(
    this WebApplicationBuilder builder,
    Action<TailwindOptions>? configure = null)
```

Registers the `TailwindHostedService` and binds `TailwindOptions` from the `"Tailwind"` configuration section. Optionally accepts a delegate to further configure options.

**Parameters**:
- `builder` — The `WebApplicationBuilder` instance
- `configure` — Optional delegate to configure `TailwindOptions`

**Returns**: The same `WebApplicationBuilder` for chaining.

**Usage**:

```csharp
using TailwindBlazor;

var builder = WebApplication.CreateBuilder(args);

// Minimal (uses defaults)
builder.UseTailwind();

// With custom options
builder.UseTailwind(options =>
{
    options.InputFile = "Styles/app.css";
    options.OutputFile = "wwwroot/css/tailwind.css";
    options.CliPath = "/custom/path/to/tailwindcss";
    options.TailwindVersion = "4.1.18";
});
```

## TailwindOptions

**Namespace**: `TailwindBlazor`

Configuration options for the Tailwind CSS integration.

```csharp
public class TailwindOptions
{
    public string InputFile { get; set; } = "Styles/app.css";
    public string OutputFile { get; set; } = "wwwroot/css/tailwind.css";
    public string? CliPath { get; set; }
    public string TailwindVersion { get; set; } = "4.1.18";
}
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `InputFile` | `string` | `"Styles/app.css"` | CSS entry point relative to the content root |
| `OutputFile` | `string` | `"wwwroot/css/tailwind.css"` | Generated CSS output path relative to the content root |
| `CliPath` | `string?` | `null` | Absolute path to a custom Tailwind CLI binary. When set, skips auto-download. |
| `TailwindVersion` | `string` | `"4.1.18"` | Tailwind CSS CLI version to download from GitHub releases |

**Binding from appsettings.json**:

```json
{
  "Tailwind": {
    "InputFile": "Styles/app.css",
    "OutputFile": "wwwroot/css/tailwind.css",
    "TailwindVersion": "4.1.18"
  }
}
```

## TailwindHostedService

**Namespace**: `TailwindBlazor`

An `IHostedService` that runs the Tailwind CSS CLI in watch mode during development.

- **Only active when** `IHostEnvironment.IsDevelopment()` returns `true`
- **On start**: Downloads CLI if needed, then spawns `tailwindcss --watch` as a background process
- **On stop**: Kills the entire process tree
- **Logging**: Streams stdout to `ILogger` at Debug level, stderr at Warning level

This service is registered automatically when you call `builder.UseTailwind()`.

## TailwindCliDownloader

**Namespace**: `TailwindBlazor`

Static utility class for managing the Tailwind CLI binary.

### Methods

```csharp
public static string GetCacheDirectory(string version)
```
Returns the cache directory path: `~/.tailwindblazor/cli/<version>/`

```csharp
public static string GetPlatformIdentifier()
```
Returns the platform string (e.g., `"macos-arm64"`, `"linux-x64"`, `"windows-x64"`).

```csharp
public static string GetBinaryName(string platform)
```
Returns the CLI binary filename (e.g., `"tailwindcss-macos-arm64"`, `"tailwindcss-windows-x64.exe"`).

```csharp
public static string GetDownloadUrl(string version, string binaryName)
```
Returns the GitHub releases download URL for the specified version and binary.

```csharp
public static string ResolveCliPath(TailwindOptions options)
```
Returns the full path to the CLI binary. Uses `options.CliPath` if set, otherwise resolves from cache.

```csharp
public static async Task EnsureCliAsync(
    TailwindOptions options,
    ILogger? logger = null,
    CancellationToken cancellationToken = default)
```
Downloads the CLI binary if not already cached. No-op if the binary already exists.

## MSBuild Properties

These are set automatically by the NuGet package and can be overridden in your `.csproj`:

| Property | Default | Description |
|----------|---------|-------------|
| `TailwindVersion` | `4.1.18` | CLI version |
| `TailwindInputFile` | `Styles/app.css` | CSS entry point |
| `TailwindOutputFile` | `wwwroot/css/tailwind.css` | CSS output path |
| `TailwindEnabled` | `true` | Enable/disable build target |
| `TailwindMinify` | `$(Optimize)` | Minify in Release |
| `TailwindCliPath` | Auto-resolved | Custom CLI path |

## MSBuild Targets

| Target | Runs Before | Description |
|--------|-------------|-------------|
| `_TailwindEnsureCli` | `_TailwindBuildCss` | Downloads CLI if not cached |
| `_TailwindBuildCss` | `BeforeBuild` | Compiles CSS from input to output |
