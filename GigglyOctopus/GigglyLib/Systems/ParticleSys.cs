using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class ParticleSys : AEntitySystem<float>
    {
        public ParticleSys()
            : base(Game1.world.GetEntities().With<CParticle>().With<CSprite>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            if (entity.Has<CScalable>())
            {
                ref var scale = ref entity.Get<CScalable>();
                scale.Scale *= 0.99f;
            }

            ref var sprite = ref entity.Get<CSprite>();
            ref var particle = ref entity.Get<CParticle>();
            var toDispose = new List<Entity>();
            sprite.Transparency *= 1.03f;
            sprite.Rotation += particle.DeltaRotation;
            sprite.X += (float)(Math.Cos(sprite.Rotation) * particle.Velocity) * Config.TileSize;
            sprite.Y += (float)(Math.Sin(sprite.Rotation) * particle.Velocity) * Config.TileSize;

            if (sprite.Transparency >= 1.0f)
            {
                toDispose.Add(entity);
            }

            foreach (var e in toDispose)
                e.Dispose();

            base.Update(state, entity);
        }
    }
}
