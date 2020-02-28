using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class TestSys : AEntitySystem<float>
    {
        private World _world;

        public TestSys(World world)
            : base(world.GetEntities().With<CPosition>().With<CMovable>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var pos = ref entity.Get<CPosition>();
            Console.WriteLine(pos.X + ", " + pos.Y);
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.W))
                pos.Y--;
            if (keyState.IsKeyDown(Keys.S))
                pos.Y++;
            if (keyState.IsKeyDown(Keys.A))
                pos.X--;
            if (keyState.IsKeyDown(Keys.D))
                pos.X++;

            base.Update(state, entity);
        }
    }
}
