using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal struct BoxSpawner
    {
        public float Timer;
        public float SpawnRate;

        public BoxSpawner(float rate)
        {
            Timer = 0f;
            SpawnRate = rate;
        }
    }
}
