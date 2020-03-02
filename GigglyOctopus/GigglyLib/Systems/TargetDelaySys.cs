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
            if (Game1.currentRoundState != 4)
                return;

            ref var targets = ref entity.Get<CTargets>();
            for (int i = 0; i < targets.Entries.Count; i++)
            {
                var (X, Y, Delay) = targets.Entries[i];
                targets.Entries[i] = (X, Y, Delay - 1);
                if (Delay == 1)
                {
                    var e = _world.CreateEntity();
                    e.Set(new CGridPosition { X = X, Y = Y });
                    e.Set(new CTargetAnim { TargetType = CTargetAnim.Type.WARNING });
                }
                if (Delay == 0)
                {
                    var e = _world.CreateEntity();
                    e.Set(new CGridPosition { X = X, Y = Y });
                    e.Set(new CDamageHere { Amount = 1 });
                    e.Set(new CTargetAnim { 
                        TargetType = 
                            entity.Has<CPlayer>() ? CTargetAnim.Type.PLAYER : 
                            CTargetAnim.Type.DANGER 
                    });
                    targets.Entries.RemoveAt(i);
                    i--;
                }
            }

            base.Update(state, entity);
        }
    }
}
