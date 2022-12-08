﻿using AdventOfCode.vis;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Collections.Concurrent;

namespace AdventOfCode;

public class Day08 : BaseDay
{
    private readonly string[] _input;
    private readonly int MAX_PER_FRAME = 30;
    Viewer _renderer = new Viewer(990, 990, 30, "Day08");
    ConcurrentQueue<RenderItem> _renderQueue = new ConcurrentQueue<RenderItem>();

    private string partOne;
    private string partTwo;

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath);        
    }

    internal Rectangle[,] recs = new Rectangle[99, 99];

    public void processFrame()
    {
        processQueue();
        for (int x = 0; x < 99; x++)
        {
            for (int y = 0; y < 99; y++)
            {
                DrawRectangle((int)recs[x,y].x, (int)recs[x, y].y, (int)recs[x, y].width, (int)recs[x, y].height, GREEN);
            }
        }
    }
    public void processQueue()
    {        
        RenderItem r;
        int thisFrame = 0;
        while (_renderQueue.TryDequeue(out r))
        {            
            if (r != null)
            {
                switch (r.Type)
                {
                    case 0:
                        recs[r.X, r.Y].x = r.X * 10;
                        recs[r.X, r.Y].y = r.Y * 10;
                        recs[r.X, r.Y].width = 10;
                        recs[r.X, r.Y].height = 10;
                        break;
                }
            }
            if (thisFrame++ >= MAX_PER_FRAME) break;
        }
    }

    public override ValueTask<string> Solve_1()
    {
        Task.Run(() => solve1());
        _renderer.loop(processFrame);        
        return new(partOne);
    }

    public override ValueTask<string> Solve_2()
    {
        solve2();
        return new(partTwo);
    }

    private void solve1()
    {
        byte[,] grid = new byte[99, 99];
        bool[,] visible = new bool[99, 99];
        parseData(grid, visible);

        for (int x = 1; x < 98; x++)
        {
            for (int y = 1; y < 98; y++)
            {
                setVisible(grid, visible, x, y);                
            }
        }

        partOne = getVisibleCount(visible).ToString();        
    }

    private void solve2()
    {
        byte[,] grid = new byte[99, 99];
        bool[,] visible = new bool[99, 99];
        int[,] scenic = new int[99, 99];
        parseData(grid, visible);

        for (int x = 1; x < 98; x++)
        {
            for (int y = 1; y < 98; y++)
            {
                setScenic(grid, scenic, x, y);
            }
        }

        partTwo = getHighestScenic(scenic).ToString();        
    }

    private int getHighestScenic(int[,] scenic)
    {
        int highest = 0;
        for (int x = 1; x < 98; x++)
        {
            for (int y = 1; y < 98; y++)
            {
                if (scenic[x, y] > highest) highest = scenic[x,y];                
            }
        }
        return highest;
    }

    private int getVisibleCount(bool[,] visible)
    {
        int visC = 0;
        for (int y = 0; y < 99; y++)
        {
            for (int x = 0; x < 99; x++)
            {
                if (visible[x, y])
                {
                    _renderQueue.Enqueue(new RenderItem(0, x, y));
                    visC++;                    
                }
            }
        }
        return visC;
    }

    private void parseData(byte[,] grid, bool[,] visible)
    {
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
            }
        }
    }

    private void setVisible(byte[,] grid, bool[,] visible, int x, int y)
    {
        // left
        bool left = true;
        for (int i = x - 1; i >= 0; i--)
        {
            if (grid[i, y] >= grid[x, y])
            {
                left = false;
                break;
            }
        }
        if (left) 
        {
            visible[x, y] = true; 
            return;         
        }

        // right
        bool right = true;
        for (int i = x + 1; i <= 98; i++)
        {
            if (grid[i, y] >= grid[x, y])
            {
                right = false;
                break;
            }                       
        }
        if (right)
        {
            visible[x, y] = true;
            return;
        }

        // up
        bool up = true;
        for (int i = y - 1; i >= 0; i--)
        {
            if (grid[x, i] >= grid[x, y])
            {
                up = false;
                break;
            }            
        }
        if (up)
        {
            visible[x, y] = true;
            return;
        }

        // down
        bool down = true;
        for (int i = y + 1; i <= 98; i++)
        {
            if (grid[x, i] >= grid[x, y])
            {
                down = false;
                break;
            }            
        }
        if (down)
        {
            visible[x, y] = true;
            return;
        }
    }

    private void setScenic(byte[,] grid, int[,] scenic, int x, int y)
    {
        // left
        int left = 0;
        for (int i = x - 1; i >= 0; i--)
        {
            left++;
            if (grid[i, y] >= grid[x, y])
            {
                break;
            }            
        }

        // right
        int right = 0;
        for (int i = x + 1; i <= 98; i++)
        {
            right++;
            if (grid[i, y] >= grid[x, y])
            {
                break;
            }            
        }

        // up
        int up = 0;
        for (int i = y - 1; i >= 0; i--)
        {
            up++;
            if (grid[x, i] >= grid[x, y])
            {
                break;
            }
        }


        // down
        int down = 0;
        for (int i = y + 1; i <= 98; i++)
        {
            down++;
            if (grid[x, i] >= grid[x, y])
            {
                break;
            }
        }

        scenic[x, y] = left * right * up * down;            
    }

}
