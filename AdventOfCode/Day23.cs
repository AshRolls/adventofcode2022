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


        Dictionary<int, List<(int, int)>> positions = new Dictionary<int, List<(int, int)>>();
        List<(int, int)> north = new List<(int, int)>() { (0, -1), (1, -1), (-1, -1) };
        List<(int, int)> south = new List<(int, int)>() { (0, 1), (1, 1), (-1, 1) };
        List<(int, int)> west = new List<(int, int)>() { (-1, 0), (-1, -1), (-1, 1) };
        List<(int, int)> east = new List<(int, int)>() { (1, 0), (1, -1), (1, 1) };
        List<(int, int)> all = new List<(int, int)>() { (-1, -1), (0, -1), (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0) };

        positions.Add(0, north);
        positions.Add(1, south);
        positions.Add(2, west);
        positions.Add(3, east);

        int w, h;
        for (int i = 0; i < 10; i++)
        {
            // find nearest neighbours
            for (int j = 0; j < elves.Count; j++)
            {
                Elf e = elves[j];
                e.NNs.Clear();
                foreach (Elf checkElf in elves)
                {
                    if (checkElf == e) continue;
                    int dist = AoCHelper.GetDiagonalDist(e.Pos.X, e.Pos.Y, checkElf.Pos.X, checkElf.Pos.Y);
                    if (dist <= 2) e.NNs.Add(checkElf, dist);
                }
            }
            // set proposed dir
            foreach (Elf e in elves) e.SetProposedDirection(i, positions, all);

            // move non-clashing elves
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
            
            //GetRectangle(elves, out w, out h);
        }

        GetRectangle(elves, out w, out h);

        int empty = ((w + 1) * (h + 1)) - elves.Count;
        _partOne = (empty.ToString());
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
                foreach(Elf e in elves)
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

        public void SetProposedDirection(int round, Dictionary<int, List<(int, int)>> positions, List<(int, int)> all)
        {
            if (NNs.Values.Select(x => x).Where(x => x > 1).Count() == NNs.Values.Count()) return;

            for (int i = 0; i < 4; i++)
            {
                bool found = false;
                foreach ((int x, int y) c in positions[(round + i) % 4])
                {
                    foreach (Elf nn in NNs.Keys)
                    {
                        if (nn.Pos == (this.Pos.X + c.x, this.Pos.Y + c.y))
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
        }

        public void MoveInProposedDirection()
        {
            this.Pos = this.Proposed;
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
