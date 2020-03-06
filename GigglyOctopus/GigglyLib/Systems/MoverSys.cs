using System;
using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework.Input;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class MoverSys : AEntitySystem<float>
    {
        public MoverSys()
            : base(Game1.world.GetEntities().With<CGridPosition>().With<CMovable>().With<CSprite>().With<CMoving>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            int speed = 6;
            ref var pos = ref entity.Get<CGridPosition>();
            ref var sprite = ref entity.Get<CSprite>();
            ref var move = ref entity.Get<CMoving>();

            int x = Math.Abs(move.DistX) < speed ?
                move.DistX :
                move.DistX < 0 ? -speed : speed;
            int y = Math.Abs(move.DistY) < speed ?
                move.DistY :
                move.DistY < 0 ? -speed : speed;

            sprite.Y += y;
            move.DistY -= y;
            sprite.X += x;
            move.DistX -= x;
            sprite.Rotation = (int)pos.Facing * (float)(Math.PI / 2f);

            if (move.DistX == 0 && move.DistY == 0) {
                entity.Remove<CMoving>();
            }
                
            base.Update(state, entity);
        }
    }
}
