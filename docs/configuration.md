# Configuration

TailwindBlazor works out of the box with sensible defaults. Everything is overridable through three methods: MSBuild properties, C# options, or appsettings.json.

## MSBuild Properties

Set these in your `.csproj` to control build-time behavior:

```xml
<PropertyGroup>
  <TailwindVersion>4.1.18</TailwindVersion>
  <TailwindInputFile>Styles/app.css</TailwindInputFile>
  <TailwindOutputFile>wwwroot/css/tailwind.css</TailwindOutputFile>
  <TailwindEnabled>true</TailwindEnabled>
  <TailwindMinify>false</TailwindMinify>
</PropertyGroup>
```

| Property | Default | Description |
|----------|---------|-------------|
| `TailwindVersion` | `4.1.18` | Tailwind CLI version to download |
| `TailwindInputFile` | `Styles/app.css` | CSS entry point relative to project root |
| `TailwindOutputFile` | `wwwroot/css/tailwind.css` | Generated CSS output path |
| `TailwindEnabled` | `true` | Enable/disable the build target |
| `TailwindMinify` | `$(Optimize)` | Minify output (auto in Release) |

## C# Options

Configure the runtime hosted service via the `UseTailwind` overload in `Program.cs`:

```csharp
builder.UseTailwind(options =>
{
    options.InputFile = "Styles/app.css";
    options.OutputFile = "wwwroot/css/tailwind.css";
    options.CliPath = "/custom/path/to/tailwindcss";
    options.TailwindVersion = "4.1.18";
});
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `InputFile` | `string` | `"Styles/app.css"` | CSS entry point relative to content root |
| `OutputFile` | `string` | `"wwwroot/css/tailwind.css"` | Generated CSS output path |
| `CliPath` | `string?` | `null` | Custom path to Tailwind CLI binary (overrides auto-download) |
| `TailwindVersion` | `string` | `"4.1.18"` | Tailwind CLI version to download |

## appsettings.json

Options are also bound from the `Tailwind` configuration section:

```json
{
  "Tailwind": {
    "InputFile": "Styles/app.css",
    "OutputFile": "wwwroot/css/tailwind.css",
    "TailwindVersion": "4.1.18"
  }
}
```

## Priority order

When the same option is set in multiple places, C# options take highest priority, then appsettings.json, then MSBuild defaults.

## Disable Tailwind for a specific build

```sh
dotnet build -p:TailwindEnabled=false
```

## Custom Tailwind CSS version

To pin a specific Tailwind CSS version:

```xml
<!-- .csproj -->
<PropertyGroup>
  <TailwindVersion>4.0.0</TailwindVersion>
</PropertyGroup>
```

Or at runtime:

```csharp
builder.UseTailwind(o => o.TailwindVersion = "4.0.0");
```

## Custom CLI path

If you have a pre-downloaded Tailwind CLI binary, skip auto-download:

```csharp
builder.UseTailwind(o => o.CliPath = "/usr/local/bin/tailwindcss");
```
