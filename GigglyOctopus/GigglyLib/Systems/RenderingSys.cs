using System;
using DefaultEcs;
using DefaultEcs.System;
using GigglyLib.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GigglyLib.Systems
{
    public class RenderingSys : AEntitySystem<float>
    {
        SpriteBatch sb;

        public RenderingSys(World world, SpriteBatch spriteBatch)
            : base(world.GetEntities().With<CPosition>().With<CRenderable>().AsSet())
        {
            sb = spriteBatch;
        }

        protected override void Update(float state, in Entity entity)
        {
            Texture2D texture = entity.Get<CRenderable>().Texture;
            CPosition pos = entity.Get<CPosition>();
            sb.Begin();
            sb.Draw(texture, new Vector2(pos.X, pos.Y), Color.White);
            sb.End();

            base.Update(state, entity);
        }
    }
}
