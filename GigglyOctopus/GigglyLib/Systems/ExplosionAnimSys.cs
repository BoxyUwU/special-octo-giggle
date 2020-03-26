using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using static GigglyLib.Game1;

namespace GigglyLib.Systems
{
    public class ExplosionAnimSys : AEntitySystem<float>
    {
        List<Entity> toPool = new List<Entity>();
        public ExplosionAnimSys()
            : base(world.GetEntities().With<CExplosionAnim>().With<CParticleColour>().With<CGridPosition>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CGridPosition>();
            if (!entity.Has<CSprite>())
            {
                CParticleColour colour = entity.Get<CParticleColour>();
                entity.Set(new CParticleSpawner {
                    Texture = PARTICLES[(int) colour.Colour],
                    Impact = 3,
                    RandomColours = colour.RandomColours
                });
                entity.Set(new CSprite
                {
                    X = pos.X * Config.TileSize,
                    Y = pos.Y * Config.TileSize,
                    Texture = "particles-explosion",
                    Rotation = Config.Rand() * (float) Math.PI * 2,
                    Depth = 0.7f
                });
                entity.Set(new CScalable
                {
                    Scale = 0.1f
                });
                Game1.playExplosion = true;
            }
            else
            {
                ref var sprite = ref entity.Get<CSprite>();
                ref var scale = ref entity.Get<CScalable>();
                sprite.Transparency += 0.3f;
                scale.Scale *= 2f;
                if (sprite.Transparency > 1.0f)
                {
                    toPool.Add(entity);
                }
            }

            base.Update(state, entity);
        }

        protected override void PostUpdate(float state)
        {
            foreach (var e in toPool)
            {
                e.Remove<CParticleSpawner>();
                e.Remove<CSprite>();
                e.Remove<CScalable>();
                e.Remove<CExplosionAnim>();
                e.Remove<CParticleColour>();
                e.Set<CExplosionPooled>();
            }
            toPool.Clear();

            base.PostUpdate(state);
        }
    }
}
