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
        _partOne = getVisited(1).ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        _visualiser.StartVisualiser(solve2);
        return new(_partTwo);
    }

    private void solve2()
    {
        _partTwo = getVisited(9).ToString();
    }

    private int getVisited(int vKnot)
    {
        Pos[] rPos = new Pos[10];
        for (int i = 0; i < rPos.Length; i++)
        {
            rPos[i] = new Pos() { X = 0, Y = 0 };
        }

        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        visited.Add((0,0));

        foreach (string line in _input)
        {
            char c = line[0];
            int v = int.Parse(line.Split(' ')[1]);
            switch (c)
            {
                case 'U':
                    move(v, 0, 1, ref rPos, visited, vKnot);
                    break;
                case 'D':
                    move(v, 0, -1, ref rPos, visited, vKnot);
                    break;
                case 'L':
                    move(v, -1, 0, ref rPos, visited, vKnot);
                    break;
                case 'R':
                    move(v, 1, 0, ref rPos, visited, vKnot);
                    break;
            }
        }

        return visited.Count();
    }

    private struct Pos
    {
        public int X;
        public int Y;
    }

    private void move(int v, int x, int y, ref Pos[] rPos, HashSet<(int, int)> visited, int vIdx)
    {
        for (int i = 0; i < v; i++)
        {
            rPos[0].X += x;
            rPos[0].Y += y;
            moveRope(ref rPos, visited, vIdx);
        }
    }

    private void moveRope(ref Pos[] rPos, HashSet<(int, int)> visited, int vIdx)
    {
        (int, int)[] rope = new (int, int)[10];
        rope[0] = (rPos[0].X, rPos[0].Y);

        for (int i = 1; i < rPos.Length; i++)
        {
            moveKnot(rPos[i - 1], ref rPos[i], visited, i == vIdx);
            rope[i] = (rPos[i].X, rPos[i].Y);
        }

        _visualiser.AddRenderItem(new Day09Vis.RenderItem(0, rope));
    }

    private static void moveKnot(Pos hPos, ref Pos tPos, HashSet<(int, int)> visited, bool isTrack)
    {
        int xDiff = hPos.X - tPos.X;
        int yDiff = hPos.Y - tPos.Y;
        if (Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1)
        {
            int x = AoCHelper.Clamp(xDiff, -1, 1);
            int y = AoCHelper.Clamp(yDiff, -1, 1);
            tPos.X += x;
            tPos.Y += y;
            if (isTrack)
            {
                (int, int) v = (tPos.X, tPos.Y);
                if (!visited.Contains(v)) visited.Add(v);                
            }
        }
    }
}
