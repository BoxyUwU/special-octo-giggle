using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using GigglyLib.Components;

namespace GigglyLib
{
    public static class Config
    {
        private static Random _random = new Random();
        public static int TileSize = 48;
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

        public static Dictionary<string, CWeapon> Weapons = new Dictionary<string, CWeapon>
        {
            ["FlipFlop"] = new CWeapon {
                Damage = 5,
                RangeFront = 10,
                RangeBack = 1,
                RangeLeft = 4,
                RangeRight = 4,
                AttackPattern = new List<string>
                {
                " 020 ",
                "33133",
                " 020 ",
                },
                CooldownMax = 3,
            },
            ["Laser"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 10,
                RangeBack = 1,
                RangeLeft = 1,
                RangeRight = 1,
                AttackPattern = new List<string>
                {
                "  0          3 3",
                "00S011112222333 ",
                "  0          3 3"
                },
                CooldownMax = 3,
            },
            ["Shockwave"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 7,
                RangeBack = 7,
                RangeLeft = 7,
                RangeRight = 7,
                AttackPattern = new List<string>
                {
                " 444 ",
                "43234",
                "42124",
                "43234",
                " 444 ",
                },
                CooldownMax = 3,
            }
        };

        public static float Rand()
        {
            return (float)_random.NextDouble();
        }
    }
}
