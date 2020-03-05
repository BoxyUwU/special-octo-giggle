using System;
using System.Collections.Generic;
using System.IO;
using DefaultEcs;
using GigglyLib.Components;

namespace GigglyLib.ProcGen
{
    public class MapGenerator
    {
        World _world;
        int _seed;
        MetaballGenerator _metaballGen;
        CAGenerator _CAGen;
        BSPGenerator _BSPGen;

        public MapGenerator(World world, int seed) { _world = world; _seed = seed; }

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

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/");
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/" + "myMap" + ".txt";
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(debugOutput);
            streamWriter.Close();

            CreateSprites(tiles);
            SpawnEnemy(3, -20, Direction.SOUTH);
            SpawnEnemy(-4, -16, Direction.SOUTH);
            SpawnEnemy(5, 5);
            SpawnEnemy(10, 7);
            SpawnEnemy(-7, 2, Direction.EAST);
        }

        private void CreateSprites(bool[,] tiles)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (!tiles[x, y])
                        continue;
                    var tileEntity = _world.CreateEntity();
                    tileEntity.Set(new CSprite { Texture = Config.Textures["asteroid"], Depth = 0.49f, X = x * Config.TileSize, Y = y * Config.TileSize });
                    tileEntity.Set(new CGridPosition { X = x, Y = y });
                }
        }

        public Entity SpawnEnemy(int gX, int gY, Direction dir = Direction.WEST)
        {
            var e = _world.CreateEntity();
            e.Set<CEnemy>();
            e.Set(new CGridPosition { X = gX, Y = gY, Facing = dir });
            e.Set<CMovable>();
            e.Set(new CHealth { Max = 15 });
            e.Set(Config.Weapons[new List<string>(Config.Weapons.Keys)[new Random().Next(0, Config.Weapons.Count)]]);
            e.Set(new CSprite { Texture = Config.Textures["enemy"], Depth = 0.25f, X = gX * Config.TileSize, Y = gY * Config.TileSize, });
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
