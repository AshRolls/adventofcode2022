using Raylib_cs;
using static Raylib_cs.Raylib;
using static Raylib_cs.Color;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace AdventOfCode.vis
{
    static class ViewerOptions
    {
        public static bool recordVideo = false;
    }

    public class Viewer
    {
        int width, height;
        FFWriter ff_writer;
        string title;
        Stopwatch stopwatch = new Stopwatch();

        public Viewer(int w, int h, int fps, string t)
        {
            Console.WriteLine("Initializing viewer: " + w + "x" + h + " @" + fps + "fps");
            width = w; height = h; title = t;
            InitWindow(width, height, "AOC2022 " + title);
            SetTargetFPS(fps);
            ff_writer = new FFWriter(width, height, fps, title);
        }

        public async void StartViewer(Action render)
        {
            int cnt = 0;
            var ff_task = Task.Run(() => { if (ViewerOptions.recordVideo) return ff_writer.run(); else return true; });
            stopwatch.Start();
            long lastts = 0;
            long lastcnt = 0;
            while (!WindowShouldClose())
            {
                BeginDrawing();
                ClearBackground(BLACK);
                render();
                cnt++;
                EndDrawing();
                if (ViewerOptions.recordVideo)
                {
                    Image screen = LoadImageFromScreen();
                    unsafe
                    {
                        ff_writer.addRawImage(screen.data);
                        MemFree(screen.data);
                    }
                }
                long ts = stopwatch.ElapsedMilliseconds;
                if (ts > lastts + 4999)
                {
                    Console.WriteLine("Rendered " + (cnt - lastcnt) + " frames in " + (ts - lastts) + " ms. " + " AFPS=" + ((cnt * 1000 + 500) / ts));
                    lastcnt = cnt;
                    lastts = ts;
                }
            }
            CloseWindow();
            if (ViewerOptions.recordVideo) ff_writer.finish();
            _ = await ff_task;
        }
    }
}
