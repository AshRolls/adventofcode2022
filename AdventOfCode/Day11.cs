using Spectre.Console;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Numerics;

namespace AdventOfCode;

public class Day11 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day11()
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
        Monkey[] monkeys = parseInput(); 
        
        for (int i = 0; i < 20; i++)
        {
            performRound(monkeys);
        }

        var insp = monkeys.Select(x => x._inspected).ToList();
        insp.Sort();
        insp.Reverse();

        _partOne = (insp[0] * insp[1]).ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        Monkey[] monkeys = parseInput();

        Console.Out.WriteLine("starting");
        for (int i = 0; i < 10000; i++)
        {
            Console.Out.Write(i.ToString() + " ");
            performRound(monkeys, false);
        }

        var insp = monkeys.Select(x => x._inspected).ToList();
        insp.Sort();
        insp.Reverse();

        _partTwo = (insp[0] * insp[1]).ToString();
    }

    private Monkey[] parseInput()
    {
        Monkey[] monkeys = new Monkey[8];
        for (int i = 0; i < 8; i++)
        {
            monkeys[i] = parseMonkey(_input[(i * 7)..(i * 7 + 6)]);
        }
        for (int i = 0; i < 8; i++)
        {
            setThrows(i, monkeys, _input[(i * 7)..(i * 7 + 6)]);
        }
        return monkeys;
    }

    static int sModM(string s, int mod)
    {
        int number = 0;
        for (int i = 0; i < s.Length; i++)
        {
            number *= 10;
            int x = int.Parse(s[i].ToString());
            number += x;
            number %= mod;
        }
        return number;
    }

    private void performRound(Monkey[] monkeys, bool worry = true)
    {
        for (int i = 0; i < 8; i++)
        {
            Monkey m = monkeys[i];
            while (m._items.Any())
            {
                m._inspected++;
                BigInteger item = m._items.Dequeue();
                item = m.Operation(item, m._opVal);
                if (worry) item = item / 3; // Math.Floor automatically as all ints
                if (m.Test(item)) m._trueMonkey._items.Enqueue(item);
                else m._falseMonkey._items.Enqueue(item);
            }
        }
    }

    private void setThrows(int i, Monkey[] monkeys, string[] strings)
    {
        int trueI = int.Parse(strings[4].Split(' ').Last());
        int falseI = int.Parse(strings[5].Split(' ').Last());
        monkeys[i]._trueMonkey = monkeys[trueI];
        monkeys[i]._falseMonkey = monkeys[falseI];
    }

    private Monkey parseMonkey(string[] strs)
    {
        string opValStr = strs[2].Split(' ').Last();
        int opVal;
        bool opSelf = false;
        if (!int.TryParse(opValStr, out opVal))
        {
            opSelf = true;
        }
        int testVal = AoCHelper.NumsFromStr(strs[3]).First();

        Monkey m = new Monkey(opVal, opSelf, testVal);

        foreach (int w in AoCHelper.NumsFromStr(strs[1]))
        {
            m._items.Enqueue(w);
        }

        string op = strs[2].Split(' ')[^2];
        if (op[0] == '+')
        {
            m.Operation = Monkey.OperationAdd;
        }
        else if (!opSelf)
        {
            m.Operation = Monkey.OperationMult;
        }
        else
        {
            m.Operation = Monkey.OperationMultSelf;
        }

        return m;
    }

    private class Monkey
    {
        public Queue<BigInteger> _items = new Queue<BigInteger>();
        public Func<BigInteger, int, BigInteger> Operation;
        public bool _opSelf = false;
        public int _opVal;
        public int _testVal;
        public Monkey _trueMonkey;
        public Monkey _falseMonkey;
        public long _inspected;

        public Monkey(int operationVal, bool opSelf, int testVal)
        {
            _opVal = operationVal;
            _opSelf = opSelf;
            _testVal = testVal;
            _inspected = 0;
        }

        public bool Test(BigInteger val)
        {
            return (val % _testVal == 0);
        }

        public static BigInteger OperationMult(BigInteger val, int op)
        {
            return BigInteger.Multiply(val, op);
        }

        public static BigInteger OperationMultSelf(BigInteger val, int opUnused)
        {
            return BigInteger.Pow(val, 2);
        }

        public static BigInteger OperationAdd(BigInteger val, int op)
        {
            return BigInteger.Add(val, op);
        }
    }
}
