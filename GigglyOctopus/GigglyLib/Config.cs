using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigglyLib
{
    public static class Config
    {
        private static Random _random = new Random();
        public static float TileSize = 48;
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;

        public static float Rand()
        {
            return (float)_random.NextDouble();
        }
    }
}
