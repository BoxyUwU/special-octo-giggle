using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class RenderingSys : AEntitySystem<float>
    {
        SpriteBatch sb;

        public RenderingSys(World world, SpriteBatch spriteBatch)
            : base(world.GetEntities().With<CSprite>().AsSet())
        {
            sb = spriteBatch;
        }

        protected override void Update(float state, in Entity entity)
        {
            CSprite sprite = entity.Get<CSprite>();
            Texture2D texture = entity.Get<CSprite>().Texture;
            float scale = 1;
            if (entity.Has<CScalable>())
            {
                scale = entity.Get<CScalable>().Scale;
            }
            sb.Draw(
                // Texture
                texture,
                // Position
                new Vector2(sprite.X, sprite.Y),
                // Source Rectangle (for animations)
                null,
                // Tint + Opacity
                Color.White * (1 - sprite.Transparency),
                // Rotation
                sprite.Rotation,
                // Origin
                new Vector2(texture.Width / 2, texture.Height / 2),
                // Scale
                scale,
                // Sprite Effects
                SpriteEffects.None,
                // Render Depth
                sprite.Depth
            );
            base.Update(state, entity);
        }
    }
}
