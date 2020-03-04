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

        public MapGenerator(World world, int seed) { _world = world; _seed = seed; }

        public void Generate()
        {
            _metaballGen = new MetaballGenerator(20f, 0.95f, _seed, 2, 4, angleVariance: 3.141f / 2f, angleVarianceDeadzone: 1f);
            bool[,] tiles = _metaballGen.Generate();

            //DebugOutput(tiles, 1);

            for (int x = 0; x < tiles.GetLength(0); x++)
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (!tiles[x, y])
                        continue;
                    var tileEntity = _world.CreateEntity();
                    tileEntity.Set(new CSprite { Texture = Config.Textures["asteroid"], Depth = 0.49f, X = x * Config.TileSize, Y = y * Config.TileSize });
                    tileEntity.Set(new CGridPosition { X = x, Y = y });
                }

            SpawnEnemy(3, -20, Direction.SOUTH);
            SpawnEnemy(-4, -16, Direction.SOUTH);
            SpawnEnemy(5, 5);
            SpawnEnemy(10, 7);
            SpawnEnemy(-7, 2, Direction.EAST);
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

        private void DebugOutput(bool[,] tileGrid = null, int iterations = 1)
        {   
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/");
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/" + "myMap" + ".txt";
            StreamWriter streamWriter = new StreamWriter(filePath);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                bool[,] tilelist;
                if (iteration > 0 || tileGrid == null)
                    tilelist = _metaballGen.Generate();
                else
                    tilelist = (bool[,])tileGrid.Clone();

                for (int y = 0; y < tilelist.GetLength(1); y++)
                {
                    for (int x = 0; x < tilelist.GetLength(0); x++)
                    {
                        if (tilelist[x, y])
                            streamWriter.Write("##");
                        else
                            streamWriter.Write("  ");
                    }
                    streamWriter.WriteLine();
                }
                streamWriter.Write("\n\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~#\n\n");
            }

            streamWriter.Close();
        }
    }
}
