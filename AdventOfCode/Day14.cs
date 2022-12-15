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
        solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        int yMax = int.MinValue;
        foreach (string line in _input)
        {
            var vals = AoCHelper.GetNumsFromStr(line);
            for (int i = 0; i < vals.Length; i++)
            {
                if (i % 2 != 0 && vals[i] > yMax) yMax = vals[i];                
            }
        }

        bool[,] grid = new bool[1000, yMax + 2];
        parseGrid(grid);

        int sandAdded = 0;
        while(!addSand(grid, yMax))
            sandAdded++;

        _partOne = sandAdded.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        //_visualiser.StartVisualiser(solve2);
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        int width = 1000;
        int yMax = int.MinValue;
        foreach (string line in _input)
        {
            var vals = AoCHelper.GetNumsFromStr(line);
            for (int i = 0; i < vals.Length; i++) 
                if (i % 2 != 0 && vals[i] > yMax) yMax = vals[i];                            
        }

        bool[,] grid = new bool[width, yMax + 3];
        parseGrid(grid);
        addFloorToGrid(grid, width, yMax + 2);

        int sandAdded = 0;
        while (!addSand(grid, int.MaxValue)) 
            sandAdded++;
        sandAdded++;

        _partTwo = sandAdded.ToString();
    }

    private void addFloorToGrid(bool[,] grid, int width, int floor)
    {
        for(int x = 0; x<width; x++)
        {
            grid[x,floor] = true;
        }
    }

    private bool addSand(bool[,] grid, int yMax)
    {
        int x = 500;
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
                if (x == 500 && y == 0) return true;                                
            }
        }
        return false;
    }

    private void parseGrid(bool[,] grid)
    {       
        foreach (string line in _input)
        {
            var points = line.Split('>');
            (int x, int y) lastPoint;
            (int x, int y) curPoint;
            for (int i = 1; i < points.Length; i++)
            {
                var l = AoCHelper.GetNumsFromStr(points[i - 1]);
                var c = AoCHelper.GetNumsFromStr(points[i]);
                lastPoint = (l[0], l[1]);
                curPoint = (c[0], c[1]);

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
}
