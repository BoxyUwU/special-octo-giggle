using System;
using System.Collections.Generic;
using System.IO;
using DefaultEcs;
using GigglyLib.Components;
using static GigglyLib.Game1;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.ProcGen
{
    public class MapGenerator
    {
        int _seed;
        MetaballGenerator _metaballGen;
        CAGenerator _CAGen;
        RoomGenerator _RoomGen;
        Random _rand;

        public MapGenerator(int seed) 
        {
            _seed = seed;
            _rand = new Random(seed);
        }

        public void Generate()
        {
            _metaballGen = new MetaballGenerator(30f, 0.90f, _seed, 2, 4, angleVariance: 3.141f / 2f, angleVarianceDeadzone: 1f);
            _CAGen = new CAGenerator(_seed);
            _RoomGen = new RoomGenerator(_seed);

            string debugOutput = "";
            bool[,] tiles = null;
            List<Room> rooms = null;
            // actual map gen code
            for (int i = 0; i < 1; i++)
            {
                tiles = _metaballGen.Generate();
                tiles = _CAGen.DoSimulationStep(tiles, 5, 0);
                rooms = _RoomGen.Generate(tiles);
                tiles = _CAGen.DoSimulationStep(tiles, 1, 1);
                debugOutput += DebugOutput(tiles);
            }

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/");
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/" + "myMap" + ".txt";
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(debugOutput);
            streamWriter.Close();

            var room = rooms[_rand.Next(0, rooms.Count)];
            var region = room.Region;
            int x = _rand.Next(region.X, region.X + region.Width);
            int y = _rand.Next(region.Y, region.Y + region.Height);
            CreatePlayer(x, y);

            CreateSprites(tiles);
            //SpawnEnemy(3, -20, Direction.SOUTH);
            //SpawnEnemy(-4, -16, Direction.SOUTH);
            //SpawnEnemy(5, 5);
            //SpawnEnemy(10, 7);
            //SpawnEnemy(-7, 2, Direction.EAST);
        }

        private void CreatePlayer(int x, int y)
        {
            var _player = Game1.world.CreateEntity();
            Game1._player = _player;
            _player.Set(new CPlayer());
            _player.Set(new CGridPosition { X = x, Y = y });
            _player.Set(new CMovable());
            _player.Set(new CHealth
            {
                Max = 30
            });
            _player.Set(new CSprite
            {
                Texture = "player",
                Transparency = 0.1f,
                Depth = 0.5f,
                X = x * Config.TileSize,
                Y = y * Config.TileSize
            });
            _player.Set(new CParticleSpawner
            {
                Texture = "particles-rainbow",
                Impact = 1.0f
            });
            var weapons = new CWeaponsArray
            {
                Weapons = new List<CWeapon>()
            };
            weapons.Weapons.Add(new CWeapon
            {
                Damage = 5,
                RangeFront = 6,
                RangeLeft = 2,
                RangeRight = 2,
                RangeBack = 1,
                CooldownMax = 0,
                AttackPattern = new List<string>
                    {
                       "0"
                    },
                Colour = (Colour)Config.RandInt(18),
                RandomColours = true
            });

            //////////////////////////////////////////////////
            //                  GODMODE +w+                 //
            //////////////////////////////////////////////////
            //foreach (var w in Config.Weapons.Values)      //
            //{                                             //
            //    var _w = w;                               //
            //    _w.Colour = (Colour)Config.RandInt(18);   //
            //    weapons.Weapons.Add(_w);                  //
            //}                                             //
            //////////////////////////////////////////////////
            _player.Set(weapons);
        }

        private void CreateSprites(bool[,] tiles)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (!tiles[x, y])
                        continue;
                    var tileEntity = Game1.world.CreateEntity();
                    tileEntity.Set(new CSprite { Texture = "asteroid", Depth = 0.49f, X = x * Config.TileSize, Y = y * Config.TileSize });
                    tileEntity.Set(new CGridPosition { X = x, Y = y });
                }
        }

        public Entity SpawnEnemy(int gX, int gY, Direction dir = Direction.WEST)
        {
            var e = Game1.world.CreateEntity();
            bool hasPowerUp = Config.Rand() < 0.2;
            e.Set(new CEnemy { 
                HasPowerUp = hasPowerUp
            });
            e.Set(new CGridPosition { X = gX, Y = gY, Facing = dir });
            e.Set<CMovable>();
            e.Set(new CHealth { Max = 15 });
            var weapon = Config.Weapons[new List<string>(Config.Weapons.Keys)[Config.RandInt(Config.Weapons.Count)]];
            weapon.Colour = (Colour)Config.RandInt(18);
            var weapons = new List<CWeapon>();
            weapons.Add(weapon);
            e.Set(new CWeaponsArray { Weapons = weapons });
            if (hasPowerUp)
            {
                e.Set(new CParticleSpawner
                {
                    Texture = PARTICLES[(int)weapons[0].Colour],
                    Impact = 1.0f
                });
            }
            e.Set(new CSprite { Texture = "enemy", Depth = 0.25f, X = gX * Config.TileSize, Y = gY * Config.TileSize, });
            return e;
        }

        private string DebugOutput(bool[,] tileGrid, List<BSPSplit> BSPLeafs = null)
        {
            string output = "";
            for (int y = 0; y < tileGrid.GetLength(1); y++)
            {
                for (int x = 0; x < tileGrid.GetLength(0); x++)
                {
                    string leaf = "";
                    if (BSPLeafs != null)
                        for (int i = 0; i < BSPLeafs.Count; i++)
                            if (BSPLeafs[i].Region[x, y])
                            {
                                leaf = i.ToString();
                                if (leaf.Length == 1)
                                    leaf = "0" + leaf;
                            }
                    if (leaf != "")
                        output += leaf;
                    else if (tileGrid[x, y])
                        output += "##";
                    else
                        output += "  ";
                }
                output += "\n";
            }
            output += "\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n\n";
            return output;
        }
    }
}
