using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpContextAccessorExtensions
    {
        public static IServiceCollection AddHttpContextAccessor(this IServiceCollection self)
        {
            return self.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
