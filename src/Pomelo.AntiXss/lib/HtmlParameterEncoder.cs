// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtmlParameterEncoder.cs" company="Microsoft Corporation">
//   Copyright (c) 2010 All Rights Reserved, Microsoft Corporation
//
//   This source is subject to the Microsoft Permissive License.
//   Please see the License.txt file for more information.
//   All other rights reserved.
//
//   THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
//   KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//   IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//   PARTICULAR PURPOSE.
//
// </copyright>
// <summary>
//   Provides HTML Parameter Encoding methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;
    using System.Collections;
    using System.Text;

    /// <summary>
    /// The type of space encoding to use.
    /// </summary>
    internal enum EncodingType
    {
        /// <summary>
        /// Encode spaces for use in query strings
        /// </summary>
        QueryString = 1,

        /// <summary>
        /// Encode spaces for use in form data
        /// </summary>
        HtmlForm = 2,
    }

    /// <summary>
    /// Provides Html Parameter Encoding methods.
    /// </summary>
    internal static class HtmlParameterEncoder
    {
        /// <summary>
        /// The value to use when encoding a space for query strings.
        /// </summary>
        private static readonly char[] QueryStringSpace = "%20".ToCharArray();

        /// <summary>
        /// The value to use when encoding a space for form data.
        /// </summary>
        private static readonly char[] FormStringSpace = "+".ToCharArray();

        /// <summary>
        /// The values to output for each character during parameter encoding.
        /// </summary>
        private static Lazy<char[][]> characterValuesLazy = new Lazy<char[][]>(InitialiseSafeList);

        /// <summary>
        /// The path character safe list.
        /// </summary>
        private static Lazy<char[][]> pathCharacterValuesLazy = new Lazy<char[][]>(InitialisePathSafeList);

        /// <summary>
        /// Encodes a string for query string encoding and returns the encoded string.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <returns>The URL-encoded text.</returns>
        /// <remarks>URL encoding ensures that all browsers will correctly transmit text in URL strings. 
        /// Characters such as a question mark (?), ampersand (&amp;), slash mark (/), and spaces might be truncated or corrupted by some browsers. 
        /// As a result, these characters must be encoded in &lt;a&gt; tags or in query strings where the strings can be re-sent by a browser 
        /// in a request string.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if the encoding is null.</exception>
        internal static string QueryStringParameterEncode(string s, Encoding encoding)
        {
            return FormQueryEncode(s, encoding, EncodingType.QueryString);
        }

        /// <summary>
        /// Encodes a string for form URL encoding and returns the encoded string.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <returns>The URL-encoded text.</returns>
        /// <remarks>URL encoding ensures that all browsers will correctly transmit text in URL strings. 
        /// Characters such as a question mark (?), ampersand (&amp;), slash mark (/), and spaces might be truncated or corrupted by some browsers. 
        /// As a result, these characters must be encoded in &lt;a&gt; tags or in query strings where the strings can be re-sent by a browser 
        /// in a request string.</remarks>
        /// <exception cref="ArgumentNullException">Thrown if the encoding is null.</exception>
        internal static string FormStringParameterEncode(string s, Encoding encoding)
        {
            return FormQueryEncode(s, encoding, EncodingType.HtmlForm);
        }

        /// <summary>
        /// Encodes a string as a URL
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <param name="encoding">The encoding context to use.</param>
        /// <returns>The encoded string.</returns>
        internal static string UrlPathEncode(string s, Encoding encoding)
        {
            return FormQueryEncode(s, encoding, EncodingType.QueryString, pathCharacterValuesLazy);
        }

        /// <summary>
        /// Encodes a string for Query String or Form Data encoding.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <param name="encodingType">The encoding type to use.</param>
        /// <returns>The encoded text.</returns>
        private static string FormQueryEncode(string s, Encoding encoding, EncodingType encodingType)
        {
            return FormQueryEncode(s, encoding, encodingType, characterValuesLazy);
        }

        /// <summary>
        /// Encodes a string for Query String or Form Data encoding.
        /// </summary>
        /// <param name="s">The text to URL-encode.</param>
        /// <param name="encoding">The encoding for the text parameter.</param>
        /// <param name="encodingType">The encoding type to use.</param>
        /// <param name="characterValues">A lazy loaded safelist to use.</param>
        /// <returns>The encoded text.</returns>
        private static string FormQueryEncode(string s, Encoding encoding, EncodingType encodingType, Lazy<char[][]> characterValues)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            // RFC 3986 states strings must be converted to their UTF8 value before URL encoding.
            // See http://tools.ietf.org/html/rfc3986
            // Conversion to char[] keeps null characters inline.
            byte[] utf8Bytes = encoding.GetBytes(s.ToCharArray());
            char[] encodedInput = new char[utf8Bytes.Length * 3]; // Each byte can potentially be encoded as %xx
            int outputLength = 0;

            char[][] safeList = characterValues.Value;

            for (int characterPosition = 0; characterPosition < utf8Bytes.Length; characterPosition++)
            {
                byte currentCharacter = utf8Bytes[characterPosition];

                if (currentCharacter == 0x00 || currentCharacter == 0x20 ||
                    currentCharacter > safeList.Length || safeList[currentCharacter] != null)
                {
                    // character needs to be encoded
                    char[] encodedCharacter;

                    if (currentCharacter == 0x20)
                    {
                        switch (encodingType)
                        {
                            case EncodingType.QueryString:
                                encodedCharacter = QueryStringSpace;
                                break;

                            // Special case for Html Form data, from http://www.w3.org/TR/html401/appendix/notes.html#non-ascii-chars
                            case EncodingType.HtmlForm:
                                encodedCharacter = FormStringSpace;
                                break;

                            default:
                                throw new ArgumentOutOfRangeException("encodingType");
                        }
                    }
                    else
                    {
                        encodedCharacter = safeList[currentCharacter];
                    }

                    for (int j = 0; j < encodedCharacter.Length; j++)
                    {
                        encodedInput[outputLength++] = encodedCharacter[j];
                    }
                }
                else
                {
                    // character does not need encoding
                    encodedInput[outputLength++] = (char)currentCharacter;
                }
            }

            return new string(encodedInput, 0, outputLength);
        }

        /// <summary>
        /// Initializes the HTML safe list.
        /// </summary>
        /// <returns>Creates the safelist</returns>
        private static char[][] InitialiseSafeList()
        {
            char[][] result = SafeList.Generate(255, SafeList.PercentThenHexValueGenerator);
            SafeList.PunchSafeList(ref result, UrlParameterSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for URL parameter encoding.
        /// </summary>
        /// <returns>The safe characters for URL parameter encoding.</returns>
        private static IEnumerable UrlParameterSafeList()
        {
            // Hyphen
            yield return 0x2D;

            // Full stop/period
            yield return 0x2E;

            // Digits
            for (int i = 0x30; i <= 0x39; i++)
            {
                yield return i;
            }  
            
            // Upper case alphabet
            for (int i = 0x41; i <= 0x5A; i++)
            {
                yield return i;
            }

            // Underscore
            yield return 0x5F;

            // Lower case alphabet
            for (int i = 0x61; i <= 0x7A; i++)
            {
                yield return i;
            }
     
            // Tilde
            yield return 0x7E;
        }

        /// <summary>
        /// Initializes the Url Path safe list.
        /// </summary>
        /// <returns>A list of characters and their encoded values for URL encoding.</returns>
        private static char[][] InitialisePathSafeList()
        {
            char[][] result = SafeList.Generate(255, SafeList.PercentThenHexValueGenerator);
            SafeList.PunchSafeList(ref result, UrlPathSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for URL path encoding.
        /// </summary>
        /// <returns>The safe characters for URL path encoding.</returns>
        private static IEnumerable UrlPathSafeList()
        {
            foreach (var c in UrlParameterSafeList())
            {
                yield return c;
            }

            // Hash
            yield return 0x23;

            // Percent
            yield return 0x25;

            // Forward slash
            yield return 0x2F;

            // Backwards slash
            yield return 0x5C;

            // Left parenthesis
            yield return 0x28;

            // Right parenthesis
            yield return 0x29;
        }
    }
}
