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
        int i;
        for (i = 0; i < _input[0].Length - 4;i++) 
        {
            var cur = _input[0].Skip(i).Take(4);
            if (cur.Distinct().Count() == 4) { break; }
        }

        return new((i+4).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }
}
