using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class MoverSys : AEntitySystem<float>
    {
        private World _world;

        public MoverSys(World world)
            : base(world.GetEntities().With<CGridPosition>().With<CMovable>().With<CSprite>().With<CMoving>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            int speed = 6;
            ref var pos = ref entity.Get<CGridPosition>();
            ref var sprite = ref entity.Get<CSprite>();
            ref var move = ref entity.Get<CMoving>();

            int distance = Math.Min(speed, move.Remaining);
            move.Remaining -= distance;

            sprite.Y -=
                pos.Facing == Direction.NORTH ? distance :
                pos.Facing == Direction.SOUTH ? -distance :
                0;

            sprite.X -=
                pos.Facing == Direction.WEST ? distance :
                pos.Facing == Direction.EAST ? -distance :
                0;

            if (move.Remaining == 0) {
                entity.Remove<CMoving>();
            }
                
            base.Update(state, entity);
        }
    }
}
