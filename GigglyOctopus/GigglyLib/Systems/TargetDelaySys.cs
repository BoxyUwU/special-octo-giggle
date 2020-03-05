using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class TargetDelaySys : AEntitySystem<float>
    {
        public TargetDelaySys()
            : base(Game1.world.GetEntities().With<CTarget>().AsSet())
        {  
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var target = ref entity.Get<CTarget>();

            if (target.Delay == 0)
            {
                entity.Set(new CDamageHere { Amount = target.Damage, Source = target.Source });
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
