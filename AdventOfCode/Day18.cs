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
        bool[,,] droplet = parseFile(ref maxX, ref maxY, ref maxZ);

        int xCnt = 0;
        int yCnt = 0;
        int zCnt = 0;
        findSurface(maxX, maxY, maxZ, droplet, ref xCnt, ref yCnt, ref zCnt);

        _partOne = (xCnt + yCnt + zCnt).ToString();
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(_partTwo);
    }

    private void solve2()
    {
        int maxX = 0;
        int maxY = 0;
        int maxZ = 0;
        bool[,,] droplet = parseFile(ref maxX, ref maxY, ref maxZ);

        Stack<(int, int, int)> neighbours = new Stack<(int, int, int)>();
        HashSet<(int, int, int)> visited = new HashSet<(int, int, int)>();

        neighbours.Push((0, 0, 0));
        while (neighbours.Count > 0)
        {
            (int x, int y, int z) cur = neighbours.Pop();
            visited.Add(cur);

            int xP = Math.Min(maxX - 1, cur.x + 1);
            if (!droplet[xP, cur.y, cur.z] && !visited.Contains((xP, cur.y, cur.z))) neighbours.Push((xP, cur.y, cur.z));
            int xN = Math.Max(0, cur.x - 1);
            if (!droplet[xN, cur.y, cur.z] && !visited.Contains((xN, cur.y, cur.z))) neighbours.Push((xN, cur.y, cur.z));

            int yP = Math.Min(maxY - 1, cur.y + 1);
            if (!droplet[cur.x, yP, cur.z] && !visited.Contains((cur.x, yP, cur.z))) neighbours.Push((cur.x, yP, cur.z));
            int yN = Math.Max(0, cur.y - 1);
            if (!droplet[cur.x, yN, cur.z] && !visited.Contains((cur.x, yN, cur.z))) neighbours.Push((cur.x, yN, cur.z));

            int zP = Math.Min(maxZ - 1, cur.z + 1);
            if (!droplet[cur.x, cur.y, zP] && !visited.Contains((cur.x, cur.y, zP))) neighbours.Push((cur.x, cur.y, zP));
            int zN = Math.Max(0, cur.z - 1);
            if (!droplet[cur.x, cur.y, zN] && !visited.Contains((cur.x, cur.y, zN))) neighbours.Push((cur.x, cur.y, zN));
        }

        bool[,,] newVoidDroplet = new bool[maxX, maxY, maxZ];
        for (int x = 0; x < maxX; x++)
            for (int y = 0; y < maxY; y++)
                for (int z = 0; z < maxZ; z++)
                    newVoidDroplet[x, y, z] = true;

        foreach ((int x, int y, int z) v in visited)
        {
            newVoidDroplet[v.x, v.y, v.z] = false;
        }

        int xCnt = 0;
        int yCnt = 0;
        int zCnt = 0;
        findSurface(maxX, maxY, maxZ, newVoidDroplet, ref xCnt, ref yCnt, ref zCnt);

        _partTwo = (xCnt + yCnt + zCnt).ToString();
    }

    private static void findSurface(int maxX, int maxY, int maxZ, bool[,,] droplet, ref int xCnt, ref int yCnt, ref int zCnt)
    {
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
                    if (droplet[x, y, z] != state)
                    {
                        state = droplet[x, y, z];
                        zCnt++;
                    }
                }
            }
        }
    }

    private bool[,,] parseFile(ref int maxX, ref int maxY, ref int maxZ)
    {
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

        return droplet;
    }
}
