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
            : base(world.GetEntities().With<CPosition>().With<CMovable>().With<CMoveTo>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CPosition>();
            ref var moveTo = ref entity.Get<CMoveTo>();

            var (x, y) = DrainMoveTo(ref moveTo);
            pos.X += x;
            pos.Y += y;

            if (moveTo.X == 0 && moveTo.Y == 0)
                entity.Remove<CMoveTo>();

            base.Update(state, entity);
        }

        private (int x, int y) DrainMoveTo(ref CMoveTo moveTo)
        {
            int drain = 3;
            (int x, int y) drained = (x: 0, y: 0);

            if ((moveTo.X > 0 && moveTo.X >= drain) || (moveTo.X < 0 && moveTo.X <= -drain))
                drained.x += moveTo.X != 0 ? (moveTo.X > 0 ? drain : -drain) : 0;
            else
                drained.x += moveTo.X;

            if ((moveTo.Y > 0 && moveTo.Y >= drain) || (moveTo.Y > 0 && moveTo.Y <= -drain))
                drained.y += moveTo.Y != 0 ? (moveTo.Y > 0 ? drain : -drain) : 0;
            else
                drained.y += moveTo.Y;
                
            moveTo.X -= drained.x;
            moveTo.Y -= drained.y;
            return drained;
        }
    }
}
