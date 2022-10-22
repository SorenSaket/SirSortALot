using Saket.ECS;
using Saket.Engine;
using SirSortALot.Components;
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
        private static Query query_movetowardsTransform = new Query().With<(MoveTowards, Transform2D)>();

        public static void MoveTowards(World world)
        {
            var entities = world.Query(query_movetowardsTransform);

            foreach (var entity in entities)
            {
                var transform = entity.Get<Transform2D>();
                var move = entity.Get<MoveTowards>();

                transform.Position = Vector2.Lerp(transform.Position, move.Target, move.speed * world.Delta);

                entity.Set(transform);
            }
        }
    }
}
