# How It Works

TailwindBlazor has two independent mechanisms: MSBuild targets for build-time CSS generation, and a hosted service for dev-time watch mode.

## At Build Time (MSBuild)

MSBuild targets run **before** compilation:

1. Detect your OS and CPU architecture
2. Download the Tailwind CLI binary to `~/.tailwindblazor/cli/<version>/` (cached — only on first build)
3. Run `tailwindcss -i <input> -o <output>` to generate the CSS
4. In Release mode, `--minify` is applied automatically

The MSBuild targets are in `build/TailwindBlazor.targets` and `build/TailwindBlazor.props`, which are included automatically when you reference the NuGet package.

**You get CSS generation even without calling `UseTailwind()` in C#.** The MSBuild targets work independently of the runtime service.

## At Dev Time (Hosted Service)

When `ASPNETCORE_ENVIRONMENT=Development` and you call `builder.UseTailwind()`, the hosted service:

1. Ensures the CLI is downloaded (same cache as MSBuild)
2. Starts `tailwindcss --watch` as a background process
3. Streams CLI output to `ILogger` (debug-level for stdout, warning-level for stderr)
4. Kills the entire process tree on application shutdown

## Architecture

```
┌──────────────────────────────────────────────────┐
│  dotnet build / dotnet publish                   │
│  ┌────────────────────────────────────────────┐  │
│  │  MSBuild Targets                           │  │
│  │  1. _TailwindEnsureCli (download if needed)│  │
│  │  2. _TailwindBuildCss (compile CSS)        │  │
│  └────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────┐
│  dotnet run / dotnet watch (Development)         │
│  ┌────────────────────────────────────────────┐  │
│  │  TailwindHostedService                     │  │
│  │  1. EnsureCliAsync (download if needed)    │  │
│  │  2. Start tailwindcss --watch              │  │
│  │  3. Stream logs to ILogger                 │  │
│  │  4. Kill process on shutdown               │  │
│  └────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────┘
```

## CLI Cache

The Tailwind CLI binary is cached at `~/.tailwindblazor/cli/<version>/`. This is shared across all projects using the same version. The binary is only downloaded once per version.

Cache location: `~/.tailwindblazor/cli/4.1.18/tailwindcss-<platform>`

## Supported Platforms

| OS | Architecture | CLI Binary |
|----|-------------|------------|
| Windows | x64 | `tailwindcss-windows-x64.exe` |
| macOS | x64 | `tailwindcss-macos-x64` |
| macOS | ARM64 | `tailwindcss-macos-arm64` |
| Linux | x64 | `tailwindcss-linux-x64` |
| Linux | ARM64 | `tailwindcss-linux-arm64` |

## Tailwind CSS v4 Content Detection

Tailwind CSS v4 automatically scans your project files for class usage. You do **not** need a `tailwind.config.js` file. The CLI detects `.razor`, `.cshtml`, `.html`, and other template files in your project directory.

If you use a non-standard project structure, ensure your `.razor` files are within the project directory so the CLI can find them.
