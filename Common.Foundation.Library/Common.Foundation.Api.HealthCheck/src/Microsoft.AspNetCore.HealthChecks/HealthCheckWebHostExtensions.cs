// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Common.Foundation.Api.HealthCheck.Microsoft.Extensions.HealthChecks;
using Microsoft.AspNetCore.Hosting;

namespace Common.Foundation.Api.HealthCheck.Microsoft.AspNetCore.HealthChecks
{
    public static class HealthCheckWebHostExtensions
    {
        private const int DEFAULT_TIMEOUT_SECONDS = 300;

        public static void RunWhenHealthy(this IWebHost webHost)
        {
            webHost.RunWhenHealthy(TimeSpan.FromSeconds(DEFAULT_TIMEOUT_SECONDS));
        }

        public static void RunWhenHealthy(this IWebHost webHost, TimeSpan timeout)
        {
            var healthChecks = webHost.Services.GetService(typeof(IHealthCheckService)) as IHealthCheckService;

            var loops = 0;
            do
            {
                var checkResult = healthChecks.CheckHealthAsync().Result;
                if (checkResult.CheckStatus == CheckStatus.Healthy)
                {
                    webHost.Run();
                    break;
                }

                System.Threading.Thread.Sleep(1000);
                loops++;

            } while (loops < timeout.TotalSeconds);
        }
    }
}
