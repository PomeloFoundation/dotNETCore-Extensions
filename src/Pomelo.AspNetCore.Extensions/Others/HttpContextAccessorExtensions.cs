using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpContextAccessorExtensions
    {
        public static IServiceCollection AddHttpContextAccessor(this IServiceCollection self)
        {
            return self.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }
    }
}
