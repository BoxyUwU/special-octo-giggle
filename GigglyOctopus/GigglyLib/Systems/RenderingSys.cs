﻿using System;
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

            float rotation = sprite.Rotation == 0f ? (entity.Has<CGridPosition>() ? (int)entity.Get<CGridPosition>().Facing * (float)(Math.PI / 2f) : sprite.Rotation) : sprite.Rotation;

            if (entity.Has<CSourceRectangle>()) {
                Rectangle rect = entity.Get<CSourceRectangle>().Rectangle;
                bool anim = entity.Has<CSpriteAnimation>();
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
                    rotation,
                    // Origin
                    new Vector2(anim ? rect.Width / 2 : texture.Width / 2, anim ? rect.Height / 2 : texture.Height / 2),
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
                    rotation,
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
