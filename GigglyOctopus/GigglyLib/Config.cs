using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GigglyLib
{
    public static class Config
    {
        private static Random _random = new Random();
        public static int TileSize = 48;
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public static Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, SoundEffect> SFX = new Dictionary<string, SoundEffect>();

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
                "  441  ",
                " 33 41 ",
                "3223412",
                "2 111 2",
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
                CooldownMax = 6,
            },
            ["Claws"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 3,
                RangeBack = 0,
                RangeLeft = 1,
                RangeRight = 1,
                AttackPattern = new List<string>
                {
                    "    1 1 1 ",
                    " 2 2121 1 ",
                    "  212121  ",
                    "S 121212  ",
                    " 1 1212 2 ",
                    "1 1 12 2 2",
                    "     2 2 2",
                },
                CooldownMax = 4,
            },
            ["Railgun"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 25,
                RangeBack = -2,
                RangeLeft = 0,
                RangeRight = 0,
                AttackPattern = new List<string>
                {
                    " 0                       ",
                    "  1                     3",
                    "S  233333333333333333333 ",
                    "  1                     3",
                    " 0                       ",
                },
                CooldownMax = 15,
            },
            ["ClusterBombs"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 5,
                RangeBack =  5,
                RangeLeft =  5,
                RangeRight = 5,
                AttackPattern = new List<string>
                {
                    "      3      ",
                    "     333     ",
                    "    33233    ",
                    "  5  333  1  ",
                    " 555  3  111 ",
                    "55455   11011",
                    " 555     111 ",
                    "  5   S   1  ",
                    "   2     4   ",
                    "  222   444  ",
                    " 22122 44344 ",
                    "  222   444  ",
                    "   2     4   ",
                },
                CooldownMax = 20,
            },
            ["Cage"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 6,
                RangeBack = 0,
                RangeLeft = 6,
                RangeRight = 6,
                AttackPattern = new List<string>
                {
                    "    3    ",
                    "   333   ",
                    "  33133  ",
                    " 33   33 ",
                    "331   133",
                    " 33   33 ",
                    "  33133  ",
                    "   333   ",
                    "    3    ",
                },
                CooldownMax = 10,
            },
            ["MineLayer"] = new CWeapon
            {
                Damage = 5,
                RangeFront = -2,
                RangeBack = 7,
                RangeLeft = 2,
                RangeRight = 2,
                AttackPattern = new List<string>
                {
                    "  5  ",
                    " 555 ",
                    "55S55",
                    " 555 ",
                    "  5  ",
                },
                CooldownMax = 2,
            },
            ["Forcefield"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 4,
                RangeBack = 4,
                RangeLeft = 4,
                RangeRight = 4,
                AttackPattern = new List<string>
                {
                    " 000 ",
                    "0   0",
                    "0 S 0",
                    "0   0",
                    " 000 ",
                },
                CooldownMax = 0,
            },
            ["Starburst"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 10,
                RangeBack = -4,
                RangeLeft = 1,
                RangeRight = 1,
                AttackPattern = new List<string>
                {
                    "   2   ",
                    " 1 2 1 ",
                    "  121  ",
                    "2222222",
                    "  121  ",
                    " 1 2 1 ",
                    "   2   ",
                },
                CooldownMax = 4,
            },
            ["TwinLasers"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 15,
                RangeBack = -4,
                RangeLeft = 1,
                RangeRight = 1,
                AttackPattern = new List<string>
                {
                    "0                      ",
                    " 0011111222223333344444",
                    "0S                     ",
                    " 0011111222223333344444",
                    "0                      ",
                },
                CooldownMax = 15,
            },
            ["Heart"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 13,
                RangeBack = -4,
                RangeLeft = 5,
                RangeRight = 5,
                AttackPattern = new List<string>
                {
                    "     0000  ",
                    "    055550 ",
                    "   05555550",
                    "  055555550",
                    " 055555550 ",
                    "055555550  ",
                    " 055555550 ",
                    "  055555550",
                    "   05555550",
                    "    055550 ",
                    "     0000  ",
                },
                CooldownMax = 16,
            },
            ["LightningStorm"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 5,
                RangeBack = 5,
                RangeLeft = 5,
                RangeRight = 5,
                AttackPattern = new List<string>
                {
                    "      3       3      ",
                    "      33     33      ",
                    "       3     3       ",
                    "        2   2        ",
                    "       22   22       ",
                    "       2     2       ",
                    "33      1   1      33",
                    " 33 22  11 11  22 33 ",
                    "   22 11 1 1 11 22   ",
                    "       110 011       ",
                    "          S          ",
                    "       110 011       ",
                    "   22 11 1 1 11 22   ",
                    " 33 22  11 11  22 33 ",
                    "33      1   1      33",
                    "       2     2       ",
                    "       22   22       ",
                    "        2   2        ",
                    "       3     3       ",
                    "      33     33      ",
                    "      3       3      ",
                },
                CooldownMax = 20,
            },
            ["Creeper"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 1,
                RangeBack = 1,
                RangeLeft = 6,
                RangeRight = 6,
                AttackPattern = new List<string>
                {
                    " 443323344 ",
                    " 5 9 2 9 5 ",
                    " 566919665 ",
                    "   79197   ",
                    "   79097   ",
                    "9  88088  9",
                    " 9990S0999 ",
                    "9  88088  9",
                    "   79097   ",
                    "   79197   ",
                    " 566919665 ",
                    " 5 9 2 9 5 ",
                    " 443323344 ",
                },
                CooldownMax = 12,
            },
            ["PrivateSquare"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 7,
                RangeBack = -1,
                RangeLeft = 3,
                RangeRight = 3,
                AttackPattern = new List<string>
                {
                    "              7            ",
                    "              7            ",
                    "                           ",
                    "    7                      ",
                    "     7        6        7   ",
                    "              6       7    ",
                    "       6                   ",
                    "        6           6      ",
                    "              5    6       ",
                    "          5 0 5            ",
                    "           50  0 5         ",
                    "          0 4 0 5          ",
                    "77  66  55 0444400         ",
                    "            4 4            ",
                    "         0044440 55  66  77",
                    "          5 0 4 0          ",
                    "         5 0  0            ",
                    "            5 0 5          ",
                    "       6    5    5         ",
                    "      6                    ",
                    "                   6       ",
                    "    7       6       6      ",
                    "   7        6              ",
                    "                      7    ",
                    "                       7   ",
                    "            7              ",
                    "            7              ",
                },
                CooldownMax = 24,
            },
            ["Flamethrower"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 4,
                RangeBack = 0,
                RangeLeft = 1,
                RangeRight = 1,
                AttackPattern = new List<string>
                {
                    "       12",
                    "0   1212 ",
                    "S011212 2",
                    "0   1212 ",
                    "       12",
                },
                CooldownMax = 0,
            },
            ["Sukima"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 13,
                RangeBack = 13,
                RangeLeft = 0,
                RangeRight = 0,
                AttackPattern = new List<string>
                {
                    "     22222     ",
                    "   227456822   ",
                    "222131819151222",
                    "   229674322   ",
                    "     22222     ",
                },
                CooldownMax = 13,
            },
            ["Wings"] = new CWeapon
            {
                Damage = 5,
                RangeFront = 3,
                RangeBack = -1,
                RangeLeft = 5,
                RangeRight = 5,
                AttackPattern = new List<string>
                {
                    "        4",
                    "     3  4",
                    "  2   3 4",
                    "1   2 3 4",
                    "   1 234 ",
                    " 0  123  ",
                    "  0 12   ",
                    " 0 1     ",
                    "S        ",
                    " 0 1     ",
                    "  0 12   ",
                    " 0  123  ",
                    "   1 234 ",
                    "1   2 3 4",
                    "  2   3 4",
                    "     3  4",
                    "        4",
                },
                CooldownMax = 15,
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
