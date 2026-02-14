using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TailwindBlazor;

public static class TailwindServiceExtensions
{
    public static WebApplicationBuilder UseTailwind(this WebApplicationBuilder builder, Action<TailwindOptions>? configure = null)
    {
        builder.Services.Configure<TailwindOptions>(builder.Configuration.GetSection("Tailwind"));

        if (configure != null)
            builder.Services.Configure(configure);

        builder.Services.AddHostedService<TailwindHostedService>();

        return builder;
    }
}
