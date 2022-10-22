using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal struct MoveTowards
    {
        public Vector2 Target;
        public float speed;

        public MoveTowards(Vector2 target, float speed = 1f)
        {
            Target = target;
            this.speed = speed;
        }
    }
}
