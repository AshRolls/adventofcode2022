using System.Diagnostics;
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

        int idx = 0;
        int sum = 0;
        foreach (Pair p in pairs)
        {
            idx++;
            p.Compare();
            if (p.correctOrder) sum += idx;
            //Console.Out.WriteLine(p.correctOrder);
        }

        _partOne = sum.ToString();
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

    private enum CompareState { pass, fail, next };

    private class Pair
    {
        public Node first;
        public Node second;
        public bool correctOrder;
        

        internal void Compare()
        {
            Node left = first;
            Node right = second;

            CompareState state = CompareState.next;
            while (state == CompareState.next)
            {
                state = Compare(left.Next(), right.Next());
            }
            if (state == CompareState.pass) correctOrder = true;
            else correctOrder = false;
        }

        private CompareState Compare(Node? left, Node? right)
        {
            if (left == null && right == null) return CompareState.next;
            if (left == null) return CompareState.pass;
            if (right == null) return CompareState.fail;

            // both val
            if (left.IsVal && right.IsVal)
            {
                if (left.val < right.val) return CompareState.pass;
                if (left.val > right.val) return CompareState.fail;
                if (left.val == right.val) return CompareState.next;
            }
            // both list
            else if (!left.IsVal && !right.IsVal)
            {
                CompareState state = CompareState.next;
                while (state == CompareState.next && ((!left.Finished || !right.Finished) && !(left.Finished && right.Finished)))
                {
                    state = Compare(left.Next(), right.Next());
                }
                return state;
            }
            // left list, right val
            else if (!left.IsVal && right.IsVal)
            {
                CompareState state = CompareState.next;
                while (state == CompareState.next)
                {
                    state = Compare(left.Next(), right);
                }
                return state;
            }
            // right list, left val
            else if (!right.IsVal && left.IsVal)
            {
                CompareState state = CompareState.next;
                while (state == CompareState.next)
                {
                    state = Compare(left, right.Next());
                }
                return state;
            }

            return CompareState.next;
        }

        
    }

    private class Node
    {
        public int? val;
        public List<Node> children = new List<Node>();
        public Node parent;
        public bool IsVal => val != null;
        public bool Finished => idx >= children.Count();
        private int idx = 0;

        public Node? Next()
        {            
            Debug.Assert (val == null);
            if (idx <= children.Count() - 1) return children[idx++];
            return null;            
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
