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

            if (Game1.warningStop && (
                keyState.IsKeyDown(Keys.W) ||
                keyState.IsKeyDown(Keys.A) ||
                keyState.IsKeyDown(Keys.S) ||
                keyState.IsKeyDown(Keys.D) ||
                keyState.IsKeyDown(Keys.Up) ||
                keyState.IsKeyDown(Keys.Down) ||
                keyState.IsKeyDown(Keys.Left) ||
                keyState.IsKeyDown(Keys.Right)

                )) { return; }
            else if (Game1.warningStop)
            {
                Game1.warningStop = false;
            }
            if (!Game1.warningStop)
            {
                if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up))
                {
                    if (!Game1.Tiles.Contains((pos.X, pos.Y - 1)))
                        entity.Set(new CMoveAction { DistY = -1 });
                }
                if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down))
                {
                    if (!Game1.Tiles.Contains((pos.X, pos.Y + 1)))
                        entity.Set(new CMoveAction { DistY = 1 });
                }
                if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
                {
                    if (!Game1.Tiles.Contains((pos.X - 1, pos.Y)))
                        entity.Set(new CMoveAction { DistX = -1 });
                }
                if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
                {
                    if (!Game1.Tiles.Contains((pos.X + 1, pos.Y)))
                        entity.Set(new CMoveAction { DistX = 1 });
                }
            }
            if (entity.Has<CMoveAction>())
            {
                Game1.currentRoundState++;
                Config.SFX["player-move"].Play();
            }

            base.Update(state, entity);
        }
    }
}
