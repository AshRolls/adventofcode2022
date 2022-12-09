using AdventOfCode.vis;
using System.Linq;

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
        (int X, int Y)[] rope = new (int, int)[10];
        for (int i = 0; i < rope.Length; i++)
        {
            rope[i] = (0, 0);
        }

        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        visited.Add((0,0));

        foreach (string line in _input)
        {
            char c = line[0];
            int v = int.Parse(line[2..]);
            switch (c)
            {
                case 'U':
                    move(v, 0, 1, ref rope);
                    break;
                case 'D':
                    move(v, 0, -1, ref rope);
                    break;
                case 'L':
                    move(v, -1, 0, ref rope);
                    break;
                case 'R':
                    move(v, 1, 0, ref rope);
                    break;
            }

            (int, int) vis = (rope[vKnot].X, rope[vKnot].Y);
            if (!visited.Contains(vis)) visited.Add(vis);
        }

        return visited.Count();
    }

    private void move(int v, int x, int y, ref (int X, int Y)[] rope)
    {
        for (int i = 0; i < v; i++)
        {
            rope[0].X += x;
            rope[0].Y += y;
            moveRope(ref rope);

            // visualisation
            (int, int)[] renderRope = new (int, int)[10];
            rope.CopyTo(renderRope, 0);
            _visualiser.AddRenderItem(new Day09Vis.RenderItem(0, renderRope));
        }
    }

    private void moveRope(ref (int X, int Y)[] rope)
    {
        for (int i = 1; i < rope.Length; i++)
        {
            moveKnot(rope[i - 1], ref rope[i]);
        }
    }

    private static void moveKnot((int X, int Y) hPos, ref (int X, int Y) tPos)
    {
        int xDiff = hPos.X - tPos.X;
        int yDiff = hPos.Y - tPos.Y;
        if (Math.Abs(xDiff) > 1 || Math.Abs(yDiff) > 1)
        {
            int x = AoCHelper.Clamp(xDiff, -1, 1);
            int y = AoCHelper.Clamp(yDiff, -1, 1);
            tPos.X += x;
            tPos.Y += y;
        }
    }
}
