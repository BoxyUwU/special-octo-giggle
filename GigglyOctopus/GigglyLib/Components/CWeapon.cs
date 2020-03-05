using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GigglyLib.Game1;

namespace GigglyLib.Components
{
    public struct CWeapon
    {
        public int RangeLeft;
        public int RangeRight;
        public int RangeFront;
        public int RangeBack;

        public int AdditionalTargets;

        public int CooldownMax;
        public int Cooldown;

        public int Damage;

        public Colour Colour;
        public bool RandomColours;

        public List<string> AttackPattern;

    }
}
