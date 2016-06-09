using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UserManagerExtensions
    {
        public static async Task<List<TUser>> GetUsersInAnyRolesAsync<TUser>(this UserManager<TUser> self, IEnumerable<string> Roles) where TUser : class
        {
            var ret = new List<TUser>();
            foreach (var r in Roles)
                ret.AddRange(await self.GetUsersInRoleAsync(r));
            return ret;
        }

        public static async Task<List<TUser>> GetUsersInAnyRolesAsync<TUser>(this UserManager<TUser> self, string Roles) where TUser : class
        {
            var ret = new List<TUser>();
            var roles = Roles.Split(',');
            for (var i = 0; i < roles.Count(); i++)
                roles[i] = roles[i].Trim(' ');
            foreach (var r in roles)
                ret.AddRange(await self.GetUsersInRoleAsync(r));
            return ret.Distinct().ToList();
        }

        public static async Task<List<TUser>> GetUsersInAnyRolesOrClaimAsync<TUser>(this UserManager<TUser> self, string Roles, Claim Claim) where TUser : class
        {
            var ret = new List<TUser>();
            var roles = Roles.Split(',');
            for (var i = 0; i < roles.Count(); i++)
                roles[i] = roles[i].Trim(' ');
            foreach (var r in roles)
                ret.AddRange(await self.GetUsersInRoleAsync(r));
            ret.AddRange(await self.GetUsersForClaimAsync(Claim));
            return ret.Distinct().ToList();
        }

        public static async Task<List<TUser>> GetUsersInAnyRolesOrClaimAsync<TUser>(this UserManager<TUser> self, string Roles, string Type, string Value) where TUser : class
        {
            return await GetUsersInAnyRolesOrClaimAsync(self, Roles, new Claim(Type, Value));
        }

        public static async Task<bool> IsInAnyRolesAsync<TUser>(this UserManager<TUser> self, TUser User, string Roles) where TUser : class
        {
            var roles = Roles.Split(',');
            for (var i = 0; i < roles.Count(); i++)
                roles[i] = roles[i].Trim(' ');
            foreach (var r in roles)
                if (await self.IsInRoleAsync(User, r))
                    return true;
            return false;
        }

        public static async Task<bool> IsInAnyRolesOrClaimsAsync<TUser>(this UserManager<TUser> self, TUser User, string Roles, List<Claim> Claims) where TUser : class
        {
            var roles = Roles.Split(',');
            for (var i = 0; i < roles.Count(); i++)
                roles[i] = roles[i].Trim(' ');
            foreach (var r in roles)
                if (await self.IsInRoleAsync(User, r))
                    return true;
            foreach(var c in Claims)
            {
                var users = await self.GetUsersForClaimAsync(c);
                if (users.Any(x => x == User)) return true;
            }
            return false;
        }

        public static async Task<bool> IsInAnyRolesOrClaimsAsync<TUser>(this UserManager<TUser> self, TUser User, string Roles, string Types, string Value) where TUser : class
        {
            var tmp = Types.Split(',');
            var claims = new List<Claim>();
            foreach (var c in tmp)
            {
                claims.Add(new Claim(c.Trim(' '), Value));
            }
            return await self.IsInAnyRolesOrClaimsAsync(User, Roles, claims);
        }
    }
}
