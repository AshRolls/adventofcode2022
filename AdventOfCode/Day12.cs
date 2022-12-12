namespace AdventOfCode;

using AdventOfCode.vis;
using Priority_Queue;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.ConstrainedExecution;


public class Day12 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private Day12Vis _visualiser = new Day12Vis();
    private static readonly int WIDTH = 64;
    private static readonly int HEIGHT = 40;

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
        Byte[,] heightGrid = new Byte[WIDTH, HEIGHT];
        Pos start = new Pos();
        Pos end = new Pos();
        parseInput(heightGrid, ref start, ref end);
      
        //int min = aStar(heightGrid, start, end);
        int min = dijkstra(heightGrid, start, end);

        _partOne = min.ToString();
    }

    private int dijkstra(Byte[,] heights, Pos start, Pos end)
    {
        PriorityQueue<Node, int> openQueue = new PriorityQueue<Node, int>();
        HashSet<Node> visitedNodes = new HashSet<Node>();
        Node[,] grid = new Node[WIDTH, HEIGHT];
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                grid[x, y] = new Node(new Pos(x, y));
            }
        }

        Node startNode = grid[start.X, start.Y];
        startNode.g = 0;        
        openQueue.Enqueue(startNode, 0);
        visitedNodes.Add(startNode);

        Node cur = null;
        List<Node> successors;
        while (openQueue.Count > 0)
        {
            _visualiser.AddRenderItem(new Day12Vis.RenderItem(2, 0, 0, 0, openQueue.Count.ToString()));
            cur = openQueue.Dequeue();            

            if (cur.position.Equals(end))
                break;

            successors = new List<Node>();
            addSuccessors(cur, successors, grid, heights);

            foreach (Node successor in successors)
            {
                if (!visitedNodes.Contains(successor))
                {
                    successor.g = cur.g + 1;
                    successor.parent = cur;                    
                    openQueue.Enqueue(successor, successor.g);
                    visitedNodes.Add(successor);
                    //_visualiser.AddRenderItem(new Day12Vis.RenderItem(0, cur.position.X, cur.position.Y, 0, String.Empty));
                }
                else if (successor.g > cur.g)
                {
                    successor.g = cur.g + 1;
                    successor.parent = cur;
                    //_visualiser.AddRenderItem(new Day12Vis.RenderItem(0, cur.position.X, cur.position.Y, 0, String.Empty));
                }
            }
        }

        Stack<Node> path = new Stack<Node>();
        while (cur != null)
        {
            path.Push(cur);            
            cur = cur.parent;
        }

        int steps = path.Count() - 1;

        // visualise
        foreach (Node n in path)
        {
            _visualiser.AddRenderItem(new Day12Vis.RenderItem(0, n.position.X, n.position.Y, 0, String.Empty));
        }

        return steps;
    }

    //private int aStar(Byte[,] heights, Pos start, Pos end)
    //{
    //    FastPriorityQueue<Node> openQueue = new FastPriorityQueue<Node>(64 * 40 * 10);
    //    Node[,] grid = new Node[64, 40];
    //    for (int x = 0; x < 64; x++)
    //    {
    //        for (int y = 0; y < 40; y++)
    //        {
    //            grid[x, y] = new Node(new Pos(x, y));
    //        }
    //    }
        
    //    Node startNode = grid[start.X, start.Y];        
    //    startNode.g = 0;
    //    startNode.h = Math.Abs(start.X - end.X) + Math.Abs(start.X - end.Y);
    //    startNode.f = startNode.g + startNode.h;
    //    openQueue.Enqueue(startNode, 0);

    //    Node cur = null;

    //    while (openQueue.Count > 0)
    //    {
    //        cur = openQueue.Dequeue();
    //        cur.closed = true;

    //        if (cur.position.Equals(end)) 
    //            break;

    //        List<Node> successors = new List<Node>();
    //        addSuccessors(cur, successors, grid, heights);

    //        foreach (Node successor in successors)
    //        {
    //            if (successor.Closed) continue;
    //            if (!openQueue.Contains(successor))
    //            {
    //                successor.g = cur.g + 1;
    //                successor.h = Math.Abs(successor.position.X - end.X) + Math.Abs(successor.position.Y - end.Y);
    //                successor.f = successor.g + successor.h;
    //                successor.parent = cur;
    //                openQueue.Enqueue(successor, successor.f);
    //            }
    //            else if (cur.g + successor.h < successor.f)
    //            {
    //                successor.g = cur.g;
    //                successor.f = successor.g + successor.h;
    //                successor.parent = cur;
    //            }
                
    //        }               
    //    }

    //    Stack<Node> path = new Stack<Node>();
    //    while (cur != null)
    //    {
    //        path.Push(cur);
    //        //_visualiser.AddRenderItem(new Day12Vis.RenderItem(0, cur.position.X, cur.position.Y));
    //        cur = cur.parent;           
    //    }

    //    return path.Count();
    //}

    private static void addSuccessors(Node cur, List<Node> successors, Node[,] grid, Byte[,] heights)
    {
        if (cur.position.Y > 0)
        {
            Pos upPos = new Pos(cur.position.X, cur.position.Y - 1);
            if (heights[upPos.X, upPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node up = grid[upPos.X, upPos.Y];                
                successors.Add(up);
            }
        }
        if (cur.position.Y < HEIGHT - 1)
        {
            Pos downPos = new Pos(cur.position.X, cur.position.Y + 1);
            if (heights[downPos.X, downPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node down = grid[downPos.X, downPos.Y];
                successors.Add(down);
            }
        }
        if (cur.position.X > 0)
        {
            Pos leftPos = new Pos(cur.position.X - 1, cur.position.Y);
            if (heights[leftPos.X, leftPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node left = grid[leftPos.X, leftPos.Y];
                successors.Add(left);
            }
        }
        if (cur.position.X < WIDTH - 1)
        {
            Pos rightPos = new Pos(cur.position.X + 1, cur.position.Y);
            if (heights[rightPos.X, rightPos.Y] - heights[cur.position.X, cur.position.Y] < 2)
            {
                Node right = grid[rightPos.X, rightPos.Y];
                successors.Add(right);
            }
        }
    }

    private class Node
    {
        public Node(Pos pos)
        {
            this.position = pos;
        }

        public Pos position { get; private set; }
        //public int f;
        public int g;
        //public int h; // manhattan
        //public bool closed;
        public Node parent;
    }

    private enum NodeState { Untested, Open, Closed }

    private void parseInput(byte[,] grid, ref Pos start, ref Pos end)
    {
        const byte asciiReduce = 97;

        for (int y = 0; y < HEIGHT; y++)
        {
            string line = _input[y];
            for (int x = 0; x < WIDTH; x++)
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
                _visualiser.AddRenderItem(new Day12Vis.RenderItem(1, x, y, grid[x, y], String.Empty));
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
