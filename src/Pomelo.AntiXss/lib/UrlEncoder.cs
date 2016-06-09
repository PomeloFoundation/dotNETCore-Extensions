// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UrlEncoder.cs" company="Microsoft Corporation">
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
//   Provides URL fragment encoding, mainly for .NET 4.0 encoder replacements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Provides URL fragment encoding, mainly for .NET 4.0 encoder replacements.
    /// </summary>
    internal static class UrlEncoder
    {
        /// <summary>
        /// A lock object to use when performing safe listing for parameter encoding.
        /// </summary>
        private static readonly ReaderWriterLockSlim SyncLock = new ReaderWriterLockSlim();

        /// <summary>
        /// The values to output for each character.
        /// </summary>
        private static char[][] characterValues;

        /// <summary>
        /// Encodes according to the URL Path encoding rules, RFC 1738.
        /// </summary>
        /// <param name="s">The URL path to encode.</param>
        /// <returns>The URL path</returns>
        internal static string PathEncode(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            if (characterValues == null)
            {
                InitializeSafeList();
            }

            // Check for query strings - we shouldn't get them, but just in case
            string[] parts = s.Split("?".ToCharArray());
            string actualPath = parts[0];

            string originalQueryString = string.Empty;
            if (parts.Length == 2)
            {
                originalQueryString = "?" + parts[1];
            }

            byte[] inputAsArray = Encoding.UTF8.GetBytes(actualPath.ToCharArray()); 
            int outputLength = 0;
            int inputLength = inputAsArray.Length;
            char[] encodedInput = new char[inputLength * 3]; // Worse case scenario - URL encoding uses %XX for encoded characters.
            
            SyncLock.EnterReadLock();
            try
            {
                for (int characterPosition = 0; characterPosition < inputLength; characterPosition++)
                {
                    byte currentCharacter = inputAsArray[characterPosition];

                    // Check if we an already encoded hex value, so we don't double encode.
                    if (currentCharacter == '%' && AreNextTwoCharactersValidHexString(inputAsArray, characterPosition + 1))
                    {
                        encodedInput[outputLength++] = (char)inputAsArray[characterPosition++]; // %
                        encodedInput[outputLength++] = (char)inputAsArray[characterPosition++]; // First part of the hex doublet.
                        encodedInput[outputLength++] = (char)inputAsArray[characterPosition]; // Second part of the hex doublet.
                    }
                    else
                    {
                        if (characterValues[currentCharacter] != null)
                        {
                            // character needs to be encoded
                            char[] encodedCharacter = characterValues[currentCharacter];
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
                }
            }
            finally
            {
                SyncLock.ExitReadLock();
            }

            return (new string(encodedInput, 0, outputLength)) + originalQueryString;
        }

        /// <summary>
        /// Returns a value indicating whether the two characters in <paramref name="input"/>, starting at position <paramref name="position"/>
        /// are valid hexadecimal characters.
        /// </summary>
        /// <param name="input">The input to check.</param>
        /// <param name="position">The position within the input to check from.</param>
        /// <returns>true if the characters are valid hexadecimal values, otherwise false.</returns>
        private static bool AreNextTwoCharactersValidHexString(byte[] input, int position)
        {
            if (position < input.Length && position + 1 < input.Length)
            {
                return IsValidHexCharacter((char)input[position]) && IsValidHexCharacter((char)input[position + 1]);
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating if the character <paramref name="c"/> is a valid hexadecimal character.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <returns>true if the character is a valid hexadecimal character, otherwise false.</returns>
        private static bool IsValidHexCharacter(char c)
        {
            bool isDigit = c >= '0' && c <= '9';
            bool isUpperHexLetter = c >= 'A' && c <= 'F';
            bool isLowerHexLetter = c >= 'a' && c <= 'f';

            return isDigit || isLowerHexLetter || isUpperHexLetter;
        }

        /// <summary>
        /// Initializes the HTML safe list.
        /// </summary>
        private static void InitializeSafeList()
        {
            SyncLock.EnterWriteLock();
            try
            {
                if (characterValues == null)
                {
                    characterValues = SafeList.Generate(0xFF, SafeList.PercentThenHexValueGenerator);
                    SafeList.PunchSafeList(ref characterValues, UrlSafeList());
                }
            }
            finally
            {
                SyncLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Provides the safe characters for URL encoding.
        /// </summary>
        /// <returns>The safe characters for URL encoding.</returns>
        private static IEnumerable<int> UrlSafeList()
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

            // Unreserved characters, RFC 3986 section 2.3
            yield return 0x2d; // -
            yield return 0x5f; // _
            yield return 0x2e; // .
            yield return 0x7e; // ~

            // Always safe
            yield return 0x21; // !
            yield return 0x2a; // *
            yield return 0x5c; // \
            yield return 0x28; // (
            yield return 0x29; // )
            yield return 0x2c; // ,

            // And obviously we don't want to encode path separators.
            yield return 0x2f; // /
        }
    }
}
