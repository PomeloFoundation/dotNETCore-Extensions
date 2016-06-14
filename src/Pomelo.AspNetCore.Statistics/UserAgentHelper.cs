using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AspNetCore.Statistics
{
    public static class UserAgentHelper
    {
        public static Browser ParseBrowser(string userAgent)
        {
            if (userAgent.IndexOf("MSIE") >= 0)
            {
                var begin = userAgent.IndexOf("MSIE") + 5;
                var version = userAgent.Substring(begin, userAgent.IndexOf(";", userAgent.IndexOf("MSIE")) - begin);
                if (userAgent.IndexOf("360SE") >= 0)
                {
                    return new Browser { Name = "Qihu 360 Security", Version = "IE " + version };
                }
                else if (userAgent.IndexOf("MetaSr") >= 0)
                {
                    return new Browser { Name = "Sougou", Version = "IE " + version };
                }
                else if (userAgent.IndexOf("Avant Browser") >= 0)
                {
                    return new Browser { Name = "Avant", Version = "IE " + version };
                }
                else if (userAgent.IndexOf("TencentTraveler") >= 0)
                {
                    return new Browser { Name = "Tencent Traveler", Version = "IE " + version };
                }
                else if (userAgent.IndexOf("Maxthon") >= 0)
                {
                    return new Browser { Name = "Maxthon", Version = "IE " + version };
                }
                return new Browser { Name = "Microsoft Internet Explorer", Version = version };
            }
            else if (userAgent.IndexOf("Edge") >= 0)
            {
                return new Browser { Name = "Microsoft Edge", Version = userAgent.Substring(userAgent.IndexOf("Edge") + 5) };
            }
            else if (userAgent.IndexOf("Chrome") >= 0)
            {
                var begin = userAgent.IndexOf("Chrome") + 7;
                var version = userAgent.Substring(begin, userAgent.IndexOf(" ", userAgent.IndexOf("Chrome")) - begin);
                return new Browser { Name = "Google Chrome", Version = version };
            }
            else if (userAgent.IndexOf("FireFox") >= 0)
            {
                var version = userAgent.Substring(userAgent.IndexOf("Firefox") + 8);
                return new Browser { Name = "Mozilla Firefox", Version = version };
            }
            else if (userAgent.IndexOf("Opera") >= 0)
            {
                var begin = userAgent.IndexOf("Opera") + 6;
                var version = userAgent.Substring(begin, userAgent.IndexOf(" ", userAgent.IndexOf("Opera")) - begin);
                return new Browser { Name = "Opera", Version = version };
            }
            else if (userAgent.IndexOf("Safari") >= 0)
            {
                if (userAgent.IndexOf("QQBrowser") >= 0)
                {
                    var b = userAgent.IndexOf("QQBrowser") + 10;
                    var e = userAgent.IndexOf(" ", b);
                    return new Browser { Name = "QQ Browser", Version = userAgent.Substring(b, e - b) };
                }
                var end = userAgent.IndexOf(" ", userAgent.IndexOf("Opera"));
                var begin = userAgent.IndexOf("Safari") + 7;
                var version = end < begin ? userAgent.Substring(begin) : userAgent.Substring(begin, end - begin);
                if (userAgent.IndexOf("UC") >= 0)
                {
                    return new Browser { Name = "UC Browser", Version = "Safari " + version };
                }
                return new Browser { Name = "Opera", Version = version };
            }
            else
            {
                return new Browser { Name = "Unknown", Version = "Unknown" };
            }
        }

        public static OperateSystem ParseOperateSystem(string userAgent)
        {
            var ret = new OperateSystem { Name = "Unknown", Version = "Unknown" };
            if (userAgent.IndexOf("Windows") >= 0)
            {
                ret.Name = "Windows";
                if (userAgent.IndexOf("Windows 98") >= 0)
                    ret.Version = "98";
                else if (userAgent.IndexOf("Windows ME") >= 0)
                    ret.Version = "Me";
                else if (userAgent.IndexOf("Windows NT 5.0") >= 0)
                    ret.Version = "2000";
                else if (userAgent.IndexOf("Windows NT 5.1") >= 0)
                    ret.Version = "XP";
                else if (userAgent.IndexOf("Windows NT 5.2") >= 0)
                    ret.Version = "2003";
                else if (userAgent.IndexOf("Windows NT 6.0") >= 0)
                    ret.Version = "Vista";
                else if (userAgent.IndexOf("Windows NT 6.1") >= 0)
                    ret.Version = "7";
                else if (userAgent.IndexOf("Windows NT 6.2") >= 0)
                    ret.Version = "8";
                else if (userAgent.IndexOf("Windows NT 6.3") >= 0)
                    ret.Version = "8.1";
                else if (userAgent.IndexOf("Windows NT 6.4") >= 0)
                    ret.Version = "10 Preview";
                else if (userAgent.IndexOf("Windows NT 10.0") >= 0)
                    ret.Version = "10";
                else if (userAgent.IndexOf("Windows NT 10.1") >= 0)
                    ret.Version = "10.1";
                else
                    ret.Version = "Unknown";
                return ret;
            }
            else if (userAgent.IndexOf("Mac OS X") >= 0)
            {
                ret.Name = "Mac OS X";
                var begin = userAgent.IndexOf("Mac OS X") + 9;
                var end = userAgent.IndexOf(";", begin);
                ret.Version = userAgent.Substring(begin, end - begin);
                return ret;
            }
            else if (userAgent.IndexOf("Linux") >= 0)
            {
                if (userAgent.IndexOf("Ubuntu") >= 0)
                {
                    var begin = userAgent.IndexOf("Ubuntu");
                    var end = userAgent.IndexOf(" ", begin);
                    if (end < begin) end = userAgent.Length;
                    ret.Version = userAgent.Substring(begin, end - begin);
                }
                ret.Name = "Linux";
            }
        }
    }
}
