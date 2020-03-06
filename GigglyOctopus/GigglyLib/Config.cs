using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using GigglyLib.Components;
using Microsoft.Xna.Framework;

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
            ["FlipFlop"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 10,
                RangeBack = -1,
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
                RangeLeft = 0,
                RangeRight = 0,
                AttackPattern = new List<string>
                {
                "  0          3 3",
                "00S011112222333 ",
                "  0          3 3"
                },
                CooldownMax = 6,
            },
            ["Shockwave"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 7,
                RangeBack = 0,
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
                CooldownMax = 5,
            },
            ["Torpedo"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 13,
                RangeBack = -3,
                RangeLeft = 2,
                RangeRight = 2,
                AttackPattern = new List<string>
                {
                    "            6   6 ",
                    "           6565656",
                    "0 0         65556 ",
                    "S0122333444455555 ",
                    "0 0         65556 ",
                    "           6565656",
                    "            6   6 ",
                },
                CooldownMax = 10,
            },
            ["Nuke"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 19,
                RangeBack = -6,
                RangeLeft = 6,
                RangeRight = 6,
                AttackPattern = new List<string>
                {
                    "          88888888         ",
                    "        888888888888        ",
                    "      8888888888888888      ",
                    "     888888888888888888     ",
                    "    88888888888888888888    ",
                    "   8888228888888888228888   ",
                    "  888822288888888882228888  ",
                    "  888222228888888822222888  ",
                    " 88822222288888888222222888 ",
                    " 88822222228888882222222888 ",
                    "8882222222228888222222222888",
                    "8882222222228888222222222888",
                    "8882222222288228822222222888",
                    "8882222222282112822222222888",
                    "8888888888882112888888888888",
                    "8888888888888228888888888888",
                    "8888888888888888888888888888",
                    "8888888888882222888888888888",
                    " 88888888888222288888888888",
                    " 88888888882222228888888888",
                    "  888888888222222888888888 ",
                    "  888888882222222288888888 ",
                    "   8888888222222228888888  ",
                    "    88888222222222288888   ",
                    "     888888222222888888    ",
                    "      8888888888888888     ",
                    "        888888888888       ",
                    "          88888888         ",
                },
                CooldownMax = 60,
            },
            ["Thunderclap"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 5,
                RangeBack = 5,
                RangeLeft = 5,
                RangeRight = 5,
                AttackPattern = new List<string>
                {
                    "     66666     ",
                    "   665555566   ",
                    "  65544444556  ",
                    " 65 4333334 56 ",
                    " 6543 222 3456 ",
                    "6543 21112 3456",
                    "654321000123456",
                    "6543210S0123456",
                    "654321000123456",
                    "6543 21112 3456",
                    " 6543 222 3456 ",
                    " 65 4333334 56 ",
                    "  65544444556  ",
                    "   665555566   ",
                    "     66666     ",
                },
                CooldownMax = 20,
            },
            ["Snowflake"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 9,
                RangeLeft = 3,
                RangeRight = 3,
                RangeBack = -2,
                CooldownMax = 12,
                AttackPattern = new List<string>
                    {
                        "   3 3   ",
                        " 3  3  3 ",
                        "3 3 2 3 3",
                        " 32 1 23 ",
                        "3  212  3",
                        " 32 1 23 ",
                        "3 3 2 3 3",
                        " 3  3  3 ",
                        "   3 3   ",
                    }
            },
            ["Vortex"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 5,
                RangeBack = 0,
                RangeLeft = 5,
                RangeRight = 5,
                AttackPattern = new List<string>
                {
                "  441 ",
                " 33 41 ",
                "3223412",
                "2 1 1 2",
                "2143223",
                " 14 33 ",
                "  144  ",
                },
                CooldownMax = 15,
            },
            ["Shotgun"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 4,
                RangeBack = 0,
                RangeLeft = 1,
                RangeRight = 1,
                AttackPattern = new List<string>
                {
                "        2 2",
                "0 0 1112 2 ",
                " S11111 2 2",
                "0 0 1112 2 ",
                "        2 2",
                },
                CooldownMax = 5,
            },
            ["Claws"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 3,
                RangeBack = 0,
                RangeLeft = 2,
                RangeRight = 2,
                AttackPattern = new List<string>
                {
                    "    0 0 0 ",
                    " 2 2020 0 ",
                    "  202020  ",
                    "S 020202  ",
                    " 0 0202 2 ",
                    "0 0 02 2 2",
                    "     2 2 2",
                },
                CooldownMax = 5,
            },
        };

        public static float Rand()
        {
            return (float)_random.NextDouble();
        }
        public static int RandInt(int max)
        {
            return _random.Next(max);
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromNonPremultiplied(v, t, p, 255);
            else if (hi == 1)
                return Color.FromNonPremultiplied(q, v, p, 255);
            else if (hi == 2)
                return Color.FromNonPremultiplied(p, v, t, 255);
            else if (hi == 3)
                return Color.FromNonPremultiplied(p, q, v, 255);
            else if (hi == 4)
                return Color.FromNonPremultiplied(t, p, v, 255);
            else
                return Color.FromNonPremultiplied(v, p, q, 255);
        }
    }
}
