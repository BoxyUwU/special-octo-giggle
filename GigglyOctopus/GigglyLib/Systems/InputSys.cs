using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework.Input;

namespace GigglyLib.Systems
{
    public class InputSys : AEntitySystem<float>
    {
        public InputSys(World world)
            : base(world.GetEntities().With<CPosition>().With<CMovable>().Without<CMoveTo>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            int x = 0;
            int y = 0;

            var keyState = Keyboard.GetState();
            ref var pos = ref entity.Get<CPosition>();

            if (keyState.IsKeyDown(Keys.W))
            {
                y -= 48;
                pos.Facing = Direction.NORTH;
            }
            else if (keyState.IsKeyDown(Keys.S))
            {
                y += 48;
                pos.Facing = Direction.SOUTH;
            }
            else if (keyState.IsKeyDown(Keys.A))
            {
                x -= 48;
                pos.Facing = Direction.WEST;
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                x += 48;
                pos.Facing = Direction.EAST;
            }

            entity.Set(new CMoveTo { X = x, Y = y });

            base.Update(state, entity);
        }
    }
}
