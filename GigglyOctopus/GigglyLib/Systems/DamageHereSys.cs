using System;
using System.Collections.Generic;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;

namespace GigglyLib.Systems
{
    public class DamageHereSys : AEntityBufferedSystem<float>
    {
        EntitySet pooledExplosionSet;
        EntitySet shipSet;
        public DamageHereSys()
            : base(Game1.world.GetEntities().With<CGridPosition>().With<CDamageHere>().AsSet())
        {
            pooledExplosionSet = Game1.world.GetEntities().With<CExplosionPooled>().AsSet();
            shipSet = Game1.world.GetEntities().With<CGridPosition>().With<CHealth>().AsSet();
        }

        protected override void Update(float state, in Entity entity)
        {
            var pos = entity.Get<CGridPosition>();
            var damage = entity.Get<CDamageHere>();
            var ships = shipSet.GetEntities();
            var toDispose = new List<Entity>();

            for (int i = 0; i < ships.Length; i++)
            {
                ref var shipPos = ref ships[i].Get<CGridPosition>();
                ref var shipHP = ref ships[i].Get<CHealth>();
                if (shipPos.X == pos.X && shipPos.Y == pos.Y)
                {
                    if (ships[i].Has<CEnemy>() && damage.Source == "PLAYER" ||
                        ships[i].Has<CPlayer>() && damage.Source == "ENEMY")
                    {
                        shipHP.Damage += damage.Amount;

                        if (ships[i].Has<CEnemy>())
                            Config.SFX["enemy-hit"].Play();
                        else Config.SFX["player-hit"].Play();

                        if (shipHP.Damage > shipHP.Max)
                        {
                            if (ships[i].Has<CPlayer>())
                                Game1.GameState = GameState.GameOver;
                            else if (ships[i].Has<CEnemy>() && ships[i].Get<CEnemy>().HasPowerUp)
                            {
                                Config.SFX["enemy-destroyed"].Play();
                                ref var sprite = ref ships[i].Get<CSprite>();
                                sprite.Rotation = 0;
                                sprite.Texture = "power-up";
                                ships[i].Set<CPowerUp>();
                                ships[i].Remove<CEnemy>();
                                ships[i].Remove<CScalable>();
                            }
                            else
                            {
                                Config.SFX["enemy-destroyed"].Play();
                                toDispose.Add(ships[i]);
                            }
                        }
                    }
                }
            }

            foreach (var e in toDispose)
                e.Dispose();

            var anim = pooledExplosionSet.GetEntities().Length > 0 ? pooledExplosionSet.GetEntities()[0] : Game1.world.CreateEntity();
            anim.Remove<CExplosionPooled>();

            anim.Set(new CGridPosition { Facing = pos.Facing, X = pos.X, Y = pos.Y });
            anim.Set(new CExplosionAnim());
            anim.Set(entity.Get<CParticleColour>());

            entity.Remove<CDamageHere>();
            base.Update(state, entity);
        }
    }
}
