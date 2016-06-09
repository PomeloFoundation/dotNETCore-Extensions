// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CssEncoder.cs" company="Microsoft Corporation">
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
//   Provides CSS Encoding methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Security.Application
{
    using System;
    using System.Collections;
    using System.Text;

    /// <summary>
    /// Provides CSS Encoding methods.
    /// </summary>
    internal static class CssEncoder
    {
        /// <summary>
        /// The values to output for each character.
        /// </summary>
        private static Lazy<char[][]> characterValuesLazy = new Lazy<char[][]>(InitialiseSafeList);

        /// <summary>
        /// Encodes according to the CSS encoding rules.
        /// </summary>
        /// <param name="input">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        internal static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            char[][] characterValues = characterValuesLazy.Value;

            // Setup a new StringBuilder for output.
            // Worse case scenario - CSS encoding wants \XXXXXX for encoded characters.
            StringBuilder builder = EncoderUtil.GetOutputStringBuilder(input.Length, 7);

            Utf16StringReader stringReader = new Utf16StringReader(input);
            while (true) 
            {
                int currentCodePoint = stringReader.ReadNextScalarValue();
                if (currentCodePoint < 0) 
                {
                    break; // EOF
                }

                if (currentCodePoint >= characterValues.Length) 
                {
                    // We don't have a pre-generated mapping of characters beyond the U+00FF, so we need
                    // to generate these encodings on-the-fly. We should encode the code point rather
                    // than the surrogate code units that make up this code point.
                    // See: http://www.w3.org/International/questions/qa-escapes#cssescapes
                    char[] encodedCharacter = SafeList.SlashThenSixDigitHexValueGenerator(currentCodePoint);
                    builder.Append(encodedCharacter);
                }
                else if (characterValues[currentCodePoint] != null) 
                {
                    // character needs to be encoded
                    char[] encodedCharacter = characterValues[currentCodePoint];
                    builder.Append(encodedCharacter);
                }
                else 
                {
                    // character does not need encoding
                    builder.Append((char)currentCodePoint);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Initializes the CSS safe list.
        /// </summary>
        /// <returns>
        /// The CSS safe list.
        /// </returns>
        private static char[][] InitialiseSafeList()
        {
            char[][] result = SafeList.Generate(0xFF, SafeList.SlashThenSixDigitHexValueGenerator);
            SafeList.PunchSafeList(ref result, CssSafeList());
            return result;
        }

        /// <summary>
        /// Provides the safe characters for CS encoding.
        /// </summary>
        /// <returns>The safe characters for CSS encoding.</returns>
        /// <remarks>See http://www.owasp.org/index.php/XSS_(Cross_Site_Scripting)_Prevention_Cheat_Sheet </remarks>
        private static IEnumerable CssSafeList()
        {
            for (int i = '0'; i <= '9'; i++)
            {
                yield return i;
            }

            for (int i = 'A'; i <= 'Z'; i++)
            {
                yield return i;
            }

            for (int i = 'a'; i <= 'z'; i++)
            {
                yield return i;
            }
        }
    }
}
