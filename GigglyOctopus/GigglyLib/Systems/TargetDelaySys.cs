using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class TargetDelaySys : AEntitySystem<float>
    {
        World _world;

        public TargetDelaySys(World world)
            : base(world.GetEntities().With<CTargets>().AsSet())
        { 
            _world = world; 
        }

        protected override void Update(float state, in Entity entity)
        {
            if (Game1.RoundState != RoundState.PlayerSimulate)
                return;

            ref var targets = ref entity.Get<CTargets>();
            for (int i = 0; i < targets.Entries.Count; i++)
            {
                var (X, Y, Delay) = targets.Entries[i];
                targets.Entries[i] = (X, Y, Delay - 1);
                if (Delay == 0)
                {
                    var e = _world.CreateEntity();
                    e.Set(new CGridPosition { X = X, Y = Y });
                    e.Set(new CDamageHere { Amount = 1 });
                    targets.Entries.RemoveAt(i);
                    i--;
                }
            }

            base.Update(state, entity);
        }
    }
}
