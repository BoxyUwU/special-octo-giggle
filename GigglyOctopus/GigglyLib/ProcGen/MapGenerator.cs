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
            var tiles = _metaballGen.Generate();
            DebugOutput(tiles, 10);
            foreach (var tile in tiles)
            {
                var blah = _world.CreateEntity();
                blah.Set(new CSprite { Texture = Config.Textures["asteroid"], Depth = 0.49f, X = tile.x * Config.TileSize, Y = tile.y * Config.TileSize });
                blah.Set(new CGridPosition { X = tile.x, Y = tile.y });
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

        private void DebugOutput(List<(int x, int y)> tiles = null, int iterations = 1)
        {   
            bool IsWall(List<(int x, int y)> tilelist, int x, int y)
            {
                for (int i = 0; i < tilelist.Count; i++)
                {
                    if (tilelist[i].x == x && tilelist[i].y == y)
                        return true;
                }
                return false;
            }

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/");
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/Maps/" + "myMap" + ".txt";
            StreamWriter streamWriter = new StreamWriter(filePath);

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                List<(int x, int y)> tilelist;
                if (iteration > 0 || tiles == null)
                    tilelist = _metaballGen.Generate();
                else
                {
                    tilelist = new List<(int x, int y)>(tiles);
                }

                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;
                for (int i = 0; i < tilelist.Count; i++)
                {
                    var (x, y) = tilelist[i];
                    if (x > maxX)
                        maxX = x;
                    if (y > maxY)
                        maxY = y;
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                }

                for (int y = minY; y <= maxY; y++)
                {
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (IsWall(tilelist, x, y))
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
