﻿using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class DamageHereSys : AEntityBufferedSystem<float>
    {
        public DamageHereSys()
            : base(Game1.world.GetEntities().With<CGridPosition>().With<CDamageHere>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            var damage = entity.Get<CDamageHere>();
            var ships = Game1.world.GetEntities().With<CGridPosition>().With<CHealth>().AsSet().GetEntities();
            var toDispose = new List<Entity>();

            for (int i = 0; i < ships.Length; i++)
            {
                var shipPos = ships[i].Get<CGridPosition>();
                ref var shipHP = ref ships[i].Get<CHealth>();
                if (shipPos.X == pos.X && shipPos.Y == pos.Y)
                {
                    if (ships[i].Has<CEnemy>() && damage.Source == "PLAYER" ||
                        ships[i].Has<CPlayer>() && damage.Source == "ENEMY")
                    {
                        shipHP.Damage += damage.Amount;
                        if (shipHP.Damage > shipHP.Max)
                        {
                            if (damage.Source == "ENEMY")
                                Game1.GameState = GameState.Starting;
                            else
                                toDispose.Add(ships[i]);
                        }
                    }
                }
            }

            foreach (var e in toDispose)
                e.Dispose();

            var anim = Game1.world.CreateEntity();
            anim.Set(pos);
            anim.Set(new CExplosionAnim());
            anim.Set(entity.Get<CParticleColour>());

            entity.Remove<CDamageHere>();
            base.Update(state, entity);
        }
    }
}
