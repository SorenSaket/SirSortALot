using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal struct ConveyorMovable
    {
        public byte Frozen;
        public ConveyorMovable()
        {
           Frozen = 0;
        }
        public ConveyorMovable(bool frozen)
        {
            if(frozen)
                Frozen = 1;
            else
                Frozen = 0;
        }
    }
}
