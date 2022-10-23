using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static class Utils
    {
        public static Vector2 RectNormal(Vector2 v)
        {
            if (MathF.Abs(v.X) > MathF.Abs(v.Y))
                return new Vector2(MathF.Sign(v.X), 0);
            return new Vector2(0, MathF.Sign(v.Y));
        }
        


        /// <summary>
        /// Subtracts negation from value by no more that values original size. Meaning the components will never loop around 0.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="negation"></param>
        /// <returns></returns>
        public static Vector2 Negate(this Vector2 value, Vector2 negation)
        {
            float x = value.X - negation.X;
            if (MathF.Sign(value.X) > 0f)
            {
                x = MathF.Max(x, 0f);
            }
            else if (MathF.Sign(value.X) < 0f)
            {
                x = MathF.Min(x, 0f);
            }
            else
            {
                x = 0f;
            }

            float y = value.Y - negation.Y;

            if (MathF.Sign(value.Y) > 0f)
            {
                y = MathF.Max(y, 0f);
            }
            else if(MathF.Sign(value.Y) < 0f)
            {
                y = MathF.Min(y, 0f);
            }
            else
            {
                y = 0f;
            }

            return new Vector2(x, y);
        }

    }
}
