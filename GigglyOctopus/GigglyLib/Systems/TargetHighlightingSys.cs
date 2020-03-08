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
        public TargetHighlightingSys()
            : base(Game1.world.GetEntities().With<CTargetAnim>().With<CGridPosition>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CGridPosition>();
            ref var anim = ref entity.Get<CTargetAnim>();
            if (!entity.Has<CSprite>())
            {
                entity.Set(new CSprite
                {
                    X = pos.X * Config.TileSize,
                    Y = pos.Y * Config.TileSize,
                    Texture =
                        anim.TargetType == CTargetAnim.Type.PLAYER ? "target-player" :
                        anim.TargetType == CTargetAnim.Type.DANGER ? "target-enemy-danger" :
                        "target-enemy-warning",
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

                if(pos.X == Game1.Player.Get<CGridPosition>().X && pos.Y == Game1.Player.Get<CGridPosition>().Y)
                {
                    if (anim.TargetType == CTargetAnim.Type.DANGER)
                    {
                        Config.SFX["danger"].Play();
                        Game1.warningStop = true;
                    }
                    if (anim.TargetType == CTargetAnim.Type.WARNING)
                    {
                        Config.SFX["warning"].Play();
                        Game1.warningStop = true;
                    }
                }

            }
            if (!entity.Has<CTarget>())
                anim.FadingOut = true;

            base.Update(state, entity);
        }
    }
}
