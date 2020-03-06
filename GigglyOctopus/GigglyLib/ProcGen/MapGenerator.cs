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
        BSPGenerator _BSPGen;

        public MapGenerator(int seed) {_seed = seed;}

        public void Generate()
        {
            _metaballGen = new MetaballGenerator(20f, 0.95f, _seed, 2, 4, angleVariance: 3.141f / 2f, angleVarianceDeadzone: 1f);
            _CAGen = new CAGenerator(_seed);
            _BSPGen = new BSPGenerator();

            string debugOutput = "";
            bool[,] tiles = null;

            // actual map gen code
            for (int i = 0; i < 1; i++)
            {
                tiles = _metaballGen.Generate();
                tiles = _CAGen.DoSimulationStep(tiles, 5);
                _BSPGen.Generate(tiles);

                debugOutput += DebugOutput(tiles);
            }

            /*Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/");
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/" + "myMap" + ".txt";
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(debugOutput);
            streamWriter.Close();*/

            CreateSprites(tiles);
            SpawnEnemy(3, -20, Direction.SOUTH);
            SpawnEnemy(-4, -16, Direction.SOUTH);
            SpawnEnemy(5, 5);
            SpawnEnemy(10, 7);
            SpawnEnemy(-7, 2, Direction.EAST);
            CreatePlayer();
        }

        private void CreatePlayer()
        {
            var _player = Game1.world.CreateEntity();
            Game1._player = _player;
            _player.Set(new CPlayer());
            _player.Set(new CGridPosition());
            _player.Set(new CMovable());
            _player.Set(new CHealth
            {
                Max = 30
            });
            _player.Set(new CSprite
            {
                Texture = "player",
                Transparency = 0.1f,
                Depth = 0.5f
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
                RangeFront = 7,
                RangeLeft = 2,
                RangeRight = 2,
                RangeBack = -1,
                CooldownMax = 0,
                AttackPattern = new List<string>
                    {
                       "0"
                    },
                Colour = (Colour)Config.RandInt(18),
                RandomColours = true
            });

            // FOR TESTING
            weapons.Weapons.Add(Config.Weapons["Cage"]);

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
            e.Set(new CGridPosition { X = gX, Y = gY, Facing = dir });
            e.Set<CMovable>();
            e.Set(new CHealth { Max = 15 });
            var weapon = Config.Weapons[new List<string>(Config.Weapons.Keys)[Config.RandInt(Config.Weapons.Count)]];
            weapon.Colour = (Colour)Config.RandInt(18);
            var weapons = new List<CWeapon>();
            weapons.Add(weapon);
            e.Set(new CWeaponsArray { Weapons = weapons });
            e.Set(new CEnemy
            {
                HasPowerUp = hasPowerUp
            });
            if (hasPowerUp)
            {
                e.Set(new CParticleSpawner
                {
                    Texture = PARTICLES[(int)weapons[0].Colour],
                    Impact = 1.0f
                });
            }
            e.Set(new CSprite { 
                Texture = PARTICLES[(int)weapon.Colour].Replace("particles", "enemy"), 
                Depth = 0.25f, 
                X = gX * Config.TileSize, 
                Y = gY * Config.TileSize,
                Transparency = 0.05f
            });
            e.Set(new CScalable{ Scale = 1.5f });
            e.Set(new CSourceRectangle
            {
                Rectangle = new Rectangle
                {
                    X = Config.RandInt(10) * Config.TileSize,
                    Y = Config.RandInt(10) * Config.TileSize,
                    Height = Config.TileSize,
                    Width = Config.TileSize
                }
            });
            return e;
        }

        private string DebugOutput(bool[,] tileGrid)
        {
            string output = "";
            for (int y = 0; y < tileGrid.GetLength(1); y++)
            {
                for (int x = 0; x < tileGrid.GetLength(0); x++)
                {
                    if (tileGrid[x, y])
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
