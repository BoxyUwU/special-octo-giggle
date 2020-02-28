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
            : base(world.GetEntities().With<CGridPosition>().With<CMovable>().With<CPlayer>().Without<CMoving>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            var keyState = Keyboard.GetState();
            ref var pos = ref entity.Get<CGridPosition>();

            if (keyState.IsKeyDown(Keys.W))
            {
                pos.Y--;
                pos.Facing = Direction.NORTH;
                entity.Set(new CMoving { Remaining = Config.TileSize });
            }
            else if (keyState.IsKeyDown(Keys.S))
            {
                pos.Y++;
                pos.Facing = Direction.SOUTH;
                entity.Set(new CMoving { Remaining = Config.TileSize });
            }
            else if (keyState.IsKeyDown(Keys.A))
            {
                pos.X--;
                pos.Facing = Direction.WEST;
                entity.Set(new CMoving { Remaining = Config.TileSize });
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                pos.X++;
                pos.Facing = Direction.EAST;
                entity.Set(new CMoving { Remaining = Config.TileSize });
            }

            base.Update(state, entity);
        }
    }
}
