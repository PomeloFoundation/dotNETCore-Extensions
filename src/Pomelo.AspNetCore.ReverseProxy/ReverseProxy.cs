using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Pomelo.AspNetCore.ReverseProxy.Extensions;

namespace Microsoft.AspNetCore.Builder
{
    public static class ReverseProxy
    {
        public static IApplicationBuilder UseReverseProxy(this IApplicationBuilder self, string origin, string dest, Func<string, string> textReplace = null)
        {
            self.Run(async (context) =>
            {
                HttpResponseMessage result;
                using (var handler = new HttpClientHandler())
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Clear();
                    foreach (var h in context.Request.Headers)
                    {
                        try
                        {
                            if (h.Key.ToLower() != "host" && h.Key.ToLower() != "accept-encoding" && h.Key.ToLower() != "cookie")
                                client.DefaultRequestHeaders.Add(h.Key, h.Value.ToString());
                        }
                        catch
                        {
                        }
                    }
                    client.DefaultRequestHeaders.Add("Cookie", context.Request.Headers["Cookie"].ToString());
                    if (dest.IndexOf("*") >= 0 && origin.IndexOf("*") >= 0)
                    {
                        var p1 = dest.Split('*');
                        var p2 = context.Request.Scheme + "://" + context.Request.Host;
                        var p3 = p2.Replace(p1[0], "").Replace(p1[1], "");
                        var p4 = origin.Split('*');
                        var p5 = origin.Replace("*", p3);
                        client.BaseAddress = new Uri(p5);
                    }
                    else
                    {
                        client.BaseAddress = new Uri(origin);
                    }
                    if (context.Request.Method.ToUpper() == "POST")
                    {
                        var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
                        try
                        {
                            foreach (var f in context.Request.Form.Files)
                                content.Add(new StreamContent(f.OpenReadStream()), f.GetFormFieldName(), f.GetFileName());
                            foreach (var x in context.Request.Form)
                                content.Add(new StringContent(x.Value), x.Key);
                        }
                        catch
                        {
                        }
                        result = await client.PostAsync(context.Request.Path + context.Request.QueryString.ToUriComponent(), content);
                    }
                    else
                    {
                        result = await client.GetAsync(context.Request.Path + context.Request.QueryString.ToUriComponent());
                    }
                    context.Response.StatusCode = (int)result.StatusCode;
                    foreach (var x in result.Content.Headers)
                    {
                        try
                        {
                            var values = new List<string>();
                            var tmp = origin.Split('*');
                            var tmp2 = dest.Split('*');
                            foreach (var v in x.Value)
                            {
                                var val = v.Replace(tmp[0], tmp2[0]);
                                if (tmp.Count() > 1 && tmp2.Count() > 1)
                                    val = val.Replace(tmp[1], tmp2[1]);
                                values.Add(val);
                            }
                            context.Response.Headers.Add(x.Key, new Microsoft.Extensions.Primitives.StringValues(values.ToArray()));
                        }
                        catch
                        {
                        }
                    }
                    if ((new string[] { "text/html", "text/javascript", "text/json" }).Contains(result?.Content?.Headers?.ContentType?.MediaType))
                    {
                        var body = await result.Content.ReadAsStringAsync();
                        var Rule = new Regex(origin.Replace("*", "[a-zA-Z0-9_\\-]{0,}"));
                        var match = Rule.Matches(body);
                        foreach (Match x in match)
                        {
                            var tmp = origin.Split('*');
                            if (tmp.Length > 1)
                            {
                                var tmp2 = x.Value.Replace(tmp[0], "").Replace(tmp[1], "");
                                var tmp3 = dest.Replace("*", tmp2);
                                body = body.Replace(x.Value, tmp3);
                            }
                            else
                            {
                                body = body.Replace(origin, dest);
                            }
                        }
                        if (textReplace != null)
                            body = textReplace(body);
                        await context.Response.WriteAsync(body);
                    }
                    else
                    {
                        var stream = await result.Content.ReadAsStreamAsync();
                        stream.CopyTo(context.Response.Body);
                    }
#if NET451
                    context.Response.Body.Close();
#else
                    context.Response.Body.Flush();
#endif
                }
            });
            return self;
        }
    }
}
