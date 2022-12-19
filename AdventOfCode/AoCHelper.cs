using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    internal static class AoCHelper
    {
        public static int[] GetNumsFromStr(string s) => Array.ConvertAll(Regex.Matches(s, @"-?\d+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray(), x => int.Parse(x));
        
        public static string ToDebugString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
            //Console.Out.WriteLine(ToDebugString<string,int>(clonedVisited) + " " + maxSmallCaves);
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static int GetCommonDivisor(int a, int b)
        {
            if (a == 0)
                return b;
            return GetCommonDivisor(b % a, a);
        }

        public static long GetCommonDivisor(long a, long b)
        {
            if (a == 0)
                return b;
            return GetCommonDivisor(b % a, a);
        }

        public static IEnumerable<long> GetAllDivisors(long[] arr, long N)
        {
            // Variable to find the gcd
            // of N numbers
            long g = arr[0];

            // Set to store all the
            // common divisors
            HashSet<long> divisors = new HashSet<long>();

            // Finding GCD of the given
            // N numbers
            for (int i = 1; i < N; i++)
            {
                g = GetCommonDivisor(arr[i], g);
            }

            // Finding divisors of the
            // HCF of n numbers
            for (int i = 1; i * i <= g; i++)
            {
                if (g % i == 0)
                {
                    divisors.Add(i);
                    if (g / i != i)
                        divisors.Add(g / i);
                }
            }

            return divisors;
        }

        public static int GetManhattanDist(int fromX, int fromY, int toX, int toY)
        {
            return Math.Abs(fromX - toX) + Math.Abs(fromY - toY);
        }

        public const char SolidBlockChar = '\u2588';

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }    
}
