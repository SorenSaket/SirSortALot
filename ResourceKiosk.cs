using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    struct Order
    {
        public int ID;
        public int Count;

        public Order(int iD, int count)
        {
            ID = iD;
            Count = count;
        }
    }
    internal class ResourceKiosk
    {
        public float difficulty = 1;
        public float timer = 0;
        public List<Order> orders = new List<Order>();

        public Random random = new Random();

        public void Update(World world)
        {
            timer -= world.Delta;

            if(orders.Count <= 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    orders.Add(new Order(random.Next(24), random.Next(1, 4)));
                }
            }
        }
    }
}
