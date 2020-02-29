using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class ParticleSys : AEntitySystem<float>
    {
        private World _world;
        private Texture2D _particleTexture;

        public ParticleSys(World world)
            : base(world.GetEntities().With<CParticle>().With<CGridPosition>().With<CSprite>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {

            if (entity.Has<CScalable>())
            {
                ref var scale = ref entity.Get<CScalable>();
                scale.Scale *= 0.99f;
            }

            ref var pos = ref entity.Get<CGridPosition>();
            ref var sprite = ref entity.Get<CSprite>();
            ref var particle = ref entity.Get<CParticle>();
            sprite.Transparency *= 1.04f;
            sprite.Rotation += particle.DeltaRotation;
            pos.X += (float)(Math.Cos(sprite.Rotation) * particle.Velocity);
            pos.Y += (float)(Math.Sin(sprite.Rotation) * particle.Velocity);

            if (sprite.Transparency >= 1.0f)
            {
                entity.Remove<CSprite>();
            }

            base.Update(state, entity);
        }
    }
}
