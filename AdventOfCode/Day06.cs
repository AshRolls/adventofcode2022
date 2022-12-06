using System.Net.Sockets;

namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly string[] _input;

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        return new(findMarker(4).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new(findMarker(14).ToString());
    }

    private int findMarker(int windowLength)
    {
        int i;
        for (i = 0; i < _input[0].Length - windowLength; i++)
        {
            var cur = _input[0].Skip(i).Take(windowLength);
            if (cur.Distinct().Count() == windowLength) break; 
        }
        return i + windowLength;
    }
}
