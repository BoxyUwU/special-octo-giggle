using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using GigglyLib.ProcGen;

namespace GigglyLib.Systems
{
    public class AISys : AEntitySystem<float>
    {
        public AISys()
            : base(Game1.world.GetEntities().With<CEnemy>().With<CMovable>().With<CGridPosition>().AsSet())
        {
        }

        public IEnumerator<CMoveAction> MoveGenerator(Entity enemy, Entity player)
        {
            var NORTH = new CMoveAction { DistX = 0, DistY = -1 };
            var EAST = new CMoveAction { DistX = 1, DistY = 0 };
            var SOUTH = new CMoveAction { DistX = 0, DistY = 1 };
            var WEST = new CMoveAction { DistX = -1, DistY = 0 };
            var NONE = new CMoveAction { DistX = 0, DistY = 0 };

            /////////////////////////
            /////////////////////////
            /// AI CODE GOES HERE ///
            /////////////////////////
            /////////////////////////
            while (true)
            {
                int patrolWidth = 5+Game1.GameStateRandom.Next(20);
                int patrolHeight = 5+ Game1.GameStateRandom.Next(20);
                int patrolDir = Game1.GameStateRandom.Next(1) == 0 ? 1 : -1;
                int patrol = (int)enemy.Get<CGridPosition>().Facing;

                int retries = 0;
                // We needa use the function to recalculate every time since this is a generator
                while (Math.Abs(player.Get<CGridPosition>().X - enemy.Get<CGridPosition>().X) > 10
                   || Math.Abs(player.Get<CGridPosition>().Y - enemy.Get<CGridPosition>().Y) > 10)
                {
                    switch (patrol)
                    {
                        case 0:
                            for (int i = 0; i < patrolHeight; ++i)
                                if (!Game1.Tiles.Contains((enemy.Get<CGridPosition>().X, enemy.Get<CGridPosition>().Y - 1)))
                                {
                                    retries = 0;
                                    yield return NORTH;
                                }
                                else break;
                            break;
                        case 1:
                            for (int i = 0; i < patrolWidth; ++i)
                                if (!Game1.Tiles.Contains((enemy.Get<CGridPosition>().X + 1, enemy.Get<CGridPosition>().Y)))
                                {
                                    retries = 0;
                                    yield return EAST;
                                }
                                else break;
                            break;
                        case 2:
                            for (int i = 0; i < patrolHeight; ++i)
                                if (!Game1.Tiles.Contains((enemy.Get<CGridPosition>().X, enemy.Get<CGridPosition>().Y + 1)))
                                {
                                    retries = 0;
                                    yield return SOUTH;
                                }
                                else break;
                            break;
                        case 3:
                            for (int i = 0; i < patrolWidth; ++i)
                                if (!Game1.Tiles.Contains((enemy.Get<CGridPosition>().X - 1, enemy.Get<CGridPosition>().Y)))
                                {
                                    retries = 0;
                                    yield return WEST;
                                }
                                else break;
                            break;
                    }
                    patrol += patrolDir;
                    patrol +=
                        patrol > 3 ? -4 :
                        patrol < 0 ? +4 :
                        0;

                    retries++;
                    if (retries > 3)
                    {
                        var toDispose = new List<Entity>();
                        toDispose.Add(enemy);
                        foreach (var e in toDispose)
                            e.Dispose();
                        yield return NONE;
                    }
                }

                AStar astar = new AStar();
                var path = astar.GetPath(
                    enemy.Get<CGridPosition>().X,
                    enemy.Get<CGridPosition>().Y,
                    player.Get<CGridPosition>().X,
                    player.Get<CGridPosition>().Y,
                    Game1.CostGrid
                );
                path.Reverse();

                foreach (var point in path)
                {
                    if (Math.Abs(player.Get<CGridPosition>().X - enemy.Get<CGridPosition>().X) > 12
                      || Math.Abs(player.Get<CGridPosition>().Y - enemy.Get<CGridPosition>().Y) > 12)
                    break;
                    yield return new CMoveAction
                    {
                        DistX = point.x - enemy.Get<CGridPosition>().X,
                        DistY = point.y - enemy.Get<CGridPosition>().Y,
                    };
                }

                int forwardRange = enemy.Get<CWeaponsArray>().Weapons[0].RangeFront / 2;

                while (Math.Abs(player.Get<CGridPosition>().X - enemy.Get<CGridPosition>().X) < Math.Max(forwardRange,2)
                   || Math.Abs(player.Get<CGridPosition>().Y - enemy.Get<CGridPosition>().Y) < Math.Max(forwardRange, 2))
                {
                    var playerPos = player.Get<CGridPosition>();
                    var enemyPos = enemy.Get<CGridPosition>();

                    CMoveAction next;

                    if (Math.Abs(playerPos.Y - enemyPos.Y) >= Math.Abs(playerPos.X - enemyPos.X))
                        next =
                            playerPos.X < enemyPos.X ? EAST :
                            playerPos.X > enemyPos.X ? WEST :
                            playerPos.Y < enemyPos.Y ? SOUTH :
                            playerPos.Y > enemyPos.Y ? NORTH :
                            NONE;


                    else next =
                        playerPos.Y < enemyPos.Y ? SOUTH :
                        playerPos.Y > enemyPos.Y ? NORTH :
                        playerPos.X < enemyPos.X ? EAST :
                        playerPos.X > enemyPos.X ? WEST :
                        NONE;

                    if (Game1.Tiles.Contains((enemyPos.X + next.DistX, enemyPos.Y + next.DistY)))
                        break;
                    else yield return next;
                }

            }
        }

        protected override void Update(float state, in Entity entity)
        {
            if (!entity.Has<CAIScript>())
                entity.Set(new CAIScript { AI = MoveGenerator(entity, Game1.Player) });

            var script = entity.Get<CAIScript>();
            script.AI.MoveNext();
            entity.Set(script.AI.Current);

            base.Update(state, entity);
        }
    }
}
