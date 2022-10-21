using Saket.ECS;
using Saket.Engine;
using SirSortALot.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        private static Query query_velocityTransform = new Query().With<(Velocity, Transform2D)>();

        public static void Velocity(World world)
        {
            var entities = world.Query(query_velocityTransform);

            foreach (var entity in entities)
            {
                var transform = entity.Get<Transform2D>();
                var velocity = entity.Get<Velocity>();

                transform.Position += velocity.value * world.Delta;

                entity.Set(transform);
            }
        }
    }
}
