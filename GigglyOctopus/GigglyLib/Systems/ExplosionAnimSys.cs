using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static GigglyLib.Game1;

namespace GigglyLib.Systems
{
    public class ExplosionAnimSys : AEntitySystem<float>
    {
        private Texture2D _explosionTexture;

        public ExplosionAnimSys(Texture2D explosionTexture)
            : base(world.GetEntities().With<CExplosionAnim>().With<CParticleColour>().With<CGridPosition>().AsSet())
        {
            _explosionTexture = explosionTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            var toDispose = new List<Entity>();
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
                    Texture = _explosionTexture,
                    Rotation = Config.Rand() * (float) Math.PI * 2,
                    Depth = 0.7f
                });
                entity.Set(new CScalable
                {
                    Scale = 0.1f
                });
            }
            else
            {
                ref var sprite = ref entity.Get<CSprite>();
                ref var scale = ref entity.Get<CScalable>();
                sprite.Transparency += 0.3f;
                scale.Scale *= 2f;
                if (sprite.Transparency > 1.0f)
                {
                    toDispose.Add(entity);
                }
            }

            foreach (var e in toDispose)
                e.Dispose();

            base.Update(state, entity);
        }
    }
}
