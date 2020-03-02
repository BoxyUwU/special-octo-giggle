using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class MarkerSpawnerSys : AEntitySystem<float>
    {
        Texture2D _playerTexture;
        Texture2D _dangerTexture;
        Texture2D _warningTexture;
        public MarkerSpawnerSys(World world, Texture2D playerTexture, Texture2D dangerTexture, Texture2D warningTexture)
            : base(world.GetEntities().With<CGridPosition>().With<CTarget>().AsSet())
        {
            _playerTexture = playerTexture;
            _dangerTexture = dangerTexture;
            _warningTexture = warningTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            var target = entity.Get<CTarget>();

            if (target.Delay == 0)
            {
                entity.Set(new CTargetAnim { 
                    TargetType = target.Source == "PLAYER" ? CTargetAnim.Type.PLAYER : CTargetAnim.Type.DANGER
                });
            }
            else if (target.Delay == 1 && target.Source == "ENEMY")
            {
                entity.Set(new CTargetAnim {
                    TargetType = CTargetAnim.Type.WARNING
                });
            }

            ref var anim = ref entity.Get<CTargetAnim>();
            entity.Set(new CSprite
            {
                Texture =
                        anim.TargetType == CTargetAnim.Type.PLAYER ? _playerTexture :
                        anim.TargetType == CTargetAnim.Type.DANGER ? _dangerTexture :
                        _warningTexture,
                Transparency = 0.0f,
                X = pos.X * Config.TileSize,
                Y = pos.Y * Config.TileSize,
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

            base.Update(state, entity);
        }
    }
}
