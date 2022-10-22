using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal struct Trashable
    {
        public bool IsTashable => tashable != 0;

        private byte tashable;

        public Trashable()
        {
            tashable = 1;
        }

        public Trashable(bool tashable)
        {
            if (tashable)
                this.tashable = 1;
            else
                this.tashable = 0;
        }
    }
}
