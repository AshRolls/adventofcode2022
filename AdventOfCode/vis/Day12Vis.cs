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

        internal class ColorRectangle
        {
            public Rectangle rec = new Rectangle();
            public Color color;
        }
        
        internal class RenderItem
        {
            internal RenderItem(byte type, int x, int y, int v = 0)
            {
                Type = type;
                X = x;
                Y = y;
                V = v;
            }

            internal byte Type { get; private set; }
            internal int X { get; private set; }
            internal int Y { get; private set; }
            internal int V { get; private set; }
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
                    DrawRectangle((int)_heightRecs[x, y].rec.x, (int)_heightRecs[x, y].rec.y, (int)_heightRecs[x, y].rec.width, (int)_heightRecs[x, y].rec.height, _heightRecs[x, y].color);
                    DrawRectangle((int)_pathRecs[x, y].x, (int)_pathRecs[x, y].y, (int)_pathRecs[x, y].width, (int)_pathRecs[x, y].height, DARKGREEN);
                }
            }
        }

        private void processQueue()
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
                            _pathRecs[r.X, r.Y].x = r.X * 10;
                            _pathRecs[r.X, r.Y].y = r.Y * 10;
                            _pathRecs[r.X, r.Y].width = 10;
                            _pathRecs[r.X, r.Y].height = 10;
                            break;
                        case 1:
                            _heightRecs[r.X, r.Y].rec.x = r.X * 10;
                            _heightRecs[r.X, r.Y].rec.y = r.Y * 10;
                            _heightRecs[r.X, r.Y].rec.width = 10;
                            _heightRecs[r.X, r.Y].rec.height = 10;
                            _heightRecs[r.X, r.Y].color = new Color(r.V * 10, r.V * 10, r.V * 10, 255);
                            break;
                    }
                }
                if (thisFrame++ >= MAX_PER_FRAME) break;
            }
        }
    }
}
