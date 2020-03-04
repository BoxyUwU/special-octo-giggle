using System;
using Microsoft.Xna.Framework;

namespace GigglyLib.ProcGen
{
    public static class VectorExtensions
    {
        public static Vector2 RotateBy(this Vector2 vector, float radians)
        {
            vector = Vector2.Transform(vector, Matrix.CreateRotationZ(radians));
            return vector;
        }
    }
}
