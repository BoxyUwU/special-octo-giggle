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
        public ParticleSys() { }

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose() { }

        public void Update(float state)
        {
            Particle p;
            for (ulong i = 0; i < ParticleManager.EndIndex; i++)
            {
                p = ParticleManager.Particles[i];

                p.Scale *= 0.99f;
                p.Transparency *= 1.03f;
                p.Rotation += p.DeltaRotation;
                p.X += (float)(Math.Cos(p.Rotation) * p.Velocity) * Config.TileSize;
                p.Y += (float)(Math.Sin(p.Rotation) * p.Velocity) * Config.TileSize;

                ParticleManager.Particles[i] = p;
                if (p.Transparency >= 1)
                    ParticleManager.FreeParticle(i);
            }
        }
    }
}
