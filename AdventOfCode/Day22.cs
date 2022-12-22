using System;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCode;

public class Day22 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day22()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        solve1();
        return new(_partOne);
    }

    private struct Cmd
    {        
        public int Val;
        public int Dir;

        public Cmd(int val, int dir)
        {
            this.Val = val;
            this.Dir = dir;
        }
    }

    private Dictionary<int, (int, int)> moves = new Dictionary<int, (int, int)>() { 
        { 3, (0, -1) },
        { 0, (1, 0) },
        { 1, (0, 1) },
        { 2, (-1, 0) }
    };

    private struct Pos
    {
        public int X;
        public int Y;
        public int Facing;
    }
    private void solve1()
    {
        int width = _input.Take(_input.Length - 2).Select(x => x.Length).Max() + 2;
        int height = _input.Length - 2 + 2;
        char[,] map = new char[width, height];
        List<Cmd> cmds = new List<Cmd>();
        parseInput(width, height, map, cmds);

        // setup initial position
        Pos p = new Pos();
        for (int x = 1; x < width - 1; x++)
        {
            if (map[x, 1] == '.')
            {
                p.X = x;
                p.Y = 1;
                p.Facing = 0;
                break;
            }
        }

        //execute commands
        foreach (Cmd c in cmds)
        {
            //Console.Out.WriteLine("CMD: {0} {1}", c.Val, c.Dir);
            //Console.Out.WriteLine("INITIAL POS: {0} {1} {2}", p.X, p.Y, p.Facing);
                        
            p.Facing = (p.Facing + c.Dir) % 4;
            if (p.Facing == -1) p.Facing = 3;
            //Console.Out.WriteLine("TURNING NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
            for (int m = 0; m < c.Val; m++)
            {
                int nextX = p.X + moves[p.Facing].Item1;
                int nextY = p.Y + moves[p.Facing].Item2;
                char nextC = map[nextX, nextY];
                if (nextC == '#')
                {
                    //Console.Out.WriteLine("WALL NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                    break;
                }
                else if (nextC == '.')
                {
                    p.X = nextX;
                    p.Y = nextY;
                    //Console.Out.WriteLine("OPEN NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                }
                else if (nextC == ' ')
                {
                    while (nextC == ' ')
                    {
                        if (nextX >= width - 1) nextX = 0;
                        else if (nextX <= 0) nextX = width - 1;
                        if (nextY >= height - 1) nextY = 0;
                        else if (nextY <= 0) nextY = height - 1;
                        nextX += moves[p.Facing].Item1;
                        nextY += moves[p.Facing].Item2;                        
                        nextC = map[nextX, nextY];
                        //Console.Out.WriteLine("WRAP NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                    }
                    if (nextC != '#')
                    {
                        p.X = nextX;
                        p.Y = nextY;
                        //Console.Out.WriteLine("WRAP OPEN NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                    }
                    else
                    {
                        //Console.Out.WriteLine("WRAP WALL POS: {0} {1} {2}", p.X, p.Y, p.Facing);
                        break;
                    }
                }
            }
            Console.Out.WriteLine();
        }

        long password = (1000 * p.Y) + (4 * p.X) + p.Facing;

        _partOne = password.ToString();
    }

    private void parseInput(int width, int height, char[,] map, List<Cmd> cmds)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = ' ';
            }
        }

        string line;
        for (int y = 1; y < height - 1; y++)
        {
            line = _input[y - 1];
            for (int x = 1; x < width - 1; x++)
            {
                if (x - 1 < line.Length) map[x, y] = line[x - 1];
            }
        }

        line = _input[^1];
        string val = String.Empty;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != 'R' && line[i] != 'L') val += line[i];
            else
            {
                if (val != String.Empty)
                {
                    cmds.Add(new Cmd(int.Parse(val), 0));
                    val = String.Empty;
                }
                cmds.Add(new Cmd(0, line[i] == 'R' ? 1 : -1));
            }
        }
        if (val != String.Empty) cmds.Add(new Cmd(int.Parse(val), 0));
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
