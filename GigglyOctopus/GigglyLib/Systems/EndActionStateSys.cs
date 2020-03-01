using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class EndActionStateSys : ISystem<float>
    {
        World _world;
        public EndActionStateSys(World world)
        {
            _world = world;
        }

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public void Dispose() { }

        public void Update(float state)
        {
            var movings = _world.GetEntities().With<CMoving>().AsSet();
            if (movings.Count == 0)
            {
                Game1.TurnState = TurnState.Player;

                var set = _world.GetEntities().With<CSprite>().With<CGridPosition>().AsSet();
                foreach (var entity in set.GetEntities())
                {
                    ref var sprite = ref entity.Get<CSprite>();
                    var pos = entity.Get<CGridPosition>();
                    sprite.X = pos.X * Config.TileSize;
                    sprite.Y = pos.Y * Config.TileSize;
                }
            }
        }
    }
}
