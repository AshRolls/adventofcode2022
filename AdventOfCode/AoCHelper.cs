using System.Text.RegularExpressions;

namespace AdventOfCode
{
    internal static class AoCHelper
    {
        public static int[] NumsFromStr(string s) => Array.ConvertAll(Regex.Matches(s, @"[0-9]+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray(), x => int.Parse(x));

        
        public static string ToDebugString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
            //Console.Out.WriteLine(ToDebugString<string,int>(clonedVisited) + " " + maxSmallCaves);
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }

    
}
