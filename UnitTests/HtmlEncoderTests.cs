using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlEncoder;

using NUnit.Framework;

namespace UnitTests
{
    public class HtmlEncoderTests
    {
        [Test]
        public void TestHtmlDecodeEmptyString()
        {
            const string EXPECTED_TEXT = "";
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            string actual = HtmlEnc.HtmlDecode("", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeNullString()
        {
            const string EXPECTED_TEXT = null;
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            string actual = HtmlEnc.HtmlDecode(null, ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestHtmlDecodeNullIndices()
        {
            int[] arr = null;
            HtmlEnc.HtmlDecode("", ref arr);
        }

        [Test]
        public void TestHtmlDecodePlainText()
        {
            const string EXPECTED_TEXT = "Hello World!";
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            string actual = HtmlEnc.HtmlDecode("Hello World!", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlAmpersand()
        {
            // Basic, but probably most common case: the ampersand
            const string EXPECTED_TEXT = "Hello World & some more text here :)";
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            string actual = HtmlEnc.HtmlDecode("Hello World &amp; some more text here :)", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlUnicodeCharCode()
        {
            const string EXPECTED_TEXT = "å";
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            string actual = HtmlEnc.HtmlDecode("&#229;", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeUnicodeCharCodeWithSurrogate()
        {
            const string EXPECTED_TEXT = "🚒";
            int[] EXPECTED_INDICES = new int[0];
            int[] arr = new int[0];
            string actual = HtmlEnc.HtmlDecode("&#128658;", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodePlainTextWithIndices()
        {
            const string EXPECTED_TEXT = "Hello World!";
            int[] EXPECTED_INDICES = new int[] { 2, 7, 11 };
            int[] arr = new int[] { 2, 7, 11 };
            string actual = HtmlEnc.HtmlDecode("Hello World!", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlAmpersandWithIndicesBefore()
        {
            // Indices before the first HTML encoded char shouldn't be affected
            const string EXPECTED_TEXT = "Hello World & some more text here :)";
            int[] EXPECTED_INDICES = new int[] { 0, 7 };
            int[] arr = new int[] { 0, 7 };
            string actual = HtmlEnc.HtmlDecode("Hello World &amp; some more text here :)", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlAmpersandWithIndexAtAmpersand()
        {
            // The index of the start of the HTML encoded &amp; will be the new index of the plain text &
            const string EXPECTED_TEXT = "Hello World & some more text here :)";
            int[] EXPECTED_INDICES = new int[] { 12 };
            int[] arr = new int[] { 12 };
            string actual = HtmlEnc.HtmlDecode("Hello World &amp; some more text here :)", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlAmpersandWithIndexDuringHtmlEncodedChar()
        {
            // The index of somewhere in the middle of the HTML encoded &amp; will be the index of the 
            //  start of the &amp; as that is where the plain text & will go
            const string EXPECTED_TEXT = "Hello World & some more text here :)";
            int[] EXPECTED_INDICES = new int[] { 12, 12, 12, 12 };
            int[] arr = new int[] { 13, 14, 15, 16 };
            string actual = HtmlEnc.HtmlDecode("Hello World &amp; some more text here :)", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlAmpersandWithIndicesAfterHtmlEncodedChar()
        {
            // Indices after the first HTML encoded char will be affected
            const string EXPECTED_TEXT = "Hello World & some more text here :)";
            int[] EXPECTED_INDICES = new int[] { 13, 20 };
            int[] arr = new int[] { 17, 24 };
            string actual = HtmlEnc.HtmlDecode("Hello World &amp; some more text here :)", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlAmpersandWithIndicesMixed()
        {
            const string EXPECTED_TEXT = "Hello World & some more text here :)";
            int[] EXPECTED_INDICES = new int[] { 0, 13, 20, 4, 12, 12 };
            int[] arr = new int[] { 0, 17, 24, 4, 12, 14 };
            string actual = HtmlEnc.HtmlDecode("Hello World &amp; some more text here :)", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlUnicodeCharCodeWithIndicesAfter()
        {
            const string EXPECTED_TEXT = "å asd";
            int[] EXPECTED_INDICES = new int[] { 1, 2 };
            int[] arr = new int[] { 6, 7 };
            string actual = HtmlEnc.HtmlDecode("&#229; asd", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlUnicodeCharCodeWithSurrogateWithIndicesAfter()
        {
            const string EXPECTED_TEXT = "🚒 asd";
            int[] EXPECTED_INDICES = new int[] { 2, 3 };
            int[] arr = new int[] { 9, 10 };
            string actual = HtmlEnc.HtmlDecode("&#128658; asd", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlMultipleWithIndices()
        {
            const string EXPECTED_TEXT = "& asd & ght << 4";
            int[] EXPECTED_INDICES = new int[] { 5, 6, 6, 15 };
            int[] arr = new int[] { 9, 10, 12, 29 };
            string actual = HtmlEnc.HtmlDecode("&amp; asd &amp; ght &lt;&lt; 4", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlMultipleIncludingUnicodeWithIndices()
        {
            const string EXPECTED_TEXT = "& asd & ght å<< 4";
            int[] EXPECTED_INDICES = new int[] { 5, 6, 6, 16 };
            int[] arr = new int[] { 9, 10, 12, 35 };
            string actual = HtmlEnc.HtmlDecode("&amp; asd &amp; ght &#229;&lt;&lt; 4", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }

        [Test]
        public void TestHtmlDecodeHtmlMultipleIncludingUnicodeWithSurrogateWithIndices()
        {
            const string EXPECTED_TEXT = "& asd & ght å<<🚒 4";
            int[] EXPECTED_INDICES = new int[] { 5, 6, 6, 18 }; // final value is 18, despite being the 17th actual character because 🚒 occupies 2 bytes and therefore 2 c# chars
            int[] arr = new int[] { 9, 10, 12, 44 };
            string actual = HtmlEnc.HtmlDecode("&amp; asd &amp; ght &#229;&lt;&lt;&#128658; 4", ref arr);

            Assert.AreEqual(EXPECTED_TEXT, actual);
            CollectionAssert.AreEqual(EXPECTED_INDICES, arr);
        }
    }
}
