using System.Collections.Specialized;
using System.Diagnostics;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode;

public class Day17 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private static bool _visualise = false;

    public Day17()
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
        const long rocksToAdd = 2022;
        const int width = 9;        
        BitRock[] rocks = setupBitRocks();

        BitGrid bg = new BitGrid(width);
        
        Run(rocksToAdd, rocks, bg);

        _partOne = bg.Highest.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        const long rocksToAdd = 1000000000000;
        const int width = 9;
        BitRock[] rocks = setupBitRocks();

        BitGrid bg = new BitGrid(width);

        Run(rocksToAdd, rocks, bg, true);

        _partTwo = bg.Highest.ToString();
    }

    private struct GridState
    {
        public long R1;
        public long R2;
        public long R3;            
        public long R4;
        public long R5;
        public long R6;
        public long R7;
        public int rock;
        public int jet;
    }

    private void Run(long rocksToAdd, BitRock[] rocks, BitGrid g, bool drop3= false)
    {
        BitRock curRock;
        long rocksAdded = 0;
        int jet = 0;
        int rock = 0;
        bool addNew = true;
        Dictionary<GridState, (long,long)> states = new Dictionary<GridState, (long,long)>();
#if DEBUG
        int dropped = 0;
        int highestDropped = 0;
#endif
#if TIMED
        Stopwatch sw = Stopwatch.StartNew();
#endif
        while (rocksAdded <= rocksToAdd)
        {
            if (addNew)
            {
                GridState state = new GridState();
                state.rock = rock;
                state.jet = jet;
                state.R1 = g.Rows[1];
                state.R2 = g.Rows[2];
                state.R3 = g.Rows[3];
                state.R4 = g.Rows[4];
                state.R5 = g.Rows[5];
                state.R6 = g.Rows[6];
                state.R7 = g.Rows[7];
                if (!states.TryAdd(state, (g.Highest, rocksAdded)))
                {
                    long hgtAdded = g.Highest - states[state].Item1;
                    long rckAdded = rocksAdded - states[state].Item2;
                    while (rocksAdded + rckAdded < rocksToAdd)
                    {
                        rocksAdded += rckAdded;
                        g.Highest += hgtAdded;
                    }
                }

#if DEBUG                               
               if (dropped > highestDropped) highestDropped = dropped;
               dropped = 0;
#endif
                
                curRock = rocks[rock++];
                if (rock == 5) rock = 0;
                rocksAdded++;                

#if TIMED
                if (rocksAdded % 5000000 == 0)
                {                                    
                    long ms = sw.ElapsedMilliseconds;
                    long minRem = ((rocksToAdd / 5000000) * ms) / 1000 / 60 / 60;                    
                    Console.Out.WriteLine("{3}%, Height: {4}, Rocks Added: {0}, Hashes: {5}, Time: {1}, Hours Rem: {2}", rocksAdded, ms, minRem, ((float)rocksAdded / (float)rocksToAdd) * (float)100, g.Highest, hashes.Count);
                    sw.Restart();                    
                }
#endif
                g.AddNewRock(curRock);
#if DEBUG
                g.PrintTower("Added Rock");
#endif
            }

            pushRock(g, jet++);
#if DEBUG
            g.PrintTower("Pushed Rock" + _input[0][jet-1]);
#endif
            if (jet > _input[0].Length - 1) jet = 0;

            addNew = !g.DropRock();
#if DEBUG
            dropped++;
#endif
        }
#if TIMED
        sw.Stop();
#endif
#if DEBUG
        Console.Out.WriteLine(highestDropped.ToString());
#endif
    }

    private BitRock[] setupBitRocks()
    {
        BitRock[] rocks = new BitRock[5];

        //byte[,] shape1 = {  { 0, 1},
        //                    { 0, 1},
        //                    { 0, 1},
        //                    { 0, 1}                           
        //};
        //byte[,] shape2 = {  { 0, 0, 1, 0},
        //                    { 0, 1, 1, 1},
        //                    { 0, 0, 1, 0}                                                        
        //};
        //byte[,] shape3 = {  { 0, 1, 0, 0},
        //                    { 0, 1, 0, 0},
        //                    { 0, 1, 1, 1}                           
        //};
        //byte[,] shape4 = {  { 0, 1, 1, 1, 1 }                            
        //};
        //byte[,] shape5 = {  { 0, 1, 1},
        //                    { 0, 1, 1}                           
        //};

        BitRock rock1 = new BitRock(8,  8,  8,  8, 4, 1);
        BitRock rock2 = new BitRock(4,  14, 4,  0, 3, 3);
        BitRock rock3 = new BitRock(8,  8,  14, 0, 3, 3);
        BitRock rock4 = new BitRock(15, 0,  0,  0, 1, 4);
        BitRock rock5 = new BitRock(12, 12, 0,  0, 2, 2);

        rocks[0] = rock1;
        rocks[1] = rock2;
        rocks[2] = rock3;
        rocks[3] = rock4;
        rocks[4] = rock5;

        return rocks;
    }

    private void pushRock(BitGrid g, int jetIdx)
    {
        switch (_input[0][jetIdx])
        {
            case '>':
                g.PushRock(1);
                break;
            default:
                g.PushRock(-1);
                break;
        }
    }

    private class BitGrid
    {
        public long[] Rows;
        public long Highest;
        private BitRock _curRock;
        private int _rY;
        private readonly int _width;

        public BitGrid(int width)
        {
            _width = width;
            Rows = new long[width];
            for (int i = 1; i < width - 1; i++)
            {
                Rows[i] = 0;
            }
            Rows[0] = long.MaxValue;
            Rows[width - 1] = long.MaxValue;

            // add base row to seventh bit
            for (int i = 1; i < width - 1; i++)
            {
                Rows[i] |= 128L;
            }

            Highest = 0;
        }

        public void AddNewRock(BitRock curRock)
        {
            int shift = 8 - getHighestSolidPos();
            // shift bits to make sure we have seven rows free at start 
            for (int i = 1; i < _width - 1; i++)
            {
                Rows[i] = Rows[i] << shift;
            }

            Highest += shift;
            _curRock = curRock;
            _rY = 3;
        }

        public void PushRock(int dir)
        {
            int newRy = _rY + dir;
            bool canPush = true;
            for (int y = 0; y < _curRock.W; y++)
            {
                if ((Rows[newRy + y] & _curRock.SelectS(y)) != 0)
                {
                    canPush = false;
                    break;
                }
            }
            if (canPush) _rY = newRy;
        }

        public bool DropRock()
        {
            bool canDrop = true;
            for (int y = 0; y < _curRock.W; y++)
            {
                if ((Rows[y + _rY] & _curRock.SelectD(y)) != 0) 
                {
                    canDrop = false;
                    break;
                }
            }

            if (canDrop)
            {
                for (int y = 0; y < _curRock.W; y++)
                {
                    _curRock.SetS(y, _curRock.SelectS(y) << 1);
                    _curRock.SetD(y, _curRock.SelectD(y) << 1);
                }
#if DEBUG
                PrintTower("drop");
#endif
            }
            else
            {
                for (int y = 0; y < _curRock.W; y++)
                {
                    Rows[_rY + y] = Rows[_rY + y] | _curRock.SelectS(y);
                }
            }

            return canDrop;
        }

        private int getHighestSolidPos()
        {
            long curBit = 0;
            int curBitHeight = 0;
            int lowestBitHeight = int.MaxValue;
            for (int i = 1; i < _width - 1; i++)
            {
                curBit = 0;
                curBitHeight = 0;
                if ((Rows[i] & curBit) == 1) continue;
                curBit++;
                curBitHeight++;
                while ((Rows[i] & curBit) == 0)
                {
                    curBit = curBit << 1;
                    curBitHeight++;
                }
                if (curBitHeight < lowestBitHeight) lowestBitHeight = curBitHeight;
            }
            return lowestBitHeight;
        }

        public void PrintTower(string msg)
        {
            if (_visualise)
            {
                Thread.Sleep(150);
                Console.Clear();
                StringBuilder sb = new StringBuilder();
                int bit = 1;
                for (int i = 0; i < 24; i++)
                {
                    for (int y = 0; y < _width; y++)
                    {
                        bool show = (Rows[y] & bit) == bit;
                        if (y >= _rY && y < _rY + _curRock.W)
                            show = show || ((_curRock.SelectS(y - _rY) & bit) == bit);
                        if (y == 0)
                            sb.Append(show ? i.ToString()[^1] : ' ');
                        else
                            sb.Append(show ? AoCHelper.SolidBlockChar : ' ');
                    }
                    bit = bit << 1;
                    if (i == 0)
                    {
                        sb.Append("H: ");
                        sb.Append(Highest.ToString());
                        sb.Append(", ");
                        sb.Append(msg);
                    }
                    Console.Out.WriteLine(sb.ToString());
                    sb.Clear();
                }
            }
        }

        internal string GetRowsAsStr()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < _width - 1; i++)
            {
                sb.Append(Rows[i].ToString());
            }
            return sb.ToString();
        }
    }

    private struct BitRock
    {
        public long S1;
        public long S2;
        public long S3;
        public long S4;
        public readonly int W;
        public readonly int H;
        public long D1;
        public long D2;
        public long D3;
        public long D4;        

        public BitRock(long s1, long s2, long s3, long s4, int width, int height)
        {
            S1 = s1;
            S2 = s2;
            S3 = s3;
            S4 = s4;
            W = width;
            H = height;
            D1 = S1 << 1;
            D2 = S2 << 1;
            D3 = S3 << 1;
            D4 = S4 << 1;
        }
        public void SetS(int i, long val)
        {
            if (i == 0) S1 = val;
            else if (i == 1) S2 = val;
            else if (i == 2) S3 = val;
            else if (i == 3) S4 = val;        
        }

        public void SetD(int i, long val)
        {
            if (i == 0) D1 = val;
            else if (i == 1) D2 = val;
            else if (i == 2) D3 = val;
            else if (i == 3) D4 = val;
        }

        public long SelectS(int i)
        {
            if (i == 0) return S1;
            else if (i == 1) return S2;
            else if (i == 2) return S3;
            else if (i == 3) return S4;
            throw new Exception();
            return 0;
        }

        public long SelectD(int i)
        {
            if (i == 0) return D1;
            else if (i == 1) return D2;
            else if (i == 2) return D3;
            else if (i == 3) return D4;
            throw new Exception();
            return 0;
        }

    }   
}
