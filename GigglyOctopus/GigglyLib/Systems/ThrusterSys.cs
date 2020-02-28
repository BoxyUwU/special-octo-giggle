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
            base.Update(state, entity);
        }
    }
}
