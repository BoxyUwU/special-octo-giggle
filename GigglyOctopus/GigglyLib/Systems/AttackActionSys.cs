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
            : base(world.GetEntities().With<CGridPosition>().With<CSimTurn>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CGridPosition>();

            var targetBuilder = _world.GetEntities().With<CGridPosition>();

            if (entity.Has<CEnemy>())
                targetBuilder.With<CPlayer>();
            if (entity.Has<CPlayer>())
                targetBuilder.With<CEnemy>();
            var targetSet = targetBuilder.AsSet().GetEntities();

            for (int i = 0; i < targetSet.Length; i++)
            {
                var targetPos = targetSet[i].Get<CGridPosition>();
                int distance = Math.Abs(pos.X - targetPos.X) + Math.Abs(pos.Y - targetPos.Y);
                if (distance < 5)
                {
                    var target = _world.CreateEntity();
                    target.Set(new CGridPosition { X = targetPos.X, Y = targetPos.Y });
                    target.Set(new CTarget {
                        Source = entity.Has<CPlayer>() ? "PLAYER" : "ENEMY"
                    });
                    break;
                }
            }

            entity.Remove<CAttackAction>();

            base.Update(state, entity);
        }
    }
}
