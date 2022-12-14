using AdventOfCode.vis;
using System.Runtime.ConstrainedExecution;

namespace AdventOfCode;

public class Day14 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private Day14Vis _visualiser = new Day14Vis();

    public Day14()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        _visualiser.StartVisualiser(solve1);
        //solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        int xMax = int.MinValue;
        int xMin = int.MaxValue;
        int yMax = int.MinValue;
        foreach (string line in _input)
        {
            var vals = AoCHelper.NumsFromStr(line);
            for (int i = 0; i < vals.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (vals[i] > xMax) xMax = vals[i];
                    if (vals[i] < xMin) xMin = vals[i];
                }
                else
                {
                    if (vals[i] > yMax) yMax = vals[i];
                }
            }
        }

        bool[,] grid = new bool[xMax - xMin + 2, yMax + 1];
        parseGrid(grid,xMin);

        int sandAdded = 0;
        while(!addSand(grid, xMin, yMax))
        {
            sandAdded++;
        }

        _partOne = sandAdded.ToString();
    }

    private bool addSand(bool[,] grid, int xMin, int yMax)
    {
        int x = 500 - xMin;
        int y = 0;
        bool rest = false;

        while (!rest)
        {
            if (y >= yMax) return true;
            else if (!grid[x, y + 1]) y++;
            else if (!grid[x - 1, y + 1]) { x--; y++; }
            else if (!grid[x + 1, y + 1]) { x++; y++; }
            else
            {
                grid[x, y] = true;
                rest = true;
                _visualiser.AddRenderItem(new Day14Vis.RenderItem(1, x, y, String.Empty));
            }
        }
        return false;
    }

    private void parseGrid(bool[,] grid, int xMin)
    {       
        foreach (string line in _input)
        {
            var points = line.Split('>');
            (int x, int y) lastPoint;
            (int x, int y) curPoint;
            for (int i = 1; i < points.Length; i++)
            {
                var l = AoCHelper.NumsFromStr(points[i - 1]);
                var c = AoCHelper.NumsFromStr(points[i]);
                lastPoint = (l[0] - xMin, l[1]);
                curPoint = (c[0] - xMin, c[1]);

                // horizontal
                if (lastPoint.x == curPoint.x)
                {
                    if (lastPoint.y < curPoint.y)
                    {
                        for (int y = lastPoint.y; y <= curPoint.y; y++)
                        {
                            grid[lastPoint.x, y] = true;
                            _visualiser.AddRenderItem(new Day14Vis.RenderItem(0, lastPoint.x, y, String.Empty));
                        }
                    }
                    else
                    {
                        for (int y = lastPoint.y; y >= curPoint.y; y--)
                        {
                            grid[lastPoint.x, y] = true;
                            _visualiser.AddRenderItem(new Day14Vis.RenderItem(0, lastPoint.x, y, String.Empty));
                        }
                    }
                }
                // vertical
                else
                {
                    if (lastPoint.x < curPoint.x)
                    {
                        for (int x = lastPoint.x; x <= curPoint.x; x++)
                        {
                            grid[x, lastPoint.y] = true;
                            _visualiser.AddRenderItem(new Day14Vis.RenderItem(0, x, lastPoint.y, String.Empty));
                        }
                    }
                    else
                    {
                        for (int x = lastPoint.x; x >= curPoint.x; x--)
                        {
                            grid[x, lastPoint.y] = true;
                            _visualiser.AddRenderItem(new Day14Vis.RenderItem(0, x, lastPoint.y, String.Empty));
                        }
                    }
                }
            }
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
