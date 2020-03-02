using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class TargetDelaySys : AEntitySystem<float>
    {
        World _world;

        public TargetDelaySys(World world)
            : base(world.GetEntities().With<CTarget>().AsSet())
        { 
            _world = world; 
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var target = ref entity.Get<CTarget>();

            if (target.Delay == 0)
            {
                entity.Set(new CDamageHere { Amount = 1 });
                entity.Remove<CTarget>();
            }
            else
            {
                target.Delay--;
            }

            base.Update(state, entity);
        }
    }
}
