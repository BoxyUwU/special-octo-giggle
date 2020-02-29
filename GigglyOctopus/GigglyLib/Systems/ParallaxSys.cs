using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class ParallaxSys : AEntitySystem<float>
    {
        private World _world;
        private Entity _player;

        public ParallaxSys(World world, Entity player)
            : base(world.GetEntities().With<CParallaxBackground>().With<CSprite>().AsSet())
        {
            _world = world;
            _player = player;
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = _player.Get<CGridPosition>();
            var playerSprite = _player.Get<CSprite>();
            ref var sprite = ref entity.Get<CSprite>();
            ref var parallax = ref entity.Get<CParallaxBackground>();

            if (_player.Has<CMoving>())
            {
                parallax.OffsetX +=
                    pos.Facing == Direction.WEST ? parallax.ScrollVelocity :
                    pos.Facing == Direction.EAST ? -parallax.ScrollVelocity :
                    0;

                parallax.OffsetY +=
                    pos.Facing == Direction.NORTH ? parallax.ScrollVelocity :
                    pos.Facing == Direction.SOUTH ? -parallax.ScrollVelocity :
                    0;

                float width = sprite.Texture.Width;
                float height = sprite.Texture.Height;

                if (parallax.OffsetX < -width/2)
                    parallax.OffsetX += width;
                if (parallax.OffsetX >= width/2)
                    parallax.OffsetX -= width;
                if (parallax.OffsetY < -height/2)
                    parallax.OffsetY += height;
                if (parallax.OffsetY >= height/2)
                    parallax.OffsetY -= height;

                sprite.X = playerSprite.X - Config.ScreenWidth / 2 + parallax.OffsetX;
                sprite.Y = playerSprite.Y - Config.ScreenHeight / 2 + parallax.OffsetY;
            }

            base.Update(state, entity);
        }
    }
}
