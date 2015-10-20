/*
 * HtmlEncoder
 *  Encode/Decode HTML, as available through System.Net.WebUtility, but with the added feature of correcting given indices
 *  in the input string to be correct for the output string
 * Authors:
 *  Josh Keegan 20/10/2015
 * 
 * Licensed under the MIT license. See LICENSE file in the project root for full license information.
 * 
 * Based on Microsoft Open Source System.Net.WebUtility.cs code from https://github.com/dotnet/corefx/blob/master/src/System.Runtime.Extensions/src/System/Net/WebUtility.cs
 */

// Don't entity encode high chars (160 to 256)
#define ENTITY_ENCODE_HIGH_ASCII_CHARS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEncoder
{
    public static class HtmlEncoder
    {
        // some consts copied from Char / CharUnicodeInfo since we don't have friend access to those types
        private const char HIGH_SURROGATE_START = '\uD800';
        private const char LOW_SURROGATE_START = '\uDC00';
        private const char LOW_SURROGATE_END = '\uDFFF';
        private const int UNICODE_PLANE00_END = 0x00FFFF;
        private const int UNICODE_PLANE01_START = 0x10000;
        private const int UNICODE_PLANE16_END = 0x10FFFF;

        private const int UnicodeReplacementChar = '\uFFFD';

        private static readonly UnicodeEncodingConformance s_htmlEncodeConformance;

        public static string HtmlEncode(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            // Don't create string writer if we don't have nothing to encode
            int index = IndexOfHtmlEncodingChars(value, 0);
            if (index == -1)
            {
                return value;
            }

            StringWriter writer = new StringWriter();
            HtmlEncode(value, writer);
            return writer.ToString();
        }

        private static unsafe void HtmlEncode(string value, TextWriter output)
        {
            if (value == null)
            {
                return;
            }
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            int index = IndexOfHtmlEncodingChars(value, 0);
            if (index == -1)
            {
                output.Write(value);
                return;
            }

            Debug.Assert(0 <= index && index <= value.Length, "0 <= index && index <= value.Length");

            int cch = value.Length - index;
            fixed (char* str = value)
            {
                char* pch = str;
                while (index-- > 0)
                {
                    output.Write(*pch++);
                }

                for (; cch > 0; cch--, pch++)
                {
                    char ch = *pch;
                    if (ch <= '>')
                    {
                        switch (ch)
                        {
                            case '<':
                                output.Write("&lt;");
                                break;
                            case '>':
                                output.Write("&gt;");
                                break;
                            case '"':
                                output.Write("&quot;");
                                break;
                            case '\'':
                                output.Write("&#39;");
                                break;
                            case '&':
                                output.Write("&amp;");
                                break;
                            default:
                                output.Write(ch);
                                break;
                        }
                    }
                    else
                    {
                        int valueToEncode = -1; // set to >= 0 if needs to be encoded

#if ENTITY_ENCODE_HIGH_ASCII_CHARS
                        if (ch >= 160 && ch < 256)
                        {
                            // The seemingly arbitrary 160 comes from RFC
                            valueToEncode = ch;
                        }
                        else
#endif // ENTITY_ENCODE_HIGH_ASCII_CHARS
                        if (s_htmlEncodeConformance == UnicodeEncodingConformance.Strict && Char.IsSurrogate(ch))
                        {
                            int scalarValue = GetNextUnicodeScalarValueFromUtf16Surrogate(ref pch, ref cch);
                            if (scalarValue >= UNICODE_PLANE01_START)
                            {
                                valueToEncode = scalarValue;
                            }
                            else
                            {
                                // Don't encode BMP characters (like U+FFFD) since they wouldn't have
                                // been encoded if explicitly present in the string anyway.
                                ch = (char)scalarValue;
                            }
                        }

                        if (valueToEncode >= 0)
                        {
                            // value needs to be encoded
                            output.Write("&#");
                            output.Write(valueToEncode.ToString(CultureInfo.InvariantCulture));
                            output.Write(';');
                        }
                        else
                        {
                            // write out the character directly
                            output.Write(ch);
                        }
                    }
                }
            }
        }

        private static unsafe int IndexOfHtmlEncodingChars(string s, int startPos)
        {
            Debug.Assert(0 <= startPos && startPos <= s.Length, "0 <= startPos && startPos <= s.Length");

            int cch = s.Length - startPos;
            fixed (char* str = s)
            {
                for (char* pch = &str[startPos]; cch > 0; pch++, cch--)
                {
                    char ch = *pch;
                    if (ch <= '>')
                    {
                        switch (ch)
                        {
                            case '<':
                            case '>':
                            case '"':
                            case '\'':
                            case '&':
                                return s.Length - cch;
                        }
                    }
#if ENTITY_ENCODE_HIGH_ASCII_CHARS
                    else if (ch >= 160 && ch < 256)
                    {
                        return s.Length - cch;
                    }
#endif // ENTITY_ENCODE_HIGH_ASCII_CHARS
                    else if (s_htmlEncodeConformance == UnicodeEncodingConformance.Strict && Char.IsSurrogate(ch))
                    {
                        return s.Length - cch;
                    }
                }
            }

            return -1;
        }

        private static unsafe int GetNextUnicodeScalarValueFromUtf16Surrogate(ref char* pch, ref int charsRemaining)
        {
            // invariants
            Debug.Assert(charsRemaining >= 1);
            Debug.Assert(Char.IsSurrogate(*pch));

            if (charsRemaining <= 1)
            {
                // not enough characters remaining to resurrect the original scalar value
                return UnicodeReplacementChar;
            }

            char leadingSurrogate = pch[0];
            char trailingSurrogate = pch[1];

            if (Char.IsSurrogatePair(leadingSurrogate, trailingSurrogate))
            {
                // we're going to consume an extra char
                pch++;
                charsRemaining--;

                // below code is from Char.ConvertToUtf32, but without the checks (since we just performed them)
                return (((leadingSurrogate - HIGH_SURROGATE_START) * 0x400) + (trailingSurrogate - LOW_SURROGATE_START) + UNICODE_PLANE01_START);
            }
            else
            {
                // unmatched surrogate
                return UnicodeReplacementChar;
            }
        }
    }
}
