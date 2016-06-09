using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SmartUserExtensions
    {
        public static IServiceCollection AddSmartUser<TUser, TKey>(this IServiceCollection self)
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            return self.AddScoped<SmartUser<TUser, TKey>>();
        }
    }
}
