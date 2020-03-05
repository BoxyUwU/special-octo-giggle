using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class EndSimSys : ISystem<float>
    {
        public EndSimSys()
        {}

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose() { }

        public void Update(float state)
        {
            var simTurnSet = Game1.world.GetEntities().With<CSimTurn>().AsSet().GetEntities();
            for (int i = 0; i < simTurnSet.Length; i++)
            {
                simTurnSet[i].Remove<CSimTurn>();
            }
            Game1.currentRoundState++;
        }

    }
}
