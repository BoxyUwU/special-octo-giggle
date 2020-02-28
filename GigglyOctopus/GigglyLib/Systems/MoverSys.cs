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
            float speed = 6;
            ref var pos = ref entity.Get<CGridPosition>();
            ref var sprite = ref entity.Get<CSprite>();
            ref var move = ref entity.Get<CMoving>();

            float amount = move.Remaining - Math.Min(speed, move.Remaining);
            Console.WriteLine($"Amount: { amount }");
            Console.WriteLine($"Before: [{ sprite.X }, { sprite.Y }]");
            sprite.Y +=
                pos.Facing == Direction.NORTH ? amount :
                pos.Facing == Direction.SOUTH ? -amount :
                0;

            sprite.X +=
                pos.Facing == Direction.WEST ? amount :
                pos.Facing == Direction.EAST ? -amount :
                0;
            Console.WriteLine($"After: [{ sprite.X }, { sprite.Y }]");

            sprite.Rotation = (float)Math.PI / 2 * (int)pos.Facing;

            move.Remaining -= Math.Min(speed, move.Remaining);
            Console.WriteLine($"Remaining: { move.Remaining }");

            if (move.Remaining <= 0.0000001) {
                entity.Remove<CMoving>();
            }
                
            base.Update(state, entity);
        }

        //private (int x, int y) DrainMoveTo(ref CMoveTo moveTo)
        //{
        //    int drain = 5;
        //    (int x, int y) drained = (x: 0, y: 0);

        //    if ((moveTo.X > 0 && moveTo.X >= drain) || (moveTo.X < 0 && moveTo.X <= -drain))
        //        drained.x += moveTo.X != 0 ? (moveTo.X > 0 ? drain : -drain) : 0;
        //    else
        //        drained.x += moveTo.X;

        //    if ((moveTo.Y > 0 && moveTo.Y >= drain) || (moveTo.Y < 0 && moveTo.Y <= -drain))
        //        drained.y += moveTo.Y != 0 ? (moveTo.Y > 0 ? drain : -drain) : 0;
        //    else
        //        drained.y += moveTo.Y;
                
        //    moveTo.X -= drained.x;
        //    moveTo.Y -= drained.y;
        //    return drained;
        //}
    }
}
