using System.Diagnostics;

namespace AdventOfCode;

public class Day22 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private bool test = false;

    public Day22()
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
        int width = _input.Take(_input.Length - 2).Select(x => x.Length).Max();
        int height = _input.Length - 2;
        if (width > height) height = width;
        else width = height;
        int side = width;
        char[,] map = new char[side, side];
        parseBaseMap(side, map, 1);
        List<Cmd> cmds = parseCmds();
        

        // setup initial position
        Pos p = new Pos();
        for (int x = 1; x < width - 1; x++)
        {
            if (map[x, 1] == '.')
            {
                p.X = x;
                p.Y = 1;
                p.Facing = 0;
                break;
            }
        }

        //execute commands
        foreach (Cmd c in cmds)
        {
            //Console.Out.WriteLine("CMD: {0} {1}", c.Val, c.Dir);
            //Console.Out.WriteLine("INITIAL POS: {0} {1} {2}", p.X, p.Y, p.Facing);
                        
            p.Facing = (p.Facing + c.Dir) % 4;
            if (p.Facing == -1) p.Facing = 3;
            //Console.Out.WriteLine("TURNING NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
            for (int m = 0; m < c.Val; m++)
            {
                int nextX = p.X + moves[p.Facing].Item1;
                int nextY = p.Y + moves[p.Facing].Item2;
                char nextC = map[nextX, nextY];
                if (nextC == '#')
                {
                    //Console.Out.WriteLine("WALL NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                    break;
                }
                else if (nextC == '.')
                {
                    p.X = nextX;
                    p.Y = nextY;
                    //Console.Out.WriteLine("OPEN NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                }
                else if (nextC == ' ')
                {
                    while (nextC == ' ')
                    {
                        if (nextX >= width - 1) nextX = 0;
                        else if (nextX <= 0) nextX = width - 1;
                        if (nextY >= height - 1) nextY = 0;
                        else if (nextY <= 0) nextY = height - 1;
                        nextX += moves[p.Facing].Item1;
                        nextY += moves[p.Facing].Item2;                        
                        nextC = map[nextX, nextY];
                        //Console.Out.WriteLine("WRAP NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                    }
                    if (nextC != '#')
                    {
                        p.X = nextX;
                        p.Y = nextY;
                        //Console.Out.WriteLine("WRAP OPEN NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
                    }
                    else
                    {
                        //Console.Out.WriteLine("WRAP WALL POS: {0} {1} {2}", p.X, p.Y, p.Facing);
                        break;
                    }
                }
            }
            Console.Out.WriteLine();
        }

        long password = (1000 * p.Y) + (4 * p.X) + p.Facing;

        _partOne = password.ToString();
    }
    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        int width = _input.Take(_input.Length - 2).Select(x => x.Length).Max();
        int height = _input.Length - 2;
        if (width > height) height = width;
        else width = height;
        int mapSize = width;
        int sideSize = mapSize / 4;
        char[,] map = new char[mapSize, mapSize];

        parseBaseMap(mapSize, map, 0);
        List<Cmd> cmds = parseCmds();
        Dictionary<int, Side> sides = parseMapIntoCube(mapSize, sideSize, map);
        

        // setup initial position
        Pos p = new Pos();
        executeCmds(ref p, cmds, sides, sideSize, map);

        long password = (1000 * (p.Side.SideMap[p.X,p.Y].MapY + 1)) + (4 * (p.Side.SideMap[p.X, p.Y].MapX + 1)) + p.Facing;

        _partTwo = password.ToString();        
    }


    private Dictionary<int, Side> parseMapIntoCube(int mapSize, int sideSize, char[,] map)
    {
        // find sides
        Dictionary<int, Side> sides = new Dictionary<int, Side>();
        int[,] foldGrid = new int[6, 6];

        int i = 0;
        for (int y = 0; y < mapSize; y += sideSize)
        {
            for (int x = 0; x < mapSize; x += sideSize)
            {
                int fx = ((x + 1) / sideSize) + 1;
                int fy = ((y + 1) / sideSize) + 1;
                if (map[x, y] != ' ')
                {
                    i++;
                    sides.Add(i, new Side(i, sideSize, x, y));
                    foldGrid[fx, fy] = i;
                }
                else
                {
                    foldGrid[fx, fy] = 0;
                }
            }
        }
        Debug.Assert(sides.Count == 6);

        // import sides
        foreach (Side side in sides.Values)
        {
            for (int y = 0; y < sideSize; y++)
            {
                for (int x = 0; x < sideSize; x++)
                {
                    side.SideMap[x + 1, y + 1] = new Cell(x + side.OriginMapX, y + side.OriginMapY, map[x + side.OriginMapX, y + side.OriginMapY]);
                }
            }            
        }        

        (int, int, char) up = (0, -1, 'U');
        (int, int, char) down = (0, 1, 'D');
        (int, int, char) left = (-1, 0, 'L');
        (int, int, char) right = (1, 0, 'R');
        List<(int, int, char)> dirs = new List<(int, int, char)>() { up, down, left, right };
        HashSet<(int, int, char)> joins = new HashSet<(int, int, char)>();
        for (int y = 1; y < 5; y++)
        {
            for (int x = 1; x < 5; x++)
            {
                foreach (var dir in dirs)
                {
                    int id = foldGrid[x, y];
                    int idCheck = foldGrid[x + dir.Item1, y + dir.Item2];
                    if (id != 0 && idCheck != 0)
                    {
                        joins.Add((id, idCheck, dir.Item3));
                    }
                }
            }
        }
        Debug.Assert(joins.Count == 10);

        int maxEdge = sideSize + 1;
        Dictionary<char, Edge> edges = new Dictionary<char, Edge>();
        edges.Add('U', new Edge(1, 0, 1, 0));
        edges.Add('u', new Edge(maxEdge - 1, 0, -1, 0));
        edges.Add('D', new Edge(1, maxEdge, 1, 0));
        edges.Add('d', new Edge(maxEdge - 1, maxEdge, -1, 0));
        edges.Add('L', new Edge(0, 1, 0, 1));
        edges.Add('l', new Edge(0, maxEdge - 1, 0, -1));
        edges.Add('R', new Edge(maxEdge, 1, 0, 1));
        edges.Add('r', new Edge(maxEdge, maxEdge - 1, 0, -1));

        Dictionary<char, char> opposites = new Dictionary<char, char>();
        opposites.Add('U', 'D');
        opposites.Add('D', 'U');
        opposites.Add('L', 'R');
        opposites.Add('R', 'L');

        List<char> sourceEdge;
        List<char> destEdge;
        foreach ((int id, int destId, char dir) join in joins)
        {
            sourceEdge = getEdgeChars(sideSize, sides, join.id, join.dir, edges);
            destEdge = getEdgeChars(sideSize, sides, join.destId, opposites[join.dir], edges);
            Debug.Assert(sourceEdge.Count == destEdge.Count);
            for (int j = 0; j < sourceEdge.Count; j++)
            {
                sides[join.id].Wrap.Add(sourceEdge[j], (join.destId, destEdge[j]));
            }
        }

        // now add the non-directly attached edges with correct orientation
        // hardcoded to test and full input
        if (test)
        {
            // 1L       3U      
            attachEdges((1, 'L'), (3, 'U'), sideSize, sides, edges);
            // 1R       6R -    
            attachEdges((1, 'R'), (6, 'r'), sideSize, sides, edges);
            // 1U       2U -    
            attachEdges((1, 'U'), (2, 'u'), sideSize, sides, edges);
            // 5L       3D -
            attachEdges((5, 'L'), (3, 'd'), sideSize, sides, edges);
            // 5D       2D -    
            attachEdges((5, 'D'), (2, 'd'), sideSize, sides, edges);
            // 6U       4R -           
            attachEdges((6, 'U'), (4, 'r'), sideSize, sides, edges);
            // 6D       2L -         
            attachEdges((6, 'D'), (2, 'l'), sideSize, sides, edges);            
        }
        else // full input
        {
            //2D 3R
            attachEdges((2, 'D'), (3, 'R'), sideSize, sides, edges);
            //2R 5r
            attachEdges((2, 'R'), (5, 'r'), sideSize, sides, edges);
            //3L 4U
            attachEdges((3, 'L'), (4, 'U'), sideSize, sides, edges);
            //4L 1l
            attachEdges((4, 'L'), (1, 'l'), sideSize, sides, edges);
            //6L 1U
            attachEdges((6, 'L'), (1, 'U'), sideSize, sides, edges);
            //6D 2U
            attachEdges((6, 'D'), (2, 'U'), sideSize, sides, edges);
            //6R 5D
            attachEdges((6, 'R'), (5, 'D'), sideSize, sides, edges);
        }

        return sides;
    }

    private void executeCmds(ref Pos p, List<Cmd> cmds, Dictionary<int, Side> sides, int sideSize, char[,] map)
    {
        for (int x = 1; x < sideSize + 1; x++)
        {
            if (sides[1].SideMap[x, 1].Val == '.')
            {
                p.X = x;
                p.Y = 1;
                p.Facing = 0;
                p.Side = sides[1];
                break;
            }
        }

        Dictionary<char, (int x, int y, int facing)> wrapEntries;
        wrapEntries = new Dictionary<char, (int x, int y, int facing)>();
        for (int y = 0; y < sideSize + 2; y++)
        {
            for (int x = 0; x < sideSize + 2; x++)
            {
                if (y == 0) wrapEntries.Add(sides[1].SideMap[x, y].Val, (x, y + 1, 1));
                else if (y == sideSize + 2 - 1) wrapEntries.Add(sides[1].SideMap[x, y].Val, (x, y - 1, 3));
                else if (x == 0) wrapEntries.Add(sides[1].SideMap[x, y].Val, (x + 1, y, 0));
                else if (x == sideSize + 2 - 1) wrapEntries.Add(sides[1].SideMap[x, y].Val, (x - 1, y, 2));
            }
        }

        //execute commands
        foreach (Cmd c in cmds)
        {
#if DEBUG
            Console.Out.WriteLine("CMD: {0} {1}", c.Val, c.Dir);
            Console.Out.WriteLine("INITIAL POS: x:{0} y:{1} f:{2} s:{3}", p.X, p.Y, p.Facing, p.Side.Id);
#endif 

            p.Facing = (p.Facing + c.Dir) % 4;
            if (p.Facing == -1) p.Facing = 3;
#if DEBUG
            Console.Out.WriteLine("TURNING NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
#endif
            for (int m = 0; m < c.Val; m++)
            {
                int nextX = p.X + moves[p.Facing].Item1;
                int nextY = p.Y + moves[p.Facing].Item2;
                char nextC = p.Side.SideMap[nextX, nextY].Val;
                if (nextC == '#')
                {
#if DEBUG
                    Console.Out.WriteLine("WALL NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
#endif
                    break;
                }
                else if (nextC == '.')
                {
                    p.X = nextX;
                    p.Y = nextY;
#if DEBUG
                    Console.Out.WriteLine("OPEN NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
#endif
                }
                else // wrap
                {
                    char destChar = p.Side.Wrap[nextC].Item2;
                    Side destSide = sides[p.Side.Wrap[nextC].destId];
                    nextX = wrapEntries[destChar].x;
                    nextY = wrapEntries[destChar].y;                    
                    nextC = destSide.SideMap[nextX, nextY].Val; 
                    if (nextC != '#')
                    {
                        p.X = nextX;
                        p.Y = nextY;
                        p.Facing = wrapEntries[destChar].facing;
                        p.Side = destSide;
#if DEBUG
                        Console.Out.WriteLine("WRAP OPEN NEWPOS: {0} {1} {2}", p.X, p.Y, p.Facing);
#endif
                    }
                    else
                    {
#if DEBUG
                        Console.Out.WriteLine("WRAP WALL POS: {0} {1} {2}", p.X, p.Y, p.Facing);
#endif
                        break;
                    }
                }
            }
            Console.Out.WriteLine();
        }
    }

    private static void attachEdges((int id,char dir) source, (int id ,char dir) dest, int sideSize, Dictionary<int, Side> sides, Dictionary<char, Edge> edges)
    {
        List<char> sourceEdge = getEdgeChars(sideSize, sides, source.id, source.dir, edges);
        List<char> destEdge = getEdgeChars(sideSize, sides, dest.id, dest.dir, edges);
        for (int j = 0; j < sourceEdge.Count; j++)
        {
            sides[source.id].Wrap.Add(sourceEdge[j], (dest.id, destEdge[j]));
            sides[dest.id].Wrap.Add(destEdge[j], (source.id, sourceEdge[j]));
        }
    }

    private static List<char> getEdgeChars(int sideSize, Dictionary<int, Side> sides, int id, char dir, Dictionary<char, Edge> edges)
    {
        List<char> edgeChars = new List<char>();        
        int cY = edges[dir].sY;
        int cX = edges[dir].sX;        
        for (int e = 0; e < sideSize; e++)
        {
            edgeChars.Add(sides[id].SideMap[cX, cY].Val);
            cX += edges[dir].vX;
            cY += edges[dir].vY;
        }
                        
        return edgeChars;
    }

    private struct Edge
    {
        public int sX, sY;
        public int vX, vY;

        public Edge(int sX, int sY, int vX, int vY)
        {
            this.sX = sX;
            this.sY = sY;
            this.vX = vX;
            this.vY = vY;
        }
    }

    private class Side
    {
        public Cell[,] SideMap;
        public Dictionary<char, (int destId, char)> Wrap;
        public int Id;
        public int OriginMapX;
        public int OriginMapY;       

        public Side(int id, int sideSize, int mapX, int mapY)
        {
            this.Id = id;
            this.OriginMapX = mapX; 
            this.OriginMapY = mapY;

            this.SideMap = new Cell[sideSize + 2, sideSize + 2];
            this.Wrap = new Dictionary<char, (int, char)>();

            byte unique = 48;
            for (int y = 0; y < sideSize + 2; y++)
            {
                for (int x = 0; x < sideSize + 2; x++)
                {
                    if ((y == 0 || y == sideSize + 2 - 1) || (x == 0 || x == sideSize + 2 - 1))
                    {                        
                        SideMap[x, y] = new Cell((char)unique++);
                    }
                }
            }
        }
    }

    private class Cell
    {
        public int MapX;
        public int MapY;
        public char Val;

        public Cell(char val)
        {
            Val = val;
        }
        public Cell(int mapX, int mapY, char val)
        {
            MapX = mapX;
            MapY = mapY;
            Val = val;
        }
    }    

    private struct Cmd
    {
        public int Val;
        public int Dir;

        public Cmd(int val, int dir)
        {
            this.Val = val;
            this.Dir = dir;
        }
    }

    private Dictionary<int, (int, int)> moves = new Dictionary<int, (int, int)>() {
        { 3, (0, -1) },
        { 0, (1, 0) },
        { 1, (0, 1) },
        { 2, (-1, 0) }
    };

    private struct Pos
    {
        public int X;
        public int Y;
        public int Facing;
        public Side Side;
    }

    private void parseBaseMap(int side, char[,] map, int padding)
    {
        for (int y = 0; y < side; y++)
        {
            for (int x = 0; x < side; x++)
            {
                map[x, y] = ' ';
            }
        }

        string line;
        for (int y = 0 + padding; y < side - padding; y++)
        {
            if (y - padding < _input.Length - 2)
            {
                line = _input[y - padding];
                for (int x = padding; x < side - padding; x++)
                {
                    if (x - padding < line.Length) map[x, y] = line[x - padding];
                }
            }
        }
      
    }


    private List<Cmd> parseCmds()
    {
        List<Cmd> cmds = new List<Cmd>();
        String line = _input[^1];
        string val = String.Empty;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] != 'R' && line[i] != 'L') val += line[i];
            else
            {
                if (val != String.Empty)
                {
                    cmds.Add(new Cmd(int.Parse(val), 0));
                    val = String.Empty;
                }
                cmds.Add(new Cmd(0, line[i] == 'R' ? 1 : -1));
            }
        }
        if (val != String.Empty) cmds.Add(new Cmd(int.Parse(val), 0));
        return cmds;
    }
}
