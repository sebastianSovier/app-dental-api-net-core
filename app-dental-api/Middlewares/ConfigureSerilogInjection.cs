
namespace app_dental_api.ConfigurationInjection;

using Microsoft.Extensions.Hosting;
using Serilog;


internal static class ConfigureSerilogInjection
{

    internal static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {

        hostBuilder
            .UseSerilog(
                static (context, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration));

        return hostBuilder;
    }
}