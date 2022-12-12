namespace AdventOfCode;

using AdventOfCode.vis;
using Priority_Queue;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;


public class Day12 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private Day12Vis _visualiser = new Day12Vis();

    public Day12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        _visualiser.StartVisualiser(solve1);
        return new(_partOne);
    }

    public struct Pos
    {
        public Pos (int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X; 
        public int Y;
    }

    private void solve1()
    {
        Byte[,] heightGrid = new Byte[64, 40];
        Pos start = new Pos();
        Pos end = new Pos();
        parseInput(heightGrid, ref start, ref end);
      
        int min = aStar(heightGrid, start, end);

        _partOne = min.ToString();
    }

    private int aStar(Byte[,] heights, Pos start, Pos end)
    {
        FastPriorityQueue<Node> openQueue = new FastPriorityQueue<Node>(64 * 40 * 10);
        Node[,] grid = new Node[64, 40];
        for (int x = 0; x < 64; x++)
        {
            for (int y = 0; y < 40; y++)
            {
                grid[x, y] = new Node(new Pos(x, y));
            }
        }

        Node endNode = grid[end.X, end.Y];
        Node startNode = grid[start.X, start.Y];        
        startNode.g = 0;
        startNode.h = Math.Abs(start.X - end.X) + Math.Abs(start.X - end.Y);
        startNode.f = startNode.g + startNode.h;
        openQueue.Enqueue(startNode, 0);

        Node cur = null;

        while (openQueue.Count > 0)
        {
            cur = openQueue.Dequeue();
            cur.closed = true;

            if (cur.position.Equals(end)) 
                break;

            List<Node> successors = new List<Node>();
            addSuccessors(cur, successors, grid, heights);

            foreach (Node successor in successors)
            {                
                if (!openQueue.Contains(successor))
                {
                    successor.g = cur.g + 1;
                    successor.h = Math.Abs(successor.position.X - end.X) + Math.Abs(successor.position.Y - end.Y);
                    successor.f = successor.g + successor.h;
                    successor.parent = cur;
                    openQueue.Enqueue(successor, successor.f);
                }
                else if (cur.g + successor.h < successor.f)
                {
                    successor.g = cur.g;
                    successor.f = successor.g + successor.h;
                    successor.parent = cur;
                }
                
            }               
        }

        Stack<Node> path = new Stack<Node>();
        while (cur != null)
        {
            path.Push(cur);
            _visualiser.AddRenderItem(new Day12Vis.RenderItem(0, cur.position.X, cur.position.Y));
            cur = cur.parent;           
        }

        return path.Count();
    }

    private static void addSuccessors(Node cur, List<Node> successors, Node[,] grid, Byte[,] heights)
    {
        if (cur.position.Y > 0)
        {
            Pos upPos = new Pos(cur.position.X, cur.position.Y - 1);
            if (heights[upPos.X, upPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node up = grid[upPos.X, upPos.Y];
                if (!up.closed) successors.Add(up);
            }
        }
        if (cur.position.Y < 39)
        {
            Pos downPos = new Pos(cur.position.X, cur.position.Y + 1);
            if (heights[downPos.X, downPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node down = grid[downPos.X, downPos.Y];
                if (!down.closed) successors.Add(down);
            }
        }
        if (cur.position.X > 0)
        {
            Pos leftPos = new Pos(cur.position.X - 1, cur.position.Y);
            if (heights[leftPos.X, leftPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node left = grid[leftPos.X, leftPos.Y];
                if (!left.closed) successors.Add(left);
            }
        }
        if (cur.position.X < 63)
        {
            Pos rightPos = new Pos(cur.position.X + 1, cur.position.Y);
            if (heights[rightPos.X, rightPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node right = grid[rightPos.X, rightPos.Y];
                if (!right.closed) successors.Add(right);
            }
        }
    }

    private class Node : FastPriorityQueueNode
    {
        public Node(Pos pos)
        {
            this.position = pos;
        }

        public Pos position { get; private set; }
        public int f;
        public int g;
        public int h; // manhattan
        public bool closed;
        public Node parent;
    }

    private enum NodeState { Untested, Open, Closed }

    private void parseInput(byte[,] grid, ref Pos start, ref Pos end)
    {
        const byte asciiReduce = 97;

        for (int y = 0; y < 40; y++)
        {
            string line = _input[y];
            for (int x = 0; x < 64; x++)
            {
                Char c = line[x];
                if (c == 'S')
                {
                    start = new(x, y);
                    c = 'a';
                }
                else if (c == 'E')
                {
                    end = new(x, y);
                    c = 'z';
                }
                grid[x, y] = (byte)((byte)c - asciiReduce);
                _visualiser.AddRenderItem(new Day12Vis.RenderItem(1, x, y, grid[x, y]));
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
