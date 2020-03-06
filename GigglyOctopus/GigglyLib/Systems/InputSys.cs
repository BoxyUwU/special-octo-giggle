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

            if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
            {
                if (pos.Y - 1 >= 0 && !Game1.Tiles[pos.X, pos.Y - 1])
                    entity.Set(new CMoveAction { DistY = -1 });
            }
            if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
            {
                if (pos.Y + 1 < Game1.Tiles.GetLength(1) && !Game1.Tiles[pos.X, pos.Y + 1])
                    entity.Set(new CMoveAction { DistY = 1 });
            }
            if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            {
                if (pos.X - 1 >= 0 && !Game1.Tiles[pos.X - 1, pos.Y])
                    entity.Set(new CMoveAction { DistX = -1 });
            }
            if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            {
                if (pos.X + 1 < Game1.Tiles.GetLength(0) && !Game1.Tiles[pos.X + 1, pos.Y])
                    entity.Set(new CMoveAction { DistX = 1 });
            }

            if (entity.Has<CMoveAction>())
                Game1.currentRoundState++;

            base.Update(state, entity);
        }
    }
}
