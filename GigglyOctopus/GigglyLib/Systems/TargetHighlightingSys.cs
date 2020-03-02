using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class TargetHighlightingSys : AEntitySystem<float>
    {
        World _world;
        Texture2D _playerTexture;
        public TargetHighlightingSys(World world, Texture2D playerTexture)
            : base(world.GetEntities().With<CTargets>().With<CGridPosition>().AsSet())
        {
            _world = world;
            _playerTexture = playerTexture;
        }

        protected override void Update(float state, in Entity entity)
        {
            if (entity.Has<CPlayer>())
            {
                var targets = entity.Get<CTargets>().Entries;

                foreach(var target in targets)
                {
                    var highlight = _world.CreateEntity();
                    highlight.Set(new CTargetHighLight());
                    highlight.Set(new CGridPosition {
                        X = target.X,
                        Y = target.Y
                    });
                    highlight.Set(new CSprite
                    {
                        Texture = _playerTexture,
                        Transparency = 0.2f
                    });
                    highlight.Set(new CSourceRectangle
                    {
                        Rectangle = new Rectangle(0, 0, Config.TileSize, Config.TileSize)
                    });
                    highlight.Set(new CSpriteAnimation
                    {
                        TotalFrames = 12,
                        SkipFrames = 5
                    });
                }
            }
            base.Update(state, entity);
        }
    }
}
