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
        Dictionary<int, Node> nodeCircular;
        Node zero;
        int nodeSize = _input.Length;
        parseFile(out nodeCircular, out zero, 1);

        //mix
        mix(nodeCircular, nodeSize);

        _partOne = findThousandths(zero).ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        Dictionary<int, Node> nodeCircular;
        Node zero;
        int nodeSize = _input.Length;
        parseFile(out nodeCircular, out zero, 811589153);

        //mix
        int numberOfMix = 10;
        for (int i = 0; i < numberOfMix; i++)
        {
            mix(nodeCircular, nodeSize);
        }

        _partTwo = findThousandths(zero).ToString();
    }

    private static void mix(Dictionary<int, Node> nodeCircular, long nodeSize)
    {
        Node cur;
        Node n;
        for (int i = 0; i< nodeSize; i++)
        {
            n = nodeCircular[i];
            long v = 0;
            
            // splice where we are taking from from back together
            Node prev = n.Prev;
            Node next = n.Next;                
            prev.Next = next;
            next.Prev = prev;

            cur = n;
            if (n.Val > 0)
            {
                while (v < n.Val % (nodeSize - 1))
                {
                    v++;
                    cur = cur.Next;
                }

                // insert
                next = cur.Next;
                prev = cur;
            }
            else if (n.Val < 0)
            {                
                while (v > n.Val % (nodeSize - 1))
                {
                    v--;
                    cur = cur.Prev;
                }

                // insert
                prev = cur.Prev;
                next = cur;
            }          
            
            n.Prev = prev;
            n.Next = next;
            prev.Next = n;
            next.Prev = n;                       
        }
    }
  
    private long findThousandths(Node zero)
    {
        Node cur = zero;
        long oneK = 0;
        long twoK = 0;
        long threeK = 0;
        for (int i = 1; i <= 3000; i++)
        {
            cur = cur.Next;
            if (i == 1000) oneK = cur.Val;
            else if (i == 2000) twoK = cur.Val;
            else if (i == 3000) threeK = cur.Val;
        }
        Console.Out.WriteLine(oneK + " " +  twoK + " " + threeK);
        return oneK + twoK + threeK;
    }

    private void parseFile(out Dictionary<int, Node> nodeCircular, out Node zero, long key)
    {
        nodeCircular = new Dictionary<int, Node>();
        Node first = new Node();
        first.Val = int.Parse(_input[0]) * key;
        nodeCircular.Add(0, first);

        zero = null;
        Node last = first;
        for (int i = 1; i < _input.Length; i++)
        {
            Node newN = new Node();
            newN.Val = long.Parse(_input[i]) * key;
            if (newN.Val == 0) zero = newN;
            newN.Prev = last;
            nodeCircular.Add(i, newN);
            last.Next = newN;
            last = newN;
        }
        last.Next = first;
        first.Prev = last;

        // add index from zero
        //Node cur = zero;
        //for (int i = 0; i < _input.Length; i++)
        //{
        //    cur.Idx = i;
        //    cur = cur.Next;
        //}
    }

    private class Node
    {
        public Node Next;
        public Node Prev;
        public long Val;
        public int Idx;
    }
}
