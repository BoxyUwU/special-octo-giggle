using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class TurnEndSys : AEntitySystem<float>
    {
        public TurnEndSys(World world)
            : base(world.GetEntities().WithEither<CPlayer>().Or<CEnemy>().AsSet())
        { }

        protected override void Update(float state, in Entity entity)
        {
            // this gets ran every frame not just end of turn
            if (entity.Has<CPlayer>() && Game1.roundOrder[Game1.currentRoundState + 1] == RoundState.Player)
                entity.Set<CSimTurn>();
            if (entity.Has<CEnemy>() && Game1.roundOrder[Game1.currentRoundState + 1] == RoundState.AI)
                entity.Set<CSimTurn>();

            base.Update(state, entity);
        }
    }
}
