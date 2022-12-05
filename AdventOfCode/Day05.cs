namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly string[] _input;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        Stack<byte>[] st = new Stack<byte>[9];
        
        foreach (string s in _input)
        {
            if (s[1] == '1') break;
            for (int i = 2; i< s.Length; i += 3)
            {
                
            }
        }

        return new("Not Solved");
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }
}
