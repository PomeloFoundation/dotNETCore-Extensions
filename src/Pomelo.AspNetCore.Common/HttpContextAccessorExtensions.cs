using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpContextAccessorExtensions
    {
        public static IServiceCollection AddContextAccessor(this IServiceCollection self)
        {
            if (self.Count(x => x.ServiceType == typeof(IHttpContextAccessor)) == 0)
                self.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            if (self.Count(x => x.ServiceType == typeof(IActionContextAccessor)) == 0)
                self.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            return self;
        }
    }
}
