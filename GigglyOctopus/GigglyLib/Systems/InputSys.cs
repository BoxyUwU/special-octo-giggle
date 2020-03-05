using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework.Input;

namespace GigglyLib.Systems
{
    public class InputSys : AEntitySystem<float>
    {
        public InputSys()
            : base(Game1.world.GetEntities().With<CGridPosition>().With<CMovable>().With<CPlayer>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            var keyState = Keyboard.GetState();
            ref var pos = ref entity.Get<CGridPosition>();

            bool moved = true;
            if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            {
                entity.Set(new CMoveAction { DistY = -1 });
            }
            else if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            {
                entity.Set(new CMoveAction { DistY = 1 });
            }
            else if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            {
                entity.Set(new CMoveAction { DistX = -1 });
            }
            else if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            {
                entity.Set(new CMoveAction { DistX = 1 });
            }
            else
                moved = false;

            if (moved)
                Game1.currentRoundState++;

            base.Update(state, entity);
        }
    }
}
