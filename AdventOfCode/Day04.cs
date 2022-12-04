using System.Security.Principal;

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
        int c = 0;
        foreach (string s in _input)
        {
            string[] pair = s.Split(',');
            int[] first = Array.ConvertAll(pair[0].Split('-'), str => int.Parse(str));
            int[] second = Array.ConvertAll(pair[1].Split('-'), str => int.Parse(str));

            var r1 = EnumerateTo(first[0], first[1]);
            var r2 = EnumerateTo(second[0], second[1]);
            
            if (r1.Intersect(r2).Count() > 0) c++;
        }

        return new(c.ToString());
    }

    public IEnumerable<int> EnumerateTo(int from, int to)
    {
        for (int i=from;i<=to;i++) yield return i;        
    }
}
