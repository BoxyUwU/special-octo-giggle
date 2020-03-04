using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class AttackActionSys : AEntitySystem<float>
    {
        World _world;

        public AttackActionSys(World world)
            : base(world.GetEntities().With<CWeapon>().With<CGridPosition>().With<CSimTurn>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CGridPosition>();
            ref var weapon = ref entity.Get<CWeapon>();

            if (weapon.Cooldown < weapon.CooldownMax)
            {
                weapon.Cooldown++;
                return;
            }

            var targetBuilder = _world.GetEntities().With<CGridPosition>();

            if (entity.Has<CEnemy>())
                targetBuilder.With<CPlayer>();
            if (entity.Has<CPlayer>())
                targetBuilder.With<CEnemy>();
            var targetSet = targetBuilder.AsSet().GetEntities();

            // All Inclusive
            int xMin =
                pos.Facing == Direction.NORTH ? pos.X - weapon.RangeLeft :
                pos.Facing == Direction.EAST ? pos.X - weapon.RangeBack :
                pos.Facing == Direction.SOUTH ? pos.X - weapon.RangeRight :
                pos.Facing == Direction.WEST ? pos.X - weapon.RangeFront :
                0;

            int xMax =
                pos.Facing == Direction.NORTH ? pos.X + weapon.RangeRight :
                pos.Facing == Direction.EAST ? pos.X + weapon.RangeFront :
                pos.Facing == Direction.SOUTH ? pos.X + weapon.RangeLeft :
                pos.Facing == Direction.WEST ? pos.X + weapon.RangeBack :
                0;

            int yMin =
                pos.Facing == Direction.NORTH ? pos.Y - weapon.RangeFront :
                pos.Facing == Direction.EAST ? pos.Y - weapon.RangeLeft :
                pos.Facing == Direction.SOUTH ? pos.Y - weapon.RangeBack :
                pos.Facing == Direction.WEST ? pos.Y - weapon.RangeRight :
                0;

            int yMax =
                pos.Facing == Direction.NORTH ? pos.Y + weapon.RangeBack :
                pos.Facing == Direction.EAST ? pos.Y + weapon.RangeRight :
                pos.Facing == Direction.SOUTH ? pos.Y + weapon.RangeFront :
                pos.Facing == Direction.WEST ? pos.Y + weapon.RangeLeft :
                0;

            List<Entity> validTargets = new List<Entity>(); 

            for (int i = 0; i < targetSet.Length; i++)
            {
                var targetPos = targetSet[i].Get<CGridPosition>();

                bool valid =
                    targetPos.X >= xMin &&
                    targetPos.X <= xMax &&
                    targetPos.Y >= yMin &&
                    targetPos.Y <= yMax;

                if (valid)
                {
                    validTargets.Add(targetSet[i]);
                }
            }
            Direction dir = pos.Facing;
            float posX = pos.X;
            float posY = pos.Y;
            // Prioritize Later
            validTargets.Sort((a, b) => {
                var aPos = a.Get<CGridPosition>();
                var bPos = b.Get<CGridPosition>();
                // Priority 1: Front > Behind

                if (dir == Direction.NORTH)
                {
                    if (aPos.Y <= posY && bPos.Y > posY)
                        return -1;
                }

                if (dir == Direction.EAST)
                {
                    if (aPos.X >= posX && bPos.X < posX)
                        return -1;
                }

                if (dir == Direction.SOUTH)
                {
                    if (aPos.Y >= posY && bPos.Y < posY)
                        return -1;
                }

                if (dir == Direction.WEST)
                {
                    if (aPos.X <= posX && bPos.X > posX)
                        return -1;
                }

                // Else Distance
                return (int) (Math.Abs(posX - aPos.X) + Math.Abs(posY - aPos.Y) - Math.Abs(posX - bPos.X) - Math.Abs(posY - bPos.Y));
            });

            if (validTargets.Count != 0)
            {
                for (int i = 0; i < Math.Min(validTargets.Count, weapon.AdditionalTargets + 1); i++)
                {

                    bool targeted = true;

                    int attackLength = weapon.AttackPattern[0].Length;
                    int attackWidth = weapon.AttackPattern.Count;
                    int offsetLength = attackLength / 2;
                    int offsetWidth = attackWidth / 2;

                    for (int j = 0; j < weapon.AttackPattern.Count; j++)
                    {
                        if (weapon.AttackPattern[j].Contains("S"))
                        {
                            targeted = false;
                            offsetWidth = j;
                            offsetLength = weapon.AttackPattern[j].IndexOf('S');
                        }
                    }

                    var targetPos = targeted ? validTargets[i].Get<CGridPosition>() : pos;

                    if (targeted)
                    {
                        var beamAnim = _world.CreateEntity();
                        beamAnim.Set(new CParticleBeam { 
                            SourceX = pos.X,
                            SourceY = pos.Y,
                            DestX = targetPos.X,
                            DestY = targetPos.Y
                        });
                    }

                    for (int y = 0; y < attackWidth; y++)
                    {
                        for (int x = 0; x < attackLength; x++)
                        {
                            if(char.IsDigit(weapon.AttackPattern[y][x]))
                            {
                                var target = _world.CreateEntity();

                                int X =
                                    pos.Facing == Direction.EAST ? targetPos.X + (x - offsetLength) :
                                    pos.Facing == Direction.SOUTH ? targetPos.X + (y - offsetWidth) :
                                    pos.Facing == Direction.WEST ? targetPos.X - (x - offsetLength) :
                                    pos.Facing == Direction.NORTH ? targetPos.X - (y - offsetWidth) :
                                    targetPos.X + (x - offsetLength);

                                int Y =
                                    pos.Facing == Direction.EAST ? targetPos.Y + (y - offsetWidth) :
                                    pos.Facing == Direction.SOUTH ? targetPos.Y + (x - offsetLength) :
                                    pos.Facing == Direction.WEST ? targetPos.Y - (y - offsetWidth) :
                                    pos.Facing == Direction.NORTH ? targetPos.Y - (x - offsetLength) :
                                    targetPos.Y + (y - offsetWidth);

                                target.Set(new CGridPosition { 
                                    X = X, Y = Y
                                });

                                target.Set(new CTarget
                                {
                                    Source = entity.Has<CPlayer>() ? "PLAYER" : "ENEMY",
                                    Damage = weapon.Damage,
                                    Delay = (int) char.GetNumericValue(weapon.AttackPattern[y][x])
                                });

                            }
                        }
                    }
                }
                weapon.Cooldown = 0;
            }

            entity.Remove<CAttackAction>();

            base.Update(state, entity);
        }
    }
}
