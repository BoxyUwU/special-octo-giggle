using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigglyLib.Components
{
    public struct CWeaponsArray
    {
        public List<CWeapon> Weapons;

        public CWeaponsArray(CWeaponsArray wepArray)
        {
            Weapons = new List<CWeapon>();
            if (wepArray.Weapons != null)
            {
                for (int i = 0; i < wepArray.Weapons.Count; i++)
                {
                    Weapons.Add(wepArray.Weapons[i]);
                }
            }
        }
    }
}
