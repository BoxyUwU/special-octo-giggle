using System;
using System.Collections.Generic;
using DefaultEcs;
using GigglyLib.Components;
using static GigglyLib.Game1;

namespace GigglyLib.ProcGen
{
    public class MapGenerator
    {
        World _world;
        public MapGenerator(World world) { _world = world; }

        public void Generate()
        {
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
            var weapon = Config.Weapons[new List<string>(Config.Weapons.Keys)[Config.RandInt(Config.Weapons.Count)]];
            weapon.Colour = (Colour)Config.RandInt(18);
            e.Set(weapon);
            e.Set(new CSprite { Texture = Config.Textures["enemy"], Depth = 0.25f, X = gX * Config.TileSize, Y = gY * Config.TileSize, });
            return e;
        }
    }
}
