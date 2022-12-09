using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace AdventOfCode.vis
{
    internal class Day09Vis
    {
        private static readonly int WINDOW_X = 1920;
        private static readonly int WINDOW_Y = 1080;
        private static readonly int MAX_PER_FRAME = 1; 
        private static readonly int FPS = 30;
        private static readonly bool FOLLOW_HEAD = false;

        private Viewer _renderer = new Viewer(WINDOW_X, WINDOW_Y, FPS, "Day09");
        private ConcurrentQueue<RenderItem> _renderQueue = new ConcurrentQueue<RenderItem>();
        private bool _isRendering = false;
        
        private Rectangle[] _recs = new Rectangle[10];
        HashSet<(int,int)> _visited = new HashSet<(int, int)>();
        private List<Rectangle> _vRecs = new List<Rectangle>();

        internal class RenderItem
        {
            internal RenderItem(byte type, (int, int)[] rope)
            {
                Type = type;
                Rope = rope;
            }

            internal byte Type { get; private set; }
            internal (int, int)[] Rope { get; private set; }
        }

        internal void StartVisualiser(Action solver)
        {
            _isRendering = true;
            Task.Run(() => solver());
            _renderer.StartViewer(processFrame);
        }

        internal void AddRenderItem(RenderItem item)
        {
            if (_isRendering) _renderQueue.Enqueue(item);
        }

        internal void processFrame()
        {
            processQueue();
            int offsetX = FOLLOW_HEAD?(int)_recs[0].x * 10 : 0;
            int offsetY = FOLLOW_HEAD?(int)_recs[0].y * 10 : 0;           

            foreach (Rectangle r in _vRecs)
            {
                int x = (int)r.x * 10 + WINDOW_X / 2 - offsetX;
                int y = (int)r.y * 10 + WINDOW_Y / 2 - offsetY;
                DrawRectangle(x, y, (int)r.width, (int)r.height, ORANGE);
            }
            for (int i = 9; i >= 0; i--)
            {
                int x = (int)_recs[i].x * 10 + WINDOW_X / 2 - offsetX;
                int y = (int)_recs[i].y * 10 + WINDOW_Y / 2 - offsetY;
                DrawRectangle(x, y, (int)_recs[i].width, (int)_recs[i].height, WHITE);
                DrawText(i.ToString(), x + 2, y, 10, BLACK);
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
                            addRope(r.Rope);                            
                            break;
                    }
                }
                if (thisFrame++ >= MAX_PER_FRAME) break;
            }
        }

        private void addRope((int, int)[] rope)
        {
            for (int i = 0; i < 10; i++)
            {
                _recs[i].x = rope[i].Item1;
                _recs[i].y = rope[i].Item2;
                _recs[i].height = 10;
                _recs[i].width = 10;
                if (i == 9) addVisited(rope[i]);
            }            
        }

        private void addVisited((int, int) tail)
        {
            if (!_visited.Contains(tail))
            {
                _visited.Add(tail); 
                Rectangle r = new Rectangle(tail.Item1,tail.Item2,10,10);
                _vRecs.Add(r);
            }
            
        }
    }
}
