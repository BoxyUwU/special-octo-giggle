using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GigglyLib.Game1;

namespace GigglyLib.Components
{
    struct CParticleBeam
    {
        public float SourceX;
        public float SourceY;
        public float DestX;
        public float DestY;
        public float X;
        public float Y;
        public int Frame;
        public Colour Colour;
    }
}
