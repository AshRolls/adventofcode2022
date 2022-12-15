using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;

namespace AdventOfCode;

public class Day15 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day15()
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
        int numSensors = _input.Length;

        Sensor[] sensors= new Sensor[numSensors];
        HashSet<Beacon> beacons = new HashSet<Beacon>();

        for (int i = 0; i < _input.Length; i++)
        {
            var v = AoCHelper.GetNumsFromStr(_input[i]);
            sensors[i] = new Sensor(v[0], v[1], AoCHelper.GetManhattanDist(v[0], v[1], v[2], v[3]));
            _ = beacons.Add(new Beacon(v[2], v[3]));
        }

        int minX = int.MaxValue;
        int maxX = int.MinValue;

        for (int i = 0; i < numSensors; i++)
        {
            int min = sensors[i].X - sensors[i].D;
            int max = sensors[i].X + sensors[i].D;
            if (min < minX) minX = min;
            if (max > maxX) maxX = max;
        }

        const int y = 2000000;        
        int withinRange = 0;
        for (int x = minX-1; x< maxX +1; x++)
        {
            for (int s = 0; s < numSensors; s++)
            {
                if (AoCHelper.GetManhattanDist(x, y, sensors[s].X, sensors[s].Y) <= sensors[s].D)
                {
                    withinRange++;
                    break;
                }
            }
        }

        // remove any beacons on the line
        foreach (Beacon b in beacons)  
            if (b.Y == y) withinRange--;
                             
        _partOne = withinRange.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        int numSensors = _input.Length;

        Sensor[] sensors = new Sensor[numSensors];
        HashSet<Beacon> beacons = new HashSet<Beacon>();

        for (int i = 0; i < _input.Length; i++)
        {
            var v = AoCHelper.GetNumsFromStr(_input[i]);
            sensors[i] = new Sensor(v[0], v[1], AoCHelper.GetManhattanDist(v[0], v[1], v[2], v[3]));
            _ = beacons.Add(new Beacon(v[2], v[3]));
        }

        int bx = 0;
        int by = 0;
        //var solution = new ConcurrentBag<(int,int)>();
        const int space = 10000;
        //bool withinRange;
        Stopwatch s = Stopwatch.StartNew();
        for (int y = 0; y <= space; y++)            
        {
            Parallel.For(0, space, x =>
            {
                bool withinRange = false;
                for (int s = 0; s < numSensors; s++)
                {
                    if (AoCHelper.GetManhattanDist(x, y, sensors[s].X, sensors[s].Y) <= sensors[s].D)
                    {
                        withinRange = true;                        
                        break;
                    }
                }
                if (!withinRange)
                {
                    bx = x;
                    by = y;
                    Console.Out.WriteLine(bx + "," + by + "  ");
                }               
            });// end parallel.for            
        }
        Console.Out.WriteLine("100000 grid time in milliseconds: {0}", s.ElapsedMilliseconds);
        s.Reset();
        s.Start();

        long tuningFreq = (long)(bx)  * 4000000 + (long)by;
        _partTwo = (tuningFreq.ToString());
    }



    private struct Sensor
    {
        public Sensor (int x, int y, int d)
        {
            X = x;
            Y= y; 
            D = d;
        }

        public int X;
        public int Y;
        public int D; // manhattan distance to closest beacon
    }

    private struct Beacon
    {
        public Beacon(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;            
        public int Y;
    }
}
