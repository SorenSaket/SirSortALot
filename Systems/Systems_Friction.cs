using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SirSortALot
{
    internal static partial class Systems
    {
        private static Query query_frictionVelocity = new Query().With<(Velocity, Friction)>();

        public static void Friction(World world)
        {
            var entities = world.Query(query_frictionVelocity);

            foreach (var entity in entities)
            {
                var friction = entity.Get<Friction>();
                var velocity = entity.Get<Velocity>();

                velocity.value *= 1f - (friction.friction*world.Delta);

                entity.Set(velocity);
            }
        }
    }
}