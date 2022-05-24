using System;

namespace AsmForThreadsStream
{
    internal class Lamp
    {
        private struct LampData
        {
            public int X, Y, W, H;
            public uint Color;
        }

        private static LampData[] lamps = new LampData[]
        {
            new LampData() { X = 20, Y = 20, W = 150, H = 150, Color = 0xFFFF0000 },
            new LampData() { X = 200, Y = 200, W = 150, H = 150, Color = 0xFF32CD32 },
            new LampData() { X = 400, Y = 200, W = 150, H = 150, Color = 0xFF0000FF },
            new LampData() { X = 0, Y = 300, W = 150, H = 150, Color = 0xFFFFFF00 },
        };

        private static int _index;

        public static NConsoleGraphics.ConsoleGraphics graphics = new NConsoleGraphics.ConsoleGraphics();

        private LampData _data;

        public Lamp()
        {
            Console.CursorVisible = false;
            _data = lamps[_index];
            _index++;
        }

        public void TurnOn()
        {
            graphics.FillRectangle(_data.Color, _data.X, _data.Y, _data.W, _data.H);
            graphics.FlipPages();
        }

        public void TurnOff()
        {
            graphics.FillRectangle(0xFF000000, _data.X, _data.Y, _data.W, _data.H);
            graphics.FlipPages();
        }
    }
}
