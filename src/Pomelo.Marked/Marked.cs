using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pomelo.Marked
{
    public static class Instance
    {
        private static _Marked marked = new _Marked();
        public static string Parse(string MarkdownContent)
        {
            return marked.Parse(MarkdownContent);
        }
    }

    internal class _Marked
    {
        public Options Options { get; set; }
        
        public _Marked()
            : this(null)
        {
        }

        public _Marked(Options options)
        {
            Options = options ?? new Options();
        }


        public virtual string Parse(string src)
        {
            if (String.IsNullOrEmpty(src))
            {
                return src;
            }

            var tokens = Lexer.Lex(src, Options);
            var result = Parser.Parse(tokens, Options);
            return result;
        }
    }
}
