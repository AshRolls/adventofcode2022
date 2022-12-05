using System.Diagnostics;

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
            int[] move = AoCHelper.NumsFromStr(_input[i]);
            for (int j = 0; j < move[0]; j++)
            {
                st[move[2]-1].Push(st[move[1] - 1].Pop());
            }
        }

        string topCrates = String.Empty;
        for (int i = 0; i < 9; i++)
        {
            topCrates += st[i].Peek();
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
            int[] move = AoCHelper.NumsFromStr(_input[i]);
            
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

        string topCrates = String.Concat(st.Select(s => s.Peek()));
        
        return new(topCrates);
    }
}
