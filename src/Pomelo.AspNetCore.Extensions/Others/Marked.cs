namespace Pomelo.AspNetCore.Extensions.Others
{
    public class Marked
    {
        public static string Parse(string md)
        {
            return Pomelo.Marked.Instance.Parse(md);
        }
    }
}

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    using Microsoft.AspNetCore.Html;

    public static class MarkedExtensions
    {
        public static HtmlString Marked(this IHtmlHelper self, string Content)
        {
            return new HtmlString(Pomelo.Marked.Instance.Parse(Content));
        }
    }
}

