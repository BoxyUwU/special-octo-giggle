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
            base.Update(state, entity);
        }
    }
}
