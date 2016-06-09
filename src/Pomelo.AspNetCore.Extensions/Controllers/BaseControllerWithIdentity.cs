using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc
{
    public abstract class BaseController<TContext, TUser, TKey> : BaseController<TContext>
    where TKey : IEquatable<TKey>
    where TUser : IdentityUser<TKey>
    {
        public UserManager<TUser> UserManager { get { return HttpContext.RequestServices?.GetService<UserManager<TUser>>(); } }
        
        public SignInManager<TUser> SignInManager { get { return HttpContext.RequestServices?.GetService<SignInManager<TUser>>(); } }
        
        public RoleManager<TUser> RoleManager { get { return HttpContext.RequestServices?.GetService<RoleManager<TUser>>(); } }
        
        public new SmartUser<TUser, TKey> User { get { return HttpContext.RequestServices?.GetService<SmartUser<TUser, TKey>>(); } }
    }
}
