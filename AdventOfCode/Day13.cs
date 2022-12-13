using System.Runtime.ExceptionServices;
using System.Text;

namespace AdventOfCode;

public class Day13 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day13()
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
        List<Pair> pairs = new List<Pair>();
        for (int i = 0; i < _input.Length; i+=3)
        {
            Pair p = new Pair();
            p.first = parseLine(_input[i]);
            p.second = parseLine(_input[i+1]);
            pairs.Add(p);
        }

        //foreach(Pair p in pairs)
        //{
        //    p.Compare();
        //    Console.Out.WriteLine(p.rightOrder);
        //}

        pairs.First().Compare();
        Console.Out.WriteLine(pairs.First().rightOrder);

        pairs.Skip(1).First().Compare();
        Console.Out.WriteLine(pairs.Skip(1).First().rightOrder);

        _partOne = "Not Solved";
    }

    private Node parseLine(string line)
    {
        Node b = new Node();
        Node cur = b;
        StringBuilder v = new StringBuilder();
        foreach(char c in line)
        {            
            switch (c)
            {
                case '[':
                    Node n = new Node();
                    n.parent = cur;
                    cur.children.Add(n);
                    cur = n;
                    break;
                case ']':
                    addNodeValue(cur, v);
                    cur = cur.parent;
                    break;
                case ',':
                    addNodeValue(cur, v);
                    break;
                default:
                    // number, but could be two digits
                    v.Append(c);                    
                    break;
            }
        }
        return b;
    }

    private static void addNodeValue(Node cur, StringBuilder v)
    {
        if (v.Length > 0)
        {
            Node n = new Node();
            n.parent = cur;
            n.val = int.Parse(v.ToString());
            cur.children.Add(n);            
            v.Clear();
        }
    }

    private class Pair
    {
        public Node first;
        public Node second;
        public bool rightOrder;        

        internal void Compare()
        {
            Node left = first;
            Node right = second;

            rightOrder = left.Compare(right, 0);
        }
    }

    private class Node
    {
        public int? val;
        public List<Node> children = new List<Node>();
        public Node parent;

        internal bool Compare(Node right, int i)
        {
            bool rightOrder = false;

            // both values are integers
            if (this.val != null && right.val != null)
            {
                if (this.val < right.val) 
                    return true;
                if (this.val > right.val) 
                    return false;
                if (this.val == right.val)
                {
                    i++;
                    rightOrder = this.parent.Compare(right.parent, i);
                }
            }
            // both are lists
            else if (this.children.Any() && right.children.Any())
            {
                // both have enough children to compare
                if (this.children.Count() > i && right.children.Count() > i)
                {
                    rightOrder = this.children[i].Compare(right.children[i], i);
                }
                // left runs out of items first
                else if (this.children.Count() < right.children.Count())
                {
                    return true;
                }
                // right runs out of items first
                else
                {
                    return false;
                }                
            }
            // left is value, right is list
            else if (this.val != null && right.children.Any())
            {
                rightOrder = this.Compare(right.children.First(), i);
            }
            // right is list, left is value
            else if (this.children.Any() && right.val != null)
            {
                rightOrder = this.children.First().Compare(right, i);
            }

            return rightOrder;
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
