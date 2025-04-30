// <copyright file="ConfigureCors.cs" company="NeoSolTec Spa">
// Copyright (c) NeoSolTec Spa. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// More info about MIT license in https://opensource.org/license/mit
// </copyright>

namespace app_dental_api.ConfigurationInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

/// <summary>
/// Configure swagger injection.
/// </summary>
internal static class ConfigureCors
{
    /// <summary>
    /// Setup CORS Service.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="policyName">Policy name.</param>
    /// <param name="configuration">Application configuration collection.</param>
    /// <returns>Service collection instance.</returns>
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

    /// <summary>
    /// AddSwagger.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <param name="policyName">Policy name.</param>
    /// <param name="useCors">Use CORS flag.</param>
    /// <returns>IServiceCollection instance.</returns>
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