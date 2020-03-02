using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class TargetHighlightingSys : AEntitySystem<float>
    {
        World _world;
        Texture2D _playerTexture;
        Texture2D _dangerTexture;
        Texture2D _warningTexture;
        public TargetHighlightingSys(World world, Texture2D playerTexture, Texture2D dangerTexture, Texture2D warningTexture)
            : base(world.GetEntities().With<CTargetAnim>().With<CGridPosition>().AsSet())
        {
            _world = world;
            _playerTexture = playerTexture;
            _dangerTexture = dangerTexture;
            _warningTexture = warningTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var anim = ref entity.Get<CTargetAnim>();
            if (!entity.Has<CSprite>())
            {
                entity.Set(new CSprite
                {
                    Texture =
                        anim.TargetType == CTargetAnim.Type.PLAYER ? _playerTexture :
                        anim.TargetType == CTargetAnim.Type.DANGER ? _dangerTexture :
                        _warningTexture,
                    Transparency = 1.0f
                });
                entity.Set(new CSourceRectangle
                {
                    Rectangle = new Rectangle(0, 0, Config.TileSize, Config.TileSize)
                });
                entity.Set(new CSpriteAnimation
                {
                    TotalFrames = 12,
                    SkipFrames = 5,
                });
            }
            else if (!anim.FadingOut)
            {
                ref var sprite = ref entity.Get<CSprite>();
                sprite.Transparency -= 0.15f;
                if (sprite.Transparency <= 0.1)
                {
                    anim.FadingOut = true;
                }
            }
            else
            {
                ref var sprite = ref entity.Get<CSprite>();
                sprite.Transparency += 0.125f;
                if (sprite.Transparency >= 1.0)
                {
                    entity.Remove<CTargetAnim>();
                }
            }
            base.Update(state, entity);
        }
    }
}
