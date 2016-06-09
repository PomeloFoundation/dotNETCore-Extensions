using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.AntiXSS
{
    public class DefaultWhiteListProvider : IWhiteListProvider
    {
        public IDictionary<string, string[]> WhiteList { get; set; } = new Dictionary<string, string[]>
        {
            {"p", new string[]          {"style", "class", "align"}},
            {"div", new string[]        {"style", "class", "align"}},
            {"span", new string[]       {"style", "class"}},
            {"br", new string[]         {"style", "class"}},
            {"hr", new string[]         {"style", "class"}},
            {"label", new string[]      {"style", "class"}},

            {"small", new string[]      {"style", "class"}},
            {"big", new string[]      {"style", "class"}},
            {"center", new string[]      {"style", "class"}},

            {"h1", new string[]         {"style", "class"}},
            {"h2", new string[]         {"style", "class"}},
            {"h3", new string[]         {"style", "class"}},
            {"h4", new string[]         {"style", "class"}},
            {"h5", new string[]         {"style", "class"}},
            {"h6", new string[]         {"style", "class"}},

            {"font", new string[]       {"style", "class",
                "color", "face", "size"}},
            {"strong", new string[]     {"style", "class"}},
            {"b", new string[]          {"style", "class"}},
            {"em", new string[]         {"style", "class"}},
            {"i", new string[]          {"style", "class"}},
            {"u", new string[]          {"style", "class"}},
            {"strike", new string[]     {"style", "class"}},
            {"ol", new string[]         {"style", "class"}},
            {"ul", new string[]         {"style", "class"}},
            {"li", new string[]         {"style", "class"}},
            {"blockquote", new string[] {"style", "class"}},
            {"code", new string[]       {"style", "class"}},
            {"pre", new string[]       {"style", "class"}},
            {"sub",new string[]{"style","class"}},
            {"sup",new string[]{"style","class"}},

            {"a", new string[]          {"style", "class", "href", "title","name","target"}},
            {"img", new string[]        {"style", "class", "src", "height",
                "width", "alt", "title", "hspace", "vspace", "border"}},

            {"table", new string[]      {"style", "class","align","border","cellpadding","cellspacing","summary"}},
            {"caption",new string[]{"style","class"}},
            {"thead", new string[]      {"style", "class"}},
            {"tbody", new string[]      {"style", "class"}},
            {"tfoot", new string[]      {"style", "class"}},
            {"th", new string[]         {"style", "class", "scope"}},
            {"tr", new string[]         {"style", "class"}},
            {"td", new string[]         {"style", "class", "colspan"}},

            {"q", new string[]          {"style", "class", "cite"}},
            {"cite", new string[]       {"style", "class"}},
            {"abbr", new string[]       {"style", "class"}},
            {"acronym", new string[]    {"style", "class"}},
            {"del", new string[]        {"style", "class"}},
            {"ins", new string[]        {"style", "class"}},
            {"address",new string[]{"style","class"}}
        };
    }
}
