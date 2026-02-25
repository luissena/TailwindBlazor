# Troubleshooting

## CLI download fails

**Symptom**: Build fails with a network error during CLI download.

**Cause**: The CLI is downloaded from `github.com/tailwindlabs/tailwindcss/releases`. Firewalls or proxy servers may block this.

**Fix**: Download the binary manually and set the path:

```xml
<!-- .csproj -->
<PropertyGroup>
  <TailwindCliPath>/path/to/tailwindcss</TailwindCliPath>
</PropertyGroup>
```

Or in C#:

```csharp
builder.UseTailwind(o => o.CliPath = "/path/to/tailwindcss");
```

## CSS is empty or missing classes

**Symptom**: The generated `tailwind.css` file is empty or doesn't include expected utility classes.

**Cause**: Tailwind CSS v4 scans your project files automatically. If your `.razor` files are outside the project directory, the CLI won't find them.

**Fix**: Ensure all `.razor`, `.cshtml`, and `.html` files with Tailwind classes are inside the project directory. Tailwind v4 does not require a `tailwind.config.js` — it detects content files automatically.

## Watch mode not starting

**Symptom**: CSS doesn't rebuild when you edit `.razor` files during development.

**Cause**: The hosted service only runs when `ASPNETCORE_ENVIRONMENT=Development`.

**Fix**: Run your app with:

```sh
dotnet watch
```

Or:

```sh
dotnet run --environment Development
```

Also verify you have `builder.UseTailwind()` in your `Program.cs`.

## Disable Tailwind for a specific build

```sh
dotnet build -p:TailwindEnabled=false
```

## CSS not found at runtime

**Symptom**: The browser returns 404 for `css/tailwind.css`.

**Fix**: Verify the output path matches your stylesheet reference:

```html
<!-- Must match TailwindOutputFile (default: wwwroot/css/tailwind.css) -->
<link rel="stylesheet" href="css/tailwind.css" />
```

## Build is slow

**Symptom**: First build takes longer than expected.

**Cause**: The Tailwind CLI is being downloaded for the first time.

**Fix**: This only happens once per Tailwind version. Subsequent builds use the cached binary at `~/.tailwindblazor/cli/<version>/`.
