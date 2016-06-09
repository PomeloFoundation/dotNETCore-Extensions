// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UriUtil.cs" company="Microsoft Corporation">
//   Copyright (c) 2008, 2009, 2010 All Rights Reserved, Microsoft Corporation
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
//   Contains helpers for URI parsing 
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;

    /// <summary>
    /// Contains helpers for URI parsing 
    /// </summary>
    internal static class UriUtil
    {
        /// <summary>
        /// Query Fragment separators.
        /// </summary>
        private static readonly char[] QueryFragmentSeparators = new[] { '?', '#' };

        /// <summary>
        /// Extracts the query string and fragment from the input path by splitting on the separator characters.
        /// Doesn't perform any validation as to whether the input represents a valid URL.
        /// Concatenating the pieces back together will form the original input string.
        /// </summary>
        /// <param name="input">The URL to split.</param>
        /// <param name="path">The path portion of <paramref name="input"/>.</param>
        /// <param name="queryAndFragment">The query and fragment of <paramref name="input"/>.</param>
        internal static void ExtractQueryAndFragment(string input, out string path, out string queryAndFragment)
        {
            int queryFragmentSeparatorPos = input.IndexOfAny(QueryFragmentSeparators);
            if (queryFragmentSeparatorPos != -1)
            {
                path = input.Substring(0, queryFragmentSeparatorPos);
                queryAndFragment = input.Substring(queryFragmentSeparatorPos);
            }
            else
            {
                // no query or fragment separator
                path = input;
                queryAndFragment = null;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the schemes used in <paramref name="url"/> is generally considered safe for the purposes of redirects or other places where URLs are rendered to the page.
        /// </summary>
        /// <param name="url">The URL to parse</param>
        /// <returns>true if the scheme is considered safe, otherwise false.</returns>
        internal static bool IsSafeScheme(string url)
        {
            return url.IndexOf(":", StringComparison.Ordinal) == -1 ||
                    url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("https:", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("file:", StringComparison.OrdinalIgnoreCase) ||
                    url.StartsWith("news:", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Attempts to split a URI into its constituent pieces.
        /// Even if this method returns true, one or more of the out parameters might contain a null or empty string, e.g. if there is no query / fragment.
        /// Concatenating the pieces back together will form the original input string.
        /// </summary>
        /// <param name="input">The input URI to split.</param>
        /// <param name="schemeAndAuthority">The scheme and authority used in the <paramref name="input"/> uri.</param>
        /// <param name="path">The path contained in the <paramref name="input"/> uri.</param>
        /// <param name="queryAndFragment">The query and fragment contained in the <paramref name="input"/> uri.</param>
        /// <returns>true if the URI could be split, otherwise false.</returns>
        internal static bool TrySplitUriForPathEncode(string input, out string schemeAndAuthority, out string path, out string queryAndFragment)
        {
            // Strip off ?query and #fragment if they exist, since we're not going to look at them
            string inputWithoutQueryFragment;
            ExtractQueryAndFragment(input, out inputWithoutQueryFragment, out queryAndFragment);

            // Use Uri class to parse the url into authority and path, use that to help decide
            // where to split the string. Do not rebuild the url from the Uri instance, as that
            // might have subtle changes from the original string (for example, see below about "://").
            Uri uri;
            if (IsSafeScheme(inputWithoutQueryFragment) && Uri.TryCreate(inputWithoutQueryFragment, UriKind.Absolute, out uri))
            {
                string authority = uri.Authority; // e.g. "foo:81" in "http://foo:81/bar"
                if (!string.IsNullOrEmpty(authority))
                {
                    // don't make any assumptions about the scheme or the "://" part.
                    // For example, the "//" could be missing, or there could be "///" as in "file:///C:\foo.txt"
                    // To retain the same string as originally given, find the authority in the original url and include
                    // everything up to that.
                    int authorityIndex = inputWithoutQueryFragment.IndexOf(authority, StringComparison.Ordinal);
                    if (authorityIndex != -1)
                    {
                        int schemeAndAuthorityLength = authorityIndex + authority.Length;
                        schemeAndAuthority = inputWithoutQueryFragment.Substring(0, schemeAndAuthorityLength);
                        path = inputWithoutQueryFragment.Substring(schemeAndAuthorityLength);
                        return true;
                    }
                }
            }

            // Not a safe URL
            schemeAndAuthority = null;
            path = null;
            queryAndFragment = null;
            return false;
        }
    }
}
