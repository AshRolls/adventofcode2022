using System.Drawing.Drawing2D;

namespace AdventOfCode;

public class Day18 : BaseDay
{
    private readonly string[] _input;
    private string _partOne;
    private string _partTwo;

    public Day18()
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
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;

        foreach (string line in _input)
        {
            var nums = AoCHelper.GetNumsFromStr(line);
            if (nums[0] > maxX) maxX = nums[0];
            if (nums[1] > maxY) maxY = nums[1];
            if (nums[2] > maxZ) maxZ = nums[2];
        }

        maxX += 2;
        maxY += 2;
        maxZ += 2;

        bool[,,] droplet = new bool[maxX, maxY, maxZ];

        foreach (string line in _input)
        {
            var nums = AoCHelper.GetNumsFromStr(line);
            droplet[nums[0], nums[1], nums[2]] = true;
        }

        int xCnt = 0;
        int yCnt = 0;
        int zCnt = 0;

        for (int y = 0; y < maxY; y++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                bool state = false;
                for (int x = 0; x < maxX; x++)
                {
                    if (droplet[x, y, z] != state)
                    {
                        state = droplet[x, y, z];
                        xCnt++;
                    }
                }
            }
        }

        for (int x = 0; x < maxX; x++)
        {
            for (int z = 0; z < maxZ; z++)
            {
                bool state = false;
                for (int y = 0; y < maxY; y++)
                {
                    if (droplet[x, y, z] != state)
                    {
                        state = droplet[x, y, z];
                        yCnt++;
                    }
                }
            }
        }

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                bool state = false;
                for (int z = 0; z < maxZ; z++)
                {
                    if (droplet[x,y,z] != state)
                    {
                        state = droplet[x, y, z];
                        zCnt++;
                    }
                }
            }
        }
       
        _partOne = (xCnt + yCnt + zCnt).ToString();
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
