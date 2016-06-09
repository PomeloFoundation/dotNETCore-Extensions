// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AntiXSS.cs" company="Microsoft Corporation">
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
//   Performs encoding of input strings to provide protection against
//   Cross-Site Scripting (XSS) attacks in various contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Microsoft.Security.Application
{
    using System;

    /// <summary>
    /// Performs encoding of input strings to provide protection against
    /// Cross-Site Scripting (XSS) attacks in various contexts.
    /// </summary>
    /// <remarks>
    /// The Anti-Cross Site Scripting Library uses the Principle 
    /// of Inclusions, sometimes referred to as "safe listing" to 
    /// provide protection against Cross-Site Scripting attacks.  With
    /// safe listing protection, algorithms look for valid inputs and 
    /// automatically treat everything outside that set as a 
    /// potential attack.  This library can be used as a defense in
    /// depth approach with other mitigation techniques. It is suitable
    /// for applications with high security requirements.
    /// </remarks>
    [Obsolete("This class has been deprecated. Please use Microsoft.Security.Application.Encoder instead.")]
    public static class AntiXss
    {
        /// <summary>
        /// Encodes input strings for use in HTML.
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <returns>
        /// Encoded string for use in HTML.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using  &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and their related encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert&#40;&#39;XSS Attack&#33;&#39;&#41;&#59;</description></item>
        /// <item><term>user@contoso.com</term><description>user&#64;contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.HtmlEncode() instead.")]
        public static string HtmlEncode(string input)
        {
            return Encoder.HtmlEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in HTML attributes.
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <returns>
        /// Encoded string for use in HTML attributes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using  &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert&#40;&#39;XSS&#32;Attack&#33;&#39;&#41;&#59;</description></item>
        /// <item><term>user@contoso.com</term><description>user&#64;contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross&#32;Site&#32;Scripting&#32;Library</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.HtmlAttributeEncode() instead.")]
        public static string HtmlAttributeEncode(string input)
        {
            return Encoder.HtmlAttributeEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX 
        /// and %uDOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert%28%27XSS%20Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.UrlEncode() instead.")]
        public static string UrlEncode(string input)
        {
            return Encoder.UrlEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in universal resource locators (URLs).
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="codepage">Codepage number of the input</param>
        /// <returns>
        /// Encoded string for use in URLs.
        /// </returns>
        /// <remarks>
        /// This function encodes the output as per the encoding parameter (codepage) passed to it. It encodes 
        /// all but known safe characters.  Characters are encoded using %SINGLE_BYTE_HEX and %DOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSSあAttack!');</term><description>alert%28%27XSS%82%a0Attack%21%27%29%3b</description></item>
        /// <item><term>user@contoso.com</term><description>user%40contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross%20Site%20Scripting%20Library</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.UrlEncode() instead.")]
        public static string UrlEncode(string input, int codepage)
        {
            return Encoder.UrlEncode(input, codepage);
        }

        /// <summary>
        /// Encodes input strings for use in XML.
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <returns>
        ///     Encoded string for use in XML.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert&#40;&#39;XSS Attack&#33;&#39;&#41;&#59;</description></item>
        /// <item><term>user@contoso.com</term><description>user&#64;contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross Site Scripting Library</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.XmlEncode() instead.")]
        public static string XmlEncode(string input)
        {
            return Encoder.XmlEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in XML attributes.
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <returns>
        /// Encoded string for use in XML attributes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using &amp;#DECIMAL; notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>alert&#40;&#39;XSS&#32;Attack&#33;&#39;&#41;&#59;</description></item>
        /// <item><term>user@contoso.com</term><description>user&#64;contoso.com</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>Anti-Cross&#32;Site&#32;Scripting&#32;Library</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.XmlAttributeEncode() instead.")]
        public static string XmlAttributeEncode(string input)
        {
            return Encoder.XmlAttributeEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in JavaScript.
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <returns>
        /// Encoded string for use in JavaScript.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using \xSINGLE_BYTE_HEX and \uDOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>'alert\x28\x27XSS Attack\x21\x27\x29\x3b'</description></item>
        /// <item><term>user@contoso.com</term><description>'user\x40contoso.com'</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>'Anti-Cross Site Scripting Library'</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.JavaScriptEncode() instead.")]
        public static string JavaScriptEncode(string input)
        {
            return Encoder.JavaScriptEncode(input);
        }

        /// <summary>
        /// Encodes input strings for use in JavaScript.
        /// </summary>
        /// <param name="input">String to be encoded</param>
        /// <param name="flagforQuote">bool flag to determine whether or not to emit quotes. true = emit quote. false = no quote.</param>
        /// <returns>
        /// Encoded string for use in JavaScript and does not return the output with en quotes.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are encoded using \xSINGLE_BYTE_HEX and \uDOUBLE_BYTE_HEX notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item><term>a-z</term><description>Lower case alphabet</description></item>
        /// <item><term>A-Z</term><description>Upper case alphabet</description></item>
        /// <item><term>0-9</term><description>Numbers</description></item>
        /// <item><term>,</term><description>Comma</description></item>
        /// <item><term>.</term><description>Period</description></item>
        /// <item><term>-</term><description>Dash</description></item>
        /// <item><term>_</term><description>Underscore</description></item>
        /// <item><term> </term><description>Space</description></item>
        /// <item><term> </term><description>Other International character ranges</description></item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item><term>alert('XSS Attack!');</term><description>'alert\x28\x27XSS Attack\x21\x27\x29\x3b'</description></item>
        /// <item><term>user@contoso.com</term><description>'user\x40contoso.com'</description></item>
        /// <item><term>Anti-Cross Site Scripting Library</term><description>'Anti-Cross Site Scripting Library'</description></item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.JavaScriptEncode() instead.")]
        public static string JavaScriptEncode(string input, bool flagforQuote)
        {
            return Encoder.JavaScriptEncode(input, flagforQuote);
        }

        /// <summary>
        /// Encodes input strings for use in Visual Basic Script.
        /// </summary>
        /// <param name="input">
        /// String to be encoded
        /// </param>
        /// <returns>
        /// Encoded string for use in Visual Basic Script.
        /// </returns>
        /// <remarks>
        /// This function encodes all but known safe characters.  Characters are 
        /// encoded using &amp;chrw(DECIMAL) notation.
        /// <newpara/>
        /// Safe characters include:
        /// <list type="table">
        /// <item>
        /// <term>a-z</term>
        /// <description>Lower case alphabet</description>
        /// </item>
        /// <item>
        /// <term>A-Z</term>
        /// <description>Upper case alphabet</description>
        /// </item>
        /// <item>
        /// <term>0-9</term>
        /// <description>Numbers</description>
        /// </item>
        /// <item>
        /// <term>,</term>
        /// <description>Comma</description>
        /// </item>
        /// <item>
        /// <term>.</term>
        /// <description>Period</description>
        /// </item>
        /// <item>
        /// <term>-</term>
        /// <description>Dash</description>
        /// </item>
        /// <item>
        /// <term>_</term>
        /// <description>Underscore</description>
        /// </item>
        /// <item>
        /// <term> </term>
        /// <description>Space</description>
        /// </item>
        /// </list>
        /// <newpara/>
        /// Example inputs and encoded outputs:
        /// <list type="table">
        /// <item>
        /// <term>alert('XSS Attack!');</term>
        /// <description>"alert"&amp;chrw(40)&amp;chrw(39)&amp;"XSS Attack"&amp;chrw(33)&amp;chrw(39)&amp;chrw(41)&amp;chrw(59)</description>
        /// </item>
        /// <item>
        /// <term>user@contoso.com</term>
        /// <description>"user"&amp;chrw(64)&amp;"contoso.com"</description>
        /// </item>
        /// <item>
        /// <term>Anti-Cross Site Scripting Library</term>
        /// <description>"Anti-Cross Site Scripting Library"</description>
        /// </item>
        /// </list>
        /// </remarks>
        [Obsolete("This method has been deprecated. Please use Encoder.VisualBasicScriptEncode() instead.")]
        public static string VisualBasicScriptEncode(string input)
        {
            return Encoder.VisualBasicScriptEncode(input);
        }
    }
}