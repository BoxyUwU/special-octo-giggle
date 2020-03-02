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
            entity.Set(new CMoveAction { DistX = -1 });

            base.Update(state, entity);
        }
    }
}
