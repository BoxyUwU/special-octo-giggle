using System;
namespace GigglyLib.Components
{

    public struct CTargetAnim
    {
        public enum Type
        {
            PLAYER,
            WARNING,
            DANGER
        }
        public Type TargetType;
        public bool FadingOut;
        public bool GoneVisible;
    }
}
