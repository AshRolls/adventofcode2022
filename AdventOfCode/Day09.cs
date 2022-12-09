using AdventOfCode.vis;

namespace AdventOfCode;

public class Day09 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private Day09Vis _visualiser = new Day09Vis();

    public Day09()
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

    public override ValueTask<string> Solve_2()
    {
        _visualiser.StartVisualiser(solve2);
        return new(_partTwo);
    }

    private void solve2()
    {
        Pos[] rPos = new Pos[10];
        for (int i = 0; i < rPos.Length; i++)
        {
            rPos[i] = new Pos() { X = 0, Y = 0 };
        }

        Dictionary<Tuple<int, int>, bool> visited = new Dictionary<Tuple<int, int>, bool>();
        visited.Add(new Tuple<int, int>(0, 0), true);

        foreach (string line in _input)
        {
            char c = line[0];
            int v = int.Parse(line.Split(' ')[1]);
            switch (c)
            {
                case 'U':
                    move(v, 0, 1, ref rPos, visited);
                    break;
                case 'D':
                    move(v, 0, -1, ref rPos, visited);
                    break;
                case 'L':
                    move(v, -1, 0, ref rPos, visited);
                    break;
                case 'R':
                    move(v, 1, 0, ref rPos, visited);
                    break;
            }
        }

        _partTwo = visited.Count().ToString();
    }    

    private struct Pos
    {
        public int X;
        public int Y;
    }

    private void move(int v, int x, int y, ref Pos hPos, ref Pos tPos, Dictionary<Tuple<int, int>, bool> visited)
    {        
        for (int i = 0; i < v; i++)
        {
            hPos.X += x;
            hPos.Y += y;
            moveKnot(hPos, ref tPos, visited);
        }
    }

    private void move(int v, int x, int y, ref Pos[] rPos, Dictionary<Tuple<int, int>, bool> visited)
    {
        for (int i = 0; i < v; i++)
        {
            rPos[0].X += x;
            rPos[0].Y += y;
            moveRope(ref rPos, visited);
        }
    }

    private void moveKnot(Pos hPos, ref Pos tPos, Dictionary<Tuple<int, int>, bool> visited, bool isTail = true)
    {
        int xDiff = hPos.X - tPos.X;
        int yDiff = hPos.Y - tPos.Y;
        if (Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1)
        {
            int x = AoCHelper.Clamp(xDiff, -1, 1);
            int y = AoCHelper.Clamp(yDiff, -1, 1);
            tPos.X += x;
            tPos.Y += y;
            if (isTail) visited.TryAdd(new Tuple<int, int>(tPos.X, tPos.Y), true);
        }
    }

    private void moveRope(ref Pos[] rPos, Dictionary<Tuple<int, int>, bool> visited)
    {
        Tuple<int, int>[] rope = new Tuple<int, int>[10];
        rope[0] = new Tuple<int, int>(rPos[0].X, rPos[0].Y);
        
        for (int i = 1; i < rPos.Length; i++)
        {            
            moveKnot(rPos[i-1], ref rPos[i], visited, i == 9);
            rope[i] = new Tuple<int, int>(rPos[i].X, rPos[i].Y);
        }

        _visualiser.AddRenderItem(new Day09Vis.RenderItem(0, rope));
    }
}
