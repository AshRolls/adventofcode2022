using System.Diagnostics;
using System.Text.RegularExpressions;

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
        Stack<char>[] st = new Stack<char>[9];
        for (int i = 0; i < 9; i++) st[i] = new Stack<char>();

        foreach (string s in _input)
        {
            if (s[1] == '1') break;
            for (int i = 0; i< 9; i++)
            {
                int stackIdx = (i * 4) + 1;
                if (s[stackIdx] != ' ') st[i].Push(s[stackIdx]);                
            }
        }

        for (int i = 0; i < 9; i++)
        {
            st[i] = new Stack<char>(st[i]);
        }

        for (int i = 10; i < _input.Length; i++)
        {
            int[] move = Array.ConvertAll(Regex.Matches(_input[i], @"[0-9]+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray(), x => int.Parse(x));
            for (int j = 0; j < move[0]; j++)
            {
                st[move[2]-1].Push(st[move[1] - 1].Pop());
            }
        }

        string topCrates = String.Empty;
        for (int i = 0; i < 9; i++)
        {
            topCrates += st[i].Pop();
        }

        return new(topCrates);
    }

    public override ValueTask<string> Solve_2()
    {
        Stack<char>[] st = new Stack<char>[9];
        for (int i = 0; i < 9; i++) st[i] = new Stack<char>();

        foreach (string s in _input)
        {
            if (s[1] == '1') break;
            for (int i = 0; i < 9; i++)
            {
                int stackIdx = (i * 4) + 1;
                if (s[stackIdx] != ' ') st[i].Push(s[stackIdx]);
            }
        }

        for (int i = 0; i < 9; i++)
        {
            st[i] = new Stack<char>(st[i]);
        }

        Stack<char> q = new Stack<char>();
        for (int i = 10; i < _input.Length; i++)
        {
            int[] move = Array.ConvertAll(Regex.Matches(_input[i], @"[0-9]+").OfType<Match>().Select(m => m.Groups[0].Value).ToArray(), x => int.Parse(x));
            
            for (int j = 0; j < move[0]; j++)
            {
                q.Push(st[move[1] - 1].Pop());
            }
            for (int j = 0; j < move[0]; j++)
            {
                st[move[2] - 1].Push(q.Pop());
            }
            Debug.Assert(q.Count() == 0);
        }

        string topCrates = String.Empty;
        for (int i = 0; i < 9; i++)
        {
            topCrates += st[i].Pop();
        }

        return new(topCrates);
    }
}
