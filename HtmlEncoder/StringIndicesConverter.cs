/*
 * HtmlEncoder
 * StringIndicesConverter static class -
 *  Converts UTF32 indices to UTF16 indices (as used by c# strings and that of many 
 *  other modern programming languages). Then they can be accessed directly in 
 *  the string via str[n] or str.Substring(n) etc...
 * Authors:
 *  Josh Keegan 23/10/2015
 * 
 * Licensed under the MIT license. See LICENSE file in the project root for full license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlEncoder
{
    public static class StringIndicesConverter
    {
        public static void ConvertUtf32IndicesToUtf16(string s, ref int[] indices)
        {
            // Validation
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            if (indices == null)
            {
                throw new ArgumentNullException("indices");
            }

            // Optimisation: Do nothing if no indices
            if (indices.Length == 0)
            {
                return;
            }

            // Iterate over all chars, checking if they are part of a surrgate pair. If so correct indices after that point
            for (int i = 0; i < s.Length - 1; i++) // Can skip final char, as it will either have been counted already as it's a trailing surrogate or it doesn't matter as it won't effect the indices
            {
                // If this is the first char in a surrogate pair
                if (s[i] >= HtmlEnc.HIGH_SURROGATE_START && s[i] <= HtmlEnc.HIGH_SURROGATE_END
                    && s[i + 1] >= HtmlEnc.LOW_SURROGATE_START && s[i + 1] <= HtmlEnc.LOW_SURROGATE_END)
                {
                    // Bump up any indices after this position
                    for (int j = 0; j < indices.Length; j++)
                    {
                        if (indices[j] > i)
                        {
                            indices[j]++;
                        }
                    }

                    // Skip the next char, as it is the second part of this surrogate pair
                    i++;
                }
            }
        }
    }
}
