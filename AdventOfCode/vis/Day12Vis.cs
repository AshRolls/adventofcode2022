using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Collections.Concurrent;

namespace AdventOfCode.vis
{
    internal class Day12Vis
    {                
        private Viewer _renderer = new Viewer(640, 400, 30, "Day12");
        private ConcurrentQueue<RenderItem> _renderQueue = new ConcurrentQueue<RenderItem>();
        private static readonly int MAX_PER_FRAME = 30;
        private static readonly int WIDTH = 64;
        private static readonly int HEIGHT = 40;
        private Rectangle[,] _pathRecs = new Rectangle[WIDTH, HEIGHT];
        private ColorRectangle[,] _heightRecs = new ColorRectangle[WIDTH, HEIGHT];
        private Dictionary<int,Color> _greyColors = new Dictionary<int,Color>();
        private Color _opaqueGreen;
        private string openNodes = string.Empty;

        internal struct ColorRectangle
        {
            public Rectangle rec;
            public int v;
        }
        
        internal struct RenderItem
        {
            internal RenderItem(byte type, int x, int y, int v, string s)
            {
                Type = type;
                X = x;
                Y = y;
                V = v;
                S = s;
            }

            internal byte Type { get; private set; }
            internal int X { get; private set; }
            internal int Y { get; private set; }
            internal int V { get; private set; }
            internal string S { get; private set; }
        }

        internal void StartVisualiser(Action solver)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    _heightRecs[x,y] = new ColorRectangle();
                }
            }
            for (int i = 0; i< 26; i++)
            {
                _greyColors.Add(i, new Color(i * 10, i * 10, i * 10, 255));
            }
            _opaqueGreen = new Color(0, 90, 0, 110);

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
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT; y++)
                {
                    DrawRectangle((int)_heightRecs[x, y].rec.x, (int)_heightRecs[x, y].rec.y, (int)_heightRecs[x, y].rec.width, (int)_heightRecs[x, y].rec.height, _greyColors[_heightRecs[x, y].v]);
                    DrawRectangle((int)_pathRecs[x, y].x, (int)_pathRecs[x, y].y, (int)_pathRecs[x, y].width, (int)_pathRecs[x, y].height, _opaqueGreen);
                    DrawText(openNodes, 10, 10, 20, WHITE);
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
                        _pathRecs[_r.X, _r.Y].x = _r.X * 10;
                        _pathRecs[_r.X, _r.Y].y = _r.Y * 10;
                        _pathRecs[_r.X, _r.Y].width = 10;
                        _pathRecs[_r.X, _r.Y].height = 10;
                        break;
                    case 1:
                        _heightRecs[_r.X, _r.Y].rec.x = _r.X * 10;
                        _heightRecs[_r.X, _r.Y].rec.y = _r.Y * 10;
                        _heightRecs[_r.X, _r.Y].rec.width = 10;
                        _heightRecs[_r.X, _r.Y].rec.height = 10;
                        _heightRecs[_r.X, _r.Y].v = _r.V;
                        break;
                    case 2:
                        openNodes = _r.S;
                        break;
                }
                if (thisFrame++ >= MAX_PER_FRAME) break;
            }
        }
    }
}
