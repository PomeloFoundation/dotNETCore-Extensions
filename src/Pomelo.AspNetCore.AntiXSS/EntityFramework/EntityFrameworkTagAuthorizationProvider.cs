using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AntiXSS;

namespace Pomelo.AspNetCore.AntiXSS.EntityFramework
{
    public class EntityFrameworkTagAuthorizationProvider<TContext, TUser, TKey> : ITagAuthorizationProvider
        where TContext : IAntiXSSDbContext
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private TContext DB { get; set; }
        private IServiceProvider services { get; set; }
        private string claimType { get; set; }

        public EntityFrameworkTagAuthorizationProvider(IServiceProvider services, string ClaimType = "AntiXSS WhiteList")
        {
            DB = services.GetRequiredService<TContext>();
            this.services = services;
            this.claimType = ClaimType;
        }

        public bool IsAbleToUse(string tag, object UserId = null)
        {
            var task = _IsAbleToUse(tag, UserId);
            task.Wait();
            return task.Result;
        }

        public async Task<bool> _IsAbleToUse(string tag, object UserId = null)
        {
            try
            {
                var t = DB.WhiteListTags.SingleOrDefault(x => x.Id == tag);
                if (t == null) return false;
                if (string.IsNullOrEmpty(t.RoleRequired)) return true;
                if (UserId == null)
                    return false;
                var UserManager = services.GetRequiredService<UserManager<TUser>>();
                var user = await UserManager.FindByIdAsync(UserId.ToString());
                var claims = await UserManager.GetClaimsAsync(user);
                var claimflag = claims.Where(x => x.Type == claimType && x.Value == tag).Count() > 0;
                var roleflag = (await UserManager.GetRolesAsync(user)).Where(x => x == t.RoleRequired).Count() > 0;
                if (claimflag || roleflag) return true;
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool IsAbleToUse(string tag, string attribute, object UserId = null)
        {
            var task = _IsAbleToUse(tag, attribute, UserId);
            task.Wait();
            return task.Result;
        }

        public async Task<bool> _IsAbleToUse(string tag, string attribute, object UserId = null)
        {
            var t = DB.WhiteListAttributes.SingleOrDefault(x => x.TagId == tag && x.Attribute == attribute);
            if (t == null) return false;
            if (string.IsNullOrEmpty(t.RoleRequired)) return true;
            if (UserId == null)
                return false;
            var UserManager = services.GetRequiredService<UserManager<TUser>>();
            var user = await UserManager.FindByIdAsync(UserId.ToString());
            var claims = await UserManager.GetClaimsAsync(user);
            var claimflag = claims.Where(x => x.Type == claimType && x.Value == tag + "[" + attribute + "]").Count() > 0;
            var roleflag = (await UserManager.GetRolesAsync(user)).Where(x => x == t.RoleRequired).Count() > 0;
            if (claimflag || roleflag) return true;
            return false;
        }
    }
}
