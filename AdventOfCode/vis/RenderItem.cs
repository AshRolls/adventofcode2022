using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.vis
{
    internal class RenderItem
    {       
        internal RenderItem(byte type, int x, int y)
        {
            Type = type;
            X = x;
            Y = y;
        }

        internal byte Type { get; private set; }
        internal int X { get; private set; }
        internal int Y { get; private set; }
    }
}
