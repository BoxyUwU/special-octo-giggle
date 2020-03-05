using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class MoveActionSys : ISystem<float>
    {
        public MoveActionSys()
        {}

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose() { }

        public void Update(float state)
        {
            var enemies = Game1.world.GetEntities().With<CGridPosition>().With<CMoveAction>().With<CEnemy>().AsSet().GetEntities();
            var players = Game1.world.GetEntities().With<CGridPosition>().With<CMoveAction>().With<CPlayer>().AsSet().GetEntities();

            foreach (var e in enemies)
                MoveEntity(e);
            foreach (var e in players)
                MoveEntity(e);
        }

        private void MoveEntity(Entity e)
        {
            ref var pos = ref e.Get<CGridPosition>();
            ref var moving = ref e.Get<CMoveAction>();

            pos.Facing =
                moving.DistX > 0 ? Direction.EAST :
                moving.DistX < 0 ? Direction.WEST :
                moving.DistY > 0 ? Direction.SOUTH :
                moving.DistY < 0 ? Direction.NORTH :
                pos.Facing;
            pos.X += moving.DistX;
            pos.Y += moving.DistY;

            e.Set(new CMoving { DistX = moving.DistX * Config.TileSize, DistY = moving.DistY * Config.TileSize });
            e.Remove<CMoveAction>();
            e.Set<CAttackAction>();
        }
    }
}
