using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace AdventOfCode;

public class Day17 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;
    private static int _pagesize = 100000;

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
        const int height = 8;
        Rock[] rocks = setupRocks();

        CollisionGrid cg = new CollisionGrid(width, height);
        
        Run(rocksToAdd, rocks, cg);

        _partOne = cg.GetTowerHeight().ToString();
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
        const int height = 8;
        Rock[] rocks = setupRocks();

        CollisionGrid cg = new CollisionGrid(width, height);

        Run(rocksToAdd, rocks, cg);

        _partTwo = cg.GetTowerHeight().ToString();
    }

    private void Run(long rocksToAdd, Rock[] rocks, CollisionGrid cg)
    {
        Rock curRock;
        long rocksAdded = 0;
        int jet = 0;
        bool addNew = true;
        Stopwatch sw = Stopwatch.StartNew();
        while (rocksAdded <= rocksToAdd)
        {
            if (addNew)
            {
                curRock = rocks[rocksAdded++ % 5];
                if (rocksAdded % 1000000 == 0)
                {                                    
                    long ms = sw.ElapsedMilliseconds;
                    long minRem = ((rocksToAdd / 1000000) * ms) / 1000 / 60;
                    Console.Out.WriteLine("{4}%, Mins: {3}, Rocks Added: {0}, Height: {1}, Time: {2}", rocksAdded, cg.GetTowerHeight().ToString(), ms, minRem, ((float)rocksAdded/(float)rocksToAdd)*(float)100);
                    sw.Restart();                    
                }
                cg.AddNewRock(curRock);
            }

            pushRock(cg, jet++);
            if (jet > _input[0].Length - 1) jet = 0;

            addNew = !cg.DropRock();
        }
        sw.Stop();
    }

    private Rock[] setupRocks()
    {
        Rock[] rocks = new Rock[5];

        byte[,] shape1 = {  { 2, 1, 0, 0, 0 },
                            { 2, 1, 0, 0, 0 },
                            { 2, 1, 0, 0, 0 },
                            { 2, 1, 0, 0, 0 },
        };
        byte[,] shape2 = {  { 0, 2, 1, 0, 0 },
                            { 2, 1, 1, 1, 0 },
                            { 0, 2, 1, 0, 0 },
                            { 0, 0, 0, 0, 0 },
        };
        byte[,] shape3 = {  { 2, 1, 0, 0, 0 },
                            { 2, 1, 0, 0, 0 },
                            { 2, 1, 1, 1, 0 },
                            { 0, 0, 0, 0, 0 },
        };
        byte[,] shape4 = {  { 2, 1, 1, 1, 1 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
        };
        byte[,] shape5 = {  { 2, 1, 1, 0, 0 },
                            { 2, 1, 1, 0, 0 },
                            { 0, 0, 0, 0, 0 },
                            { 0, 0, 0, 0, 0 },
        };
        Rock rock1 = new Rock(shape1);
        Rock rock2 = new Rock(shape2);
        Rock rock3 = new Rock(shape3);
        Rock rock4 = new Rock(shape4);
        Rock rock5 = new Rock(shape5);
        rocks[0] = rock1;
        rocks[1] = rock2;
        rocks[2] = rock3;
        rocks[3] = rock4;
        rocks[4] = rock5;

        return rocks;
    }

    private void pushRock(CollisionGrid cg, int jetIdx)
    {
        switch (_input[0][jetIdx])
        {
            case '>':
                cg.PushRock(1);
                break;
            default:
                cg.PushRock(-1);
                break;
        }
    }

    private class CollisionGrid
    {
        private Rock _currentRock;
        private int _rx;
        private int _ry;
        private Row[] _rows; 
        private Row _highestSolidRow;
        private Row _baseRow;
        private bool[,] _grid;
        private int _gridHeight;
        private int _gridWidth;
        private long _lastHighestHeight;
        private Row _lastHighestRow;

        public CollisionGrid(int width, int height)
        {
            _gridWidth = width;
            _gridHeight = height;
            _grid = new bool[width, height];
            _rows = new Row[height];
            _baseRow = new Row(0);
            for (int i = 0; i < width; i++) _baseRow.Filled[i] = true;
            _highestSolidRow = _baseRow;                        
           _lastHighestHeight = 0;
           _lastHighestRow = _baseRow;
        }        

        internal void AddNewRock(Rock curRock)
        {
            // reset existing grid
            for (int y = 0; y < _gridHeight; y++)
            {
                for (int x = 1; x < _gridWidth -1; x++) // don't reset walls
                {
                    _grid[x, y] = false;
                }
            }
            // reset rows
            for (int y = 0; y < _gridHeight; y++) _rows[y] = null;                            
            _rows[0] = _highestSolidRow;
            fillRows();

            // copy highest solid row to bottom of grid 
            for (int x = 0; x < _gridWidth; x++)
            {
                _grid[x, 0] = _highestSolidRow.Filled[x];
            }

            // add the rock 
            _currentRock = curRock;
            _rx = 3;
            _ry = 3;
        }

        internal void PushRock(int dir)
        {
            int newX = _rx + dir;
            bool pushable = true;
            for (int y = 1; y < 5; y++)                
            {
                for (int x = 0; x < 4; x++)
                {
                    if ((newX + x > _gridWidth - 1 || _grid[newX + x, _ry + y]) && _currentRock.Shape[x, y] == (byte)1)
                    {
                        pushable = false;
                        break;
                    }
                }
                if (!pushable) break;
            }
            if (pushable) _rx = newX;
        }

        internal bool DropRock()
        {
            // check if we are able to drop
            bool dropping = true;
            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if ((x + _rx > _gridWidth - 1 || _grid[x + _rx, y + _ry]) && _currentRock.Shape[x, y] == (byte)2)
                    {
                        dropping = false;
                        break;
                    }                   
                }
                if (!dropping) break;
            }

            if (!dropping)
            {
                // add to rows as permanent and set new highest solid                
                bool lineFilled;
                Row highestFilledRow = null;
                for (int y = 1; y < 5; y++)
                {
                    lineFilled = false;
                    for (int x = 0; x < 4; x++)
                    {
                        if (_currentRock.Shape[x, y] == (byte)1)
                        {
                            //Grid[x + _rx, y + _ry] = true;  // no need to add to grid, it will be cleared soon
                            _rows[y + _ry].Filled[x + _rx] = true;
                            lineFilled = true;
                        }
                    }
                    if (lineFilled)
                    {
                        highestFilledRow = _rows[y + _ry];
                    }
                }
                if (highestFilledRow != null && highestFilledRow.Height > _highestSolidRow.Height)
                {
                    _highestSolidRow = highestFilledRow;
                    if (_highestSolidRow.Height - _lastHighestHeight > _pagesize)
                    {
                        _lastHighestHeight = _highestSolidRow.Height;
                        _lastHighestRow.PrevRow = null; // break the chain to free mem
                        _lastHighestRow = _highestSolidRow;
                    }
                }
            }
            else
            {
                // shift rows up
                for (int i = _gridHeight - 1; i > 0; i--)
                {
                    _rows[i] = _rows[i - 1];
                }
                if (_rows[1].PrevRow != null)
                {
                    _rows[0] = _rows[1].PrevRow;                    
                }                
                copyRowsToGrid();
            }

            return dropping;
        }

        private void copyRowsToGrid()
        {            
            for (int y = 0; y < _gridHeight; y++)
            {                
                for (int x = 0; x < _gridWidth; x++)
                {
                    _grid[x,y] = _rows[y].Filled[x];
                }
            }
        }

        private void fillRows()
        {
            List<Row> rs = new List<Row>();
            for (int i = 1; i<_gridHeight; i++)
            {
                Row newRow = new Row(_highestSolidRow.Height + i);
                rs.Add(newRow);
            }
            Debug.Assert(rs.Count == 7);
            for (int i = rs.Count - 1; i > 0; i--)
            {
                rs[i].PrevRow = rs[i - 1];
                _rows[i + 1] = rs[i];
            }
            rs[0].PrevRow = _highestSolidRow;
            _rows[1] = rs[0];
        }

        public long GetTowerHeight()
        {
            return _highestSolidRow.Height;            
        }

        public void PrintGrid()
        {
            for (int y = _gridHeight - 1; y >= 0; y--)
            {                
                Console.Out.WriteLine(_rows[y]);
            }
        }

        public void PrintTower()
        {
            Row curRow = _highestSolidRow;
            while (curRow != _baseRow)
            {
                Console.Out.WriteLine(curRow);
                curRow = curRow.PrevRow;                
            }
            Console.Out.WriteLine(_baseRow);
        }
    }        

    private struct Rock
    {
        public Rock(byte[,] shape)
        {
            Shape = shape;            
        }

        public byte[,] Shape;        
    }

    private class Row
    {
        public Row PrevRow;
        public bool[] Filled = new bool[9];
        public long Height;
        
        public Row(long height)
        {
            Height = height;
            // walls
            Filled[0] = true;
            Filled[^1] = true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (bool b in Filled)
            {
                if (b) sb.Append('#');
                else sb.Append('.');
            }
            return sb.ToString();
        }
    }   
}
