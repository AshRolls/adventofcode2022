using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Collections.Concurrent;

namespace AdventOfCode.vis
{
    internal class Day08Vis
    {                
        private Viewer _renderer = new Viewer(990, 990, 30, "Day08");
        private ConcurrentQueue<RenderItem> _renderQueue = new ConcurrentQueue<RenderItem>();
        private readonly int MAX_PER_FRAME = 30;        
        private Rectangle[,] _recs = new Rectangle[99, 99];

        internal void StartVisualiser(Action solver)
        {
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
            for (int x = 0; x < 99; x++)
            {
                for (int y = 0; y < 99; y++)
                {
                    DrawRectangle((int)_recs[x, y].x, (int)_recs[x, y].y, (int)_recs[x, y].width, (int)_recs[x, y].height, DARKGREEN);
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
                            _recs[r.X, r.Y].x = r.X * 10;
                            _recs[r.X, r.Y].y = r.Y * 10;
                            _recs[r.X, r.Y].width = 10;
                            _recs[r.X, r.Y].height = 10;
                            break;
                    }
                }
                if (thisFrame++ >= MAX_PER_FRAME) break;
            }
        }
    }
}
