namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);        
    }

    public override ValueTask<string> Solve_1()
    {
        int c = 0;
        foreach (string s in _input)
        {
            string[] pair = s.Split(',');
            int[] first = Array.ConvertAll(pair[0].Split('-'), str => int.Parse(str));
            int[] second = Array.ConvertAll(pair[1].Split('-'), str => int.Parse(str));

            if ((first[0] <= second[0]) && (first[1] >= second[1])) c++;
            else if ((second[0] <= first[0]) && (second[1] >= first[1])) c++;
        }

        return new(c.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }
}
