using System.ComponentModel;
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
        //solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        int w, h;
        Cell[,] startGrid;
        Dictionary<char, (int, int)> dirs;
        parseInput(out w, out h, out startGrid, out dirs);

        Dictionary<int, Cell[,]> minutes = new Dictionary<int, Cell[,]>();
        minutes.Add(0, startGrid);
        //printStorms(minutes[0], w, h);
        (int x, int y) end = (w - 2, h - 1);

        PriorityQueue<State, int> queue = new PriorityQueue<State, int>();
        HashSet<State> visited = new HashSet<State>();
        queue.Enqueue(new State(1, 0, 0, 0), AoCHelper.GetManhattanDist(1, 0, end.x, end.y));
        int cnt = 0;
        int best = int.MaxValue;
        while (queue.Count > 0)
        {
            cnt++;
            State state = queue.Dequeue();
            if (state.X == end.x && state.Y == end.y)
            {
                if (state.M < best) best = state.M;
            }
            visited.Add(state);

            // generate next round if we don't already have it
            if (!minutes.ContainsKey(state.M + 1))
            {
                addMinute(state.M + 1, minutes, dirs, w, h);
                //printStorms(minutes[m + 1], w, h);
            }

            // add valid moves to queue
            int newX, newY, dist;
            foreach ((int x, int y) dir in dirs.Values)
            {
                newX = state.X + dir.x;
                newY = state.Y + dir.y;
                dist = AoCHelper.GetManhattanDist(newX, newY, end.x, end.y);
                if (((newX > 0 && newX < w && newY > 0 && newY < h) || (newX == 1 && newY == 0) || (newX == end.x && newY == end.y))
                    && minutes[state.M + 1][newX, newY].Storms == String.Empty
                    && !visited.Contains(new State(newX, newY, state.M + 1, 0))
                    && (state.M + 1 + dist) < best)
                {
                    queue.Enqueue(new State(newX, newY, state.M + 1, 0), dist);
                }
            }
            //if (cnt % 5000000 == 0) Console.Out.WriteLine("{0}: {1} {2}", cnt, state.M, AoCHelper.GetManhattanDist(state.X, state.Y, end.x, end.y));
        }

        _partOne = best.ToString();
    }

   

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        int w, h;
        Cell[,] startGrid;
        Dictionary<char, (int, int)> dirs;
        parseInput(out w, out h, out startGrid, out dirs);

        Dictionary<int, Cell[,]> minutes = new Dictionary<int, Cell[,]>();
        minutes.Add(0, startGrid);
        //printStorms(minutes[0], w, h);
        (int x, int y) start = (1, 0);
        (int x, int y) end = (w - 2, h - 1);

        PriorityQueue<State, int> queue = new PriorityQueue<State, int>();
        HashSet<State> visited = new HashSet<State>();
        int legDist = AoCHelper.GetManhattanDist(1, 0, end.x, end.y);
        queue.Enqueue(new State(1, 0, 0, 2), legDist * 3);
        int cnt = 0;
        int best = int.MaxValue;
        while (queue.Count > 0)
        {
            cnt++;
            State state = queue.Dequeue();
            visited.Add(state);

            if (state.V == 0 && state.Y == end.y && state.X == end.x && state.M < best) best = state.M;            

            // generate next round if we don't already have it
            if (!minutes.ContainsKey(state.M + 1))
            {
                addMinute(state.M + 1, minutes, dirs, w, h);
                //printStorms(minutes[m + 1], w, h);
            }

            
            int newX, newY, newV, dist, gX, gY;
            if (state.V == 1)
            {
                gX = start.x;
                gY = start.y;
            }
            else
            {
                gX = end.x;
                gY = end.y;
            }
            // add valid moves to queue
            foreach ((int x, int y) dir in dirs.Values)
            {
                newX = state.X + dir.x;
                newY = state.Y + dir.y;
                newV = state.V;
                if (state.V == 2 && newY == end.y && newX == end.x) newV--;
                else if (state.V == 1 && newY == start.y && newX == start.x) newV--;
                dist = AoCHelper.GetManhattanDist(newX, newY, gX, gY) + (newV * legDist);
                if (((newX > 0 && newX < w && newY > 0 && newY < h) || (newX == 1 && newY == 0) || (newX == end.x && newY == end.y))
                    && minutes[state.M + 1][newX, newY].Storms == String.Empty
                    && !visited.Contains(new State(newX, newY, state.M + 1, newV))
                    && (state.M + 1 + dist) < best)
                {                                                        
                    queue.Enqueue(new State(newX, newY, state.M + 1, newV), dist);
                }
            }
            if (cnt % 100000 == 0) Console.Out.WriteLine("{0}: {1} {2}", cnt, state.M, state.V);
        }

        _partTwo = best.ToString();
    }

    private void parseInput(out int w, out int h, out Cell[,] startGrid, out Dictionary<char, (int, int)> dirs)
    {
        w = _input[0].Length;
        h = _input.Length;
        startGrid = new Cell[w, h];

        dirs = new Dictionary<char, (int, int)>();
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
    }

    private struct State
    {
        public int X, Y, M, V;

        public State(int x, int y, int m, int v)
        {
            X = x;
            Y = y;
            M = m;
            V = v;
        }
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


}
