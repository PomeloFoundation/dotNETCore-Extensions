using System;
using System.IO;
using System.Text.Encodings.Web;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    internal class CodeCombHtmlEncoder : HtmlEncoder
    {
        public override int MaxOutputCharactersPerInputCharacter
        {
            get { return 1; }
        }

        public override string Encode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length == 0)
            {
                return string.Empty;
            }

            return $"HtmlEncode[[{value}]]";
        }

        public override void Encode(TextWriter output, char[] value, int startIndex, int characterCount)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (characterCount == 0)
            {
                return;
            }
            
            output.Write(value, startIndex, characterCount);
        }

        public override void Encode(TextWriter output, string value, int startIndex, int characterCount)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (characterCount == 0)
            {
                return;
            }
            
            output.Write(value.Substring(startIndex, characterCount));
        }

        public override bool WillEncode(int unicodeScalar)
        {
            return false;
        }

        public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
        {
            return -1;
        }

        public override unsafe bool TryEncodeUnicodeScalar(
            int unicodeScalar,
            char* buffer,
            int bufferLength,
            out int numberOfCharactersWritten)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            numberOfCharactersWritten = 0;
            return false;
        }
    }
    public static class HtmlContentExtensions
    {
        public static string ToHtmlString(this TagBuilder self)
        {
            using (var writer = new StringWriter())
            {
                self.WriteTo(writer, new CodeCombHtmlEncoder());
                return writer.ToString();
            }
        }
    }
}
