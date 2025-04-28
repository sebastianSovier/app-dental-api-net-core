// <copyright file="ConfigureSerilogInjection.cs" company="NeoSolTec Spa">
// Copyright (c) NeoSolTec Spa. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// More info about MIT license in https://opensource.org/license/mit
// </copyright>

namespace app_dental_api.ConfigurationInjection;

using Microsoft.Extensions.Hosting;
using Serilog;

/// <summary>
/// Configure swagger injection.
/// </summary>
internal static class ConfigureSerilogInjection
{
    /// <summary>
    /// Configure Serilog.
    /// </summary>
    /// <param name="hostBuilder">Builder host application.</param>
    /// <returns>Interface host application builder.</returns>
    internal static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        // Todo: add sink to log to database: https://github.com/b00ted/serilog-sinks-postgresql
        // Todo: add sink to log to seq: https://docs.datalust.co/docs/using-serilog
        // todo: add sink to log async: https://github.com/serilog/serilog-sinks-async
        hostBuilder
            .UseSerilog(
                static (context, configuration) =>
                    configuration.ReadFrom.Configuration(context.Configuration));

        return hostBuilder;
    }
}