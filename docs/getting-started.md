# Getting Started

TailwindBlazor integrates Tailwind CSS v4 into your Blazor app with zero external dependencies. No Node.js, no npm, no manual CLI installs.

## Installation

```sh
dotnet add package TailwindBlazor
```

## Setup (3 steps)

### 1. Create the CSS entry point

Create a file at `Styles/app.css` in your project root:

```css
@import "tailwindcss";
```

### 2. Register the service

Add one line to your `Program.cs`:

```csharp
using TailwindBlazor;

var builder = WebApplication.CreateBuilder(args);
builder.UseTailwind();
```

### 3. Reference the generated CSS

Add the stylesheet link in your `App.razor` or `_Host.cshtml`:

```html
<link rel="stylesheet" href="css/tailwind.css" />
```

That's it. Use Tailwind classes in any `.razor` file:

```html
<h1 class="text-3xl font-bold text-gray-900">Hello, TailwindBlazor!</h1>
```

## What happens automatically

- **Build time**: MSBuild targets download the Tailwind CLI (cached at `~/.tailwindblazor/cli/`) and compile CSS before your project builds.
- **Dev time**: A hosted service runs `tailwindcss --watch` in the background for instant CSS rebuilds when `ASPNETCORE_ENVIRONMENT=Development`.
- **Publish**: CSS is minified automatically in Release configuration.

## Minimal complete example

```csharp
// Program.cs
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

## Compatibility

- .NET 8.0+
- Blazor Server, Blazor WebAssembly (hosted), Blazor Web App
- Tailwind CSS v4
- Windows (x64), macOS (x64, ARM64), Linux (x64, ARM64)
