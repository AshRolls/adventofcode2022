using System.Text.RegularExpressions;

namespace AdventOfCode
{
    internal static class AoCHelper
    {
        public static int[] NumsFromStr(string s) => Array.ConvertAll(Regex.Matches(s, @"[0-9]+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray(), x => int.Parse(x));    
        
    }
}
