namespace AdventOfCode;

public class Day20 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day20()
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
        Dictionary<int,Node> nodeCircular = new Dictionary<int,Node>();
        Node first = new Node();
        first.Val = int.Parse(_input[0]);
        nodeCircular.Add(0, first);

        Node? zero = null;
        Node last = first;
        for (int i = 1; i < _input.Length; i++)
        {
            Node newN = new Node();
            newN.Val = int.Parse(_input[i]);
            if (newN.Val == 0) zero = newN;
            newN.Prev = last;
            nodeCircular.Add(i, newN);
            last.Next = newN;
            last = newN;
        }
        last.Next = first;
        first.Prev = last;

        //mix
        Node cur;
        foreach (Node n in nodeCircular.Values)
        {
            int i = 0;
            if (n.Val > 0) 
            {
                Node prev = n.Prev;
                Node next = n.Next;
                prev.Next = next;
                next.Prev = prev;
                cur = n;
                while (i < n.Val)
                {
                    i++;
                    cur = cur.Next;
                }
                next = cur.Next;
                prev = cur;
                n.Prev = prev;
                n.Next = next;
                prev.Next = n;
                next.Prev = n;
            }
            if (n.Val < 0)
            {
                Node prev = n.Prev;
                Node next = n.Next;
                prev.Next = next;
                next.Prev = prev;
                cur = n;
                while (i > n.Val)
                {
                    i--;
                    cur = cur.Prev;
                }
                prev = cur.Prev;
                next = cur;
                n.Prev = prev;
                n.Next = next;
                prev.Next = n;
                next.Prev = n;
            }
        }

        cur = zero;
        int oneK = 0;
        int twoK = 0;
        int threeK = 0;
        for (int i = 1; i<=3000; i++)
        {
            cur = cur.Next;
            if (i == 1000) oneK = cur.Val;
            else if (i == 2000) twoK = cur.Val;
            else if (i == 3000) threeK = cur.Val;            
        }

        _partOne = (oneK + twoK + threeK).ToString();
    }

    private class Node
    {
        public Node Next;
        public Node Prev;
        public int Val;
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
