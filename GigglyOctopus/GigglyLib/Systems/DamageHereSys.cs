using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class DamageHereSys : AEntityBufferedSystem<float>
    {
        World _world;

        public DamageHereSys(World world)
            : base(world.GetEntities().With<CGridPosition>().With<CDamageHere>().AsSet())
        {
            _world = world;
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();

            var ships = _world.GetEntities().With<CGridPosition>().With<CEnemy>().With<CHealth>().AsSet().GetEntities();
            var toDispose = new List<Entity>();

            for (int i = 0; i < ships.Length; i++)
            {
                var shipPos = ships[i].Get<CGridPosition>();
                ref var shipHP = ref ships[i].Get<CHealth>();
                if (shipPos.X == pos.X && shipPos.Y == pos.Y && entity.Get<CDamageHere>().Source == "PLAYER")
                {
                    shipHP.Damage += entity.Get<CDamageHere>().Amount;
                    if (shipHP.Damage >= shipHP.Max)
                        toDispose.Add(ships[i]);
                }
            }

            foreach (var e in toDispose)
                e.Dispose();

            var anim = _world.CreateEntity();
            anim.Set(pos);
            anim.Set(new CExplosionAnim());
            anim.Set(entity.Get<CParticleColour>());

            entity.Remove<CDamageHere>();
            base.Update(state, entity);
        }
    }
}
