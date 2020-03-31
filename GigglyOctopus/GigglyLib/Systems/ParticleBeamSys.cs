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
    public class ParticleBeamSys : AEntitySystem<float>
    {
        public ParticleBeamSys()
            : base(Game1.world.GetEntities().With<CParticleBeam>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var beam = ref entity.Get<CParticleBeam>();
            if(beam.Frame < 6)
            {
                float sparsity = 3f;
                float distX = (beam.DestX - beam.SourceX) * Config.TileSize;
                float distY = (beam.DestY - beam.SourceY) * Config.TileSize;
                float distH = (float)Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2));

                float deltaX = distX * sparsity / distH;
                float deltaY = distY * sparsity / distH;

                float tracer = 0;
                while (tracer < distH / 6)
                {
                    ParticleManager.CreateParticle(
                        x: beam.SourceX * Config.TileSize + beam.X,
                        y: beam.SourceY * Config.TileSize + beam.Y,
                        texture: PARTICLES[beam.RandomColours ? Game1.NonDeterministicRandom.Next(18) : (int)beam.Colour],
                        deltaRotation: Game1.NonDeterministicRandom.NextFloat() * 0.05f,
                        velocity: Game1.NonDeterministicRandom.NextFloat() * 0.02f,
                        scale: (Game1.NonDeterministicRandom.NextFloat() * 0.25f) + 0.15f,
                        transparency: (Game1.NonDeterministicRandom.NextFloat() * 0.2f) + 0.12f,
                        rotation: (Game1.NonDeterministicRandom.NextFloat() * 0.2f) + 0.12f
                        );

                    beam.X += deltaX;
                    beam.Y += deltaY;
                    tracer += sparsity;
                }
                beam.Frame++;
            }
            else
            {
                var toDispose = new List<Entity>();
                toDispose.Add(entity);
                foreach (var e in toDispose)
                    e.Dispose();
            }

            base.Update(state, entity);
        }
    }
}
