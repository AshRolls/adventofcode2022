namespace AdventOfCode;

using System.Diagnostics;

public class Day01 : BaseDay
{
    private readonly string[] _input;

    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        int currentCal = 0;
        int highestCal = 0;

        foreach (string s in _input)
        {
            if (s.Length == 0)
            {
                currentCal = 0;
            }
            int lineCal;
            Int32.TryParse(s, out lineCal);
            currentCal += lineCal;
            if (currentCal > highestCal)
            {
                highestCal = currentCal;
            }
        }

        return new(highestCal.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }
}
