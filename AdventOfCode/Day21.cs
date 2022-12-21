using System.ComponentModel.Design.Serialization;

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
        //solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        Dictionary<string, long> broadcasting = new Dictionary<string, long>();
        HashSet<Monkey> waiting = new HashSet<Monkey>();
        Monkey root = parseFile(broadcasting, waiting);
        runMonkeys(broadcasting, waiting, root);

        _partOne = broadcasting["root"].ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {        
        long p1 = 0;
        long p2 = 1;
        long lastp1;
        long humnVal = 1;
        GetVals(humnVal, out p1, out p2);
        lastp1 = p1;

        humnVal += 100000000;        
        GetVals(humnVal, out p1, out p2);

        long diff = lastp1 - p1;
        humnVal = ((p1 - p2) / diff) * 100000000;
        // above gets us reasonably close then hone in on the answer

        long plusVal = 1000000;
        long minusVal = 1000000;
        bool plus = p1 > p2;
        while (p1 != p2)
        {
            GetVals(humnVal, out p1, out p2);

            if (p1 == p2) break;
            if (p1 > p2)
            {
                if (!plus && plusVal > 1)
                    plusVal /= 10;
                humnVal += plusVal;
                plus = true;
            }
            else
            {
                if (plus && minusVal > 1)
                    minusVal /= 10;
                humnVal -= minusVal;
                plus = false;
            }
        }

        // note the val +1 above the actual answer will also mean p1 == p2. probably a bug in my solution with the divisor and rounding.
        _partTwo = humnVal.ToString();
    }

    private void GetVals(long humnVal, out long p1, out long p2)
    {
        Dictionary<string, long> broadcasting = new Dictionary<string, long>();
        HashSet<Monkey> waiting = new HashSet<Monkey>();
        Monkey root = parseFile(broadcasting, waiting);
        root.Op = '=';
        broadcasting["humn"] = humnVal;
        (long, long) result = runMonkeys(broadcasting, waiting, root, humnVal);
        p1 = result.Item1;
        p2 = result.Item2;
    }

    private static (long, long) runMonkeys(Dictionary<string, long> broadcasting, HashSet<Monkey> waiting, Monkey root, long humn = 1342)
    {
        while (waiting.Count > 0)
        {
            List<Monkey> monkeys = waiting.ToList();
            for (int i = 0; i < monkeys.Count; i++)
            {
                Monkey m = monkeys[i];
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
                        case '=':
                            // do nothing
                            break;
                    }
                    waiting.Remove(m);
                    broadcasting.Add(m.N, val);
                }
            }
        }
        return (broadcasting[root.M1], broadcasting[root.M2]);
    }

    private Monkey parseFile(Dictionary<string, long> broadcasting, HashSet<Monkey> waiting)
    {
        Monkey root = new Monkey();
        foreach (string line in _input)
        {
            var s = line.Split(':');
            if (s[1].Length < 6) broadcasting.Add(s[0], long.Parse(s[1]));
            else
            {
                var ms = s[1].Split(' ');
                Monkey m = new Monkey(s[0], ms[1], ms[3], ms[2][0]);
                waiting.Add(m);
                if (m.N == "root") root = m;
            }
        }
        return root;
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

   
}
