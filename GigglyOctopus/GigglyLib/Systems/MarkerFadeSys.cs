using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class MarkerFadeSys : AEntitySystem<float>
    {
        public MarkerFadeSys(World world)
            : base (world.GetEntities().With<CTargetAnim>().With<CGridPosition>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var anim = ref entity.Get<CTargetAnim>();
            ref var sprite = ref entity.Get<CSprite>();
            var toDispose = new List<Entity>();

            if (anim.FadingOut && anim.GoneVisible)
            {
                sprite.Transparency += 0.1f;
                if (sprite.Transparency >= 1f)
                    toDispose.Add(entity);
            }
            else
            {
                sprite.Transparency -= 0.1f;
                if (sprite.Transparency <= 0f)
                {
                    sprite.Transparency = 0f;
                    anim.GoneVisible = true;
                }
            }

            foreach (var e in toDispose)
                e.Dispose();

            base.Update(state, entity);
        }
    }
}
