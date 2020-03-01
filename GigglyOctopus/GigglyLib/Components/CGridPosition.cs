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
        public int X;
        public int Y;
        public Direction Facing;
    }
}
