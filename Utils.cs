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
        
    }
}
