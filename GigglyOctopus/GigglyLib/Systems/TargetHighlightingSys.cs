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
        Texture2D _playerTexture;
        Texture2D _dangerTexture;
        Texture2D _warningTexture;
        public TargetHighlightingSys(Texture2D playerTexture, Texture2D dangerTexture, Texture2D warningTexture)
            : base(Game1.world.GetEntities().With<CTargetAnim>().With<CGridPosition>().AsSet())
        {
            _playerTexture = playerTexture;
            _dangerTexture = dangerTexture;
            _warningTexture = warningTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            ref var anim = ref entity.Get<CTargetAnim>();
            if (!entity.Has<CSprite>())
            {
                entity.Set(new CSprite
                {
                    X = pos.X * Config.TileSize,
                    Y = pos.Y * Config.TileSize,
                    Texture =
                        anim.TargetType == CTargetAnim.Type.PLAYER ? _playerTexture :
                        anim.TargetType == CTargetAnim.Type.DANGER ? _dangerTexture :
                        _warningTexture,
                    Transparency = 1.0f,
                    Depth = 0.2f
                });
                entity.Set(new CSourceRectangle
                {
                    Rectangle = new Rectangle(0, 0, Config.TileSize, Config.TileSize)
                });
                entity.Set(new CSpriteAnimation
                {
                    TotalFrames = 24,
                    SkipFrames = 1,
                });
            }
            if (!entity.Has<CTarget>())
                anim.FadingOut = true;

            base.Update(state, entity);
        }
    }
}
