using System.Diagnostics;
using System.Drawing.Printing;
using System.Net.Http.Headers;
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
        int idx = 0;
        int sum = 0;
        for (int i = 0; i < _input.Length; i+=3)
        {
            idx++;
            Node first = parseLine(_input[i]);            
            Node second = parseLine(_input[i+1]);
            if (first.CompareTo(second) < 0) sum += idx;                        
        }               
        _partOne = sum.ToString();
    }


    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        List<Node> allNodes = new List<Node>();
        Node twoDiv = parseLine("[[2]]");
        Node sixDiv = parseLine("[[6]]");                

        for (int i = 0; i < _input.Length; i++)
        {
            if (_input[i].Length > 0)
            {
                allNodes.Add(parseLine(_input[i]));                
            }
        }

        allNodes.Add(twoDiv);
        allNodes.Add(sixDiv);
        allNodes.Sort();

        int key = (allNodes.IndexOf(twoDiv) + 1) * (allNodes.IndexOf(sixDiv) + 1);
        _partTwo = key.ToString();
    }

    private Node parseLine(string line)
    {
        Node? cur = null;
        StringBuilder v = new StringBuilder();
        foreach(char c in line)
        {            
            switch (c)
            {
                case '[':
                    Node n = new Node();
                    if (cur != null)
                    {
                        n.parent = cur;
                        cur.children.Add(n);
                    }
                    cur = n;
                    break;
                case ']':
                    addNodeValue(cur, v);
                    if (cur.parent != null) cur = cur.parent;
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
        return cur;
    }

    private static void addNodeValue(Node cur, StringBuilder v)
    {
        if (v.Length > 0)
        {
            Node n = new Node(int.Parse(v.ToString()));
            n.parent = cur;            
            cur.children.Add(n);
            v.Clear();
        }
    }   

    private class Node : IComparable<Node>
    {
        public int val;
        public List<Node>? children;
        public Node parent;
        public bool IsVal => children == null;
        public override string ToString()
        {
            if (IsVal) return val.ToString();
            return '[' + string.Join(',', children) + ']';
        }

        public Node()
        {
            children = new List<Node>();
        }

        public Node(int v)
        {
            val = v;
        }

        public int CompareTo(Node? right)
        {
            if (IsVal && right.IsVal) return val.CompareTo(right.val);
            if (!IsVal && !right.IsVal)
            {
                int idx = 0;
                while (idx < children.Count || idx < right.children.Count)
                {
                    if (idx >= children.Count) return -1;
                    if (idx >= right.children.Count) return 1;
                    int c = children[idx].CompareTo(right.children[idx]);
                    if (c != 0) return c;
                    idx++;
                }
                return 0;
            }
            if (!right.IsVal) return -right.CompareTo(this);
            if (!IsVal)
            {
                if (children.Count == 0) return -1;
                int c = children[0].CompareTo(right);
                if (c != 0) return c;
                if (children.Count > 1) return 1;
            }
            return 0;
        }
    }
}
