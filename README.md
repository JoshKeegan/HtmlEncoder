# HtmlEncoder
A .NET Library for encoding & decoding HTML, as available through System.Net.WebUtility, but with the added feature of correcting given indices in the input string to be correct for the output string.

This is particularly useful for correcting the indices of entities in Social Media statuses when received in HTML.  
e.g. You store a tweet in your database in plaintext, but the Twitter API sends the content of the Tweet in HTML.  
You convert the text of the Tweet to plaintext, but now the indices for Entities (Mentions, Hashtags and URLs) could be wrong.  
By using HtmlEncoder to decode the HTML, you will also get corrected indices for Entities.
  
## Usage
TODO: NuGet package.  
Build the latest source code in Release mode & reference HtmlEncoder.dll from your project.  
Add using:
```
using HtmlEncoder;
```
Call HtmlEnc.HtmlDecode from within your code.  
Note that the array of indices are passed by reference, so the values in the array will be changed. If you wish to retain the original indices, please deep copy the value before passing it.

## Example Code
Example of HtmlEnc.HtmlDecode being used, for more examples see UnitTests/HtmlEncTests.cs
```
// Input HTML & indices for a substring
int[] indices = new int[] { 17, 24 };
string html = "Hello World &amp; some more text here :)";

// Decode HTML to plaintext, correcting indices
string plaintext = HtmlEnc.HtmlDecode(html, ref indices);

// Values:
// plaintext: Hello World & some more text here :)
// indices: { 13, 20 }
```

## License
HtmlEncoder is licensed under the MIT License, which is included with HtmlEncoder in the LICENSE file.
