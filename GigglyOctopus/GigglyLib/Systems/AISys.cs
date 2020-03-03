using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class AISys : AEntitySystem<float>
    {
        public AISys(World world)
            : base(world.GetEntities().With<CEnemy>().With<CMovable>().With<CGridPosition>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            var (x, y) =
                pos.Facing == Direction.NORTH ? (0, -1) :
                pos.Facing == Direction.EAST ? (1, 0) :
                pos.Facing == Direction.SOUTH ? (0, 1) :
                (-1, 0);
            entity.Set(new CMoveAction { DistX = x, DistY = y });

            base.Update(state, entity);
        }
    }
}
