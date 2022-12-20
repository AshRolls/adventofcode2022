using Spectre.Console;
using System.Drawing.Printing;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace AdventOfCode;

public class Day19 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day19()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        //solve1();
        return new(_partOne);
    }

    private void solve1()
    {
        Blueprint[] blueprints = parseInput();

        int totalQual = 0;
        for(int i = 0; i < _input.Length; i++)
        {
            blueprints[i].GeodesMax = GetMaxGeodes(blueprints[i], 24);            
            totalQual += blueprints[i].Quality;
        }        

        _partOne = totalQual.ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        Blueprint[] blueprints = parseInput();

        int geodesMult = 1;
        for (int i = 0; i < 3; i++)
        {
            if (i < _input.Length)
            {
                blueprints[i].GeodesMax = GetMaxGeodes(blueprints[i], 32);
                geodesMult *= blueprints[i].GeodesMax;
            }
        }

        _partTwo = geodesMult.ToString();
    }

    private Blueprint[] parseInput()
    {
        Blueprint[] blueprints = new Blueprint[_input.Length];
        for (int i = 0; i < _input.Length; i++)
        {
            var nums = AoCHelper.GetNumsFromStr(_input[i]);
            blueprints[i] = new Blueprint(nums[0], nums[1], nums[2], nums[3], nums[4], nums[5], nums[6]);
        }

        return blueprints;
    }

    private static int GetMaxGeodes(Blueprint bp, int maxMins)
    {
        Node startN = new Node(1, 0, 0, 0, 0, 0, 0, 0, 0);

        Stack<Node> stack = new Stack<Node>();
        stack.Push(startN);

        int highestGeodes = 0;

        long nodeCount = 0;

        while (stack.Count > 0)
        {
            Node cur = stack.Pop();
            nodeCount++;
            if (nodeCount % 500000000 == 0)
            {
                Console.Out.WriteLine(nodeCount.ToString());
            }

            if (cur.Geo > highestGeodes)
            {
                Console.Out.WriteLine("{0} {1} {2}", bp.Number, cur.Min, cur.Geo);
                highestGeodes = cur.Geo;
            }

            if (cur.Min < maxMins)
            {
                int minsRem = maxMins - cur.Min;
                int gaussGeo = AoCHelper.GetSumRange(cur.GeoRobots, cur.GeoRobots + minsRem) + cur.Geo;

                if (highestGeodes >  gaussGeo) continue;

                bool canOre = canBuildOreR(bp, cur);
                bool canClay = canBuildClayR(bp, cur);
                bool canObs = canBuildObsR(bp, cur);
                bool canGeo = canBuildGeoR(bp, cur);

                //case BuildOptions.GeoR:
                if (canGeo)
                {
                    addNode(stack, cur.OreRobots, cur.ClayRobots, cur.ObsRobots, cur.GeoRobots + 1, 
                                cur.Ore - bp.GeodeROre + cur.OreRobots, 
                                cur.Clay + cur.ClayRobots, 
                                cur.Obs - bp.GeodeRObsidian + cur.ObsRobots, 
                                cur.Geo + cur.GeoRobots, 
                                cur.Min + 1);
                }
                else
                {
                    //case BuildOptions.None: 
                    if (!(canOre && canClay && canObs))
                    {
                        addNode(stack, cur.OreRobots, cur.ClayRobots, cur.ObsRobots, cur.GeoRobots,
                            cur.Ore + cur.OreRobots,
                            cur.Clay + cur.ClayRobots,
                            cur.Obs + cur.ObsRobots,
                            cur.Geo + cur.GeoRobots,
                            cur.Min + 1);
                    }

                    //case BuildOptions.ObsR:
                    if (canObs)
                    {
                        addNode(stack, cur.OreRobots, cur.ClayRobots, cur.ObsRobots + 1, cur.GeoRobots,
                            cur.Ore + cur.OreRobots - bp.ObsidianROre,
                            cur.Clay + cur.ClayRobots - bp.ObsidianRClay,
                            cur.Obs + cur.ObsRobots,
                            cur.Geo + cur.GeoRobots,
                            cur.Min + 1);
                    }

                    //case BuildOptions.ClayR:
                    if (canClay)
                    {
                        addNode(stack, cur.OreRobots, cur.ClayRobots + 1, cur.ObsRobots, cur.GeoRobots,
                            cur.Ore + cur.OreRobots - bp.ClayROre,
                            cur.Clay + cur.ClayRobots,
                            cur.Obs + cur.ObsRobots,
                            cur.Geo + cur.GeoRobots,
                            cur.Min + 1);
                    }

                    //case BuildOptions.OreR:                
                    if (canOre)
                    {                                                
                        addNode(stack, cur.OreRobots + 1, cur.ClayRobots, cur.ObsRobots, cur.GeoRobots,
                            cur.Ore + cur.OreRobots - bp.OreROre,
                            cur.Clay + cur.ClayRobots,
                            cur.Obs + cur.ObsRobots,
                            cur.Geo + cur.GeoRobots,
                            cur.Min + 1);
                    }                                        
                }  // end if cangeo                     
            } // end if max mins                               
        } // end stack

        return highestGeodes;
    }

    private static bool canBuildGeoR(Blueprint bp, Node cur)
    {
        return cur.Obs >= bp.GeodeRObsidian && cur.Ore >= bp.GeodeROre;
    }

    private static bool canBuildObsR(Blueprint bp, Node cur)
    {
        return cur.ObsRobots <= bp.MaxObsR && cur.Clay >= bp.ObsidianRClay && cur.Ore >= bp.ObsidianROre;
    }

    private static bool canBuildClayR(Blueprint bp, Node cur)
    {
        return cur.ClayRobots <= bp.MaxClayR && cur.Ore >= bp.ClayROre;
    }

    private static bool canBuildOreR(Blueprint bp, Node cur)
    {
        return cur.OreRobots <= bp.MaxOreR && cur.Ore >= bp.OreROre;
    }

    private static void addNode(Stack<Node> stack, int oreR, int clayR, int obsR, int geoR, int ore, int clay, int obs, int geo, int min)
    {
        Node n = new Node(oreR, clayR, obsR, geoR, ore, clay, obs, geo, min);
        stack.Push(n);
    }

    private struct Node
    {
        public Node(int oreR, int clayR, int obsR, int geoR, int ore, int clay, int obs, int geo, int min)
        {
            this.OreRobots = oreR;
            this.ClayRobots = clayR;
            this.ObsRobots = obsR;
            this.GeoRobots = geoR;
            this.Ore = ore;
            this.Clay = clay;
            this.Obs = obs;
            this.Geo = geo;
            this.Min = min;
        }

        public int OreRobots;
        public int ClayRobots;
        public int ObsRobots;
        public int GeoRobots;

        public int Ore;
        public int Clay;
        public int Obs;
        public int Geo;

        public int Min;
    }

    private struct Blueprint
    {
        public Blueprint(int number, int oreROre, int clayROre, int obsidianROre, int obsidianRClay, int geodeROre, int geodeRObsidian)
        {
            Number = number;
            OreROre = oreROre;
            ClayROre = clayROre;
            ObsidianROre = obsidianROre;
            ObsidianRClay = obsidianRClay;
            GeodeROre = geodeROre;
            GeodeRObsidian = geodeRObsidian;

            MaxOreR = Math.Max(Math.Max(Math.Max(OreROre, ClayROre), ObsidianROre), GeodeROre);
            MaxClayR = ObsidianRClay;
            MaxObsR = GeodeRObsidian;           
        }

        public int Number;
        public int OreROre;
        public int ClayROre;
        public int ObsidianROre;
        public int ObsidianRClay;
        public int GeodeROre;
        public int GeodeRObsidian;

        public readonly int Quality => Number * GeodesMax;
        public int GeodesMax;

        public int MaxOreR;
        public int MaxClayR;
        public int MaxObsR;
    }
}
