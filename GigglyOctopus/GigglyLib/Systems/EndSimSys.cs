﻿using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class EndSimSys : ISystem<float>
    {
        EntitySet set;
        public EndSimSys()
        {
            set = Game1.world.GetEntities().With<CSimTurn>().AsSet();
        }

        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Dispose() { }

        public void Update(float state)
        {
            var simTurnSet = set.GetEntities();
            for (int i = 0; i < simTurnSet.Length; i++)
            {
                simTurnSet[i].Remove<CSimTurn>();
            }
            Game1.currentRoundState++;
        }

    }
}
