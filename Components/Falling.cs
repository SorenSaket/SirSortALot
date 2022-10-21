using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal struct Falling
    {
        public float timer;
        public Transform2D baseTransform;

        public Falling(Transform2D baseTransform)
        {
            this.timer = 0;
            this.baseTransform = baseTransform;
        }
    }
}
