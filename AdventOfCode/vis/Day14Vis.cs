using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Collections.Concurrent;

namespace AdventOfCode.vis
{
    internal class Day14Vis
    {
        private Viewer _renderer;
        private ConcurrentQueue<RenderItem> _renderQueue = new ConcurrentQueue<RenderItem>();
        private static readonly int MAX_PER_FRAME = 60;
        private int _width = 800;
        private int _height = 164;
        private int _cellSize = 3;
        private Rectangle[,] _sandRecs;
        private Rectangle[,] _wallRecs;
        private string sandN = string.Empty;

        internal struct RenderItem
        {
            internal RenderItem(byte type, int x, int y, string s)
            {
                Type = type;
                X = x;
                Y = y;
                S = s;
            }

            internal byte Type { get; private set; }
            internal int X { get; private set; }
            internal int Y { get; private set; }        
            internal string S { get; private set; }
        }

        internal void StartVisualiser(Action solver)
        {
            _sandRecs = new Rectangle[_width, _height];
            _wallRecs = new Rectangle[_width, _height];
            _renderer = new Viewer(_width * _cellSize, _height * _cellSize, 30, "Day14");
            Task.Run(() => solver());
            _renderer.StartViewer(processFrame);
        }

        internal void AddRenderItem(RenderItem item)
        {
            _renderQueue.Enqueue(item);
        }

        internal void processFrame()
        {
            processQueue();
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    DrawRectangle((int)_wallRecs[x, y].x, (int)_wallRecs[x, y].y, (int)_wallRecs[x, y].width, (int)_wallRecs[x, y].height, GRAY);
                    DrawRectangle((int)_sandRecs[x, y].x, (int)_sandRecs[x, y].y, (int)_sandRecs[x, y].width, (int)_sandRecs[x, y].height, YELLOW);
                    //DrawText(openNodes, 10, 10, 20, WHITE);
                }
            }
        }

        private RenderItem _r;
        private void processQueue()
        {            
            int thisFrame = 0;
            while (_renderQueue.TryDequeue(out _r))
            {
                switch (_r.Type)
                {
                    case 0:
                        _wallRecs[_r.X, _r.Y].x = _r.X * _cellSize;
                        _wallRecs[_r.X, _r.Y].y = _r.Y * _cellSize;
                        _wallRecs[_r.X, _r.Y].width = _cellSize;
                        _wallRecs[_r.X, _r.Y].height = _cellSize;
                        break;
                    case 1:
                        _sandRecs[_r.X, _r.Y].x = _r.X * _cellSize;
                        _sandRecs[_r.X, _r.Y].y = _r.Y * _cellSize;
                        _sandRecs[_r.X, _r.Y].width = _cellSize;
                        _sandRecs[_r.X, _r.Y].height = _cellSize;
                        break;                    
                    case 2:
                        sandN = _r.S;
                        break;
                }
                if (thisFrame++ >= MAX_PER_FRAME) break;
            }
        }
    }
}
