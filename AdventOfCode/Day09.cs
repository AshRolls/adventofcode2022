using System.ComponentModel;

namespace AdventOfCode;

public class Day09 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        solve1();
        return new(_partOne);
    }

    private struct Pos
    {
        public int X; 
        public int Y;
    }

    private void solve1()
    {
        Pos hPos = new Pos() { X = 0, Y = 0 };
        Pos tPos = new Pos() { X = 0, Y = 0 }; 
        Dictionary<Tuple<int,int>,bool> visited = new Dictionary<Tuple<int, int>, bool>();
        visited.Add(new Tuple<int, int>(tPos.X,tPos.Y), true);

        foreach (string line in _input)
        {
            char c = line[0];
            int v = int.Parse(line.Split(' ')[1]);
            switch (c)
            {
                case 'U':
                    move(v, 0, 1, ref hPos, ref tPos, visited);
                    break;
                case 'D':
                    move(v, 0, -1, ref hPos, ref tPos, visited);
                    break;
                case 'L':
                    move(v, -1, 0, ref hPos, ref tPos, visited);
                    break;
                case 'R':
                    move(v, 1, 0, ref hPos, ref tPos, visited);
                    break;
            }
        }

        _partOne = visited.Count().ToString();
    }

    private void move(int v, int x, int y, ref Pos hPos, ref Pos tPos, Dictionary<Tuple<int, int>, bool> visited)
    {        
        for (int i = 0; i < v; i++)
        {
            hPos.X += x;
            hPos.Y += y;
            moveTail(hPos, ref tPos, visited);
        }
    }

    private void moveTail(Pos hPos, ref Pos tPos, Dictionary<Tuple<int, int>, bool> visited)
    {
        int xDiff = hPos.X - tPos.X;
        int yDiff = hPos.Y - tPos.Y;
        if (Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1)
        {
            int x = AoCHelper.Clamp(xDiff, -1, 1);
            int y = AoCHelper.Clamp(yDiff, -1, 1);
            tPos.X += x;
            tPos.Y += y;
            visited.TryAdd(new Tuple<int, int>(tPos.X, tPos.Y), true);
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
