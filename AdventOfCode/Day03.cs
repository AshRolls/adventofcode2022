using System.Diagnostics;
using System.Text;

namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string[] _input;

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        int total = 0;
        foreach(string s in _input)
        {
            int half = s.Length/2;
            Debug.Assert((s.Length % 2) == 0);
            byte[] first = Encoding.ASCII.GetBytes(s.Substring(0, half));
            byte[] second = Encoding.ASCII.GetBytes(s.Substring(half , half));
            Debug.Assert(first.Length == second.Length);

            List<byte> common = first.Intersect(second).ToList();

            int priority = 0;
            foreach(byte b in common)
            {
                int p = b;
                if (b < 91) p -= (64 - 26);
                else p -= 96;
                priority += p;
            }
            total += priority;
        }

        return new(total.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }
}
