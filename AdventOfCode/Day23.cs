using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode;

public class Day23 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day23()
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
        List<Elf> elves = parseInput();

        Dictionary<int, List<(int, int)>> positions = setupPositions();
        List<(int, int)> all = new List<(int, int)>() { (-1, -1), (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0) };

        int w, h;
        int stationary = 0;
        for (int i = 0; i < 10; i++)
        {
            // find nearest neighbours
            findNearestNeighbours(2, elves);

            // set proposed dir
            stationary = 0;
            foreach (Elf e in elves)
            {
                if (e.SetProposedDirection(i, positions, all)) stationary++;
            }
            if (stationary == elves.Count) break;

            // move non-clashing elves
            moveElves(elves);
        }

        GetRectangle(elves, out w, out h);

        int empty = ((w + 1) * (h + 1)) - elves.Count;
        _partOne = (empty.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        List<Elf> elves = parseInput();

        Dictionary<int, List<(int, int)>> positions = setupPositions();
        List<(int, int)> all = new List<(int, int)>() { (-1, -1), (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0) };

        int stationary = 0;
        int i = 0;
        while (true)
        {
            // find nearest neighbours
            if (i % 5 == 0) findNearestNeighbours(7, elves);

            // set proposed dir
            stationary = 0;
            foreach (Elf e in elves)
            {
                if (e.SetProposedDirection(i, positions, all, true)) stationary++;
            }
            if (stationary == elves.Count) break;

            // move non-clashing elves
            moveElves(elves);
            i++;
#if DEBUG
            if (i % 25 == 0)
            {
                Console.Out.WriteLine("{0}: {1}/{2}", i, stationary, elves.Count);
            }
#endif
        }

        _partTwo = (i+1).ToString();
    }

    private static void moveElves(List<Elf> elves)
    {
        Dictionary<(int, int), Elf> proposed = new Dictionary<(int, int), Elf>();
        HashSet<Elf> clashes = new HashSet<Elf>();
        foreach (Elf e in elves)
        {
            if (!proposed.TryAdd(e.Proposed, e))
            {
                _ = clashes.Add(e);
                _ = clashes.Add(proposed[e.Proposed]);
            }
        }
        foreach (Elf e in clashes) e.Proposed = e.Pos;
        foreach (Elf e in elves) e.MoveInProposedDirection();
    }

    private static void findNearestNeighbours(int maxDist, List<Elf> elves)
    {
        for (int j = 0; j < elves.Count; j++)
        {
            Elf e = elves[j];
            e.NNs.Clear();
            foreach (Elf checkElf in elves)
            {
                if (checkElf == e) continue;
                int dist = AoCHelper.GetDiagonalDist(e.Pos.X, e.Pos.Y, checkElf.Pos.X, checkElf.Pos.Y);
                if (dist <= maxDist) e.NNs.Add(checkElf, dist);
            }
        }
    }

    private static Dictionary<int, List<(int, int)>> setupPositions()
    {
        Dictionary<int, List<(int, int)>> positions = new Dictionary<int, List<(int, int)>>();
        List<(int, int)> north = new List<(int, int)>() { (0, -1), (1, -1), (-1, -1) };
        List<(int, int)> south = new List<(int, int)>() { (0, 1), (1, 1), (-1, 1) };
        List<(int, int)> west = new List<(int, int)>() { (-1, 0), (-1, -1), (-1, 1) };
        List<(int, int)> east = new List<(int, int)>() { (1, 0), (1, -1), (1, 1) };

        positions.Add(0, north);
        positions.Add(1, south);
        positions.Add(2, west);
        positions.Add(3, east);
        return positions;
    }

    private class Elf
    {
        public (int X, int Y) Pos;        
        public Dictionary<Elf,int> NNs;
        public (int X, int Y) Proposed;        

        public Elf((int,int) pos)
        {
            Pos = pos;
            Proposed = pos;
            NNs = new Dictionary<Elf, int>();
        }

        public bool SetProposedDirection(int round, Dictionary<int, List<(int, int)>> positions, List<(int, int)> all, bool recalcNN = false)
        {
            if (recalcNN) 
                foreach (Elf checkElf in NNs.Keys)
                    NNs[checkElf] = AoCHelper.GetDiagonalDist(Pos.X, Pos.Y, checkElf.Pos.X, checkElf.Pos.Y);
            
            if (NNs.Values.Select(x => x).Where(x => x > 1).Count() == NNs.Values.Count()) return true;

            for (int i = 0; i < 4; i++)
            {
                bool found = false;
                foreach ((int x, int y) c in positions[(round + i) % 4])
                {
                    foreach (KeyValuePair<Elf,int> nn in NNs)
                    {
                        if (nn.Value <= 2 && nn.Key.Pos == (this.Pos.X + c.x, this.Pos.Y + c.y))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (!found)
                {
                    switch ((round + i) % 4)
                    {
                        case 0:
                            this.Proposed = (Pos.X, Pos.Y - 1);                            
                            break;
                        case 1:
                            this.Proposed = (Pos.X, Pos.Y + 1);
                            break;
                        case 2:
                            this.Proposed = (Pos.X - 1, Pos.Y);
                            break;
                        case 3:
                            this.Proposed = (Pos.X + 1, Pos.Y);
                            break;
                    }
                    break;
                }
            }

            return false;
        }

        public void MoveInProposedDirection()
        {
            this.Pos = this.Proposed;
        }
    }

    private void GetRectangle(List<Elf> elves, out int w, out int h)
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Elf e in elves)
        {
            if (e.Pos.X > maxX) maxX = e.Pos.X;
            if (e.Pos.X < minX) minX = e.Pos.X;
            if (e.Pos.Y > maxY) maxY = e.Pos.Y;
            if (e.Pos.Y < minY) minY = e.Pos.Y;
        }

        w = maxX - minX;
        h = maxY - minY;
        //printGrid(minX, maxX, minY, maxY, elves);
    }

    private void printGrid(int minX, int maxX, int minY, int maxY, List<Elf> elves)
    {
        StringBuilder sb = new StringBuilder();
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                bool found = false;
                foreach (Elf e in elves)
                {
                    if (e.Pos == (x, y))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) sb.Append(".");
                else sb.Append("#");
            }
            Console.Out.WriteLine(sb.ToString());
            sb.Clear();
        }
        Console.Out.WriteLine();
    }

    private List<Elf> parseInput()
    {
        List<Elf> elves = new List<Elf>();
        string line;
        for (int y = 0; y < _input.Length; y++)
        {
            line = _input[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#') elves.Add(new Elf((x, y)));
            }
        }

        return elves;
    }
}
