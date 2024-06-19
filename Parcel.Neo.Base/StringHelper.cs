using System;
using System.Collections.Generic;
using System.Linq;

namespace Parcel.Neo.Base
{
    public static class StringHelper
    {
        #region Command-Line String Parsing
        public static IEnumerable<string> SplitCSVLine(this string csvline)
            => SplitCommandLine(csvline, ',');
        public static IEnumerable<string> SplitCommandLine(this string commandLine, char delimiter = ' ')
        {
            bool inQuotes = false;
            
            return commandLine.Split(c =>
                {
                    if (c == '\"')
                        inQuotes = !inQuotes;

                    return !inQuotes && c == delimiter;
                })
                .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                .Where(arg => !string.IsNullOrEmpty(arg));
        }
        private static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }
        private static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) && 
                (input[0] == quote) && (input[^1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }
        #endregion

        #region Tags
        public static string[] SplitTags(string csv, char splitter = ',')
            => csv.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().ToLower()).Distinct().OrderBy(t => t).ToArray();
        public static string DisplayTags(string[] itemTags) => string.Join(", ", itemTags);
        #endregion
    }
}