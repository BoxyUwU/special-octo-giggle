using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class ParticleSys : ISystem<float>
    {
        List<Entity> toPool = new List<Entity>();
        EntitySet entitySet;
        public ParticleSys()
        {
            entitySet = Game1.world.GetEntities().With<CParticle>().With<CSprite>().Without<CPartPooled>().AsSet();
        }

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose() { }

        public void Update(float state)
        {
            var set = entitySet.GetEntities();
            for (int i = 0; i < set.Length; i++)
            {
                var entity = set[i];
                if (entity.Has<CScalable>())
                {
                    ref var scale = ref entity.Get<CScalable>();
                    scale.Scale *= 0.99f;
                }

                ref var sprite = ref entity.Get<CSprite>();
                ref var particle = ref entity.Get<CParticle>();
                sprite.Transparency *= 1.03f;
                sprite.Rotation += particle.DeltaRotation;
                sprite.X += (float)(Math.Cos(sprite.Rotation) * particle.Velocity) * Config.TileSize;
                sprite.Y += (float)(Math.Sin(sprite.Rotation) * particle.Velocity) * Config.TileSize;

                if (sprite.Transparency >= 1.0f)
                {
                    toPool.Add(entity);
                }
            }

            foreach (var e in toPool)
            {
                e.Remove<CSprite>();
                e.Remove<CParticle>();
                e.Remove<CScalable>();
                e.Set<CPartPooled>();
            }
            toPool.Clear();
        }
    }
}
