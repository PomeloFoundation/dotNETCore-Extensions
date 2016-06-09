using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection self, out IConfiguration config, string fileName = "config")
        {
            var services = self.BuildServiceProvider();
            var env = services.GetRequiredService<IHostingEnvironment>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"{fileName}.json")
                .AddJsonFile($"{fileName}.{env.EnvironmentName}.json", optional: true);
            var configuration = builder.Build();
            self.AddSingleton<IConfiguration>(configuration);
            config = configuration;

            return self;
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection self, string fileName = "config")
        {
            var services = self.BuildServiceProvider();
            var env = services.GetRequiredService<IHostingEnvironment>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"{fileName}.json")
                .AddJsonFile($"{fileName}.{env.EnvironmentName}.json", optional: true);
            var configuration = builder.Build();
            self.AddSingleton<IConfiguration>(configuration);

            return self;
        }
    }
}
