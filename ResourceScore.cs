using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal class ResourceScore
    {
        public int Score { get; set; }

        public List<ECSPointer> letters = new();

        public void Update(World world)
        {
            
        }
    }
}
