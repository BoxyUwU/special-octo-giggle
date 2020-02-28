using System;

namespace GigglyLib.Components
{
    public enum Direction
    {
        NORTH,
        EAST,
        SOUTH,
        WEST,
    }

    public struct CGridPosition
    {
        public float X;
        public float Y;
        public Direction Facing;
    }
}
