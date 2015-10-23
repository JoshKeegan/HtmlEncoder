/*
 * HtmlEncoder Tests
 * StringIndicesConverter Tests
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
using HtmlEncoder;
using NUnit.Framework;

namespace UnitTests
{
    public class StringIndicesConverterTests
    {
        [Test]
        public void TestConvertUtf32IndicesToUtf16EmptyString()
        {
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16EmptyIndicesWithString()
        {
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asdf", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16AllUtf16Chars()
        {
            int[] EXPECTED_INDICES = new int[] { 1, 7, 2, 30 };
            int[] arr = new int[] { 1, 7, 2, 30 };
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asdfjyuhqbgwnetfyw34tbKSDJFH78fnsyf9unhgj", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16IndicesBeforeUtf32Char()
        {
            int[] EXPECTED_INDICES = new int[] { 0, 2};
            int[] arr = new int[] { 0, 2 };
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asd🚒defsa", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16IndicesAfterUtf32Char()
        {
            int[] EXPECTED_INDICES = new int[] { 5, 9 };
            int[] arr = new int[] { 4, 8 };
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asd🚒defsa", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16IndexAtUtf32Char()
        {
            int[] EXPECTED_INDICES = new int[] { 3 };
            int[] arr = new int[] { 3 };
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asd🚒defsa", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16IndicesMixed()
        {
            int[] EXPECTED_INDICES = new int[] { 0, 2, 3, 5, 9 };
            int[] arr = new int[] { 0, 2, 3, 4, 8 };
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asd🚒defsa", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestConvertUtf32IndicesToUtf16IndicesMixedMultipleSurrogates()
        {
            int[] EXPECTED_INDICES = new int[] { 0, 2, 3, 5, 9, 10, 12, 13 };
            int[] arr = new int[] { 0, 2, 3, 4, 8, 9, 10, 11 };
            StringIndicesConverter.ConvertUtf32IndicesToUtf16("asd🚒defsa🚒sd", ref arr);

            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }
    }
}
