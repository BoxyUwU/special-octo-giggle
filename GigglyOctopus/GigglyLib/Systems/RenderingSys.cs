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

        public RenderingSys(SpriteBatch spriteBatch)
            : base(Game1.world.GetEntities().With<CSprite>().AsSet())
        {
            sb = spriteBatch;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref CSprite sprite = ref entity.Get<CSprite>();
            var texture = Config.Textures[sprite.Texture];
            float scale = 1;
            if (entity.Has<CScalable>())
            {
                scale = entity.Get<CScalable>().Scale;
            }

            if (entity.Has<CSourceRectangle>()) {
                Rectangle rect = entity.Get<CSourceRectangle>().Rectangle;
                sb.Draw(
                    // Texture
                    texture,
                    // Position
                    new Vector2(sprite.X, sprite.Y),
                    // Source Rectangle (for animations and tiling)
                    rect,
                    // Tint + Opacity
                    Color.White * (1 - sprite.Transparency),
                    // Rotation
                    sprite.Rotation,
                    // Origin
                    new Vector2(rect.Width / 2, rect.Height / 2),
                    // Scale
                    scale,
                    // Sprite Effects
                    SpriteEffects.None,
                    // Render Depth
                    sprite.Depth
                );
            }
            else {
                sb.Draw(
                    // Texture
                    texture,
                    // Position
                    new Vector2(sprite.X, sprite.Y),
                    // Source Rectangle (for animations and tiling)
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
            }
            base.Update(state, entity);
        }
    }
}
