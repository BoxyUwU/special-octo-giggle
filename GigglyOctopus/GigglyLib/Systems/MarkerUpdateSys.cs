using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class MarkerUpdateSys : AEntitySystem<float>
    {
        public MarkerUpdateSys()
            : base(Game1.world.GetEntities().With<CGridPosition>().With<CTarget>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            ref var target = ref entity.Get<CTarget>();

            if (target.Delay == 0)
            {
                entity.Remove<CSprite>();
                entity.Set(new CTargetAnim { 
                    TargetType = target.Source == "PLAYER" ? CTargetAnim.Type.PLAYER : CTargetAnim.Type.DANGER
                });
            }
            else if (target.Delay == 1 && target.Source == "ENEMY")
            {
                entity.Set(new CTargetAnim {
                    TargetType = CTargetAnim.Type.WARNING
                });
            }

            base.Update(state, entity);
        }
    }
}
