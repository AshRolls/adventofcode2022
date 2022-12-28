using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode;

public class Day24 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day24()
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
        int w = _input[0].Length;
        int h = _input.Length;
        Cell[,] startGrid = new Cell[w, h];

        Dictionary<char, (int, int)> dirs = new Dictionary<char, (int, int)>();
        dirs.Add('^', (0, -1));
        dirs.Add('v', (0, 1));
        dirs.Add('<', (-1, 0));
        dirs.Add('>', (1, 0));
        dirs.Add('#', (0, 0));

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {                
                if (_input[y][x] != '.') startGrid[x, y] = new Cell(_input[y][x].ToString());
                else startGrid[x, y] = new Cell(String.Empty);
            }

        Dictionary<int, Cell[,]> minutes = new Dictionary<int, Cell[,]>();
        minutes.Add(0, startGrid);
        //printStorms(minutes[0], w, h);

        (int x, int y) end = (w - 2, h - 1);
        PriorityQueue<(int,int,int), int> queue = new PriorityQueue<(int,int,int), int>();
        HashSet<(int, int, int)> visited = new HashSet<(int, int, int)>();
        queue.Enqueue((1,0,0), AoCHelper.GetManhattanDist(1, 0, end.x, end.y));        
        int cnt = 0;
        int best = int.MaxValue;
        while (queue.Count > 0)
        {
            cnt++;
            (int x, int y, int m) state = queue.Dequeue();
            if (state.x == end.x && state.y == end.y)
            {
                if (state.m < best) best = state.m;
            }
            visited.Add(state);

            // generate next round if we don't already have it
            if (!minutes.ContainsKey(state.m + 1))
            {
                addMinute(state.m + 1, minutes, dirs, w, h);
                //printStorms(minutes[m + 1], w, h);
            }

            // add valid moves to queue
            int newX, newY, dist;
            foreach ((int x, int y) dir in dirs.Values)
            {
                newX = state.x + dir.x;
                newY = state.y + dir.y;
                dist = AoCHelper.GetManhattanDist(newX, newY, end.x, end.y);
                if ( ((newX > 0 && newX < w && newY > 0 && newY < h) || (newX == 1 && newY == 0))
                    && minutes[state.m + 1][newX, newY].Storms == String.Empty
                    && !visited.Contains((newX, newY, state.m + 1)) 
                    && (state.m + 1 + dist) < best)
                {                    
                    queue.Enqueue((newX, newY, state.m + 1), dist);
                }
            }
            //if (cnt % 5000000 == 0) Console.Out.WriteLine("{0}: {1} {2}", cnt, state.m, AoCHelper.GetManhattanDist(state.x, state.y, end.x, end.y));
        }

        _partOne = best.ToString();
    }

    private void printStorms(Cell[,] minutes, int w, int h)
    {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (minutes[x, y].Storms.Length == 0) sb.Append(' ');
                else if (minutes[x, y].Storms.Length == 1) sb.Append(minutes[x, y].Storms);
                else sb.Append(minutes[x, y].Storms.Length);
            }
            Console.Out.WriteLine(sb.ToString());
            sb.Clear();
        }
        Console.Out.WriteLine();
    }

    private struct Cell
    {
        public string Storms;

        public Cell(string storms)
        {
            this.Storms = storms;
        }
        
    }

    private void addMinute(int m, Dictionary<int, Cell[,]> minutes, Dictionary<char, (int, int)> stormDirs, int w, int h)
    {
        Cell[,] lastMinute = minutes[m - 1];

        Cell[,] newMinute = new Cell[w,h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                newMinute[x, y] = new Cell(String.Empty);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {                
                string storms = lastMinute[x, y].Storms;
                foreach (char c in storms)
                {
                    (int x, int y) newPos = (x, y);
                    switch (c)
                    {
                        case '^':
                            newPos = moveStorm(stormDirs['^'], x, y, w, h);
                            break;
                        case 'v':
                            newPos = moveStorm(stormDirs['v'], x, y, w, h);
                            break;
                        case '<':
                            newPos = moveStorm(stormDirs['<'], x, y, w, h);
                            break;
                        case '>':
                            newPos = moveStorm(stormDirs['>'], x, y, w, h);
                            break;
                    }
                    newMinute[newPos.x, newPos.y].Storms += c;
                }
            }
        }
        minutes.Add(m, newMinute);
    }

    private (int, int) moveStorm((int x, int y) dir, int x, int y, int w, int h)
    {
        int newX = x + dir.x;
        int newY = y + dir.y;
        if (newX == 0) newX = w - 2;
        else if (newX == w - 1) newX = 1;
        if (newY == 0) newY = h - 2;
        else if (newY == h - 1) newY = 1;
        return (newX, newY);
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
