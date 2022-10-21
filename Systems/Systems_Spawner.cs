using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        private static Random random = new Random();
        private static Query query_Spawner = new Query().With<(BoxSpawner, Transform2D)>();

        public static void Spawner(World world)
        {
            var entities = world.Query(query_Spawner);

            foreach (var entity in entities)
            {
                var transform = entity.Get<Transform2D>();
                var spawner = entity.Get<BoxSpawner>();

                spawner.Timer += world.Delta* spawner.SpawnRate;

                if(spawner.Timer >= 1f)
                {
                    int id = random.Next(24);
                    spawner.Timer = 0;
                    Entity e = world.CreateEntity();
                    e.Add(new Sprite(2, id, int.MaxValue));
                    e.Add(new Transform2D(2f, 14.5f, 2f));
                    e.Add(new Box(id));
                    e.Add(new ConveyorMovable());
                    e.Add(new Trashable());
                    e.Add(new Collider2DBox(Vector2.One));
                }

                entity.Set(spawner);
            }

        }
    }
}
