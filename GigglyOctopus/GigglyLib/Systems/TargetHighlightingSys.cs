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
            : base(world.GetEntities().With<CTargetAnim>().With<CSprite>().With<CGridPosition>().AsSet())
        {
            _world = world;
            _playerTexture = playerTexture;
            _dangerTexture = dangerTexture;
            _warningTexture = warningTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var anim = ref entity.Get<CTargetAnim>();
            ref var sprite = ref entity.Get<CSprite>();

            sprite.Texture =
                    anim.TargetType == CTargetAnim.Type.PLAYER ? _playerTexture :
                    anim.TargetType == CTargetAnim.Type.DANGER ? _dangerTexture :
                    _warningTexture;
                
            sprite.Transparency += 0.12f;
            if (sprite.Transparency >= 1.0)
            {
                entity.Remove<CTargetAnim>();
            }

            base.Update(state, entity);
        }
    }
}
