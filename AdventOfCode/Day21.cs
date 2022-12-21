namespace AdventOfCode;

public class Day21 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day21()
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
        Dictionary<string, long> broadcasting = new Dictionary<string, long>();
        List<Monkey> waiting = new List<Monkey>();

        foreach (string line in _input)
        {
            var s = line.Split(':');
            if (s[1].Length < 6) broadcasting.Add(s[0], long.Parse(s[1]));
            else
            {
                var ms = s[1].Split(' ');
                Monkey m = new Monkey(s[0], ms[1], ms[3], ms[2][0]);
                waiting.Add(m);
            }
        }

        while (waiting.Count > 0)
        {
            for (int i = 0; i < waiting.Count; i++)
            {
                Monkey m = waiting[i];
                long v1;
                long v2;
                if (broadcasting.TryGetValue(m.M1, out v1) && broadcasting.TryGetValue(m.M2, out v2))
                {
                    long val = 0;
                    switch (m.Op)
                    {
                        case '+':
                            val = v1 + v2;
                            break;
                        case '-':
                            val = v1 - v2;
                            break;
                        case '*':
                            val = v1 * v2;
                            break;
                        case '/':
                            val = v1 / v2;
                            break;
                    }
                    waiting.Remove(m);
                    broadcasting.Add(m.N, val);
                }
            }
        }

        _partOne = broadcasting["root"].ToString();
    }

    private struct Monkey
    {
        public string N;
        public string M1;
        public string M2;
        public char Op;

        public Monkey(string n, string m1, string m2, char op)
        {
            N = n;
            M1 = m1;
            M2 = m2;
            Op = op;
        }
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        _partTwo = "Not Solved";
    }
}
