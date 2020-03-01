using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class ThrusterSys : AEntitySystem<float>
    {
        private World _world;
        private Texture2D _particleTexture;

        public ThrusterSys(World world, Texture2D particleTexture)
            : base(world.GetEntities().With<CPlayer>().With<CGridPosition>().AsSet())
        {
            _world = world;
            _particleTexture = particleTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            var sprite = entity.Get<CSprite>();
            var gPos = entity.Get<CGridPosition>();
            var particle = _world.CreateEntity();

            float x = sprite.X;
            float y = sprite.Y;

            x += (Config.Rand() - 0.5f) * 0.2f * Config.TileSize;
            y += (Config.Rand() - 0.5f) * 0.2f * Config.TileSize;

            particle.Set(new CSprite {
                Texture = _particleTexture,
                Rotation = Config.Rand() * 2 * (float)Math.PI,
                Transparency = (Config.Rand() * 0.2f) + 0.12f,
                X = x,
                Y = y,
            });

            particle.Set(new CScalable { Scale = (Config.Rand() * 0.4f) + 0.3f });
            particle.Set(new CParticle { DeltaRotation = Config.Rand() * 0.05f, Velocity = Config.Rand() * 0.02f });

            base.Update(state, entity);
        }
    }
}
