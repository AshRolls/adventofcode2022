using AdventOfCode.vis;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    private readonly string[] _input;
    ASCIIRay renderer = new ASCIIRay(1920, 1080, 30, 10, "Day08");

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath);        
    }

    public override ValueTask<string> Solve_1()
    {
        renderer.loop(x => render1(x));

        return new("rendering");
    }

    private bool render1(int idx)
    {
        byte[,] grid = new byte[99, 99];
        bool[,] visible = new bool[99, 99];
        string line;
        for (int y = 0; y < 99; y++)
        {
            line = _input[y];
            for (int x = 0; x < 99; x++)
            {
                bool vis = false;
                if (x == 0 || x == 98 || y == 0 || y == 98) vis = true;
                grid[x, y] = byte.Parse(line[x].ToString());
                visible[x, y] = vis;
                if (vis) renderer.WriteXY(x, y, "X");
            }
        }

        for (int x = 1; x < 98; x++)
        {
            for (int y = 1; y < 98; y++)
            {
                setVisible(grid, visible, x, y);
            }
        }

        int visC = 0;
        for (int y = 0; y < 99; y++)
        {
            for (int x = 0; x < 99; x++)
            {
                if (visible[x, y])
                {
                    visC++;
                    renderer.WriteXY(x, y, "X");
                }
            }
        }

        return false;
    }

    public override ValueTask<string> Solve_2()
    {
        return new("Not Solved");
    }

    private void setVisible(byte[,] grid, bool[,] visible, int x, int y)
    {
        // left
        bool up = true, down = true, left = true, right = true;
        for (int i = 0; i < x; i++)
        {
            if (grid[i,y] >= grid[x,y])
            {
                left = false;
                break;
            }
        }        
        // right
        for (int i = 98; i > x; i--)
        {
            if (grid[i, y] >= grid[x, y])
            {
                right = false;
                break;
            }                       
        }
        // up
        for (int i = 0; i < y; i++)
        {
            if (grid[x, i] >= grid[x, y])
            {
                up = false;
                break;
            }            
        }
        // down
        for (int i = 98; i > y; i--)
        {
            if (grid[x, i] >= grid[x, y])
            {
                down = false;
                break;
            }            
        }
        visible[x, y] = up || down || left || right;
    }
}
