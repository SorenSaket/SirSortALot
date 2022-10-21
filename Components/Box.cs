using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal struct Box
    {
        public int id;
        public float weight;

        public Box(int id, float weight = 1)
        {
            this.id = id;
            this.weight = weight;
        }
    }
}
