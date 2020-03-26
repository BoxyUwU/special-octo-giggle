using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class MarkerFadeSys : AEntitySystem<float>
    {
        List<Entity> toPool = new List<Entity>();
        public MarkerFadeSys()
            : base (Game1.world.GetEntities().With<CTargetAnim>().With<CGridPosition>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var anim = ref entity.Get<CTargetAnim>();
            ref var sprite = ref entity.Get<CSprite>();

            if (anim.FadingOut && anim.GoneVisible)
            {
                sprite.Transparency += 0.1f;
                if (sprite.Transparency >= 1f)
                    toPool.Add(entity);
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

            base.Update(state, entity);
        }

        protected override void PostUpdate(float state)
        {
            foreach (var e in toPool)
            {
                e.Remove<CSprite>();
                e.Remove<CParticleColour>();
                e.Remove<CTarget>();
                e.Remove<CTargetAnim>();
                e.Set<CMarkerPooled>();
            }
            toPool.Clear();

            base.PostUpdate(state);
        }
    }
}
