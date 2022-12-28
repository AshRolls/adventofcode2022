using System.Text;

namespace AdventOfCode;

public class Day25 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day25()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        List<List<int>> snafuIn = new List<List<int>>();
        foreach (string line in _input)
        {
            List<int> vals = new List<int>();
            for (int i = line.Length - 1; i >= 0; i--)
            {
                switch (line[i])
                {
                    case '=': vals.Add(-2); break;
                    case '-': vals.Add(-1); break;
                    default: vals.Add(int.Parse(line[i].ToString())); break;
                }
            }
            snafuIn.Add(vals);
        }

        long base5, base10;
        long total = 0;
        foreach (List<int> snafu in snafuIn)
        {
            base5 = 1;
            base10 = 0;
            foreach (int v in snafu)
            {
                base10 += v * base5;
                base5 *= 5;
            }
            total += base10;
        }

        char[] snafuDigit = new char[5] {'0', '1', '2', '=', '-'};
        string snafuS = string.Empty;
        base5 = 1;
        long newTotal;
        long newAdd = 0;        
        snafuS += snafuDigit[total % 5]; // first digit
        while (base5 * 5 < total)
        {
            newAdd += 2 * base5;
            base5 *= 5;
            newTotal = (total + newAdd) / base5;
            snafuS += snafuDigit[(newTotal) % 5];
        }
     
        _partOne = new String(snafuS.Reverse().ToArray());
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        _partTwo = "Not Set";
    }
}
