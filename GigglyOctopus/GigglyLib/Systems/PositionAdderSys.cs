using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class TestSys : AEntitySystem<float>
    {
        private World _world;

        public TestSys(World world)
            : base(world.GetEntities().With<CPosition>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CPosition>();
            Console.WriteLine(pos.X + ", " + pos.Y);
            pos.X += 1;
            pos.Y += 1;

            base.Update(state, entity);
        }
    }
}
