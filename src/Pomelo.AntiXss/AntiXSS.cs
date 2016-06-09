using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Xml;
using Pomelo.HtmlAgilityPack;

namespace Pomelo.AntiXSS
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public static class Instance
    {
        public static string Sanitize(string Content)
        {
            var inst = new AntiXSS(new DefaultWhiteListProvider());
            return inst.Sanitize(Content);
        }
    }

    public class AntiXSS
    {
        private IWhiteListProvider WhiteListProvider;

        public IDictionary<string, string[]> WhiteList
        {
            get
            {
                return WhiteListProvider.WhiteList;
            }
        }

        public AntiXSS(IWhiteListProvider whiteListProvider)
        {
            WhiteListProvider = whiteListProvider;
        }

        /// <summary>
        /// Sanitize Html
        /// </summary>
        /// <param name="Html">Html</param>
        /// <returns></returns>
        public string Sanitize(string Html)
        {
            return SanitizeHtml(Html, WhiteList);
        }

        private static volatile AntiXSS _instance;
        private static object _root = new object();

        // Original list courtesy of Robert Beal :
        // http://www.robertbeal.com/
        /// <summary>
        /// Takes raw HTML input and cleans against a whitelist
        /// </summary>
        /// <param name="source">Html source</param>
        /// <returns>Clean output</returns>
        public string SanitizeHtml(string source, IDictionary<string, string[]> WhiteListDic = null)
        {
            if (WhiteListDic == null)
                WhiteListDic = WhiteList;

            HtmlDocument html = GetHtml(source);
            if (html == null) return string.Empty;

            // All the nodes
            HtmlNode allNodes = html.DocumentNode;

            // Select whitelist tag names
            string[] whitelist = (from kv in WhiteListDic
                                  select kv.Key).ToArray();

            // Scrub tags not in whitelist
            CleanNodes(allNodes, whitelist);

            // Filter the attributes of the remaining
            foreach (KeyValuePair<string, string[]> tag in WhiteListDic)
            {
                IEnumerable<HtmlNode> nodes = (from n in allNodes.DescendantsAndSelf()
                                               where n.Name == tag.Key
                                               select n);

                // No nodes? Skip.
                if (nodes == null) continue;

                foreach (var n in nodes)
                {
                    // No attributes? Skip.
                    if (!n.HasAttributes) continue;

                    // Get all the allowed attributes for this tag
                    HtmlAttribute[] attr = n.Attributes.ToArray();
                    foreach (HtmlAttribute a in attr)
                    {
                        if (!tag.Value.Contains(a.Name))
                        {
                            a.Remove(); // Attribute wasn't in the whitelist
                        }
                        else
                        {
                            // *** New workaround. This wasn't necessary with the old library
                            if (a.Name == "href" || a.Name == "src")
                            {
                                a.Value = (!string.IsNullOrEmpty(a.Value)) ? a.Value.Replace("\r", "").Replace("\n", "") : "";
                                a.Value =
                                    (!string.IsNullOrEmpty(a.Value) &&
                                    (a.Value.IndexOf("javascript") < 10 || a.Value.IndexOf("eval") < 10)) ?
                                    a.Value.Replace("javascript", "").Replace("eval", "") : a.Value;
                            }
                            /*
                        else if (a.Name == "class" || a.Name == "style")
                        {
                            a.Value =
                                Microsoft.Security.Application.Encoder.CssEncode(a.Value);
                        }
                             * */
                            else
                            {
                                a.Value =
                                    Microsoft.Security.Application.Encoder.HtmlAttributeEncode(a.Value);
                            }
                        }
                    }
                }
            }

            // *** New workaround (DO NOTHING HAHAHA! Fingers crossed)
            return allNodes.InnerHtml;

            // *** Original code below

            /*
            // Anything we missed will get stripped out
            return
                Microsoft.<span class="goog_qs-tidbit goog_qs-tidbit-0">Security.Application.Sanitizer.GetSafeHtmlFragment(allNodes.InnerHtml);
             */
        }

        /// <summary>
        /// Takes a raw source and removes all HTML tags
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string StripHtml(string source)
        {
            source = SanitizeHtml(source);

            // No need to continue if we have no clean Html
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            HtmlDocument html = GetHtml(source);
            StringBuilder result = new StringBuilder();

            // For each node, extract only the innerText
            foreach (HtmlNode node in html.DocumentNode.ChildNodes)
                result.Append(node.InnerText);

            return result.ToString();
        }

        /// <summary>
        /// Recursively delete nodes not in the whitelist
        /// </summary>
        private static void CleanNodes(HtmlNode node, string[] whitelist)
        {
            if (node.NodeType == HtmlNodeType.Element)
            {
                if (!whitelist.Contains(node.Name))
                {
                    node.ParentNode.RemoveChild(node);
                    return; // We're done
                }
            }

            if (node.HasChildNodes)
                CleanChildren(node, whitelist);
        }

        /// <summary>
        /// Apply CleanNodes to each of the child nodes
        /// </summary>
        private static void CleanChildren(HtmlNode parent, string[] whitelist)
        {
            for (int i = parent.ChildNodes.Count - 1; i >= 0; i--)
                CleanNodes(parent.ChildNodes[i], whitelist);
        }

        /// <summary>
        /// Helper function that returns an HTML document from text
        /// </summary>
        private static HtmlDocument GetHtml(string source)
        {
            HtmlDocument html = new HtmlDocument();
            html.OptionFixNestedTags = true;
            html.OptionAutoCloseOnEnd = true;
            html.OptionDefaultStreamEncoding = Encoding.UTF8;

            html.LoadHtml(source ?? "");

            // Encode any code blocks independently so they won't
            // be stripped out completely when we do a final cleanup
            foreach (var n in html.DocumentNode.DescendantsAndSelf())
            {
                if (n.Name == "code")
                {
                    //** Code tag attribute vulnerability fix 28-9-12 (thanks to Natd)
                    HtmlAttribute[] attr = n.Attributes.ToArray();
                    foreach (HtmlAttribute a in attr)
                    {
                        if (a.Name != "style" && a.Name != "class") { a.Remove(); }
                    } //** End fix
                    n.InnerHtml = System.Net.WebUtility.HtmlEncode(System.Net.WebUtility.HtmlDecode(n.InnerHtml));
                }
            }

            return html;
        }
    }
}
