using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.AntiXSS;

namespace Pomelo.AspNetCore.AntiXSS.Json
{
    public class JsonTagAuthorizationProvider<TUser, TKey> : ITagAuthorizationProvider
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        private JsonWhiteListProvider whitelist { get; set; }

        private IServiceProvider services { get; set; }

        private string ClaimType { get; set; }

        public JsonTagAuthorizationProvider(IWhiteListProvider wlp, IServiceProvider services, string ClaimType = "AntiXSS WhiteList")
        {
            whitelist = wlp as JsonWhiteListProvider;
            this.services = services;
            this.ClaimType = ClaimType;
        }

        public bool IsAbleToUse(string tag, object UserId = null)
        {
            var task = _IsAbleToUse(tag, UserId);
            task.Wait();
            return task.Result;
        }

        public async Task<bool> _IsAbleToUse(string tag, object UserId = null)
        {
            if (!whitelist.RawWhiteList.ContainsKey(tag)) return false;
            string[] role = null;
            if (whitelist.RawWhiteList[tag].Type == Newtonsoft.Json.Linq.JTokenType.Array)
                return true;
            try
            {
                if (whitelist.RawWhiteList[tag].role.Type == Newtonsoft.Json.Linq.JTokenType.String)
                {
                    role = new string[] { whitelist.RawWhiteList[tag].role.ToString() };
                }
                else
                {
                    role = whitelist.RawWhiteList[tag].role.ToObject<string[]>();
                }
            }
            catch
            {
                return true;
            }
            if (role == null) return true;
            if (UserId==null)
                return false;
            var UserManager = services.GetRequiredService<UserManager<TUser>>();
            var user = await UserManager.FindByIdAsync(UserId.ToString());
            var claims = await UserManager.GetClaimsAsync(user);
            var claimflag = claims.Where(x => x.Type == ClaimType && x.Value == tag).Count() > 0;
            var roleflag = (await UserManager.GetRolesAsync(user)).Where(x => role.Contains(x)).Count() > 0;
            if (claimflag || roleflag) return true;
            return false;
        }

        public bool IsAbleToUse(string tag, string attribute, object UserId = null)
        {
            var task = _IsAbleToUse(tag, attribute, UserId);
            task.Wait();
            return task.Result;
        }

        public async Task<bool> _IsAbleToUse(string tag, string attribute, object UserId = null)
        {
            if (!whitelist.RawWhiteList.ContainsKey(tag)) return false;
            string[] role = null;
            if (whitelist.RawWhiteList[tag].Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                foreach (var x in whitelist.RawWhiteList[tag])
                {
                    if (x.Type == Newtonsoft.Json.Linq.JTokenType.String && x.ToString() == attribute)
                        return true;

                    if (!(x.Type == Newtonsoft.Json.Linq.JTokenType.String) && x.attribute == attribute)
                    {
                        try
                        {
                            if (x.role.Type == Newtonsoft.Json.Linq.JTokenType.String)
                            {
                                role = new string[] { x.role.ToString() };
                            }
                            else
                            {
                                role = x.role.ToObject<string[]>();
                            }
                            break;
                        }
                        catch
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (var x in whitelist.RawWhiteList[tag].attributes)
                {
                    if (x.Type == Newtonsoft.Json.Linq.JTokenType.String && x.ToString() == attribute)
                        return true;

                    if (!(x.Type == Newtonsoft.Json.Linq.JTokenType.String) && x.attribute == attribute)
                    {
                        try
                        {
                            if (x.role.Type == Newtonsoft.Json.Linq.JTokenType.String)
                            {
                                role = new string[] { x.role.ToString() };
                            }
                            else
                            {
                                role = x.role.ToObject<string[]>();
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            return true;
                        }
                    }
                }
            }
            
            if (role == null) return true;
            if (UserId == null)
                return false;
            var UserManager = services.GetRequiredService<UserManager<TUser>>();
            var user = await UserManager.FindByIdAsync(UserId.ToString());
            var claims = await UserManager.GetClaimsAsync(user);
            var claimflag = claims.Where(x => x.Type == ClaimType && x.Value == tag + "[" + attribute + "]").Count() > 0;
            var roleflag = (await UserManager.GetRolesAsync(user)).Where(x => role.Contains(x)).Count() > 0;
            if (claimflag || roleflag) return true;
            return false;
        }
    }
}
