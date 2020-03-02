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
            if (Game1.currentRoundState != 4)
                return;

            ref var target = ref entity.Get<CTarget>();

            if (target.Delay == 0)
            {
                entity.Set(new CDamageHere { Amount = 1 });
                entity.Remove<CTargetAnim>();
                entity.Set(new CTargetAnim
                {
                    TargetType =
                        target.Source == "PLAYER" ? CTargetAnim.Type.PLAYER :
                        CTargetAnim.Type.DANGER
                });
                entity.Remove<CTarget>();
            }
            else
            {
                if (target.Delay == 1 && target.Source == "ENEMY")
                {
                    entity.Set(new CTargetAnim { TargetType = CTargetAnim.Type.WARNING });
                }
                target.Delay--;
            }

            base.Update(state, entity);
        }
    }
}
