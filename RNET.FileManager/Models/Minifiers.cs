using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace RNET.FileManager.Minifiers
{
    public static class JsMinifier
    {
        const int EOF = -1;

        static StringReader sr;
        static StringWriter sw;
        static StringBuilder sb;
        static int theA;
        static int theB;
        static int theLookahead = EOF;

        public static string GetMinifiedCode(string regularJsCode)
        {
            string result = null;
            using (sr = new StringReader(regularJsCode))
            {
                sb = new StringBuilder();
                using (sw = new StringWriter(sb))
                {
                    jsmin();
                }
            }
            sw.Flush();
            result = sb.ToString();
            return result;
        }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        private static void jsmin()
        {
            theA = '\n';
            action(3);
            while (theA != EOF)
            {
                switch (theA)
                {
                    case ' ':
                        {
                            if (isAlphanum(theB))
                            {
                                action(1);
                            }
                            else
                            {
                                action(2);
                            }
                            break;
                        }
                    case '\n':
                        {
                            switch (theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        action(1);
                                        break;
                                    }
                                case ' ':
                                    {
                                        action(3);
                                        break;
                                    }
                                default:
                                    {
                                        if (isAlphanum(theB))
                                        {
                                            action(1);
                                        }
                                        else
                                        {
                                            action(2);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (theB)
                            {
                                case ' ':
                                    {
                                        if (isAlphanum(theA))
                                        {
                                            action(1);
                                            break;
                                        }
                                        action(3);
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    action(1);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (isAlphanum(theA))
                                                    {
                                                        action(1);
                                                    }
                                                    else
                                                    {
                                                        action(3);
                                                    }
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        action(1);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        private static void action(int d)
        {
            if (d <= 1)
            {
                put(theA);
            }
            if (d <= 2)
            {
                theA = theB;
                if (theA == '\'' || theA == '"')
                {
                    for (; ; )
                    {
                        put(theA);
                        theA = get();
                        if (theA == theB)
                        {
                            break;
                        }
                        if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", theA));
                        }
                        if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                    }
                }
            }
            if (d <= 3)
            {
                theB = next();
                if (theB == '/' && (theA == '(' || theA == ',' || theA == '=' ||
                                    theA == '[' || theA == '!' || theA == ':' ||
                                    theA == '&' || theA == '|' || theA == '?' ||
                                    theA == '{' || theA == '}' || theA == ';' ||
                                    theA == '\n'))
                {
                    put(theA);
                    put(theB);
                    for (; ; )
                    {
                        theA = get();
                        if (theA == '/')
                        {
                            break;
                        }
                        else if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                        else if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", theA));
                        }
                        put(theA);
                    }
                    theB = next();
                }
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */
        private static int next()
        {
            int c = get();
            if (c == '/')
            {
                switch (peek())
                {
                    case '/':
                        {
                            for (; ; )
                            {
                                c = get();
                                if (c <= '\n')
                                {
                                    return c;
                                }
                            }
                        }
                    case '*':
                        {
                            get();
                            for (; ; )
                            {
                                switch (get())
                                {
                                    case '*':
                                        {
                                            if (peek() == '/')
                                            {
                                                get();
                                                return ' ';
                                            }
                                            break;
                                        }
                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }
            return c;
        }
        /* peek -- get the next character without getting it.
        */
        private static int peek()
        {
            theLookahead = get();
            return theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        private static int get()
        {
            int c = theLookahead;
            theLookahead = EOF;
            if (c == EOF)
            {
                c = sr.Read();
            }
            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }
            if (c == '\r')
            {
                return '\n';
            }
            return ' ';
        }
        private static void put(int c)
        {
            sw.Write((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        private static bool isAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                c > 126);
        }
    }

    public static class CssMinifier
    {
        public static int AppendReplacement(this Match match, StringBuilder sb, string input, string replacement, int index)
        {
            var preceding = input.Substring(index, match.Index - index);

            sb.Append(preceding);
            sb.Append(replacement);

            return match.Index + match.Length;
        }

        public static void AppendTail(this Match match, StringBuilder sb, string input, int index)
        {
            sb.Append(input.Substring(index));
        }

        public static uint ToUInt32(this ValueType instance)
        {
            return Convert.ToUInt32(instance);
        }

        public static string RegexReplace(this string input, string pattern, string replacement)
        {
            return Regex.Replace(input, pattern, replacement);
        }

        public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options)
        {
            return Regex.Replace(input, pattern, replacement, options);
        }

        public static string Fill(this string format, params object[] args)
        {
            return String.Format(format, args);
        }

        public static string RemoveRange(this string input, int startIndex, int endIndex)
        {
            return input.Remove(startIndex, endIndex - startIndex);
        }

        public static bool EqualsIgnoreCase(this string left, string right)
        {
            return String.Compare(left, right, true) == 0;
        }

        public static string ToHexString(this int value)
        {
            var sb = new StringBuilder();
            var input = value.ToString();

            foreach (char digit in input)
            {
                sb.Append("{0:x2}".Fill(digit.ToUInt32()));
            }

            return sb.ToString();
        }

        #region YUI Compressor's CssMin originally written by Isaac Schlueter

        /// <summary>
        /// Minifies CSS.
        /// </summary>
        /// <param name="css">The CSS content to minify.</param>
        /// <returns>Minified CSS content.</returns>
        public static string CssMinify(this string css)
        {
            return CssMinify(css, 0);
        }

        /// <summary>
        /// Minifies CSS with a column width maximum.
        /// </summary>
        /// <param name="css">The CSS content to minify.</param>
        /// <param name="columnWidth">The maximum column width.</param>
        /// <returns>Minified CSS content.</returns>
        public static string CssMinify(this string css, int columnWidth)
        {
            css = css.RemoveCommentBlocks();
            css = css.RegexReplace("\\s+", " ");
            css = css.RegexReplace("\"\\\\\"}\\\\\"\"", "___PSEUDOCLASSBMH___");
            css = css.RemovePrecedingSpaces();
            css = css.RegexReplace("([!{}:;>+\\(\\[,])\\s+", "$1");
            css = css.RegexReplace("([^;\\}])}", "$1;}");
            css = css.RegexReplace("([\\s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)", "$1$2");
            css = css.RegexReplace(":0 0 0 0;", ":0;");
            css = css.RegexReplace(":0 0 0;", ":0;");
            css = css.RegexReplace(":0 0;", ":0;");
            css = css.RegexReplace("background-position:0;", "background-position:0 0;");
            css = css.RegexReplace("(:|\\s)0+\\.(\\d+)", "$1.$2");
            css = css.ShortenRgbColors();
            css = css.ShortenHexColors();
            css = css.RegexReplace("[^\\}]+\\{;\\}", "");

            if (columnWidth > 0)
            {
                css = css.BreakLines(columnWidth);
            }

            css = css.RegexReplace("___PSEUDOCLASSBMH___", "\"\\\\\"}\\\\\"\"");
            css = css.Trim();

            return css;
        }

        private static string RemoveCommentBlocks(this string input)
        {
            var startIndex = 0;
            var endIndex = 0;
            var iemac = false;

            startIndex = input.IndexOf(@"/*", startIndex);
            while (startIndex >= 0)
            {
                endIndex = input.IndexOf(@"*/", startIndex + 2);
                if (endIndex >= startIndex + 2)
                {
                    if (input[endIndex - 1] == '\\')
                    {
                        startIndex = endIndex + 2;
                        iemac = true;
                    }
                    else if (iemac)
                    {
                        startIndex = endIndex + 2;
                        iemac = false;
                    }
                    else
                    {
                        input = input.RemoveRange(startIndex, endIndex + 2);
                    }
                }
                startIndex = input.IndexOf(@"/*", startIndex);
            }

            return input;
        }

        private static string ShortenRgbColors(this string css)
        {
            var sb = new StringBuilder();
            Regex p = new Regex("rgb\\s*\\(\\s*([0-9,\\s]+)\\s*\\)");
            Match m = p.Match(css);

            int index = 0;
            while (m.Success)
            {
                string[] colors = m.Groups[1].Value.Split(',');
                StringBuilder hexcolor = new StringBuilder("#");

                foreach (string color in colors)
                {
                    int val = Int32.Parse(color);
                    if (val < 16)
                    {
                        hexcolor.Append("0");
                    }
                    hexcolor.Append(val.ToHexString());
                }

                index = m.AppendReplacement(sb, css, hexcolor.ToString(), index);
                m = m.NextMatch();
            }

            m.AppendTail(sb, css, index);
            return sb.ToString();
        }

        private static string ShortenHexColors(this string css)
        {
            var sb = new StringBuilder();
            Regex p = new Regex("([^\"'=\\s])(\\s*)#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])");
            Match m = p.Match(css);

            int index = 0;
            while (m.Success)
            {
                if (m.Groups[3].Value.EqualsIgnoreCase(m.Groups[4].Value) &&
                    m.Groups[5].Value.EqualsIgnoreCase(m.Groups[6].Value) &&
                    m.Groups[7].Value.EqualsIgnoreCase(m.Groups[8].Value))
                {
                    var replacement = String.Concat(m.Groups[1].Value, m.Groups[2].Value, "#", m.Groups[3].Value, m.Groups[5].Value, m.Groups[7].Value);
                    index = m.AppendReplacement(sb, css, replacement, index);
                }
                else
                {
                    index = m.AppendReplacement(sb, css, m.Value, index);
                }

                m = m.NextMatch();
            }

            m.AppendTail(sb, css, index);
            return sb.ToString();
        }

        private static string RemovePrecedingSpaces(this string css)
        {
            var sb = new StringBuilder();
            Regex p = new Regex("(^|\\})(([^\\{:])+:)+([^\\{]*\\{)");
            Match m = p.Match(css);

            int index = 0;
            while (m.Success)
            {
                var s = m.Value;
                s = s.RegexReplace(":", "___PSEUDOCLASSCOLON___");

                index = m.AppendReplacement(sb, css, s, index);
                m = m.NextMatch();
            }
            m.AppendTail(sb, css, index);

            var result = sb.ToString();
            result = result.RegexReplace("\\s+([!{};:>+\\(\\)\\],])", "$1");
            result = result.RegexReplace("___PSEUDOCLASSCOLON___", ":");

            return result;
        }

        private static string BreakLines(this string css, int columnWidth)
        {
            int i = 0;
            int start = 0;

            var sb = new StringBuilder(css);
            while (i < sb.Length)
            {
                var c = sb[i++];
                if (c == '}' && i - start > columnWidth)
                {
                    sb.Insert(i, '\n');
                    start = i;
                }
            }
            return sb.ToString();
        }
        #endregion

    }
}


/*
 * @version   : 2.5.0
 * @author    : Ext.NET, Inc. http://www.ext.net/
 * @date      : 2014-10-20
 * @copyright : Copyright (c) 2008-2014, Ext.NET, Inc. (http://www.ext.net/). All rights reserved.
 * @license   : See license.txt and http://www.ext.net/license/. 
 * @website   : http://www.ext.net/
 */

/* Originally written in 'C', this code has been converted to the C# language.
 * The author's copyright message is reproduced below.
 * All modifications from the original to C# are placed in the public domain.
 */

/* jsmin.c
   2007-05-22

Copyright (c) 2002 Douglas Crockford  (www.crockford.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

The Software shall be used for Good, not Evil.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Ext.Net.Utilities
{
    using System;
    using System.IO;

    class JSMin
    {
        const int EOF = -1;

        StreamReader sr;
        StreamWriter sw;

        int theA;
        int theB;
        int theLookahead = EOF;

        public void Minify(StreamReader[] readers, string dst)
        {
            sw = new StreamWriter(dst);
            for (int i = 0; i < readers.Length; i++)
            {
                using (sr = readers[i])
                {
                    jsmin();
                }
            }
            sw.Close();
        }

        public void Minify(StreamReader reader, StreamWriter writer)
        {
            using (sr = reader)
            {
                using (sw = writer)
                {
                    jsmin();
                }
            }
        }

        public void Minify(string instance, string dst)
        {
            this.Minify(new StreamReader(instance), new StreamWriter(dst));
        }

        /* jsmin -- Copy the input to the output, deleting the characters which are
                insignificant to JavaScript. Comments will be removed. Tabs will be
                replaced with spaces. Carriage returns will be replaced with linefeeds.
                Most spaces and linefeeds will be removed.
        */
        void jsmin()
        {
            theA = '\n';
            action(3);
            while (theA != EOF)
            {
                switch (theA)
                {
                    case ' ':
                        {
                            if (isAlphanum(theB))
                            {
                                action(1);
                            }
                            else
                            {
                                action(2);
                            }
                            break;
                        }
                    case '\n':
                        {
                            switch (theB)
                            {
                                case '{':
                                case '[':
                                case '(':
                                case '+':
                                case '-':
                                    {
                                        action(1);
                                        break;
                                    }
                                case ' ':
                                    {
                                        action(3);
                                        break;
                                    }
                                default:
                                    {
                                        if (isAlphanum(theB))
                                        {
                                            action(1);
                                        }
                                        else
                                        {
                                            action(2);
                                        }
                                        break;
                                    }
                            }
                            break;
                        }
                    default:
                        {
                            switch (theB)
                            {
                                case ' ':
                                    {
                                        if (isAlphanum(theA))
                                        {
                                            action(1);
                                            break;
                                        }
                                        action(3);
                                        break;
                                    }
                                case '\n':
                                    {
                                        switch (theA)
                                        {
                                            case '}':
                                            case ']':
                                            case ')':
                                            case '+':
                                            case '-':
                                            case '"':
                                            case '\'':
                                                {
                                                    action(1);
                                                    break;
                                                }
                                            default:
                                                {
                                                    if (isAlphanum(theA))
                                                    {
                                                        action(1);
                                                    }
                                                    else
                                                    {
                                                        action(3);
                                                    }
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        action(1);
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }
        }
        /* action -- do something! What you do is determined by the argument:
                1   Output A. Copy B to A. Get the next B.
                2   Copy B to A. Get the next B. (Delete A).
                3   Get the next B. (Delete B).
           action treats a string as a single character. Wow!
           action recognizes a regular expression if it is preceded by ( or , or =.
        */
        void action(int d)
        {
            if (d <= 1)
            {
                put(theA);
            }
            if (d <= 2)
            {
                theA = theB;
                if (theA == '\'' || theA == '"')
                {
                    for (; ; )
                    {
                        put(theA);
                        theA = get();
                        if (theA == theB)
                        {
                            break;
                        }
                        if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated string literal: {0}\n", theA));
                        }
                        if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                    }
                }
            }
            if (d <= 3)
            {
                theB = next();
                if (theB == '/' && (theA == '(' || theA == ',' || theA == '=' ||
                                    theA == '[' || theA == '!' || theA == ':' ||
                                    theA == '&' || theA == '|' || theA == '?' ||
                                    theA == '{' || theA == '}' || theA == ';' ||
                                    theA == '\n'))
                {
                    put(theA);
                    put(theB);
                    for (; ; )
                    {
                        theA = get();
                        if (theA == '/')
                        {
                            break;
                        }
                        else if (theA == '\\')
                        {
                            put(theA);
                            theA = get();
                        }
                        else if (theA <= '\n')
                        {
                            throw new Exception(string.Format("Error: JSMIN unterminated Regular Expression literal : {0}.\n", theA));
                        }
                        put(theA);
                    }
                    theB = next();
                }
            }
        }
        /* next -- get the next character, excluding comments. peek() is used to see
                if a '/' is followed by a '/' or '*'.
        */
        int next()
        {
            int c = get();

            if (c == '/')
            {
                switch (peek())
                {
                    case '/':
                        {
                            for (; ; )
                            {
                                c = get();
                                if (c <= '\n')
                                {
                                    return c;
                                }
                            }
                        }
                    case '*':
                        {
                            get();
                            for (; ; )
                            {
                                switch (get())
                                {
                                    case '*':
                                        {
                                            if (peek() == '/')
                                            {
                                                get();
                                                return ' ';
                                            }
                                            break;
                                        }
                                    case EOF:
                                        {
                                            throw new Exception("Error: JSMIN Unterminated comment.\n");
                                        }
                                }
                            }
                        }
                    default:
                        {
                            return c;
                        }
                }
            }

            return c;
        }
        /* peek -- get the next character without getting it.
        */
        int peek()
        {
            theLookahead = get();

            return theLookahead;
        }
        /* get -- return the next character from stdin. Watch out for lookahead. If
                the character is a control character, translate it to a space or
                linefeed.
        */
        int get()
        {
            int c = theLookahead;
            theLookahead = EOF;

            if (c == EOF)
            {
                c = sr.Read();
            }
            if (c >= ' ' || c == '\n' || c == EOF)
            {
                return c;
            }
            if (c == '\r')
            {
                return '\n';
            }

            return ' ';
        }
        void put(int c)
        {
            sw.Write((char)c);
        }
        /* isAlphanum -- return true if the character is a letter, digit, underscore,
                dollar sign, or non-ASCII character.
        */
        bool isAlphanum(int c)
        {
            return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                c > 126);
        }
    }
}


public static class CSSMin
{
    public static string Bundle(string body)
    {
        body = Regex.Replace(body, @"[a-zA-Z]+#", "#");
        body = Regex.Replace(body, @"[\n\r]+\s*", string.Empty);
        body = Regex.Replace(body, @"\s+", " ");
        body = Regex.Replace(body, @"\s?([:,;{}])\s?", "$1");
        body = body.Replace(";}", "}");
        body = Regex.Replace(body, @"([\s:]0)(px|pt|%|em)", "$1");

        // Remove comments from CSS
        body = Regex.Replace(body, @"/\*[\d\D]*?\*/", string.Empty);

        return body;
    }
}
