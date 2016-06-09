using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CookiesExtensions
    {
        public static IServiceCollection AddSmartCookies(this IServiceCollection self)
        {
            return self.AddScoped<SmartCookies>();
        }
    }
}
