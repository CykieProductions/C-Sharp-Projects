using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CysmicEngine
{
    public struct Vector2
    {
        public float x, y;

        public Vector2(float xv = 0, float yv = 0)
        {
            x = xv;
            y = yv;
        }

        public static float Distance(Vector2 pnt1, Vector2 pnt2)
        {
            return (float)Math.Sqrt(Math.Pow(pnt2.x - pnt1.x, 2) + Math.Pow(pnt2.y - pnt1.y, 2));
        }

        /// <summary>
        /// A Vector2 of (0, 0)
        /// </summary>
        /// <returns></returns>
        public static readonly Vector2 Zero = (0, 0);

        public static implicit operator Vector2((float, float) tuple)
        {
            Vector2 result = new Vector2();
            result.x = tuple.Item1;
            result.y = tuple.Item2;

            return result;
        }
        /*public static implicit operator Vector2(bool isNegInf)
        {
            Vector2 result = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            if(!isNegInf)
                result = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            return result;
        }*/

        public static implicit operator string(Vector2 vector2)
        {
            return "(" + vector2.x +"," + vector2.y + ")";
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }
    }
}
