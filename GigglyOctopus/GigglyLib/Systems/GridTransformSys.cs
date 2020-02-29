using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework.Input;

namespace GigglyLib.Systems
{
    public class GridTransformSys : AEntitySystem<float>
    {
        public GridTransformSys(World world)
            : base(world.GetEntities().With<CGridPosition>().With<CSprite>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            ref var sprite = ref entity.Get<CSprite>();
            sprite.X = pos.X * Config.TileSize;
            sprite.Y = pos.Y * Config.TileSize;
            if (entity.Has<CPlayer>() || entity.Has<CEnemy>())
                sprite.Rotation = (int)pos.Facing * (float)Math.PI / 2f;
            base.Update(state, entity);
        }
    }
}
