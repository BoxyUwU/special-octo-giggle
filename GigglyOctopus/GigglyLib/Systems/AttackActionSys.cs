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

            // Prioritize Later
            // validTargets.Sort();

            if (validTargets.Count != 0)
            {
                for (int i = 0; i < Math.Min(validTargets.Count, weapon.AdditionalTargets + 1); i++)
                {
                    var targetPos = targetSet[i].Get<CGridPosition>();
                    var target = _world.CreateEntity();
                    target.Set(new CGridPosition { X = targetPos.X, Y = targetPos.Y });
                    target.Set(new CTarget
                    {
                        Source = entity.Has<CPlayer>() ? "PLAYER" : "ENEMY",
                        Damage = weapon.Damage
                    });
                }
                weapon.Cooldown = 0;
            }

            entity.Remove<CAttackAction>();

            base.Update(state, entity);
        }
    }
}
