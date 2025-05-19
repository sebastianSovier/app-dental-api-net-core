

namespace app_dental_api.ConfigurationInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;


internal static class ConfigureCors
{

    public static IServiceCollection SetupCorsService(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        string policyName)
    {

        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
        services
            .AddCors(options =>
            {
                options.AddPolicy(
                    policyName, policy =>
                    {
                        policy
                            .WithOrigins(allowedOrigins)
                            .SetPreflightMaxAge(TimeSpan.FromSeconds(2520))
                            .WithMethods("POST").WithHeaders("Authorization", "Content-Type", "Accept", "Accept-Encoding", "Accept-Language", "Cookie", "X-Q").AllowCredentials();
                    });
            });

        return services;
    }

    internal static IApplicationBuilder AddCors(
        this IApplicationBuilder app,
        string policyName,
        bool useCors)
    {
        if (!useCors)
        {
            return app;
        }

        app.UseCors(policyName);

        return app;
    }
}