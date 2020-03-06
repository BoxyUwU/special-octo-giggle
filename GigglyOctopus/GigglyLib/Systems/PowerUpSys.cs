using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class PowerUpSys : AEntityBufferedSystem<float>
    {
        public PowerUpSys()
            : base(Game1.world.GetEntities().With<CPowerUp>().AsSet())
        {
        }

        protected override void Update(float state, in Entity entity)
        {
            ref var powerup = ref entity.Get<CPowerUp>();
            if (!powerup.Animate)
            {
                ref var pos = ref entity.Get<CGridPosition>();
                ref var playerPos = ref Game1.Player.Get<CGridPosition>();
                if (pos.X == playerPos.X && pos.Y == playerPos.Y)
                {
                    var drop = entity.Get<CWeaponsArray>().Weapons[0];
                    ref var playerWeapons = ref Game1.Player.Get<CWeaponsArray>();
                    playerWeapons.Weapons.Add(drop);
                    ref var playerHealth = ref Game1.Player.Get<CHealth>();
                    playerHealth.Damage = 0;
                    entity.Set(new CScalable { Scale = 1.0f });
                    powerup.Animate = true;
                }
            }

            base.Update(state, entity);
        }
    }
}
